using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Configuration;

namespace EFService
{
    public class DataHelper
    {
        string MessageHeader = "DataHelper";
        //constructor
        public string[] CreateSalesOrder(string companyId, ARSalesorderHeader objARSalesOrderHeader, ARSalesorderDetail[] objARSalesOrderDetails)
        {
            string[] result =new string[] {};
            string strSalesOrderNo = string.Empty, strCustomerType = string.Empty;
            bool bCheckValidate = true;
            Dictionary<string, string> dicGetValues = new Dictionary<string, string>();
            List<ARSalesDetail> lstARSalesDetail = new List<ARSalesDetail>();
            DataTable dtGSTCodeTaxValues = new DataTable();
            string strGSTRegistered = string.Empty, strGSTExclusive = string.Empty, strTaxCode = string.Empty, strARSalesOrderDetailRefNo = string.Empty;
            decimal dPercentage = 0, dSalesPrice = 0, dQty = 0, dNetTotal = 0, dTaxTotal = 0, dGrossTotal = 0;

            #region Decalre DataTable

            DataTable dtARSID = new DataTable();
            dtARSID.Clear();
            dtARSID.Columns.Add("SalesOrderDetailRefNo", typeof(string));
            dtARSID.Columns.Add("SalesLineNo", typeof(string));
            dtARSID.Columns.Add("AccountCode", typeof(string));
            dtARSID.Columns.Add("InventoryCode", typeof(string));
            dtARSID.Columns.Add("Description", typeof(string));
            dtARSID.Columns.Add("CodeUnitMeasure", typeof(string));
            dtARSID.Columns.Add("Quantity", typeof(string));
            dtARSID.Columns.Add("RequestQuantity", typeof(string));
            dtARSID.Columns.Add("ReceivedQuantity", typeof(string));
            dtARSID.Columns.Add("UnitPrice", typeof(string));
            dtARSID.Columns.Add("NetTotal", typeof(string));
            dtARSID.Columns.Add("TaxAmount", typeof(string));
            dtARSID.Columns.Add("GrossTotal", typeof(string));
            dtARSID.Columns.Add("TaxCode", typeof(string));
            dtARSID.Columns.Add("ForeignNetTotal", typeof(string));
            dtARSID.Columns.Add("ForeignTaxAmount", typeof(string));
            dtARSID.Columns.Add("ForeignGrossTotal", typeof(string));
            dtARSID.Columns.Add("Size", typeof(string));

            #endregion

            try
            {
                #region Validation Mandatory Fields

                if (string.IsNullOrEmpty(objARSalesOrderHeader.InvoiceDate))
                {
                    bCheckValidate = false;
                    result = new string[] { "Error", "Invoice Date Missing !" };
                   
                }
                else if (string.IsNullOrEmpty(objARSalesOrderHeader.CustomerID))
                {
                    bCheckValidate = false;
                    result = new string[] { "Error", "Customer ID Missing !" };
                }

                #endregion

                if (bCheckValidate)
                {
                    if (objARSalesOrderDetails.Length > 0 && objARSalesOrderDetails[0] != null)
                    {
                        dtGSTCodeTaxValues = PrivateMethods.CheckGSTExculisveOrInclusive(companyId);
                        strCustomerType = PrivateMethods.GetActiveCustomerType(companyId, objARSalesOrderHeader.CustomerID);

                        if (dtGSTCodeTaxValues != null && dtGSTCodeTaxValues.Rows.Count > 0)
                        {
                            strGSTRegistered = (!string.IsNullOrEmpty(Convert.ToString(dtGSTCodeTaxValues.Rows[0]["GSTRegistered"])) ? Convert.ToString(dtGSTCodeTaxValues.Rows[0]["GSTRegistered"]) : "N");
                            strGSTExclusive = (!string.IsNullOrEmpty(Convert.ToString(dtGSTCodeTaxValues.Rows[0]["GSTExclusive"])) ? Convert.ToString(dtGSTCodeTaxValues.Rows[0]["GSTExclusive"]) : "N");
                            strTaxCode = (!string.IsNullOrEmpty(Convert.ToString(dtGSTCodeTaxValues.Rows[0]["Code"])) ? Convert.ToString(dtGSTCodeTaxValues.Rows[0]["Code"]) : "SR");
                            dPercentage = (!string.IsNullOrEmpty(Convert.ToString(dtGSTCodeTaxValues.Rows[0]["Percentage"])) ? Convert.ToDecimal(dtGSTCodeTaxValues.Rows[0]["Percentage"].ToString()) : 7);
                        }
                        else
                        {
                            strTaxCode = "ZR";
                        }
                        for (int i = 0; i < objARSalesOrderDetails.Length; i++)
                        {
                            #region Validation Mandatory Fields - > Sales Order Details

                            if (string.IsNullOrEmpty(objARSalesOrderDetails[i].InventoryCode))
                            {
                                bCheckValidate = false;
                                result = new string[] { "Error", "SOD InventoryCode Missing !" };
                            }
                            else if (string.IsNullOrEmpty(objARSalesOrderDetails[i].Description))
                            {
                                bCheckValidate = false;
                                result = new string[] { "Error", "SOD Description Missing !" };

                            }
                            else if (string.IsNullOrEmpty(objARSalesOrderDetails[i].Quantity))
                            {
                                bCheckValidate = false;
                                result = new string[] { "Error", "SOD Quantity Missing !" };
                            }
                            else if (string.IsNullOrEmpty(objARSalesOrderDetails[i].Size ))
                            {
                                bCheckValidate = false;
                                result = new string[] { "Error", "SOD Size Missing !" };
                            }

                            #endregion

                            if (bCheckValidate)
                            {
                                #region NetTotal - TaxTotal - GrossTotal --> Calculations

                                DataTable dtGetPriceAndInvTitle = new DataTable();
                                string strInvTitle = string.Empty, strCodeUnitMeasure = string.Empty;
                                dSalesPrice = 0; dQty = 0; dNetTotal = 0; dTaxTotal = 0; dGrossTotal = 0;

                                strARSalesOrderDetailRefNo = (!string.IsNullOrEmpty(objARSalesOrderDetails[i].ARSalesOrderDetailRefNo) ? Convert.ToString(objARSalesOrderDetails[i].ARSalesOrderDetailRefNo) : "0");
                                if (strARSalesOrderDetailRefNo != "0")
                                {


                                    dtGetPriceAndInvTitle = PrivateMethods.GetSalesPriceAndInvTitleForInventoryCodeAcc(companyId, Convert.ToInt64(strARSalesOrderDetailRefNo), objARSalesOrderDetails[i].InventoryCode);
                                    if (dtGetPriceAndInvTitle != null && dtGetPriceAndInvTitle.Rows.Count > 0)
                                    {
                                        dSalesPrice = (!string.IsNullOrEmpty(Convert.ToString(dtGetPriceAndInvTitle.Rows[0]["UnitPrice"])) ? Convert.ToDecimal(dtGetPriceAndInvTitle.Rows[0]["UnitPrice"].ToString()) : 0);
                                        strInvTitle = (!string.IsNullOrEmpty(Convert.ToString(dtGetPriceAndInvTitle.Rows[0]["InventoryTitle"])) ? Convert.ToString(dtGetPriceAndInvTitle.Rows[0]["InventoryTitle"]) : string.Empty);
                                        strCodeUnitMeasure = (!string.IsNullOrEmpty(Convert.ToString(dtGetPriceAndInvTitle.Rows[0]["CodeUnitMeasure"])) ? Convert.ToString(dtGetPriceAndInvTitle.Rows[0]["CodeUnitMeasure"]) : string.Empty);
                                    }
                                    dQty = (!string.IsNullOrEmpty(objARSalesOrderDetails[i].ReceivedQuantity) ? Convert.ToDecimal(objARSalesOrderDetails[i].ReceivedQuantity) : 0);
                                }
                                else if (strARSalesOrderDetailRefNo.Equals("0"))
                                {
                                    dtGetPriceAndInvTitle = PrivateMethods.GetSalesPriceAndInvTitleForInventoryCode(companyId, objARSalesOrderDetails[i].InventoryCode, strCustomerType);
                                    if (dtGetPriceAndInvTitle != null && dtGetPriceAndInvTitle.Rows.Count > 0)
                                    {
                                        dSalesPrice = (!string.IsNullOrEmpty(Convert.ToString(dtGetPriceAndInvTitle.Rows[0]["SalesPrice"])) ? Convert.ToDecimal(dtGetPriceAndInvTitle.Rows[0]["SalesPrice"].ToString()) : 0);
                                        strInvTitle = (!string.IsNullOrEmpty(Convert.ToString(dtGetPriceAndInvTitle.Rows[0]["InventoryTitle"])) ? Convert.ToString(dtGetPriceAndInvTitle.Rows[0]["InventoryTitle"]) : string.Empty);
                                        strCodeUnitMeasure = (!string.IsNullOrEmpty(Convert.ToString(dtGetPriceAndInvTitle.Rows[0]["CodeUnitMeasure"])) ? Convert.ToString(dtGetPriceAndInvTitle.Rows[0]["CodeUnitMeasure"]) : string.Empty);
                                    }
                                    dQty = (!string.IsNullOrEmpty(objARSalesOrderDetails[i].Quantity) ? Convert.ToDecimal(objARSalesOrderDetails[i].Quantity) : 0);
                                }

                                if (!string.IsNullOrEmpty(strGSTRegistered))
                                {
                                    if (strGSTRegistered.Equals("Y"))
                                    {
                                        if (strGSTExclusive.Equals("Y"))
                                        {
                                            dNetTotal = ((dQty) * (dSalesPrice));
                                            dTaxTotal = (dNetTotal * dPercentage / 100);
                                            dGrossTotal = (dNetTotal + dTaxTotal);
                                        }
                                        else if (strGSTExclusive.Equals("N"))
                                        {
                                            dGrossTotal = ((dQty) * (dSalesPrice));
                                            dNetTotal = (dGrossTotal / ((100 + dPercentage) / 100));
                                            dTaxTotal = (dGrossTotal - dNetTotal);
                                        }
                                    }
                                }
                                else
                                {
                                    dNetTotal = ((dQty) * (dSalesPrice));
                                    dTaxTotal = 0;
                                    dGrossTotal = (dNetTotal + dTaxTotal);
                                }

                                #endregion
                                dtARSID.Rows.Add(new object[]
                                   {
                                    strARSalesOrderDetailRefNo,
                                    i+1,(!string.IsNullOrEmpty(objARSalesOrderDetails[i].AccountCode) ? objARSalesOrderDetails[i].AccountCode : "Sales"),
                                    objARSalesOrderDetails[i].InventoryCode, strInvTitle, strCodeUnitMeasure, //objARSalesOrderDetails[i].Description
                                    dQty, dQty, (!string.IsNullOrEmpty(objARSalesOrderDetails[i].ReceivedQuantity) ? objARSalesOrderDetails[i].ReceivedQuantity : "0"),
                                    dSalesPrice, dNetTotal,dTaxTotal, dGrossTotal,strTaxCode,
                                    dNetTotal,dTaxTotal, dGrossTotal,objARSalesOrderDetails[i].Size                            
                                    //Math.Round(dSalesPrice,4), Math.Round(dNetTotal,2),Math.Round(dTaxTotal,2), Math.Round(dGrossTotal,2),strTaxCode,
                                    //Math.Round(dNetTotal,2),Math.Round(dTaxTotal,2), Math.Round(dGrossTotal,2)                                  
                                   });
                            }
                        }

                        String companyDbString = PrivateMethods.GetCompanyDbString(companyId, "Accounts");
                        if (!string.IsNullOrEmpty(companyDbString))
                        {
                            using (SqlConnection con = new SqlConnection(companyDbString))
                            {
                                try
                                {
                                    using (SqlCommand cmd = new SqlCommand("uspFAARSalesOrderSave8GEMS", con))
                                    {
                                        con.Open();
                                        cmd.CommandType = CommandType.StoredProcedure;


                                        cmd.Parameters.Add("@InvoiceNo", SqlDbType.NVarChar, 16).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.InvoiceNo) ? objARSalesOrderHeader.InvoiceNo : "0");
                                        cmd.Parameters.Add("@InvoiceDate", SqlDbType.DateTime).Value = Convert.ToDateTime(objARSalesOrderHeader.InvoiceDate);    //Convert.ToDateTime(DateTime.Now);  //(!string.IsNullOrEmpty(objARSalesOrderHeader.InvoiceDate) ? DateTime.ParseExact(objARSalesOrderHeader.InvoiceDate, "dd-MM-yyyy", CultureInfo.InvariantCulture) : System.DateTime.Now.Date);
                                        cmd.Parameters.Add("@CustomerID", SqlDbType.NVarChar, 20).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.CustomerID) ? objARSalesOrderHeader.CustomerID : string.Empty);
                                        cmd.Parameters.Add("@CodeSalesType", SqlDbType.NVarChar, 20).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.CodeSalesType) ? objARSalesOrderHeader.CodeSalesType : "Credit Sales");
                                        cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar, 800).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.Remarks) ? objARSalesOrderHeader.Remarks : string.Empty);
                                        cmd.Parameters.Add("@OurRefNo", SqlDbType.NVarChar, 20).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.OurRefNo) ? objARSalesOrderHeader.OurRefNo : string.Empty);
                                        cmd.Parameters.Add("@YourRefNo", SqlDbType.NVarChar, 20).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.YourRefNo) ? objARSalesOrderHeader.YourRefNo : string.Empty);
                                        cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 9).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.CreatedUserID) ? (objARSalesOrderHeader.CreatedUserID) : "System");
                                        cmd.Parameters.Add("@CodeLocation", SqlDbType.NVarChar, 20).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.CodeLocation) ? objARSalesOrderHeader.CodeLocation : "1");  // Default Code Location Passed "1"                                                      
                                        cmd.Parameters.Add("@CurrencyCode", SqlDbType.NVarChar, 10).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.CurrencyCode) ? (objARSalesOrderHeader.CurrencyCode) : "SGD");
                                        cmd.Parameters.Add("@ExchangeRate", SqlDbType.Decimal).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.ExchangeRate) ? objARSalesOrderHeader.ExchangeRate : "1.00");
                                        cmd.Parameters.Add("@ProjectName", SqlDbType.NVarChar, 150).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.ProjectName) ? objARSalesOrderHeader.ProjectName : "Select");
                                        cmd.Parameters.Add("@InventoryType", SqlDbType.NChar, 1).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.InventoryType) ? objARSalesOrderHeader.InventoryType : "Y");
                                        cmd.Parameters.Add("@DeliveryDate", SqlDbType.NChar, 20).Value = Convert.ToDateTime(objARSalesOrderHeader.DeliveryDate);
                                        cmd.Parameters.Add("@AMorPM", SqlDbType.NChar, 2).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.DeliveryPeriod ) ? objARSalesOrderHeader.DeliveryPeriod : "AM");
                                        cmd.Parameters.Add("@RemainderFlag", SqlDbType.NChar, 1).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.RemainderFlag ) ? objARSalesOrderHeader.RemainderFlag : "0" );
                                        cmd.Parameters.Add("@AddressID", SqlDbType.NChar, 10).Value = (!string.IsNullOrEmpty(objARSalesOrderHeader.DeliveryAddressID) ? objARSalesOrderHeader.DeliveryAddressID : "0");
                                        if (objARSalesOrderHeader.DeliveryPeriod == "AM")
                                        {
                                            cmd.Parameters.Add("@FromTime", SqlDbType.NChar, 10).Value = "12:00";
                                            cmd.Parameters.Add("@ToTime", SqlDbType.NChar, 10).Value = "11:59";
                                        }
                                        else
                                        {
                                            cmd.Parameters.Add("@FromTime", SqlDbType.NChar, 10).Value = "11:49";
                                            cmd.Parameters.Add("@ToTime", SqlDbType.NChar, 10).Value = "12:00";
                                        }
                                        cmd.Parameters.Add("@GSTExclIncl", SqlDbType.NVarChar, 50).Value = (strGSTExclusive);
                                        cmd.Parameters.AddWithValue("@UserDefineTable", dtARSID); // passing Datatable
                                        cmd.Parameters.Add("@SalesOrderNo", SqlDbType.NVarChar, 160).Direction = ParameterDirection.Output;

                                        cmd.ExecuteNonQuery();
                                        strSalesOrderNo = (string)cmd.Parameters["@SalesOrderNo"].Value;
                                        if (!string.IsNullOrEmpty(strSalesOrderNo))
                                        { result = new string[] { "Success", strSalesOrderNo }; }
                                        else
                                        { result = new string[] { "Error03", "Insert Operation Failed" }; }
                                    }
                                }
                                finally
                                {
                                    if (con.State == ConnectionState.Open)
                                        con.Close();
                                }
                            }
                        }
                        else
                        {
                            result = new string[] { "Connection String Is Null." };

                        }
                    }
                    else
                        result = new string[] { "Sales Invoice Detail Value Missing !" };
                }
            }
            catch (Exception exec)
            {
                result = new string[] { "Error05", exec.ToString() };
            }
            return result;
        }
        public List<ARSalesOrder> GetSalesOrderDetails(string companyId, string strInvoiceNo)
        {
            DataTable dtValues = new DataTable();
            List<ARSalesOrder> objARSalesOrder = new List<ARSalesOrder>();
            try
            {
                if (!string.IsNullOrEmpty(strInvoiceNo))
                {
                    String companyDbString = PrivateMethods.GetCompanyDbString(companyId, "Accounts");
                    if (!string.IsNullOrEmpty(companyDbString))
                    {
                        using (SqlConnection con = new SqlConnection(companyDbString))
                        {
                            try
                            {
                                using (SqlCommand cmd = new SqlCommand("uspGetSalesOrderDetailsSQMS", con))
                                {
                                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                    {
                                        con.Open();
                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.Add("@InvoiceNo", SqlDbType.NVarChar, 16).Value = strInvoiceNo;
                                        sda.Fill(dtValues);
                                        try
                                        {
                                            if (dtValues != null && dtValues.Rows.Count > 0)
                                            {
                                                foreach (DataRow dr in dtValues.Rows)
                                                {
                                                    ARSalesOrder aRSalesOrder = new ARSalesOrder();
                                                    aRSalesOrder.InvoiceNo = dr["InvoiceNo"].ToString();
                                                    aRSalesOrder.InvoiceDate = dr["InvoiceDate"].ToString();
                                                    aRSalesOrder.CustomerID = dr["CustomerID"].ToString();
                                                    aRSalesOrder.ARSalesOrderDetailRefNo = dr["ARSalesOrderDetailRefNo"].ToString();
                                                    aRSalesOrder.InventoryCode = dr["InventoryCode"].ToString();
                                                    aRSalesOrder.Description = dr["Description"].ToString();
                                                    aRSalesOrder.Quantity = dr["Quantity"].ToString();
                                                    aRSalesOrder.RequestQuantity = dr["RequestQuantity"].ToString();
                                                    aRSalesOrder.ReceivedQuantity = dr["ReceivedQuantity"].ToString();
                                                    aRSalesOrder.UnitPrice = dr["UnitPrice"].ToString();
                                                    aRSalesOrder.NetTotal = dr["NetTotal"].ToString();
                                                    aRSalesOrder.TaxAmount = dr["TaxAmount"].ToString();
                                                    aRSalesOrder.GrossTotal = dr["GrossTotal"].ToString();
                                                    aRSalesOrder.DeliveryDate  = dr["DeliveryDate"].ToString();
                                                    aRSalesOrder.DeliveryPeriod  = dr["DeliveryPeriod"].ToString();
                                                    aRSalesOrder.RemainderFlag  = dr["RemainderFlag"].ToString();
                                                    aRSalesOrder.Size = dr["Size"].ToString();
                                                    aRSalesOrder.DeliveryAddressID = dr["DeliveryAddressID"].ToString();
                                                    objARSalesOrder.Add(aRSalesOrder);
                                                }
                                            }
                                            //dr = null;
                                            if (objARSalesOrder.Count == 0)
                                            {
                                                ARSalesOrder arSalesOrder = new ARSalesOrder();
                                                objARSalesOrder.Add(arSalesOrder);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            ARSalesOrder arSalesOrder = new ARSalesOrder();
                                            objARSalesOrder.Add(arSalesOrder);
                                            arSalesOrder.ErrorMsg = ex.Message.ToString();
                                            return objARSalesOrder;
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
                }
            }
            catch (Exception ex)
            {
                ARSalesOrder arSalesOrder = new ARSalesOrder();
                objARSalesOrder.Add(arSalesOrder);
                arSalesOrder.ErrorMsg = ex.Message.ToString();
                return objARSalesOrder;
            }
            return objARSalesOrder;
        }
        public List<ARSalesOrder> GetSalesOrderDetailsByCustomerId(String companyId, string strCustomerId)
        {
            DataTable dtValues = new DataTable();
            List<ARSalesOrder> objARSalesOrder = new List<ARSalesOrder>();
            if (!string.IsNullOrEmpty(strCustomerId))
            {
                String companyDbString = PrivateMethods.GetCompanyDbString(companyId, "Accounts");
                if (!string.IsNullOrEmpty(companyDbString))
                {
                    using (SqlConnection con = new SqlConnection(companyDbString))
                    {
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand("uspGetSalesOrderDetailsSQMSByCustomerId", con))
                            {
                                using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                                {
                                    con.Open();
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Add("@CustomerId", SqlDbType.NVarChar, 16).Value = strCustomerId;
                                    sda.Fill(dtValues);

                                    if (dtValues != null && dtValues.Rows.Count > 0)
                                    {
                                        foreach (DataRow dr in dtValues.Rows)
                                        {
                                            ARSalesOrder aRSalesOrder = new ARSalesOrder();
                                            aRSalesOrder.InvoiceNo = dr["InvoiceNo"].ToString();
                                            aRSalesOrder.InvoiceDate = dr["InvoiceDate"].ToString();
                                            objARSalesOrder.Add(aRSalesOrder);
                                        }
                                    }
                                    if (objARSalesOrder.Count == 0)
                                    {
                                        ARSalesOrder arSalesOrder = new ARSalesOrder();
                                        objARSalesOrder.Add(arSalesOrder);
                                    }
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            ARSalesOrder arSalesOrder = new ARSalesOrder();
                            arSalesOrder.ErrorMsg = ex.Message.ToString(); 
                            objARSalesOrder.Add(arSalesOrder);

                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                                con.Close();
                        }
                    }
                }
            }
            return objARSalesOrder;
        }
        //public string[] authenticateUser(string userID, string password, string companyID)
        //{
        //    password = PrivateMethods.Encrypt(password);
        //    //List<ARDeliveryAddress> lstDeliveryAddress = new List<ARDeliveryAddress>();
        //    string conString = string.Empty;
        //    string[] result = new string[] {"Error03","","Not getting any output", };
        //    using (SqlConnection con = new SqlConnection(Convert.ToString(ConfigurationManager.ConnectionStrings["conString"].ConnectionString)))
        //    { 
        //         ConnectionState state = con.State;
        //        try
        //    {
        //        using (SqlCommand sqlCommand = new SqlCommand("uspAuthenticateUser", con))
        //        {
                    
        //            con.Open();
        //            sqlCommand.CommandType = CommandType.StoredProcedure;
        //            sqlCommand.Parameters.Add("@UserID", SqlDbType.NVarChar, 16).Value = userID;
        //            sqlCommand.Parameters.Add("@Password", SqlDbType.NVarChar, 100).Value = password;
        //            sqlCommand.Parameters.Add("@CompanyID", SqlDbType.NVarChar, 20).Value = companyID;
        //            SqlDataReader reader = sqlCommand.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                result = new string[] { Convert.ToString(reader["Result"]), Convert.ToString(reader["MenuID"]), Convert.ToString(reader["CustomerID"]) };
                    
        //            }
        //            if (reader != null)
        //                ((IDisposable)reader).Dispose();
        //        }
            
            
        //    }
        //    catch (Exception ex)
        //    {
        //        { result = new string[] { "Error05", "",ex.Message  }; }
        //    }
        //    finally
        //    {
        //        state = con.State;
        //        if (state == ConnectionState.Open)
        //        {
        //            con.Close();
        //        }
        //    }
        //}
        //    return result;

            
        //}
        public List<ARDeliveryAddress>DHGetCustomerDeliveryAddress(string companyID, string customerCode)
        {
            List<ARDeliveryAddress> lstDeliveryAddress = new List<ARDeliveryAddress>();
            string conString = string.Empty;
            String companyDbString = PrivateMethods.GetCompanyDbString(companyID, "Accounts");
            SqlConnection sqlConnectionString = new SqlConnection(companyDbString);
            ConnectionState state = sqlConnectionString.State;
            try
            {
                if (state == ConnectionState.Open)
                {
                    sqlConnectionString.Close();
                }

                using (SqlCommand sqlCommand = new SqlCommand("spGetArCustomerDeliveryAddress", sqlConnectionString))
                {
                    sqlConnectionString.Open();
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@CustomerCode", SqlDbType.NVarChar, 20).Value = customerCode;
                    SqlDataReader reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        ARDeliveryAddress objAddress = new ARDeliveryAddress();
                        objAddress.AddressID = Convert.ToString(reader["SNo"]);
                        objAddress.CustomerCode = Convert.ToString(reader["CustomerID"]);
                        objAddress.CustomerAddress  = Convert.ToString(reader["CustomerName"]);
                        lstDeliveryAddress.Add(objAddress);
                    }
                    if (reader != null)
                        ((IDisposable)reader).Dispose();
                }
            }
            catch (Exception ex)
            {
                ARDeliveryAddress objAddress = new ARDeliveryAddress();
                objAddress.CustomerAddress  = ex.Message.ToString();
                lstDeliveryAddress.Add(objAddress);
                return lstDeliveryAddress;
            }
            finally
            {
                state = sqlConnectionString.State;
                if (state == ConnectionState.Open)
                {
                    sqlConnectionString.Close();
                }
            }
            return lstDeliveryAddress;
        }
       
        public string[] AddToCustomerDeliveryAddress( string companyID, ARDeliveryAddressAll objCustomerAddressInfo)
        {
            String strReturnMsgSP = string.Empty;
            string[] result = new string[] { "Error", "Not getting any output", };
        
            string conString = string.Empty;
            String companyDbString = PrivateMethods.GetCompanyDbString(companyID, "Accounts");
            SqlConnection sqlConnectionString = new SqlConnection(companyDbString);
            ConnectionState state = sqlConnectionString.State;

            try
            {
                if (state == ConnectionState.Open)
                {
                    sqlConnectionString.Close();
                }

                using (SqlCommand cmd = new SqlCommand("spFACustomerDeliveryAddressInsert8gemsEcom", sqlConnectionString))
                {
                            sqlConnectionString.Open();
                            cmd.CommandType = CommandType.StoredProcedure;
                   

                    cmd.Parameters.Add("@CustomerID", SqlDbType.NVarChar, 50).Value = objCustomerAddressInfo.CustomerID;
                    cmd.Parameters.Add("@CustomerName", SqlDbType.NVarChar, 100).Value = objCustomerAddressInfo.CustomerName;
                    cmd.Parameters.Add("@BlockNo", SqlDbType.NVarChar, 5).Value = (!string.IsNullOrEmpty(objCustomerAddressInfo.BlockNo)?objCustomerAddressInfo.BlockNo:string.Empty);
                    cmd.Parameters.Add("@FloorNo", SqlDbType.NVarChar, 3).Value = (!string.IsNullOrEmpty(objCustomerAddressInfo.FloorNo) ? objCustomerAddressInfo.FloorNo : string.Empty);
                    cmd.Parameters.Add("@UnitNo", SqlDbType.NVarChar, 5).Value = (!string.IsNullOrEmpty(objCustomerAddressInfo.UnitNo) ? objCustomerAddressInfo.UnitNo : string.Empty);
                    cmd.Parameters.Add("@StreetName", SqlDbType.NVarChar, 200).Value = (!string.IsNullOrEmpty(objCustomerAddressInfo.StreetName) ? objCustomerAddressInfo.StreetName : string.Empty);
                    cmd.Parameters.Add("@BuildingName", SqlDbType.NVarChar, 100).Value = (!string.IsNullOrEmpty(objCustomerAddressInfo.BuildingName) ? objCustomerAddressInfo.BuildingName : string.Empty);
                    cmd.Parameters.Add("@PostalCode", SqlDbType.NVarChar, 6).Value = (!string.IsNullOrEmpty(objCustomerAddressInfo.PostalCode) ? objCustomerAddressInfo.PostalCode : string.Empty);
                    cmd.Parameters.Add("@CodeCountry", SqlDbType.NVarChar, 20).Value = (!string.IsNullOrEmpty(objCustomerAddressInfo.CodeCountry) ? objCustomerAddressInfo.CodeCountry : string.Empty);
                    
                    cmd.Parameters.Add("@CreatedUserID", SqlDbType.NVarChar,9).Value = (!string.IsNullOrEmpty(objCustomerAddressInfo.CreatedUserID) ? (objCustomerAddressInfo.CreatedUserID) : "System");
                    cmd.Parameters.Add("@CreatedDateTime", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@ContactPerson", SqlDbType.VarChar,150).Value = (!string.IsNullOrEmpty(objCustomerAddressInfo.ContactPerson) ? objCustomerAddressInfo.ContactPerson : string.Empty);
                    cmd.Parameters.Add("@ContactNo", SqlDbType.VarChar, 20).Value = (!string.IsNullOrEmpty(objCustomerAddressInfo.ContactNo) ? objCustomerAddressInfo.ContactNo : string.Empty);
                    cmd.ExecuteNonQuery();
                            strReturnMsgSP = "Success";
                            result = new string[] { "Success", "Successfully updated", };
                }
                    }
                catch(Exception exec)
                {
                result = new string[] { "Error05", exec.ToString() };
             }
            finally
            {
                state = sqlConnectionString.State;
                if (state == ConnectionState.Open)
                {
                    sqlConnectionString.Close();
                }

            }
            return result;
        }

        public string[] authenticateUser(string userID, string password, string companyID)
        {
            password = EncryptDecrypt.Encrypt(password);
            string[] result = new string[] { "Error", MessageHeader, "Not getting any output", };
              try
              {
                        List<SqlParameter> sp = new List<SqlParameter>()
                        {
                        new SqlParameter() {ParameterName = "@UserID", SqlDbType = SqlDbType.NVarChar, Value=userID},
                        new SqlParameter() {ParameterName = "@Password", SqlDbType = SqlDbType.NVarChar, Value = password},
                        new SqlParameter() {ParameterName = "@CompanyID", SqlDbType = SqlDbType.NVarChar, Value = companyID},
                        };
                        SQLExecution objExecution = new SQLExecution(companyID, "Master");
                         result = objExecution.RunSP_ReturnStringArray("uspAuthenticateUser", sp);
                   }
                catch (Exception ex)
                {
                    { result = new string[] { "Error05", MessageHeader, ex.Message }; }
                }
            return result;
        }

        public string[] authenticateUser8gems(string userID, string password, string companyID)
        {
            password = EncryptDecrypt.Encrypt(password);
            string[] result = new string[] { "Error", MessageHeader, "Not getting any output", };
            try
            {
                List<SqlParameter> sp = new List<SqlParameter>()
                        {
                        new SqlParameter() {ParameterName = "@UserID", SqlDbType = SqlDbType.NVarChar, Value=userID},
                        new SqlParameter() {ParameterName = "@Password", SqlDbType = SqlDbType.NVarChar, Value = password},
                        new SqlParameter() {ParameterName = "@CompanyID", SqlDbType = SqlDbType.NVarChar, Value = companyID},
                        };
                SQLExecution objExecution = new SQLExecution(companyID, "Master");
                result = objExecution.RunSP_ReturnStringArray("uspAuthenticateUser8gems", sp);
            }
            catch (Exception ex)
            {
                { result = new string[] { "Error05", MessageHeader, ex.Message }; }
            }
            return result;
        }

        public string[] DHForgetPassword(string userID, string mobileNo)
        {
            string[] result = new string[] { "Error",  "Not getting any output", };
                 try
                {
                    List<SqlParameter> sp = new List<SqlParameter>()
                        {
                        new SqlParameter() {ParameterName = "@userID", SqlDbType = SqlDbType.NVarChar, Value=userID },
                        new SqlParameter() {ParameterName = "@mobileNo", SqlDbType = SqlDbType.NVarChar, Value = mobileNo  }
                       
                        };
                    SQLExecution objExecution = new SQLExecution("companyID", "Master");
                    DataTable dt = objExecution.RunSP_ReturnDT("uspForgetPassword8GEMS", sp);

                    if (dt.Rows.Count > 0)
                    {
                        result = new string[] { "Success", "Your password has been send registered mobileno", };
                    }
                    else
                    {
                        result = new string[] { "Failed", "No record found", };
                    }
                }
                catch (Exception ex)
                {
                    { result = new string[] { "Error05",  ex.Message }; }
                }
                return result;
         
        }
        public AccountDetails  DHGetAccountDetails(string userID, string customerCode, string companyID)
        {
            AccountDetails objAccountDetails = new AccountDetails();
            try
            {
                List<SqlParameter> sp = new List<SqlParameter>()
                        {
                          new SqlParameter() {ParameterName = "@userID", SqlDbType = SqlDbType.NVarChar, Value=userID},
                        new SqlParameter() {ParameterName = "@customerCode", SqlDbType = SqlDbType.NVarChar, Value = customerCode },
                        new SqlParameter() {ParameterName = "@companyID", SqlDbType = SqlDbType.NVarChar, Value = companyID},
                        };
                SQLExecution objExecution = new SQLExecution(companyID, "Master");
                DataTable dt = objExecution.RunSP_ReturnDT("uspGetUserDetailsFor8GEMS", sp);
                if (dt != null)
                 {
                    // string filePath = "C:\\inetpub\\wwwroot\\efservice.excelpayit.com\\UserProfilePic\\ERPDEMO\\"+userID+"\\" + userID.ToUpper() + ".jpg";
                     foreach (DataRow dr in dt.Rows)
                     {
                        
                         objAccountDetails.UserName = dr["UserName"].ToString();
                         objAccountDetails.UserID = userID;
                         objAccountDetails.EmailID = dr["EmailID"].ToString();
                         objAccountDetails.MobileNo = dr["MobileNo"].ToString();
                         objAccountDetails.URL = dr["URL"].ToString();
                         objAccountDetails.Password = EncryptDecrypt.Decrypt(dr["Password"].ToString());
                         //if (File.Exists(filePath))
                         //    objAccountDetails.URL = "http:\\efservice.excelpayit.com\\UserProfilePic\\ERPDEMO\\" + userID + "\\" + userID.ToUpper() + ".jpg";
                         //else
                         //    objAccountDetails.URL = "http:\\efservice.excelpayit.com\\UserProfilePic\\ERPDEMO\\" + userID + "\\";
                         objAccountDetails.Message = "Success";

                     }
                 }
                 else
                 {
                     objAccountDetails.Message = "Failed";
                 }
            }
            catch (Exception ex)
            {
                objAccountDetails.Message = "Error6001 " + ex.Message.ToString();

            }
            return objAccountDetails;

        }
        public List<NotificationDetails>  DHGetNotificationDetails(int notificationID, string customerCode, string companyID)
        {
            List<NotificationDetails> lstObjNotificationDetails = new List<NotificationDetails>();
            try
            {
                List<SqlParameter> sp = new List<SqlParameter>()
                        {
                        new SqlParameter() {ParameterName = "@notificationID", SqlDbType = SqlDbType.Int , Value=notificationID},
                        new SqlParameter() {ParameterName = "@customerCode", SqlDbType = SqlDbType.NVarChar, Value = customerCode }
                        };
                SQLExecution objExecution = new SQLExecution(companyID, "Accounts");
                DataTable dt = objExecution.RunSP_ReturnDT("uspGetNotificationDetails", sp);
                if (dt != null)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        NotificationDetails objNotificationDetails = new NotificationDetails();
                        objNotificationDetails.CustomerCode = dr["CustomerCode"].ToString();
                        objNotificationDetails.NotificationText  = dr["NotificationText"].ToString();
                        objNotificationDetails.NotificationDetailsID  =Convert.ToInt32(dr["NotificationDetailsID"].ToString());
                        objNotificationDetails.NotificationID  = Convert.ToInt32(dr["NotificationID"].ToString());
                        lstObjNotificationDetails.Add(objNotificationDetails);
                    }
                }
               
            }
            catch (Exception ex)
            {
                NotificationDetails objNotificationDetails = new NotificationDetails();

                objNotificationDetails.NotificationText = "Error 801 " + ex.Message.ToString();
                objNotificationDetails.NotificationDetailsID = 0;
                objNotificationDetails.NotificationID =0;
                lstObjNotificationDetails.Add(objNotificationDetails);

            }
            return lstObjNotificationDetails;

        }
        public List<NotificationHeader> DHGetNotificationHeader(string companyID)
        {
            List<NotificationHeader> lstObjNotificationHeader = new List<NotificationHeader>();
            try
            {
                SQLExecution objExecution = new SQLExecution(companyID, "Accounts");
                DataTable dt = objExecution.RunSP_ReturnDT ("uspGetNotificationHeader");
                if (dt != null)
                {
                  
                    foreach (DataRow dr in dt.Rows)
                    {
                        NotificationHeader objNotificationHeader = new NotificationHeader();
                        objNotificationHeader.NotificationName = dr["NotificationName"].ToString();
                        objNotificationHeader.NotificationID = Convert.ToInt32(dr["NotificationID"].ToString());
                        lstObjNotificationHeader.Add(objNotificationHeader);
                    }
                }

            }
            catch (Exception ex)
            {
                NotificationHeader objNotificationHeader = new NotificationHeader();
                objNotificationHeader.NotificationName = "Error 805 " + ex.Message.ToString();
                objNotificationHeader.NotificationID = 0;
                lstObjNotificationHeader.Add(objNotificationHeader);
                return lstObjNotificationHeader;
            }
            return lstObjNotificationHeader;

        }
        public string[] UpdateAccountDetails(string userID, string companyID,string userName,string mobileNo,string emailID,string password,string profilePicture)
        {
            string[] result = new string[] { "Error", "Not getting any output", };
            //password = EncryptDecrypt.Encrypt(password);
            try
            {
                //List<SqlParameter> sp = new List<SqlParameter>()
                //        {
                //        new SqlParameter() {ParameterName = "@userID", SqlDbType = SqlDbType.NVarChar, Value=userID},
                //        new SqlParameter() {ParameterName = "@userName", SqlDbType = SqlDbType.NVarChar, Value = userName  },
                //        new SqlParameter() {ParameterName = "@mobileNo", SqlDbType = SqlDbType.NVarChar, Value = mobileNo  },
                //         new SqlParameter() {ParameterName = "@emailID", SqlDbType = SqlDbType.NVarChar, Value = emailID  },
                //        new SqlParameter() {ParameterName = "@companyID", SqlDbType = SqlDbType.NVarChar, Value = companyID},
                //        };
                //SQLExecution objExecution = new SQLExecution(companyID, "Master");
                //result = objExecution.ExecuteNonQuery("uspUpdateUserDetails8Gems", sp);

                password = EncryptDecrypt.Encrypt(password);
                String companyDbString = PrivateMethods.GetCompanyDbString(companyID , "Master");
                using (SqlConnection con = new SqlConnection(companyDbString))
                {
                    string[] fileLocation = new string[] { };

                    if (!string.IsNullOrEmpty(profilePicture))
                    {

                        fileLocation = SaveSignature1(userID,companyID , profilePicture);
                    }

                    using (SqlCommand cmd = new SqlCommand("uspUpdateUserDetails8Gems", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@userID", SqlDbType.NVarChar).Value = userID ;
                        cmd.Parameters.Add("@userName", SqlDbType.NVarChar).Value = userName;
                        cmd.Parameters.Add("@mobileNo", SqlDbType.NVarChar).Value = mobileNo;
                        cmd.Parameters.Add("@emailID", SqlDbType.NVarChar).Value = emailID;
                        cmd.Parameters.Add("@companyID", SqlDbType.NVarChar).Value = companyID;
                        cmd.Parameters.Add("@url", SqlDbType.NVarChar).Value = fileLocation[1].ToString ();
                        cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = password;
                        cmd.ExecuteNonQuery();
                         result = new string[] { "Success", "Successfully updated", };
                    }
                }


             
            }
            catch (Exception ex)
            {
                { result = new string[] { "Error05", ex.Message }; }
            }
            return result;

        }

        public string[] UpdateFCMTokenid(string userID,string companyID, string tokenID)
        {
            string[] result = new string[] { "Error", "Not getting any output", };
            
            try
            {
                String companyDbString = PrivateMethods.GetCompanyDbString(companyID, "Master");
                using (SqlConnection con = new SqlConnection(companyDbString))
                {
                    
                    using (SqlCommand cmd = new SqlCommand("uspUpdateUserTokenId8Gems", con))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@userID", SqlDbType.NVarChar).Value = userID;
                        cmd.Parameters.Add("@companyID", SqlDbType.NVarChar).Value = companyID;
                        
                        cmd.Parameters.Add("@tokenid", SqlDbType.NVarChar).Value = tokenID;
                        cmd.ExecuteNonQuery();
                        result = new string[] { "Success", "Successfully updated", };
                    }
                }



            }
            catch (Exception ex)
            {
                { result = new string[] { "Error05", ex.Message }; }
            }
            return result;

        }


        public string[] SaveSignature1(string filename,string companyID, string base64string)
        {
            string[] result = new string[] { };
            string fileLocation = string.Empty;
            try
            {
                string filepath = WebConfigurationManager.AppSettings["ImgLocation"].ToString();
                string imgPath1 = filepath + "\\" + companyID + "\\" + filename + "\\" + filename + ".png";
                try
                {
                    if (File.Exists(imgPath1))
                    {
                        File.Delete(imgPath1);
                    }
                }
                catch (Exception ex)
                {
                }

                try
                {
                    string FilePath = filepath + "\\" + companyID + "\\" + filename;
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }
                }
                catch (Exception ex)
                {
                    // handle them here
                }

              //  fileLocation = "/Signature/TONGALK/Workorder/" + filename + ".png";
                fileLocation = "http://efservice.excelpayit.com/UserProfilePic/" + companyID + "/" + filename + "/" + filename + ".png";
                var decodedFileBytes = Convert.FromBase64String(base64string);
                File.WriteAllBytes(imgPath1, decodedFileBytes);

                //3
                string imgPath2 = filepath + "\\" + companyID + "\\" + filename + "\\" + filename + ".txt";
                try
                {
                    if (File.Exists(imgPath2))
                    {
                        File.Delete(imgPath2);
                    }
                }
                catch (Exception ex)
                {
                }
                if (!File.Exists(imgPath2))
                {
                    // Create a file to write to.
                    using (StreamWriter sw = File.CreateText(imgPath2))
                    {

                        sw.WriteLine(base64string);

                    }
                }
             
            }
            catch (Exception ex)
            {
                result = new string[] { "Error", ex.Message.ToString() };
                return result;
            }
            result = new string[] { "Success", fileLocation  };
            return result;
        }
        public string  SendSMS()
        {
            string result = string.Empty;
            try
            {
                SMSTwilio objTwilio = new SMSTwilio();
                objTwilio.SendSmsToCustomer();
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();
            }
            return result;
        }
    }
}