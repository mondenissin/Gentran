using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Gentran.Controllers.api.Admin
{
    public class ReportsController : ApiController
    {
        string userID = HttpContext.Current.Session["UserId"].ToString();
        List<Transaction> trows = new List<Transaction>();
        // GET api/reports
        public object Get()
        {
            bool success = true;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String sQuery = "";

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            try
            {
                sQuery = "select umid,umfirstname,umlastname from tblusermaster where umtype != 'DEV' order by umfirstname";
                SqlCommand cmd = new SqlCommand(sQuery, connection);
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

        // GET api/reports/5
        public string Get(int id)
        {   
            return "value";
        }

        // POST api/reports
        public void Post([FromBody]string value)
        {
        }

        // PUT api/reports/5
        public object Put([FromBody]Data values)
        {
            bool success = true;
            string response = "Successful";
            string dateFilter = "";
            String sQuery = "";
            String reportAct = "",reportType = "";
            DateTime now = new DateTime();
            now = DateTime.Now;
            
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            try
            {
                dateFilter = values.payload[0].reportDate == "odate" ? "a.ulorderdate" : "a.uldeliverydate";

                if (values.reportName == "purchase") {
                    reportType = "PO Report";
                    reportAct = "REP20";
                    if (values.operation == "all")
                    {
                        sQuery = @"select top 100 a.ulponumber,a.ulcustomer,left(a.ulorderdate,12) as ulorderdate,
                                left(a.ULDeliveryDate,12) as uldeliverydate,left(r.RFRetrieveDate,12) + '- ' + CONVERT (varchar(15),CAST(r.RFRetrieveDate as time),100) as ulretrievedate,
                                left(r.RFReadDate,12) + '- ' + CONVERT (varchar(15),CAST(r.RFReadDate as time),100) as ulreaddate,
                                (case CAST(r.RFSubmitDate as time) when CAST('00:00' as time) then 'Not yet Submitted' else left(r.RFSubmitDate,12) + '- ' + CONVERT (varchar(15),CAST(r.RFSubmitDate as time),100) end) as ulsubmitdate,
                                (case r.RFSubmitUser when '' then 'Not yet Submitted' else r.RFSubmitUser end) as ulsubmituser,
                                (case a.ulstatus when '20' then 'New' when '0' then 'Deleted' else 'Failed' end) as ulstatus,ui.countulquantity,ui.sumulquantity,ui.uiprice from tbluploadlog a 
                                left join tblrawfile r on a.ulfile = r.RFId 
                                left join tblcustomermaster b on a.ulcustomer = b.cmid 
                                left join tblusermaster c on c.umid = r.RFSubmitUser 
                                left join (SELECT SUM(uiquantity * ppprice) as uiprice, SUM(uiquantity) as sumulquantity,COUNT(uiquantity) as countulquantity,uiid FROM tblUploadItems 
                                left join tblrawfile on RFId = uiid left join tblUploadLog on ULFile = RFId left join tblCustomerMaster on cmid = ulcustomer
                                left join tblProductPricing on ppproduct = uiproduct and pparea = cmarea WHERE uistatus NOT IN ('3','0') group by uiid ) ui on a.ULFile = ui.UIId  ";

                        if (values.payload[0].reportType == "latepo"){
                            sQuery += "where CAST(r.RFReadDate as time) > CAST('12:00' as time) ";
                        }else if (values.payload[0].reportType == "latesubpo"){
                            sQuery += "where CAST(r.RFSubmitDate as date) != CAST(r.RFReadDate as date) and a.ulstatus = 25 ";
                        }else if (values.payload[0].reportType == "queuedpo"){
                            sQuery += "where a.ulstatus <= 20";
                        }else if (values.payload[0].reportType == "submitpo"){
                            sQuery += "where a.ulstatus = 25";
                        }

                        sQuery += "order by "+ dateFilter + " desc";

                        response = "By all";
                    }
                    else if (values.operation == "by_user")
                    {
                        sQuery = @"select a.ulponumber,a.ulcustomer,left(a.ulorderdate,12) as ulorderdate,
                                left(a.ULDeliveryDate,12) as uldeliverydate,left(r.RFRetrieveDate,12) + '- ' + CONVERT (varchar(15),CAST(r.RFRetrieveDate as time),100) as ulretrievedate,
                                left(r.RFReadDate,12) + '- ' + CONVERT (varchar(15),CAST(r.RFReadDate as time),100) as ulreaddate,
                                (case CAST(r.RFSubmitDate as time) when CAST('00:00' as time) then 'Not yet Submitted' else left(r.RFSubmitDate,12) + '- ' + CONVERT (varchar(15),CAST(r.RFSubmitDate as time),100) end) as ulsubmitdate,
                                (case r.RFSubmitUser when '' then 'Not yet Submitted' else r.RFSubmitUser end) as ulsubmituser,
                                (case a.ulstatus when '20' then 'New' when '0' then 'Deleted' else 'Failed' end) as ulstatus,ui.countulquantity,ui.sumulquantity,ui.uiprice from tbluploadlog a 
                                left join tblrawfile r on a.ulfile = r.RFId 
                                left join tblcustomermaster b on a.ulcustomer = b.cmid 
                                left join tblusermaster c on c.umid = r.RFSubmitUser 
                                left join (SELECT SUM(uiquantity * ppprice) as uiprice, SUM(uiquantity) as sumulquantity,COUNT(uiquantity) as countulquantity,uiid FROM tblUploadItems 
                                left join tblrawfile on RFId = uiid left join tblUploadLog on ULFile = RFId left join tblCustomerMaster on cmid = ulcustomer
                                left join tblProductPricing on ppproduct = uiproduct and pparea = cmarea WHERE uistatus NOT IN ('3','0') group by uiid ) ui on a.ULFile = ui.UIId 
                                where r.RFReadUser = '" + values.payload[0].ULUser + "' and "+ dateFilter + " between '" + values.payload[0].dateFrom + "' and '" + values.payload[0].dateTo + "' ";

                        if (values.payload[0].reportType == "latepo") {
                            sQuery += "and CAST(r.RFReadDate as time) < CAST('12:00' as time) ";
                        } else if (values.payload[0].reportType == "latesubpo") {
                            sQuery += "and CAST(r.RFSubmitDate as date) != CAST(r.RFReadDate as date) and a.ulstatus = 25 ";
                        } else if (values.payload[0].reportType == "queuedpo") {
                            sQuery += "and a.ulstatus <= 20";
                        } else if (values.payload[0].reportType == "submitpo") {
                            sQuery += "and a.ulstatus = 25";
                        }
                        
                        sQuery += "order by " + dateFilter + " desc";

                        response = "By user";
                    }
                    else
                    {
                        sQuery = @"select a.ulponumber,a.ulcustomer,left(a.ulorderdate,12) as ulorderdate,
                                left(a.ULDeliveryDate,12) as uldeliverydate,left(r.RFRetrieveDate,12) + '- ' + CONVERT (varchar(15),CAST(r.RFRetrieveDate as time),100) as ulretrievedate,
                                left(r.RFReadDate,12) + '- ' + CONVERT (varchar(15),CAST(r.RFReadDate as time),100) as ulreaddate,
                                (case CAST(r.RFSubmitDate as time) when CAST('00:00' as time) then 'Not yet Submitted' else left(r.RFSubmitDate,12) + '- ' + CONVERT (varchar(15),CAST(r.RFSubmitDate as time),100) end) as ulsubmitdate,
                                (case r.RFSubmitUser when '' then 'Not yet Submitted' else r.RFSubmitUser end) as ulsubmituser,
                                (case a.ulstatus when '20' then 'New' when '0' then 'Deleted' else 'Failed' end) as ulstatus,ui.countulquantity,ui.sumulquantity,ui.uiprice from tbluploadlog a 
                                left join tblrawfile r on a.ulfile = r.RFId 
                                left join tblcustomermaster b on a.ulcustomer = b.cmid 
                                left join tblusermaster c on c.umid = r.RFSubmitUser 
                                left join (SELECT SUM(uiquantity * ppprice) as uiprice, SUM(uiquantity) as sumulquantity,COUNT(uiquantity) as countulquantity,uiid FROM tblUploadItems 
                                left join tblrawfile on RFId = uiid left join tblUploadLog on ULFile = RFId left join tblCustomerMaster on cmid = ulcustomer
                                left join tblProductPricing on ppproduct = uiproduct and pparea = cmarea WHERE uistatus NOT IN ('3','0') group by uiid ) ui on a.ULFile = ui.UIId 
                                where " + dateFilter + " between '" + values.payload[0].dateFrom + "' and '" + values.payload[0].dateTo + "' ";

                        if (values.payload[0].reportType == "latepo"){
                            sQuery += "and CAST(r.RFReadDate as time) < CAST('12:00' as time) ";
                        }else if (values.payload[0].reportType == "latesubpo"){
                            sQuery += "and CAST(r.RFSubmitDate as date) != CAST(r.RFReadDate as date) and a.ulstatus = 25 ";
                        }else if (values.payload[0].reportType == "queuedpo"){
                            sQuery += "and a.ulstatus <= 20";
                        }else if (values.payload[0].reportType == "submitpo"){
                            sQuery += "and a.ulstatus = 25";
                        }

                        sQuery += "order by " + dateFilter + " desc";

                        response = "By normal";
                    }
                }
                else if (values.reportName == "product"){
                    reportType = "Product Report";
                    reportAct = "REP10";

                    ///prodcategory
                    
                    if (values.operation == "all")
                    {
                        sQuery = @"select top 100 a.ulponumber,a.ulorderdate,uiproduct as pmid,uiquantity,pmcode,pmdescription,(select pcdescription from tblproductcategory where pcid = pmcategory ) as pmcategory,
                                (case pmstatus when '1' then 'Active' else 'Inactive' end) as pmstatus,ppprice,amdescription from tbluploaditems left join
                                tbluploadlog a on uiid = a.ulfile left join tblproductmaster on uiproduct = pmid left join tblProductPricing
                                on PPProduct = PMId left join tblAreaMaster on AMId = PPArea where PPPrice > 0 ";

                        if (values.payload[0].reportType == "queuedsku"){
                            sQuery += "and a.ulstatus <= '20' ";
                        }else if (values.payload[0].reportType == "submitsku"){
                            sQuery += "and a.ulstatus = '25' ";
                        }

                        sQuery += "order by " + dateFilter + " desc";

                        response = "By all";
                    }
                    else if (values.operation == "by_category")
                    {
                        sQuery = @"select a.ulponumber,a.ulorderdate,uiproduct as pmid,uiquantity,pmcode,pmdescription,(select pcdescription from tblproductcategory where pcid = pmcategory ) as pmcategory,
                                (case pmstatus when '1' then 'Active' else 'Inactive' end) as pmstatus,ppprice,amdescription from tbluploaditems left join
                                tbluploadlog a on uiid = a.ulfile left join tblproductmaster on uiproduct = pmid left join tblProductPricing
                                on PPProduct = PMId left join tblAreaMaster on AMId = PPArea where PPPrice > 0 and PMCategory = '" + values.payload[0].prodcategory+"' and "+dateFilter+ @"
                                between '" + values.payload[0].dateFrom + "' and '" + values.payload[0].dateTo + "' ";

                        if (values.payload[0].reportType == "queuedsku"){
                            sQuery += "and a.ulstatus <= '20' ";
                        }else if (values.payload[0].reportType == "submitsku"){
                            sQuery += "and a.ulstatus = '25' ";
                        }

                        sQuery += "order by "+dateFilter+" desc";
                        response = "By category";
                    }
                    else
                    {
                        sQuery = @"select a.ulponumber,a.ulorderdate,uiproduct as pmid,uiquantity,pmcode,pmdescription,(select pcdescription from tblproductcategory where pcid = pmcategory ) as pmcategory,
                                (case pmstatus when '1' then 'Active' else 'Inactive' end) as pmstatus,ppprice,amdescription from tbluploaditems left join
                                tbluploadlog a on uiid = a.ulfile left join tblproductmaster on uiproduct = pmid left join tblProductPricing
                                on PPProduct = PMId left join tblAreaMaster on AMId = PPArea where PPPrice > 0 and " + dateFilter + @"
                                between '" + values.payload[0].dateFrom + "' and '" + values.payload[0].dateTo + "' ";

                        if (values.payload[0].reportType == "queuedsku"){
                            sQuery += "and a.ulstatus <= '20' ";
                        }else if (values.payload[0].reportType == "submitsku"){
                            sQuery += "and a.ulstatus = '25' ";
                        }

                        sQuery += "order by " + dateFilter + " desc";

                        response = "By normal";
                    }
                }
                
                connection.Open();
                SqlCommand cmd = new SqlCommand(sQuery, connection);
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                cmd = new SqlCommand(sQuery, connection);

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

                string newpaypload = JsonConvert.SerializeObject(values.payload, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                trows.Add(new Transaction { activity = reportAct, date = now, remarks = response, user = userID, type = "ADM", value = reportType, changes = "", payloadvalue = newpaypload, customernumber = "", ponumber = "" });
                return new Response { success = success, reports = rows, detail = trows, notiftext = response };
            }
            catch (Exception ex)        
            {
                success = false;
                response = ex.Message;
            }

            return new Response { success = success, detail = response };
        }

        // DELETE api/reports/5
        public void Delete(int id)
        {
        }
    }
}
