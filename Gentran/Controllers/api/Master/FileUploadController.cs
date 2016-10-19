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
using System.IO;

namespace Gentran.Controllers.api.Master
{
    public class FileUploadController : ApiController
    {
        public void Get()
        {
        }

        public void Get(string id)
        {
        }
        public object Post()
        {
            string response = "";
            bool success = false;
            string path = @"C:\inetpub\wwwroot\files\Gentran\assignment\";
            try
            {
                var file = HttpContext.Current.Request.Files["file"];

                if (Path.GetExtension(file.FileName).ToLower() != ".csv") {
                    response = "Invalid file type. Must be a CSV file!";
                    success = false;   
                    return new Response { success = success, detail = response };
                }

                string fileName = DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Year.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + Path.GetExtension(file.FileName).ToLower();

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                file.SaveAs(path + fileName);

                response = fileName;
                success = true;
            }
            catch (Exception ex)
            {
                response = ex.Message;
                success = false;
            }

            return new Response { success = success, detail = response };
        }
    }
}
