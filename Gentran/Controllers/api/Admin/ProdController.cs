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
using Newtonsoft.Json.Linq;

namespace Gentran.Controllers.api
{
    public class ProdController : ApiController
    {
        string userID = HttpContext.Current.Session["UserId"].ToString();
        List<Transaction> rows = new List<Transaction>();
        Boolean success = false;
        // GET api/default1
        public Object Get()
        {
            bool success = true;
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            String sQuery = "select * from tblproductmaster left join tblProductStatus on PMStatus = PSId";
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

        // GET api/default1/5
        public object Get([FromUri]string value)
        {
            Data values = JsonConvert.DeserializeObject<Data>(value);

            bool success = true;

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);

            String sQuery = "";

            if (values.operation == "get_accounts")
            {
                sQuery = "select ATDescription,ATId from tblaccounttype order by ATDescription asc";
            }
            else if (values.operation == "get_mapping")
            {
                sQuery = "select ATDescription,PAAccount,PACode,PAProduct from tblproductassignment left join tblaccounttype on PAAccount = ATId where PAProduct = '" + values.payload[0].pmid + "' order by ATDescription asc";
            }
            else
            {
                sQuery = "select * from tblproductmaster left join tblProductStatus on PMStatus = PSId";
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

        // POST api/default1
        public object Post([FromBody]Data values)
        {
            string activity = "";

            DateTime now = new DateTime();
            now = DateTime.Now;

            string type = "ADM";
            string changes = "";

            string newpaypload = JsonConvert.SerializeObject(values.payload, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            string response = "";
            string value = "";

            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);

            String sQuery = "";
            SqlCommand cmd;//= new SqlCommand(sQuery, connection);
            SqlDataReader dr;

            try
            {



                if (values.operation == "add_product")
                {
                    activity = "ADD30";

                    for (int i = 0; i < values.payload.Count; i++)
                    {
                        var pmcode = values.payload[i].pmcode;
                        var pmbcode = values.payload[i].pmbcode;
                        var pmdesc = values.payload[i].pmdesc;
                        var pmcategory = values.payload[i].pmcategory;
                        var pmstatus = values.payload[i].pmstatus;

                        sQuery = "select top 1 pmcode,pmbarcode from tblproductmaster where pmcode = '" + pmcode + "' or pmbarcode = '" + pmbcode + "'";
                        connection.Open();
                        cmd = new SqlCommand(sQuery, connection);
                        dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            dr.Read();
                            var code = dr["pmcode"].ToString();
                            var bcode = dr["pmbarcode"].ToString();
                            dr.Close();
                            connection.Close();

                            if (pmcode == code)
                            {
                                response = "Product: " + code + " already exist!";
                            }
                            else if (pmbcode == bcode)
                            {
                                response = "Barcode: " + pmbcode + " is already assigned to " + bcode + "!";
                            }

                            success = false;

                            return new Response { success = success, detail = response };
                        }
                        else
                        {
                            connection.Close();
                            sQuery = "insert into tblproductmaster select (SELECT max(pmid)+1 from tblproductmaster ),'" + pmcode + "','" + pmdesc + "','" + pmbcode + "',0,'" + pmcategory + "','" + pmstatus + "'";
                            connection.Open();
                            cmd = new SqlCommand(sQuery, connection);
                            cmd.ExecuteNonQuery();
                            connection.Close();
                            response = "Successful";

                            changes += "Product " + pmcode + " successully added!,";

                            success = true;
                            value = pmcode;
                        }
                    }
                }
                else if (values.operation == "update_mapping")
                {

                    activity = "EDI40";
                                      
                    string pmcode = "";
                    string pmid = "";
                    string pacode = "";
                    string oldpacode = "";
                    string paaccount = "";

                    for (int i = 0; i < values.payload.Count; i++)
                    {
                        pmcode = values.payload[i].pmcode;
                        pmid = values.payload[i].pmid;
                        pacode = values.payload[i].assignedcode;
                        paaccount = values.payload[i].acctype;

                        if (values.payload[i].Status == "1")
                        {

                            sQuery = "select * from tblproductassignment where paproduct='" + pmid + "' AND paaccount='" + paaccount + "'";

                            connection.Open();
                            cmd = new SqlCommand(sQuery, connection);
                            dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                dr.Read();
                                oldpacode = dr["pacode"].ToString();
                                dr.Close();
                                connection.Close();

                                if (pacode != oldpacode)
                                {
                                    changes += "Updated Data Mapping Code for " + pmcode + " [" + paaccount + "] " + " from " + oldpacode + " to " + pacode + ",";
                                    sQuery = "update tblproductassignment set pacode='" + pacode + "' where paproduct='" + pmid + "' AND paaccount='" + paaccount + "'";
                                    connection.Open();
                                    cmd = new SqlCommand(sQuery, connection);
                                    cmd.ExecuteNonQuery();
                                    connection.Close();
                                }
                                else
                                {
                                    connection.Open();
                                    sQuery = "delete from tblproductassignment where paproduct='" + pmid + "' AND paaccount='" + paaccount + "'";
                                    cmd = new SqlCommand(sQuery, connection);
                                    cmd.ExecuteNonQuery();

                                    sQuery = "insert into tblproductassignment select '" + pacode + "','" + pmid + "','" + paaccount + "'";
                                    cmd = new SqlCommand(sQuery, connection);
                                    cmd.ExecuteNonQuery();
                                    connection.Close();
                                }
                                success = true;
                            }
                            else
                            {
                                connection.Close();
                                changes += "Added Data Mapping for " + pmcode + " [" + paaccount + "] " + " with Code " + pacode + ",";
                                sQuery = "insert into tblproductassignment select '" + pacode + "','" + pmid + "','" + paaccount + "'";
                                connection.Open();
                                cmd = new SqlCommand(sQuery, connection);
                                cmd.ExecuteNonQuery();
                                connection.Close();
                                success = true;
                            }
                        }       
                    }
                    string Accounts = "";
                    for (int ctr = 0; ctr < values.payload.Count; ctr++)
                    {
                        if (values.payload[ctr].Status == "1")
                        {
                            if (Accounts == "")
                            {
                                Accounts = "'" + values.payload[ctr].acctype + "'";
                            }
                            else
                            {
                                Accounts += ",'" + values.payload[ctr].acctype + "'";
                            }
                        }
                    }              

                    string atdescription = "";

                    if (Accounts != "")
                    {
                        sQuery = "select distinct PAAccount, atdescription, PACode, PMCode from tblProductAssignment left join tblProductMaster on PMId = PAProduct left join tblAccountType on atid = PAAccount where PAAccount not in (" + Accounts + ") and PAProduct='" + pmid + "'";
                    }
                    else
                    {
                        sQuery = "select distinct PAAccount, atdescription, PACode, PMCode from tblProductAssignment left join tblProductMaster on PMId = PAProduct left join tblAccount on atid = PAAccount where PAProduct='" + pmid + "'";
                    }

                    connection.Open();
                    cmd = new SqlCommand(sQuery, connection);
                    dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        sQuery = "";
                        while (dr.Read())
                        {
                            pacode = dr["PACode"].ToString();
                            pmcode = dr["PMCode"].ToString();
                            paaccount = dr["PAAccount"].ToString();
                            atdescription = dr["atdescription"].ToString();
                            sQuery += "DELETE FROM tblProductAssignment WHERE paproduct='" + pmid + "' AND PACode='" + pacode + "' AND PAAccount='" + paaccount + "';";
                            changes += "Deleted " + pmcode + " data mapping for " + atdescription + " with code " + pacode + ",";
                        }
                        dr.Close();
                        connection.Close();

                        connection.Open();
                        cmd = new SqlCommand(sQuery, connection);
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }

                    value = pmcode;

                    success = true;
                    response = "Successful";
                }
            }
            catch (Exception ex)
            {

                connection.Close();

                success = false;
                response = ex.Message;

                return new Response { success = success, detail = response };
            }

            changes = (changes == "") ? "NC" : changes;
            rows.Add(new Transaction { activity = activity, date = now, remarks = response, user = userID, value = "Product Code: " + value, type = type, changes = changes, payloadvalue = newpaypload, customernumber = "", ponumber = "" });
            return new Response { success = success, detail = rows };

        }

        // PUT api/default1/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/default1/5
        public void Delete(int id)
        {
        }
    }
}
