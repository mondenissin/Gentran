﻿retrieveModule.controller("smViewModel", function ($sce, $scope, retrieveService, $http, $q, $routeParams, $window, $location, viewModelHelper, $timeout, DTOptionsBuilder, DTColumnDefBuilder, $route, filterFilter) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.retrieveService = retrieveService;
    $scope.trustAsHtml = $sce.trustAsHtml;

    var initialize = function () {
        $scope.refreshSM();
    }

    $scope.refreshSM = function () {
        $scope.search = {};
        $scope.searchBy = "files";

        viewModelHelper.apiGet('api/ftp/sm', null, function (result) {
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
                    rObj["thumbnail"] = "../Images/thumbnails/excel.PNG";
                    rObj["rawID"] = split[split.length - 1].split('-')[0];
                    //rObj["thumbnail"] = "../Images/thumbnails/" + ext[0].replace(/[" "]/g, "") + ".jpg";
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

    $scope.loadHover = function () {
        $timeout(function () {
            $('.po-files').hover(function () {
                $($(this).context.lastElementChild).css({ top: 'auto', bottom: '0px' });
            }, function () {
                $($(this).context.lastElementChild).animate({ top: '85px' }, 50)
            });
        }, 0);
    }

    $scope.downloadFile = function (data) {

        var split = data.directory.split('\\');

        downloadURI(host + "ftp/sm/" + split[split.length - 1], split[split.length - 1]);
        function downloadURI(uri, name) {
            var link = document.createElement("a");
            link.download = name;
            link.href = uri;
            link.click();
        }
    }

    $scope.showFile = function (data) {

        var temponame = data.directory;
        var temp1 = temponame.split('.');
        var temp2 = temp1[(temp1.length - 1)];
        var fileLoc = "";
        fileLoc = "ftp/sm/";

        viewModelHelper.fileViewer(temp2, data.files, fileLoc);
    }


    $scope.selectFile = function ($event) {
        /*if (this.check == true) {
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
                listFile[ctr].outlet = "SM";
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
                        var mapData = result.data.filecontent;
                        execTime = result.data.execution;
                        console.log(result.data.filecontent);
                        console.log(execTime);
                        $('#' + fileName[0].fileID).text("Reading");
                        $('#' + fileName[0].fileID).animate({ width: '100%' }, execTime);

                        setTimeout(function () {

                            if (result.data.success == true) {
                                $('#' + fileName[0].fileID).text("Read Successful");
                            } else {
                                $('#' + fileName[0].fileID).text("With error");
                            }

                            $('.progress-striped').removeClass("active");

                            $(elemName[0].element).remove();
                            console.log(elemName[0].element);
                            //$scope.refreshSM();
                        }, execTime);

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
                notif_warning('File Reader', 'Please select file to read');
            }
        });
    }

    /*$scope.clearSuccess = function () {
        //$('#mapModal .modal-body table tbody').remove();
    }*/
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