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

        $scope.areaType = 'Daily';

        $scope.Items = null;
        $scope.filterText = "Date Covered: This Year";
        $scope.refreshDash($scope.Items);

        var date = new Date();
        var year = date.getFullYear();
        $('.txt_daterange').val(year + '/01/01-' + year + '/12/31');
    }

    $scope.toggleFilter = function (x, filt) {
        $(x).closest('div').find('.active').attr('disabled',false);
        $(x).closest('div').find('.active').addClass('btn-default').removeClass('btn-primary active');
        $(x).removeClass(' btn-default');
        $(x).addClass('btn-primary');
        $(x).addClass('active');
        $(x).attr('disabled', true);
        $scope.areaType = filt;

        $scope.setFilter();
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

    $scope.refreshAreaData = function (Items, callback) {
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
                $('#sales-chart').find('svg').show();
                $('#sales-chart').find('.morris-hover').css({ 'visibility': '' });
                callback(data);
            } else {
                $('#sales-chart').find('svg').hide();
                $('#sales-chart').find('.morris-hover').css({ 'visibility': 'hidden' });
                $scope.errorMessage = 'No data available.';
            }

            $scope.totalOrders = orders;
            $scope.totalSKUs = skus;
            $scope.totalQuantity = tskus;
            $scope.totalIncome = amount.toLocaleString();
        });
    }

    $scope.refreshDonutData = function (Items, callback) {
        viewModelHelper.apiGet('api/dash', Items, function (result) {
            var data = result.data;
            if (data.success == true) {
                $('#product-chart').find('svg').show();
                $('#product-chart').find('.morris-hover').css({ 'visibility': '' });
                callback(data);
            } else {
                $('#product-chart').find('svg').hide();
                $('#product-chart').find('.morris-hover').css({ 'visibility': 'hidden' });
                $scope.errorMessage = 'No data available.';
            }
        });
    }

    var ti;
    $scope.setFilter = function () {
        var filterArea = $('.ddl_area').val();
        var filterChain = $('.ddl_chain').val();
        var filterDate = $('.txt_daterange').val().split('-');

        var dFrom = filterDate[0].split('/');
        var dTo = filterDate[1].split('/');

        var sFrom = dFrom[1] + '-' + dFrom[2] + '-' + dFrom[0];
        var sTo = dTo[1] + '-' + dTo[2] + '-' + dTo[0];

        var operation;
        var Item = {};

        Item.dateFrom = sFrom + ' 00:00:00.000';
        Item.dateTo = sTo + ' 23:59:59.999';
        Item.cmArea = filterArea;
        Item.acctype = filterChain;
        Item.prefix = $scope.areaType;

        $scope.filterText = sFrom == sTo ? 'Date Covered: ' + sTo : 'Date Covered: ' + sFrom + '  to ' + sTo;

        function loadAreaChart() {
            operation = filterArea != '-1' && filterChain != '-1' ? "filter_area_dashboard_by_all" : filterArea != '-1' ? "filter_area_dashboard_by_area" : filterChain != '-1' ? "filter_area_dashboard_by_chain" : "filter_area_dashboard_by_date";

            $scope.Items = {};
            $scope.Items.operation = operation;
            $scope.Items.payload = _payloadParser(Item);

            $scope.refreshAreaData($scope.Items, function (data) {
                var areaData = [];
                for (var i = 0; i < data.detail.length; i++) {
                    areaData[i] = {};
                    areaData[i].label = data.detail[i].RFSubmitDate;
                    areaData[i].orders = data.detail[i].ULTotalOrders;
                    areaData[i].amount = data.detail[i].ULTotalAmount;
                }
                $scope.area.setData(areaData);
            });
        }

        function loadDonutChart() {
            operation = filterArea != '-1' && filterChain != '-1' ? "filter_donut_dashboard_by_all" : filterArea != '-1' ? "filter_donut_dashboard_by_area" : filterChain != '-1' ? "filter_donut_dashboard_by_chain" : "filter_donut_dashboard_by_date";
            $scope.Items = {};
            $scope.Items.operation = operation;
            $scope.Items.payload = _payloadParser(Item);

            $scope.refreshDonutData($scope.Items, function (data) {
                console.log(data);
                var donutData = [];
                for (var i = 0; i < data.detail.length; i++) {
                    donutData[i] = {};
                    donutData[i].label = data.detail[i].PCDescription;
                    donutData[i].value = data.detail[i].STotalQuantity;
                }
                $scope.donut.setData(donutData);
            });
        }

        $scope.errorMessage = 'Please wait.';

        loadAreaChart();
        loadDonutChart();

        clearInterval(ti);

        ti = setInterval(function () {
            loadAreaChart();
            loadDonutChart();
        }, 60000*5);
    }

    $scope.refreshDash = function (Items) {

        $scope.errorMessage = 'Please wait.';

        $scope.refreshAreaData(Items, function (data) {
            $scope.AreaChartData(data.detail);
        });

        $scope.data = {};
        $scope.data.operation = 'filter_donut_dashboard_by_default';

        $scope.refreshDonutData($scope.data, function (data) {
            $scope.DonutChartData(data.detail);
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
            areaData[i].label = data[i].RFSubmitDate;
            areaData[i].orders = data[i].ULTotalOrders;
            areaData[i].amount = data[i].ULTotalAmount;
        }
        $scope.area = new Morris.Bar({
            element: 'sales-chart',
            resize: true,
            data: areaData,
            xkey: 'label',
            parseTime: false,
            ykeys: ['amount'],
            preUnits: 'P ',
            labels: ['Amount'],
            lineColors: ['#a0d0e0'],
            // behaveLikeLine: true,
            hideHover: 'auto',
            smooth: false //No Curve
        });
    }

    $scope.DonutChartData = function (data) {
        var donutData = [];
        for (var i = 0; i < data.length; i++) {
            donutData[i] = {};
            donutData[i].label = data[i].PCDescription;
            donutData[i].value = data[i].STotalQuantity;
        }

        $scope.donut = new Morris.Donut({
            element: 'product-chart',
            resize: true,
            colors: ["#3c8dbc", "#f56954", "#00a65a", "#f0ad4e", "rgb(241, 130, 212)"],
            data: donutData,
            hideHover: 'auto'
        });
    }

    initialize();
})