using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using Newtonsoft.Json;

namespace Gentran.Controllers.api
{
    public class DashController : ApiController
    {
        // GET api/default1
        public Object Get()
        {
            bool success = true;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            string year = DateTime.Now.Year.ToString();
            String sQuery = @"SELECT DISTINCT ULSubmitDate,
                            SUM(ULTotalQuantity) as ULTotalQuantity,
                            SUM(ULTotalSKUs) as  ULTotalSKUs,
                            SUM(ULTotalAmount) as  ULTotalAmount,
                            SUM(ULTotalOrders) as  ULTotalOrders
                            from (
                            select CONVERT(VARCHAR(10), ULSubmitDate, 111) as ULSubmitDate,
                            SUM(sumulquantity) as ULTotalQuantity,
                            SUM(countulquantity) as ULTotalSKUs,
                            SUM(uiprice) as ULTotalAmount, count(*) as ULTotalOrders 
                            from tblRawFile left join tblUploadLog on ULFile = RFId left join
                            (SELECT DISTINCT
                            ul.ULFile,
                            ui.sumulquantity,
                            ui.countulquantity,
                            ul.ulstatus,
                            ui.uiprice 
                            FROM tblUploadLog ul
                            LEFT JOIN tblCustomerMaster
                            ON ul.ulcustomer = cmid 
                            LEFT JOIN tblUserAssignment ua 
                            ON ul.ulcustomer = ua.uacustomer 
                            LEFT JOIN
                            (SELECT
                            uiprice = SUM(uiquantity * ppprice),
                            sumulquantity = SUM(uiquantity),
                            countulquantity = COUNT(uiquantity),
                            uiid
                            FROM tblUploadItems
                            left join tbluploadlog
                            on ulid = uiid
                            left join tblCustomerMaster
                            on cmid = ulcustomer
                            LEFT JOIN tblProductPricing
                            on ppproduct = uiproduct and pparea = cmarea
                            WHERE uistatus NOT IN ('3','0')
                            group by uiid ) ui 
                            ON ul.ulid = ui.uiid  
                            ) UL on ul.ULFile = RFId
                            where RFStatus = 1 AND ul.ulstatus in (21,22,25)
                            AND ULSubmitDate BETWEEN '" + year + @"-01-01 00:00:00.000' AND '" + year + @"-12-31 23:59:59.999'                            
                            group by ULSubmitDate
                            ) TB
                            WHERE ULTotalQuantity > 0
                            group by ULSubmitDate";
            SqlCommand cmd = new SqlCommand(sQuery, connection);
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            try
            {
                connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        row = new Dictionary<string, object>();
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            var cName = dr.GetName(i);
                            row.Add(cName, dr[cName]);
                        }
                        rows.Add(row);
                    }
                }
                else
                    success = false;
            }
            catch (Exception ex)
            {
                success = false;
                row = new Dictionary<string, object>();
                row.Add("Error", ex.Message);
                rows.Add(row);
            }

            return new Response { success = success, detail = rows };
        }

        // GET api/default1/5
        public object Get([FromUri]string value)
        {
            Data values = JsonConvert.DeserializeObject<Data>(value);

            bool success = true;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String sQuery;// = "select top 100 TLId,TLRemarks,TLValue,Left(TLDate,12) as TLDate,TADescription,UMUsername from tbltransactionlog left join tblTransactionActivity on taid = tlactivity left join tblusermaster on umid = tluser order by tlid desc";

            string byWeekPrefix = @"select 
                                    CONCAT('Week ', DATEPART(wk, ULSubmitDate)) as ULSubmitDate,
                                    SUM(ULTotalQuantity) as ULTotalQuantity,
                                    SUM(ULTotalSKUs) as ULTotalSKUs,
                                    SUM(ULTotalAmount) as ULTotalAmount,
                                    SUM(ULTotalOrders) as  ULTotalOrders
                                    from
                                    (";
            string byWeekSuffix = @") DSQL
                                    group by DATEPART(wk, ULSubmitDate),ULTotalOrders
                                    order by ULSubmitDate";

            if (values.operation == "filter_area_dashboard_by_date")
            {
                sQuery = @"SELECT DISTINCT ULSubmitDate,
                        SUM(ULTotalQuantity) as ULTotalQuantity,
                        SUM(ULTotalSKUs) as  ULTotalSKUs,
                        SUM(ULTotalAmount) as  ULTotalAmount,
                        SUM(ULTotalOrders) as  ULTotalOrders
                        from (
                        select CONVERT(VARCHAR(10), ULSubmitDate, 111) as ULSubmitDate,
                        SUM(sumulquantity) as ULTotalQuantity,
                        SUM(countulquantity) as ULTotalSKUs,
                        SUM(uiprice) as ULTotalAmount, count(*) as ULTotalOrders 
                        from tblRawFile left join tblUploadLog on ULFile = RFId left join
                        (SELECT DISTINCT
                        ul.ULFile,
                        ui.sumulquantity,
                        ui.countulquantity,
                        ul.ulstatus,
                        ui.uiprice 
                        FROM tblUploadLog ul
                        LEFT JOIN tblCustomerMaster
                        ON ul.ulcustomer = cmid 
                        LEFT JOIN tblUserAssignment ua 
                        ON ul.ulcustomer = ua.uacustomer 
                        LEFT JOIN
                        (SELECT
                        uiprice = SUM(uiquantity * ppprice),
                        sumulquantity = SUM(uiquantity),
                        countulquantity = COUNT(uiquantity),
                        uiid
                        FROM tblUploadItems
                        left join tbluploadlog
                        on ulid = uiid
                        left join tblCustomerMaster
                        on cmid = ulcustomer
                        LEFT JOIN tblProductPricing
                        on ppproduct = uiproduct and pparea = cmarea
                        WHERE uistatus NOT IN ('3','0')
                        group by uiid ) ui 
                        ON ul.ulid = ui.uiid  
                        ) UL on ul.ULFile = RFId
                        where RFStatus = 1 AND ul.ulstatus in (21,22,25)
                        AND ULSubmitDate BETWEEN '" + values.payload[0].dateFrom + @"' AND '" + values.payload[0].dateTo + @"'
                        group by ULSubmitDate
                        ) TB
                        WHERE ULTotalQuantity > 0
                        group by ULSubmitDate";
                if (values.payload[0].prefix == "Weekly") {
                    sQuery = byWeekPrefix + sQuery + byWeekSuffix;
                }
            }
            else if (values.operation == "filter_area_dashboard_by_chain")
            {
                sQuery = @"SELECT DISTINCT ULSubmitDate,
                        SUM(ULTotalQuantity) as ULTotalQuantity,
                        SUM(ULTotalSKUs) as  ULTotalSKUs,
                        SUM(ULTotalAmount) as  ULTotalAmount,
                        SUM(ULTotalOrders) as  ULTotalOrders
                        from (
                        select CONVERT(VARCHAR(10), ULSubmitDate, 111) as ULSubmitDate,
                        SUM(sumulquantity) as ULTotalQuantity,
                        SUM(countulquantity) as ULTotalSKUs,
                        SUM(uiprice) as ULTotalAmount, count(*) as ULTotalOrders 
                        from tblRawFile left join tblUploadLog on ULFile = RFId left join
                        (SELECT DISTINCT
                        ul.ULFile,
                        ui.sumulquantity,
                        ui.countulquantity,
                        ul.ulstatus,
                        ui.uiprice 
                        FROM tblUploadLog ul
                        LEFT JOIN tblCustomerMaster
                        ON ul.ulcustomer = cmid 
                        LEFT JOIN tblUserAssignment ua 
                        ON ul.ulcustomer = ua.uacustomer 
                        LEFT JOIN
                        (SELECT
                        uiprice = SUM(uiquantity * ppprice),
                        sumulquantity = SUM(uiquantity),
                        countulquantity = COUNT(uiquantity),
                        uiid
                        FROM tblUploadItems
                        left join tbluploadlog
                        on ulid = uiid
                        left join tblCustomerMaster
                        on cmid = ulcustomer
                        LEFT JOIN tblProductPricing
                        on ppproduct = uiproduct and pparea = cmarea
                        WHERE uistatus NOT IN ('3','0')
                        and ULCustomer in (SELECT Distinct(CACustomer) from tblCustomerAssignment where CAAccount = '" + values.payload[0].acctype + @"')
                        group by uiid ) ui 
                        ON ul.ulid = ui.uiid  
                        ) UL on ul.ULFile = RFId
                        where RFStatus = 1 AND ul.ulstatus in (21,22,25)
                        AND ULSubmitDate BETWEEN '" + values.payload[0].dateFrom + @"' AND '" + values.payload[0].dateTo + @"'
                        group by ULSubmitDate
                        ) TB
                        WHERE ULTotalQuantity > 0
                        group by ULSubmitDate";
                if (values.payload[0].prefix == "Weekly")
                {
                    sQuery = byWeekPrefix + sQuery + byWeekSuffix;
                }

            }
            else if (values.operation == "filter_area_dashboard_by_area")
            {
                sQuery = @"SELECT DISTINCT ULSubmitDate,
                        SUM(ULTotalQuantity) as ULTotalQuantity,
                        SUM(ULTotalSKUs) as  ULTotalSKUs,
                        SUM(ULTotalAmount) as  ULTotalAmount,
                        SUM(ULTotalOrders) as  ULTotalOrders
                        from (
                        select CONVERT(VARCHAR(10), ULSubmitDate, 111) as ULSubmitDate,
                        SUM(sumulquantity) as ULTotalQuantity,
                        SUM(countulquantity) as ULTotalSKUs,
                        SUM(uiprice) as ULTotalAmount, count(*) as ULTotalOrders 
                        from tblRawFile left join tblUploadLog on ULFile = RFId left join
                        (SELECT DISTINCT
                        ul.ULFile,
                        ui.sumulquantity,
                        ui.countulquantity,
                        ul.ulstatus,
                        ui.uiprice 
                        FROM tblUploadLog ul
                        LEFT JOIN tblCustomerMaster
                        ON ul.ulcustomer = cmid 
                        LEFT JOIN tblUserAssignment ua 
                        ON ul.ulcustomer = ua.uacustomer 
                        LEFT JOIN
                        (SELECT
                        uiprice = SUM(uiquantity * ppprice),
                        sumulquantity = SUM(uiquantity),
                        countulquantity = COUNT(uiquantity),
                        uiid
                        FROM tblUploadItems
                        left join tbluploadlog
                        on ulid = uiid
                        left join tblCustomerMaster
                        on cmid = ulcustomer
                        LEFT JOIN tblProductPricing
                        on ppproduct = uiproduct and pparea = cmarea
                        WHERE uistatus NOT IN ('3','0')
                        and CMArea = '" + values.payload[0].cmArea + @"'
                        group by uiid ) ui 
                        ON ul.ulid = ui.uiid  
                        ) UL on ul.ULFile = RFId
                        where RFStatus = 1 AND ul.ulstatus in (21,22,25)
                        AND ULSubmitDate BETWEEN '" + values.payload[0].dateFrom + @"' AND '" + values.payload[0].dateTo + @"'
                        group by ULSubmitDate
                        ) TB
                        WHERE ULTotalQuantity > 0
                        group by ULSubmitDate";

                if (values.payload[0].prefix == "Weekly")
                {
                    sQuery = byWeekPrefix + sQuery + byWeekSuffix;
                }
            }
            else if (values.operation == "filter_area_dashboard_by_all")
            {
                sQuery = @"SELECT DISTINCT ULSubmitDate,
                        SUM(ULTotalQuantity) as ULTotalQuantity,
                        SUM(ULTotalSKUs) as  ULTotalSKUs,
                        SUM(ULTotalAmount) as  ULTotalAmount,
                        SUM(ULTotalOrders) as  ULTotalOrders
                        from (
                        select CONVERT(VARCHAR(10), ULSubmitDate, 111) as ULSubmitDate,
                        SUM(sumulquantity) as ULTotalQuantity,
                        SUM(countulquantity) as ULTotalSKUs,
                        SUM(uiprice) as ULTotalAmount, count(*) as ULTotalOrders 
                        from tblRawFile left join tblUploadLog on ULFile = RFId left join
                        (SELECT DISTINCT
                        ul.ULFile,
                        ui.sumulquantity,
                        ui.countulquantity,
                        ul.ulstatus,
                        ui.uiprice 
                        FROM tblUploadLog ul
                        LEFT JOIN tblCustomerMaster
                        ON ul.ulcustomer = cmid 
                        LEFT JOIN tblUserAssignment ua 
                        ON ul.ulcustomer = ua.uacustomer 
                        LEFT JOIN
                        (SELECT
                        uiprice = SUM(uiquantity * ppprice),
                        sumulquantity = SUM(uiquantity),
                        countulquantity = COUNT(uiquantity),
                        uiid
                        FROM tblUploadItems
                        left join tbluploadlog
                        on ulid = uiid
                        left join tblCustomerMaster
                        on cmid = ulcustomer
                        LEFT JOIN tblProductPricing
                        on ppproduct = uiproduct and pparea = cmarea
                        WHERE uistatus NOT IN ('3','0')
                        and ULCustomer in (SELECT Distinct(CACustomer) from tblCustomerAssignment where CAAccount = '" + values.payload[0].acctype + @"')
                        and CMArea = '" + values.payload[0].cmArea + @"'
                        group by uiid ) ui 
                        ON ul.ulid = ui.uiid  
                        ) UL on ul.ULFile = RFId
                        where RFStatus = 1 AND ul.ulstatus in (21,22,25)
                        AND ULSubmitDate BETWEEN '" + values.payload[0].dateFrom + @"' AND '" + values.payload[0].dateTo + @"'
                        group by ULSubmitDate
                        ) TB
                        WHERE ULTotalQuantity > 0
                        group by ULSubmitDate";
                if (values.payload[0].prefix == "Weekly")
                {
                    sQuery = byWeekPrefix + sQuery + byWeekSuffix;
                }
            }
            else if (values.operation == "filter_donut_dashboard_by_default")
            {
                string year = DateTime.Now.Year.ToString();
                sQuery = @"select 
                        PMCategory,
                        PCDescription,
                        SUM(TotalQuantity) as STotalQuantity
                        from(
                        select
                        CONVERT(VARCHAR(10), ULSubmitDate, 111) as ULSubmitDate,
                        PMCategory,
                        TotalQuantity from tblRawFile
                        left join tblUploadLog on ULFile = RFId
                        left  join (
                        select UIId, PMCategory, sum(UIQuantity) as TotalQuantity
                        from tblUploadItems
                        left join tblProductMaster on pmid = UIProduct
                        where UIStatus not in ('0', '3')
                        group by UIId,PMCategory ) UI on UIId = ULId
                        where ULStatus in (21,22,25)
                        AND RFStatus = '1'
                        AND ULSubmitDate BETWEEN '" + year + @"-01-01 00:00:00.000' AND '" + year + @"-12-31 23:59:59.999'
                        ) UI left join tblProductCategory on PMCategory = PCId
                        group by PMCategory,PCDescription";
            }
            else if (values.operation == "filter_donut_dashboard_by_date")
            {
                sQuery = @"select 
                        PMCategory,
                        PCDescription,
                        SUM(TotalQuantity) as STotalQuantity
                        from(
                        select
                        CONVERT(VARCHAR(10), ULSubmitDate, 111) as ULSubmitDate,
                        PMCategory,
                        TotalQuantity from tblRawFile
                        left join tblUploadLog on ULFile = RFId
                        left join (
                        select UIId, PMCategory, sum(UIQuantity) as TotalQuantity
                        from tblUploadItems
                        left join tblProductMaster on pmid = UIProduct
                        where UIStatus not in ('0', '3')
                        group by UIId,PMCategory ) UI on UIId = ULId
                        where ULStatus in (21,22,25)
                        AND RFStatus = '1'
                        AND ULSubmitDate BETWEEN '" + values.payload[0].dateFrom + @"' AND '" + values.payload[0].dateTo + @"'
                        ) UI left join tblProductCategory on PMCategory = PCId
                        group by PMCategory,PCDescription";
            }
            else if (values.operation == "filter_donut_dashboard_by_area")
            {
                sQuery = @"select 
                        PMCategory,
                        PCDescription,
                        SUM(TotalQuantity) as STotalQuantity
                        from(
                        select
                        CONVERT(VARCHAR(10), ULSubmitDate, 111) as ULSubmitDate,
                        PMCategory,
                        TotalQuantity from tblRawFile
                        left join tblUploadLog on ULFile = RFId
                        left join (
                        select UIId, PMCategory, sum(UIQuantity) as TotalQuantity
                        from tblUploadItems
                        left join tblProductMaster on pmid = UIProduct
                        where UIStatus not in ('0', '3')
                        group by UIId,PMCategory ) UI on UIId = ULId
                        where ULStatus in (21,22,25)
                        AND ULCustomer in (SELECT CMId from tblCustomerMaster WHERE CMArea = '" + values.payload[0].cmArea + @"')
                        where RFStatus = '1'
                        AND ULSubmitDate BETWEEN '" + values.payload[0].dateFrom + @"' AND '" + values.payload[0].dateTo + @"'
                        ) UI left join tblProductCategory on PMCategory = PCId
                        group by PMCategory,PCDescription";
            }
            else if (values.operation == "filter_donut_dashboard_by_chain")
            {
                sQuery = @"select 
                        PMCategory,
                        PCDescription,
                        SUM(TotalQuantity) as STotalQuantity
                        from(
                        select
                        CONVERT(VARCHAR(10), ULSubmitDate, 111) as ULSubmitDate,
                        PMCategory,
                        TotalQuantity from tblRawFile
                        left join tblUploadLog on ULFile = RFId
                        left join (
                        select UIId, PMCategory, sum(UIQuantity) as TotalQuantity
                        from tblUploadItems
                        left join tblProductMaster on pmid = UIProduct
                        where UIStatus not in ('0', '3')
                        group by UIId,PMCategory ) UI on UIId = ULId
                        where ULStatus in (21,22,25)
                        AND ULCustomer in (SELECT Distinct(CACustomer) from tblCustomerAssignment where CAAccount = '" + values.payload[0].acctype + @"')
                        where RFStatus = '1'
                        AND ULSubmitDate BETWEEN '" + values.payload[0].dateFrom + @"' AND '" + values.payload[0].dateTo + @"'
                        ) UI left join tblProductCategory on PMCategory = PCId
                        group by PMCategory,PCDescription";
            }
            else if (values.operation == "filter_donut_dashboard_by_all")
            {
                sQuery = @"select 
                        PMCategory,
                        PCDescription,
                        SUM(TotalQuantity) as STotalQuantity
                        from(
                        select
                        CONVERT(VARCHAR(10), ULSubmitDate, 111) as ULSubmitDate,
                        PMCategory,
                        TotalQuantity from tblRawFile
                        left join tblUploadLog on ULFile = RFId
                        left join (
                        select UIId, PMCategory, sum(UIQuantity) as TotalQuantity
                        from tblUploadItems
                        left join tblProductMaster on pmid = UIProduct
                        where UIStatus not in ('0', '3')
                        group by UIId,PMCategory ) UI on UIId = ULId
                        where ULStatus in (21,22,25)
                        AND ULCustomer in (SELECT  Distinct(CMId) from tblCustomerMaster WHERE CMArea = '" + values.payload[0].cmArea + @"')
                        AND ULCustomer in (SELECT Distinct(CACustomer) from tblCustomerAssignment where CAAccount = '" + values.payload[0].acctype + @"')
                        AND RFStatus = '1'
                        AND ULSubmitDate BETWEEN '" + values.payload[0].dateFrom + @"' AND '" + values.payload[0].dateTo + @"'
                        ) UI left join tblProductCategory on PMCategory = PCId
                        group by PMCategory,PCDescription";
            }
            else
            {
                sQuery = @"SELECT DISTINCT ULSubmitDate,
                        SUM(ULTotalQuantity) as ULTotalQuantity,
                        SUM(ULTotalSKUs) as  ULTotalSKUs,
                        SUM(ULTotalAmount) as  ULTotalAmount,
                        SUM(ULTotalOrders) as  ULTotalOrders
                        from (
                        select CONVERT(VARCHAR(10), ULSubmitDate, 111) as ULSubmitDate,
                        SUM(sumulquantity) as ULTotalQuantity,
                        SUM(countulquantity) as ULTotalSKUs,
                        SUM(uiprice) as ULTotalAmount, count(*) as ULTotalOrders 
                        from tblRawFile left join tblUploadLog on ULFile = RFId left join
                        (SELECT DISTINCT
                        ul.ULFile,
                        ui.sumulquantity,
                        ui.countulquantity,
                        ul.ulstatus,
                        ui.uiprice 
                        FROM tblUploadLog ul
                        LEFT JOIN tblCustomerMaster
                        ON ul.ulcustomer = cmid 
                        LEFT JOIN tblUserAssignment ua 
                        ON ul.ulcustomer = ua.uacustomer 
                        LEFT JOIN
                        (SELECT
                        uiprice = SUM(uiquantity * ppprice),
                        sumulquantity = SUM(uiquantity),
                        countulquantity = COUNT(uiquantity),
                        uiid
                        FROM tblUploadItems
                        left join tbluploadlog
                        on ulid = uiid
                        left join tblCustomerMaster
                        on cmid = ulcustomer
                        LEFT JOIN tblProductPricing
                        on ppproduct = uiproduct and pparea = cmarea
                        WHERE uistatus NOT IN ('3','0')
                        group by uiid ) ui 
                        ON ul.ulid = ui.uiid  
                        ) UL on ul.ULFile = RFId
                        where RFStatus = 1 AND ul.ulstatus in (21,22,25)
                        AND ULSubmitDate BETWEEN '" + values.payload[0].dateFrom + @"' AND '" + values.payload[0].dateTo + @"'
                        group by ULSubmitDate
                        ) TB
                        WHERE ULTotalQuantity > 0
                        group by ULSubmitDate";
                if (values.payload[0].prefix == "Weekly")
                {
                    sQuery = byWeekPrefix + sQuery + byWeekSuffix;
                }
            }

            SqlCommand cmd = new SqlCommand(sQuery, connection);
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            try
            {
                connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        row = new Dictionary<string, object>();
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            var cName = dr.GetName(i);
                            row.Add(cName, dr[cName]);
                        }
                        rows.Add(row);
                    }
                }
                else
                    success = false;
            }
            catch (Exception ex)
            {
                success = false;
                row = new Dictionary<string, object>();
                row.Add("Error", ex.Message);
                rows.Add(row);
            }

            return new Response { success = success, detail = rows };
        }

        // POST api/default1
        public void Post([FromBody]string value)
        {
        }

        // PUT api/default1/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/default1/5
        public void Delete(int id)
        {
        }
    }
}