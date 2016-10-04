adminModule.controller("prodViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    var initialize = function () {
        $scope.refreshProd();
    }

    $scope.refreshProd = function () {
        $scope.search = {};
        $scope.searchBy = "PMCode";

        viewModelHelper.apiGet('api/prod', null, function (result) {
            $scope.product = result.data.detail;
        });
    }
    $scope.showProduct = function (product) {
        alert('You selected ' + product.PMCode + ' ' + product.PMDescription);
    }

    initialize();
});