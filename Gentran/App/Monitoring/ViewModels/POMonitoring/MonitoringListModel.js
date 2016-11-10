monitoringModule.controller("monitoringViewModel", function ($scope, monitoringService, $http, $q, $routeParams, $window, $location, viewModelHelper, DTOptionsBuilder, DTColumnDefBuilder, filterFilter) {

    $scope.viewModelHelper = viewModelHelper;
    $scope.monitoringService = monitoringService;

    var initialize = function () {
        $scope.refreshCust();
    }

    $scope.refreshCust = function () {
        $scope.search = {};
        $scope.searchBy = "TLId";

        viewModelHelper.apiGet('api/monitor', null, function (result) {
                $scope.pageSize = 5;
                $scope.entryLimit = 50;

                $scope.monitor = result.data.detail;
                console.log(result.data.detail);
                $scope.$watch('search[searchBy]', function () {
                    $scope.filterList = filterFilter($scope.monitor, $scope.search);
                    $scope.noOfPages = Math.ceil($scope.filterList.length / $scope.entryLimit);
                    $scope.currentPage = 1;
                });
        });

        $scope.dtOptions = DTOptionsBuilder.newOptions();
        $scope.dtColumnDefs = [
           DTColumnDefBuilder.newColumnDef('no-sort').notSortable()
        ];
    }

    $scope.showDetails = function (monitoring) {
       // $scope.ResetEditFields();
        $('#txt_ponum').text(monitoring.ULPONumber);
        $('#txt_custnum').text(monitoring.ULCustomer != 0 ? monitoring.ULCustomer : 'No Customer');
        $('#txt_custname').text(monitoring.ULCustomer);
        $('#txt_orderdate').text(monitoring.ULOrderDate);
        $('#txt_deliverydate').text(monitoring.ULDeliveryDate);
        $('#txt_retrievedate').text(monitoring.ULUploadDate);
        $('#txt_uploaddate').text(monitoring.ULUploadDate);
        $('#txt_submitteddate').text(monitoring.ULUploadDate);
        $('#txt_reader').text(monitoring.ULUser);
        $('#txt_submittedby').text(monitoring.ULUser);
        $('#txt_sku').text(monitoring.UIOrigQuantity);
        $('#txt_qty').text(monitoring.UIQuantity);
        $('#txt_amount').text(monitoring.UIPrice);

        $('#ViewPODetailsModal').modal('show');
    }

    $scope.GenerateXml = function (downloadfile) {

        var Item = {};
        Item.ulponumber = downloadfile.ULPONumber;

        $scope.data = {};
        $scope.data.payload = _payloadParser(Item);
        
        viewModelHelper.apiDownloadXML('api/monitor', $scope.data, function (result) {
            $scope.DownloadXml(result.data.detail[0].ULPONumber, '<?xml version="1.0" encoding="UTF-8" standalone="yes" ?><Order><header><sender>NCCC Supermarket</sender><company_name>NCCC SUPERMARKET</company_name><company_address>Sta. Ana-Gempesaw Ext. Street. Davao City 8000</company_address><company_phone>Tel #: (082) 227-3449/(082) 227-3450</company_phone><message_name>PURCHASE ORDER</message_name></header><podetails><po_number>5033245</po_number><back_orderno>00</back_orderno><podate>10/24/16</podate><delvdatewc>10/27/2016</delvdatewc><vendor>248281-MONDE NISSIN CORPORATION</vendor><postatus>RELEASED</postatus><dept>102-DRY FOOD 2-</dept><buyer>KATHYRYN P. MAGADA</buyer><terms>NET 30</terms><lastmoddate>10/24/16</lastmoddate></podetails><delivery_details><deliver_to>12027-CHOICEMART LANANG DAMOSA</deliver_to><delv_dest_add1>.</delv_dest_add1><delv_dest_add2>DAMOSA</delv_dest_add2><delv_dest_add3>LANANG. DAVAO CITY</delv_dest_add3><delvdate>10/27/16</delvdate><canceldate>11/27/16</canceldate><hdr_remarks1>CN 3%</hdr_remarks1></delivery_details><lineitem_details><lineitem><item_no>1</item_no><order_qty>1.00</order_qty><sku>010494063</sku><vend_partno>82122078</vend_partno><upc>8886467100062</upc><itemdesc><![CDATA[PRINGLES SLT&SEAWD 110G]]></itemdesc><buy_uom>C12</buy_uom><sell_uom>PCS</sell_uom><disc_cost>723.60</disc_cost><unit_cost>723.60</unit_cost><amount>723.60</amount></lineitem><lineitem><item_no>2</item_no><order_qty>1.00</order_qty><sku>010736409</sku><vend_partno>PSO114822</vend_partno><upc>8886467100260</upc><itemdesc>PRINGLES ORIG PCKT CAN 47G</itemdesc><buy_uom>C12</buy_uom><sell_uom>PCS</sell_uom><disc_cost>304.08</disc_cost><unit_cost>304.08</unit_cost><amount>304.08</amount></lineitem><lineitem><item_no>3</item_no><order_qty>1.00</order_qty><sku>011125683</sku><upc>8886467105197</upc><itemdesc><![CDATA[PRINGLES TRTLA SRCRM&ONN110G]]></itemdesc><buy_uom>C12</buy_uom><sell_uom>PCS</sell_uom><disc_cost>746.04</disc_cost><unit_cost>746.04</unit_cost><amount>746.04</amount></lineitem><lineitem><item_no>4</item_no><order_qty>1.00</order_qty><sku>011126078</sku><upc>8886467105814</upc><itemdesc>PRINGLES CHSE PCKT CAN 42G</itemdesc><buy_uom>C12</buy_uom><sell_uom>PCS</sell_uom><disc_cost>304.08</disc_cost><unit_cost>304.08</unit_cost><amount>304.08</amount></lineitem><lineitem><item_no>5</item_no><order_qty>1.00</order_qty><sku>011144979</sku><upc>8886467103643</upc><itemdesc><![CDATA[PRINGLES SR CRM&ONN PCH 40G]]></itemdesc><buy_uom>C60</buy_uom><sell_uom>PCS</sell_uom><disc_cost>1335.00</disc_cost><unit_cost>1335.00</unit_cost><amount>1335.00</amount></lineitem></lineitem_details><total_amount><totqty>5.00</totqty><grosstot>3047.13</grosstot><nettot>3412.80</nettot></total_amount><trailer><preparedby>Saguira Datu</preparedby><po_remarks1>1. BRING PO AND SALES DOCUMENT UPON DELIVERY.</po_remarks1><po_remarks2>2. DELIVER BEFORE EXPIRATION OF THIS PO.</po_remarks2><po_remarks3>3. CLAIM BAD ORDERS/RETURNS UPON DELIVER (LOCAL).</po_remarks3><po_remarks4>4. CLAIM BAD ORDERS/RETURNS UPON ADVISE (MANILA).</po_remarks4><timestamps>10/24/2016  7:09:33pm</timestamps></trailer></Order>');
           
        });
    };

    $scope.DownloadXml = function (filename, data) {
        // Set up the link
        var link = document.createElement("a");
        link.setAttribute("target", "_blank");
        if (Blob !== undefined) {
            var blob = new Blob([data], { type: "text/xml" });
            link.setAttribute("href", URL.createObjectURL(blob));
        } else {
            link.setAttribute("href", "data:text/xml," + encodeURIComponent(data));
        }
        link.setAttribute("download", filename);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    };

    initialize();
}).filter('start', function () {
    return function (input, start) {
        if (!input || !input.length) { return; }

        start = +start;
        return input.slice(start);
    };
});