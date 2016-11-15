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

namespace Gentran.Controllers.api.Master
{
    public class MasterController : ApiController
    {
        public string Get()
        {
            string access = "";
            string sQuery = "";
            SqlCommand cmd;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);

            try {
                sQuery = "select umtype from tblusermaster where umid = '" + HttpContext.Current.Session["UserId"].ToString() + "' ";
                cmd = new SqlCommand(sQuery, connection);
                connection.Open();
                access = cmd.ExecuteScalar().ToString();
                connection.Close();
            } catch (Exception e) {
                access = "ERR";
            }
            return access;
        }

        public object Get([FromUri] string value)
        {
            Data values = JsonConvert.DeserializeObject<Data>(value);

            bool success = true;

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);

            string UMId = HttpContext.Current.Session["UserId"].ToString();

            String sQuery = "";

            if (values.operation == "get_user_profile")
            {
                sQuery = "select UMId, UMFirstname, UMLastname, UMUsername, UMImage, UTDescription from tblusermaster left join tblUserType on UMType = UTId where UMId ='" + UMId + "'";
            }
            else if (values.operation == "view_user_profile") {
                sQuery = "SELECT  UMId, UMUsername, UMEmail, UMStatus, (select USDescription from tbluserstatus where USId = UMStatus) as USStatus, UMNickname, UMFirstname, UMMiddlename, UMLastname,UMType,UMImage,UTDescription as UMUType FROM tblusermaster left join tblUserType on UTId = UMType where UMId ='" + UMId + "'";
            }
            else
            {
                sQuery = "select UMId, UMFirstname, UMLastname, UMUsername, UMImage, UTDescription from tblusermaster left join tblUserType on UMType = UTId where UMId ='" + UMId + "'";
            }

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

    }
}
