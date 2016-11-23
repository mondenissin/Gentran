retrieveModule.controller("retrieveViewModel", function ($sce,$scope, retrieveService, $http, $q, $routeParams, $window, $location, viewModelHelper, $timeout, DTOptionsBuilder, DTColumnDefBuilder, $route, filterFilter) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.retrieveService = retrieveService;
    $scope.trustAsHtml = $sce.trustAsHtml;

    var initialize = function () {
        $scope.refreshRetrieve();
    }

    $scope.refreshRetrieve = function () {

        $scope.search = {};
        $scope.searchBy = "files";

        viewModelHelper.apiGet('api/ftp/'+$scope.activeOutlet, null, function (result) {
            $scope.pageSize = 5;
            $scope.entryLimit = 50;
            $scope.entryLimitPO = 12;

            if (result.data.success) {
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

                $scope.$watch('search[searchBy]', function () {
                    $scope.files = filterFilter($scope.filesList, $scope.search);

                    $scope.noOfPages = Math.ceil($scope.files.length / $scope.entryLimit);
                    $scope.noOfPagesPO = Math.ceil($scope.files.length / $scope.entryLimitPO);

                    $scope.currentPage = 1;
                });

                //viewModelHelper.saveTransaction(result.data.transactionDetail);
                console.log($scope.files);
            } else {
                console.log(result.data.detail[0].error);
            }
        });

        $scope.dtOptions = DTOptionsBuilder.newOptions();
        $scope.dtColumnDefs = [
           DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
        ];
    }

    //$scope.loadHover = function () {
    //    $timeout(function () {
    //        $('.po-files').hover(function () {
    //            $($(this).context.lastElementChild).css({ top: 'auto', bottom: '0px' });
    //        }, function () {
    //            $($(this).context.lastElementChild).animate({ top: '85px' }, 50)
    //        });
    //    }, 0);
    //}

    $scope.unsuccess = function () {
        viewModelHelper.apiGet('api/unsuccess/' + $scope.activeOutlet, null, function (result) {
            $scope.pageSize = 5;
            $scope.entryLimit = 50;
            $scope.entryLimitPO = 12;

            if (result.data.success) {

                $scope.unsuccessList = result.data.detail.map(function (obj) {
                    var rObj = {};
                    var split = obj.files.split('\\');
                    var ext = split[split.length - 1].split('.');
                    rObj["files"] = split[split.length - 1];
                    rObj["directory"] = obj.files;
                    rObj["rawID"] = "";
                    rObj["extension"] = "../Images/files/" + ext[ext.length - 1] + ".png";
                    rObj["retrievedate"] = "";

                    return rObj;
                });
                $scope.qweqwe = $scope.unsuccessList;

                $scope.$watch('search[searchBy]', function () {
                    $scope.unsuccessList = filterFilter($scope.qweqwe, $scope.search);

                    $scope.noOfPages = Math.ceil($scope.unsuccessList.length / $scope.entryLimit);
                    $scope.noOfPagesPO = Math.ceil($scope.unsuccessList.length / $scope.entryLimitPO);

                    $scope.currentPage = 1;
                });

                console.log($scope.unsuccessList);
            } else {
                console.log(result.data);
            }
        });
    };

    $scope.unsuccessRet = function () {
        var chk = false;
        var ctr = 0;
        var listFile = [];
        var listElem = [];

        $('.po-cbx-unsucc').each(function () {

            if (this.checked == true) {

                chk = true;

                //$(this.parentElement).remove();
                //this.checked = false;

                listFile[ctr] = {};
                console.log($(this));
                listElem[ctr] = {};
                listElem[ctr].element = $($($(this.parentElement)[0].parentElement)[0].parentElement);
                listFile[ctr].outlet = $scope.activeOutlet.toUpperCase();
                listFile[ctr].fileName = $($($($(this.parentElement)[0].parentElement)[0].children[1])[0].children)[1].title;
                listFile[ctr].rawID = $($($($(this.parentElement)[0].parentElement)[0].children[1])[0].children)[3].textContent;
                listFile[ctr].fileLogo = $($($($(this.parentElement)[0].parentElement)[0].children[1])[0].children)[0].outerHTML;
                listFile[ctr].name = $($($($(this.parentElement)[0].parentElement)[0].children[1])[0].children)[2].textContent;
                listFile[ctr].fileID = $($($($(this.parentElement)[0].parentElement)[0].children[1])[0].children)[2].textContent.replace(/[. ]/g, '');

                ctr++;
            }
        }).promise().done(function () {
            $scope.uploadedFile = listFile;

            console.log($scope.uploadedFile);
            
            if (chk) {
                var i = 0, count = listFile.length;
                var execTime;

                function LaodFile() {
                    var fileName = [];
                    var elemName = [];
                    var items = {};

                    elemName[0] = listElem[i];
                    fileName[0] = listFile[i];
                    items.payload = fileName;

                    viewModelHelper.apiPost('api/unsuccess', JSON.stringify(items), function (result) {

                        console.log(result.data);
                        if (result.data.success == true) {
                            $(elemName[0].element).remove();
                        }else{
                
                        }

                        viewModelHelper.saveTransaction(result.data.detail);
                        
                    });

                    i++;
                    if (i < count) {
                        setTimeout(LaodFile, 1000);
                    } else if (i == count) {
                        $scope.autoRetrieve();
                    }
                }
                setTimeout(function () {
                    LaodFile();
                }, execTime);
            } else {
                notif_warning('File Retriever', 'Please select file to retrieve');
            }
        });
    }

    $scope.downloadFile = function (data) {
        window.location.href = viewModelHelper.getRootPath() + "api/download/" + data.rawID + ',order';
    }

    $scope.selectFile = function ($event) {
        /*console.log(this);
        if (this.check == true) {
            $($($event.target)[0].nextElementSibling).css('border', '2px solid green');
            $($($event.target)[0].nextElementSibling).css('background', 'hsla(120,100%,50%,0.3)');
        }
        else if(this.check == false){
            $($($event.target)[0].nextElementSibling).css('border', '2px solid #e1e1e1');
            $($($event.target)[0].nextElementSibling).css('background', 'transparent');
        }*/
    }
        
    $scope.selectFileAll = function () {
        if (this.all == true) {
            $('.po-files').css('border', '2px solid green');
            $('.po-files').css('background', 'hsla(120,100%,50%,0.3)');
        } else if (this.all == false) {
            $('.po-files').css('border', '2px solid #e1e1e1');
            $('.po-files').css('background', 'transparent');
        }
    }

    $scope.selectFileAllUnsucc = function () {
        if (this.allU == true) {
            $('.po-files').css('border', '2px solid green');
            $('.po-files').css('background', 'hsla(120,100%,50%,0.3)');
        } else if (this.allU == false) {
            $('.po-files').css('border', '2px solid #e1e1e1');
            $('.po-files').css('background', 'transparent');
        }
    }

    function convertHTML(tag) {

        return $sce.trustAsHtml(tag);
    }

    $scope.getSelected = function () {
        var chk = false;
        var ctr = 0;
        var listFile = [];
        var listElem = [];

        $('.po-cbx').each(function () {

            if (this.checked == true) {

                chk = true;

                //$(this.parentElement).remove();
                //this.checked = false;

                listFile[ctr] = {};
                console.log($(this));
                listElem[ctr] = {};
                listElem[ctr].element = $($($(this.parentElement)[0].parentElement)[0].parentElement);
                listFile[ctr].outlet = $scope.activeOutlet.toUpperCase();
                listFile[ctr].fileName = $($($($(this.parentElement)[0].parentElement)[0].children[1])[0].children)[1].title;
                listFile[ctr].rawID = $($($($(this.parentElement)[0].parentElement)[0].children[1])[0].children)[3].textContent;
                listFile[ctr].fileLogo = $($($($(this.parentElement)[0].parentElement)[0].children[1])[0].children)[0].outerHTML;
                listFile[ctr].name = $($($($(this.parentElement)[0].parentElement)[0].children[1])[0].children)[2].textContent;
                listFile[ctr].fileID = $($($($(this.parentElement)[0].parentElement)[0].children[1])[0].children)[2].textContent.replace(/[. ]/g, '');
                
                ctr++;
            }
        }).promise().done(function () {
            $scope.uploadedFile = listFile;

            if (chk) {
                $('#mapModal').modal('show');
                var i = 0, count = listFile.length;
                var execTime;

                function LaodFile() {
                    var fileName = [];
                    var elemName = [];
                    var items = {};

                    elemName[0] = listElem[i];
                    fileName[0] = listFile[i];
                    items.payload = fileName;

                    viewModelHelper.apiPost('api/masteruploader', JSON.stringify(items), function (result) {

                        console.log(result.data.success);
                        var mapData = result.data.filecontent;
                        execTime = result.data.execution;
                        console.log(result.data.filecontent);
                        console.log(result.data.detail);

                        $('#' + fileName[0].fileID).text("Reading");
                        $('#' + fileName[0].fileID).animate({ width: '100%' }, execTime);

                        setTimeout(function () {

                            if (result.data.success == true) {
                                $('#' + fileName[0].fileID).text("Read Successful");
                            } else {
                                $('#' + fileName[0].fileID).text("With error");
                            }

                            $(elemName[0].element).remove();
                            //$scope.refreshSM();
                        }, execTime);

                        viewModelHelper.saveTransaction(result.data.detail);
                    });

                    i++;
                    if (i < count) {
                        setTimeout(LaodFile, 1000);
                    } else if(i == count) {
                        $scope.autoRetrieve();
                    }
                }
                setTimeout(function () {
                    LaodFile();
                }, execTime);
            } else {
                notif_warning('File Reader', 'Please select file to read');
            }
        });
    }


    initialize();
});


retrieveModule.directive('tooltip', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $(element).hover(function () {
                // on mouseenter
                $(element).tooltip('show');
            }, function () {
                // on mouseleave
                $(element).tooltip('hide');
            });
        }
    };
});