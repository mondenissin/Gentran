using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Gentran.Controllers.api
{
    public class MonitorController : ApiController
    {
        // GET api/default1
        public Object Get()
        {
            bool success = true;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String sQuery = @"select 
                                RFId,
                                RFFilename,
                                ULPONumber,
                                ULCustomer,
                                CMDescription,
                                left(ULOrderDate,12) as ULOrderDate,
                                left(ULDeliveryDate,12) as ULDeliveryDate,
                                left(RFRetrieveDate,12) + '- ' + CONVERT (varchar(15),CAST(RFRetrieveDate as time),100) as RFRetrieveDate,
                                left(RFReadDate,12) + '- ' + CONVERT (varchar(15),CAST(RFReadDate as time),100) as RFReadDate,
                                left(RFSubmitDate,12) + '- ' + CONVERT (varchar(15),CAST(RFSubmitDate as time),100) as RFSubmitDate,
                                RFReadUser,
                                RFSubmitUser,
                                RFAccount,
                                RFStatus
                            from tblrawfile 
                            left join tbluploadlog 
                                on ulfile = rfid
                            left join tblcustomermaster
                                on cmid = ulcustomer";
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
        
        public Object Get([FromUri]string value)
        {
            
            Data values = JsonConvert.DeserializeObject<Data>(value);

            bool success = true;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String sQuery = "select * from tblrawfile left join tbluploadlog on ulfile = rfid where ulponumber = " + values.payload[0].ULPONumber + " ";
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
