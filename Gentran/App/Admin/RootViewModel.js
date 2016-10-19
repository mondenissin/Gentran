adminModule.controller("rootViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, viewModelHelper, $route) {

    // This is the parent controller/viewmodel for 'orderModule' and its $scope is accesible
    // down controllers set by the routing engine. This controller is bound to the Order.cshtml in the
    // Home view-folder.
    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    $scope.flags = { shownFromList: false };

    var initialize = function () {
        //$scope.pageHeading = "User Section";
    }

    $scope.dashBoardv1 = function () {
        viewModelHelper.navigateTo('Admin/Dashboard');
    }

    $scope.userList = function () {
        viewModelHelper.navigateTo('Admin/Users');
    }

    $scope.userCards = function () {
        viewModelHelper.navigateTo('Admin/Users-Cards');
    }

    $scope.prodList = function () {
        viewModelHelper.navigateTo('Admin/Products');
    }

    $scope.custList = function () {
        viewModelHelper.navigateTo('Admin/Customers');
    }

    $scope.logList = function () {
        viewModelHelper.navigateTo('Admin/TransactionLogs');
    }
    
    $scope.connections = function () {
        viewModelHelper.navigateTo('Admin/Connections');
    }


    $scope.saveUserDetails = function () {

        var ret = UserProfileValidate();

        if (ret.valid === true) {
            $scope.saveProfile(ret.items);
        }
    }

    $scope.saveProfile = function (values) {
        $scope.data = {};
        $scope.data.operation = 'save_user';
        $scope.data.payload = _payloadParser(values);

        viewModelHelper.apiPut('api/userlist', $scope.data, function (result) {
            var data = result.data;
            if (data.success === true) {
                $('#editUserModal').modal('hide');
                resetFields();
                $route.reload();
                notif_success('User Profile Update', 'User successfully updated!');

                viewModelHelper.saveTransaction(data.detail);
            } else {
                notif_warning('User Profile Update', data.detail)
            }
        });
    }

    initialize();
});
