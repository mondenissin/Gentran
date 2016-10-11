orderModule.controller("orderViewModel", function ($sce, $scope, orderService, $http, $q, $routeParams, $window, $location, viewModelHelper, $timeout) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.orderService = orderService;

    var initialize = function () {
        $scope.refreshOrders();
    }

    $scope.refreshOrders = function () {

        viewModelHelper.apiGet('api/order', null, function (result) {
            console.log(result.data.detail);
            $scope.order = result.data.detail;
        });
    }

    $scope.showDetails = function (order) {
        $scope.flags.shownFromList = true;
        console.log(order);

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

    initialize();
});