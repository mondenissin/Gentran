monitoringModule.controller("monitoringViewModel", function ($scope, monitoringService, $http, $q, $routeParams, $window, $location, viewModelHelper, DTOptionsBuilder, DTColumnDefBuilder, filterFilter) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.monitoringService = monitoringService;

    var initialize = function () {
        $scope.refreshCust();
    }

    $scope.refreshCust = function () {
        $scope.search = {};
        $scope.searchBy = "TLId";

        viewModelHelper.apiGet('api/monitor', null, function (result) {
                $scope.pageSize = 5;
                $scope.entryLimit = 50;

              $scope.monitor = result.data.detail;
              console.log(result.data.detail);
                $scope.$watch('search[searchBy]', function () {
                    $scope.filterList = filterFilter($scope.monitor, $scope.search);
                    $scope.noOfPages = Math.ceil($scope.filterList.length / $scope.entryLimit);
                    $scope.currentPage = 1;
                });
        });

        $scope.dtOptions = DTOptionsBuilder.newOptions();
        $scope.dtColumnDefs = [
           DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
        ];
    }

    //$scope.clearSearch = function () {
    //    $scope.search = {};
    //}

    initialize();
}).filter('start', function () {
    return function (input, start) {
        if (!input || !input.length) { return; }

        start = +start;
        return input.slice(start);
    };
});