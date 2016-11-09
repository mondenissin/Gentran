using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;

namespace Gentran.Controllers.api
{
    public class TransactionsController : ApiController
    {
        // GET api/default1
        public void Get()
        {  
        }

        // GET api/default1/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/default1
        public object Post([FromBody]List<Transaction> values)
        {
            Boolean success = true;
            string detail = "Successful";
            DateTime now = new DateTime();
            now = DateTime.Now;
            List<Transaction> rows = new List<Transaction>();

            //string folderPath = System.Web.Hosting.HostingEnvironment.MapPath("~/transactions/");
            string folderPath = @"C:\inetpub\wwwroot\files\Gentran\transactions\";
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String selectTLID = "SELECT MAX(TLId) AS TLId FROM tblTransactionLog";
            SqlCommand selectCmdTLId = new SqlCommand(selectTLID, connection);
            connection.Open();
            SqlDataReader drTLId = selectCmdTLId.ExecuteReader();
            int TLId = 0;

            if (drTLId.HasRows)
            {
                drTLId.Read();
                if (drTLId["TLId"].ToString() == "null" || drTLId["TLId"].ToString() == "NULL" || drTLId["TLId"].ToString() == "")
                {
                    TLId = 1;
                }
                else
                {
                    TLId = Convert.ToInt32(drTLId["TLId"]) + 1;
                }

                connection.Close();
            }

            for (int i = 0; i < values.Count; i++)
            {
                try
                {
                    if (values[i].value == "NA")
                    {
                        String strTransaction = "select cmid from tblcustomermaster where cmcode ='" + values[i].customernumber + "'";
                        SqlCommand cmdTransaction = new SqlCommand(strTransaction, connection);
                        connection.Open();
                        SqlDataReader sDr = cmdTransaction.ExecuteReader();
                        if (sDr.HasRows)
                        {
                            sDr.Read();
                            values[i].value = sDr["cmid"].ToString() + values[i].ponumber;
                        }
                        connection.Close();
                    }


                    string changes = "";

                    values[i].changes = (values[i].changes == null) ? "" : values[i].changes;
                    values[i].payloadvalue = (values[i].payloadvalue == null) ? "" : values[i].payloadvalue;

                    if (values[i].changes.Contains(",") == true)
                    {
                        changes = values[i].changes.Replace("'", "''").Remove(values[i].changes.LastIndexOf(","));

                        string[] changesArray = changes.Split(new string[] { "," }, StringSplitOptions.None);

                        foreach (string s in changesArray)
                        {
                            String strTransaction = "insert into tblTransactionLog select '" + (TLId + i) + "','" + values[i].type + "','" + values[i].activity + "','" + values[i].value + "','" + values[i].remarks.Replace("'", "''") + " [" + s + "]','" + values[i].date + "','" + values[i].user + "'";
                            SqlCommand cmdTransaction = new SqlCommand(strTransaction, connection); 
                            connection.Open();
                            cmdTransaction.ExecuteNonQuery();
                            connection.Close();
                        }
                    }
                    else
                    {
                        if (values[i].changes != "NC") //MNS 06252016 Save if has changes
                        {
                            String strTransaction = "insert into tblTransactionLog select '" + (TLId + i) + "','" + values[i].type + "','" + values[i].activity + "','" + values[i].value + "','" + values[i].remarks.Replace("'", "''") + "','" + values[i].date + "','" + values[i].user + "'";
                            SqlCommand cmdTransaction = new SqlCommand(strTransaction, connection);
                            connection.Open();
                            cmdTransaction.ExecuteNonQuery();
                            connection.Close();
                        }
                    }

                    detail = "Successful";

                    //================================================================
                    string jsonpath = Path.Combine(folderPath, TLId + ".json");
                    StreamWriter testData = new StreamWriter(jsonpath, true);
                    testData.WriteLine(values[i].payloadvalue);
                    testData.Close();
                    testData.Dispose();
                    //================================================================

                }
                catch (Exception ex)
                {
                    detail = "ID " + values[i].value + ": " + ex.Message;
                    success = false;
                }

                rows.Add(new Transaction { type = values[i].type, activity = values[i].activity, value = values[i].value, remarks = detail, date = values[i].date, user = values[i].user, payloadvalue = values[i].payloadvalue });
            }

            return new Response { success = success, detail = rows };
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
