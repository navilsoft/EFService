using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace EFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IEightGems
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
       // UriTemplate = "CreateARSalesOrder",
       RequestFormat = WebMessageFormat.Json,
       ResponseFormat = WebMessageFormat.Json,
       BodyStyle = WebMessageBodyStyle.Wrapped)]
        string[] CreateARSalesOrder(String companyID, ARSalesorderHeader objARSalesOrderHeader, ARSalesorderDetail[] objARSalesOrderDetails);

        [OperationContract]
        [WebInvoke(Method = "GET",
        //UriTemplate = "GetSalesOrderDetails",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<ARSalesOrder> GetSalesOrderDetails(string companyID, string invoiceNo);

        [OperationContract]
        [WebInvoke(Method = "GET",
        //UriTemplate = "GetSalesOrderDetailsByCustomerId",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<ARSalesOrder> GetSalesOrderDetailsByCustomerId(string companyID, string customerId);

        [OperationContract]
        [WebInvoke(Method = "GET",
        //UriTemplate = "GetSalesOrderDetailsByCustomerId",
        ResponseFormat = WebMessageFormat.Json,
        BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<ARDeliveryAddress> GetCustomerDeliveryAddress(string companyID, string customerCode);



        [OperationContract]
        [WebInvoke(Method = "GET",
          // UriTemplate = "AuthenticateUser",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Wrapped)]
        string[] AuthenticateUser(string userID, string password, string companyID);

      //  [OperationContract]
      //  [WebInvoke(Method = "POST",
      ////  UriTemplate = "CreateARSalesOrder",
      //  RequestFormat = WebMessageFormat.Json,
      //  ResponseFormat = WebMessageFormat.Json,
      //  BodyStyle = WebMessageBodyStyle.Wrapped)]
      //  string[] CreateARSalesOrder(String companyID, ARSalesorderHeader objARSalesOrderHeader, ARSalesorderDetail[] objARSalesOrderDetails);

      //  [OperationContract]
      //  [WebInvoke(Method = "GET",
      //  //UriTemplate = "GetSalesOrderDetails",
      //  ResponseFormat = WebMessageFormat.Json,
      //  BodyStyle = WebMessageBodyStyle.Wrapped)]
      //  List<ARSalesOrder> GetSalesOrderDetails(string companyID, string invoiceNo);

      //  [OperationContract]
      //  [WebInvoke(Method = "GET",
      //  //UriTemplate = "GetSalesOrderDetailsByCustomerId",
      //  ResponseFormat = WebMessageFormat.Json,
      //  BodyStyle = WebMessageBodyStyle.Wrapped  )]
      //  List<ARSalesOrder> GetSalesOrderDetailsByCustomerId(string companyID, string customerId);

        //[OperationContract]
        //[WebInvoke(Method = "GET",
        //    //UriTemplate = "GetSalesOrderDetailsByCustomerId",
        //ResponseFormat = WebMessageFormat.Json,
        //BodyStyle = WebMessageBodyStyle.Wrapped)]
        //List<ARDeliveryAddress> GetCustomerDeliveryAddress(string companyID, string customerCode);

        [OperationContract]
        [WebInvoke(Method = "POST",
            // UriTemplate = "AuthenticateUser",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Wrapped)]
        string[] ForgetPassword(string userID, string mobileNo);


        [OperationContract]
        [WebInvoke(Method = "GET",
            // UriTemplate = "AuthenticateUser",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Wrapped)]
        AccountDetails  GetAccountDetails(string userID, string customerCode, string companyID);

          [OperationContract]
        [WebInvoke(Method = "POST",
            // UriTemplate = "AuthenticateUser",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Wrapped)]
        string[]  UpdateAccountDetails(string userID, string companyID ,string userName,string mobileNo,string emailID,string password,string profilePicture);

        [OperationContract]
        [WebInvoke(Method = "GET",
            // UriTemplate = "AuthenticateUser",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<NotificationHeader>  GetNotificationHeader(string companyID);

        [OperationContract]
        [WebInvoke(Method = "GET",
            // UriTemplate = "AuthenticateUser",
          RequestFormat = WebMessageFormat.Json,
          ResponseFormat = WebMessageFormat.Json,
          BodyStyle = WebMessageBodyStyle.Wrapped)]
        List<NotificationDetails> GetNotificationDetails(int notificationID, string customerCode, string companyID);


        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here
    }
    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }

  [DataContract]
    public class AccountDetails
    {
        string userName = string.Empty;
        string userID = string.Empty;
        string mobileNo = string.Empty;
        string emailID = string.Empty;
        string url = string.Empty;
        string message = string.Empty;
        string password = string.Empty;


         [DataMember]
        public string MobileNo
        {
            get { return mobileNo; }
            set { mobileNo = value; }
        }

         [DataMember]
         public string EmailID
         {
             get { return emailID; }
             set { emailID = value; }
         }

         [DataMember]
         public string UserName
         {
             get { return userName; }
             set { userName = value; }
         }

         [DataMember]
         public string UserID
         {
             get { return userID; }
             set { userID = value; }
         }
         [DataMember]
         public string URL
         {
             get { return url; }
             set { url = value; }
         }
         [DataMember]
         public string Message
         {
             get { return message; }
             set { message = value; }
         }
         [DataMember]
         public string Password
         {
             get { return password ; }
             set { password = value; }
         }

    }


  [DataContract]
  public class NotificationHeader
  {
      string notificationName = string.Empty;
      int notificationID = 0;
      

      [DataMember]
      public string NotificationName
      {
          get { return notificationName; }
          set { notificationName = value; }
      }

      [DataMember]
      public int NotificationID
      {
          get { return notificationID; }
          set { notificationID = value; }
      }
  }



    [DataContract]
    public class NotificationDetails
    {
        string notificationText = string.Empty;
        int notificationID = 0;
        int notificationDetailsID = 0;
        string customerCode = string.Empty;



        [DataMember]
        public string NotificationText
        {
            get { return notificationText; }
            set { notificationText = value; }
        }

        [DataMember]
        public int NotificationID
        {
            get { return notificationID; }
            set { notificationID = value; }
        }

        [DataMember]
        public int NotificationDetailsID
        {
            get { return notificationDetailsID; }
            set { notificationDetailsID = value; }
        }

        [DataMember]
        public string CustomerCode
        {
            get { return customerCode; }
            set { customerCode = value; }
        }




    }
    [DataContract]
    public class ARSalesorderHeader
    {
        string invoiceNo = string.Empty;
        string invoiceDate = string.Empty;
        string customerID = string.Empty;
        string codeSalesType = string.Empty;
        string ourRefNo = string.Empty;
        string yourRefNo = string.Empty;
        string netTotal = string.Empty;
        string taxAmount = string.Empty;
        string grossTotal = string.Empty;
        string remarks = string.Empty;
        string createdUserID = string.Empty;
        string permitNo = string.Empty;
        string salesLineNo = string.Empty;
        string transactionDate = string.Empty;
        string foreignNetTotal = string.Empty;
        string foreignTaxAmount = string.Empty;
        string foreignGrossTotal = string.Empty;
        string currencyCode = string.Empty;
        string exchangeRate = string.Empty;
        string codeLocation = string.Empty;
        string projectName = string.Empty;
        string inventoryType = string.Empty;
        string deliveryDate = string.Empty;
        string remainderFlag = string.Empty;
        string deliveryPeriod = string.Empty;
        string deliveryAddressID = string.Empty;

        [DataMember]
        public string InvoiceNo
        { get { return invoiceNo; } set { invoiceNo = value; } }
        [DataMember]
        public string InvoiceDate
        { get { return invoiceDate; } set { invoiceDate = value; } }
        [DataMember]
        public string CustomerID
        { get { return customerID; } set { customerID = value; } }
        [DataMember]
        public string CodeSalesType
        { get { return codeSalesType; } set { codeSalesType = value; } }
        [DataMember]
        public string OurRefNo
        { get { return ourRefNo; } set { ourRefNo = value; } }
        [DataMember]
        public string YourRefNo
        { get { return yourRefNo; } set { yourRefNo = value; } }
        [DataMember]
        public string NetTotal
        { get { return netTotal; } set { netTotal = value; } }
        [DataMember]
        public string TaxAmount
        { get { return taxAmount; } set { taxAmount = value; } }
        [DataMember]
        public string GrossTotal
        { get { return grossTotal; } set { grossTotal = value; } }
        [DataMember]
        public string Remarks
        { get { return remarks; } set { remarks = value; } }
        [DataMember]
        public string CreatedUserID
        { get { return createdUserID; } set { createdUserID = value; } }
        [DataMember]
        public string PermitNo
        { get { return permitNo; } set { permitNo = value; } }
        [DataMember]
        public string SalesLineNo
        { get { return salesLineNo; } set { salesLineNo = value; } }
        [DataMember]
        public string TransactionDate
        { get { return transactionDate; } set { transactionDate = value; } }
        [DataMember]
        public string ForeignNetTotal
        { get { return foreignNetTotal; } set { foreignNetTotal = value; } }
        [DataMember]
        public string ForeignTaxAmount
        { get { return foreignTaxAmount; } set { foreignTaxAmount = value; } }
        [DataMember]
        public string ForeignGrossTotal
        { get { return foreignGrossTotal; } set { foreignGrossTotal = value; } }
        [DataMember]
        public string CurrencyCode
        { get { return currencyCode; } set { currencyCode = value; } }
        [DataMember]
        public string ExchangeRate
        { get { return exchangeRate; } set { exchangeRate = value; } }
        [DataMember]
        public string CodeLocation
        { get { return codeLocation; } set { codeLocation = value; } }
        [DataMember]
        public string ProjectName
        { get { return projectName; } set { projectName = value; } }
        [DataMember]
        public string InventoryType
        { get { return inventoryType; } set { inventoryType = value; } }
        [DataMember]
        public string DeliveryDate
        { get { return deliveryDate; } set { deliveryDate = value; } }
        [DataMember]
        public string RemainderFlag
        { get { return remainderFlag; } set { remainderFlag = value; } }
        [DataMember]
        public string DeliveryPeriod
        { get { return deliveryPeriod; } set { deliveryPeriod = value; } }
        [DataMember]
        public string DeliveryAddressID { get { return deliveryAddressID; } set { deliveryAddressID = value; } }
    }
    [DataContract]
    public class ARSalesorderDetail
    {
        public string aRSalesOrderDetailRefNo = string.Empty;
        public string salesLineNo = string.Empty;
        public string accountCode = string.Empty;
        public string inventoryCode = string.Empty;
        public string description = string.Empty;
        public string quantity = string.Empty;
        public string requestQuantity = string.Empty;
        public string receivedQuantity = string.Empty;
        public string unitPrice = string.Empty;
        public string netTotal = string.Empty;
        public string taxAmount = string.Empty;
        public string grossTotal = string.Empty;
        public string taxCode = string.Empty;
        public string createdUserID = string.Empty;
        public string foreignNetTotal = string.Empty;
        public string foreignTaxAmount = string.Empty;
        public string foreignGrossTotal = string.Empty;
        public string codeUnitMeasure = string.Empty;
        public string size = string.Empty;
        [DataMember]
        public string ARSalesOrderDetailRefNo { get { return aRSalesOrderDetailRefNo; } set { aRSalesOrderDetailRefNo = value; } }
        [DataMember]
        public string SalesLineNo { get { return salesLineNo; } set { salesLineNo = value; } }
        [DataMember]
        public string AccountCode { get { return accountCode; } set { accountCode = value; } }
        [DataMember]
        public string InventoryCode { get { return inventoryCode; } set { inventoryCode = value; } }
        [DataMember]
        public string Description { get { return description; } set { description = value; } }
        [DataMember]
        public string Quantity { get { return quantity; } set { quantity = value; } }
        [DataMember]
        public string RequestQuantity { get { return requestQuantity; } set { requestQuantity = value; } }
        [DataMember]
        public string ReceivedQuantity { get { return receivedQuantity; } set { receivedQuantity = value; } }
        [DataMember]
        public string UnitPrice { get { return unitPrice; } set { unitPrice = value; } }
        [DataMember]
        public string NetTotal { get { return netTotal; } set { netTotal = value; } }
        [DataMember]
        public string TaxAmount { get { return taxAmount; } set { taxAmount = value; } }
        [DataMember]
        public string GrossTotal { get { return grossTotal; } set { grossTotal = value; } }
        [DataMember]
        public string TaxCode { get { return taxCode; } set { taxCode = value; } }
        [DataMember]
        public string CreatedUserID { get { return createdUserID; } set { createdUserID = value; } }
        [DataMember]
        public string ForeignNetTotal { get { return foreignNetTotal; } set { foreignNetTotal = value; } }
        [DataMember]
        public string ForeignTaxAmount { get { return foreignTaxAmount; } set { foreignTaxAmount = value; } }
        [DataMember]
        public string ForeignGrossTotal { get { return foreignGrossTotal; } set { foreignGrossTotal = value; } }
        [DataMember]
        public string CodeUnitMeasure { get { return codeUnitMeasure; } set { codeUnitMeasure = value; } }
        [DataMember]
        public string Size { get { return size; } set { size = value; } }
    }
    [DataContract]
    public class ARSalesOrder
    {
        public string invoiceNo = string.Empty;
        public string invoiceDate = string.Empty;
        public string customerID = string.Empty;
        public string aRSalesOrderDetailRefNo = string.Empty;
        public string inventoryCode = string.Empty;
        public string description = string.Empty;
        public string quantity = string.Empty;
        public string requestQuantity = string.Empty;
        public string receivedQuantity = string.Empty;
        public string unitPrice = string.Empty;
        public string netTotal = string.Empty;
        public string taxAmount = string.Empty;
        public string grossTotal = string.Empty;
        public string errorMsg = string.Empty;
        public string deliveryDate = string.Empty;
        public string remainderFlag = string.Empty;
        public string deliveryPeriod = string.Empty;
        public string size = string.Empty;
        public string deliveryAddressID = string.Empty;
        [DataMember]
        public string InvoiceNo { get { return invoiceNo; } set { invoiceNo = value; } }
        [DataMember]
        public string InvoiceDate { get { return invoiceDate; } set { invoiceDate = value; } }
        [DataMember]
        public string CustomerID { get { return customerID; } set { customerID = value; } }
        [DataMember]
        public string ARSalesOrderDetailRefNo { get { return aRSalesOrderDetailRefNo; } set { aRSalesOrderDetailRefNo = value; } }
        [DataMember]
        public string InventoryCode { get { return inventoryCode; } set { inventoryCode = value; } }
        [DataMember]
        public string Description { get { return description; } set { description = value; } }
        [DataMember]
        public string Quantity { get { return quantity; } set { quantity = value; } }
        [DataMember]
        public string RequestQuantity { get { return requestQuantity; } set { requestQuantity = value; } }
        [DataMember]
        public string ReceivedQuantity { get { return receivedQuantity; } set { receivedQuantity = value; } }
        [DataMember]
        public string UnitPrice { get { return unitPrice; } set { unitPrice = value; } }
        [DataMember]
        public string NetTotal { get { return netTotal; } set { netTotal = value; } }
        [DataMember]
        public string TaxAmount { get { return taxAmount; } set { taxAmount = value; } }
        [DataMember]
        public string GrossTotal { get { return grossTotal; } set { grossTotal = value; } }
        [DataMember]
        public string ErrorMsg { get { return errorMsg; } set { errorMsg = value; } }
        [DataMember]
        public string DeliveryDate
        { get { return deliveryDate; } set { deliveryDate = value; } }
        [DataMember]
        public string RemainderFlag
        { get { return remainderFlag; } set { remainderFlag = value; } }
        [DataMember]
        public string DeliveryPeriod
        { get { return deliveryPeriod; } set { deliveryPeriod = value; } }

        [DataMember]
        public string Size { get { return size; } set { size = value; } }
        [DataMember]
        public string DeliveryAddressID { get { return deliveryAddressID; } set { deliveryAddressID = value; } }
    }
    [DataContract]
    public class ARSalesDetail
    {
        public string salesLineNo = string.Empty;
        public string accountCode = string.Empty;
        public string inventoryCode = string.Empty;
        public string description = string.Empty;
        public string quantity = string.Empty;
        public string unitPrice = string.Empty;
        public string netTotal = string.Empty;
        public string taxAmount = string.Empty;
        public string grossTotal = string.Empty;
        public string taxCode = string.Empty;
        public string foreignNetTotal = string.Empty;
        public string foreignTaxAmount = string.Empty;
        public string foreignGrossTotal = string.Empty;
        public string discountAmount = string.Empty;
        public string discountPercentage = string.Empty;

        [DataMember]
        public string SalesLineNo { get { return salesLineNo; } set { salesLineNo = value; } }
        [DataMember]
        public string AccountCode { get { return accountCode; } set { accountCode = value; } }
        [DataMember]
        public string InventoryCode { get { return inventoryCode; } set { inventoryCode = value; } }
        [DataMember]
        public string Description { get { return description; } set { description = value; } }
        [DataMember]
        public string Quantity { get { return quantity; } set { quantity = value; } }
        [DataMember]
        public string UnitPrice { get { return unitPrice; } set { unitPrice = value; } }
        [DataMember]
        public string NetTotal { get { return netTotal; } set { netTotal = value; } }
        [DataMember]
        public string TaxAmount { get { return taxAmount; } set { taxAmount = value; } }
        [DataMember]
        public string GrossTotal { get { return grossTotal; } set { grossTotal = value; } }
        [DataMember]
        public string TaxCode { get { return taxCode; } set { taxCode = value; } }
        [DataMember]
        public string ForeignNetTotal { get { return foreignNetTotal; } set { foreignNetTotal = value; } }
        [DataMember]
        public string ForeignTaxAmount { get { return foreignTaxAmount; } set { foreignTaxAmount = value; } }
        [DataMember]
        public string ForeignGrossTotal { get { return foreignGrossTotal; } set { foreignGrossTotal = value; } }
        [DataMember]
        public string DiscountAmount { get { return discountAmount; } set { discountAmount = value; } }
        [DataMember]
        public string DiscountPercentage { get { return discountPercentage; } set { discountPercentage = value; } }

    }
    [DataContract]
    public class ARDeliveryAddress
    {
        public string customerCode = string.Empty;
        public string customerAddress = string.Empty;
        public string addressID = string.Empty;
        [DataMember]
        public string CustomerCode { get { return customerCode; } set { customerCode = value; } }
        [DataMember]
        public string CustomerAddress { get { return customerAddress; } set { customerAddress = value; } }
        [DataMember]
        public string AddressID { get { return addressID; } set { addressID = value; } }
    }


}


