function getRoot() {
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
            $http.get(MyApp.rootPath + uri, data)
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

        self.apiPut = function (uri, data, success, failure) {
            self.modelIsValid = true;
            $http.put(MyApp.rootPath + uri, data, {
                  headers: {
                    'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8'
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

        return this;
    };
    myApp.viewModelHelper = viewModelHelper;
}(window.MyApp));
