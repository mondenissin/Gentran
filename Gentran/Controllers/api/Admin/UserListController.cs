using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;

namespace Gentran.Controllers.api
{
    public class UserListController : ApiController
    {
        // GET api/userlis
        public Object Get()
        {
            bool success = true;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String sQuery = "select * from tblusermaster";
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

        // GET api/userlis/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/userlis
        public void Post([FromBody]string value)
        {
        }

        // PUT api/userlis/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/userlis/5
        public void Delete(int id)
        {
        }
    }
}
