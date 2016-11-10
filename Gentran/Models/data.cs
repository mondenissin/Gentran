using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Runtime.Serialization;

//---------INPUTS------------//
public class Data
{
    public string reportName { get; set; }
    public string operation { get; set; }
    public List<Value> payload { get; set; }
    public List<Value> customer { get; set; }
    public List<Value> downloadfile { get; set; }
}
public class Value
{
    //------POSheet--------//
    public string PSid { get; set; }
    public string PSCustomer { get; set; }
    public string PSProduct { get; set; }
    public string PSQuantity { get; set; }
    public string PSUser { get; set; }

    //------Customer-------//
    public string cmID { get; set; }

    public string cmCode { get; set; }

    public string cmName { get; set; }

    public string cmStat { get; set; }

    public string cmArea { get; set; }

    //------Customer Assignment-------//

    public string caCode { get; set; }

    //------USER------//

    public string umid { get; set; }

    public string id { get; set; }

    public string status { get; set; }

    public string username { get; set; }

    public string firstname { get; set; }

    public string nickname { get; set; }

    public string middlename { get; set; }

    public string lastname { get; set; }

    public string email { get; set; }

    public string oldpassword { get; set; }

    public string newpassword { get; set; }

    public string image { get; set; }

    public string password { get; set; }

    public string access { get; set; }

    public string type { get; set; }

    //------SEARCH------//

    public string column { get; set; }

    public string keyword { get; set; }

    //------ORDERS------//

    public string ULId { get; set; }

    public string ponumber { get; set; }

    public string customernumber { get; set; }

    public string ulstatus { get; set; }

    public string ULPONumber { get; set; }

    public string ULCustomer { get; set; }

    public string ULFileName { get; set; }

    public string ULUser { get; set; }

    public string ULOrderDate { get; set; }

    public string ULDeliveryDate { get; set; }

    public string ULRemarks { get; set; }

    public string UIProduct { get; set; }

    public string UIQuantity { get; set; }

    public string UIPrice { get; set; }

    public string UIStatus { get; set; }

    public string ULAccount { get; set; }

    public string ULTotalPO { get; set; }

    public bool ReUpload { get; set; }

    public bool isMultiple { get; set; }

    public string changes { get; set; }

    public string dateFrom { get; set; }
    
    public string dateTo { get; set; }

    //------ERROR LOG------//

    public string errorResult { get; set; }

    public string customerNo { get; set; }

    public string poNo { get; set; }


    //-------savetosql--------//


    public string POnum { get; set; }

    public string Custnum { get; set; }

    public string SKU { get; set; }

    public string Qty { get; set; }

    public string OrderDate { get; set; }

    public string DeliveryDate { get; set; }

    public string remarks { get; set; }

    public string userID { get; set; }

    public string ProdCode { get; set; }

    public string FileName { get; set; }

    public string run { get; set; }

    public string Operation { get; set; }

    public string Status { get; set; }

    public string POprice { get; set; }

    //------Upload--------//


    public string fileName { get; set; }

    public string nameSrc { get; set; }

    public string number { get; set; }

    public string pon { get; set; }

    public string ind { get; set; }

    public string name { get; set; }

    public string element { get; set; }

    public string filename { get; set; }

    public string fileID { get; set; }

    public string fileLogo { get; set; }

    public string outlet { get; set; }

    public string rawID { get; set; }

    //-------type---------//


    public string sugacc { get; set; }

    public string sugacc2 { get; set; }

    //---------tester--------//


    public string dummy { get; set; }

    //---------admin-products--------//


    public string PCode { get; set; }

    public string PDesc { get; set; }


    public string pmid { get; set; }

    public string pmcode { get; set; }

    public string pmdesc { get; set; }

    public string pmbcode { get; set; }

    public decimal pmprice { get; set; }

    public string pmcategory { get; set; }

    public string pmstatus { get; set; }

    public string pparea { get; set; }

    public decimal ppprice { get; set; }


    public string acctype { get; set; }

    public string assignedcode { get; set; }

    public string product { get; set; }



    public string pacode { get; set; }

    public string paproduct { get; set; }

    public string paaccount { get; set; }

    //------ addtnl filemover ----//

    public string prefix { get; set; }

    public string uiproduct { get; set; }

    public decimal uiprice { get; set; }

    public string sku { get; set; }

    public string cnum { get; set; }
}

public class Response
{
    public Boolean success { get; set; }
    public object detail { get; set; }
    public object unmapdetail { get; set; }
    public object errortype { get; set; }
    public string[] filecontent { get; set; }
    public string payloadvalue { get; set; }
    public long execution { get; set; }
    public string notiftext { get; set; }
    public object reports { get; set; }
    public object transactionDetail { get; set; }
}
public class Transaction
{
    public string type { get; set; }
    public string activity { get; set; }
    public string value { get; set; }
    public string remarks { get; set; }
    public string ponumber { get; set; }
    public string customernumber { get; set; }
    public DateTime date { get; set; }
    public string user { get; set; }
    public string payloadvalue { get; set; }
    public string changes { get; set; }
    public string operation { get; set; }
    public string response { get; set; }
}