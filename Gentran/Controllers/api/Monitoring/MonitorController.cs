using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using Newtonsoft.Json;
using System.Web;
using System.Data;

namespace Gentran.Controllers.api.Monitor
{
    public class MonitorController : ApiController
    {
        string userID = HttpContext.Current.Session["UserId"].ToString();
        List<Transaction> rows = new List<Transaction>();
        // GET api/default1
        public Object Get()
        {
            Boolean success = false;
            try
            {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);

            String selectStr = "";

            selectStr = @"SELECT DISTINCT
                            ur.rfid,
                            ul.ulid,
                            ul.ulponumber,
                            cm.cmcode as ulcustomer,
                            cm.cmdescription, 
                            LEFT(ul.ulorderdate,12) AS ulorderdate,
                            LEFT(ul.uldeliverydate,12) AS uldeliverydate,
                            LEFT(ul.ulcanceldate,12) AS ulcanceldate,
                            ul.ulreaddate as sortupload,
                            LEFT(ul.ulreaddate,12) +'- ' + CONVERT(varchar(15), CAST(ul.ulreaddate as time), 100) AS ulreaddate,
                            LEFT(ul.ulsubmitdate,12) +'- ' + CONVERT(varchar(15), CAST(ul.ulsubmitdate as time), 100) AS ulsubmitdate,
                            ui.sumulquantity,
                            ui.countulquantity,
                            ul.ulstatus,
                            us.usdescription,
                            ul.ulremarks,
                            ur.rffilename,
                            ui.uiprice,
                            ur.rfaccount
                            FROM tblUploadLog ul
                            LEFT JOIN tblUploadStatus us
                            on ulstatus = usid
                            LEFT JOIN tblCustomerMaster cm
                            ON ul.ulcustomer = cm.cmid 
                            LEFT JOIN tblUserAssignment ua 
                            ON ul.ulcustomer = ua.uacustomer 
                            LEFT JOIN tblRawFile ur
                            ON ur.RFId = ul.ULFile
                            LEFT JOIN
                            (SELECT
                            uiprice = SUM(uiquantity * ppprice),
                            sumulquantity = SUM(uiquantity),
                            countulquantity = COUNT(uiquantity),
                            uiid
                            FROM tblUploadItems
                            left join tblUploadLog
                            on ulid = uiid
                            left join tblCustomerMaster
                            on cmid = ulcustomer
                            LEFT JOIN tblProductPricing
                            on ppproduct = uiproduct and pparea = cmarea
                            WHERE uistatus NOT IN ('4','3','0')
                            group by uiid ) ui 
                            ON ul.ulid = ui.uiid 
                            WHERE uatype = 'KAS' 
                            AND ulstatus != 25
                            OR ulstatus != 21  
                            AND ul.ulReadDate = GETDATE()
                            ORDER BY sortupload desc"; //Dec. 6, 2016 // Dec 16 - ua.uauser = '17002'

                SqlDataAdapter dataadapter = new SqlDataAdapter(selectStr, connection);
                connection.Open();
                dataadapter.Fill(dt);

                List<Dictionary<object, object>> rows = new List<Dictionary<object, object>>();
                Dictionary<object, object> row;
                foreach (DataRow dr in dt.Rows)
                {
                    string icon = "";
                    string sStat = "";
                    string oClass = "";
                    int eCtr = 0;
                    row = new Dictionary<object, object>();
                    foreach (DataColumn col in dt.Columns)
                    {

                        if (col.ColumnName == "ulstatus" && (dr[col].ToString() == "11" || dr[col].ToString() == "12"))
                        {
                            sStat = dr["usdescription"].ToString();
                            icon = "fa-exclamation-circle";
                            oClass = "label label-danger";
                        }
                        else if (col.ColumnName == "ulstatus" && dr[col].ToString() == "20")
                        {
                            sStat = dr["usdescription"].ToString();
                            icon = "fa-check";
                            oClass = "label label-success";
                        }
                        row.Add(col.ColumnName, dr[col].ToString());
                    }
                    row.Add("sStatus", sStat);
                    row.Add("uicons", icon);
                    row.Add("eCtr", eCtr);
                    row.Add("oClass", oClass);
                    rows.Add(row);
                }

                connection.Close();

                success = true;

                if (dt.Rows.Count == 0)
                {
                    success = false;
                }
                return new Response { success = success, detail = rows };
            }
            catch (Exception ex)
            {
                success = false;
                return new Response { success = success, detail = ex.Message };
            }
            
        }
        
