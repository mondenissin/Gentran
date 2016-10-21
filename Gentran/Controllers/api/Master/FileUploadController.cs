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

        public object Get([FromUri] string value)
        {
            Data values = JsonConvert.DeserializeObject<Data>(value);
            string[] text = { };
            bool success = false;
            string response = "";
            try {
                string filepath;
                filepath = values.payload[0].fileName;
                if (values.operation == "read_csv") {
                    text = File.ReadAllLines(filepath);
                    success = true;
                    if (File.Exists(filepath))
                    {
                        File.Delete(filepath);
                    }
                }
                 

            }
            catch (Exception ex) {
                response = ex.Message;
                success = false;
            }
            
             
            return new Response { success = success, filecontent = text, detail = response };
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

                string filePath = path + fileName;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                file.SaveAs(filePath);

                response = filePath;
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
