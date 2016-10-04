using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Web;

namespace Gentran.Controllers.api
{
    public class LoginController : ApiController
    {
        private AppSettings AppSettings = new AppSettings();
        private string response = "";
        // GET api/login
        public string Get()
        {
            response = "Connected!";

            SqlConnection.ClearAllPools();
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            try
            {
                connection.Open();

                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                connection.Close();
                if (ex.Message.Contains("The server was not found or was not accessible"))
                {
                    response = "Connection failed! Trying to reconnect..";
                }
                else
                {
                    response = "Connection failed! Trying to reconnect..";
                }
            }

            return response;
        }

        // GET api/login/5
        public string Get(string id)
        {
            string[] split = id.Split(',');

            try
            {
                SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
                String selectStr = "select UMId, UMUserName, UMPassword,UMStatus,UMType FROM tblUsermaster WHERE (UMUserName = @UMId OR UMEmail = @UMId OR UMId = @UMId) and UMPassword = @UMPassword";
                connection.Open();
                SqlCommand selectcmd = new SqlCommand(selectStr, connection);
                selectcmd.Parameters.Add("@UMId",split[0]);
                selectcmd.Parameters.Add("@UMPassword", AppSettings.Encrypt(split[1]));

                SqlDataReader dr;

                dr = selectcmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();

                    if (dr["UMStatus"].ToString() == "DEA")
                    {
                        connection.Close();
                        response = "Your account is deactivated!";
                    }
                    else if (dr["UMStatus"].ToString() == "DIS")
                    {
                        connection.Close();
                        response = "You are currently disabled!";
                    }

                    else
                    {
                        response = "Login Success";
                        HttpContext.Current.Session.Add("UserId", dr["UMId"].ToString());
                        HttpContext.Current.Session.Add("UserType", dr["UMType"].ToString());
                        
                        connection.Close();
                    }
                }
                else
                {
                    connection.Close();
                    response = "Invalid Login Credentials!";
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            return response;
        }

        // POST api/login
        public void Post([FromBody]string value)
        {
        }

        // PUT api/login/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/login/5
        public void Delete(int id)
        {
        }
    }
}