        // GET api/order/5
        public object Get([FromUri]string value)
        {
            Data values = JsonConvert.DeserializeObject<Data>(value);

            bool success = false;
            try
            {
                DataTable dt = new DataTable();
                SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String selectStr = "";

            selectStr = @"SELECT DISTINCT
                            ur.rfid,
                            ul.ulid,
                            ul.ulponumber,
                            cm.cmcode as ulcustomer,
                            cm.cmdescription, 
                            LEFT(ul.ulorderdate,12) AS ulorderdate,
                            LEFT(ul.uldeliverydate,12) AS uldeliverydate,
                            LEFT(ul.ulcanceldate,12) AS ulcanceldate,
                            ul.ulreaddate as sortupload,
                            LEFT(ul.ulreaddate,12) +'- ' + CONVERT(varchar(15), CAST(ul.ulreaddate as time), 100) AS ulreaddate,
                            LEFT(ul.ulsubmitdate,12) +'- ' + CONVERT(varchar(15), CAST(ul.ulsubmitdate as time), 100) AS ulsubmitdate,
                            ui.sumulquantity,
                            ui.countulquantity,
                            ul.ulstatus,
                            us.usdescription,
                            ul.ulremarks,
                            ur.rffilename,
                            ui.uiprice,
                            ur.rfaccount
                            FROM tblUploadLog ul
                            LEFT JOIN tblUploadStatus us
                            on ulstatus = usid
                            LEFT JOIN tblCustomerMaster cm
                            ON ul.ulcustomer = cm.cmid 
                            LEFT JOIN tblUserAssignment ua 
                            ON ul.ulcustomer = ua.uacustomer 
                            LEFT JOIN tblRawFile ur
                            ON ur.RFId = ul.ULFile
                            LEFT JOIN
                            (SELECT
                            uiprice = SUM(uiquantity * ppprice),
                            sumulquantity = SUM(uiquantity),
                            countulquantity = COUNT(uiquantity),
                            uiid
                            FROM tblUploadItems
                            left join tblUploadLog
                            on ulid = uiid
                            left join tblCustomerMaster
                            on cmid = ulcustomer
                            LEFT JOIN tblProductPricing
                            on ppproduct = uiproduct and pparea = cmarea
                            WHERE uistatus NOT IN ('4','3','0')
                            group by uiid ) ui 
                            ON ul.ulid = ui.uiid 
                            WHERE uatype = 'KAS' 
                            AND (ulstatus != 25
                            OR ulstatus != 21)  
                            AND (ul.ulReadDate >= '" + values.payload[0].dateFrom + "' and  ul.ulReadDate <= dateadd(day,1,'" + values.payload[0].dateTo + "')) ORDER BY sortupload desc"; //Dec. 6, 2016
                
                SqlDataAdapter dataadapter = new SqlDataAdapter(selectStr, connection);
                connection.Open();
                dataadapter.Fill(dt);

                List<Dictionary<object, object>> rows = new List<Dictionary<object, object>>();
                Dictionary<object, object> row;
                foreach (DataRow dr in dt.Rows)
                {
                    string icon = "";
                    string sStat = "";
                    string oClass = "";
                    int eCtr = 0;
                    row = new Dictionary<object, object>();
                    foreach (DataColumn col in dt.Columns)
                    {

                        if (col.ColumnName == "ulstatus" && dr[col].ToString() == "11" || dr[col].ToString() == "12")
                        {
                            sStat = dr["usdescription"].ToString();
                            icon = "fa-exclamation-circle";
                            oClass = "label label-danger";
                        }
                        else if (col.ColumnName == "ulstatus" && dr[col].ToString() == "20")
                        {
                            sStat = dr["usdescription"].ToString();
                            icon = "fa-check";
                            oClass = "label label-success";
                        }
                        row.Add(col.ColumnName, dr[col].ToString());
                    }
                    row.Add("sStatus", sStat);
                    row.Add("uicons", icon);
                    row.Add("eCtr", eCtr);
                    row.Add("oClass", oClass);
                    rows.Add(row);
                }

                connection.Close();

                success = true;

                if (dt.Rows.Count == 0)
                {
                    success = false;
                }
                return new Response { success = success, detail = rows };
            }
            catch (Exception ex)
            {
                success = false;
                return new Response { success = success, detail = ex.Message };
            }
        }

