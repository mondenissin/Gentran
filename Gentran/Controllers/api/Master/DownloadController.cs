using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Gentran.Controllers.api.Master
{
    public class DownloadController : ApiController
    {
        string sQuery = "";
        SqlCommand cmd;
        SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
        
        // GET api/download
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/download/5
        public string Get(string id)
        {
            string response = "Sucess";
            string fileExt = "";

            sQuery = "select * from tblrawfile where rfid='" + id + "'";
            cmd = new SqlCommand(sQuery, connection);
            connection.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                dr.Read();
                string tempContentType = "";
                string tempF = dr["rffilename"].ToString();
                string[] temp = tempF.ToString().Split('.');
                fileExt = temp[temp.Length - 1].ToString();
                byte[] fileByte = dr["rfcontent"] as byte[];

                tempContentType = fileExt == "xml" ? "application/xml" : "application/vnd.ms-excel";

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.BufferOutput = true;
                HttpContext.Current.Response.Charset = "";
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                HttpContext.Current.Response.ContentType = tempContentType;
                HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + tempF);
                HttpContext.Current.Response.BinaryWrite(fileByte);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.End();

            }
            else {
                response = "Unsuccessful";
            }

            connection.Close();

            return response;
        }

        // POST api/download
        public void Post([FromBody]string value)
        {
        }

        // PUT api/download/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/download/5
        public void Delete(int id)
        {
        }
    }
}
