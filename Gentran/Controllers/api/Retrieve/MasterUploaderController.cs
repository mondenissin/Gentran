using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Gentran.Controllers.api.Retrieve
{
    public class MasterUploaderController : ApiController
    {
        AppSettings appSet = new AppSettings();
        List<Transaction> trows = new List<Transaction>();
        string userID = HttpContext.Current.Session["UserId"].ToString();
        Boolean success = true;
        private DateTime uDate = DateTime.Now;
        private AppSettings app = new AppSettings();
        private string filePath = "", absoluteName = "";
        private Boolean ifMult = false;
        // GET api/masteruploader
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/masteruploader/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/masteruploader
        public object Post([FromBody] Data values)
        {
            //DataTable dtP = new DataTable() , dtC = new DataTable();
            //List<MapData> map = new List<MapData>();
            //List<UnmapData> unmap = new List<UnmapData>();
            //map.Add(new MapData { PONumber = "" });

            //string ponum, custcode = "", pocode = "", qty, data = "";
            //string response = "";
            var time = System.Diagnostics.Stopwatch.StartNew();

            List<Dictionary<object, object>> xmlObject = new List<Dictionary<object, object>>();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;

            int datactr = 0;
            bool ifFormatted = false;
            string tempCust = "";
            string[] data = { };
            string[] tempData = new string[8];
            string[] colName = { "CustNo", "PONum", "oDate", "dDate", "ProdCode", "qty", "uUser", "uAcct" };
            string cust = "", ponum = "", podate = "", deldate = "", qty = "", barcode = "";
            //cust,ponum,podate,deldate,blank,blank,blank,prod,qty,userid,outlet

            try
            {
                if (values.payload[0].outlet == "SM")
                {
                    data = appSet.csv(values.payload[0].fileName);
                    datactr = data.Length;
                    ifFormatted = true;
                }
                else if (values.payload[0].outlet == "S8")
                {
                    xmlObject = appSet.xml(values.payload[0].fileName);

                    int arrayCtr = 0;
                    var obj = from ctr in xmlObject select ctr;
                    var objValues = obj.ToList().Where(x => x.ContainsKey("DestinationCode")
                        || x.ContainsKey("PONumber")
                        || x.ContainsKey("PODate")
                        || x.ContainsKey("DeliveryDate")
                        || x.ContainsKey("Barcode")
                        || x.ContainsKey("Quantity")
                    );
                    datactr = obj.ToList().Where(x => x.ContainsKey("Barcode")).Count();
                    data = new string[datactr];
                    
                    foreach (var listKey in objValues)
                    {
                        if (listKey.Keys.ElementAt(0).ToString() == "DestinationCode") {
                            cust = listKey["DestinationCode"].ToString();
                        }
                        if (listKey.Keys.ElementAt(0).ToString() == "PONumber")
                        {
                            ponum = listKey["PONumber"].ToString();
                        }
                        if (listKey.Keys.ElementAt(0).ToString() == "PODate")
                        {
                            podate = listKey["PODate"].ToString().Substring(4) + listKey["PODate"].ToString().Substring(2, 2);
                        }
                        if (listKey.Keys.ElementAt(0).ToString() == "DeliveryDate")
                        {
                            deldate = listKey["DeliveryDate"].ToString().Substring(4) + listKey["DeliveryDate"].ToString().Substring(2, 2);
                        }
                        if (listKey.Keys.ElementAt(0).ToString() == "Barcode") {
                            barcode = listKey["Barcode"].ToString();
                        }
                        if (listKey.Keys.ElementAt(0).ToString() == "Quantity" && barcode != "")
                        {
                            data[arrayCtr] = cust + "," + ponum + "," + podate.Replace(" ", "") + "," + deldate.Replace(" ", "") + "," + "," + "," + "," + barcode + "," + listKey["Quantity"].ToString();
                            arrayCtr++;
                        }
                    }
                    
                    ifFormatted = true;
                } else if (values.payload[0].outlet == "NCC") {

                    xmlObject = appSet.xml(values.payload[0].fileName);

                    int arrayCtr = 0;
                    var obj = from ctr in xmlObject select ctr;
                    var objValues = obj.ToList().Where(x => x.ContainsKey("deliver_to")
                        || x.ContainsKey("po_number")
                        || x.ContainsKey("podate")
                        || x.ContainsKey("delvdate")
                        || x.ContainsKey("upc")
                        || x.ContainsKey("order_qty")
                    );
                    datactr = obj.ToList().Where(x => x.ContainsKey("sku")).Count();
                    data = new string[datactr];

                    foreach (var listKey in objValues)
                    {
                        if (listKey.Keys.ElementAt(0).ToString() == "deliver_to")
                        {
                            string[] custSplit = listKey["deliver_to"].ToString().Split('-');
                            cust = custSplit[1] +"-"+custSplit[0];
                        }
                        if (listKey.Keys.ElementAt(0).ToString() == "po_number")
                        {
                            ponum = listKey["po_number"].ToString();
                        }
                        if (listKey.Keys.ElementAt(0).ToString() == "podate")
                        {
                            podate = listKey["podate"].ToString().Replace('/',' ');
                        }
                        if (listKey.Keys.ElementAt(0).ToString() == "delvdate")
                        {
                            deldate = listKey["delvdate"].ToString().Replace('/', ' ');
                        }
                        if (listKey.Keys.ElementAt(0).ToString() == "order_qty")
                        {
                            qty = listKey["order_qty"].ToString().Substring(0, listKey["order_qty"].ToString().IndexOf('.'));
                        }
                        if (listKey.Keys.ElementAt(0).ToString() == "upc" && qty != "")
                        {
                            data[arrayCtr] = cust + "," + ponum + "," + podate.Replace(" ","") + "," + deldate.Replace(" ", "") + "," + "," + "," + "," + listKey["upc"].ToString() + "," + qty;
                            arrayCtr++;
                        }
                    }

                    ifFormatted = true;
                }
                
                if (ifFormatted)
                {
                    string[] temp = values.payload[0].fileName.Split('\\');
                    absoluteName = temp[temp.Length - 1];
                    filePath = values.payload[0].fileName;

                    File.Move(values.payload[0].fileName, @"C:\inetpub\wwwroot\files\Gentran\queued\" + absoluteName);

                    for (int x = 0, y = datactr; x < y; x++)
                    {
                        //string[] split = tempdata[x].Split(',').Where(a => !String.IsNullOrEmpty(a)).ToArray();
                        string[] split = data[x].Split(',');
                        if (x == 1) tempCust = split[0];

                        if (split[4] == "")
                        {
                            tempData[0] = split[0]; // Customer Code
                            tempData[1] = split[1]; // PO Number
                            tempData[2] = app.toDate((split[2])); // Order date
                            tempData[3] = app.toDate(split[3]); // Delivery date
                            tempData[4] = split[7]; // Product Code 
                            tempData[5] = split[8]; // Quantity
                            tempData[6] = HttpContext.Current.Session["UserId"].ToString(); // User ID
                            tempData[7] = values.payload[0].outlet; // Outlet

                            row = new Dictionary<string, object>();
                            if (tempCust != tempData[0])
                            {
                                ifMult = true;
                                SaveData(rows);
                                tempCust = tempData[0];
                                row.Clear();
                                rows.Clear();
                            }

                            for (int z = 0; z < colName.Length; z++)
                            {
                                row.Add(colName[z], tempData[z]);
                            }
                            rows.Add(row);

                            if (x == datactr - 1)
                            {
                                ifMult = false;
                                SaveData(rows);
                            }
                        }
                    }
                }
                else {
                    data[0] = "File not formatted!";
                }
                #region Version2_Linq

                /*SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
                String sQuery = "select * from tblproductmaster";
                SqlDataAdapter dataAdap = new SqlDataAdapter(sQuery, connection);
                connection.Open();
                dataAdap.Fill(dtP);
                connection.Close();*/

                //var datas = (from prod in dtP.AsEnumerable() where split.Any(c => c.Equals(prod.Field<string>("PMCode"))) select prod.ItemArray[1]);
                //var eDatas = (from prod in dtP.AsEnumerable() where split.Any(c => c!=prod.Field<string>("PMCode")) select prod.ItemArray[1]);

                //foreach (var d in datas) { map.Add(new MapData { POCode = d.ToString(), CustCode = custcode }); }
                //foreach (var d in eDatas) { unmap.Add(new UnmapData { ErrorData = d.ToString() }); }

                /*for (int z=0,c = split.Length; z<c; z++) {
                    var product = from prod in dtP.AsEnumerable() where prod.Field<string>("PMCode") == split[z] select prod.ItemArray[1];
                    //var customer = from cust in dtC.AsEnumerable() where cust.Field<int>("CMCode") == Convert.ToInt32(split[z]) select cust.ItemArray[1];
                    foreach (var pr in product){
                        map.Add(new MapData { POCode = pr.ToString(), CustCode = custcode });
                    }
                    //foreach (var cc in customer) { custcode = cc.ToString(); }

                    /*row = new Dictionary<string, object>();
                    row.Add("Error", ex.Message);
                    rows.Add(row);*/
                //}


                //var qwe = map;
                //var product = from prod in dt.AsEnumerable() where prod.Field<string>("PMCode") == "PSSC12X110G11" select prod.ItemArray[1];
                //string asd = product.ToString();

                //success = true;
                #endregion

            }
            catch (Exception ex)
            {
                data[0] = ex.Message;
                success = false;
            }

            time.Stop();
            var execTime = time.ElapsedMilliseconds;

            return new Response { success = success, filecontent = data, execution = execTime, detail = trows};
        }

        // PUT api/masteruploader/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/masteruploader/5
        public void Delete(int id)
        {
        }

        /*private string[] csv(string fileName) {


            string[] readed = { };
            string filepath;
            filepath = fileName;
            readed = File.ReadAllLines(filepath);

            return readed;
        }*/


        private void SaveData(List<Dictionary<string, object>> data) {
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            string uQty = "", uPrice = "", uPONum = "", uAcct = "", uODate = "", uDDate = "", uUser = "",uRemarks = "", uCust = "",uID="",uProd="";
            string response = "Successful";
            Boolean NoCustomer = false;
            Boolean validPO = true;

            for (int x=0;x<data.Count;x++) {
                try
                {
                    uQty = data[x]["qty"].ToString();
                    uPrice = "0";

                    if (x == 0)
                    {
                        //<!GET ORDER HEADER------------------------------------------------------------------

                        uPONum = data[x]["PONum"].ToString();
                        uAcct = data[x]["uAcct"].ToString();
                        uODate = data[x]["oDate"].ToString();
                        uDDate = data[x]["dDate"].ToString();
                        uRemarks = "";
                        uUser = data[x]["uUser"].ToString();
                        uCust = data[x]["CustNo"].ToString();

                        String selectULCustomer = "SELECT CACustomer FROM tblCustomerAssignment WHERE CACode = '" + uCust + "' and CAAccount = '" + uAcct + "'";
                        SqlCommand selectCmdULCustomer = new SqlCommand(selectULCustomer, connection);
                        connection.Open();
                        SqlDataReader drULCustomer = selectCmdULCustomer.ExecuteReader();

                        if (drULCustomer.HasRows)
                        {
                            drULCustomer.Read();
                            uCust = drULCustomer["CACustomer"].ToString();
                            connection.Close();
                        }
                        else
                        {
                            connection.Close();
                            uCust = "0";
                            //if (ULAccount == "ALT" || ULAccount == "GGM" || ULAccount == "UNT" || ULAccount == "PRN" || ULAccount == "IST")
                                //NoCustomer = false;
                            //else
                            NoCustomer = true;
                        }

                        uID = uCust + uPONum;

                        //GET ORDER HEADER-------------------------------------------------------------------->

                        //<!CHECK ORDER HEADER-----------------------------------------------------------------

                        String selectULId = "SELECT * FROM tblUploadLog WHERE ULPONumber = '" + uPONum + "' AND ULAccount = '" + uAcct + "'";
                        connection.Open();
                        SqlCommand cmdULId = new SqlCommand(selectULId, connection);
                        SqlDataReader drULId = cmdULId.ExecuteReader();
                        if (drULId.HasRows)
                        {
                            drULId.Read();

                            if (Convert.ToInt32(drULId["ULStatus"]) <= 20)
                            {
                                drULId.Close();

                                String deleteULId = "DELETE FROM tblUploadLog WHERE ULPONumber = '" + uPONum + "' AND ULAccount = '" + uAcct + "'";
                                SqlCommand deleteCmdULId = new SqlCommand(deleteULId, connection);
                                deleteCmdULId.ExecuteNonQuery();
                                connection.Close();
                            }
                            else
                            {
                                connection.Close();
                                success = false;
                                validPO = false;
                                response = "PO Number " + uPONum + " already Exist!";
                            }
                        }
                        else
                        {
                            connection.Close();
                        }

                        //CHECK ORDER HEADER------------------------------------------------------------------>

                        //<!INSERT ORDER HEADER----------------------------------------------------------------
                        if (validPO == true)
                        {
                            String insertULId = "INSERT INTO tblUploadLog SELECT '" + uID + "','" + uPONum + "','" + uCust + "','" + uODate + "','" + uDDate + "','" + uDate + "','','" + uUser + "','10','" + uRemarks + "','" + uAcct + "'";
                            connection.Open();
                            SqlCommand insertCmdULId = new SqlCommand(insertULId, connection);
                            insertCmdULId.ExecuteNonQuery();
                            connection.Close();


                            //INSERT ORDER HEADER------------------------------------------------------------------>

                            //<!CHECK CUSTOMER----------------------------------------------------------------------

                            if (NoCustomer == true)
                            {
                                String insertELId = "insert into tblErrorLog select '" + uID + "','101','" + uCust + "'";
                                connection.Open();
                                SqlCommand cmdELIdInsert = new SqlCommand(insertELId, connection);
                                cmdELIdInsert.ExecuteNonQuery();
                                connection.Close();

                                success = false;
                                response = "Invalid Store Code: " + uCust;
                            }
                        }
                        //CHECK CUSTOMER------------------------------------------------------------------------>

                    }
                    //<!INSERT ORDER ------------------------------------------------------------------------
                    if (validPO == true)
                    {
                        if (NoCustomer == false)
                        {
                            String selectUIProduct = "SELECT PAProduct FROM tblProductAssignment WHERE PACode = '" + data[x]["ProdCode"].ToString() + "' and PAAccount = '" + uAcct + "'";
                            SqlCommand cmdUIProduct = new SqlCommand(selectUIProduct, connection);
                            connection.Open();
                            SqlDataReader drUIProduct = cmdUIProduct.ExecuteReader();

                            if (drUIProduct.HasRows)
                            {
                                drUIProduct.Read();
                                uProd = drUIProduct["PAProduct"].ToString();
                                connection.Close();

                                String selectUIId = "select * from tblUploadItems where uiid = '" + uID + "' and uiproduct = '" + uProd + "'";
                                connection.Open();
                                SqlCommand cmdUIId = new SqlCommand(selectUIId, connection);
                                SqlDataReader drUIId = cmdUIId.ExecuteReader();

                                if (drUIId.HasRows)
                                {
                                    connection.Close();
                                    success = false;
                                    response = "Duplicate SKU " + uProd + " with " + uQty + " quantity ordered!";

                                    String insertELId = "insert into tblErrorLog select '" + uID + "','103','" + response + "'";
                                    connection.Open();
                                    SqlCommand cmdELIdInsert = new SqlCommand(insertELId, connection);
                                    cmdELIdInsert.ExecuteNonQuery();
                                    connection.Close();
                                }
                                else
                                {
                                    connection.Close();
                                    
                                    if (uQty != "0")
                                    {
                                        String insertUIId = "insert into tblUploadItems select '" + uID + "','" + uProd + "','" + uQty + "','" + uQty + "','" + uPrice + "','1'";
                                        connection.Open();
                                        SqlCommand cmdUIIdInsert = new SqlCommand(insertUIId, connection);
                                        cmdUIIdInsert.ExecuteNonQuery();
                                        connection.Close();
                                    }
                                    
                                }
                            }
                            else
                            {
                                connection.Close();
                                success = false;
                                response = "Invalid Store Product: " + data[x]["ProdCode"].ToString();

                                String insertELId = "insert into tblErrorLog select '" + uID + "','102','" + data[x]["ProdCode"].ToString() + "'";
                                connection.Open();
                                SqlCommand cmdELIdInsert = new SqlCommand(insertELId, connection);
                                cmdELIdInsert.ExecuteNonQuery();
                                connection.Close();
                            }
                        }
                    }
                    //INSERT ORDER ------------------------------------------------------------------------->
                }
                catch (Exception ex)
                {
                    connection.Close();
                    success = false;
                    response = ex.Message;
                }
            }
            // UPDATE ORDERS AND FILE CONTROLLER----------------------------------------------------------------------->
            if (validPO == true)
            {
                try
                {
                    String fileExtension = "";
                    String ULStatus = "";
                    String destinationFolder = "";
                    
                    string[] splitEx = filePath.Split('.');
                    fileExtension = splitEx[splitEx.Length - 1];

                    if (success == true)
                    {
                        ULStatus = "20";
                        
                        destinationFolder = "successful";
                    }
                    else
                    {
                        ULStatus = "11";
                        destinationFolder = "failed";
                    }

                    String update = "update tblUploadLog set ulstatus = '" + ULStatus + "',ulfilename = '" + uID + "." + fileExtension + "' where ULId = '" + uID + "';";
                    connection.Open();
                    SqlCommand updateCmd = new SqlCommand(update, connection);
                    updateCmd.ExecuteNonQuery();
                    connection.Close();

                    /*string[] fileName = filePath.Split('.');
                    string newFileName = fileName[1] == "xls" || fileName[1] == "xlsx" || fileName[1] == "xlsm" ? newFileName = fileName[0] + ".csv" : newFileName = values.payload[0].ULFileName;

                    if (fileName[1] == "xls" || fileName[1] == "xlsx" || fileName[1] == "xlsm")
                    {
                        String csvSource = "";
                        if (!values.payload[0].ReUpload)
                            csvSource = @"C:\inetpub\wwwroot\files\pos\scheduled\" + newFileName;
                        else
                            csvSource = @"C:\inetpub\wwwroot\files\pos\failed\" + newFileName;

                        if (File.Exists(csvSource))
                        {
                            File.Delete(csvSource);
                        }
                    }*/
                    //DTO
                    //if (!values.payload[0].ReUpload)
                    //{
                    String source = @"C:\inetpub\wwwroot\files\Gentran\queued\" + absoluteName;

                    String destination = @"C:\inetpub\wwwroot\files\Gentran\" + destinationFolder + @"\" + uID + "." + fileExtension;

                    String failed = @"C:\inetpub\wwwroot\files\Gentran\failed\" + uID + "." + fileExtension;

                    if (destinationFolder == "successful")
                    {
                        if (File.Exists(failed))
                        {
                            File.Delete(failed);
                        }
                    }

                    if (File.Exists(destination))
                    {
                        File.Delete(destination);
                    }

                    if (ifMult)
                        File.Copy(source, destination);
                    else
                        File.Move(source, destination);


                    string newpaypload = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    trows.Add(new Transaction { response = response, activity = "UPL20", date = uDate, remarks = uPONum, user = userID, type = "ADM", value = "PO ID:" + uCust + uPONum, changes = "", payloadvalue = newpaypload, customernumber = uCust, ponumber = uPONum });

                    /*}
                    else
                    {
                        String source = @"C:\inetpub\wwwroot\files\pos\failed\" + values.payload[0].ULFileName;

                        String destination = @"C:\inetpub\wwwroot\files\pos\" + destinationFolder + @"\" + ULId + "." + fileExtension;

                        if (destinationFolder == "successful")
                        {
                            if (File.Exists(source))
                            {
                                if (File.Exists(destination))
                                {
                                    File.Delete(destination);
                                }
                            }
                        }

                        File.Move(source, destination);
                    }*/
                }
                catch (Exception ex)
                {
                    connection.Close();
                    success = false;
                    response = ex.Message;
                }
            }
            else
            {
                try
                {
                    String source = @"C:\inetpub\wwwroot\files\Gentran\scheduled\" + "";

                    if (File.Exists(source))
                    {
                        File.Delete(source);
                    }

                }
                catch (Exception ex)
                {
                    connection.Close();
                    success = false;
                    response = ex.Message;
                }
            }
        }
    }
}
