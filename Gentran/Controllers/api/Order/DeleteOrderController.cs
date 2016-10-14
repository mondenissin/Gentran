using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Gentran.Controllers.api.Order
{
    public class DeleteOrderController : ApiController
    {
        // GET api/deleteorder
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/deleteorder/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/deleteorder
        public object Post([FromBody]Data values)
        {
            bool success = true;
            string response = "";
            string POid = "";

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            try
            {
                for (int x = 0,y = values.payload.Count; x<y; x++) {

                    String sQuery = "select CMId from tblcustomermaster where cmcode = '"+values.payload[x].customernumber+"'";
                    connection.Open();
                    SqlCommand cCmd = new SqlCommand(sQuery, connection);
                    SqlDataReader dr = cCmd.ExecuteReader();

                    if (dr.HasRows) {
                        dr.Read();
                        POid = dr[0].ToString() + values.payload[x].ponumber;
                        connection.Close();

                        sQuery = "update tbluploadlog set ulstatus = '0' where ulid = '" + POid + "'";
                        connection.Open();
                        cCmd = new SqlCommand(sQuery, connection);
                        cCmd.ExecuteNonQuery();
                        connection.Close();

                        sQuery = "update tbluploaditems set uistatus = '0' where uiid = '" + POid + "'";
                        connection.Open();
                        cCmd = new SqlCommand(sQuery, connection);
                        cCmd.ExecuteNonQuery();
                        connection.Close();

                        response += "- " + values.payload[x].ponumber;
                    }
                    else {
                        success = false;
                        response = "Customer Not Found!";
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                response = ex.Message;
            }

            return new Response { success = success, detail = response };
        }

        // PUT api/deleteorder/5
        //public object Put(string id, [FromBody]Data values)
        public object Put([FromBody]Data values)
        {
            bool success = true;
            string response = "";
            String sQuery = "";
            SqlCommand cCmd;

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            try
            {
                sQuery = "update tbluploadlog set ulstatus = '0' where ulid = '" + values.payload[0].ULId + "'";
                connection.Open();
                cCmd = new SqlCommand(sQuery, connection);
                cCmd.ExecuteNonQuery();
                connection.Close();

                sQuery = "update tbluploaditems set uistatus = '0' where uiid = '" + values.payload[0].ULId + "'";
                connection.Open();
                cCmd = new SqlCommand(sQuery, connection);
                cCmd.ExecuteNonQuery();
                connection.Close();

                response = "PO ID : " + values.payload[0].ULId;
            }
            catch (Exception ex)
            {
                success = false;
                response = ex.Message;
            }

            return new Response { success = success, detail = response };
        }

        // DELETE api/deleteorder/5
        public void Delete(int id)
        {
        }
    }
}

