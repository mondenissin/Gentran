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
            String sQuery = @"select distinct  RFRetrieveDate,
                        SUM(ULTotalQuantity) as ULTotalQuantity,
                        SUM(ULTotalSKUs) as  ULTotalSKUs,
                        SUM(ULTotalAmount) as  ULTotalAmount,
                        SUM(ULTotalOrders) as  ULTotalOrders
                        from (
                        select CONVERT(VARCHAR(10), RFRetrieveDate, 111) as RFRetrieveDate,
                        SUM(sumulquantity) as ULTotalQuantity,
                        SUM(countulquantity) as ULTotalSKUs,
                        SUM(uiprice) as ULTotalAmount, count(*) as ULTotalOrders 
                        from tblRawFile left join
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
                        on ulfile = uiid
                        left join tblCustomerMaster
                        on cmid = ulcustomer
                        LEFT JOIN tblProductPricing
                        on ppproduct = uiproduct and pparea = cmarea
                        WHERE uistatus NOT IN ('3','0')
                        group by uiid ) ui 
                        ON ul.ulfile = ui.uiid  
                        ) UL on ul.ULFile = RFId
                        group by RFRetrieveDate
                        ) TB group by RFRetrieveDate";
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

            if (values.operation == "filter_dashboard_date")
            {
                sQuery = @"select distinct  RFRetrieveDate,
                        SUM(ULTotalQuantity) as ULTotalQuantity,
                        SUM(ULTotalSKUs) as  ULTotalSKUs,
                        SUM(ULTotalAmount) as  ULTotalAmount,
                        SUM(ULTotalOrders) as  ULTotalOrders
                        from (
                        select CONVERT(VARCHAR(10), RFRetrieveDate, 111) as RFRetrieveDate,
                        SUM(sumulquantity) as ULTotalQuantity,
                        SUM(countulquantity) as ULTotalSKUs,
                        SUM(uiprice) as ULTotalAmount, count(*) as ULTotalOrders 
                        from tblRawFile left join
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
                        ) UL on ul.ulfile = RFId
                        where  RFRetrieveDate BETWEEN '" + values.payload[0].dateFrom + @"' AND '" + values.payload[0].dateTo + @"'
                         group by RFRetrieveDate
                        ) TB group by RFRetrieveDate";
            }
            else {
                sQuery = @"select distinct  RFRetrieveDate,
                        SUM(ULTotalQuantity) as ULTotalQuantity,
                        SUM(ULTotalSKUs) as  ULTotalSKUs,
                        SUM(ULTotalAmount) as  ULTotalAmount,
                        SUM(ULTotalOrders) as  ULTotalOrders
                        from (
                        select CONVERT(VARCHAR(10), RFRetrieveDate, 111) as RFRetrieveDate,
                        SUM(sumulquantity) as ULTotalQuantity,
                        SUM(countulquantity) as ULTotalSKUs,
                        SUM(uiprice) as ULTotalAmount, count(*) as ULTotalOrders 
                        from tblRawFile left join
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
                        group by RFRetrieveDate
                        ) TB group by RFRetrieveDate";
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