monitoringModule.controller("monitoringViewModel", function ($scope, monitoringService, $http, $q, $routeParams, $window, $location, viewModelHelper, DTOptionsBuilder, DTColumnDefBuilder) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.monitoringService = monitoringService;

    var initialize = function () {
        //$scope.refreshCust();
    }

    //$scope.refreshCust = function () {
    //    $scope.search = {};
    //    $scope.searchBy = "CMCode";

    //    viewModelHelper.apiGet('api/cust', null, function (result) {
    //        $scope.customers = result.data.detail;
    //    });

    //    $scope.dtOptions = DTOptionsBuilder.newOptions();
    //    $scope.dtColumnDefs = [
    //       DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
    //    ];
    //}

    //$scope.addCustomer = function () {
    //    $('#AddCustomerModal').modal('show');
    //}

    initialize();
});