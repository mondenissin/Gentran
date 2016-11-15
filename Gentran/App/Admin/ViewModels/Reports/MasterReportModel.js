adminModule.controller("reportViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, $timeout, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;
    $scope.ReportType = "purchase";
    $scope.reportMaster = "";
    $scope.reportDateMaster = "";

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
        $scope.reportGenerated = [];

        if ($scope.reportMaster == "" && $scope.ReportType == "purchase") {
            notif_warning('Purchase Report!', 'Select Report Type');
            return;
        } else if ($scope.reportDateMaster == "" && $scope.ReportType == "purchase") {
            notif_warning('Purchase Report!', 'Select Filter Date');
            return;
        } else if ($scope.reportMaster == "" && $scope.ReportType == "product") {
            notif_warning('Product Report!', 'Select Report Type');
            return;
        } else if ($scope.reportDateMaster == "" && $scope.ReportType == "product") {
            notif_warning('Product Report!', 'Select Filter Date');
            return;
        }

        var ope = "";
        var Item = {};
        Item.dateFrom = ($scope.dateFrom.getMonth() + 1) + "/" + $scope.dateFrom.getDate() + "/" + $scope.dateFrom.getFullYear();
        Item.dateTo = ($scope.dateTo.getMonth() + 1) + "/" + $scope.dateTo.getDate() + "/" + $scope.dateTo.getFullYear();
        Item.reportType = $scope.reportMaster;
        Item.reportDate = $scope.reportDateMaster;

        if ($scope.All && $scope.ReportType == "purchase") {
            ope = "all";
        }else if ($scope.All && $scope.ReportType == "product") {
            ope = "all";
        }else if ($scope.byUser && $scope.ReportType == "purchase") {
            if ($scope.User == "" || $scope.User == null) {
                notif_warning('Warning!', 'Please select User');
                return;
            }else{
                ope = "by_user";
                Item.ULUser = $scope.User;
            }
        } else if ($scope.byCategory && $scope.ReportType == "product") {
            if ($scope.Category == "" || $scope.Category == null) {
                notif_warning('Warning!', 'Please select Category');
                return;
            } else {
                ope = "by_category";
                Item.prodcategory = $scope.Category;
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
    
    $scope.exportReport = function (content) {
        console.log($scope.reportGenerated);
        if ($scope.reportGenerated == null || $scope.reportGenerated.length == 0) {
            notif_warning('Warning!', 'No data to be Export');
        } else {
            var strContentHead = "";
            var strContentBody = "";

            strContentHead = "<div><table><thead><tr>";
            $($('#' + content + ' table thead tr')[0].children).each(function (index, item) {
                strContentHead += "<th>" + $(item)[0].textContent + "</th>";
            });
            strContentHead += "</tr></thead>";
            strContentBody += "<tbody>";

            $('#' + content + ' table tbody tr').each(function (index, item) {
                strContentBody += "<tr>";
                $($($(item)[0])[0].children).each(function (i, it) {
                    strContentBody += "<td>" + $(it)[0].textContent.toString() + "</td>";
                });
                strContentBody += "</tr>";
            });
            strContentBody += "</tbody></table></div>";

            window.open('data:application/vnd.ms-excel,' + strContentHead + strContentBody);
            window.close();

            notif_success('Successful', 'Successfully exported to excel');
        }
    }
   
    initialize();
})