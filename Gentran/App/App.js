﻿function getRoot() {
    return MyApp.rootPath;
}

var commonModule = angular.module('common', ['ngRoute']);
var mainModule = angular.module('main', ['common']);

commonModule.factory('viewModelHelper', function ($http, $q, $window, $location) { return MyApp.viewModelHelper($http, $q, $window, $location); });



(function (myApp) {
    var viewModelHelper = function ($http, $q, $window, $location) {
        
        var self = this;

        self.modelIsValid = true;
        self.modelErrors = [];

        self.resetModelErrors = function () {
            self.modelErrors = [];
            self.modelIsValid = true;
        };

        self.apiGet = function (uri, data, success, failure) {
            self.modelIsValid = true;
            $http.get(MyApp.rootPath + uri, { 'params': { 'value': data } })
                .then(function (result) {
                    success(result);
                }, function (result) {
                    if (failure !== null) {
                        failure(result);
                    }
                    else {
                        var errorMessage = result.status + ':' + result.statusText;
                        if (result.data !== null && result.data.Message !== null)
                            errorMessage += ' - ' + result.data.Message;
                        self.modelErrors = [errorMessage];
                        self.modelIsValid = false;
                    }
                });
        };

        self.apiPost = function (uri, data, success, failure) {
            self.modelIsValid = true;
            $http.post(MyApp.rootPath + uri, data)
                .then(function (result) {
                    success(result);
                }, function (result) {
                    if (failure !== null) {
                        failure(result);
                    }
                    else {
                        var errorMessage = result.status + ':' + result.statusText;
                        if (result.data !== null && result.data.Message !== null)
                            errorMessage += ' - ' + result.data.Message;
                        self.modelErrors = [errorMessage];
                        self.modelIsValid = false;
                    }
                });
        };

        self.apiDownloadXML = function (uri, data, success, failure) {
            self.modelIsValid = true;
            $http.get(MyApp.rootPath + uri, {
                'params': { 'value': data }, headers: { 'Content-Type': 'application/json' }
            }
                )
                .then(function (result) {
                    success(result);
                }, function (result) {
                    if (failure !== null) {
                        failure(result);
                    }
                    else {
                        var errorMessage = result.status + ':' + result.statusText;
                        if (result.data !== null && result.data.Message !== null)
                            errorMessage += ' - ' + result.data.Message;
                        self.modelErrors = [errorMessage];
                        self.modelIsValid = false;
                    }
                });
        };

        self.apiFileUpload = function (uri, data, success, failure) {
            self.modelIsValid = true;
            $http.post(MyApp.rootPath + uri, data, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
                })
                .then(function (result) {
                    success(result);
                }, function (result) {
                    if (failure !== null) {
                        failure(result);
                    }
                    else {
                        var errorMessage = result.status + ':' + result.statusText;
                        if (result.data !== null && result.data.Message !== null)
                            errorMessage += ' - ' + result.data.Message;
                        self.modelErrors = [errorMessage];
                        self.modelIsValid = false;
                    }
                });
        };

        self.apiPut = function (uri, data, success, failure) {
            self.modelIsValid = true;

            $http.put(MyApp.rootPath + uri, data, {
                  headers: {
                      'Content-Type': 'application/json'
                  },
                })
                .then(function (result) {
                    success(result);
                }, function (result) {
                    if (failure !== null) {
                        failure(result);
                    }
                    else {
                        var errorMessage = result.status + ':' + result.statusText;
                        if (result.data !== null && result.data.Message !== null)
                            errorMessage += ' - ' + result.data.Message;
                        self.modelErrors = [errorMessage];
                        self.modelIsValid = false;
                    }
                });
        };

        self.saveTransaction = function (transactionData) {
            self.apiPost('api/transactions', transactionData, function (result) {
                var data = result.data;
                if (data.success != true) {
                    for (var i = 0; i < data.detail.length; i++) {
                        if (data.detail[i].remarks != 'Successful') {
                            notif_error('Transaction Log', data.detail[i].remarks);
                        }
                    }
                }
                else {
                    for (var i = 0; i < data.detail.length; i++) {
                        if (data.detail[i].remarks != 'Successful') {
                            notif_error('Transaction Log', data.detail[i].remarks);
                        }
                    }
                }
            });

        }
        self.loadUserProfile = function () {
            var data = {};
            data.operation = 'get_user_profile';

            self.apiGet('api/master', data, function (result) {
                var data = result.data;
                for (var i = 0; i < data.detail.length; i++) {
                    $('.img_userProfPic').prop('src', data.detail[i].UMImage);
                    $('.lbl_userfullname').text(data.detail[i].UMFirstname + ' ' + data.detail[i].UMLastname);
                    $('.lbl_username').text(data.detail[i].UMUsername);
                    $('.lbl_userid').text(data.detail[i].UMId);
                    $('.lbl_usertype').text(data.detail[i].UTDescription);
                }
            });
        }

        self.readAssign = function (gen, type) {
            var storage = [];
            var ics = 1;
            for (var s = 0; s < gen.length; s++) {
                var th = gen[s].split(',');

                if (th[0] != "" && th[1] != "" && th[2] != "") {
                    storage[s] = th[0] + "," + th[1] + "," + th[2];
                }
            }
           // console.log(storage);
            self.SaveAssign(storage, type)
        }

        self.SaveAssign = function (dstorage, type) {
            var upitems = [];
            var chson = 0;
            var x = 1;
            loopR();
            function loopR() {
                if (dstorage[x] != null) {
                    var th = dstorage[x].split(',');
                    upitems[chson] = {};

                    if (type == "cust") {
                        upitems[chson].cmCode = th[0];
                        upitems[chson].caCode = th[1];
                        upitems[chson].acctype = th[2];
                    } else if (type == "prod") {
                        upitems[chson].pmcode = th[0];
                        upitems[chson].pacode = th[1];
                        upitems[chson].acctype = th[2];
                    }

                    chson++;
                    x++;
                    loopR();
                }
            }
            self.SavingAssignForReal(upitems, type);
        }

        self.SavingAssignForReal = function (upitems, type) {

            var url = type == "cust" ? "api/cust" : "api/prod";

            var rData = {};
            rData.operation = 'batch_mapping';
            rData.payload = _payloadParser(upitems);
            self.apiPost(url, rData, function (result) {
                var data = result.data;
                if (data.success === true) {

                    $('.upload-image').empty();
                    $('.dropzone').css('display', 'none');
                    $('.upload-image').css('display', 'block');
                    $('.progress').append('<div id="load" class="progress-bar progress-bar-success progress-bar-striped active" role="progressbar"><img src="../Images/master/check-white.png" class="img-check"/><div id="txtProgress" class="progress-bar-txt">Reading 1 of 1</div></div>');
                    $('.progress').css('display', 'block');
                    $('.batch-log').css('display', 'block');

                    fileLogo = '<div class="upload-image-icon">';
                    fileLogo += '<img src="../Images/files/csv.png">';
                    fileLogo += '</div>';
                    $('.upload-image').append(fileLogo);
                    $('.batch-log table tbody tr td').remove();

                    for (var i = 0; i < data.detail.length; i++) {
                        tr = '<tr>';
                        tr = tr + '<td>' + data.detail[i].MNC + '</td>';
                        tr = tr + '<td>' + data.detail[i].CODE + '</td>';
                        tr = tr + '<td>' + data.detail[i].ACCT + '</td>';
                        tr = tr + '<td>' + data.detail[i].REMARKS + '</td>';
                        tr = tr + '</tr>';
                        $('.batch-log table tbody').append(tr);
                    }
                    $(".progress-bar-striped").animate({
                        width: 100 + "%"
                    }, 1000, function () {
                        $('.img-check').css('display', 'block');
                        $('.progress-bar-txt').text("Read Successful!");
                        $('.progress-bar.active').css('animation', 'none');
                        $('input#fuBatch').val('');
                        $('.notifyjs-metro-info').closest('.notifyjs-wrapper').remove();
                    });
                } else {
                    $('input#fuBatch').val('');
                    $('.notifyjs-metro-info').closest('.notifyjs-wrapper').remove();
                    notif_error('Product Mapping', 'An error occured while reading your file. Please refresh your browser then try again.');
                }
            });
        }

        self.fileViewer = function (format, file, directory) {

            var screenLeft = window.screenLeft != undefined ? window.screenLeft : screen.left;
            var screentop = window.screenTop != undefined ? window.screenTop : screen.top;

            width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
            height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

            var w = '1100';
            var h = '600';
            var left = ((width / 2) - (w / 2)) + screenLeft;
            var top = ((height / 2) - (h / 2)) + screentop;

            if (format == "txt") {
                printWindow = window.open(host + 'ftp/sm/' + file + '', '', 'left=' + left + ',top=' + top + ',width=' + w + ',height=' + h + ',status=0');
                setTimeout(function () {
                    printWindow.print();
                    printWindow.close();
                }, 500);
            }
            else if (format == "pdf") {
                var strContent = "<html><head>";
                strContent = strContent + "<title>Invoice Printing</title>";
                strContent = strContent + "</head><body>";
                strContent = strContent + "<div class=\"print-wrapper\">";
                strContent = strContent + "<object data='" + host + directory + file + "' type='application/pdf' width='100%' height='100%'>";
                strContent = strContent + "alt : <a href='" + host + directory + file + "'>Your browser is not supported. Please click here to download.</a>";
                strContent = strContent + "</object>";
                strContent = strContent + "</div>";
                strContent = strContent + "</body>";
                strContent = strContent + "</html>";
                printWindow = window.open('', '', 'left=' + left + ',top=' + top + ',width=' + w + ',height=' + h + ',status=0');
                printWindow.document.write(strContent);
            } else if (format == "xml") {
                window.open(host + directory + file, "Test", "width=" + w + ",height=" + h + ",scrollbars=1,resizable=1");
            } else {
                var strContent = "<html><head>";
                strContent = strContent + "<title>Invoice Printing</title>";
                strContent = strContent + "</head><body>";
                strContent = strContent + "<div class=\"print-wrapper\">";
                strContent = strContent + "NOTE : <a href='" + host + directory + file + "'>Excel/CSV file not supported to view content. Please click here to download.</a>";
                strContent = strContent + "</object>";
                strContent = strContent + "</div>";
                strContent = strContent + "</body>";
                strContent = strContent + "</html>";
                printWindow = window.open('', '', 'left=' + left + ',top=' + top + ',width=' + w + ',height=' + h + ',status=0');
                printWindow.document.write(strContent);
            }
        };

        self.saveUserDetails = function () {

            var ret = UserProfileValidate();

            if (ret.valid === true) {
                self.saveProfile(ret.items);
            }
        }

        self.saveProfile = function (values) {
            self.data = {};
            self.data.operation = 'save_user';
            self.data.payload = _payloadParser(values);

            self.apiPut('api/userlist', self.data, function (result) {
                var data = result.data;
                if (data.success === true) {
                    $('#editUserModal').modal('hide');

                    resetFields();
                    notif_success('User Profile Update', 'User successfully updated!');

                    self.loadUserProfile();

                    self.saveTransaction(data.detail);

                    self.loadUserProfile();
                } else {
                    notif_warning('User Profile Update', data.detail)
                }
            });
        }

        self.getUserProfile = function () {

            var data = {};
            data.operation = 'view_user_profile';

            self.apiGet('api/master', data, function (result) {
                var data = result.data;
                for (var i = 0; i < data.detail.length; i++) {
                    $('#txt_edituserid').val(data.detail[i].UMId);
                    $('#txt_edituname').val(data.detail[i].UMUsername);
                    $('#txt_editnname').val(data.detail[i].UMNickname);
                    $('#txt_editfname').val(data.detail[i].UMFirstname);
                    $('#txt_editmname').val(data.detail[i].UMMiddlename);
                    $('#txt_editlname').val(data.detail[i].UMLastname);
                    $('#txt_editemail').val(data.detail[i].UMEmail);

                    if (data.detail[i].UMImage == '' || data.detail[i].UMImage.length < 100 || data.detail[i].UMImage == null) {
                        $('#imgEditProf').remove();
                        $('.link-user-imgprof').text('');
                        $('.link-user-imgprof').css('background-color', '#282E6C');
                        $('.link-user-imgprof').css('line-height', '120px');
                        $('.link-user-imgprof').text(data.detail[i].UMFirstname.charAt(0) + data.detail[i].UMLastname.charAt(0));
                    }
                    else {
                        $('#imgEditProf').remove();
                        $('.link-user-imgprof').text('');
                        $('.link-user-imgprof').css('background-color', '');
                        $('.link-user-imgprof').css('line-height', '');
                        $('.link-user-imgprof').append('<img ID="imgEditProf" src="img/loading/load.gif" style="border:none;"/>');
                        $('#imgEditProf').prop('src', data.detail[i].UMImage);
                    }

                    $('#editUserModal').modal('show');
                }
            });
        }

        self.goBack = function () {
            $window.history.back();
        };

        self.navigateTo = function (path) {
            $location.path(MyApp.rootPath + path);
        };

        self.refreshPage = function (path) {
            $window.location.href = MyApp.rootPath + path;
        };

        self.clone = function (obj) {
            return JSON.parse(JSON.stringify(obj));
        };
        self.getRootPath = function () {
            return myApp.rootPath;
        };

        self.loadUserProfile();

        return this;
    };

    
    myApp.viewModelHelper = viewModelHelper;
}(window.MyApp));
