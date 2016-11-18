retrieveModule.controller("rootViewModel", function ($timeout,$route,$scope, retrieveService, $http, $q, $routeParams, $window, $location, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.retrieveService = retrieveService;

    $scope.flags = { shownFromList: false };

    var initialize = function () {
        $scope.POfile = [];
        $scope.autoRetrieve();
        var time = 0;
        
        setInterval(function () {
            time++;
            if(time % 5 == 0){
                $scope.$apply(function () {
                    $scope.autoRetrieve();
                    time = 0;
                });
            }
        },1000);
    }
    $scope.autoRetrieve = function () {
        $scope.retrieveCtr('SM', 'c_sm');
        $scope.retrieveCtr('S8', 'c_s8');
        $scope.retrieveCtr('NCC', 'c_ncc');
    }

    $scope.retrieveCtr = function (chain, acct) {
        viewModelHelper.apiGet('api/ftp/' + acct, null, function (result) {
            if (result.data.success) {
                if (chain == "SM") {
                    $scope.POfile.SM = parseInt(result.data.notiftext);
                }else if(chain == "S8"){
                    $scope.POfile.S8 = parseInt(result.data.notiftext);
                } else if (chain == "NCC") {
                    $scope.POfile.NCC =parseInt(result.data.notiftext);
                }
                console.log(result.data);
            } else {
            }
        });
    }

    $scope.smFile = function () {
        viewModelHelper.navigateTo('Retrieve/SM');
        if ($location.path() == "/Gentran/Retrieve/SM") {
            $route.reload();
        }
    }

    $scope.superFile = function () {
        viewModelHelper.navigateTo('Retrieve/Super8');
        if ($location.path() == "/Gentran/Retrieve/Super8") {
            $route.reload();
        }
    }

    $scope.nccFile = function () {
        viewModelHelper.navigateTo('Retrieve/NCC');
        if ($location.path() == "/Gentran/Retrieve/NCC") {
            $route.reload();
        }
    }
    initialize();
});
