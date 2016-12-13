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

    $scope.reportTranslog = function () {
        viewModelHelper.navigateTo('Admin/reports');
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

    $scope.getUserProfile = function () {
        viewModelHelper.getUserProfile();
    }

    $scope.saveUserDetails = function () {
        viewModelHelper.saveUserDetails();
    }

    initialize();
});
