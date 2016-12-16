retrieveModule.controller("rootViewModel", function ($timeout,$route,$scope, retrieveService, $http, $q, $routeParams, $window, $location, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.retrieveService = retrieveService;
    $scope.activeOutlet = "";
    $scope.flags = { shownFromList: false };

    var initialize = function () {
        $scope.POfile = [];
        $scope.POfileErr = [];
        $scope.outletConn = [];
        $scope.color = [];
        $scope.autoRetrieve();
        var time = 0;
        
        setInterval(function () {

            console.log(retrieveService.getPause());

            if(retrieveService.getPause() == false){
                time++;

                if (time % 5 == 0) {
                    $scope.$apply(function () {
                        retrieveService.setPause(true);
                        console.log(retrieveService.getPause());
                        $scope.autoRetrieve();
                        time = 0;
                    });
                }
            }
        },1000);
    }
    $scope.autoRetrieve = function () {
        $scope.retrieveCtr('SM', 'c_sm');
        $scope.retrieveCtr('S8', 'c_s8');
        $scope.retrieveCtr('NCC', 'c_ncc');
        $scope.retrieveCtr('PRG', 'c_prg');
        $scope.retrieveCtr('WTM', 'c_wtm');
        $scope.retrieveCtr('UTM', 'c_utm');
    }

    $scope.retrieveCtr = function (chain, acct) {
        viewModelHelper.apiGet('api/ftp/' + acct, null, function (result) {

            if (result.data.success) {
                if (chain == "SM") {
                    $scope.POfile.SM = parseInt(result.data.notiftext);
                    $scope.outletConn.SM = result.data.notifConnection;
                    $scope.color.SM = result.data.connColor;
                }else if(chain == "S8"){
                    $scope.POfile.S8 = parseInt(result.data.notiftext);
                    $scope.outletConn.S8 = result.data.notifConnection;
                    $scope.color.S8 = result.data.connColor;
                } else if (chain == "PRG") {
                    $scope.POfile.PRG = parseInt(result.data.notiftext);
                    $scope.outletConn.PRG = result.data.notifConnection;
                    $scope.color.PRG = result.data.connColor;
                } else if (chain == "WTM") {
                    $scope.POfile.WTM = parseInt(result.data.notiftext);
                    $scope.outletConn.WTM = result.data.notifConnection;
                    $scope.color.WTM = result.data.connColor;
                } else if (chain == "UTM") {
                    $scope.POfile.UTM = parseInt(result.data.notiftext);
                    $scope.outletConn.UTM = result.data.notifConnection;
                    $scope.color.UTM = result.data.connColor;
                } else if (chain == "NCC") {
                    $scope.POfile.NCC = parseInt(result.data.notiftext);
                    $scope.outletConn.NCC = result.data.notifConnection;
                    $scope.color.NCC = result.data.connColor;
                }
                //console.log(result.data);
            } else {
                $scope.color = "red";
                if (chain == "SM") {
                    $scope.outletConn.SM = result.data.notifConnection;
                } else if (chain == "S8") {
                    $scope.outletConn.S8 = result.data.notifConnection;
                } else if (chain == "PRG") {
                    $scope.outletConn.PRG = result.data.notifConnection;
                } else if (chain == "WTM") {
                    $scope.outletConn.WTM = result.data.notifConnection;
                } else if (chain == "UTM") {
                    $scope.outletConn.UTM = result.data.notifConnection;
                } else if (chain == "NCC") {
                    $scope.outletConn.NCC = result.data.notifConnection;
                }
            }
            retrieveService.setPause(false);
        });
    }

    $scope.smFile = function () {
        retrieveService.setPause(true);
        viewModelHelper.navigateTo('Retrieve/SM');
        if ($location.path() == "/Gentran/Retrieve/SM") {
            $route.reload();
        }
        $scope.activeOutlet = "sm";
    }

    $scope.superFile = function () {
        retrieveService.setPause(true);
        viewModelHelper.navigateTo('Retrieve/Super8');
        if ($location.path() == "/Gentran/Retrieve/Super8") {
            $route.reload();
        }
        $scope.activeOutlet = "s8";
    }

    $scope.nccFile = function () {
        retrieveService.setPause(true);
        viewModelHelper.navigateTo('Retrieve/NCC');
        if ($location.path() == "/Gentran/Retrieve/NCC") {
            $route.reload();
        }
        $scope.activeOutlet = "ncc";
    }

    $scope.prgFile = function () {
        retrieveService.setPause(true);
        viewModelHelper.navigateTo('Retrieve/Puregold');
        if ($location.path() == "/Gentran/Retrieve/Puregold") {
            $route.reload();
        }
        $scope.activeOutlet = "prg";
    }

    $scope.wtmFile = function () {
        retrieveService.setPause(true);
        viewModelHelper.navigateTo('Retrieve/Waltermart');
        if ($location.path() == "/Gentran/Retrieve/Waltermart") {
            $route.reload();
        }
        $scope.activeOutlet = "wtm";
    }

    $scope.utmFile = function () {
        retrieveService.setPause(true);
        viewModelHelper.navigateTo('Retrieve/Ultramega');
        if ($location.path() == "/Gentran/Retrieve/Ultramega") {
            $route.reload();
        }
        $scope.activeOutlet = "utm";
    }
    initialize();
});
