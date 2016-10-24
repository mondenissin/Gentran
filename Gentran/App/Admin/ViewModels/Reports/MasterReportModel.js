adminModule.controller("reportViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, $timeout, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;
    var reportType = "purchase";

    var initialize = function () {
        $scope.hideReport = true;
        $scope.refreshReport();
        $scope.dateFrom = new Date();
        $scope.dateTo = new Date();
    }

    $scope.refreshReport = function () {
        viewModelHelper.apiGet('api/reports', null, function (result) {
            $scope.transactions = result.data.detail;
        });

        $(function () {
            $('.product-chooser').not('.disabled').find('.product-chooser-item').on('click', function () {
                reportType = $($(this).find('span')[0])[0].title;
                $(this).parent().parent().find('.product-chooser-item').removeClass('selected');
                $(this).addClass('selected');
                $(this).find('input[type="radio"]').prop("checked", true);

            });
        });
    }

    $scope.generateReport = function () {
        console.log(reportType);
        $scope.hideReport = false;

        if (reportType == "purchase") {
            $('#' + reportType).show(true);
            $('#transaction').hide();
        } else if (reportType == "transaction") {
            $('#' + reportType).show(true);
            $('#purchase').hide();
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
        $scope.dataReport.reportName = reportType;
        $scope.dataReport.operation = ope;
        $scope.dataReport.payload = _payloadParser(Item);

        viewModelHelper.apiPut('api/reports', $scope.dataReport, function (result) {

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