adminModule.controller("userViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, viewModelHelper) {

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
    }
    $scope.showPerson = function (person) {
        console.log('You selected ' + person.UMFirstName + ' ' + person.UMLastName);
    }

    initialize();
});