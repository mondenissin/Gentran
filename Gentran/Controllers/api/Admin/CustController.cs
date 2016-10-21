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
            else if (values.operation == "get_userAssignment") //user - customer mapping
            {
                sQuery = "select UACustomer,UAUser,UAType,UMFirstname,UMMiddlename,UMLastname from tblUserassignment left join tblUserMaster on UAUser = UMId where UACustomer = '" + values.payload[0].cmID + "' order by UMFirstname asc";
            }
            else if (values.operation == "get_users") //user - customer mapping
            {
                sQuery = "select UMId, UMFirstname, UMLastname from tblusermaster order by UMLastname,UMFirstname";
            }
            else if (values.operation == "get_unassigned_users") //user - customer mapping
            {
                sQuery = "select UMId, UMFirstname, UMLastname from tblusermaster where umid not in (" + values.payload[0].keyword + ") order by UMLastname,UMFirstname";
            }
            else if (values.operation == "add_users_to_table") //user - customer mapping
            {
                sQuery = "select UMId, UMFirstname, UMLastname from tblusermaster where umid in (" + values.payload[0].keyword + ") order by UMLastname,UMFirstname";
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
                else if (values.operation == "batch_mapping")
                {
                    success = true;
                    DataTable dt = new DataTable();
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;

                    dt.Columns.Add("MNC", typeof(String));
                    dt.Columns.Add("CODE", typeof(String));
                    dt.Columns.Add("ACCT", typeof(String));
                    dt.Columns.Add("REMARKS", typeof(String));

                    for (int i = 0; i < values.payload.Count; i++)
                    {
                        try
                        {
                            string id;
                            sQuery = "select cmid from tblcustomermaster where cmcode = '" + values.payload[i].cmCode + "'";
                            connection.Open();
                            SqlCommand selectcmd2 = new SqlCommand(sQuery, connection);
                            dr2 = selectcmd2.ExecuteReader();

                            if (dr2.HasRows)
                            {
                                dr2.Read();
                                id = dr2["cmid"].ToString();

                                SqlConnection connection3 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
                                sQuery = "select * from tblcustomerassignment where cacustomer = '" + id + "' and caaccount = '" + values.payload[i].acctype + "'";
                                connection3.Open();
                                SqlCommand selectcmd3 = new SqlCommand(sQuery, connection3);
                                SqlDataReader dr3 = selectcmd3.ExecuteReader();
                                if (dr3.HasRows)
                                {
                                    dt.Rows.Add(new Object[] { values.payload[i].cmCode, values.payload[i].caCode, values.payload[i].acctype, "Duplicated" });
                                    response += values.payload[0].cmCode;
                                    connection3.Close();
                                }
                                else
                                {
                                    connection3.Close();
                                    sQuery = "select * from tblaccounttype where atid='" + values.payload[i].acctype + "'";
                                    SqlConnection connection4 = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
                                    connection4.Open();
                                    SqlCommand selectcmd = new SqlCommand(sQuery, connection4);
                                    SqlDataReader dr4 = selectcmd.ExecuteReader();
                                    if (dr4.HasRows)
                                    {
                                        connection4.Close();
                                        sQuery = "insert into tblcustomerassignment select '" + values.payload[i].caCode + "','" + id + "','" + values.payload[i].acctype + "'";
                                        connection4.Open();
                                        selectcmd = new SqlCommand(sQuery, connection4);
                                        selectcmd.ExecuteNonQuery();
                                        dt.Rows.Add(new Object[] { values.payload[i].cmCode, values.payload[i].caCode, values.payload[i].acctype, "assigned " + id + " a new code : " + values.payload[i].caCode });
                                        connection4.Close();
                                    }
                                    else
                                    {
                                        connection4.Close();
                                        dt.Rows.Add(new Object[] { values.payload[i].cmCode, values.payload[i].caCode, values.payload[i].acctype, "Invalid Account Type" });
                                    }
                                }
                            }
                            else
                            {
                                dt.Rows.Add(new Object[] { values.payload[i].cmCode, values.payload[i].caCode, values.payload[i].acctype, "Invalid Customer Code" });
                            }
                            connection.Close();
                        }
                        catch (Exception ex)
                        {
                            success = false;
                        }
                    }

                    foreach (DataRow sDr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, sDr[col]);
                        }
                        rows.Add(row);
                    }
                    return new Response { success = success, detail = rows };
                }
                else if (values.operation == "update_mapping")
                {

                    activity = "EDI30";

                    string cmID = "";
                    string cmCode = "";
                    string caCode = "";
                    string caaccount = "";
                    string oldcaCode = "";


                    for (int i = 0; i < values.payload.Count; i++)
                    {
                        cmID = values.payload[i].cmID;
                        cmCode = values.payload[i].cmCode;
                        caCode = values.payload[i].assignedcode;
                        caaccount = values.payload[i].acctype;

                        if (values.payload[i].Status == "1")
                        {

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

                    if (Accounts != "")
                    {
                        sQuery = "select distinct CAAccount, ATDescription, CACode, CMCode, CMId from tblCustomerAssignment left join tblCustomerMaster on CMId = CACustomer left join tblAccountType on ATId = CAAccount where CAAccount not in (" + Accounts + ") and CACustomer='" + cmID + "'";
                    }
                    else
                    {
                        sQuery = "select distinct CAAccount, ATDescription, CACode, CMCode, CMId from tblCustomerAssignment left join tblCustomerMaster on CMId = CACustomer left join tblAccountType on ATId = CAAccount where CACustomer='" + cmID + "'";
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
                else if (values.operation == "update_user_assignment")
                {

                    activity = "EDI50";

                    string cmID = "";
                    string umID = "";
                    string cmCode = "";
                    
                    for (int i = 0; i < values.payload.Count; i++)
                    {
                        cmID = values.payload[i].cmID;
                        cmCode = values.payload[i].cmCode;
                        umID = values.payload[i].umid;

                        sQuery = "select * from tblusermaster where umid='" + umID + "'";

                        connection.Close();

                        connection.Open();
                        cmd = new SqlCommand(sQuery, connection);
                        dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            connection.Close();

                            sQuery = "select * from tbluserassignment where uauser='" + umID + "' and uacustomer='" + cmID + "'";
                            connection.Open();
                            cmd = new SqlCommand(sQuery, connection);
                            dr2 = cmd.ExecuteReader();
                            if (!dr2.HasRows)
                            {
                                connection.Close();
                                changes += "Assigned User(" + umID + ") to Customer(" + cmCode + "), ";

                                sQuery = "insert into tbluserassignment select '" + cmID + "','" + umID + "','KAS'";
                                connection.Open();
                                cmd = new SqlCommand(sQuery, connection);
                                cmd.ExecuteNonQuery();
                                connection.Close();
                                success = true;
                            }
                            connection.Close();
                        }
                    }

                    string UMIDs = "";
                    cmID = values.payload[0].cmID;
                    cmCode = values.payload[0].cmCode;


                    for (int ctr = 0; ctr < values.payload.Count; ctr++)
                    {
                        UMIDs += (UMIDs == "") ? values.payload[ctr].umid : "," + values.payload[ctr].umid;
                    }

                    success = true;

                    sQuery = "select distinct uacustomer, uauser from tbluserassignment where uauser not in (" + UMIDs + ") and uacustomer='" + cmID + "'";

                    connection.Open();
                    cmd = new SqlCommand(sQuery, connection);
                    dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        sQuery = "";
                        while (dr.Read())
                        {
                            umID = dr["uauser"].ToString();
                            sQuery += "DELETE FROM tbluserassignment WHERE uacustomer='" + cmID + "' AND uauser='" + umID + "';";
                            changes += "Delete assignment of User(" + umID + ") to Customer(" + cmCode + "), ";
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
