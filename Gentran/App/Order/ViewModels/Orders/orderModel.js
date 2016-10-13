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
        var ifNew = true;
        var ctr = 0;
        var listOrder = [];

        $('.cbxSub').each(function () {
            if (this.checked == true) {
                var status = $(this).closest('tr').find('.table-data-ustat').text();
                if (status == "New") {
                    ifNew = true;
                    selected = true;

                    listOrder[ctr] = {};

                    listOrder[ctr].PONum = $(this).closest('tr').find('.table-data-ponum').text();
                    listOrder[ctr].CustNum = $(this).closest('tr').find('.table-data-ucust').text();
                    listOrder[ctr].CustName = $(this).closest('tr').find('.table-data-cdesc').text();
                    listOrder[ctr].OrderDate = $(this).closest('tr').find('.table-data-uodate').text();
                    listOrder[ctr].Quantity = $(this).closest('tr').find('.table-data-sqty').text();
                    console.log();
                    ctr++;
                } else{
                    ifNew = false;
                }
            }
        }).promise().done(function () {
            $scope.submitQueue = listOrder;

            if (selected) {
                $('#submitModal').modal('show');
            }else {
                if(!(ifNew)){
                    alert("Only New PO's can be submit");
                } else {
                    alert("Select order to submit");
                }
            }
        });
    }

    $scope.submittingOrders = function (queued) {
        console.log(queued);
    }

    $scope.getStatus = function (id) {
        var datas = id.ulid + "," + id.ulstatus;

        if(parseInt(id.ulstatus) < 20){
            viewModelHelper.apiGet('api/ordererrors/' + datas, null, function (result) {
                //READY FOR BALLOON RESPONSE    
                if (result.data.success == true) {
                    console.log(result.data.detail);
                    for (var i = 0; i < result.data.detail.length; i++) {
                        var error = "PO ID: " + result.data.detail[i].ELId + "\n";
                        var productErrorDetail = "";
                        var storeErrorDetail = "";

                        if (result.data.detail[i].ELType == 'Invalid Product') {
                            if (productErrorDetail == "") {
                                productErrorDetail = "Product Code: " + result.data.detail[i].ELDetail;
                                error += result.data.detail[i].ELType + "\n"+ productErrorDetail + "\n";
                            }
                        }
                        else if (result.data.detail[i].ELType == 'Invalid Customer') {
                            if (storeErrorDetail == "") {
                                storeErrorDetail = "Store Code: " + result.data.detail[i].ELDetail;
                                error += result.data.detail[i].ELType + "\n" + storeErrorDetail + "\n";
                            }
                        }
                        else {
                            error += result.data.detail[i].ELDetail + "\n";
                        }
                        alert(error);
                    }
                } else {
                    alert("API ERROR!");
                }
            });
        }
        else {
            alert("Successful");
        }
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