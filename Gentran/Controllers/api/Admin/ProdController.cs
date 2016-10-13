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
    public class ProdController : ApiController
    {
        string userID = HttpContext.Current.Session["UserId"].ToString();
        List<Transaction> rows = new List<Transaction>();
        Boolean success = false;
        // GET api/default1
        public Object Get()
        {
            bool success = true;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String sQuery = "select * from tblproductmaster left join tblProductStatus on PMStatus = PSId";
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
            string error = ""; ;

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);

            String sQuery = "";
            SqlCommand cmd;//= new SqlCommand(sQuery, connection);
            SqlDataReader dr;

            try
            {

                for (int i = 0; i < values.payload.Count; i++)
                {
                    var pmcode = values.payload[i].pmcode;
                    var pmbcode = values.payload[i].pmbcode;
                    var pmdesc = values.payload[i].pmdesc;
                    var pmcategory = values.payload[i].pmcategory;
                    var pmstatus = values.payload[i].pmstatus;
                    if (values.operation == "add_product")
                    {
                        sQuery = "select top 1 pmcode,pmbarcode from tblproductmaster where pmcode = '"+ pmcode + "' or pmbarcode = '"+ pmbcode +"'";
                        connection.Open();
                        cmd = new SqlCommand(sQuery, connection);
                        dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            dr.Read();
                            var code = dr["pmcode"].ToString();
                            var bcode = dr["pmbarcode"].ToString();
                            dr.Close();
                            connection.Close();

                            if (pmcode == code)
                            {
                                response = "Product: " + code + " already exist!";
                            }
                            else if (pmbcode == bcode) {
                                response = "Barcode: " + pmbcode + " is already assigned to "+ bcode + "!";
                            }

                            success = false;
                        }
                        else
                        {
                            connection.Close();
                            sQuery = "insert into tblproductmaster select (SELECT max(pmid)+1 from tblproductmaster ),'"+ pmcode +"','"+ pmdesc +"','"+ pmbcode + "',0,'"+ pmcategory +"','"+ pmstatus +"'";
                            connection.Open();
                            cmd = new SqlCommand(sQuery,connection);
                            cmd.ExecuteNonQuery();
                            connection.Close();
                            response = "Successful";

                            changes += "Product " + pmcode + " successully added!,";

                            success = true;

                            changes = (changes == "") ? "NC" : changes;

                            rows.Add(new Transaction { activity = "ADD30", date = now, remarks = response, user = userID, value = pmcode, type = type, changes = changes, payloadvalue = newpaypload, customernumber = "", ponumber = "" });
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
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/default1/5
        public void Delete(int id)
        {
        }
    }
}
