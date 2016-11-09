adminModule.controller("reportViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, $timeout, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;
    $scope.ReportType = "purchase";
    $scope.reportMaster = "";

    var initialize = function () {
        $scope.hideReport = true;
        $scope.refreshReport();
        $scope.dateTo = new Date();

        //1 WEEK DATE GAP
        $scope.dateFrom = new Date(Date.parse(($scope.dateTo.getMonth()+1) + "/" + $scope.dateTo.getDate() + "/" + $scope.dateTo.getFullYear()));
        $scope.dateFrom.setDate(+$scope.dateTo.getDate() - 7)
    }

    $scope.refreshReport = function () {
        viewModelHelper.apiGet('api/reports', null, function (result) {
            $scope.transactions = result.data.detail;
        });
    }

    $scope.generateReport = function () {
        console.log($scope.reportMaster);

        if ($scope.reportMaster == "") {
            notif_warning('Information!', 'Select Report Type');
            return;
        }

        var ope = "";
        var Item = {};
        Item.dateFrom = ($scope.dateFrom.getMonth() + 1) + "/" + $scope.dateFrom.getDate() + "/" + $scope.dateFrom.getFullYear();
        Item.dateTo = ($scope.dateTo.getMonth() + 1) + "/" + $scope.dateTo.getDate() + "/" + $scope.dateTo.getFullYear();

        if ($scope.All) {
            ope = "all";
        } else if ($scope.byUser) {
            if ($scope.User == "" || $scope.User == null) {
                notif_warning('Warning!', 'Please select User');
                return;
            }else{
                ope = "by_user";
                Item.ULUser = $scope.User;
            }
        } else {
            ope = "normal";
        }

        $scope.dataReport = {};
        $scope.dataReport.reportName = $scope.ReportType;
        $scope.dataReport.operation = ope;
        $scope.dataReport.payload = _payloadParser(Item);

        $scope.hideReport = false;

        if ($scope.ReportType == "purchase") {
            $('#generatedModalPurc').modal('show');
        } else if ($scope.ReportType == "product") {
            $('#generatedModalProd').modal('show');
        }
        console.log($scope.dataReport);
        viewModelHelper.apiPut('api/reports', $scope.dataReport, function (result) {

            console.log(result.data.reports);
            if (result.data.success == true) {
                $scope.reportGenerated = result.data.reports;
                viewModelHelper.saveTransaction(result.data.detail);
                notif_success('Succesful!', 'Report Generated');
            } else {
                if (result.data.notiftext == null || result.data.notiftext == "") {
                    notif_error('Error!', 'Report not generated');
                } else {
                    $scope.reportGenerated = result.data.reports;
                    viewModelHelper.saveTransaction(result.data.detail);
                    notif_success('Succesful!', 'Report Generated');
                    console.log(result.data.reports);
                }
            }
        });
    }

    $scope.printReport = function (content) {
        console.log($scope.reportGenerated);
        if ($scope.reportGenerated == null || $scope.reportGenerated.length == 0) {
            notif_warning('Warning!','No data to be print');
        } else {
            var dataContent = document.getElementById(content).innerHTML;
            console.log(dataContent);
            var mywindow = window.open('', 'Transaction Report', 'height=' + window.screen.availHeight + ',width=' + window.screen.availWidth + '');
            mywindow.document.write('<html><head><title>Transaction Report</title>');
            mywindow.document.write('</head><body >');
            mywindow.document.write(dataContent);
            mywindow.document.write('</body></html>');

            mywindow.document.close();
            mywindow.focus();

            mywindow.print();
            mywindow.close();
        }
    }
   
    initialize();
})