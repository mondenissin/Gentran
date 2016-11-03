retrieveModule.controller("rootViewModel", function ($scope, retrieveService, $http, $q, $routeParams, $window, $location, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.retrieveService = retrieveService;

    $scope.flags = { shownFromList: false };

    var initialize = function () {
        //$scope.pageHeading = "User Section";
    }

    $scope.smFile = function () {
        viewModelHelper.navigateTo('Retrieve/SM');
    }

    $scope.superFile = function () {
        viewModelHelper.navigateTo('Retrieve/Super8');
    }

    $scope.nccFile = function () {
        viewModelHelper.navigateTo('Retrieve/NCC');
    }
    initialize();
});
