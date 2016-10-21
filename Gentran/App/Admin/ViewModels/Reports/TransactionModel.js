adminModule.controller("reportViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, $timeout, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    var initialize = function () {
        $scope.refreshReport();
        $scope.dateFrom = new Date();
        $scope.dateTo = new Date();
    }

    $scope.refreshReport = function () {
        viewModelHelper.apiGet('api/reports', null, function (result) {
            $scope.transactions = result.data.detail;
        });
    }


    $scope.generateReport = function () {

        var ope = "";
        var Item = {};
        Item.dateFrom = ($scope.dateFrom.getMonth() + 1) + "/" + $scope.dateFrom.getDate() + "/" + $scope.dateFrom.getFullYear();
        Item.dateTo = ($scope.dateTo.getMonth() + 1) + "/" + $scope.dateTo.getDate() + "/" + $scope.dateTo.getFullYear();

        if ($scope.All) {
            ope = "all";
        } else if ($scope.byUser) {
            ope = "by_user";
            Item.ULUser = $scope.User;
        } else {
            ope = "normal";
        }

        $scope.dataReport = {};
        $scope.dataReport.operation = ope;
        $scope.dataReport.payload = _payloadParser(Item);

        viewModelHelper.apiPut('api/reports', $scope.dataReport, function (result) {

            if (result.data.success == true) {
                viewModelHelper.saveTransaction(result.data.detail);
                notif_success('Succesful!', 'Report Generated');
                console.log(result.data.reports);
            } else {
                if (result.data.notiftext == null || result.data.notiftext == "") {
                    notif_error('Error!', 'Report not generated');
                } else {
                    viewModelHelper.saveTransaction(result.data.detail);
                    notif_success('Succesful!', 'Report Generated');
                    console.log(result.data.reports);
                }
            }
        });
    }
   
    initialize();
})