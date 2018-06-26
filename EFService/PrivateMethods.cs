using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EFService
{
    public static class PrivateMethods
    {
        #region PrivateMethods
        public static DataTable CheckGSTExculisveOrInclusive(String companyId)
        {
            CreateConnectionString objcon = new CreateConnectionString();
            DataTable dt = new DataTable();
            String companyDbString = GetCompanyDbString(companyId, "Accounts");
            if (!string.IsNullOrEmpty(companyDbString))
            {
                using (SqlConnection con = new SqlConnection(companyDbString))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("uspCheckGSTExculisveOrInclusiveSQMS", con))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                con.Open();
                                cmd.CommandType = CommandType.StoredProcedure;
                                sda.Fill(dt);
                            }
                        }
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                            con.Close();
                    }
                }
            }
            return dt;
        }
        public static string GetCompanyDbString(string companyId, string moduleName)
        {
            SQLExecution objExecution = new SQLExecution(companyId, moduleName);
            return objExecution.ConnectionString;
        }
        public static DataTable GetSalesPriceAndInvTitleForInventoryCodeAcc(String companyId, Int64 strARSalesOrderDetailRefNo, String strInventoryCode)
        {
            DataTable dtGetSalesPriceAndInvTitleForInventoryCodeAcc = new DataTable();
            String companyDbString = GetCompanyDbString(companyId, "Accounts");
            if (!string.IsNullOrEmpty(companyDbString))
            {
                using (SqlConnection con = new SqlConnection(companyDbString))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("uspGetSalesPriceForInventoryCodeAccSQMS", con))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                con.Open();
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add(new SqlParameter("@ARSalesOrderDetailRefNo", strARSalesOrderDetailRefNo));
                                cmd.Parameters.Add(new SqlParameter("@InventoryCode", strInventoryCode));
                                sda.Fill(dtGetSalesPriceAndInvTitleForInventoryCodeAcc);
                            }
                        }
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                            con.Close();
                    }
                }
            }
            return dtGetSalesPriceAndInvTitleForInventoryCodeAcc;
        }
        public static string GetActiveCustomerType(String companyId, String customerId)
        {
            DataTable dt = new DataTable();
            string strCustomerType = string.Empty;
            String companyDbString = GetCompanyDbString(companyId, "Accounts");
            if (!string.IsNullOrEmpty(companyDbString))
            {
                using (SqlConnection con = new SqlConnection(companyDbString))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("spCustomerTypeList", con))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                con.Open();
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@CustomerId", SqlDbType.NVarChar, 16).Value = customerId;
                                sda.Fill(dt);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    strCustomerType = (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["CustomerType"])) ? Convert.ToString(dt.Rows[0]["CustomerType"]) : string.Empty);
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                            con.Close();
                    }
                }
            }
            return strCustomerType;
        }
        public static DataTable GetSalesPriceAndInvTitleForInventoryCode(String companyId, String strInventoryCode, string customerType)
        {
            DataTable dtGetSalesPriceAndTitleForInventoryCode = new DataTable();
            String companyDbString = GetCompanyDbString(companyId, "INVENTORY");
            if (!string.IsNullOrEmpty(companyDbString))
            {
                using (SqlConnection con = new SqlConnection(companyDbString))
                {
                    try
                    {
                           using (SqlCommand cmd = new SqlCommand("uspGetSalesPriceForInventoryCode", con))
                        //using (SqlCommand cmd = new SqlCommand("uspGetSalesPriceFor8GEMS", con))
                        {
                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                con.Open();
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@InventoryCode", SqlDbType.NVarChar, 10).Value = strInventoryCode;
                                cmd.Parameters.Add("@CustomerType", SqlDbType.NVarChar, 10).Value = customerType;
                                sda.Fill(dtGetSalesPriceAndTitleForInventoryCode);
                            }
                        }
                    }
                    finally
                    {
                        if (con.State == ConnectionState.Open)
                            con.Close();
                    }
                }
            }
            return dtGetSalesPriceAndTitleForInventoryCode;
        }
        public static string Encrypt(string txt)
        {
            string SecretKey = "_$!9_?#_)";
            // any 8 charS
            byte[] key = {
	                };
            byte[] b4 = {
		                251,
		                255,
		                96,
		                111,
		                124,
		                180,
		                209,
		                255
	                };

            // any 8 numbers(255 is max)
            if (txt == null | txt.Trim() == string.Empty)
            {
                return null;
            }
            string encryptKey = SecretKey;
            try
            {
                //string Encoding;
                key = System.Text.Encoding.UTF8.GetBytes(encryptKey.ToCharArray(), 0, 8);
                //Dim des As SHA256CryptoServiceProvider = New SHA256CryptoServiceProvider()
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByte = System.Text.Encoding.UTF8.GetBytes(txt);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, b4), CryptoStreamMode.Write);
                cs.Write(inputByte, 0, inputByte.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }
        #endregion PrivateMethods
    }
}
