using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.SessionState;
using Newtonsoft.Json;


namespace Gentran.Controllers.api
{
    public class ConnController : ApiController
    {
        private AppSettings app = new AppSettings();
        string userID = HttpContext.Current.Session["UserId"].ToString();
        List<Transaction> rows = new List<Transaction>();
        // GET api/default1
        public Object Get()
        {
            bool success = true;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String sQuery = "select * from tblConnectionStatus ";
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
                        row.Add("ftppassword", app.Decrypt(dr[5].ToString()));
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
        public string Get(int id)
        {
            return "value";
        }

        // POST api/default1
        public object Post([FromBody]Data values)
        {
            string response = "";
            bool success = false;
            string newpaypload = JsonConvert.SerializeObject(values.payload, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);

            String sQuery = "";
            String sQueryi = "";
            SqlCommand cmd;
            SqlDataReader dr;
            try
            {
                if (values.operation == "add_connection")
                {
                    string csacct = values.payload[0].CSAccount;

                    sQuery = "SELECT ATId from tblAccountType where ATId IN (select CSAccount from tblConnectionStatus where CSAccount = '" + csacct + "' )";
                    connection.Open();
                    cmd = new SqlCommand(sQuery, connection);
                    dr = cmd.ExecuteReader();
                
                    if (dr.HasRows)
                    {
                        dr.Close();
                        connection.Close();
                        success = false;
                        response = "Duplicate record";
                        
                    }
                    else
                    {
                        connection.Close();
                        string sQuery1 = "SELECT MAX(CSId) as csid from tblConnectionStatus";
                        connection.Open();
                        cmd = new SqlCommand(sQuery1, connection);
                        dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            dr.Read();
                            int csid = Convert.ToInt32(dr["csid"]) + 1;
                            dr.Close();

                            string cshost = values.payload[0].CSHost;
                            string csport = values.payload[0].CSPort;
                            string csusername = values.payload[0].CSUserName;
                            string cspassword = values.payload[0].CSPassword;

                            sQueryi = @"INSERT INTO tblConnectionStatus 
                                   values ('" + csid 
                                        + "', '" + csacct
                                        + "', '" + cshost
                                        + "', '" + csport
                                        + "', '" + csusername
                                        + "', '" + app.Encrypt(cspassword) 
                                        + "', '1')";
                            
                            cmd = new SqlCommand(sQueryi, connection);
                            cmd.ExecuteNonQuery();
                            connection.Close();
                        }
                        connection.Close();
                        success = true;
                        response = "Successful";

                    }
                }
            }
            catch (Exception ex)
            {

                connection.Close();

                success = false;
                response = ex.Message;

            }

            return new Response { success = success, detail = response };
        }

        // PUT api/default1/5
        public object Put([FromBody]Data values)
        {
            string newpayload = JsonConvert.SerializeObject(values.payload, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            bool success = false;
            string response = "";
            string activity = "";

            DateTime now = new DateTime();
            now = DateTime.Now;

            string type = "ADM";
            string changes = "";
            string sValue = "";

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);

            SqlCommand cmd;
            String sQuery = "";

            string csacct = values.payload[0].CSAccount;
            if (values.operation == "edit_connection")
            {
                activity = "EDI60";
                string csid = values.payload[0].CSId;
                string cshost = values.payload[0].CSHost;
                string csport = values.payload[0].CSPort;
                string csusername = values.payload[0].CSUserName;
                string cspassword = values.payload[0].CSPassword;

                sQuery = "Update tblConnectionStatus set CSHost = '" + cshost
                        + "', CSPort = '" + csport
                        + "', CSUserName = '" + csusername
                        + "', CSPassword = '" + app.Encrypt(cspassword)
                        + "' where CSId = '" + csid + "'";
                connection.Open();
                cmd = new SqlCommand(sQuery, connection);
                cmd.ExecuteNonQuery();
                connection.Close();
                success = true;
                sValue = csacct;
                response = csacct + " Connection Edited Successful";

                rows.Add(new Transaction { activity = activity, date = now, remarks = response, user = userID, value = sValue + " Edited Successfully", type = type, changes = changes, payloadvalue = newpayload, customernumber = "", ponumber = "" });
            }
            else if (values.operation == "disable_connection")
            {
                activity = "DIS10";
                string csid = values.payload[0].CSId;
                sQuery = "Update tblConnectionStatus set CSStatus = 0 where CSId = '" + csid + "'";
                connection.Open();
                cmd = new SqlCommand(sQuery, connection);
                cmd.ExecuteNonQuery();
                connection.Close();
                success = true;
                sValue = csacct;
                response = csacct + " Disable Connection Successful";

                rows.Add(new Transaction { activity = activity, date = now, remarks = response, user = userID, value = sValue + " Disabled Successfully", type = type, changes = changes, payloadvalue = newpayload, customernumber = "", ponumber = "" });
            }

            else if (values.operation == "enable_connection")
            {
                activity = "ENB10";
                string csid = values.payload[0].CSId;
                sQuery = "Update tblConnectionStatus set CSStatus = 1 where CSId = '" + csid + "'";
                connection.Open();
                cmd = new SqlCommand(sQuery, connection);
                cmd.ExecuteNonQuery();
                connection.Close();
                success = true;
                sValue = csacct;
                response = csacct + " Enable Connection Successful";

                rows.Add(new Transaction { activity = activity, date = now, remarks = response, user = userID, value = sValue + " Enabled Successfully", type = type, changes = changes, payloadvalue = newpayload, customernumber = "", ponumber = "" });
            }

            return new Response { success = success, detail = response, transactionDetail = rows };
        }

        // DELETE api/default1/5
        public void Delete(int id)
        {
        }
    }
}
