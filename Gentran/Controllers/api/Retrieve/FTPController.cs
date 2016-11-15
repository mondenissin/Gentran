using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;
using System.IO;
using System.Text;
using Aspose.Cells;
using Aspose.Cells.Rendering;
using System.Drawing;
using System.Web;
using System.Data.SqlClient;

namespace Gentran.Controllers.api
{
    public class FTPController : ApiController
    {
        private AppSettings app = new AppSettings();
        private List<Transaction> trows = new List<Transaction>();
        private DateTime rDate = DateTime.Now;
        private SqlCommand cmd;
        private string sQuery = "";
        private SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
        private string userID = HttpContext.Current.Session["UserId"].ToString();
        string m_ftpSite, m_strUsername, m_strPassword = "",rawAcct = "";
        // GET api/ftp
        public Object Get(string id)
        {
            string acct = id;
            int notifCtr = 0;
            bool success = true;
            
            List<string> fileList = new List<string>();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            try
            {
                if (acct == "sm" || acct == "c_sm") {
                    m_ftpSite = System.Configuration.ConfigurationManager.AppSettings["smftpsite"];
                    m_strUsername = System.Configuration.ConfigurationManager.AppSettings["smusername"];
                    m_strPassword = System.Configuration.ConfigurationManager.AppSettings["smpassword"];
                } else if (acct == "s8" || acct == "c_s8") {
                    m_ftpSite = System.Configuration.ConfigurationManager.AppSettings["s8ftpsite"];
                    m_strUsername = System.Configuration.ConfigurationManager.AppSettings["s8username"];
                    m_strPassword = System.Configuration.ConfigurationManager.AppSettings["s8password"];
                } else if (acct == "ncc" || acct == "c_ncc") {
                    m_ftpSite = System.Configuration.ConfigurationManager.AppSettings["nccftpsite"];
                    m_strUsername = System.Configuration.ConfigurationManager.AppSettings["nccusername"];
                    m_strPassword = System.Configuration.ConfigurationManager.AppSettings["nccpassword"];
                }

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(m_ftpSite);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                request.Credentials = new NetworkCredential(m_strUsername, app.Decrypt(m_strPassword));

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);

                StringBuilder strInfo = new StringBuilder();
                //string line = reader.ReadLine().ToString();
                string line = "";
                
                while (!string.IsNullOrEmpty((line = reader.ReadLine()))) {
                    if (acct == "c_sm" || acct == "c_s8" || acct == "c_ncc")
                    {
                        notifCtr++;
                    }
                    else{

                        sQuery = "SELECT MAX(RFId) AS RFId FROM tblRawFile"; // TBLRAWFILE
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
                        //string inputfilepath = @"C:\inetpub\wwwroot\files\ftp\" + acct + @"\" + RFId+"-"+line;
                        string ftpfullpath = m_ftpSite + "/" + line;

                        using (WebClient webreq = new WebClient())
                        {
                            webreq.Credentials = new NetworkCredential(m_strUsername, app.Decrypt(m_strPassword));
                            byte[] fileData = webreq.DownloadData(ftpfullpath);

                            // TBLRAWFILE
                            sQuery = "insert into tblRawFile values('" + RFId + "','" + RFId + "-" + line + "',@fileContent,'" + rDate + "','','','','','0','" + acct.ToUpper() + "')";
                            cmd = new SqlCommand(sQuery, conn);
                            cmd.Parameters.AddWithValue("fileContent",fileData);
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                            /*
                            using (FileStream file = File.Create(inputfilepath))
                            {
                                file.Write(fileData, 0, fileData.Length);
                                file.Close();
                            }*/
                                
                            if (acct != "ncc")
                            {
                                FtpWebRequest request2 = (FtpWebRequest)WebRequest.Create(ftpfullpath);
                                request2.Method = WebRequestMethods.Ftp.DeleteFile;
                                FtpWebResponse response2 = (FtpWebResponse)request2.GetResponse();
                                string asd = response2.StatusDescription;
                                response2.Close();
                            }
                        }
                        trows.Add(new Transaction { response = "File Retrieved", activity = "RET10", date = rDate, remarks = line, user = userID, type = "ADM", value = "Successfully Retrieved", changes = "", payloadvalue = "", customernumber = "", ponumber = "" });
                    }
                }
                response.Close();

                rows.Clear();
                if (acct == "c_sm" || acct == "c_s8" || acct == "c_ncc")
                {
                }
                else {

                    rawAcct = acct == "sm" ? "SM" : acct == "s8" ? "S8" : "NCC";

                    sQuery = "select rffilename from tblrawfile where RFAccount = '" + rawAcct + "' and RFStatus = '0' ";
                    cmd = new SqlCommand(sQuery, conn);
                    conn.Open();
                    SqlDataReader rd = cmd.ExecuteReader();

                    if (rd.HasRows)
                    {
                        while (rd.Read())
                        {
                            row = new Dictionary<string, object>();
                            row.Add("files", rd[0].ToString());
                            rows.Add(row);
                        }
                    }
                    rd.Close();
                    conn.Close();
                }
                /*
                fileList.Clear();
                if (acct == "c_sm" || acct == "c_s8" || acct == "c_ncc")
                {
                }
                else {
                    if (acct == "sm")
                    {
                        //fileList.AddRange(Directory.GetFiles(@"C:\inetpub\wwwroot\files\ftp\sm", "*.pdf"));
                        //fileList.AddRange(Directory.GetFiles(@"C:\inetpub\wwwroot\files\ftp\sm", "*.txt"));
                        fileList.AddRange(Directory.GetFiles(@"C:\inetpub\wwwroot\files\ftp\sm", "*.csv"));
                    }
                    else if (acct == "s8")
                    {
                        fileList.AddRange(Directory.GetFiles(@"C:\inetpub\wwwroot\files\ftp\s8", "*.xml"));
                    }
                    else if (acct == "ncc")
                    {
                        fileList.AddRange(Directory.GetFiles(@"C:\inetpub\wwwroot\files\ftp\ncc", "*.xml"));
                    }

                    int intCount = 0;
                    intCount = fileList.Count;
                    string[] array = new string[intCount];

                    for (int i = 0; i < intCount; i++)
                    {
                        row = new Dictionary<string, object>();
                        row.Add("files", fileList[i]);
                        rows.Add(row);

                        /*if (acct == "sm")
                        {
                            //FOR THUMBNAILS
                            String imageDir = "";
                            String[] aName = fileList[i].Split('\\');
                            String sName = aName[aName.Length - 1].Replace(" ", "");
                            sName = sName.Substring(0, sName.IndexOf('.'));
                            String asd = Directory.GetCurrentDirectory();

                            imageDir = @"C:\inetpub\wwwroot\Gentran\Gentran\Images\thumbnails\" + sName + ".jpg";

                            Workbook book = new Workbook(fileList[i]);
                            Worksheet sheet = book.Worksheets[0];
                            sheet.PageSetup.PrintArea = "A1:J10";

                            ImageOrPrintOptions imgOptions = new ImageOrPrintOptions();
                            imgOptions.ImageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                            imgOptions.OnePagePerSheet = true;

                            SheetRender sr = new SheetRender(sheet, imgOptions);
                            Bitmap bitmap = sr.ToImage(0);
                            bitmap.Save(imageDir);
                        }
                    }
                }*/
            }
            catch (Exception ex){
                success = false;
                row = new Dictionary<string, object>();
                row.Add("error", ex.Message);
                rows.Add(row);
            }

            return new Response { success = success,detail = rows, transactionDetail = trows, notiftext = notifCtr.ToString() };
        }

        // GET api/ftp/5
        public string Get()
        {
            return "value";
        }

        // POST api/ftp
        public void Post([FromBody]string value)
        {
        }

        // PUT api/ftp/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/ftp/5
        public void Delete(int id)
        {
        }
    }
}
