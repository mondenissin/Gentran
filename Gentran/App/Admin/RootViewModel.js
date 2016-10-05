adminModule.controller("rootViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, viewModelHelper) {

    // This is the parent controller/viewmodel for 'orderModule' and its $scope is accesible
    // down controllers set by the routing engine. This controller is bound to the Order.cshtml in the
    // Home view-folder.
    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    $scope.flags = { shownFromList: false };

    var initialize = function () {
        //$scope.pageHeading = "User Section";
    }

    $scope.userList = function () {
        viewModelHelper.navigateTo('Admin/Userlist');
    }

    $scope.prodList = function () {
        viewModelHelper.navigateTo('Admin/Prodlist');
    }

    initialize();
});
