adminModule.controller("connectionViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, viewModelHelper,DTColumnDefBuilder, DTOptionsBuilder, filterFilter) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    var initialize = function () {
        $scope.refreshConn();
    }


    $scope.refreshConn = function () {
        $scope.search = {};
        $scope.searchBy = "CSAccount";

        viewModelHelper.apiGet('api/conn', null, function (result) {
            $scope.pageSize = 5;
            $scope.entryLimit = 50;

            $scope.transactions = result.data.detail;

            $scope.$watch('search[searchBy]', function () {
                $scope.filterList = filterFilter($scope.transactions, $scope.search);
                $scope.noOfPages = Math.ceil($scope.filterList.length / $scope.entryLimit);
                $scope.currentPage = 1;
            });
        });
    }

    $scope.dtOptions = DTOptionsBuilder.newOptions();
    $scope.dtColumnDefs = [
       DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
    ];

    $scope.addconnModal = function () {
        $('#addConnModal').modal('show');
    }

    $scope.saveAddConn = function () {
        var Items = {};
        Items.CSAccount = $('#txt_addacct').val();
        Items.CSHost = $('#txt_addhost').val();
        Items.CSPort = $('#txt_addport').val();
        Items.CSUserName = $('#txt_adduname').val();
        Items.CSPassword = $('#txt_addpass').val();

        $scope.data = {};
        $scope.data.operation = 'add_connection';
        $scope.data.payload = _payloadParser(Items);

        viewModelHelper.apiPost('api/conn', $scope.data, function (result) {
            var data = result.data;
            if (data.success == true) {
                $('#addConnModal').modal('hide');

                $scope.refreshConn();

                notif_success('Add Connection', 'Connection successfully added!');

            } else {
                notif_error('Add Connection', data.detail)
            }
        });
    }

    $scope.editconnModal = function (con) {
        $('#txt_outlet').text(con.CSAccount);
        $('#txt_hiddenid').val(con.CSId);
        $('#txt_edithost').val(con.CSHost);
        $('#txt_editport').val(con.CSPort);
        $('#txt_edituname').val(con.CSUserName);
        $('#txt_editpass').val(con.ftppassword);

        $('#editConnModal').modal('show');
    }

    $scope.saveEditConn = function (connection) {
        var Items = {};
        Items.CSId = $('#txt_hiddenid').val();
        Items.CSHost = $('#txt_edithost').val();
        Items.CSPort = $('#txt_editport').val();
        Items.CSUserName = $('#txt_edituname').val();
        Items.CSPassword = $('#txt_editpass').val();
        
        $scope.data = {};
        $scope.data.operation = 'edit_connection';
        $scope.data.payload = _payloadParser(Items);

        viewModelHelper.apiPut('api/conn', $scope.data, function (result) {
            var data = result.data;
            if (data.success == true) {
                $('#editConnModal').modal('hide');

                $scope.refreshConn();

                notif_success('Edit Connection', 'Connection successfully saved!');

            } else {
                notif_error('Edit Connection', data.detail)
            }
        });
    }

    $scope.clearSearch = function () {
        $scope.search = {};
    }

    initialize();
}).filter('start', function () {
    return function (input, start) {
        if (!input || !input.length) { return; }

        start = +start;
        return input.slice(start);
    };
});