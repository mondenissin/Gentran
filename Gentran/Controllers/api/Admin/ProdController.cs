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
            String sQuery = "select PMId, PMCode, PMDescription, PMBarcode, PSDescription, PCDescription as PMCategory from tblproductmaster left join tblProductStatus on PMStatus = PSId left join tblProductCategory on PMCategory = PCId where PMId != 0";
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
            bool success = false;  
            string response = "";
                                  
            SqlConnection connection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["DB_GEN"].ConnectionString);
            SqlCommand cmd;
            SqlDataReader dr;
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            String sQuery = "";
            try
            {

                if (values.operation == "check_map_availability")
                {

                    sQuery = "select * from tblProductMaster where PMCode = '"+ values.payload[0].pmcode +"'";
                    cmd = new SqlCommand(sQuery, connection);
                    connection.Open();
                    dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        dr.Read();
                        string pmid = dr["PMId"].ToString();
                        dr.Close();  
                        connection.Close();

                        sQuery = "select top 1 PMCode,PACode from tblProductAssignment left join tblProductMaster on PMid = PAProduct where PAProduct = '" + pmid + "' AND PAAccount = '" + values.payload[0].acctype + "'";
                        cmd = new SqlCommand(sQuery, connection);
                        connection.Open();
                        dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            dr.Read();
                            string pmcode = dr["PMCode"].ToString();
                            string pacode = dr["PACode"].ToString();
                            dr.Close();

                            connection.Close();
                            success = false;
                            response = "SKU ["+ pmcode + "] is already mapped with Mapping Code ["+ pacode + "]";
                            row = new Dictionary<string, object>();
                            row.Add("response", response);
                            rows.Add(row);
                        }
                        else
                        {       
                            connection.Close();
                            sQuery = "select top 1 PMCode from tblProductAssignment left join tblProductMaster on PMid = PAProduct where PACode = '" + values.payload[0].pacode + "' AND PAAccount = '" + values.payload[0].acctype + "'";
                            cmd = new SqlCommand(sQuery, connection);
                            connection.Open();
                            dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                dr.Read();
                                string pmcode = dr["PMCode"].ToString();
                                dr.Close();
                                connection.Close();

                                success = false;
                                response = "Mapping Code [" + values.payload[0].pacode + "] is already assigned to SKU [" + pmcode + "]" ;
                                row = new Dictionary<string, object>();
                                row.Add("response", response);
                                rows.Add(row);
                            }
                            else
                            {
                                connection.Close();
                                success = true;
                                response = "Product mapping available";
                                row = new Dictionary<string, object>();
                                row.Add("response", response);
                                rows.Add(row);
                            }
                        }   

                    }
                    else
                    {
                        connection.Close();

                        success = false;
                        response = "Product not found";
                        row = new Dictionary<string, object>();
                        row.Add("response", response);
                        rows.Add(row);
                    }     
                }
                else
                {
                    if (values.operation == "get_accounts")
                    {
                        sQuery = "select ATDescription,ATId from tblaccounttype order by ATDescription asc";
                    }
                    else if (values.operation == "get_mapping")
                    {
                        sQuery = "select ATDescription,PAAccount,PACode,PAProduct from tblproductassignment left join tblaccounttype on PAAccount = ATId where PAProduct = '" + values.payload[0].pmid + "' order by ATDescription asc";
                    }
                    else if (values.operation == "suggestions")
                    {
                        sQuery = "select TOP 20 PMCode,PMDescription from tblproductmaster WHERE PMId != '0' AND (PMCode LIKE '%" + values.payload[0].prefix + "%' or PMDescription LIKE '%" + values.payload[0].prefix + "%') AND PMId not IN (SELECT PAProduct FROM tblProductAssignment where PAAccount = '" + values.payload[0].acctype + "') order by PMCode asc";
                    }
                    else
                    {
                        sQuery = "select PMId, PMCode, PMDescription, PMBarcode, PMStatus, PMCategory from tblproductmaster left join tblProductStatus on PMStatus = PSId";
                    }


                    cmd = new SqlCommand(sQuery, connection);
                    connection.Open();
                    dr = cmd.ExecuteReader();
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
                        success = true;
                    }
                    else
                        success = false;  

                }
            }
            catch (Exception ex)
            {
                if (connection.State == ConnectionState.Open) { 
                    connection.Close();
                }
                       
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
                else if (values.operation == "batch_mapping")
                {
                    string id = "";
                    success = true;
                    DataTable dt = new DataTable();
                    List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                    Dictionary<string, object> row;

                    dt.Columns.Add("MNC", typeof(String));
                    dt.Columns.Add("CODE", typeof(String));
                    dt.Columns.Add("ACCT", typeof(String));
                    dt.Columns.Add("REMARKS", typeof(String));

                    for (int i = 0; i < values.payload.Count; i++)
                    {
                        try
                        {
                            sQuery = "select pmid from tblproductmaster where pmcode = '" + values.payload[i].pmcode + "'";
                            connection.Open();
                            SqlCommand selectcmd2 = new SqlCommand(sQuery, connection);
                            SqlDataReader dr2 = selectcmd2.ExecuteReader();
                            if (dr2.HasRows)
                            {
                                dr2.Read();
                                id = dr2["pmid"].ToString();
                                connection.Close();

                                sQuery = "select * from tblproductassignment where pacode = '" + values.payload[i].pacode + "' and paproduct = '" + id + "' and paaccount = '" + values.payload[i].acctype + "'";
                                connection.Open();
                                SqlCommand selectcmd3 = new SqlCommand(sQuery, connection);
                                SqlDataReader dr3 = selectcmd3.ExecuteReader();
                                if (dr3.HasRows)
                                {
                                    dr3.Read();
                                    connection.Close();
                                    dt.Rows.Add(new Object[] { values.payload[i].pmcode, values.payload[i].pacode, values.payload[i].acctype, "Duplicate" });
                                }
                                else
                                {
                                    sQuery = "delete tblproductassignment where paproduct = '" + id + "' and paaccount = '" + values.payload[i].acctype + "'";
                                    connection.Open();
                                    SqlCommand selectcmd = new SqlCommand(sQuery, connection);
                                    selectcmd.ExecuteNonQuery();
                                    connection.Close();

                                    sQuery = "insert into tblproductassignment values ('" + values.payload[i].pacode + "','" + id + "','" + values.payload[i].acctype + "')";
                                    connection.Open();
                                    selectcmd = new SqlCommand(sQuery, connection);
                                    selectcmd.ExecuteNonQuery();
                                    connection.Close();
                                    dt.Rows.Add(new Object[] { values.payload[i].pmcode, values.payload[i].pacode, values.payload[i].acctype, "assigned " + values.payload[i].paproduct + " a new code : " + values.payload[i].pacode });
                                }
                            }
                            else
                            {
                                connection.Close();
                                dt.Rows.Add(new Object[] { values.payload[i].pmcode, values.payload[i].pacode, values.payload[i].acctype, "Product Code not found" });
                            }
                            success = true;
                        }
                        catch (Exception ex)
                        {
                            connection.Close();
                            success = false;
                        }
                    }

                    foreach (DataRow sDr in dt.Rows)
                    {
                        row = new Dictionary<string, object>();
                        foreach (DataColumn col in dt.Columns)
                        {
                            row.Add(col.ColumnName, sDr[col]);
                        }
                        rows.Add(row);
                    }
                    return new Response { success = success, detail = rows };
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
                        sQuery = "select distinct PAAccount, atdescription, PACode, PMCode from tblProductAssignment left join tblProductMaster on PMId = PAProduct left join tblAccountType on atid = PAAccount where PAProduct='" + pmid + "'";
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
