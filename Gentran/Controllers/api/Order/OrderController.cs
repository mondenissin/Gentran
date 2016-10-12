using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Gentran.Controllers.api.Order
{
    public class OrderController : ApiController
    {
        // GET api/order
        public object Get()
        {
            Boolean success = false;

            try
            {
                DataTable dt = new DataTable();
                SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);

                String selectStr = "";
               
                selectStr = @"SELECT DISTINCT
                            ul.ulid,
                            ul.ulponumber,
                            cm.cmcode as ulcustomer,
                            cm.cmdescription, 
                            LEFT(ul.ulorderdate,12) AS ulorderdate,
                            LEFT(ul.uldeliverydate,12) AS uldeliverydate,
                            ul.uluploaddate as sortupload,
                            LEFT(ul.uluploaddate,12) AS uluploaddate,
                            ui.sumulquantity,
                            ui.countulquantity,
                            ul.ulstatus,
                            ul.ulfilename,
                            ui.uiprice 
                            FROM tblUploadLog ul
                            LEFT JOIN (SELECT * from tblCustomerMaster ) cm
                            ON ul.ulcustomer = cm.cmid 
                            LEFT JOIN tblUserAssignment ua 
                            ON ul.ulcustomer = ua.uacustomer 
                            LEFT JOIN
                            (SELECT
                            uiprice = SUM(uiquantity * ppprice),
                            sumulquantity = SUM(uiquantity),
                            countulquantity = COUNT(uiquantity),
                            uiid
                            FROM tblUploadItems
                            left join tbluploadlog
                            on ulid = uiid
                            left join tblCustomerMaster
                            on cmid = ulcustomer
                            LEFT JOIN tblProductPricing
                            on ppproduct = uiproduct and pparea = cmarea
                            WHERE uistatus NOT IN ('3','0')
                            group by uiid ) ui 
                            ON ul.ulid = ui.uiid 
                            WHERE ua.uauser = '" + HttpContext.Current.Session["UserId"].ToString() + @"'
                            AND uatype = 'KAS' 
                            AND ulstatus !=0  
                            AND ulstatus !=10 
                            AND ulstatus <=20 
                            AND ULUploadDate > DATEADD(day, -15, GETDATE())
                            ORDER BY sortupload desc"; //MNS20160820
                

                SqlDataAdapter dataadapter = new SqlDataAdapter(selectStr, connection);
                connection.Open();
                dataadapter.Fill(dt);

                List<Dictionary<object, object>> rows = new List<Dictionary<object, object>>();
                Dictionary<object, object> row;
                foreach (DataRow dr in dt.Rows)
                {
                    string icon = "";
                    row = new Dictionary<object, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.ColumnName == "ulstatus" && dr[col].ToString() == "11") {
                            icon = "fa-exclamation-circle";
                        } else if (col.ColumnName == "ulstatus" && dr[col].ToString() == "20") {
                            icon = "fa-check";
                        }
                        else if(col.ColumnName == "ulstatus"){
                            icon = "fa-check";
                        }
                        row.Add(col.ColumnName, dr[col].ToString());
                    }
                    row.Add("uicons", icon);
                    rows.Add(row);
                }
                
                connection.Close();

                success = true;

                if (dt.Rows.Count == 0)
                {
                    success = false;
                }
                return new Response { success = success, detail = rows };
            }
            catch (Exception ex)
            {
                success = false;
                return new Response { success = success, detail = ex.Message };
            }
        }

        // GET api/order/5
        public object Get(string id)
        {
            bool success = true;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String sQuery = @"select
                                ULAccount,
                                ULFilename,
                                UIStatus,
                                ULRemarks,
                                ULStatus,
                                ULPONumber,
                                ULUser, 
                                left(ulorderdate,12) as ULOrderDate,
                                left(uldeliverydate,12) as ULDeliveryDate,
                                uiquantity as UIQuantity,
                                cmcode as ULCustomer,
                                pmcode as UIProduct,
                                pmdescription as PMDescription,
                                ppprice  as UIPrice,
                                (ppprice * uiquantity) as UITPrice
                                from
                                tbluploadlog
                                left join tbluploaditems
                                on tbluploadlog.ULId = tbluploaditems.uiid
                                left join tblcustomermaster
                                on cmid = ulcustomer
                                left join tblproductmaster
                                on pmid = uiproduct
                                left join tblproductpricing
                                on ppproduct = uiproduct and pparea = cmarea
                                where
                                ULId ='" + id + @"'
                                and uiquantity != 0";
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

        // POST api/order
        public void Post([FromBody]string value)
        {
        }

        // PUT api/order/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/order/5
        public void Delete(int id)
        {
        }
    }
}
