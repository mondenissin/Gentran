orderModule.controller("rootViewModel", function ($scope, orderService, $http, $q, $routeParams, $window, $location, viewModelHelper) {

    // This is the parent controller/viewmodel for 'orderModule' and its $scope is accesible
    // down controllers set by the routing engine. This controller is bound to the Order.cshtml in the
    // Home view-folder.
    $scope.viewModelHelper = viewModelHelper;
    $scope.orderService = orderService;

    $scope.flags = { shownFromList: false };

    var initialize = function () {
        //$scope.pageHeading = "User Section";
    }
    $scope.ftpList = function () {
        viewModelHelper.navigateTo('Order/FTP');
    }

    initialize();
});
