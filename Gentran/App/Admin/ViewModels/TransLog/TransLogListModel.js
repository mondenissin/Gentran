adminModule.controller("translogViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, viewModelHelper, DTOptionsBuilder) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    var initialize = function () {
        $scope.refreshLogs();
    }

    $scope.currentPage = 1, $scope.numPerPage = 50, $scope.maxSize = 5;

    $scope.refreshLogs = function () {
        $scope.search = {};
        $scope.searchBy = "TLId";

        viewModelHelper.apiGet('api/logs', null, function (result) {
            $scope.transactionsPerPage = result.data.detail;

            $scope.numPages = function () {
                return Math.ceil($scope.transactionsPerPage.length / $scope.numPerPage);
            };

            $scope.$watch('currentPage + numPerPage', function () {
                var begin = (($scope.currentPage - 1) * $scope.numPerPage),
                  end = begin + $scope.numPerPage;

                $scope.transactions = $scope.transactionsPerPage.slice(begin, end);
            });
        });
    }

    initialize();
});