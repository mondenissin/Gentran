adminModule.controller("userViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, viewModelHelper, DTOptionsBuilder, DTColumnDefBuilder) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    var initialize = function () {
        $scope.refreshUser();
    }

    $scope.refreshUser = function () {
        $scope.search = {};
        $scope.searchBy = "UMFirstName";

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

    $scope.addUser = function () {
        $('#AddUserModal').modal('show');
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
