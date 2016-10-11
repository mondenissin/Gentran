retrieveModule.controller("superViewModel", function ($scope, retrieveService, $http, $q, $routeParams, $window, $location, viewModelHelper, $timeout) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.retrieveService = retrieveService;

    var initialize = function () {
        $scope.refreshS8();
    }

    $scope.refreshS8 = function () {

        viewModelHelper.apiGet('api/ftp/s8', null, function (result) {
            
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
        console.log(this);
        if (this.check == true) {
            $($($event.target)[0].nextElementSibling).css('border', '2px solid green');
            $($($event.target)[0].nextElementSibling).css('background', 'hsla(120,100%,50%,0.3)');
        }
        else if(this.check == false){
            $($($event.target)[0].nextElementSibling).css('border', '2px solid #e1e1e1');
            $($($event.target)[0].nextElementSibling).css('background', 'transparent');
        }
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