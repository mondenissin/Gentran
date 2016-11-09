adminModule.controller("dashViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, $timeout, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    $scope.totalOrders = 9999;
    $scope.totalSKUs = 9999;
    $scope.totalCustomers = 9999;
    $scope.totalIncome = $scope.totalCustomers.toLocaleString();

    var initialize = function () {
        $scope.refreshDash();
    }

    $scope.refreshDash = function () {
        
    }
   
    $scope.tools = function (proc, $event) {
        var selected;
        var area = $('#message-text');
        
        if (area[0] == $(window.getSelection().anchorNode)[0] || area[0] == $(window.getSelection().anchorNode)[0].parentElement) {
            selected = window.getSelection().anchorNode.textContent.substring(window.getSelection().extentOffset, window.getSelection().anchorOffset);
            
            switch (proc) {
                case 'bold':
                    area.text("Not Functioning");
                    break;
                case 'italic':
                    area.text("Not Functioning");
                    break;
                default:
                    alert("asd");
            }
        }
    }
    initialize();
})