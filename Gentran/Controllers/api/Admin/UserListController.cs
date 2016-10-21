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
    public class UserListController : ApiController
    {
        private AppSettings AppSettings = new AppSettings();
        string userID = HttpContext.Current.Session["UserId"].ToString();
        List<Transaction> rows = new List<Transaction>();
        Boolean success = false;
        // GET api/userlis
        public Object Get()
        {                       
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String sQuery = "SELECT  UMId, UMUsername, UMEmail, UMStatus, (select USDescription from tbluserstatus where USId = UMStatus) as USStatus, UMNickname, UMFirstname, UMMiddlename, UMLastname,UMType,UMImage,UTDescription as UMUType FROM tblusermaster left join tblUserType on UTId = UMType where UMType != 'DEV'";
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

        // GET api/userlis/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/userlis
        public object Post([FromBody]Data values)
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

                    string uid = values.payload[i].umid;
                    string utype = values.payload[i].type;
                    string uname = values.payload[i].username;
                    string nname = values.payload[i].nickname;
                    string fname = values.payload[i].firstname;
                    string mname = values.payload[i].middlename;
                    string lname = values.payload[i].lastname;
                    string email = values.payload[i].email;
                    string image = values.payload[i].image;
                    string opass = values.payload[i].oldpassword;
                    string npass = values.payload[i].newpassword;

                    if (values.operation == "add_user")
                    {
                        activity = "10ADD";

                        int userCount = 0;
                        string prefix = DateTime.Now.ToString("yy");

                        sQuery = "SELECT MAX(UMId) as userCount from tblUserMaster where UMId like '" + prefix + "%'";
                        connection.Open();
                        cmd = new SqlCommand(sQuery, connection);
                        dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            dr.Read();
                            userCount = dr["userCount"] == DBNull.Value ? 0 : Convert.ToInt32(dr["userCount"]);
                            dr.Close();
                        }
                        connection.Close();

                        if (userCount < 9999)
                        {
                            uid = prefix + userCount.ToString("D4");
                        }
                        else
                        {
                            uid = (userCount + 1).ToString();
                        }

                        if (nname == "")
                        {
                            nname = fname;
                        }

                        var encPass = AppSettings.Encrypt("def-password");

                        sQuery = "INSERT INTO tblUserMaster select '" + uid + "','" + fname + " " + lname +
                                    "','" + fname + "','" + nname + "','" + mname +
                                    "','" + lname + "','" + email + "','','" + utype +
                                    "','INA','" + encPass + "','" + utype + "'";

                        connection.Open();
                        cmd = new SqlCommand(sQuery, connection);
                        cmd.ExecuteNonQuery();
                        connection.Close();

                        sQuery = "INSERT INTO tblUserAccess select '" + uid + "','" + utype + "'";

                        connection.Open();
                        cmd = new SqlCommand(sQuery, connection);
                        cmd.ExecuteNonQuery();
                        connection.Close();

                        success = true;
                        sValue = uid;
                        response = "Successful";
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

            rows.Add(new Transaction { activity = activity, date = now, remarks = response, user = userID, value = "User ID:" + sValue, type = type, changes = changes, payloadvalue = newpaypload, customernumber = "", ponumber = "" });
            return new Response { success = success, detail = rows };
        }

        // PUT api/userlis/5
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

                    string uid = values.payload[i].umid;
                    string utype = values.payload[i].type;
                    string uname = values.payload[i].username;
                    string nname = values.payload[i].nickname;
                    string fname = values.payload[i].firstname;
                    string mname = values.payload[i].middlename;
                    string lname = values.payload[i].lastname;
                    string email = values.payload[i].email;
                    string image = values.payload[i].image;
                    string opass = values.payload[i].oldpassword;
                    string npass = values.payload[i].newpassword;

                    if (values.operation == "save_user")
                    {
                        activity = "10UPD";
                        sValue = uid;

                        connection.Open();
                        if (values.payload[i].oldpassword != "unchanged")
                        {
                            sQuery = "SELECT * FROM tblUserMaster where umid = '" + uid + "' and umpassword = @UMPassword";
                            cmd = new SqlCommand(sQuery, connection);
                            cmd.Parameters.Add("@UMPassword", AppSettings.Encrypt(opass));
                            dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                connection.Close();

                                if (image == "unchanged")
                                {
                                    sQuery = "Update tblusermaster set umusername = '" + uname
                                            + "', umnickname = '" + nname
                                            + "', umfirstname = '" + fname
                                            + "', ummiddlename = '" + mname
                                            + "', umlastname = '" + lname
                                            + "', umemail = '" + email
                                            + "', umstatus = 'ACT"
                                            + "', umpassword = @UMPassword where umid = '" + uid + "'";
                                }
                                else
                                {
                                    sQuery = "Update tblusermaster set umusername = '" + uname
                                            + "', umnickname = '" + nname
                                            + "', umfirstname = '" + fname
                                            + "', ummiddlename = '" + mname
                                            + "', umlastname = '" + lname
                                            + "', umimage = '" + image
                                            + "', umemail = '" + email
                                            + "', umstatus = 'ACT"
                                            + "', umpassword = @UMPassword where umid = '" + uid + "'";
                                }
                                connection.Open();
                                cmd = new SqlCommand(sQuery, connection);
                                cmd.Parameters.Add("@UMPassword", AppSettings.Encrypt(npass));
                                cmd.ExecuteNonQuery();
                                connection.Close();

                                success = true;
                                response = "Successful";   
                            }
                            else
                            {
                                success = false;
                                response = "Invalid user credentials!";
                                return new Response { success = success, detail = response };
                            }
                        }
                        else
                        {

                            connection.Close();

                            if (image == "unchanged")
                            {
                                sQuery = "Update tblusermaster set umusername = '" + uname
                                        + "', umnickname = '" + nname
                                        + "', umfirstname = '" + fname
                                        + "', ummiddlename = '" + mname
                                        + "', umlastname = '" + lname
                                        + "', umemail = '" + email
                                        + "', umstatus = 'ACT"
                                        + "' where umid = '" + uid + "'";
                            }
                            else
                            {
                                sQuery = "Update tblusermaster set umusername = '" + uname
                                        + "', umnickname = '" + nname
                                        + "', umfirstname = '" + fname
                                        + "', ummiddlename = '" + mname
                                        + "', umlastname = '" + lname
                                        + "', umimage = '" + image
                                        + "', umemail = '" + email
                                        + "', umstatus = 'ACT"
                                        + "' where umid = '" + uid + "'";
                            }

                            connection.Open();
                            cmd = new SqlCommand(sQuery, connection);
                            cmd.ExecuteNonQuery();
                            connection.Close();

                            success = true;
                            response = "Successful";

                        }

                    }

                    else if (values.operation == "reset_user")
                    {

                        activity = "10RES";
                        sValue = uid;

                        sQuery = "select UMId,UMType,UMFirstName,UMLastName from tblusermaster where UMId = '" + uid + "'";

                        connection.Open();

                        cmd = new SqlCommand(sQuery, connection);

                        dr = cmd.ExecuteReader();

                        if (dr.HasRows)
                        {
                            dr.Read();
                            String userId = dr["UMId"].ToString();
                            String access = dr["UMType"].ToString();
                            String UMFirstName = dr["UMFirstName"].ToString();
                            String UMLastName = dr["UMLastName"].ToString();
                            dr.Close();

                            connection.Close();

                            sQuery = "update tblusermaster set umstatus = 'INA',umpassword = PWDENCRYPT('def-password'),umimage = '',UMUserName = '" + UMFirstName + ' ' + UMLastName + "' where umid = '" + uid + "'";

                            connection.Open();
                            cmd = new SqlCommand(sQuery, connection);
                            cmd.ExecuteNonQuery();
                            connection.Close();

                            String deleteStr = "delete from tblUserAccess where uauser = '" + userId + "' and uaaccess not in ('" + access + "')";
                            connection.Open();
                            SqlCommand deletecmd = new SqlCommand(deleteStr, connection);
                            deletecmd.ExecuteNonQuery();
                            connection.Close();

                            success = true;
                            response = "Successful";                         
                        }
                        else
                        {
                            response = "Cant find ID #: " + uid;
                            success = false;
                            return new Response { success = success, detail = response };
                        }

                    }
                    else if (values.operation == "deactivate_user")
                    {
                        activity = "10DEA";
                        sValue = uid;
                        sQuery = "update tblusermaster set umstatus = 'DEA' where umid = '" + uid + "'";
                        connection.Open();
                        cmd = new SqlCommand(sQuery, connection);
                        cmd.ExecuteNonQuery();
                        connection.Close();

                        success = true;
                        response = "Successful";

                        rows.Add(new Transaction { activity = activity, date = now, remarks = response, user = userID, value = "User ID:" + uid, type = type, changes = changes, payloadvalue = newpaypload, customernumber = "", ponumber = "" });
                        return new Response { success = success, detail = rows };
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

            rows.Add(new Transaction { activity = activity, date = now, remarks = response, user = userID, value = "User ID:" + sValue, type = type, changes = changes, payloadvalue = newpaypload, customernumber = "", ponumber = "" });
            return new Response { success = success, detail = rows };
        }

        // DELETE api/userlis/5
        public void Delete(int id)
        {
        }
    }
}
