﻿using System;
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
using System.Data;

namespace Gentran.Controllers.api
{
    public class MonitorController : ApiController
    {
        private DataSet dt;
        string userID = HttpContext.Current.Session["UserId"].ToString();
        List<Transaction> rows = new List<Transaction>();
        // GET api/default1
        public Object Get()
        {
            DataTable dt = new DataTable();
            bool success = true;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            //String sQuery = @"select 
            //                    RFId,
            //                    RFFilename,
            //                    ULPONumber,
            //                    ULCustomer,
            //                    CMDescription,
            //                    left(ULOrderDate,12) as ULOrderDate,
            //                    left(ULDeliveryDate,12) as ULDeliveryDate,
            //                    left(ULCancelDate,12) as ULCancelDate,
            //                    left(RFRetrieveDate,12) + '- ' + CONVERT (varchar(15),CAST(RFRetrieveDate as time),100) as RFRetrieveDate,
            //                    left(ULReadDate,12) + '- ' + CONVERT (varchar(15),CAST(ULReadDate as time),100) as ULReadDate,
            //                    left(ULSubmitDate,12) + '- ' + CONVERT (varchar(15),CAST(ULSubmitDate as time),100) as ULSubmitDate,
            //                    ULReadUser,
            //                    ULSubmitUser,
            //                    RFAccount,
            //                    RSDescription
            //                from tblrawfile 
            //                left join tbluploadlog 
            //                    on ulfile = rfid
            //                left join tblcustomermaster
            //                    on cmid = ulcustomer
            //                left join tblrawfilestatus
            //                    on rfstatus = rsid
            //                order by RFRetrieveDate desc";

            String selectStr = @"SELECT DISTINCT
                            ur.rfid,
                            ul.ulid,
                            ul.ulponumber,
                            cm.cmcode as ulcustomer,
                            cm.cmdescription, 
                            LEFT(ul.ulorderdate,12) AS ulorderdate,
                            LEFT(ul.uldeliverydate,12) AS uldeliverydate,
                            LEFT(ul.ulcanceldate,12) AS ulcanceldate,
                            ul.ulreaddate as sortupload,
                            LEFT(ul.ulreaddate,12) +'- ' + CONVERT(varchar(15), CAST(ul.ulreaddate as time), 100) AS ulreaddate,
                            LEFT(ul.ulsubmitdate,12) +'- ' + CONVERT(varchar(15), CAST(ul.ulsubmitdate as time), 100) AS ulsubmitdate,
                            ui.sumulquantity,
                            ui.countulquantity,
                            ul.ulstatus,
                            us.usdescription,
                            ul.ulremarks,
                            ur.rffilename,
                            ui.uiprice,
                            ur.rfaccount
                            FROM tblUploadLog ul
                            LEFT JOIN tblUploadStatus us
                            on ulstatus = usid
                            LEFT JOIN tblCustomerMaster cm
                            ON ul.ulcustomer = cm.cmid 
                            LEFT JOIN tblUserAssignment ua 
                            ON ul.ulcustomer = ua.uacustomer 
                            LEFT JOIN tblRawFile ur
                            ON ur.RFId = ul.ULFile
                            LEFT JOIN
                            (SELECT
                            uiprice = SUM(uiquantity * ppprice),
                            sumulquantity = SUM(uiquantity),
                            countulquantity = COUNT(uiquantity),
                            uiid
                            FROM tblUploadItems
                            left join tblUploadLog
                            on ulid = uiid
                            left join tblCustomerMaster
                            on cmid = ulcustomer
                            LEFT JOIN tblProductPricing
                            on ppproduct = uiproduct and pparea = cmarea
                            WHERE uistatus NOT IN ('3','0')
                            group by uiid ) ui 
                            ON ul.ulid = ui.uiid 
                            WHERE ua.uauser = '17002'
                            AND uatype = 'KAS' 
                            AND ulstatus != 25
                            OR ulstatus != 21  
                            AND ul.ulReadDate = GETDATE()
                            ORDER BY sortupload desc"; //Dec. 6, 2016

            //SqlCommand cmd = new SqlCommand(sQuery, connection);
            //List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            //Dictionary<string, object> row;

            try
            {
                //connection.Open();
                //SqlDataReader dr = cmd.ExecuteReader();
                //if (dr.HasRows)
                //{
                //    while (dr.Read())
                //    {
                //        row = new Dictionary<string, object>();
                //        for (int i = 0; i < dr.FieldCount; i++)
                //        {
                //            var cName = dr.GetName(i);
                //            row.Add(cName, dr[cName]);
                //        }
                //        rows.Add(row);
                //    }
                //}
                //else
                //    success = false;

                SqlDataAdapter dataadapter = new SqlDataAdapter(selectStr, connection);
                connection.Open();
                dataadapter.Fill(dt);

                List<Dictionary<object, object>> rows = new List<Dictionary<object, object>>();
                Dictionary<object, object> row;
                foreach (DataRow dr in dt.Rows)
                {
                    string icon = "";
                    string sStat = "";
                    string oClass = "";
                    int eCtr = 0;
                    row = new Dictionary<object, object>();
                    foreach (DataColumn col in dt.Columns)
                    {

                        if (col.ColumnName == "ulstatus" && dr[col].ToString() == "11")
                        {
                            sStat = dt.Rows[0]["usdescription"].ToString();
                            icon = "fa-exclamation-circle";
                            oClass = "label label-danger";
                        }
                        else if (col.ColumnName == "ulstatus" && dr[col].ToString() == "20")
                        {
                            sStat = dt.Rows[0]["usdescription"].ToString();
                            icon = "fa-check";
                            oClass = "label label-success";
                        }
                        row.Add(col.ColumnName, dr[col].ToString());
                    }
                    row.Add("sStatus", sStat);
                    row.Add("uicons", icon);
                    row.Add("eCtr", eCtr);
                    row.Add("oClass", oClass);
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
                //success = false;
                //row = new Dictionary<string, object>();
                //row.Add("Error", ex.Message);
                //rows.Add(row);
                success = false;
                return new Response { success = success, detail = ex.Message };
            }

            //return new Response { success = success, detail = rows };
        }

        //public Object Get([FromUri]string value)
        //{

        //    Data values = JsonConvert.DeserializeObject<Data>(value);

        //    bool success = true;
        //    SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
        //    String sQuery = "";
        //    if (values.operation != "transaction")
        //    {
        //        sQuery = "select * from tblrawfile left join tbluploadlog on ulfile = rfid where ulponumber = " + values.payload[0].ULPONumber + " ";
        //    }
        //    else {
        //        sQuery = @"select 
        //                        TLId, 
        //                        TADescription, 
        //                        TLValue, 
        //                        TLRemarks, 
        //                        left(TLDate,12) +'- ' + CONVERT(varchar(15), CAST(TLDate as time), 100) as TLDate ,
        //                        TLUser
        //                    from tblTransactionLog
        //                    left join tblTransactionActivity
        //                        on taid = TLActivity
        //                    where TLUser = '000000'
        //                    order by TLId desc"; //" + HttpContext.Current.Session["UserId"].ToString() + "
        //    }

        //    SqlCommand cmd = new SqlCommand(sQuery, connection);
        //    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        //    Dictionary<string, object> row;

        //    try
        //    {
        //        connection.Open();
        //        SqlDataReader dr = cmd.ExecuteReader();
        //        if (dr.HasRows)
        //        {
        //            while (dr.Read())
        //            {
        //                row = new Dictionary<string, object>();
        //                for (int i = 0; i < dr.FieldCount; i++)
        //                {
        //                    var cName = dr.GetName(i);
        //                    row.Add(cName, dr[cName]);
        //                }
        //                rows.Add(row);
        //            }
        //        }
        //        else
        //            success = false;
        //    }
        //    catch (Exception ex)
        //    {
        //        success = false;
        //        row = new Dictionary<string, object>();
        //        row.Add("Error", ex.Message);
        //        rows.Add(row);
        //    }

        //    return new Response { success = success, detail = rows };
        //}
        // GET api/order/5
        public object Get(string id)
        {
            bool success = true;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String sQuery = @"select
                            rfaccount,
                            rffilename,
                            UIStatus,
                            ULRemarks,
                            ULStatus,
                            ULPONumber,
                            ULReadUser, 
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
                            on tbluploadlog.ulid = tbluploaditems.uiid
                            left join tblRawFile
                            on RFId = ULFile
                            left join tblcustomermaster
                            on cmid = ulcustomer
                            left join tblproductmaster
                            on pmid = uiproduct
                            left join tblproductpricing
                            on ppproduct = uiproduct and pparea = cmarea
                            where
                            ulid ='" + id + @"'
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
        public object Put([FromBody]Data values)
        {
            bool success = true;
            string response = "Successful";
            string CustomerNumber = "";
            string error = "";
            String status = "", sQuery = string.Empty;
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
                        String updateULId = "update tbluploadlog set ULStatus = '" + status + "', ulcustomer = '" + CustomerNumber + "', ULDeliveryDate = '" + values.payload[0].DeliveryDate + "', ulremarks = '" + values.payload[0].remarks + "' where ulid = '" + values.payload[0].ULId + "'";
                        //GMC String updateULId = "update tbluploadlog set ULPONumber = '" + values.payload[0].ULPONumber + "',ULStatus = '" + status + "',ULId = '" + CustomerNumber + values.payload[0].ULPONumber + "', ulcustomer = '" + CustomerNumber + "', ULDeliveryDate = '" + values.payload[0].ULDeliveryDate + "', ulremarks = '" + values.payload[0].ULRemarks + "' where ULId = '" + values.payload[0].ULId + "'";

                        connection.Open();
                        SqlCommand updateCmdULId = new SqlCommand(updateULId, connection);
                        updateCmdULId.ExecuteNonQuery();
                        connection.Close();

                        string newpaypload = JsonConvert.SerializeObject(values.payload, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                        rows.Add(new Transaction { activity = "EDI20", date = now, remarks = values.payload[0].changes, user = userID, type = "ADM", value = "PO ID:" + values.payload[0].ULId, changes = "", payloadvalue = newpaypload, customernumber = "", ponumber = "" });
                        return new Response { success = success, detail = rows, notiftext = response };

                    }
                }
                else
                {
                    connection.Close();
                    success = false;
                    response = "Invalid Customer Code!";
                }
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
