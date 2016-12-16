var retrieveModule = angular.module('retrieve', ['common', 'sly', 'datatables', 'ui.bootstrap'])
    .config(function ($routeProvider, $locationProvider) {
        var _root = getRoot();
        $routeProvider.when(_root + 'Retrieve/SM', { templateUrl: _root + 'App/Retrieve/Views/RetrieveMaster/retrieve.html', controller: 'retrieveViewModel' });
        $routeProvider.when(_root + 'Retrieve/Super8', { templateUrl: _root + 'App/Retrieve/Views/RetrieveMaster/retrieve.html', controller: 'retrieveViewModel' });
        $routeProvider.when(_root + 'Retrieve/NCC', { templateUrl: _root + 'App/Retrieve/Views/RetrieveMaster/retrieve.html', controller: 'retrieveViewModel' });
        $routeProvider.when(_root + 'Retrieve/Puregold', { templateUrl: _root + 'App/Retrieve/Views/RetrieveMaster/retrieve.html', controller: 'retrieveViewModel' });
        $routeProvider.when(_root + 'Retrieve/Waltermart', { templateUrl: _root + 'App/Retrieve/Views/RetrieveMaster/retrieve.html', controller: 'retrieveViewModel' });
        $routeProvider.when(_root + 'Retrieve/Ultramega', { templateUrl: _root + 'App/Retrieve/Views/RetrieveMaster/retrieve.html', controller: 'retrieveViewModel' });
        $routeProvider.otherwise({ redirectTo: _root + 'Retrieve' });
        $locationProvider.html5Mode({
            enabled: true,
            requireBase: false
        });
    });


retrieveModule.run(function (DTDefaultOptions) {
    // Display 50 items per page by default
    DTDefaultOptions.setDisplayLength(-1);
    var cDom = "<'row dt-info'<'col-sm-5'><'col-sm-7'>>" +
            "<'row'<'col-sm-12 page-table'tr>>" +
            "<'row'<'col-sm-5'><'col-sm-7'>>";
    DTDefaultOptions.setOption('dom', cDom);
});

retrieveModule.factory('retrieveService', function ($rootScope, $http, $q, $location, viewModelHelper) { return MyApp.retrieveService($rootScope, $http, $q, $location, viewModelHelper); });

(function (myApp) {
    var retrieveService = function ($rootScope, $http, $q, $location, viewModelHelper) {

        var self = this;
        var globalPause = false;

        self.setPause = function (data) {
            globalPause = data;
        };

        self.getPause = function (data) {
            return globalPause;
        }

        //self.orderId = 0;
         
        return this;
    };
    myApp.retrieveService = retrieveService;
}(window.MyApp));
