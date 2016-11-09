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

    $scope.showDetails = function (monitoring) {
       // $scope.ResetEditFields();
        $('#txt_ponum').text(monitoring.ULPONumber);
        $('#txt_custnum').text(monitoring.ULCustomer != 0 ? monitoring.ULCustomer : 'No Customer');
        $('#txt_custname').text(monitoring.ULCustomer);
        $('#txt_orderdate').text(monitoring.ULOrderDate);
        $('#txt_deliverydate').text(monitoring.ULDeliveryDate);
        $('#txt_retrievedate').text(monitoring.ULUploadDate);
        $('#txt_uploaddate').text(monitoring.ULUploadDate);
        $('#txt_submitteddate').text(monitoring.ULUploadDate);
        $('#txt_reader').text(monitoring.ULUser);
        $('#txt_submittedby').text(monitoring.ULUser);
        $('#txt_sku').text(monitoring.UIOrigQuantity);
        $('#txt_qty').text(monitoring.UIQuantity);
        $('#txt_amount').text(monitoring.UIPrice);

        $('#ViewPODetailsModal').modal('show');
    }

    initialize();
}).filter('start', function () {
    return function (input, start) {
        if (!input || !input.length) { return; }

        start = +start;
        return input.slice(start);
    };
});