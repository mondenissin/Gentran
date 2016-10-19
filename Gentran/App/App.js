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
