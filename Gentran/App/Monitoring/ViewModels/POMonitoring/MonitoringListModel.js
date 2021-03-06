﻿monitoringModule.controller("monitoringViewModel", function ($scope, monitoringService, $http, $q, $routeParams, $window, $location, viewModelHelper, DTOptionsBuilder, DTColumnDefBuilder, filterFilter) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.monitoringService = monitoringService;
    $scope.custIfCorrect = false;

    var initialize = function () {
        $scope.refreshMonitor();
        //$scope.refreshTransact();
        $scope.getAccess();
    }

    $scope.getAccess = function () {
        viewModelHelper.apiGet('api/master', null, function (result) {
            console.log(result);
            $scope.UserAccess = result.data;
        });
    }

    $scope.refreshMonitor = function () {

        $scope.searchPO = {};
        $scope.searchByPO = "ulponumber";

        viewModelHelper.apiGet('api/monitor', null, function (result) {
            $scope.pageSize = 5;
            $scope.entryLimit = 50;

            var orders = result.data.detail.filter(x=>x.uiprice = parseInt(x.uiprice).toLocaleString());
            orders.filter(x=> {
                var data = x.ulid + ',' + x.ulstatus;
                if (x.ulstatus < 20) {
                    viewModelHelper.apiGet('api/monitorerrors/' + data, null, function (result) {
                        x.eCtr = result.data.detail.length;
                    });
                }
            });

            $scope.monitorPO = result.data.detail;
            console.log(result.data.detail);
            $scope.$watch('searchPO[searchByPO]', function () {
                $scope.filterMonitor = filterFilter($scope.monitorPO, $scope.searchPO);
                $scope.noOfPages = Math.ceil($scope.filterMonitor.length / $scope.entryLimit);
                $scope.currentPage = 1;
            });
        });

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('order', [7, 'desc']);
        $scope.dtColumnDefs = [
           DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
        ];
    }

    //<!---- MNS20161208
    $scope.AutoCompleteCust = function (x, ctr) {
        $('#txt_' + ctr).autocomplete({
            source: function (request, response) {
                var Search = {};
                Search.prefix = request.term;
                Search.acctype = $('#txt_account_det').val();

                $scope.data = {};
                $scope.data.operation = 'suggestions';
                $scope.data.payload = _payloadParser(Search);

                viewModelHelper.apiGet('api/cust', $scope.data, function (result) {
                    var data = result.data;
                    if (data.detail[0].Data == "No Data Found") {
                        response($.map(data.detail, function (item) {
                            return {
                                label: '<b>No Data Found</b>',
                                value: request.term
                            }
                        }))
                    } else {
                        response($.map(data.detail, function (item) {
                            return {
                                label: '<b>' + item.CMId + '</b> <span>' + item.CMDescription + '</span>',
                                value: item.CMId
                            }
                        }))
                    }
                });

            },
            select: function (e, i) {
                $('#txt_' + ctr).val(i.item.value);
            },
            minLength: 1
        });
    }

    $scope.AutoComplete = function (x, ctr) {
        $('#txt_' + ctr).autocomplete({
            source: function (request, response) {
                var Search = {};
                Search.prefix = request.term;
                Search.acctype = $('#txt_account_det').val();

                $scope.data = {};
                $scope.data.operation = 'suggestions';
                $scope.data.payload = _payloadParser(Search);

                viewModelHelper.apiGet('api/prod', $scope.data, function (result) {
                    var data = result.data;
                    response($.map(data.detail, function (item) {
                        return {
                            label: '<b>' + item.PMCode + '</b> <span>' + item.PMDescription + '</span>',
                            value: item.PMCode
                        }
                    }))
                });

            },
            select: function (e, i) {
                $('#txt_' + ctr).val(i.item.value);
            },
            minLength: 1
        });
    }

    $scope.textboxFocus = function (x, ctr) {
        $('#txt_' + ctr).removeClass('prod-valid');
        $('#txt_' + ctr).removeClass('prod-invalid');
        $('#txt_' + ctr).removeClass('cust-valid');
        $('#txt_' + ctr).removeClass('cust-invalid');
    }
    $scope.textboxBlurCust = function (x, ctr) {
        var custText = $('#txt_' + ctr);
        if (custText.val().trim() != '0' || custText.val().trim() != '') {

            var Item = {};
            Item.customernumber = $('#txt_' + ctr).val();
            Item.acctype = $('#txt_account_det').val();

            $scope.data = {};
            $scope.data.operation = 'check_map_availability';
            $scope.data.payload = _payloadParser(Item);

            viewModelHelper.apiGet('api/cust', $scope.data, function (result) {
                var data = result.data;
                console.log(data);
                if (data.success) {
                    if (data.detail[0].CMId == '0') {
                        $scope.custIfCorrect = false;
                        $('#txt_' + ctr).addClass('cust-invalid');
                        $('#txt_' + ctr).removeClass('cust-valid');
                        $('#txt_' + ctr).attr('title', 'NO CUSTOMER');
                    } else {
                        $scope.custIfCorrect = true;
                        $('#txt_' + ctr).addClass('cust-valid');
                        $('#txt_' + ctr).removeClass('cust-invalid');
                        $('#txt_' + ctr).attr('title', data.detail[0].CMDescription);
                    }
                } else {
                    $scope.custIfCorrect = false;
                    $('#txt_' + ctr).addClass('cust-invalid');
                    $('#txt_' + ctr).removeClass('cust-valid');
                    $('#txt_' + ctr).attr('title', data.detail[0].Data);
                }
            });
        } else {
            $scope.custIfCorrect = false;
            $('#txt_' + ctr).addClass('cust-invalid');
            $('#txt_' + ctr).removeClass('cust-valid');
            $('#txt_' + ctr).attr('title', '');
        }
    }
    $scope.textboxBlur = function (x, ctr) {

        if ($('#txt_' + ctr).val().trim() != '') {

            var Item = {};
            Item.pmcode = $('#txt_' + ctr).val();
            Item.pacode = $('#txt_' + ctr).closest('tr').find('.table-data-spcode').text();
            Item.acctype = $('#txt_account_det').val();

            $scope.data = {};
            $scope.data.operation = 'check_map_availability';
            $scope.data.payload = _payloadParser(Item);

            viewModelHelper.apiGet('api/prod', $scope.data, function (result) {
                var data = result.data;
                if (data.success) {
                    $('#txt_' + ctr).addClass('prod-valid');
                    $('#txt_' + ctr).removeClass('prod-invalid');
                    $('#txt_' + ctr).attr('title', '');
                } else {
                    $('#txt_' + ctr).addClass('prod-invalid');
                    $('#txt_' + ctr).removeClass('prod-valid');
                    $('#txt_' + ctr).attr('title', data.detail[0].response);
                }
            });
        } else {
            $('#txt_' + ctr).addClass('prod-invalid');
            $('#txt_' + ctr).removeClass('prod-valid');
            $('#txt_' + ctr).attr('title', '');
        }
    }
    // --- MNS20161208 --->

    $scope.customerMapping = function (cust, assign) {
        var customer = $('#' + cust).val();
        var assigned = $('#' + assign).val();

        if (customer == '' || assigned == '') {
            notif_warning('Customer Mapping', 'Please fill all the fields.');
        }
        else if ($scope.custIfCorrect == false) {
            notif_warning('Customer Mapping', 'Customer Number Not Found');
        }
        else {
            var Item = {};
            Item.customernumber = customer;
            Item.assignedcode = assigned;
            Item.ponumber = $('#txt_ponum').val();
            Item.ULId = $('#txt_id').val();
            Item.acctype = $('#txt_account_det').val();
            $scope.data = {};
            $scope.data.operation = 'customer_mapping';
            $scope.data.payload = _payloadParser(Item);
            
            viewModelHelper.apiPut('api/cust', $scope.data, function (result) {
                var data = result.data;

                if (data.success) {
                    console.log(data);
                    $scope.refreshMonitor();
                    viewModelHelper.saveTransaction(data.detail);
                    notif_info('Success', 'Successfully Assigned');
                    $('#orderCustDetailsModal').modal('hide');

                } else {
                    console.log(data);
                    $scope.refreshMonitor();
                    viewModelHelper.saveTransaction(data.detail);
                    notif_warning('Unsuccess', data.detail[0].remarks);
                    $('#orderCustDetailsModal').modal('hide');
                }
            });
        }
    }

    //<!---- MNS20161209

    $scope.validateMapping = function () {
        var Items = [];
        var ctr = 0;         
        var iDuplicates = 0;
        var iBlank = 0;
        $('.table-data-pmcode').each(function () {
            element = this.closest('tr');
            Items[ctr] = {};
            Items[ctr].pacode = $(element).find('.table-data-spcode').text().trim();
            Items[ctr].pmcode = $(element).find('.table-data-pmcode').val().trim();
            Items[ctr].acctype = $('#txt_account_det').val().trim();
            Items[ctr].ULId = $('#txt_id').val().trim();
            ctr++;
        });

        for (var x = 0; x < Items.length; x++) {
            if (Items[x].pmcode == '') {
                iBlank++;
            } else {
                for (var y = x + 1; y < Items.length; y++) {
                    if (Items[x].pmcode == Items[y].pmcode) {
                        iDuplicates++;
                    }
                }
            }
        }

        if (iBlank > 0) {
            notif_warning('Order Details', 'Please fill all required fields.');
        }
        else if (iDuplicates > 0) {
            notif_warning('Order Details', 'Duplicate SKU input.');
        }
        else {
            $scope.updateMapping(Items);
        }
        
    }

    $scope.updateMapping = function (Item) {
        $scope.Items = Item;

        $scope.data = {};
        $scope.data.operation = "update_order_map";
        $scope.data.payload = _payloadParser($scope.Items);


        viewModelHelper.apiPost('api/monitor', $scope.data, function (result) {
            var data = result.data;
            if (data.success) {
                for (var x = 0; x < data.detail.length; x++) {
                    if ( data.detail[x].status){
                        notif_success('Order Details', data.detail[x].response);
                    }
                    else{
                        notif_warning('Order Details', data.detail[x].response);
                    }
                    $scope.refreshMonitor();
                    $('#orderDetailsModal').modal('hide');
                }
            } else {
                notif_warning('Order Details', data.detail[0].response);
            }
        });
    }

    //----------GMC 20161219 FOR REUPLOADING -------------> 

    $scope.reUpload = function (order) {
        console.log(order);

        var items = {};
        var obj = [];
        var custNum = order.txt_custnum;
        console.log(order.txt_custnum);
        if (custNum == $('#txt_custnum').text()) {
            notif_warning('Order Details', 'Nothings Change');
            return;
        }


        obj[0] = {};
        obj[0].outlet = order.txt_account;
        obj[0].ULId = order.txt_orderid;
        obj[0].rawID = order.txt_rawID;
        items.payload = obj;

        console.log(items);
        /*
        viewModelHelper.apiPost('api/masteruploader', JSON.stringify(items), function (result) {
            var mapData = result.data.filecontent;
            execTime = result.data.execution;
            //console.log(mapData);
            console.log(execTime);
            console.log(result);
           
        });
        */
    }


    // --- MNS20161209 --->



    //$scope.refreshTransact = function () {

    //    $scope.searchTransaction = {};
    //    $scope.searchByTransaction = "TLValue";

    //    $scope.data = {};
    //    $scope.data.operation = 'transaction';
        
    //    viewModelHelper.apiGet('api/monitor', $scope.data, function (result) {
    //        $scope.pageSizeT = 5;
    //        $scope.entryLimitT = 50;

    //        $scope.transact = result.data.detail;
    //        console.log(result.data.detail);
    //        $scope.$watch('searchTransaction[searchByTransaction]', function () {
    //            $scope.filterTransaction = filterFilter($scope.transact, $scope.searchTransaction);
    //            $scope.noOfPagesT = Math.ceil($scope.filterTransaction.length / $scope.entryLimitT);
    //            $scope.currentPageT = 1;
    //        });
    //    });

    //    $scope.dtOptionsTrans = DTOptionsBuilder.newOptions().withOption('order', [0, 'desc']);
    //    $scope.dtColumnDefsTrans = [
    //       DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
    //    ];
    //}

    //$scope.showDetails = function (monitoring) {
    //   // $scope.ResetEditFields();
    //    $('#txt_ponum').text(monitoring.ULPONumber == null || monitoring.ULPONumber == '' ? 'N/A' : monitoring.ULPONumber);
    //    $('#txt_custnum').text(monitoring.ULCustomer == null || monitoring.ULCustomer == '' ? 'N/A' : monitoring.ULCustomer);
    //    $('#txt_custname').text(monitoring.CMDescription == null || monitoring.CMDescription == '' ? 'N/A' : monitoring.CMDescription);
    //    $('#txt_orderdate').text(monitoring.ULOrderDate == null || monitoring.ULOrderDate == '' ? 'N/A' : monitoring.ULOrderDate);
    //    $('#txt_deliverydate').text(monitoring.ULDeliveryDate == null || monitoring.ULDeliveryDate == '' ? 'N/A' : monitoring.ULDeliveryDate);
    //    $('#txt_canceldate').text(monitoring.ULCancelDate == null || monitoring.ULCancelDate == '' ? 'N/A' : monitoring.ULCancelDate);
    //    $('#txt_retrievedate').text(monitoring.RFRetrieveDate);
    //    $('#txt_readdate').text(monitoring.ULReadDate == null || monitoring.ULReadDate == '' ? 'N/A' : monitoring.ULReadDate);
    //    $('#txt_submitteddate').text(monitoring.ULSubmitDate == null || monitoring.ULSubmitDate == '' ? 'N/A' : monitoring.ULSubmitDate);
    //    $('#txt_reader').text(monitoring.ULReadUser == null || monitoring.ULReadUser == '' ? 'N/A' : monitoring.ULReadUser);
    //    $('#txt_submittedby').text(monitoring.ULSubmitUser == null || monitoring.ULSubmitUser == '' ? 'N/A' : monitoring.ULSubmitUser);
    //    $('#txt_sku').text(monitoring.UIOrigQuantity == null || monitoring.UIOrigQuantity == '' ? 'N/A' : monitoring.UIOrigQuantity);
    //    $('#txt_qty').text(monitoring.UIQuantity == null || monitoring.UIQuantity == '' ? 'N/A' : monitoring.UIQuantity);
    //    $('#txt_amount').text(monitoring.UIPrice == null || monitoring.UIPrice == '' ? 'N/A' : monitoring.UIPrice);
    //    $('#txt_account').text(monitoring.RFAccount == null || monitoring.RFAccount == '' ? 'N/A' : monitoring.RFAccount);

    //    $('#ViewPODetailsModal').modal('show');
    //}

    $scope.advsearch = function () {
        $scope.Items = {};
        $scope.Items.dateFrom = $("#datefrom").val();
        $scope.Items.dateTo = $("#dateto").val();
       
        $scope.data = {};
        //$scope.data.operation = "advsearch";
        $scope.data.payload = _payloadParser($scope.Items);

       

        viewModelHelper.apiGet('api/monitor', $scope.data, function (result) {

            var orders = result.data.detail.filter(x=>x.uiprice = parseInt(x.uiprice).toLocaleString());
            orders.filter(x=> {
                var data = x.ulid + ',' + x.ulstatus;
                if (x.ulstatus < 20) {
                    viewModelHelper.apiGet('api/monitorerrors/' + data, null, function (result) {
                        x.eCtr = result.data.detail.length;
                    });
                }
            });

            $scope.filterMonitor = result.data.detail;
            $scope.noOfPages = Math.ceil($scope.filterMonitor.length / $scope.entryLimit);
            $scope.currentPage = 1;
        });

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('order', [7, 'desc']);
        $scope.dtColumnDefs = [
           DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
        ];
    }

    $scope.showDetails = function (order) {
        console.log(order);
        var orderDetails = [];
        orderDetails[0] = {};

        $scope.ifConfirm = false;

        $('.table-data-pmcode').attr('title', '');
        $('.table-data-pmcode').val();

        $scope.flags.shownFromList = true;
        if (order.uiprice < 0) {
            notif_warning('Order Details', 'Lack of Product Mapping');
        } else if (order.ulcustomer == '0') {
            notif_info('Customer Mapping', 'Please Assign Customer Number');

            orderDetails.txt_cmdescription = order.cmdescription;
            orderDetails.txt_ponum = order.ulponumber;
            orderDetails.txt_custnum = order.ulcustomer;
            orderDetails.txt_status = order.ulstatus;
            orderDetails.txt_orderid = order.ulid;

            var Item = {};
            Item.ULId = order.ulid;

            $scope.data = {};
            $scope.data.operation = 'get_custError';
            $scope.data.payload = _payloadParser(Item);

            viewModelHelper.apiGet('api/cust', $scope.data, function (result) {
                var data = result.data;

                if (data.success) {
                    orderDetails.txt_elid = data.detail[0].ELDetail;
                } else {
                    orderDetails.txt_elid = '';
                }

                $scope.ordersInfo = orderDetails;

                $('#txt_ponum').val(order.ulponumber);
                $('#txt_id').val(order.ulid);
                $('#txt_account_det').val(order.rfaccount);

                $('#orderCustDetailsModal').modal('show');
            })
        }
        else {

            if (order.ulstatus == "20") {
                $scope.ifEditable = false;
            } else {
                $scope.ifEditable = true;
            }
            orderDetails.txt_ponum = order.ulponumber;
            orderDetails.txt_custnum = order.ulcustomer;
            orderDetails.txt_orderdate = new Date(order.ulorderdate);
            orderDetails.txt_deliverydate = new Date(order.uldeliverydate);
            orderDetails.txt_remarks = order.ulremarks; 
            orderDetails.txt_status = order.ulstatus;
            orderDetails.txt_orderid = order.ulid;

            $scope.ordersInfo = orderDetails;

            viewModelHelper.apiGet('api/order/' + order.ulid, null, function (result) {
                console.log(order.ulid);
                console.log(result.data.detail);
                $scope.orderItems = result.data.detail.filter(x=>x.UITPrice = parseInt(x.UITPrice).toLocaleString());
                $scope.orderItems = result.data.detail.filter(x=>x.UIPrice = parseInt(x.UIPrice).toLocaleString());
            });

            $('#txt_account_det').val(order.rfaccount);
            $('#txt_id').val(order.ulid);
            $('#txt_status').val(order.ulstatus);

            $('#orderDetailsModal').modal('show');
        }
    }

    $scope.downloadJSON = function (data) {
        console.log(data.TLId);
        window.location.href = viewModelHelper.getRootPath() + "api/download/" + data.TLId+',trans';
    };

    $scope.downloadFile = function (data) {
        console.log(data.RFId);
        window.location.href = viewModelHelper.getRootPath() + "api/download/" + data.RFId+',order';
    };

    $scope.advancedsearch = function () {
        $('#advancedModal').modal('show');
    }

    $scope.getStatus = function (id, $event) {
        var datas = id.ulid + ',' + id.ulstatus;
        $scope.errorPO = "";
        if (parseInt(id.ulstatus) < 20) {
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
                    for (var i = 0, j = eSplit.length; i < j - 1; i++) {
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
            notif_success("New order", "Successful");
        }
    }

    $("#datefrom").datepicker({
        dateFormat: 'yy-mm-dd' ,
        defaultDate: new Date(),
        onSelect: function (dateStr) {
            $("#dateto").val(dateStr);
            $("#dateto").datepicker("option", { minDate: new Date(dateStr) })
        }
    });

    $('#dateto').datepicker({
        dateFormat: 'yy-mm-dd',
        defaultDate: new Date(),
        onSelect: function (dateStr) {
            toDate = new Date(dateStr);
        }
    });

    initialize();
}).filter('start', function () {
    return function (input, start) {
        if (!input || !input.length) { return; }

        start = +start;
        return input.slice(start);
    };
});