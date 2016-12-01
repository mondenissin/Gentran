retrieveModule.controller("rootViewModel", function ($timeout,$route,$scope, retrieveService, $http, $q, $routeParams, $window, $location, viewModelHelper) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.retrieveService = retrieveService;
    $scope.activeOutlet = "";
    $scope.flags = { shownFromList: false };

    var initialize = function () {
        $scope.POfile = [];
        $scope.POfileErr = [];
        $scope.autoRetrieve();
        var time = 0;
        
        setInterval(function () {
            time++;
            if(time % 5 == 0){
                $scope.$apply(function () {
                    $scope.autoRetrieve();
                    //$scope.files;
                    time = 0;
                });
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
                    $scope.POfile.SM = parseInt(result.data.detail.length);
                    $scope.POfileErr.SM = parseInt(result.data.notiftextErr);
                }else if(chain == "S8"){
                    $scope.POfile.S8 = parseInt(result.data.detail.length);
                    $scope.POfileErr.S8 = parseInt(result.data.notiftextErr);
                } else if (chain == "PRG") {
                    $scope.POfile.PRG = parseInt(result.data.detail.length);
                    $scope.POfileErr.PRG = parseInt(result.data.notiftextErr);
                } else if (chain == "WTM") {
                    $scope.POfile.WTM = parseInt(result.data.detail.length);
                    $scope.POfileErr.WTM = parseInt(result.data.notiftextErr);
                } else if (chain == "UTM") {
                    $scope.POfile.UTM = parseInt(result.data.detail.length);
                    $scope.POfileErr.UTM = parseInt(result.data.notiftextErr);
                } else if (chain == "NCC") {
                    $scope.POfile.NCC = parseInt(result.data.detail.length);
                    $scope.POfileErr.NCC = parseInt(result.data.notiftextErr);
                    $scope.files = result.data.detail.map(function (obj) {
                        var rObj = {};
                        var split = obj.files.split('\\');
                        var ext = split[split.length - 1].split('.');
                        rObj["files"] = split[split.length - 1];
                        rObj["directory"] = obj.files;
                        rObj["rawID"] = obj.rawid;
                        rObj["thumbnail"] = "../Images/thumbnails/" + ext[ext.length - 1] + ".PNG";
                        rObj["extension"] = "../Images/files/" + ext[ext.length - 1] + ".png";
                        rObj["retrievedate"] = obj.retdate;

                        return rObj;
                    });
                    $scope.filesList = $scope.files;

                    $scope.$applyAsync();

                    console.log($scope.files);
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
        $scope.activeOutlet = "sm";
    }

    $scope.superFile = function () {
        viewModelHelper.navigateTo('Retrieve/Super8');
        if ($location.path() == "/Gentran/Retrieve/Super8") {
            $route.reload();
        }
        $scope.activeOutlet = "s8";
    }

    $scope.nccFile = function () {
        viewModelHelper.navigateTo('Retrieve/NCC');
        if ($location.path() == "/Gentran/Retrieve/NCC") {
            $route.reload();
        }
        $scope.activeOutlet = "ncc";
    }

    $scope.prgFile = function () {
        viewModelHelper.navigateTo('Retrieve/Puregold');
        if ($location.path() == "/Gentran/Retrieve/Puregold") {
            $route.reload();
        }
        $scope.activeOutlet = "prg";
    }

    $scope.wtmFile = function () {
        viewModelHelper.navigateTo('Retrieve/Waltermart');
        if ($location.path() == "/Gentran/Retrieve/Waltermart") {
            $route.reload();
        }
        $scope.activeOutlet = "wtm";
    }

    $scope.utmFile = function () {
        viewModelHelper.navigateTo('Retrieve/Ultramega');
        if ($location.path() == "/Gentran/Retrieve/Ultramega") {
            $route.reload();
        }
        $scope.activeOutlet = "utm";
    }
    initialize();
});
