using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Gentran.Controllers.api.Order
{
    public class MonitorErrorsController : ApiController
    {
        // GET api/ordererrors
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/ordererrors/5
        public object Get(string id)
        {
            string[] data = id.Split(',');
            Boolean success = true;
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            try
            {
                DataTable dt = new DataTable();
                SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
                if (Convert.ToInt32(data[1]) < 20)
                {
                    String selectStr = "select ELId,(select ETDescription from tblerrortype where ETId = ELType) as ELType,ELDetail from tblErrorLog where eltype < '200' and  elid = '" + data[0] + "' group by elid,eltype,eldetail";
                    SqlDataAdapter dataadapter = new SqlDataAdapter(selectStr, connection);
                    connection.Open();
                    dataadapter.Fill(dt);
                    connection.Close();

                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                }
                else
                {
                    String selectStr = "select ELId,(select ETDescription from tblerrortype where ETId = ELType) as ELType,ELDetail from tblErrorLog where eltype > '200' and elid = '" + data[0] + "' group by elid,eltype,eldetail";
                    SqlDataAdapter dataadapter = new SqlDataAdapter(selectStr, connection);
                    connection.Open();
                    dataadapter.Fill(dt);
                    connection.Close();

                    foreach (DataRow dr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }
                        rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                return new Response { success = success, detail = ex.Message };
            }
            
            return new Response { success = success, detail = rows };
        }

        // POST api/ordererrors
        public void Post([FromBody]string value)
        {
        }

        // PUT api/ordererrors/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/ordererrors/5
        public void Delete(int id)
        {
        }
    }
}
