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
            String sQuery = "";
            String reportAct = "",reportType = "";
            DateTime now = new DateTime();
            now = DateTime.Now;
            
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            try
            {
                if (values.reportName == "purchase") {
                    reportType = "PO Report";
                    reportAct = "REP20";
                    if (values.operation == "all")
                    {
                        sQuery = "select top 100 a.ulponumber,b.cmdescription,left(a.ulorderdate,12) as ulorderdate,(case a.ulstatus when '20' then 'New' else 'Failed' end) as ulstatus, c.umusername from tbluploadlog a left join tblcustomermaster b on a.ulcustomer = b.cmid left join tblusermaster c on c.umid = a.uluser order by a.ulorderdate desc";
                        response = "By all";
                    }
                    else if (values.operation == "by_user")
                    {
                        sQuery = "select a.ulponumber,b.cmdescription,left(a.ulorderdate,12) as ulorderdate,(case a.ulstatus when '20' then 'New' else 'Failed' end) as ulstatus, c.umusername from tbluploadlog a left join tblcustomermaster b on a.ulcustomer = b.cmid left join tblusermaster c on c.umid = a.uluser where a.uluser = '" + values.payload[0].ULUser + "' and a.ulorderdate between '" + values.payload[0].dateFrom + "' and '" + values.payload[0].dateTo + "' order by a.ulorderdate desc";
                        response = "By user";
                    }
                    else
                    {
                        sQuery = "select a.ulponumber,b.cmdescription,left(a.ulorderdate,12) as ulorderdate,(case a.ulstatus when '20' then 'New' else 'Failed' end) as ulstatus, c.umusername from tbluploadlog a left join tblcustomermaster b on a.ulcustomer = b.cmid left join tblusermaster c on c.umid = a.uluser where a.ulorderdate between '" + values.payload[0].dateFrom + "' and '" + values.payload[0].dateTo + "' order by a.ulorderdate desc";
                        response = "By normal";
                    }
                }
                else if (values.reportName == "product"){
                    reportType = "Transation Report";
                    reportAct = "REP10";
                    if (values.operation == "all")
                    {
                        sQuery = "select top 100 TLId,TLRemarks,TLValue,Left(TLDate,12) as TLDate,TADescription,UMUsername from tbltransactionlog left join tblTransactionActivity on taid = tlactivity left join tblusermaster on umid = tluser order by tlid desc";
                        response = "By all";
                    }
                    else if (values.operation == "by_user")
                    {
                        sQuery = "select TLId,TLRemarks,TLValue,Left(TLDate,12) as TLDate,TADescription,UMUsername from tbltransactionlog left join tblTransactionActivity on taid = tlactivity left join tblusermaster on umid = tluser where tluser = '" + values.payload[0].ULUser + "' and tldate between '" + values.payload[0].dateFrom + "' and '" + values.payload[0].dateTo + "'order by tlid desc";
                        response = "By user";
                    }
                    else
                    {
                        sQuery = "select TLId,TLRemarks,TLValue,Left(TLDate,12) as TLDate,TADescription,UMUsername from tbltransactionlog left join tblTransactionActivity on taid = tlactivity left join tblusermaster on umid = tluser where tldate between '" + values.payload[0].dateFrom + "' and '" + values.payload[0].dateTo + "' order by tlid desc";
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
