﻿adminModule.controller("prodViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, $timeout, viewModelHelper, DTOptionsBuilder, DTColumnDefBuilder, $route, filterFilter, $log) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    $scope.dataTableOpt = {
        "aLengthMenu": [[10, 50, 100, -1], [10, 50, 100, 'All']],
    };

    var initialize = function () {
        $scope.refreshProd();
        $scope.getAccounts();
        $scope.accountSelections = "";
    }

    $scope.refreshProd = function () {
        $scope.search = {};
        $scope.searchBy = "PMCode";

        viewModelHelper.apiGet('api/prod', null, function (result) {

            $scope.pageSize = 5;
            $scope.entryLimit = 50;

            $scope.product = result.data.detail;

            $scope.$watch('search[searchBy]', function () {
                $scope.filterList = filterFilter($scope.product, $scope.search);
                $scope.noOfPages = Math.ceil($scope.filterList.length / $scope.entryLimit);
                $scope.currentPage = 1;
            });
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

    //<!-------  Add Product
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

    $scope.SaveAddProduct = function (Item) {
        $scope.data = {};
        $scope.data.operation = 'add_product';
        $scope.data.payload = _payloadParser(Item);

        viewModelHelper.apiPost('api/prod', $scope.data, function (result) {
            var data = result.data;
            if (data.success === true) {
                resetFields();
                $('#AddProductModal').modal('hide');

                $scope.refreshProd();

                notif_success('Add Product', "Product " + Item.pmcode + " successully added!");

                viewModelHelper.saveTransaction(data.detail);
            } else {
                notif_error('Add Product', data.detail)
            }
        });
    }
    // End of Add Product -------->


    //<!-------  Product Mapping
    $scope.getAccounts = function () {

        $scope.data = {};
        $scope.data.operation = 'get_accounts';

        viewModelHelper.apiGet('api/prod', $scope.data, function (result) {
            $scope.maxAccount = result.data.detail.length;
            for (var i = 0; i < $scope.maxAccount; i++) {
                $scope.accountSelections += '<option value="' + result.data.detail[i].ATId + '">' + result.data.detail[i].ATDescription + '</option>';
            }
        });
    }

    $scope.getMapping = function (product) {
        var Item = {};
        Item.pmid = product.PMId;

        $scope.data = {};
        $scope.data.operation = 'get_mapping';
        $scope.data.payload = _payloadParser(Item);

        viewModelHelper.apiGet('api/prod', $scope.data, function (result) {
            var data = result.data;

            $('#MappingModalTable tbody').remove();
            $('#MappingModalTable').append('<tbody/>');

            for (var i = 0; i < data.detail.length; i++) {
                tr = $('<tr class="table-row-edit"/>');
                tr.append('<td><select id="selAcc_' + i + '" class="table-data-account"><option value="' + data.detail[i].PAAccount + '">' + data.detail[i].ATDescription + '</option>' + $scope.accountSelections + '</select></td>');
                tr.append('<td><input type="text" class="table-data-code" value="' + data.detail[i].PACode + '" maxlength="20"/></td>');
                tr.append('<td class="table-data-delete"><input type="hidden" class="hidden-status" value="1"/><img src="../Images/master/delete.png" class="assign-delete-img" title="Remove item" align="center"/><img src="../Images/master/restore.png" title="Restore item" align="center" class="assign-restore-img" style="display:none;"/></td>');
                $('#MappingModalTable tbody').append(tr);
                $('select#selAcc_' + i + ' option').each(function () {
                    $(this).siblings("[value='" + this.value + "']").remove();
                });

                $scope.rowControl();
            }

            $scope.modalRows = $('#MappingModalTable tbody tr').length;
            $('#spn_mappmcode').text(product.PMCode);
            $('#txt_mappmid').val(product.PMId);

            $('#ProductMappingModal').modal('show');
        });
    }

    $scope.addModalRow = function () {
        $scope.rowNum = $('#MappingModalTable tbody tr').length;

        if ($scope.rowNum < $scope.maxAccount) {
            tr = $('<tr class="table-row-edit new" style="display: none;"/>');
            tr.append('<td><select class="table-data-account">' + $scope.accountSelections + '</select></td>');
            tr.append('<td><input type="text" class="table-data-code" maxlength="20"/></td>');
            tr.append('<td class="table-data-delete"><input type="hidden" class="hidden-status" value="1"/><img src="../Images/master/delete.png" class="assign-delete-img" title="Remove item" align="center"/><img src="../Images/master/restore.png" title="Restore item" align="center" class="assign-restore-img" style="display:none;"/></td>');
            $('#MappingModalTable tbody').append(tr);

            $('.new').fadeIn(500, function () {
                $('tr.table-row-edit').removeClass("new");
            });

            $scope.modalRows = $('#MappingModalTable tbody tr').length;

            $scope.rowControl();

        } else {
            notif_warning('Product Mapping', 'Maximum number of accounts already reached!');
        }
    }

    $scope.rowControl = function(){
        $('.assign-delete-img').click(function () {
            if ($(this).closest('tr').find('.table-data-code').val() == '') {
                $(this).closest('tr').fadeOut(200, function () {
                    $(this).closest('tr').remove();
                });
            } else {
                $(this).hide();
                $(this).closest('tr').find('.assign-restore-img').show();
                $(this).parent('td').find('.hidden-status').val(0);
                $(this).closest('tr').css('background-color', '#f9dede');
            }
        });

        $('.assign-restore-img').click(function () {
            $(this).hide();
            $(this).closest('tr').find('.assign-delete-img').show();
            $(this).parent('td').find('.hidden-status').val(1);
            $(this).closest('tr').css('background-color', '');
        });
    }

    $scope.ValidateSaveMaping = function () {
        if ($("#MappingModalTable tbody tr").length > 0) {
            var gs = 0;
            var empty = 0;
            var duplicate = 0;

            var Items = [];

            var $rows = $("#MappingModalTable tbody tr").each(function (index) {
                Items[index] = {};
                Items[index]["pmid"] = $('#txt_mappmid').val();
                Items[index]["pmcode"] = $('#spn_mappmcode').text();
                Items[index]["acctype"] = $(this).find('.table-data-account').val();
                Items[index]["assignedcode"] = $(this).find('.table-data-code').val();
                Items[index]["Status"] = $(this).find('.hidden-status').val();
            });


            for (var i = 0; i < Items.length; i++) {
                if (Items[i].assignedcode == null || Items[i].assignedcode == 'NaN' || Items[i].assignedcode == '') {
                    empty++;
                }
                for (var v = i + 1; v < Items.length; v++) {
                    if (Items[i].acctype == Items[v].acctype && Items[i].assignedcode == Items[v].assignedcode && Items[i].Status == Items[v].Status && Items[i].Status == '1') {
                        duplicate++;
                    }
                }
            }

            if (empty > 0) {
                notif_warning('Product Mapping', 'Please fill all the required fields.');
            }
            else if (duplicate > 0) {
                notif_error('Product Mapping', 'Duplicate Account type found.');
            }
            else {
                $scope.saveAssignment(Items);
            }
        } else {
            notif_info('Product Mapping', 'Nothing to save!');
            $('#ProductMappingModal').modal('hide');
        }
    }

    $scope.saveAssignment = function (Items) {

        $scope.data = {};
        $scope.data.operation = 'update_mapping';
        $scope.data.payload = _payloadParser(Items);

        viewModelHelper.apiPost('api/prod', $scope.data, function (result) {
            var data = result.data;
            if (data.success === true) {
                $('#ProductMappingModal').modal('hide');
                if (data.detail[0].changes != 'NC') { //No Changes 
                    notif_success('Product Mapping', "Successully updated!");
                    viewModelHelper.saveTransaction(data.detail);
                } else {
                    notif_info('Product Mapping', 'No changes made.');
                }
            } else {
                notif_error('Product Mapping', data.detail)
            }
        });
    }
    // End of Product Mapping -------->

    //<!-------  Product Batch Mapping
    $scope.showBatchModal = function () {

        $('.upload-image').empty();
        $('.dropzone').css('display', 'block');
        $('.upload-image').css('display', 'none');
        $('.progress-bar').remove();
        $('.progress').css('display', 'none');
        $('.batch-log').css('display', 'none');
        $('.batch-log table tbody tr td').remove();

        $('.batchAssignModal').modal('show');
    }

    $scope.fileUpload = function (files) {
        if (files.length == 1) {
            if (files[0].name.split('.').pop().toLowerCase() == 'csv') {
                if (files[0].size <= 1048576) {
                    var formData = new FormData();
                    formData.append("file", files[0]);
                    viewModelHelper.apiFileUpload('api/FileUpload', formData, function (result) {
                        var data = result.data;
                        if (data.success == true) {
                            notif_info('Batch Mapping', 'Reading you file...');

                            var Item = {};
                            Item.fileName = data.detail;

                            $scope.data = {};
                            $scope.data.operation = 'read_csv';
                            $scope.data.payload = _payloadParser(Item);

                            viewModelHelper.apiGet('api/FileUpload', $scope.data, function (result) {
                                var data = result.data;
                                if (data.success == true) {
                                    //console.log(data.filecontent);
                                    viewModelHelper.readAssign(data.filecontent, 'prod');
                                }
                                else {
                                    notif_error('Batch Mapping', data.detail);
                                }

                            });


                        } else {
                            notif_error('Batch Mapping', data.detail);
                        }
                    });
                } else {
                    notif_error('Batch Mapping', 'Maximim file size has been reached! Must be less than 1MB.');
                }
            } else {
                notif_error('Batch Mapping', 'Invalid file type. Must be a CSV file!');
            }
        }
        else if (files.length > 1) {
            notif_warning('Batch Mapping', 'You must drag one(1) CSV file at a time!');
        }
        else {
            return false;
        }
    }

    $scope.ondrop = function (files) {
        $scope.fileUpload(files);
    };
    // End of Product Batch Mapping -------->

    $scope.clearSearch = function () {
        $scope.search = {};
    }

    initialize();
}).filter('start', function () {
    return function (input, start) {
        if (!input || !input.length) { return; }

        start = +start;
        return input.slice(start);
    };
});