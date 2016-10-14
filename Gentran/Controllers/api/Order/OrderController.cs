using Newtonsoft.Json;
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
        string userID = HttpContext.Current.Session["UserId"].ToString();
        List<Transaction> rows = new List<Transaction>();
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
                            ul.ulremarks,
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
                    string sStat = "";
                    row = new Dictionary<object, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.ColumnName == "ulstatus" && dr[col].ToString() == "11") {
                            sStat = "Read Failed";
                            icon = "fa-exclamation-circle";
                        } else if (col.ColumnName == "ulstatus" && dr[col].ToString() == "20") {
                            sStat = "New";
                            icon = "fa-check";
                        }
                        else if(col.ColumnName == "ulstatus"){
                            sStat = "New";
                            icon = "fa-check";
                        }
                        row.Add(col.ColumnName, dr[col].ToString());
                    }
                    row.Add("sStatus", sStat);
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
        public object Put([FromBody]Data values)   // FOR EDIT PO DETAILS
        {
            bool success = true;
            string response = "Successful";
            string error = "";
            String status = "", sQuery = string.Empty;
            String CustomerNumber = "";
            DateTime now = new DateTime();
            now = DateTime.Now;

            if (values.payload[0].ulstatus == "15")
            {
                status = "20";
            }
            else
            {
                status = values.payload[0].ulstatus;
            }

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            try
            {
                String selectCMId = "select cmid from tblcustomermaster where cmcode ='" + values.payload[0].customernumber + "'";
                connection.Open();
                SqlCommand selectCmdCMId = new SqlCommand(selectCMId, connection);
                SqlDataReader dr = selectCmdCMId.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    CustomerNumber = dr["cmid"].ToString();
                    connection.Close();
                    if (CustomerNumber == "0")
                    {
                        success = false;
                        response = "Please enter a Customer Code!";
                    }
                    else
                    {
                        if (values.payload[0].ULId == CustomerNumber + values.payload[0].ponumber)
                        {
                            String updateULId = "update tbluploadlog set ULStatus = '" + status + "',ULId = '" + CustomerNumber + values.payload[0].ponumber + "', ulcustomer = '" + CustomerNumber + "', ULDeliveryDate = '" + values.payload[0].DeliveryDate + "', ulremarks = '" + values.payload[0].remarks + "' where ULId = '" + values.payload[0].ULId + "'";
                            //GMC String updateULId = "update tbluploadlog set ULPONumber = '" + values.payload[0].ULPONumber + "',ULStatus = '" + status + "',ULId = '" + CustomerNumber + values.payload[0].ULPONumber + "', ulcustomer = '" + CustomerNumber + "', ULDeliveryDate = '" + values.payload[0].ULDeliveryDate + "', ulremarks = '" + values.payload[0].ULRemarks + "' where ULId = '" + values.payload[0].ULId + "'";

                            connection.Open();
                            SqlCommand updateCmdULId = new SqlCommand(updateULId, connection);
                            updateCmdULId.ExecuteNonQuery();
                            connection.Close();

                            string newpaypload = JsonConvert.SerializeObject(values.payload, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                            rows.Add(new Transaction { activity = "EDI20", date = now, remarks = values.payload[0].changes, user = userID, type = "ADM" ,value = "PO ID:" + CustomerNumber + values.payload[0].ponumber, changes = values.payload[0].changes, payloadvalue = newpaypload, customernumber = CustomerNumber, ponumber = values.payload[0].ponumber });
                            return new Response { success = success, detail = rows };
                        }
                    }
                }
                else
                {
                    connection.Close();
                    success = false;
                    response = "Invalid Customer Code!";
                }

                /*if (success == true)
                {
                    String Product = "";
                    String Products = "";

                    for (int i = 0; i < values.payload.Count; i++)
                    {
                        String selectPMId = "select * from tblproductmaster where pmcode = '" + values.payload[i].UIProduct + "'";
                        connection.Open();
                        SqlCommand selectCmdPMId = new SqlCommand(selectPMId, connection);
                        SqlDataReader drPMId = selectCmdPMId.ExecuteReader();

                        if (drPMId.HasRows)
                        {
                            drPMId.Read();
                            Product = drPMId["PMId"].ToString();
                            connection.Close();
                        }
                        else
                        {
                            connection.Close();
                            success = false;
                            response = "Invalid Product Code " + values.payload[i].UIProduct;
                        }

                        if (i == 0)
                        {
                            Products = Product;
                        }
                        else
                        {
                            Products = Products + "," + Product;
                        }
                    }
                }*/
            }
            catch (Exception ex)
            {
                connection.Close();
                success = false;
                response = ex.Message;
            }
            
            return new Response { success = success, detail = response, errortype = error };
        }

        // DELETE api/order/5
        public void Delete(int id)
        {
        }
    }
}
