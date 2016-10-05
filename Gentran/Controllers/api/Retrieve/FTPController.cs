using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Gentran.Controllers.api
{
    public class FTPController : ApiController
    {
        string m_ftpSite, m_strUsername, m_strPassword = "";
        // GET api/ftp
        public Object Get(string id)
        {
            string acct = id;
            bool success = true;
            List<string> fileList = new List<string>();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            try
            {
                if (acct == "sm") {
                    m_ftpSite = System.Configuration.ConfigurationManager.AppSettings["smftpsite"];
                    m_strUsername = System.Configuration.ConfigurationManager.AppSettings["smusername"];
                    m_strPassword = System.Configuration.ConfigurationManager.AppSettings["smpassword"];
                } else if (acct == "s8") {
                    m_ftpSite = System.Configuration.ConfigurationManager.AppSettings["s8ftpsite"];
                    m_strUsername = System.Configuration.ConfigurationManager.AppSettings["s8username"];
                    m_strPassword = System.Configuration.ConfigurationManager.AppSettings["s8password"];

                }

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(m_ftpSite);
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                request.Credentials = new NetworkCredential(m_strUsername, m_strPassword);

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                StringBuilder strInfo = new StringBuilder();
                string line = reader.ReadLine().ToString();

                while (line != null)
                {
                    if (!(File.Exists(@"C:\inetpub\wwwroot\files\ftp\" + line)))
                    {
                        string inputfilepath = @"C:\inetpub\wwwroot\files\ftp\" + line;
                        string ftpfullpath = m_ftpSite +"/"+ line;
                        
                        using (WebClient webreq = new WebClient())
                        {
                            webreq.Credentials = new NetworkCredential(m_strUsername, m_strPassword);
                            byte[] fileData = webreq.DownloadData(ftpfullpath);

                            using (FileStream file = File.Create(inputfilepath))
                            {
                                file.Write(fileData, 0, fileData.Length);
                                file.Close();
                            }

                            FtpWebRequest request2 = (FtpWebRequest)WebRequest.Create(ftpfullpath);
                            request2.Method = WebRequestMethods.Ftp.DeleteFile;
                            FtpWebResponse response2 = (FtpWebResponse)request2.GetResponse();
                            string asd = response2.StatusDescription;
                            response2.Close();
                        }
                    }
                    line = reader.ReadLine().ToString();
                }
                response.Close();
            }
            catch(Exception ex){
                string asd = ex.Message;
                if (ex.Message != "Object reference not set to an instance of an object.")
                {
                    success = false;
                    row = new Dictionary<string, object>();
                    row.Add("error", ex.Message);
                    rows.Add(row);
                }
                else {

                    fileList.Clear();
                    fileList.AddRange(Directory.GetFiles(@"C:\inetpub\wwwroot\files\ftp", "*.pdf"));
                    fileList.AddRange(Directory.GetFiles(@"C:\inetpub\wwwroot\files\ftp", "*.txt"));

                    int intCount = 0;
                    intCount = fileList.Count;
                    string[] array = new string[intCount];

                    for (int i = 0; i < intCount; i++)
                    {
                        row = new Dictionary<string, object>();
                        row.Add("files", fileList[i]);
                        rows.Add(row);
                    }
                }
            }

            return new Response { success = success,detail = rows };
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
