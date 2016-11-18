monitoringModule.controller("monitoringViewModel", function ($scope, monitoringService, $http, $q, $routeParams, $window, $location, viewModelHelper, DTOptionsBuilder, DTColumnDefBuilder, filterFilter) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.monitoringService = monitoringService;

    var initialize = function () {
        $scope.refreshMonitor();
        $scope.refreshTransact();
        $scope.getAccess();
    }

    $scope.getAccess = function () {
        viewModelHelper.apiGet('api/master', null, function (result) {
            console.log(result);
            $scope.UserAccess = result.data;
        });
    }

    $scope.refreshMonitor = function () {

        $scope.searchPO = {};
        $scope.searchByPO = "ULPONumber";

        viewModelHelper.apiGet('api/monitor', null, function (result) {
            $scope.pageSize = 5;
            $scope.entryLimit = 50;

            $scope.monitor = result.data.detail;
            console.log(result.data.detail);
            $scope.$watch('searchPO[searchByPO]', function () {
                $scope.filterMonitor = filterFilter($scope.monitor, $scope.searchPO);
                $scope.noOfPages = Math.ceil($scope.filterMonitor.length / $scope.entryLimit);
                $scope.currentPage = 1;
            });
        });

        $scope.dtOptions = DTOptionsBuilder.newOptions().withOption('order', [2, 'desc']);
        $scope.dtColumnDefs = [
           DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
        ];
    }

    $scope.refreshTransact = function () {

        $scope.searchTransaction = {};
        $scope.searchByTransaction = "TLValue";

        $scope.data = {};
        $scope.data.operation = 'transaction';
        
        viewModelHelper.apiGet('api/monitor', $scope.data, function (result) {
            $scope.pageSize = 5;
            $scope.entryLimit = 50;

            $scope.transact = result.data.detail;
            console.log(result.data.detail);
            $scope.$watch('searchTransaction[searchByTransaction]', function () {
                $scope.filterTransaction = filterFilter($scope.transact, $scope.searchTransaction);
                $scope.noOfPages = Math.ceil($scope.filterTransaction.length / $scope.entryLimit);
                $scope.currentPage = 1;
            });
        });

        $scope.dtOptionsTrans = DTOptionsBuilder.newOptions().withOption('order', [0, 'desc']);
        $scope.dtColumnDefsTrans = [
           DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
        ];
    }

    $scope.showDetails = function (monitoring) {
       // $scope.ResetEditFields();
        $('#txt_ponum').text(monitoring.ULPONumber == null || monitoring.ULPONumber == '' ? 'N/A' : monitoring.ULPONumber);
        $('#txt_custnum').text(monitoring.ULCustomer == null || monitoring.ULCustomer == '' ? 'N/A' : monitoring.ULCustomer);
        $('#txt_custname').text(monitoring.CMDescription == null || monitoring.CMDescription == '' ? 'N/A' : monitoring.CMDescription);
        $('#txt_orderdate').text(monitoring.ULOrderDate == null || monitoring.ULOrderDate == '' ? 'N/A' : monitoring.ULOrderDate);
        $('#txt_deliverydate').text(monitoring.ULDeliveryDate == null || monitoring.ULDeliveryDate == '' ? 'N/A' : monitoring.ULDeliveryDate);
        $('#txt_retrievedate').text(monitoring.RFRetrieveDate);
        $('#txt_readdate').text(monitoring.RFReadDate == null || monitoring.RFReadDate == '' ? 'N/A' : monitoring.RFReadDate);
        $('#txt_submitteddate').text(monitoring.RFSubmitDate == null || monitoring.RFSubmitDate == '' ? 'N/A' : monitoring.RFSubmitDate);
        $('#txt_reader').text(monitoring.RFReadUser == null || monitoring.RFReadUser == '' ? 'N/A' : monitoring.RFReadUser);
        $('#txt_submittedby').text(monitoring.RFSubmitUser == null || monitoring.RFSubmitUser == '' ? 'N/A' : monitoring.RFSubmitUser);
        $('#txt_sku').text(monitoring.UIOrigQuantity == null || monitoring.UIOrigQuantity == '' ? 'N/A' : monitoring.UIOrigQuantity);
        $('#txt_qty').text(monitoring.UIQuantity == null || monitoring.UIQuantity == '' ? 'N/A' : monitoring.UIQuantity);
        $('#txt_amount').text(monitoring.UIPrice == null || monitoring.UIPrice == '' ? 'N/A' : monitoring.UIPrice);
        $('#txt_account').text(monitoring.RFAccount == null || monitoring.RFAccount == '' ? 'N/A' : monitoring.RFAccount);

        $('#ViewPODetailsModal').modal('show');
    }

    $scope.downloadJSON = function (data) {
        console.log(data.TLId);
        window.location.href = viewModelHelper.getRootPath() + "api/download/" + data.TLId+',trans';
    };

    $scope.downloadFile = function (data) {
        console.log(data.RFId);
        window.location.href = viewModelHelper.getRootPath() + "api/download/" + data.RFId+',order';
    };

    initialize();
}).filter('start', function () {
    return function (input, start) {
        if (!input || !input.length) { return; }

        start = +start;
        return input.slice(start);
    };
});