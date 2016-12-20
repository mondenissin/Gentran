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
using Newtonsoft.Json;
using System.Data;

namespace Gentran.Controllers.api
{
    public class FTPController : ApiController
    {
        private SqlCommand cmd;
        private SqlCommand cmd1;
        private string sQuery = "";
        private string cQuery = "";
        private SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
        string rawAcct = "";
        private AppSettings app = new AppSettings();
        // GET api/ftp

        public Object Get(string id)
        {
            string acct = id;
            int notifCtrErr = 0,notifCtr = 0;
            string notifConnection = "";
            string connColor = "";
            bool success = true;
            DataTable dt = new DataTable();
            List<string> fileList = new List<string>();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            try
            {
                
                rows.Clear();
                rawAcct = acct == "sm" || acct == "c_sm" ? "SM" : acct == "s8" || acct == "c_s8" ? "S8" : acct == "ncc" || acct == "c_ncc" ? "NCC" : acct == "prg" || acct == "c_prg" ? "PRG" : acct == "wtm" || acct == "c_wtm" ? "WTM" : "UTM";

                try
                    {
                        
                        cQuery = "select * from tblConnectionStatus where csaccount = '" + rawAcct + "' and csstatus = 1 ";
                        cmd = new SqlCommand(cQuery, conn);
                        conn.Open();
                        SqlDataReader rd = cmd.ExecuteReader();

                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(rd[2].ToString());
                                FtpWebResponse res;
                                StreamReader reader;

                                ftp.Credentials = new NetworkCredential(rd[4].ToString().Normalize(), app.Decrypt(rd[5].ToString().Normalize()));
                                ftp.KeepAlive = false;
                                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                                ftp.UsePassive = true;

                                using (res = (FtpWebResponse)ftp.GetResponse())
                                {
                                    reader = new StreamReader(res.GetResponseStream());
                                    notifConnection = "fa-user";
                                    connColor = "green";
                                }
                            }
                        }
                        else {
                            notifConnection = "fa-user-times";
                            connColor = "red";
                        }
                        rd.Close();

                        conn.Close();
                    }
                    catch (Exception)
                    {
                        conn.Close();
                        success = false;
                        notifConnection = "fa-user-times";
                        connColor = "red";
                    }

                success = true;
                sQuery = "select rffilename,left(RFRetrieveDate,12) + '- ' + CONVERT (varchar(15),CAST(RFRetrieveDate as time),100) as RFRetrieveDate,rfid from tblrawfile where RFAccount = '" + rawAcct + "' and RFStatus = '0' order by RFRetrieveDate desc";
                cmd1 = new SqlCommand(sQuery, conn);
                conn.Open();
                SqlConnection.ClearPool(conn);
                SqlDataReader rd1 = cmd1.ExecuteReader();

                if (acct == "c_sm" || acct == "c_s8" || acct == "c_ncc" || acct == "c_prg" || acct == "c_wtm" || acct == "c_utm")
                {
                    if (rd1.HasRows)
                    {
                        dt.Load(rd1);
                        notifCtr = dt.Rows.Count;
                    }
                    rd1.Close();

                    fileList.Clear();
                }
                else
                {
                    if (rd1.HasRows)
                    {
                        while (rd1.Read())
                        {
                            row = new Dictionary<string, object>();
                            row.Add("files", rd1[0].ToString());
                            row.Add("retdate", rd1[1].ToString());
                            row.Add("rawid", rd1[2].ToString());
                            rows.Add(row);
                        }
                    }
                    rd1.Close();
                }

                conn.Close();
                #region For thumbnail

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

                #endregion
            }
            catch (Exception ex){
                success = false;
                row = new Dictionary<string, object>();
                row.Add("error", ex.Message);
                rows.Add(row);
            }

            return new Response { success = success, detail = rows, notiftextErr = notifCtrErr.ToString(), notifConnection = notifConnection, connColor = connColor,notiftext = notifCtr.ToString() };
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
