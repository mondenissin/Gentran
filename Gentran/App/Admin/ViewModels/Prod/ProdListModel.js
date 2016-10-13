adminModule.controller("prodViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location,$timeout, viewModelHelper, DTOptionsBuilder, DTColumnDefBuilder) {

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

        $scope.dtOptions = DTOptionsBuilder.newOptions();
        $scope.dtColumnDefs = [
           DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
        ];
    }
    $scope.showProduct = function (product) {
        alert('You selected ' + product.PMCode + ' ' + product.PMDescription); 
    }

    $scope.getDetails = function (p) {
        $('#txt_prodID').val(p.PMId);
        $('#txt_prodCode').val(p.PMCode);
        $('#txt_prodDescription').val(p.PMDescription);
        $('#txt_prodBarcode').val(p.PMBarcode);
        $('#txt_prodType').val(p.PMCategory);
        $('#txt_prodStatus').val(p.PMStatus);

        $('#ProductDetailsModal').modal('show');
    }

    $scope.addProduct = function () {
        $('#AddProductModal').modal('show');
    }

    $scope.dataTableOpt = {
        "aLengthMenu": [[10, 50, 100, -1], [10, 50, 100, 'All']],
    };
    initialize();
});