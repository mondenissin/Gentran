﻿adminModule.controller("custViewModel", function ($scope, adminService, $http, $q, $routeParams, $window, $location, viewModelHelper, DTOptionsBuilder, DTColumnDefBuilder, $route, filterFilter) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.adminService = adminService;

    var initialize = function () {
        $scope.refreshCust();
        $scope.getAccounts();
        $scope.accountSelections = "";
    }

    $scope.refreshCust = function () {
        $scope.search = {};
        $scope.searchBy = "CMCode";

        viewModelHelper.apiGet('api/cust', null, function (result) {
           
            $scope.pageSize = 5;
            $scope.entryLimit = 50;

            $scope.customers = result.data.detail;
            
            $scope.$watch('search[searchBy]', function () {
                $scope.filterList = filterFilter($scope.customers, $scope.search);
                $scope.noOfPages = Math.ceil($scope.filterList.length / $scope.entryLimit);
                $scope.currentPage = 1;
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

                $scope.refreshCust();

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

                $scope.refreshCust();

                notif_success('Add Customer', 'Customer successfully added!');

                viewModelHelper.saveTransaction(data.detail)
            } else {
                notif_error('Add Customer', data.detail)
            }
        });
    }
    // End of Add Customer-----/>
    //<----- Customer  Mapping
    $scope.getAccounts = function () {

        $scope.data = {};
        $scope.data.operation = 'get_accounts';

        viewModelHelper.apiGet('api/cust', $scope.data, function (result) {
            $scope.maxAccount = result.data.detail.length;
            for (var i = 0; i < $scope.maxAccount; i++) {
                $scope.accountSelections += '<option value="' + result.data.detail[i].ATId + '">' + result.data.detail[i].ATDescription + '</option>';
            }
        });
    }

    $scope.getMapping = function (customer) {
        var Item = {};
        Item.cmID = customer.CMId;

        $scope.data = {};
        $scope.data.operation = 'get_mapping';
        $scope.data.payload = _payloadParser(Item);

        viewModelHelper.apiGet('api/cust', $scope.data, function (result) {
            var data = result.data;

            $('#MappingModalTable tbody').remove();
            $('#MappingModalTable').append('<tbody/>');

            for (var i = 0; i < data.detail.length; i++) {
                tr = $('<tr class="table-row-edit"/>');
                tr.append('<td><select id="selAcc_' + i + '" class="table-data-account"><option value="' + data.detail[i].CAAccount + '">' + data.detail[i].ATDescription + '</option>' + $scope.accountSelections + '</select></td>');
                tr.append('<td><input type="text" class="table-data-code" value="' + data.detail[i].CACode + '" maxlength="20"/></td>');
                tr.append('<td class="table-data-delete"><input type="hidden" class="hidden-status" value="1"/><img src="../Images/master/delete.png" class="assign-delete-img" title="Remove item" align="center"/><img src="../Images/master/restore.png" title="Restore item" align="center" class="assign-restore-img" style="display:none;"/></td>');
                $('#MappingModalTable tbody').append(tr);
                $('select#selAcc_' + i + ' option').each(function () {
                    $(this).siblings("[value='" + this.value + "']").remove();
                });

                $scope.rowControl();
            }

            $scope.modalRows = $('#MappingModalTable tbody tr').length;
            $('#spn_mapcmcode').text(customer.CMCode);
            $('#txt_mapcmid').val(customer.CMId);

            $('#CustomerMappingModal').modal('show');
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

    $scope.rowControl = function () {
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
                Items[index]["cmID"] = $('#txt_mapcmid').val();
                Items[index]["cmCode"] = $('#spn_mapcmcode').text();
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
                notif_warning('Customer Mapping', 'Please fill all the required fields.');
            }
            else if (duplicate > 0) {
                notif_error('Customer Mapping', 'Duplicate Account type found.');
            }
            else {
                $scope.saveAssignment(Items);
            }
        } else {
            notif_info('Customer Mapping', 'Nothing to save!');
            $('#ProductMappingModal').modal('hide');
        }
    }

    $scope.saveAssignment = function (Items) {

        $scope.data = {};
        $scope.data.operation = 'update_mapping';
        $scope.data.payload = _payloadParser(Items);

        viewModelHelper.apiPost('api/cust', $scope.data, function (result) {
            var data = result.data;
            if (data.success === true) {
                $('#CustomerMappingModal').modal('hide');
                if (data.detail[0].changes != 'NC') { //No Changes 
                    notif_success('Customer Mapping', "Successully updated!");
                    viewModelHelper.saveTransaction(data.detail);
                } else {
                    notif_info('Customer Mapping', 'No changes made.');
                }
            } else {
                notif_error('Customer Mapping', data.detail)
            }
        });
    }
    // End of Customer Mapping -----/>

    //<----- Customer to User Assignment
    $scope.CustomerToUser = function (customer) {
        var Item = {};
        Item.cmID = customer.CMId;
        Item.cmCode = customer.CMCode;

        $('#spn_UAcustCode').text(customer.CMCode);
        $('#txt_UAcustID').val(customer.CMId);

        $scope.data = {};
        $scope.data.operation = 'get_userAssignment';
        $scope.data.payload = _payloadParser(Item);


        viewModelHelper.apiGet('api/cust', $scope.data, function (result) {
            var data = result.data;

            $('#ModalTable_UserAssignment tbody').remove();
            $('#ModalTable_UserAssignment').append('<tbody/>');

            var tr = '';

            for (var i = 0; i < data.detail.length; i++) {
                tr += '<tr class="user-assigned">';
                tr += '<td>' + data.detail[i].UMLastname + ', ' + data.detail[i].UMFirstname + '</td>';
                tr += '<td class="tbl-data-umid">' + data.detail[i].UAUser + '</td>';
                tr += '<td class="table-data-delete"><img src="../Images/master/delete.png" class="assign-delete-img" title="Remove item" align="center"/></td>';
                tr += '</tr>';
            }
            $('#ModalTable_UserAssignment tbody').append(tr);

            $scope.removeUser();
            $scope.getUserList(customer);

            $('#UserAssignmentModal').modal('show');
        });
    }

    $scope.getUserList = function (customer) {
        var assigneduser = '';

        $('#ModalTable_UserAssignment tbody tr').each(function () {
            $(this).find('.tbl-data-umid').each(function () {
                assigneduser += assigneduser == '' ? $(this).text() : ',' + $(this).text();
            });
        });


        $('#lstUsers option').remove();

        var Item = {};
        Item.cmID = customer.CMId;
        Item.cmCode = customer.CMCode;
        Item.keyword = assigneduser;

        $scope.data = {};
        $scope.data.operation = assigneduser == '' ? 'get_users' : 'get_unassigned_users';
        $scope.data.payload = _payloadParser(Item);

        viewModelHelper.apiGet('api/cust', $scope.data, function (result) {
            var data = result.data;
            if (data.success == true) {
                for (var i = 0; i < data.detail.length; i++) {
                    var sOption = '<option value="' + data.detail[i].UMId + '">' + data.detail[i].UMFirstname + ' ' + data.detail[i].UMLastname + '</option>';
                    $('#lstUsers').append(sOption);
                }
                $('#lstUsers').selectpicker('refresh');
            }
            else {
                notif_error('Uncaught error!','Please refresh your browser.')
            }
        });


    }

    $scope.addToTable = function () {

        var selected = $("#lstUsers option:selected");
        var umids = "";

        selected.each(function () {
            umids += (umids == "") ? $(this).val() : "," + $(this).val();
        });

        if (umids !== "") {

            var Item = {};
            Item.keyword = umids;

            $scope.data = {};
            $scope.data.operation = 'add_users_to_table';
            $scope.data.payload = _payloadParser(Item);

            viewModelHelper.apiGet('api/cust', $scope.data, function (result) {
                var data = result.data;
                if (data.success == true) {
                    for (var i = 0; i < data.detail.length; i++) {
                        var tr = $('<tr class="add-assigned"/>');
                        tr.append('<td>' + data.detail[i].UMLastname + ', ' + data.detail[i].UMFirstname + '<span class="pull-right" title="New">*</span></td>');
                        tr.append('<td class="tbl-data-umid">' + data.detail[i].UMId + '</td>');
                        tr.append('<td class="table-data-delete"><img src="../Images/master/delete.png" class="assign-delete-img" title="Remove item" align="center"/></td>');
                        $('#ModalTable_UserAssignment tbody').prepend(tr);
                    }
                    $scope.removeUser();
                }
                else {
                    notif_error('Uncaught error!', 'Please refresh your browser.')
                }
            });

        } else {
            notif_warning('User Assignment', 'You must select atleast one(1) user.');
        }
        $('.selectpicker').selectpicker('deselectAll');
    }

    $scope.removeUser = function () {
            $('.assign-delete-img').click(function () {
                if ($('#ModalTable_UserAssignment tbody tr').length > 1) {
                    $(this).closest('tr').fadeOut(300, function () {
                        $(this).remove();
                    });
                } else {
                    $('.notifyjs-wrapper').remove();
                    notif_error('Error', 'You must assign atleast one(1) user.');
                }
            });
    }

    $scope.ValidateUserAssignment = function () {
        if ($('#ModalTable_UserAssignment tbody tr:visible').length > 0) {
            var Item = [];
            $('#ModalTable_UserAssignment tbody tr:visible').each(function (index) {
                Item[index] = {};
                Item[index]["umid"] = $(this).find('.tbl-data-umid').text();
                Item[index]["cmID"] = $('#txt_UAcustID').val();
                Item[index]["cmCode"] = $('#spn_UAcustCode').text();
            });
            $scope.SaveUserAssignment(Item);
        } else {
            notif_info('User to Customer Assignment', 'No changes made.');
            $('#UserAssignmentModal').modal('hide');
        }

        
    }

    $scope.SaveUserAssignment = function (Item) {

        $scope.data = {};
        $scope.data.operation = 'update_user_assignment';
        $scope.data.payload = _payloadParser(Item);

        viewModelHelper.apiPost('api/cust', $scope.data, function (result) {
            var data = result.data;
            if (data.success == true) {
                $('#UserAssignmentModal').modal('hide');
                if (data.detail[0].changes != 'NC') { //No Changes 
                    notif_success('User to Customer Assignmen', "Successully updated!");
                    viewModelHelper.saveTransaction(data.detail);
                    $scope.refreshCust();
                } else {
                    notif_info('User to Customer Assignment', 'No changes made.');
                }
            } else {
                notif_error('User to Customer Assignment', data.detail)
            }
        });
    }

    // End of Customer to User Assignment -----/>

    //<!-------  Product Batch Customer
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
                                    viewModelHelper.readAssign(data.filecontent, 'cust');
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
    
    $scope.clearSearch = function() {
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