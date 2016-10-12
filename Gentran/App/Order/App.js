var orderModule = angular.module('order', ['common'])
    .config(function ($routeProvider, $locationProvider) {
        var _root = getRoot();
        $routeProvider.when(_root + 'Order/OrderList', { templateUrl: _root + 'App/Order/Views/Orders/order.html', controller: 'orderViewModel' });
        $routeProvider.otherwise({ redirectTo: _root + 'Order/OrderList' });
        $locationProvider.html5Mode({
            enabled: true,
            requireBase: false
        });
    });
orderModule.factory('orderService', function ($rootScope, $http, $q, $location, viewModelHelper) { return MyApp.orderService($rootScope, $http, $q, $location, viewModelHelper); });


(function (myApp) {
    var orderService = function ($rootScope, $http, $q, $location, viewModelHelper) {

        var self = this;

        //self.orderId = 0;
         
        return this;
    };
    myApp.orderService = orderService;
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
