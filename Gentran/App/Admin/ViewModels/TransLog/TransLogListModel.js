adminModule.controller("translogViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, viewModelHelper, DTOptionsBuilder) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    var initialize = function () {
        $scope.refreshLogs();
    }

    $scope.refreshLogs = function () {
        $scope.search = {};
        $scope.searchBy = "TLId";

        viewModelHelper.apiGet('api/logs', null, function (result) {
            $scope.transactions = result.data.detail;
        });
    }

    initialize();
});