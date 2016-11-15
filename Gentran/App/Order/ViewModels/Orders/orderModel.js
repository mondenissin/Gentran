orderModule.controller("orderViewModel", function ($sce, $scope, orderService, $http, $q, $routeParams, $window, $location, viewModelHelper, $timeout, DTOptionsBuilder, DTColumnDefBuilder, filterFilter) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.orderService = orderService;

    var initialize = function () {
        $scope.refreshOrders();
    }

    $scope.refreshOrders = function () {
        $scope.search = {};
        $scope.searchBy = "ulponumber";
        $scope.ifEdit = true;

        viewModelHelper.apiGet('api/order', null, function (result) {
            console.log(result.data.detail);
            $scope.pageSize = 5;
            $scope.entryLimit = 50;

            var orders = result.data.detail.filter(x=>x.uiprice = parseInt(x.uiprice).toLocaleString());
            orders.filter(x=> {
                var data = x.ulid+','+x.ulstatus;
                if (x.ulstatus < 20) {
                    viewModelHelper.apiGet('api/ordererrors/' + data, null, function (result) { 
                        x.eCtr = result.data.detail.length;
                    });
                }
            });

            console.log(orders);
            $scope.order = result.data.detail;

            $scope.$watch('search[searchBy]', function () {
                $scope.filterList = filterFilter($scope.order, $scope.search);
                $scope.noOfPages = Math.ceil($scope.filterList.length / $scope.entryLimit);
                $scope.currentPage = 1;
            });
        });

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('order', [7, 'desc']);
        $scope.dtColumnDefs = [
           DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
        ];
    }
    
    $scope.showFile = function(data){
        console.log(data.ulstatus);
        console.log(data.rffilename);
    
        var temponame = data.rffilename;
        var temp1 = temponame.split('.');
        var temp2 = temp1[(temp1.length - 1)];
        var fileLoc = "";
        
        fileLoc = data.ulstatus == '11' ? 'Gentran/failed/' : 'Gentran/successful/';
        
        viewModelHelper.fileViewer(temp2, data.rffilename, fileLoc);
    }
    $scope.showDetails = function (order) {
        $scope.flags.shownFromList = true;
        console.log(order);
        if (order.uiprice == "NaN" || order.uiprice == 0) {
            notif_warning('Order Details','Lack of Product Mapping');
        } else {

            if (order.ulstatus == "20") {
                $scope.ifEditable = true;
            } else {
                $scope.ifEditable = false;
            }
            var orderDetails = [];
            orderDetails[0] = {};
            orderDetails.txt_ponum = order.ulponumber;
            orderDetails.txt_custnum = order.ulcustomer;
            orderDetails.txt_orderdate = new Date(order.ulorderdate);
            orderDetails.txt_deliverydate = new Date(order.uldeliverydate);
            orderDetails.txt_remarks = order.ulremarks;
            orderDetails.txt_status = order.ulstatus;
            orderDetails.txt_orderid = order.ulid;
            $scope.ordersInfo = orderDetails;
            console.log(order);
            viewModelHelper.apiGet('api/order/' + order.ulid, null, function (result) {
                console.log(result.data.detail);
                $scope.orderItems = result.data.detail.filter(x=>x.UITPrice = parseInt(x.UITPrice).toLocaleString());
                $scope.orderItems = result.data.detail.filter(x=>x.UIPrice = parseInt(x.UIPrice).toLocaleString());
            });

            $('#orderDetailsModal').modal('show');
        }
    }


    $scope.reuploadPO = function (order) {
        $('.progress-bar').css('width', '0.9%');
        $('.progress-bar').text("Reading");
        $scope.reupRes = null;
        $scope.reupPO = order;
        var items = {};
        var obj = [];
        obj[0] = {};
        obj[0].fileName = "C:\\inetpub\\wwwroot\\files\\Gentran\\failed\\" + order.rffilename;
        obj[0].outlet = order.rfaccount;
        obj[0].ULId = order.ulid;
        obj[0].rawID = order.rfid;
        items.payload = obj;
        
        $('#reupModal').modal('show');
        console.log(order);
        viewModelHelper.apiPost('api/masteruploader', JSON.stringify(items), function (result) {
            var mapData = result.data.filecontent;
            execTime = result.data.execution;
            //console.log(mapData);
            console.log(execTime);

            $('#reupMdal').modal('show');

            console.log(order);
            viewModelHelper.apiGet('api/ordererrors/' + order.ulid + ',' + order.ulstatus, null, function (resultError) {
                console.log(resultError.data.detail);
                $('.progress-bar').animate({ width: '100%' }, execTime);

                setTimeout(function () {

                    $scope.reupRes = resultError.data.detail;
                    console.log($scope.reupRes);

                    $('.progress-bar').text("Reupload Completed!");
                    $('#progress-bar').removeClass("active");
                    viewModelHelper.saveTransaction(result.data.detail);

                    $scope.refreshOrders();
                }, execTime);
            });
        });
    }

    $scope.submitOrder = function () {

        var selected = false;
        var ifNew = true;
        var ctr = 0;
        var listOrder = [];

        $('.cbxSub').each(function () {
            if (this.checked == true) {
                var status = $(this).closest('tr').find('.table-data-ustat').text().trim();

                if (status == "Read Failed") {
                    ifNew = false;
                } else{
                    selected = true;

                    listOrder[ctr] = {};
                    
                    listOrder[ctr].ULId = $(this).closest('tr').find('.table-data-ulid').text();
                    listOrder[ctr].PONum = $(this).closest('tr').find('.table-data-ponum').text();
                    listOrder[ctr].CustNum = $(this).closest('tr').find('.table-data-ucust').text();
                    listOrder[ctr].CustName = $(this).closest('tr').find('.table-data-cdesc').text();
                    listOrder[ctr].OrderDate = $(this).closest('tr').find('.table-data-uodate').text();
                    listOrder[ctr].Quantity = $(this).closest('tr').find('.table-data-sqty').text();
                    ctr++;
                }
            }
        }).promise().done(function () {
            $scope.submitQueue = listOrder;

            if (!(ifNew)) {
                notif_warning("Warning", "Only New PO's can be submit");
            } else {
                if (selected) {
                    $('#submitModal').modal('show');
                } else {
                    notif_warning("Orders", "Select order to submit");
                }
            }
        });
    }

    $scope.submittingOrders = function (queued) {
        console.log(queued);
    }

    $scope.deleteOrder = function () {
        var ctr = 0;
        var listDel = [];
        var selected = false;

        $('.cbxSub').each(function () {
            if (this.checked == true) {
                selected = true;
                listDel[ctr] = {};
                listDel[ctr].ULId = $(this).closest('tr').find('.table-data-ulid').text();
                listDel[ctr].PONum = $(this).closest('tr').find('.table-data-ponum').text();
                listDel[ctr].CustNum = $(this).closest('tr').find('.table-data-ucust').text();
                listDel[ctr].CustName = $(this).closest('tr').find('.table-data-cdesc').text();
                listDel[ctr].OrderDate = $(this).closest('tr').find('.table-data-uodate').text();

                ctr++;
            }
        }).promise().done(function () {
            $scope.deleteQueue = listDel;

            if (selected) {
                $('#deleteModal').modal('show');
            } else {
                notif_warning("Orders", "Select order to Delete");
            }
        });
    }

    $scope.deleteYes = function (toDelete) {
        console.log(toDelete);
        var items = {};
        var listDelete = [];

        for (var x = 0, y = toDelete.length; x < y; x++) {
            listDelete[x] = {};
            listDelete[x].ponumber = toDelete[x].PONum;
            listDelete[x].customernumber = toDelete[x].CustNum;
            listDelete[x].ULId = toDelete[x].ULId;
        }
        items.payload = listDelete;
        viewModelHelper.apiPost('api/deleteorder', items, function (result) {
            var data = result.data;
            console.log(data);

            if (data.success === true) {
                $('#deleteModal').modal('hide');
                $scope.refreshOrders();

                viewModelHelper.saveTransaction(data.detail);

                notif_success('Deleted', data.notiftext);
            } else {
                notif_error('Error!', data.detail)
            }
        });
        
        console.log(toDelete);
    }

    $scope.deleteThis = function (deleteID) {

        var id = {};
        var items = {};
        id.rawID = deleteID;
        items.payload = _payloadParser(id);

        viewModelHelper.apiPut('api/deleteorder', items, function (result) {
            var data = result.data;
            console.log(data);

            if (data.success === true) {
                $scope.refreshOrders();
                $('#orderDetailsModal').modal('hide');

                viewModelHelper.saveTransaction(data.detail);

                notif_success('Deleted', data.notiftext);
            } else {
                notif_error('Error!', data.detail)
            }
        });
        console.log(deleteID);
    }

    /*$scope.delItem = function (item, $event) {
        console.log($($(event.target))[0].lastChild.className);
    }*/
    
    $scope.getStatus = function (id, $event) {
        var datas = id.ulid + ',' + id.ulstatus;
        $scope.errorPO = "";
        if(parseInt(id.ulstatus) < 20){
            viewModelHelper.apiGet('api/ordererrors/' + datas, null, function (result) {
                //READY FOR BALLOON RESPONSE    
                if (result.data.success == true) {
                    $scope.errorNotifCtr = result.data.detail.length;
                    var productErrorDetail = "";
                    var storeErrorDetail = "";
                    var error = "";
                    for (var i = 0; i < result.data.detail.length; i++) {
                        if (result.data.detail[i].ELType == 'Invalid Product') {
                            if (productErrorDetail == "") {
                                productErrorDetail = "Invalid Product Code: " + result.data.detail[i].ELDetail + ",";
                            } else {
                                productErrorDetail += "Invalid Product Code: " + result.data.detail[i].ELDetail + ",";
                            }
                        }
                        else if (result.data.detail[i].ELType == 'Invalid Customer') {
                            if (storeErrorDetail == "") {
                                storeErrorDetail = "Invalid Store Code: " + result.data.detail[i].ELDetail + ",";
                            } else {
                                storeErrorDetail += "Invalid Store Code: " + result.data.detail[i].ELDetail + ",";
                            }
                        }
                        else {
                            error += result.data.detail[i].ELDetail + ",";
                            $scope.errorPO += error;
                        }

                    }
                    if (productErrorDetail != "") {
                        error += productErrorDetail;
                    }
                    if (storeErrorDetail != "") {
                        error += storeErrorDetail;
                    }

                    var eSplit = error.split(',');
                    $scope.errorPO = [];
                    for (var i = 0, j = eSplit.length;i<j-1;i++){
                        $scope.errorPO[i] = {};
                        $scope.errorPO[i].errorDet = i + 1 + '. ' + eSplit[i];
                    }
                    //showOverlay();
                } else {
                    $scope.errorPO += result.data.detail;
                }
            });

            $('#ErrorModal').modal('show');
        }
        else {
            notif_success("New order","Successful");
        }

        //function showOverlay() {
        //    var row = $($($($event.target)[0].offsetParent)[0].firstElementChild);
        //    row.toggle();
        //    $('.overlayBtn').focusout(function () {
        //        row.hide();
        //    });
        //}
    }

    var cust = "";
    var delDate = "";
    var remarks = "";

    $scope.editDetails = function ($event) {
        var changes = "";

        if ($($($event.target)[0]).text() == "Edit") {
            cust = $scope.ordersInfo.txt_custnum;
            delDate = $scope.ordersInfo.txt_deliverydate;
            remarks = $scope.ordersInfo.txt_remarks;

            //$scope.ifEdit = false;
            $($($event.target)[0]).text("Save")
            $('#txt_remarks')[0].disabled = false;
            $('#txt_deliverydate')[0].disabled = false;
            $('#txt_custnum')[0].disabled = false;

        } else {
            var newCust = $scope.ordersInfo.txt_custnum;
            var newDelDate = $scope.ordersInfo.txt_deliverydate;
            var newRemarks = $scope.ordersInfo.txt_remarks;

            if(cust == newCust && delDate == newDelDate && remarks == newRemarks){
                notif_info("Information","No data has been changed");
            } else {
                var Item = {};

                if(cust != newCust)
                    changes += "Customer Code: " + cust + " - " + newCust + "\n";
                if(delDate != newDelDate)
                    changes += "Delivery Date: " + delDate.toString().slice(0, 10) + " - " + newDelDate.toString().slice(0, 10) + "\n";
                if (remarks != newRemarks)
                    changes += "Remarks: " + remarks + " - " + newRemarks + "\n";

                Item.customernumber = newCust;
                Item.DeliveryDate = (newDelDate.getMonth() + 1) + "/" + newDelDate.getDate() + "/" + newDelDate.getFullYear();
                Item.remarks = newRemarks;
                Item.ponumber = $scope.ordersInfo.txt_ponum;
                Item.ulstatus = $scope.ordersInfo.txt_status;
                Item.ULId = $scope.ordersInfo.txt_orderid;
                Item.changes = changes;

                $scope.data = {};
                $scope.data.operation = "edit_po";
                $scope.data.payload = _payloadParser(Item);

                viewModelHelper.apiPut('api/order', $scope.data, function (result) {
                    var data = result.data;
                    console.log(data);

                    if (data.success === true) {
                        notif_success('PO Updated', changes);

                        viewModelHelper.saveTransaction(data.detail);

                        $scope.refreshOrders();
                    } else {
                        notif_error('Error!', data.detail)
                    }
                });
            }
            //$scope.ifEdit = true;
            $($($event.target)[0]).text("Edit")
            $('#txt_remarks')[0].disabled = true;
            $('#txt_deliverydate')[0].disabled = true;
            $('#txt_custnum')[0].disabled = true;
        }
    }

    $scope.clearSearch = function () {
        $scope.search = {};
    }

    initialize();
}).filter('start', function () {
    return function (input, start) {
        if (!input || !input.length) { return; }

        start = +start;
        return input.slice(start);
    };
});