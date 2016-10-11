orderModule.controller("rootViewModel", function ($scope, orderService, $http, $q, $routeParams, $window, $location, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.orderService = orderService;

    $scope.flags = { shownFromList: false };

    var initialize = function () {
        //$scope.pageHeading = "User Section";
    }

    $scope.orderList = function () {
        viewModelHelper.navigateTo('Order/OrderList');
    }

    initialize();
});
