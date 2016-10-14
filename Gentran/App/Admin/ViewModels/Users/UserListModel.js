adminModule.controller("userViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, viewModelHelper, DTOptionsBuilder, DTColumnDefBuilder,$route) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    var initialize = function () {
        $scope.refreshUser();
    }

    $scope.refreshUser = function () {
        $scope.search = {};
        $scope.searchBy = "UMId";

        viewModelHelper.apiGet('api/userlist', null, function (result) {
            $scope.people = result.data.detail;
        });

        $scope.dtOptions = DTOptionsBuilder.newOptions();
        $scope.dtColumnDefs = [
           DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
        ];
    }
    $scope.showPerson = function (person) {
        console.log('You selected ' + person.UMFirstName + ' ' + person.UMLastName);
    }

    $scope.UserUpdate = function (operation,umid) {

        var Item = {};
        Item.umid = umid;

        $scope.data = {};
        $scope.data.operation = operation;
        $scope.data.payload = _payloadParser(Item);

        viewModelHelper.apiPut('api/userlist', $scope.data, function (result) {
            var data = result.data;
            if (data.success === true) {

                if (operation == "reset_user") {
                    notif_success('User Update', 'User has been reset!');
                }
                else if (operation == "deactivate_user") {
                    notif_success('User Update', 'User has been deactivated!');
                }

                $('#userOptionsModal').modal('hide');

                viewModelHelper.saveTransaction(data.detail);

                $scope.refreshUser();

            } else {
                notif_error('Error!',data.detail)
            }

        });
    }

    $scope.ShowConfirmModal = function (operation, person) {

        if (operation == "reset") {
            $('.confirm-header-message').text('Reset User Credentials');
            $('.confirmMessage').text('Do you wish to reset ' + person.UMFirstname + ' ' + person.UMLastname + '?');
        }
        else {
            $('.confirm-header-message').text('Deactivate User');
            $('.confirmMessage').text('Do you wish to deactivate ' + person.UMFirstname + ' ' + person.UMLastname + '?');
        }

        $('.txt_hidden_umid').val(person.UMId);
        $('.txt_hidden_operation').val(operation);

        $('#userOptionsModal').modal('show');
    }

    $scope.ActionSelected = function () {
        var umid = $('.txt_hidden_umid').val();
        var operation = $('.txt_hidden_operation').val();

        if (operation == "reset") {
            $scope.UserUpdate('reset_user', umid);
        }
        else {
            $scope.UserUpdate('deactivate_user', umid);
        }
    }

    $scope.addUser = function () {
        $('#AddUserModal').modal('show');
    } 

    $scope.saveAddUser = function () {
        var ret = ValidateAddUser();

        if (ret.valid == true) {
            $scope.data = {};
            $scope.data.operation = 'add_user';
            $scope.data.payload = _payloadParser(ret.items);

            viewModelHelper.apiPost('api/userlist', $scope.data, function (result) {
                var data = result.data;
                if (data.success === true) {
                    resetFields();                      
                    $('#AddUserModal').modal('hide');

                    setTimeout(function () {
                        $route.reload();
                    }, 300);

                    notif_success('Add User', 'User Successfully added!');

                    viewModelHelper.saveTransaction(data.detail);

                } else {
                    notif_error('Add User', data.detail)
                }
            });
        }
    }

	$scope.userDetails = function (person) {
        $('#txt_edituserid').val(person.UMId);
        $('#txt_edituname').val(person.UMUsername);
        $('#txt_editnname').val(person.UMNickname);
        $('#txt_editfname').val(person.UMFirstname);
        $('#txt_editmname').val(person.UMMiddlename);
        $('#txt_editlname').val(person.UMLastname);
        $('#txt_editemail').val(person.UMEmail);

        if (person.UMImage == '' || person.UMImage.length < 100 || person.UMImage == null) {
            $('#imgEditProf').remove();
            $('.link-user-imgprof').text('');
            $('.link-user-imgprof').css('background-color', '#282E6C');
            $('.link-user-imgprof').css('line-height', '120px');
            $('.link-user-imgprof').text(person.UMFirstname.charAt(0) + person.UMLastname.charAt(0));
        }
        else {
            $('#imgEditProf').remove();
            $('.link-user-imgprof').text('');
            $('.link-user-imgprof').css('background-color', '');
            $('.link-user-imgprof').css('line-height', '');
            $('.link-user-imgprof').append('<img ID="imgEditProf" src="img/loading/load.gif" style="border:none;"/>');
            $('#imgEditProf').prop('src', person.UMImage);
        }

        $('#editUserModal').modal('show');
    }
     
    initialize();
});
