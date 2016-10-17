adminModule.controller("translogViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, viewModelHelper, DTOptionsBuilder, filterFilter) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    var initialize = function () {
        $scope.refreshLogs();
    }


    $scope.refreshLogs = function () {
        $scope.search = {};
        $scope.searchBy = "TLId";

        viewModelHelper.apiGet('api/logs', null, function (result) {
            $scope.pageSize = 5;
            $scope.entryLimit = 50;

            $scope.transactions = result.data.detail;

            $scope.$watch('search[searchBy]', function () {
                $scope.filterList = filterFilter($scope.transactions, $scope.search);
                $scope.noOfPages = Math.ceil($scope.filterList.length / $scope.entryLimit);
                $scope.currentPage = 1;
            });
        });
    }

    $scope.clearSearch = function () {
        $scope.search = {};
    }

    initialize();
}).filter('start', function () {
    return function (input, start) {
        if (!input || !input.length) { return; }

        start = +start;
        return input.slice(start);
    };
});