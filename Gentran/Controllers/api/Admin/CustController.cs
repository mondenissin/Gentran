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
        public string Get(int id)
        {
            return "value";
        }

        // POST api/default1
        public object Post([FromBody]Data values)
        {
            string activity = "";

            DateTime now = new DateTime();
            now = DateTime.Now;

            string type = "ADM";
            string changes = "";

            string newpaypload = JsonConvert.SerializeObject(values.payload, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


            string response = "";
            string error = "";

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);

            String sQuery = "";
            SqlCommand cmd;
            SqlDataReader dr;

            try
            {

                for (int i = 0; i < values.payload.Count; i++)
                {
                    var cmCode = values.payload[i].cmCode;
                    var cmName = values.payload[i].cmName;
                    var cmArea = values.payload[i].cmArea;
                    var cmStat = values.payload[i].cmStat;

                    if (values.operation == "add_customer")
                    {
                        activity = "ADD20";

                        sQuery = "select cmcode from tblcustomermaster where cmcode = '" + cmCode + "'";
                        connection.Open();
                        cmd = new SqlCommand(sQuery, connection);
                        dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            connection.Close();
                            response = "Customer: " + cmCode + " already exist!";
                            success = false;
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
                            rows.Add(new Transaction { activity = activity, date = now, remarks = response, user = userID, value = cmCode, type = type, changes = changes, payloadvalue = newpaypload, customernumber = "", ponumber = "" });
                            return new Response { success = success, detail = rows };
                        }

                    }

                    error = success == false ? sQuery : "";
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

        // PUT api/default1/5
        public object Put([FromBody]Data values)
        {
            string activity = "";

            DateTime now = new DateTime();
            now = DateTime.Now;

            string type = "ADM";
            string changes = "";

            string newpaypload = JsonConvert.SerializeObject(values.payload, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


            string response = "";
            string error = "";

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);

            String sQuery = "";
            SqlCommand cmd;//= new SqlCommand(sQuery, connection);
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
                            response = "Successful";

                            changes = "Customer Update,";

                            changes = (changes == "") ? "NC" : changes;
                            rows.Add(new Transaction { activity = activity, date = now, remarks = response, user = userID, value = cmCode, type = type, changes = changes, payloadvalue = newpaypload, customernumber = "", ponumber = "" });
                            return new Response { success = success, detail = rows };
                        }
                        else
                        {
                            response = "Customer: " + cmCode + " not found!";
                            success = false;
                        }

                    }   

                    error = success == false ? sQuery : "";
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

        // DELETE api/default1/5
        public void Delete(int id)
        {
        }
    }
}
