var adminModule = angular.module('admin', ['common', 'datatables', 'sly', 'ui.bootstrap', 'ng-fileDrop'])
.config(function ($routeProvider, $locationProvider) {
    var _root = getRoot();
    $routeProvider.when(_root + 'Admin/Users', { templateUrl: _root + 'App/Admin/Views/Users/UserList.html', controller: 'userViewModel' });
    $routeProvider.when(_root + 'Admin/Users-Cards', { templateUrl: _root + 'App/Admin/Views/Users/UserCards.html', controller: 'userViewModel' });
    $routeProvider.when(_root + 'Admin/Products', { templateUrl: _root + 'App/Admin/Views/Prod/ProdList.html', controller: 'prodViewModel' });
    $routeProvider.when(_root + 'Admin/Customers', { templateUrl: _root + 'App/Admin/Views/Cust/CustList.html', controller: 'custViewModel' });
    $routeProvider.when(_root + 'Admin/TransactionLogs', { templateUrl: _root + 'App/Admin/Views/TransLog/TransLogList.html', controller: 'translogViewModel' });
    $routeProvider.when(_root + 'Admin/Connections', { templateUrl: _root + 'App/Admin/Views/Connections/ConnectionList.html', controller: 'connectionViewModel' });
    $routeProvider.when(_root + 'Admin/Dashboard', { templateUrl: _root + 'App/Admin/Views/Dashboard/Dashboard.html', controller: 'dashViewModel' });
    $routeProvider.when(_root + 'Admin/reports/translog', { templateUrl: _root + 'App/Admin/Views/Reports/Transaction.html', controller: 'reportViewModel' });
    $routeProvider.otherwise({ redirectTo: _root + 'Admin' });
    $locationProvider.html5Mode({
        enabled: true,
        requireBase: false
    });    
});

adminModule.run(function (DTDefaultOptions) {
    // Display 50 items per page by default
    DTDefaultOptions.setDisplayLength(-1);
    var cDom = "<'row dt-info'<'col-sm-5'><'col-sm-7'>>" +
            "<'row'<'col-sm-12 page-table'tr>>" +
            "<'row'<'col-sm-5'><'col-sm-7'>>";
    DTDefaultOptions.setOption('dom', cDom);
});

adminModule.factory('adminService', function ($rootScope, $http, $q, $location, viewModelHelper) { return MyApp.adminService($rootScope, $http, $q, $location, viewModelHelper); });

(function (myApp) {
    var adminService = function ($rootScope, $http, $q, $location, viewModelHelper) {

        var self = this;

        //self.orderId = 0;
         
        return this;
    };
    myApp.adminService = adminService;
}(window.MyApp));

/*
appMainModule.controller("customerListViewModel", function ($scope, $http, $location) {
    $scope.search = {};
    $scope.searchBy = "CMId";

    $scope.people = [];
    var limit = 21;
    var index = 0;
    var input = {};
    input.id = "0331";

    $scope.headingCaption = "Customer List";

    $http({
        url: "/api/load_custList/?format=json",
        method: "POST",
        data: '{"operation": "get_user_information", payload: [' + JSON.stringify(input) + ']}',
    }).success(function (data) {
        $scope.peoples = data.detail;
        for (var x = 0; x < limit; x++) {
            $scope.people[x] = $scope.peoples[x];
        }
        //$scope.people = data.detail;
        //console.log($scope.peoples);
    });

    $scope.firstPage = function () {
        index = 0;
        for (var x = 0; x < limit; x++) {
            $scope.people[x] = $scope.peoples[index + x];
        }
        if (index <= 0)
            document.getElementById("btnPrev").disabled = true;

        if (index >= 0)
            document.getElementById("btnNext").disabled = false;
    }

    $scope.lastPage = function () {
        index = $scope.peoples.length - 1;
        for (var x = 0; x < limit; x++) {
            $scope.people[x] = $scope.peoples[(index - (limit - 1)) + x]
        }
        index -= 20;
        if (index >= $scope.peoples.length - 21)
            document.getElementById("btnNext").disabled = true;
        if (index > 0)
            document.getElementById("btnPrev").disabled = false;
    }
    $scope.nextPage = function (i) {
        if (index < $scope.peoples.length) {
            index += i;
            for (var x = 0 ; x < limit; x++) {
                $scope.people[x] = $scope.peoples[index + x];
            }
        }
        if (index >= $scope.peoples.length - 21)
            document.getElementById("btnNext").disabled = true;
        if (index > 0)
            document.getElementById("btnPrev").disabled = false;
    }
    $scope.prevPage = function (i) {
        if (index > 0) {
            index -= i;
            for (var y = 0; y < limit; y++) {
                $scope.people[y] = $scope.peoples[index + y];
            }
        }
        if (index <= 0)
            document.getElementById("btnPrev").disabled = true;

        if (index > 0)
            document.getElementById("btnNext").disabled = false;
    }

    $scope.showPerson = function (person) {
        alert('Youve just selected ' + person.CMId + ' ' + person.CMCode + ' ' + person.CMDescription);
    };
});*/
