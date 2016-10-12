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
        
        var orderDetails = [];
        orderDetails[0] = {};
        orderDetails.txt_ponum = order.ulponumber;
        orderDetails.txt_custnum = order.ulcustomer;
        orderDetails.txt_orderdate = order.ulorderdate;
        orderDetails.txt_deliverydate = order.uldeliverydate;
        orderDetails.txt_remarks = "";
        $scope.ordersInfo = orderDetails;

        viewModelHelper.apiGet('api/order/' + order.ulid, null, function (result) {
            console.log(result.data.detail);
            $scope.orderItems = result.data.detail;
        });

        $('#orderDetailsModal').modal('show');
        //viewModelHelper.navigateTo('order/show/' + order.OrderId);
    }

    $scope.submitOrder = function () {

        var selected = false;
        var ctr = 0;
        var listOrder = [];

        $('.cbxSub').each(function () {
            if (this.checked == true) {
                selected = true;
                //this.checked = false;

                listOrder[ctr] = {};

                listOrder[ctr].PONum = $(this).closest('tr').find('.table-data-ponum').text();
                listOrder[ctr].CustNum = $(this).closest('tr').find('.table-data-ucust').text();
                listOrder[ctr].CustName = $(this).closest('tr').find('.table-data-cdesc').text();
                listOrder[ctr].OrderDate = $(this).closest('tr').find('.table-data-uodate').text();
                listOrder[ctr].Quantity = $(this).closest('tr').find('.table-data-sqty').text();

                ctr++;
            }
        }).promise().done(function () {
            $scope.submitQueue = listOrder;

            if (selected) {
                $('#submitModal').modal('show');
                
            } else {
                alert("Select order to submit");
            }
        });
    }

    $scope.submittingOrders = function (queued) {
        console.log(queued);
    }

    $scope.getStatus = function (id) {
        var datas = id.ulid + "," + id.ulstatus;
        viewModelHelper.apiGet('api/ordererrors/' + datas, null, function (result) {
            //READY FOR BALLOON RESPONSE

            if (result.data.success == true) {
                var error = "PO ID: " + result.data.detail[0].ELId + "\n";
                for (var x = 0; x < result.data.detail.length; x++) {
                    error += result.data.detail[x].ELDetail + "\n";

                    var productError = "";
                    var productErrorDetail = "";
                    var storeError = "";
                    var storeErrorDetail = "";

                    if (result.data.detail[x].ELType == 'Invalid Product') {
                        if (productErrorDetail == "") {
                            productErrorDetail = "Product Code: " + data.detail[i].ELDetail;
                            productError = data.detail[i].ELType;
                        }
                        else {
                            productErrorDetail = productErrorDetail + ", " + data.detail[i].ELDetail;
                        }
                    }
                    else if (result.data.detail[x].ELType == 'Invalid Customer') {
                        if (storeErrorDetail == "") {
                            storeErrorDetail = "Store Code: " + data.detail[i].ELDetail;
                            storeError = data.detail[i].ELType;
                        }
                        else {
                            storeErrorDetail = storeErrorDetail + ", " + data.detail[i].ELDetail;
                        }
                    }
                    else {
                        alert(error);
                    }
                }
            } else {
                alert("API ERROR!");
            }
        });
    }

    $scope.editDetails = function ($event) {

        if ($($($event.target)[0]).text() == "Edit") {
            $scope.ifEdit = false;
            $($($event.target)[0]).text("Save")
        } else {
            $scope.ifEdit = true;
            $($($event.target)[0]).text("Edit")
        }
    }

    initialize();
});