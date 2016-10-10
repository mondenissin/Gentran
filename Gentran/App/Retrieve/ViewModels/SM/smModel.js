﻿retrieveModule.controller("smViewModel", function ($sce,$scope, retrieveService, $http, $q, $routeParams, $window, $location, viewModelHelper, $timeout) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.retrieveService = retrieveService;
    $scope.trustAsHtml = $sce.trustAsHtml;
    var initialize = function () {
        $scope.refreshSM();
    }

    $scope.refreshSM = function () {
        
        viewModelHelper.apiGet('api/ftp/sm', null, function (result) {
            
            if (result.data.success) {
                $scope.files = result.data.detail.map(function (obj) {
                    var rObj = {};
                    var split = obj.files.split('\\');
                    var ext = split[split.length - 1].split('.');
                    rObj["files"] = split[split.length - 1];
                    rObj["directory"] = obj.files;
                    rObj["extension"] = "../Images/files/" + ext[ext.length - 1] + ".png";

                    return rObj;
                });

                console.log($scope.files);
            } else {
                console.log(result.data.detail[0].error);
            }
        });
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

        downloadURI(host + "ftp/" + split[split.length - 1], split[split.length - 1]);
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

        var screenLeft = window.screenLeft != undefined ? window.screenLeft : screen.left;
        var screentop = window.screenTop != undefined ? window.screenTop : screen.top;

        width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
        height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

        var w = '1100';
        var h = '600';
        var left = ((width / 2) - (w / 2)) + screenLeft;
        var top = ((height / 2) - (h / 2)) + screentop;

        if (temp2 == "txt") {
            printWindow = window.open(host + 'ftp/' + data.files + '', '', 'left=' + left + ',top=' + top + ',width=' + w + ',height=' + h + ',status=0');
            setTimeout(function () {
                printWindow.print();
                printWindow.close();
            }, 500);
        }
        else {
            console.log(host + 'ftp/' + data.files);
            var strContent = "<html><head>";
            strContent = strContent + "<title>Invoice Printing</title>";
            strContent = strContent + "</head><body>";
            strContent = strContent + "<div class=\"print-wrapper\">";
            strContent = strContent + "<object data='" + host + 'ftp/' + data.files + "' type='application/pdf' width='100%' height='100%'>";
            strContent = strContent + "alt : <a href='" + host + 'ftp/' + data.files + "'>Your browser is not supported. Please click here to download.</a>";
            strContent = strContent + "</object>";
            strContent = strContent + "</div>";
            strContent = strContent + "</body>";
            strContent = strContent + "</html>";
            printWindow = window.open('', '', 'left=' + left + ',top=' + top + ',width=' + w + ',height=' + h + ',status=0');
            printWindow.document.write(strContent);
        }
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
                var tr;

                //$(this.parentElement).remove();
                this.checked = false;

                listFile[ctr] = {};

                listElem[ctr] = {};
                listElem[ctr].element = $(this.parentElement);
                
                listFile[ctr].outlet = "SM";
                listFile[ctr].fileName = this.nextElementSibling.children[0].children[2].textContent;
                listFile[ctr].fileLogo = this.nextElementSibling.children[0].children[0].outerHTML;
                listFile[ctr].name = this.nextElementSibling.children[0].children[1].textContent;
                listFile[ctr].fileID = this.nextElementSibling.children[0].children[1].textContent.replace('.', '');

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
                        if (result.data.success) {
                            var mapData = result.data.filecontent;
                            execTime = result.data.execution;
                            //console.log(mapData);
                            console.log(execTime);
                            $('#' + fileName[0].fileID).text("Reading");
                            $('#' + fileName[0].fileID).animate({ width: '100%' }, execTime);

                            setTimeout(function () {
                                $('#' + fileName[0].fileID).text("Successful");

                                //IF PO IS SUCCESSFUL
                                $(elemName[0].element).remove();
                            }, execTime);
                        }
                    });

                    i++;
                    if (i < count) {
                        setTimeout(LaodFile, 1000);
                    }
                }
                setTimeout(function () {
                    LaodFile();
                }, execTime);
            } else {
                alert("Select File");
            }
        });
    }

    $scope.clearSuccess = function () {
        //$('#mapModal .modal-body table tbody').remove();
    }
    
    $scope.closeModal = function () {
        $('#mapModal').modal('hide');
        //$('#mapModal .modal-body table tbody').remove();
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