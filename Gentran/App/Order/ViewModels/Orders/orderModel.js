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

    $scope.showOrder = function (order) {
        $scope.flags.shownFromList = true;

        console.log(order);

        //viewModelHelper.navigateTo('order/show/' + order.OrderId);
    }


    initialize();
});