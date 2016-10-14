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
    public class CustController : ApiController
    {
        private AppSettings AppSettings = new AppSettings();
        string userID = HttpContext.Current.Session["UserId"].ToString();
        List<Transaction> rows = new List<Transaction>();
        Boolean success = false;
        // GET api/default1
        public Object Get()
        {
            bool success = true;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String sQuery = "select CMId, CMCode, CMDescription, CMStatus, CMArea, COUNT(distinct UAUser) as UCount from tblCustomerMaster left join tblUserAssignment on UACustomer = CMId group by CMId,CMCode, CMDescription, CMStatus,CMArea order by cmid desc";
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

            String sQuery = "";

            if (values.operation == "get_accounts")
            {
                sQuery = "select ATDescription,ATId from tblaccounttype order by ATDescription asc";
            }
            else if (values.operation == "get_mapping")
            {
                sQuery = "select ATDescription,CAAccount,CACode,CACustomer from tblcustomerassignment left join tblaccounttype on CAAccount = ATId where CACustomer = '" + values.payload[0].cmID + "' order by ATDescription asc";
            }
            else
            {
                sQuery = "select CMId, CMCode, CMDescription, CMStatus, CMArea, COUNT(distinct UAUser) as UCount from tblCustomerMaster left join tblUserAssignment on UACustomer = CMId group by CMId,CMCode, CMDescription, CMStatus,CMArea order by cmid desc";
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
        public object Post([FromBody]Data values)
        {
            string activity = "";

            DateTime now = new DateTime();
            now = DateTime.Now;

            string type = "ADM";
            string changes = "";

            string value = "";

            string newpaypload = JsonConvert.SerializeObject(values.payload, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            string response = "";

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);

            String sQuery = "";
            SqlCommand cmd;
            SqlDataReader dr,dr2;

            try
            {
                if (values.operation == "add_customer")
                {
                    for (int i = 0; i < values.payload.Count; i++)
                    {
                        string cmCode = values.payload[i].cmCode;
                        string cmName = values.payload[i].cmName;
                        string cmArea = values.payload[i].cmArea;
                        string cmStat = values.payload[i].cmStat;
                        activity = "ADD20";
                        value = cmCode;

                        sQuery = "select cmcode from tblcustomermaster where cmcode = '" + cmCode + "'";
                        connection.Open();
                        cmd = new SqlCommand(sQuery, connection);
                        dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            connection.Close();
                            response = "Customer: " + cmCode + " already exist!";
                            success = false; 

                            return new Response { success = success, detail = response };
                        }
                        else
                        {
                            connection.Close();
                            sQuery = "insert into tblcustomermaster select (SELECT max(cmid)+1 from tblcustomermaster ),'" + cmCode + "','" + cmName + "','" + cmArea + "','" + cmStat + "'";
                            connection.Open();
                            cmd = new SqlCommand(sQuery, connection);
                            cmd.ExecuteNonQuery();
                            connection.Close();

                            success = true;
                            response = "Successful";

                            changes += "Customer " + cmCode + " added.,";

                            changes = (changes == "") ? "NC" : changes;
                        }
                    }

                }
                else if (values.operation == "update_mapping") {

                    activity = "EDI30";

                    string cmID = "";
                    string cmCode = "";
                    string caCode = "";
                    string caaccount = "";
                    string oldcaCode = "";


                    for (int i = 0; i < values.payload.Count; i++)
                    {
                        if (values.payload[i].Status == "1")
                        {
                            cmID = values.payload[i].cmID;
                            cmCode = values.payload[i].cmCode;
                            caCode = values.payload[i].assignedcode;
                            caaccount = values.payload[i].acctype;

                            sQuery = "select * from tblcustomerassignment where cacustomer='" + cmID + "' AND caaccount='" + caaccount + "'";

                            connection.Open();
                            cmd = new SqlCommand(sQuery, connection);
                            dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                dr.Read();
                                oldcaCode = dr["cacode"].ToString();
                                dr.Close();
                                connection.Close();

                                if (caCode != oldcaCode)
                                {
                                    changes += "Updated Data Mapping Code for Customer " + cmCode + " [" + caaccount + "] from " + oldcaCode + " to " + caCode + ", ";
                                    sQuery = "update tblcustomerassignment set cacode='" + caCode + "' where cacustomer='" + cmID + "' AND caaccount='" + caaccount + "'";
                                    connection.Open();
                                    cmd = new SqlCommand(sQuery, connection);
                                    cmd.ExecuteNonQuery();
                                    connection.Close();
                                }
                                success = true;
                            }
                            else
                            {
                                connection.Close();
                                changes += "Added Data Mapping for Customer " + cmCode + " [" + caaccount + "] with Code " + caCode + ", ";
                                sQuery = "insert into tblcustomerassignment select '" + caCode + "','" + cmID + "','" + caaccount + "'";
                                connection.Open();
                                cmd = new SqlCommand(sQuery, connection);
                                cmd.ExecuteNonQuery();
                                connection.Close();
                                success = true;
                            }
                        }

                    }
                    string Accounts = "";
                    for (int ctr = 0; ctr < values.payload.Count; ctr++)
                    {
                        if (values.payload[ctr].Status == "1")
                        {
                            if (Accounts == "")
                            {
                                Accounts = "'" + values.payload[ctr].acctype + "'";
                            }
                            else
                            {
                                Accounts += ",'" + values.payload[ctr].acctype + "'";
                            }
                        }
                    }
                    success = true;
                    string ATDescription = "";

                    if (Accounts.Length > 0)
                    {
                        sQuery = "select distinct CAAccount, ATDescription, CACode, CMCode from tblCustomerAssignment left join tblCustomerMaster on CMId = CACustomer left join tblAccountType on ATId = CAAccount where CAAccount not in (" + Accounts + ") and CACustomer='" + values.payload[0].cmID + "'";
                    }
                    else
                    {
                        sQuery = "select distinct CAAccount, ATDescription, CACode, CMCode from tblCustomerAssignment left join tblCustomerMaster on CMId = CACustomer left join tblAccountType on ATId = CAAccount where CACustomer='" + values.payload[0].cmID + "'";
                    }

                    connection.Open();
                    cmd = new SqlCommand(sQuery, connection);
                    dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        sQuery = "";
                        while (dr.Read())
                        {
                            caCode = dr["CACode"].ToString();
                            cmCode = dr["CMCode"].ToString();
                            caaccount = dr["CAAccount"].ToString();
                            ATDescription = dr["ATDescription"].ToString();
                            sQuery += "DELETE FROM tblCustomerAssignment WHERE cacustomer='" + cmID + "' AND CACode='" + caCode + "' AND CAAccount='" + caaccount + "';";
                            changes += "Deleted data mapping for Customer [" + cmCode + " ] " + ATDescription + " with code " + caCode + ",";
                        }
                        dr.Close();
                        connection.Close();

                        connection.Open();
                        cmd = new SqlCommand(sQuery, connection);
                        cmd.ExecuteNonQuery();
                        connection.Close();

                        success = true;
                    }
                    value = cmCode;
                }
            }
            catch (Exception ex)
            {

                connection.Close();

                success = false;
                response = ex.Message;

                return new Response { success = success, detail = response };
            }

            changes = (changes == "") ? "NC" : changes;
            rows.Add(new Transaction { activity = activity, date = now, remarks = "Successful", user = userID, value = "Customer Code: " + value, type = "ADM", changes = changes, payloadvalue = newpaypload, customernumber = "", ponumber = "" });
            return new Response { success = success, detail = rows };
        }

        // PUT api/default1/5
        public object Put([FromBody]Data values)
        {
            string activity = "";

            DateTime now = new DateTime();
            now = DateTime.Now;

            string type = "ADM";
            string changes = "";
            string response = "";
            string sValue = "";

            string newpaypload = JsonConvert.SerializeObject(values.payload, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);

            String sQuery = "";
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {

                for (int i = 0; i < values.payload.Count; i++)
                {

                    string cmID = values.payload[i].cmID;
                    string cmCode = values.payload[i].cmCode;
                    string cmName = values.payload[i].cmName;
                    string cmArea = values.payload[i].cmArea;
                    string cmStat = values.payload[i].cmStat; 
                 
                    if (values.operation == "save_edit_customer")
                    {
                        activity = "EDI30";

                        sQuery = "select * from tblcustomermaster where CMId = '" + cmID + "'";

                        connection.Open();

                        cmd = new SqlCommand(sQuery, connection);

                        dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        { 
                            connection.Close();

                            sQuery = "update tblcustomermaster set CMCode='"+ cmCode +"',CMDescription='"+ cmName +"',CMArea = '"+ cmArea +"',CMStatus='"+ cmStat +"' where CMId = '"+ cmID +"'";

                            connection.Open();
                            cmd = new SqlCommand(sQuery, connection);
                            cmd.ExecuteNonQuery();
                            connection.Close();

                            success = true;
                            sValue = cmCode;
                            response = "Successful";  
                            changes = "Customer Update,";

                        }
                        else
                        {
                            response = "Customer: " + cmCode + " not found!";
                            success = false;
                            return new Response { success = success, detail = response };
                        }

                    }                                      
                }
            }
            catch (Exception ex)
            {

                connection.Close();

                success = false;
                response = ex.Message;
                return new Response { success = success, detail = response };
            }

            changes = (changes == "") ? "NC" : changes;
            rows.Add(new Transaction { activity = activity, date = now, remarks = response, user = userID, value = sValue, type = type, changes = changes, payloadvalue = newpaypload, customernumber = "", ponumber = "" });
            return new Response { success = success, detail = rows };
        }

        // DELETE api/default1/5
        public void Delete(int id)
        {
        }
    }
}
