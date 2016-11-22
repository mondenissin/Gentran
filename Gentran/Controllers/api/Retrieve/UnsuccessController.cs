using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Gentran.Controllers.api.Retrieve
{
    public class UnsuccessController : ApiController
    {
        string userID = HttpContext.Current.Session["UserId"].ToString();
        private AppSettings app = new AppSettings();
        private List<Transaction> trows = new List<Transaction>();
        private DateTime rDate = DateTime.Now;
        private SqlCommand cmd;
        private string sQuery = "";
        private SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
        string m_ftpSite, m_strUsername, m_strPassword = "", rawAcct = "";

        // GET api/unsuccess/5
        public Object Get(string id)
        {
            string acct = id;
            bool success = true;
            DataTable dt = new DataTable();
            List<string> fileList = new List<string>();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            try
            {
                if (acct == "sm")
                {
                    fileList.AddRange(Directory.GetFiles(@"C:\inetpub\wwwroot\files\ftp\error\sm", "*.csv"));
                }
                else if (acct == "s8")
                {
                    fileList.AddRange(Directory.GetFiles(@"C:\inetpub\wwwroot\files\ftp\error\s8", "*.xml"));
                }
                else if (acct == "ncc")
                {
                    fileList.AddRange(Directory.GetFiles(@"C:\inetpub\wwwroot\files\ftp\error\ncc", "*.xml"));
                }

                int intCount = 0;
                intCount = fileList.Count;

                for (int i = 0; i < intCount; i++)
                {
                    row = new Dictionary<string, object>();
                    row.Add("files", fileList[i]);
                    rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                success = false;
                row = new Dictionary<string, object>();
                row.Add("error", ex.Message);
                rows.Add(row);
            }

            return new Response { success = success, detail = rows };
        }

        // GET api/unsuccess
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/unsuccess
        public object Post([FromBody] Data values)
        {
            bool success = true;
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> retRows = new List<Dictionary<string, object>>();
            Dictionary<string, object> prow;
            Dictionary<string, object> row;
            string sResponse = "", sSucc = "";

            try
            {
                for (int x = 0, y = values.payload.Count; x < y; x++)
                {
                    string errorFileDir = @"C:\inetpub\wwwroot\files\ftp\error\"+values.payload[x].outlet.ToLower()+ @"\" +values.payload[x].fileName;
                    
                    byte[] fileData = File.ReadAllBytes(errorFileDir);

                    if (File.Exists(errorFileDir)){
                        File.Delete(errorFileDir);
                    }

                    sQuery = "SELECT MAX(CONVERT(int,RFId)) AS RFId FROM tblRawFile"; // TBLRAWFILE
                    cmd = new SqlCommand(sQuery, conn);
                    conn.Open();
                    SqlDataReader drRFId = cmd.ExecuteReader();
                    int RFId = 0;

                    if (drRFId.HasRows)
                    {
                        drRFId.Read();
                        if (drRFId["RFId"].ToString() == "null" || drRFId["RFId"].ToString() == "NULL" || drRFId["RFId"].ToString() == "")
                        {
                            RFId = 1;
                        }
                        else
                        {
                            RFId = Convert.ToInt32(drRFId["RFId"]) + 1;
                        }
                        conn.Close();
                        drRFId.Close();
                    }

                    try
                    {
                        // TBLRAWFILE
                        sQuery = "insert into tblRawFile values('" + RFId + "','" + values.payload[x].fileName + "',@fileContent,'" + rDate + "','"+ userID + "','','" + values.payload[x].outlet + "')";
                        cmd = new SqlCommand(sQuery, conn);
                        cmd.Parameters.AddWithValue("fileContent", fileData);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        sResponse = "File Retrieved";
                        sSucc = "Successfully Retrieved";
                    }
                    catch (Exception ex)
                    {
                        conn.Close();
                        string errorfilepath = @"C:\inetpub\wwwroot\files\ftp\error\" + values.payload[x].outlet.ToLower() + @"\" + values.payload[x].fileName;

                        using (FileStream file = File.Create(errorfilepath))
                        {
                            file.Write(fileData, 0, fileData.Length);
                            file.Close();
                        }

                        success = false;
                        sQuery = "insert into tblErrorLog select '" + RFId + "','204','File: " + values.payload[x].fileName + " ( " + ex.Message.ToString() + " )'";
                        conn.Open();
                        cmd = new SqlCommand(sQuery, conn);
                        cmd.ExecuteNonQuery();
                        conn.Close();

                        sResponse = "Error Retrieved";
                        sSucc = "Error while Retrieving";
                    }

                    prow = new Dictionary<string, object>();
                    prow.Add("RFId", RFId.ToString());
                    prow.Add("RFFilename", values.payload[x].fileName);
                    prow.Add("RFRetrieveDate", rDate);
                    prow.Add("Outlet", values.payload[x].outlet);
                    retRows.Add(prow);

                    string newpaypload = JsonConvert.SerializeObject(prow, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    trows.Add(new Transaction { response = sResponse, activity = "RET10", date = rDate, remarks = values.payload[x].fileName, user = userID, type = "ADM", value = "Successfully Retrieved", changes = "", payloadvalue = newpaypload, customernumber = "", ponumber = "" });
                }
            }
            catch (Exception ex)
            {
                success = false;
                sQuery = "insert into tblErrorLog select 'Local Folder Error','204','" + ex.Message.ToString() + "'";
                conn.Open();
                cmd = new SqlCommand(sQuery, conn);
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            return new Response { success = success, detail = trows };
        }

        // PUT api/unsuccess/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/unsuccess/5
        public void Delete(int id)
        {
        }
    }
}
