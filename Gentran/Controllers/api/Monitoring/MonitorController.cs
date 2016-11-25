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
using System.Web;

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
                                left(ULCancelDate,12) as ULCancelDate,
                                left(RFRetrieveDate,12) + '- ' + CONVERT (varchar(15),CAST(RFRetrieveDate as time),100) as RFRetrieveDate,
                                left(ULReadDate,12) + '- ' + CONVERT (varchar(15),CAST(ULReadDate as time),100) as ULReadDate,
                                left(ULSubmitDate,12) + '- ' + CONVERT (varchar(15),CAST(ULSubmitDate as time),100) as ULSubmitDate,
                                ULReadUser,
                                ULSubmitUser,
                                RFAccount,
                                RFStatus
                            from tblrawfile 
                            left join tbluploadlog 
                                on ulfile = rfid
                            left join tblcustomermaster
                                on cmid = ulcustomer
                            order by RFRetrieveDate desc";
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
            String sQuery = "";
            if (values.operation != "transaction")
            {
                sQuery = "select * from tblrawfile left join tbluploadlog on ulfile = rfid where ulponumber = " + values.payload[0].ULPONumber + " ";
            }
            else {
                sQuery = @"select 
                                TLId, 
                                TADescription, 
                                TLValue, 
                                TLRemarks, 
                                left(TLDate,12) +'- ' + CONVERT(varchar(15), CAST(TLDate as time), 100) as TLDate ,
                                TLUser
                            from tblTransactionLog
                            left join tblTransactionActivity
                                on taid = TLActivity
                            where TLUser = '000000'
                            order by TLId desc"; //" + HttpContext.Current.Session["UserId"].ToString() + "
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
