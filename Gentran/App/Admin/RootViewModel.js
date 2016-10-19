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
                notif_success('User Profile Update', 'User successfully updated!');

                viewModelHelper.loadUserProfile();

                viewModelHelper.saveTransaction(data.detail);

                $route.reload();
            } else {
                notif_warning('User Profile Update', data.detail)
            }
        });
    }

    $scope.getUserProfile = function () {

        var data = {};
        data.operation = 'view_user_profile';

        viewModelHelper.apiGet('api/master', data, function (result) {
            var data = result.data;
            for (var i = 0; i < data.detail.length; i++) {
                $('#txt_edituserid').val(data.detail[i].UMId);
                $('#txt_edituname').val(data.detail[i].UMUsername);
                $('#txt_editnname').val(data.detail[i].UMNickname);
                $('#txt_editfname').val(data.detail[i].UMFirstname);
                $('#txt_editmname').val(data.detail[i].UMMiddlename);
                $('#txt_editlname').val(data.detail[i].UMLastname);
                $('#txt_editemail').val(data.detail[i].UMEmail);

                if (data.detail[i].UMImage == '' || data.detail[i].UMImage.length < 100 || data.detail[i].UMImage == null) {
                    $('#imgEditProf').remove();
                    $('.link-user-imgprof').text('');
                    $('.link-user-imgprof').css('background-color', '#282E6C');
                    $('.link-user-imgprof').css('line-height', '120px');
                    $('.link-user-imgprof').text(data.detail[i].UMFirstname.charAt(0) + data.detail[i].UMLastname.charAt(0));
                }
                else {
                    $('#imgEditProf').remove();
                    $('.link-user-imgprof').text('');
                    $('.link-user-imgprof').css('background-color', '');
                    $('.link-user-imgprof').css('line-height', '');
                    $('.link-user-imgprof').append('<img ID="imgEditProf" src="img/loading/load.gif" style="border:none;"/>');
                    $('#imgEditProf').prop('src', data.detail[i].UMImage);
                }

                $('#editUserModal').modal('show');
            }
        });


    }

    initialize();
});
