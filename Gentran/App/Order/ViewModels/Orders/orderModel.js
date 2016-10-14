orderModule.controller("orderViewModel", function ($sce, $scope, orderService, $http, $q, $routeParams, $window, $location, viewModelHelper, $timeout) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.orderService = orderService;

    var initialize = function () {
        $scope.refreshOrders();
    }

    $scope.refreshOrders = function () {
        $scope.ifEdit = true;
        viewModelHelper.apiGet('api/order', null, function (result) {
            console.log(result.data.detail);
            $scope.order = result.data.detail.filter(x=>x.uiprice = parseInt(x.uiprice).toLocaleString());
        });
    }

    $scope.showDetails = function (order) {
        $scope.flags.shownFromList = true;
        
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

        viewModelHelper.apiGet('api/order/' + order.ulid, null, function (result) {
            console.log(result.data.detail);
            $scope.orderItems = result.data.detail.filter(x=>x.UITPrice = parseInt(x.UITPrice).toLocaleString());
            $scope.orderItems = result.data.detail.filter(x=>x.UIPrice = parseInt(x.UIPrice).toLocaleString());
        });

        $('#orderDetailsModal').modal('show');
    }

    $scope.submitOrder = function () {

        var selected = false;
        var ifNew = true;
        var ctr = 0;
        var listOrder = [];

        $('.cbxSub').each(function () {
            if (this.checked == true) {
                var status = $(this).closest('tr').find('.table-data-ustat').text();
                if (status == "Read Failed") {
                    ifNew = false;
                } else{
                    selected = true;

                    listOrder[ctr] = {};

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
                    notif_info("Orders", "Select order to submit");
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
                notif_info("Orders", "Select order to Delete");
            }
        });
    }

    $scope.deleteYes = function (toDelete) {
        
        var items = {};
        var listDelete = [];

        for (var x = 0, y = toDelete.length; x < y; x++) {
            listDelete[x] = {};
            listDelete[x].ponumber = toDelete[x].PONum;
            listDelete[x].customernumber = toDelete[x].CustNum;
        }
        items.payload = listDelete;
        viewModelHelper.apiPost('api/deleteorder', items, function (result) {
            var data = result.data;
            console.log(data);

            if (data.success === true) {
                $('#deleteModal').modal('hide');
                $scope.refreshOrders();
                notif_success('Deleted', data.detail);
            } else {
                notif_error('Error!', data.detail)
            }
        });
        
        console.log(toDelete);
    }

    $scope.deleteThis = function (deleteID) {

        var id = {};
        var items = {};
        id.ULId = deleteID;
        items.payload = _payloadParser(id);

        viewModelHelper.apiPut('api/deleteorder', items, function (result) {
            var data = result.data;
            console.log(data);

            if (data.success === true) {
                $scope.refreshOrders();
                $('#orderDetailsModal').modal('hide');
                notif_success('Deleted', data.detail);
            } else {
                notif_error('Error!', data.detail)
            }
        });
        console.log(deleteID);
    }

    /*$scope.delItem = function (item, $event) {
        console.log($($(event.target))[0].lastChild.className);
    }*/
    
    $scope.getStatus = function (id) {
        var datas = id.ulid + "," + id.ulstatus;

        if(parseInt(id.ulstatus) < 20){
            viewModelHelper.apiGet('api/ordererrors/' + datas, null, function (result) {
                //READY FOR BALLOON RESPONSE    
                if (result.data.success == true) {
                    
                    var productErrorDetail = "";
                    var storeErrorDetail = "";

                    for (var i = 0; i < result.data.detail.length; i++) {
                        var error = "";
                        if (result.data.detail[i].ELType == 'Invalid Product') {
                            if (productErrorDetail == "") {
                                productErrorDetail = result.data.detail[i].ELDetail;
                            } else {
                                productErrorDetail += ", " + result.data.detail[i].ELDetail;
                            }
                        }
                        else if (result.data.detail[i].ELType == 'Invalid Customer') {
                            if (storeErrorDetail == "") {
                                storeErrorDetail = result.data.detail[i].ELDetail;
                            } else {
                                storeErrorDetail += ", " + result.data.detail[i].ELDetail;
                            }
                        }
                        else {
                            error += result.data.detail[i].ELDetail + "\n";
                            notif_error("", error);
                        }

                    }
                    if(productErrorDetail!="")
                        notif_error("Invalid Product Code", productErrorDetail);
                    if(storeErrorDetail!="")
                        notif_error("Invalid Store Code", storeErrorDetail);
                } else {
                    notif_error("Error Detail", result.data.detail);
                }
            });
        }
        else {
            notif_success("New order","Successful");
        }
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
                Item.ulstatus = $scope.ordersInfo.txt_status;;
                Item.ULId = $scope.ordersInfo.txt_orderid;

                $scope.data = {};
                $scope.data.operation = "edit_po";
                $scope.data.payload = _payloadParser(Item);

                viewModelHelper.apiPut('api/order', $scope.data, function (result) {
                    var data = result.data;
                    console.log(data);

                    if (data.success === true) {
                        notif_success('PO Updated', changes);
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

    initialize();
});