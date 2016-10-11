using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;

namespace Gentran.Controllers.api
{
    public class UserListController : ApiController
    {
        private AppSettings AppSettings = new AppSettings();
        // GET api/userlis
        public Object Get()
        {
            bool success = true;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String sQuery = "SELECT  UMId, UMUsername, UMEmail, UMStatus, (select USDescription from tbluserstatus where USId = UMStatus) as USStatus, UMNickname, UMFirstname, UMMiddlename, UMLastname,UMType,UMImage FROM tblusermaster where UMType != 'DEV'";
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
            Boolean success = false;
            string response = "";
            string error = "";

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);

            String sQuery;
            SqlCommand cmd;//= new SqlCommand(sQuery, connection);
            SqlDataReader dr;


            try {

                for (int i = 0; i < values.payload.Count; i++) {
                    connection.Open();

                    string uid = values.payload[i].umid;
                    string uname = values.payload[i].username;
                    string nname = values.payload[i].nickname;
                    string fname = values.payload[i].firstname;
                    string mname = values.payload[i].middlename;
                    string lname = values.payload[i].lastname;
                    string email = values.payload[i].email;
                    string image = values.payload[i].image;
                    string opass = values.payload[i].oldpassword;
                    string npass = values.payload[i].newpassword;

                    if (values.payload[i].oldpassword != "unchanged")
                    {
                        sQuery = "SELECT * FROM tblUserMaster where umid = '" + uid + "' and umpassword = '" + AppSettings.Encrypt(opass) + "'";
                        cmd = new SqlCommand(sQuery, connection);
                       // cmd.Parameters.Add("@UMPassword", AppSettings.Encrypt(opass));
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
                            else {  
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
                            response = "USER UPDATED!";
                        }
                        else
                        {
                            success = false;
                            response = "Invalid user credentials!";
                        }
                    }
                    else {

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
                        response = "USER UPDATED!";

                    }
                    error = sQuery;
                } 
            }
            catch (Exception ex) {

                connection.Close();

                success = false;
                response = ex.Message;
            } 
            return new Response { success = success, detail = response , errortype = error };
        }

        // PUT api/userlis/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/userlis/5
        public void Delete(int id)
        {
        }
    }
}
