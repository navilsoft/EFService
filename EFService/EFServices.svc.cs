using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace EFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class EFServices : IEightGems
    {
        //public string[] AuthenticateUser1(string userID, string password, string companyID)
        //{
        //    string[] result = new string[] { };
        //    try
        //    {
        //        DataHelper objHelper = new DataHelper();
        //        result = objHelper.authenticateUser1(userID, password, companyID);
        //    }
        //    catch (Exception ex)
        //    {
        //        result = new string[] { "Error01", "", ex.ToString() };

        //    }
        //    return result;
        //}
        public string[] AuthenticateUser(string userID,string password,string companyID)
        {
            string[] result = new string[] { };
            try
            {
                DataHelper objHelper = new DataHelper();
              result=  objHelper.authenticateUser(userID, password, companyID);
            }
            catch(Exception ex)
            {
                result = new string[] { "Error01","", ex.ToString() };

            }
            return result;
         }
        public string[] CreateARSalesOrder(String companyID, ARSalesorderHeader objARSalesOrderHeader, ARSalesorderDetail[] objARSalesOrderDetails)
        {
            string[] result = new string[] { };
            try
            {
                if (string.IsNullOrEmpty(companyID))
                    result = new string[] {"Error201", "CompanyID Required"  };
                else if (objARSalesOrderHeader==null)
                    result = new string[] {"Error202", "SalesHeader should not be null"  };
                else if (objARSalesOrderDetails == null)
                    result = new string[] { "Error203", "SalesHeader should not be null" };
              
                DataHelper objHelper = new DataHelper();
                result = objHelper.CreateSalesOrder(companyID, objARSalesOrderHeader, objARSalesOrderDetails);
            }
            catch (Exception ex)
            {
                result = new string[] { "Error01", ex.ToString() };

            }
            return result;
        }
        public List<ARSalesOrder> GetSalesOrderDetails(string companyID, string invoiceNo)
        {
            List<ARSalesOrder> objARSalesOrder = new List<ARSalesOrder>();
            try
            {
                DataHelper objHelper = new DataHelper();
                objARSalesOrder = objHelper.GetSalesOrderDetails(companyID, invoiceNo);
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
        public List<ARSalesOrder> GetSalesOrderDetailsByCustomerId(string companyID, string customerId)
        {
            DataHelper objHelper = new DataHelper();
            List<ARSalesOrder> objSalesOrder = new List<ARSalesOrder>();
            try
            {
                objSalesOrder=objHelper.GetSalesOrderDetailsByCustomerId(companyID, customerId);

            }
            catch(Exception ex)
            {
                ARSalesOrder objARsalesOrder = new ARSalesOrder();
                objARsalesOrder.ErrorMsg = ex.Message.ToString();
            }
            return objSalesOrder;
        }
        public List<ARDeliveryAddress> GetCustomerDeliveryAddress(string companyID,string customerCode)
        {
            DataHelper objHelper = new DataHelper();
            List<ARDeliveryAddress> objLstDeliveryAddress = new List<ARDeliveryAddress>();
            try
            {
                objLstDeliveryAddress = objHelper.DHGetCustomerDeliveryAddress(companyID, customerCode);
            }
            catch (Exception ex)
            {
                ARDeliveryAddress objAddress = new ARDeliveryAddress();
                objAddress.CustomerAddress  = ex.Message.ToString();
                objLstDeliveryAddress.Add(objAddress);
            }
            return objLstDeliveryAddress;
        }
        public  string[] ForgetPassword(string userID, string mobileNo)
       {
           string[] result = new string[] { };
           try
           {
               if (string.IsNullOrEmpty(userID))
                   result = new string[] { "Error201", "userID Required" };
               if (string.IsNullOrEmpty(mobileNo))
                   result = new string[] { "Error202", "mobileNo Required" };
               //if (string.IsNullOrEmpty(companyID))
               //    result = new string[] { "Error203", "companyID Required" };
               DataHelper objHelper = new DataHelper();
               result = objHelper.DHForgetPassword(userID, mobileNo);


           }
           catch (Exception ex)
           {
               result = new string[] { "Error01", ex.ToString() };

           }
           return result;

       }
       public   AccountDetails GetAccountDetails(string userID, string customerCode, string companyID)
       {
           DataHelper objHelper = new DataHelper();
           AccountDetails objAccountDetails = new AccountDetails();
           try
           {
               objAccountDetails = objHelper.DHGetAccountDetails(userID, customerCode, companyID);
           }
           catch (Exception ex)
           {

               objAccountDetails.Message = ex.Message.ToString();
               return objAccountDetails;
           }
           return objAccountDetails;

       }
       public  List <NotificationHeader>  GetNotificationHeader(string companyID)
       {
           DataHelper objHelper = new DataHelper();
           List<NotificationHeader> lstObjNotificationHeader = new List<NotificationHeader>();
           try
           {
               lstObjNotificationHeader = objHelper.DHGetNotificationHeader(companyID ) ;
           }
           catch (Exception ex)
           {   NotificationHeader objNotificationHeader = new NotificationHeader();

           objNotificationHeader.NotificationName = ex.Message.ToString();
           lstObjNotificationHeader.Add(objNotificationHeader);
            return lstObjNotificationHeader;
           }
           return lstObjNotificationHeader;
       }
       public  List<NotificationDetails>  GetNotificationDetails(int notificationID, string customerCode, string companyID)
       {
           DataHelper objHelper = new DataHelper();
           List<NotificationDetails> lstObjNotificationDetails = new List<NotificationDetails>();
           try
           {
               lstObjNotificationDetails = objHelper.DHGetNotificationDetails(notificationID, customerCode, companyID);
           }
           catch (Exception ex)
           {
               NotificationDetails objNotificationDetails = new NotificationDetails();

               objNotificationDetails.NotificationText  = ex.Message.ToString();
               lstObjNotificationDetails.Add(objNotificationDetails);
               return lstObjNotificationDetails;
           }
           return lstObjNotificationDetails;

       }
       public   string[] UpdateAccountDetails(string userID, string companyID ,string userName,string mobileNo,string emailID,string password,string profilePicture)
       {
           string[] result = new string[] { };
           try
           {
               if (string.IsNullOrEmpty(userID))
                   result = new string[] { "Error201", "CompanyID Required" };
               if (string.IsNullOrEmpty(userName))
                   result = new string[] { "Error202", "CustomerCode Required" };
               if (string.IsNullOrEmpty(companyID))
                   result = new string[] { "Error203", "companyID Required" };
               if (string.IsNullOrEmpty(password))
                   result = new string[] { "Error204", "Password Required" };
            
               DataHelper objHelper = new DataHelper();
               result = objHelper.UpdateAccountDetails(userID, companyID, userName, mobileNo, emailID, password, profilePicture);


           }
           catch (Exception ex)
           {
               result = new string[] { "Error01", ex.ToString() };

           }
           return result;
       }

        //Default functionalities
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public string SendSMS()
        {
            string result = string.Empty;
            try
            {
                DataHelper objHelper = new DataHelper();
                result = objHelper.SendSMS ();
            }
            catch (Exception ex)
            {
                result = ex.Message.ToString();

            }
            return result;
        }
    }
}
