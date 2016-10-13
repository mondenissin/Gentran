adminModule.controller("prodViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, $timeout, viewModelHelper, DTOptionsBuilder, DTColumnDefBuilder, $route) {

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

    $scope.ShowAddProductModal = function () {
        $('#AddProductModal').modal('show');
    }

    $scope.ValidateAddProduct = function () {

        var Items = {};
        Items.pmcode = $('#txt_addprodCode').val().trim();
        Items.pmbcode = $('#txt_addprodBarcode').val().trim();
        Items.pmdesc = $('#txt_addprodDescription').val().trim();
        Items.pmcategory = $('#ddl_addprodType').val().trim();
        Items.pmstatus = $('#ddl_addprodStatus').val().trim();

        if (Items.pmcode == '' || Items.pmbcode == '' || Items.pmdesc == '' || Items.pmcategory == '' || Items.pmstatus == '') {
            notif_warning('Add Product', 'Please fill all the required fields!');
        } else {
            $scope.SaveAddProduct(Items);
        }
    }

    //Saving Add Product
    $scope.SaveAddProduct = function (Item) {
        $scope.data = {};
        $scope.data.operation = 'add_product';
        $scope.data.payload = _payloadParser(Item);

        viewModelHelper.apiPost('api/prod', $scope.data, function (result) {
            var data = result.data;
            if (data.success === true) {
                resetFields();
                $('#AddProductModal').modal('hide');

                setTimeout(function () {
                    $route.reload();
                }, 300);

                notif_success('Add Product', "Product " + Item.pmcode + " successully added!");

                viewModelHelper.saveTransaction(data.detail);
            } else {
                notif_error('Add Product', data.detail)
            }
        });
    }

    $scope.dataTableOpt = {
        "aLengthMenu": [[10, 50, 100, -1], [10, 50, 100, 'All']],
    };
    initialize();
});