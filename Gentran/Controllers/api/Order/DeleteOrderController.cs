using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Gentran.Controllers.api.Order
{
    public class DeleteOrderController : ApiController
    {
        string userID = HttpContext.Current.Session["UserId"].ToString();
        List<Transaction> rows = new List<Transaction>();
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
            string response = "",responseList = "";
            DateTime now = new DateTime();
            now = DateTime.Now;

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            try
            {
                string newpaypload = JsonConvert.SerializeObject(values.payload, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                for (int x = 0,y = values.payload.Count; x<y; x++) {

                    String sQuery = "update tbluploadlog set ulstatus = '0' where ulid = '" + values.payload[x].ULId + "'";
                    connection.Open();
                    SqlCommand cCmd = new SqlCommand(sQuery, connection);
                    cCmd.ExecuteNonQuery();
                    connection.Close();

                    response = "- " + values.payload[x].ponumber;
                    responseList += "- " + values.payload[x].ponumber;

                    rows.Add(new Transaction { activity = "DEL20", date = now, remarks = "PO # " + response, user = userID, type = "ADM", value = "PO ID:" + values.payload[x].ULId, changes = "", payloadvalue = newpaypload, customernumber = "", ponumber = "" });
                }
                
                return new Response { success = success, detail = rows, notiftext = responseList };
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
            DateTime now = new DateTime();
            now = DateTime.Now;

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

                string newpaypload = JsonConvert.SerializeObject(values.payload, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                rows.Add(new Transaction { activity = "DEL20", date = now, remarks = response, user = userID, type = "ADM", value = "PO ID:" + values.payload[0].ULId, changes = "", payloadvalue = newpaypload, customernumber = "", ponumber = "" });
                return new Response { success = success, detail = rows ,notiftext = response };
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

