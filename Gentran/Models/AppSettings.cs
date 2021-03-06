﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Xml;

public class AppSettings
{
    public AppSettings()
    {
    }

    public string ConnectionString { get; set; }

    public string Encrypt(string clearText)
    {
        if (clearText == "" || clearText == " ") {
            return "";
        }
        try
        {
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Dispose();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        catch {
            clearText = "";
        }
        
        return clearText;
    }
    public string Decrypt(string cipherText)
    {
        if (cipherText == "" || cipherText == " "){
            return "";
        }
        try
        {
            string EncryptionKey = "abc123";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Dispose();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
        }
        catch {
            cipherText = "";
        }
        
        return cipherText;
    }

    public byte[] StringToBytes(string str)
    {
        byte[] bytes = new byte[str.Length * sizeof(char)];
        System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    public string BytesToString(byte[] bytes)
    {
        char[] chars = new char[bytes.Length / sizeof(char)];
        System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    public string toDate(string date) {

        if (date == "" || date == null) {
            return date;
        }
        char[] dSplit = date.ToCharArray();

        if (date.Length == 6)
        {
            date = date.Insert(2, "/").Insert(5, "/");
        }
        else if (date.Length == 5 && ((date.StartsWith("1") && (dSplit[1] == '1' || dSplit[1] == '2' || dSplit[1] == '0')))) {
            date = date.Insert(2, "/").Insert(4, "/");
        }
        else {
            date = date.Insert(1, "/").Insert(4, "/");
        }

        return date;
    }
    
    public string[] csv(byte[] fileByte)
    {
        var stream = new StreamReader(new MemoryStream(fileByte));
        string data = "";

        List<string> dataList = new List<string>();
        while ((data = stream.ReadLine()) != null ) {
            dataList.Add(data.ToString());
        }

        string[] readed = dataList.ToArray();
        return readed;
    }

    public List<Dictionary<object, object>> xml(string fileName) {
        Boolean success = false;
        string element = "";
        List<Dictionary<object, object>> rows = new List<Dictionary<object, object>>();
        Dictionary<object, object> row;

        try
        {
            XmlTextReader reader = new XmlTextReader(new StringReader(fileName));
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        element = reader.Name;
                        break;
                    case XmlNodeType.Text:
                        row = new Dictionary<object, object>();
                        row.Add(element, reader.Value);
                        rows.Add(row);
                        break;
                }
            }
            reader.Close();

            success = true;
        }
        catch (Exception ex)
        {
            success = false;
        }

        return rows;
    }
}