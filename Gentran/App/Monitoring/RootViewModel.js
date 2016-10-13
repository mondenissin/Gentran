monitoringModule.controller("rootViewModel", function ($scope, monitoringService, $http, $q, $routeParams, $window, $location, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.monitoringService = monitoringService;

    $scope.flags = { shownFromList: false };

    var initialize = function () {
        //$scope.pageHeading = "User Section";
    }

    //$scope.monitoringList = function () {
    //    viewModelHelper.navigateTo('Monitoring/POMonitoring');
    //}

    //$scope.showOrder = function () {

    //    if (orderService.orderId == undefined) {
    //        alert("Enter PO Number");
    //    } else {
    //        console.log(orderService.orderId);
    //        console.log($scope.order);
    //    }
    //}

    initialize();
});
