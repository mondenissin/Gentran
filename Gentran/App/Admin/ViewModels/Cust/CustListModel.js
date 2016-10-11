adminModule.controller("custViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, viewModelHelper, DTOptionsBuilder, DTColumnDefBuilder) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    var initialize = function () {
        $scope.refreshCust();
    }

    $scope.refreshCust = function () {
        $scope.search = {};
        $scope.searchBy = "CMCode";

        viewModelHelper.apiGet('api/cust', null, function (result) {
            $scope.customers = result.data.detail;
        }); 

        $scope.dtOptions = DTOptionsBuilder.newOptions();
        $scope.dtColumnDefs = [
           DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
        ];
    }

    $scope.addCustomer = function () {
        $('#AddCustomerModal').modal('show');
    }
    
    initialize();
});