        // POST api/order
        public object Post([FromBody]Data values)
        {
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String sQuery = "";
            SqlCommand cmd; 
            SqlDataReader dr;  

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            bool success = false;
            string response = ""; 

            string ulid = "";
            string pmid = "";
            string pmcode = "";
            string pacode = "";
            string acctype = "";


            try {
                for (int i = 0; i < values.payload.Count; i++) {
                    ulid = values.payload[i].ULId;
                    acctype = values.payload[i].acctype;
                    pmcode = values.payload[i].pmcode;
                    pacode = values.payload[i].pacode;

                    sQuery = "select top 1 * from tblProductMaster where pmcode = '"+ pmcode +"'";
                    connection.Open();
                    cmd = new SqlCommand(sQuery, connection);
                    dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        pmid = dr["PMId"].ToString();
                        dr.Close();
                        connection.Close();

                        sQuery = "select top 1 * from tblProductAssignment left join tblAccountType on PAAccount = ATId where paproduct = '" + pmid + "' and paaccount = '" + acctype + "'";
                        connection.Open();
                        cmd = new SqlCommand(sQuery, connection);
                        dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            dr.Read();       
                            string account = dr["ATDescription"].ToString();
                            dr.Close();
                            connection.Close();
                             
                            response = "SKU " + pmcode + " already has assignment for "+ account;
                            row = new Dictionary<string, object>();
                            row.Add("status", false);
                            row.Add("response", response);
                            rows.Add(row);
                        }
                        else
                        {
                            connection.Close();

                            sQuery = "select top 1 * from tblProductAssignment left join tblAccountType on PAAccount = ATId where pacode = '" + pacode  + "' and paaccount = '" + acctype + "'";
                            connection.Open();
                            cmd = new SqlCommand(sQuery, connection);
                            dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                dr.Read();
                                string account = dr["ATDescription"].ToString();
                                dr.Close();
                                connection.Close();

                                response = "Product Code " + pacode + " already has assignment for " + account;
                                row = new Dictionary<string, object>();
                                row.Add("status", false);
                                row.Add("response", response);
                                rows.Add(row);
                            }
                            else
                            {
                                connection.Close();

                                sQuery = "INSERT INTO tblProductAssignment select '" + pacode + "','" + pmid + "','" + acctype + "'";
                                connection.Open();
                                cmd = new SqlCommand(sQuery, connection);
                                cmd.ExecuteNonQuery();
                                connection.Close();

                                sQuery = "UPDATE tblUploadItems set uiproduct = '" + pmid + "', uistatus='1' where uiid in (select ULId from tblUploadLog left join tblRawFile on ULFile = RFId where RFAccount = '" + acctype + "') AND uicode='" + pacode + "'";
                                connection.Open();
                                cmd = new SqlCommand(sQuery, connection);
                                cmd.ExecuteNonQuery();
                                connection.Close();

                                sQuery = "delete from tblErrorLog where elid in (select ULId from tblUploadLog left join tblRawFile on ULFile = RFId where RFAccount = '" + acctype + "') AND eldetail='" + pacode + "'";
                                connection.Open();
                                cmd = new SqlCommand(sQuery, connection);
                                cmd.ExecuteNonQuery();
                                connection.Close();

                                response = "SKU " + pmcode + " assignment saved!";
                                row = new Dictionary<string, object>();
                                row.Add("status", true);
                                row.Add("response", response);
                                rows.Add(row);

                                // <!--- MNS20161209
                                // Checking of affected POs, starts here
                                sQuery = @"SELECT distinct ULId FROM tblUploadLog left join tblUploadItems on ULId = UIId
                                           where ULStatus = '11'";
                                connection.Open(); 
                                cmd = new SqlCommand(sQuery, connection);
                                dr = cmd.ExecuteReader();
                                if (dr.HasRows)
                                {
                                    List<string> ulids = new List<string>();

                                    while (dr.Read())
                                    {
                                        ulids.Add(dr["ULId"].ToString());
                                      
                                    }
                                    dr.Close();
                                    connection.Close();

                                    foreach (string id in ulids) {
                                        sQuery = "select * from tblUploadItems where uiid ='" + id + "' AND uistatus in ('0','3','4')";

                                        cmd = new SqlCommand(sQuery, connection);
                                        connection.Open();
                                        dr = cmd.ExecuteReader();
                                        if (!dr.HasRows)
                                        {
                                            connection.Close();
                                            sQuery = "UPDATE tblUploadLog set ulstatus='20' where ulid ='" + id + "' and ulstatus = '11' ";
                                            connection.Open();
                                            cmd = new SqlCommand(sQuery, connection);
                                            cmd.ExecuteNonQuery();
                                            connection.Close();
                                        }
                                        else {
                                            connection.Close();
                                        }
                                    }

                                }
                                else
                                {
                                    connection.Close();
                                }
                                // Checking of affected POs, ends here
                                // --- MNS20161209 ----->

                            }
                            

                        }

                    }
                    else {

                        connection.Close();
                        response = "SKU "+ pmcode + " not found.";  
                        row = new Dictionary<string, object>();
                        row.Add("status", false);
                        row.Add("response", response);
                        rows.Add(row);
                    }  
                }
                success = true;
            }
            catch (Exception ex) {
                connection.Close();
                success = false;
                row = new Dictionary<string, object>();
                row.Add("response", ex.Message);
                rows.Add(row);
            }
            return new Response { success = success, detail = rows };
        }

        // PUT api/order/5
        public object Put([FromBody]Data values)
        {
            bool success = true;
            string response = "Successful";
            string CustomerNumber = "";
            string error = "";
            String status = "", sQuery = string.Empty;
            DateTime now = new DateTime();
            now = DateTime.Now;

            if (values.payload[0].ulstatus == "15")
            {
                status = "20";
            }
            else
            {
                status = values.payload[0].ulstatus;
            }

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            try
            {
                String selectCMId = "select cmid from tblcustomermaster where cmcode ='" + values.payload[0].customernumber + "'";
                connection.Open();
                SqlCommand selectCmdCMId = new SqlCommand(selectCMId, connection);
                SqlDataReader dr = selectCmdCMId.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    CustomerNumber = dr["cmid"].ToString();
                    connection.Close();
                    if (CustomerNumber == "0")
                    {

                        success = false;
                        response = "Please enter a Customer Code!";
                    }
                    else
                    {
                        String updateULId = "update tbluploadlog set ULStatus = '" + status + "', ulcustomer = '" + CustomerNumber + "', ULDeliveryDate = '" + values.payload[0].DeliveryDate + "', ulremarks = '" + values.payload[0].remarks + "' where ulid = '" + values.payload[0].ULId + "'";
                       
                        connection.Open();
                        SqlCommand updateCmdULId = new SqlCommand(updateULId, connection);
                        updateCmdULId.ExecuteNonQuery();
                        connection.Close();

                        string newpaypload = JsonConvert.SerializeObject(values.payload, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                        rows.Add(new Transaction { activity = "EDI20", date = now, remarks = values.payload[0].changes, user = userID, type = "ADM", value = "PO ID:" + values.payload[0].ULId, changes = "", payloadvalue = newpaypload, customernumber = "", ponumber = "" });
                        return new Response { success = success, detail = rows, notiftext = response };

                    }
                }
                else
                {
                    connection.Close();
                    success = false;
                    response = "Invalid Customer Code!";
                }
            }
            catch (Exception ex)
            {
                connection.Close();
                success = false;
                response = ex.Message;
            }

            return new Response { success = success, detail = response, errortype = error };
        }

        // DELETE api/order/5
        public void Delete(int id)
        {
        }
    }
}
