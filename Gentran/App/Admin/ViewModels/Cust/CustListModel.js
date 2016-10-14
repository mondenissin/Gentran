adminModule.controller("custViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, viewModelHelper, DTOptionsBuilder, DTColumnDefBuilder,$route) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    var initialize = function () {
        $scope.refreshCust();
    }
    $scope.currentPage = 1, $scope.numPerPage = 50, $scope.maxSize = 5;

    $scope.refreshCust = function () {
        $scope.search = {};
        $scope.searchBy = "CMCode";

        viewModelHelper.apiGet('api/cust', null, function (result) {
            
            $scope.customersPerPage = result.data.detail;

            $scope.numPages = function () {
                return Math.ceil($scope.customersPerPage.length / $scope.numPerPage);
            };

            $scope.$watch('currentPage + numPerPage', function () {
                var begin = (($scope.currentPage - 1) * $scope.numPerPage),
                  end = begin + $scope.numPerPage;

                $scope.customers = $scope.customersPerPage.slice(begin, end);
            });
        }); 

        $scope.dtOptions = DTOptionsBuilder.newOptions();
        $scope.dtColumnDefs = [
           DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
        ];
    }

    //<----- View/Edit Customer
    $scope.ViewCustomerDetails = function (customer) {
        $scope.ResetEditFields();
        $('#txt_editcustid').val(customer.CMId);
        $('#txt_editcustcode').val(customer.CMCode);
        $('#txt_editcustname').val(customer.CMDescription);
        $('#ddl_editcustarea').val(customer.CMArea);
        $('#ddl_editcuststatus').val(customer.CMStatus);

        $('#EditCustomerModal').modal('show');
    }

    $scope.EditCustomer = function () {
        $('.btn_editcust').hide();
        $('.btn_savecust').show();
        $('#txt_editcustcode').attr('disabled', false);
        $('#txt_editcustname').attr('disabled', false);
        $('#ddl_editcustarea').attr('disabled', false);
        $('#ddl_editcuststatus').attr('disabled', false);
    }

    $scope.ValidateEditCustomer = function () {
        var Items = {};
        Items.cmID = parseInt($('#txt_editcustid').val());
        Items.cmCode = parseInt($('#txt_editcustcode').val());
        Items.cmName = $('#txt_editcustname').val().replace(/\'+/gi,'\'\'');
        Items.cmArea = $('#ddl_editcustarea').val();
        Items.cmStat = $('#ddl_editcuststatus').val().trim();

        if (isNaN(Items.cmID) || isNaN(Items.cmCode) || Items.cmName == '' || Items.cmArea == null || Items.cmStat == '') {
            notif_warning('Add Customer', 'Please fill all the required fields!');
        } else {
            $scope.SaveEditCustomer(Items);
        }
    }

    $scope.SaveEditCustomer = function (Item) {
        $scope.data = {};
        $scope.data.operation = 'save_edit_customer';
        $scope.data.payload = _payloadParser(Item);

        viewModelHelper.apiPut('api/cust', $scope.data, function (result) {
            var data = result.data;
            if (data.success === true) {
                resetFields();
                $('#EditCustomerModal').modal('hide');

                setTimeout(function () {
                    $route.reload();
                }, 1000);

                notif_success('Edit Customer', 'Customer successfully updated!');

                viewModelHelper.saveTransaction(data.detail)
            } else {
                notif_error('Edit Customer', data.detail)
            }
        });
    }

    $scope.ResetEditFields = function () {
        $('.btn_editcust').show();
        $('.btn_savecust').hide();
        $('#txt_editcustcode').attr('disabled', true);
        $('#txt_editcustname').attr('disabled', true);
        $('#ddl_editcustarea').attr('disabled', true);
        $('#ddl_editcuststatus').attr('disabled', true);
    }
    // End of View/Edit Customer-----/>

    //<----- Add Customer
    $scope.ShowAddCustomerModal = function () {
        $('#AddCustomerModal').modal('show');
    }

    $scope.ValidateAddCustomer = function () {
        var Items = {};
        Items.cmCode = parseInt($('#txt_addcustcode').val());
        Items.cmName = $('#txt_addcustname').val().replace(/\'+/gi, '\'\'');
        Items.cmArea = $('#ddl_addcustarea').val();
        Items.cmStat = $('#ddl_addcuststatus').val().trim();
        
        if (Items.cmCode == 0 || isNaN(Items.cmCode) || Items.cmName == '' || Items.cmArea == null || Items.cmStat == '') {
            notif_warning('Add Customer', 'Please fill all the required fields!');
        } else {
            $scope.SaveAddCustomer(Items);
        }
    }

    $scope.SaveAddCustomer = function (Item) {
        $scope.data = {};
        $scope.data.operation = 'add_customer';
        $scope.data.payload = _payloadParser(Item);

        viewModelHelper.apiPost('api/cust', $scope.data, function (result) {
            var data = result.data;
            if (data.success === true) {
                resetFields();
                $('#AddCustomerModal').modal('hide');

                setTimeout(function () {
                    $route.reload();
                }, 1000);

                notif_success('Add Customer', 'Customer successfully added!');

                viewModelHelper.saveTransaction(data.detail)
            } else {
                notif_error('Add Customer', data.detail)
            }
        });
    }
    // End of Add Customer-----/>
    
    initialize();
});