adminModule.controller("dashViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, $timeout, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    //========
    $scope.totalOrders = 0;
    $scope.totalSKUs = 0;
    $scope.totalQuantity = 0;
    $scope.totalIncome = 0;

    $scope.filterText = "";
    //========

    var initialize = function () {
        $scope.getArea();
        $scope.getAccounts();

        $scope.Items = null;
        $scope.filterText = "(This year)";
        $scope.refreshDash($scope.Items);
    }


    $scope.getArea = function () {
        $scope.data = {};
        $scope.data.operation = 'get_area';
        viewModelHelper.apiGet('api/cust', $scope.data, function (result) {
            $scope.areaMaster = result.data.detail;
        });
    }

    $scope.getAccounts = function () {
        $scope.data = {};
        $scope.data.operation = 'get_accounts';
        viewModelHelper.apiGet('api/cust', $scope.data, function (result) {
            $scope.accountMaster = result.data.detail;
        });
    }

    $scope.refreshData = function (Items,callback) {
        viewModelHelper.apiGet('api/dash', Items, function (result) {
            var data = result.data;
            var orders = 0, skus = 0, tskus = 0, amount = 0;
            if (data.success == true) {
                for (var i = 0; i < data.detail.length; i++) {
                    orders += data.detail[i].ULTotalOrders;
                    skus += data.detail[i].ULTotalSKUs;
                    tskus += data.detail[i].ULTotalQuantity;
                    amount += data.detail[i].ULTotalAmount;
                }
                callback(data);
            } else {
                $scope.area.setData([]);
            }

            $scope.totalOrders = orders;
            $scope.totalSKUs = skus;
            $scope.totalQuantity = tskus;
            $scope.totalIncome = amount.toLocaleString();
        });
    }
    var ti;
    $scope.setFilter = function () {
        var filterArea = $('.ddl_area').val();
        var filterChain = $('.ddl_chain').val();
        var filterDate = $('.txt_daterange').val().split('-');

        var operation = filterArea != '-1' && filterChain != '-1' ? "filter_dashboard_by_all" : filterArea != '-1' ? "filter_dashboard_by_area" : filterChain != '-1' ? "filter_dashboard_by_chain" : "filter_dashboard_date";

        var dFrom = filterDate[0].split('/');
        var dTo = filterDate[1].split('/');

        var sFrom = dFrom[1] + '-' + dFrom[2] + '-' + dFrom[0];
        var sTo = dTo[1] + '-' + dTo[2] + '-' + dTo[0];
        var Item = {};

        Item.dateFrom = sFrom + ' 00:00:00.000';
        Item.dateTo = sTo + ' 23:59:59.999';
        Item.cmArea = filterArea;
        Item.acctype = filterChain;

        $scope.Items = {};
        $scope.Items.operation = operation;
        $scope.Items.payload = _payloadParser(Item);

        $scope.filterText = sFrom == sTo ? 'Filter: ' + sTo : 'Filter: ' + sFrom + '  to ' + sTo;

        function load() {
            $scope.refreshData($scope.Items, function (data) {
                var areaData = [];
                for (var i = 0; i < data.detail.length; i++) {
                    areaData[i] = {};
                    areaData[i].label = data.detail[i].RFRetrieveDate;
                    areaData[i].orders = data.detail[i].ULTotalOrders;
                    areaData[i].amount = data.detail[i].ULTotalAmount;
                }
                $scope.area.setData(areaData);
            });
        }

        load();
        clearInterval(ti);

        ti = setInterval(function () {
            load();
        }, 60000*5);
    }

    $scope.refreshDash = function (Items) {
        $scope.refreshData(Items, function (data) {
            $scope.AreaChartData(data.detail);
        });
    }
   
    $scope.tools = function (proc, $event) {
        var selected;
        var area = $('#message-text');
        
        if (area[0] == $(window.getSelection().anchorNode)[0] || area[0] == $(window.getSelection().anchorNode)[0].parentElement) {
            selected = window.getSelection().anchorNode.textContent.substring(window.getSelection().extentOffset, window.getSelection().anchorOffset);
            
            switch (proc) {
                case 'bold':
                    area.text("Not Functioning");
                    break;
                case 'italic':
                    area.text("Not Functioning");
                    break;
                default:
                    alert("asd");
            }
        }
    }

    //Area Chart
    $scope.AreaChartData = function (data) {
        var areaData = [];
        for (var i = 0; i < data.length; i++) {
            areaData[i] = {};
            areaData[i].label = data[i].RFRetrieveDate;
            areaData[i].orders = data[i].ULTotalOrders;
            areaData[i].amount = data[i].ULTotalAmount;
        }
        $scope.area = new Morris.Bar({
            element: 'revenue-chart',
            resize: true,
            data: areaData,
            xkey: 'label',
            parseTime: false,
            ykeys: ['amount'],
            yLabelFormat: function (y) { return 'P ' + y.toString(); },
            xLabelFormat: function (x) { var w = x.label.split('/'); return w[1] + '/' + w[2] + '/' + w[0]; },
            labels: ['Amount'],
            lineColors: ['#a0d0e0'],
           // behaveLikeLine: true,
            hideHover: 'auto',

            smooth: false //No Curve
        });
    }

    initialize();
})