using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFService
{
   public class SQLExecution
    {
        private string MessageHeader { get; set; }
        public string ConnectionString { get; set; }
        CreateConnectionString objConnectionString;
        public SQLExecution(string companyID, string moduleName)
        {
            MessageHeader = "SqlExecution";
            objConnectionString =new  CreateConnectionString();
            ConnectionString = objConnectionString.GetCompanyDbString(companyID, moduleName);
         }
        public string[] ExecuteNonQuery(string procedureName, List<SqlParameter> parameters)
        {
            string[] result = new string[] { };
            int res = 0;
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand sqlCommand = new SqlCommand(procedureName, sqlConn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            sqlCommand.Parameters.AddRange(parameters.ToArray());
                        }
                     res =  sqlCommand.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception ex)
            {
                result = new string[] { "Error", ex.ToString() };
            }
            result = new string[] {"Success",  res.ToString() };
            return result;
        }
        public object RunSP_ReturnDynObject(string procedureName, List<SqlParameter> parameters)
        {
             
            dynamic obj = new ExpandoObject();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(procedureName, sqlConn))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        sqlCommand.Parameters.AddRange(parameters.ToArray());
                    }
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(obj);
                    }
                }
            }
            return obj;
        }
        public DataTable  RunSP_ReturnDT(string procedureName, List<SqlParameter> parameters)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(procedureName, sqlConn))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        sqlCommand.Parameters.AddRange(parameters.ToArray());
                    }
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return dt;
        }
        public DataTable RunSP_ReturnDT(string procedureName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(procedureName, sqlConn))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    //if (parameters != null)
                    //{
                    //    sqlCommand.Parameters.AddRange(parameters.ToArray());
                    //}
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return dt;
        }
        public object RunSP_ReturnObject(string procedureName)
        {

            dynamic obj = new ExpandoObject();
            using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand(procedureName, sqlConn))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    //if (parameters != null)
                    //{
                    //    sqlCommand.Parameters.AddRange(parameters.ToArray());
                    //}
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(obj);
                    }
                }
            }
            return obj;
        }
        public string[] RunSP_ReturnStringArray(string procedureName, List<SqlParameter> parameters)
        {
                     
            DataTable dt = new DataTable();
            string[] result = new string[] { "Error", MessageHeader, "EF501" };
            try
            {
                using (SqlConnection sqlConn = new SqlConnection(ConnectionString))
                {
                  
                    using (SqlCommand sqlCommand = new SqlCommand(procedureName, sqlConn))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            sqlCommand.Parameters.AddRange(parameters.ToArray());
                        }
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                        {
                            sqlDataAdapter.Fill(dt);
                        }
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    result = new string[] { Convert.ToString(dt.Rows[0]["Result"]), Convert.ToString(dt.Rows[0]["MenuID"]), Convert.ToString(dt.Rows[0]["CustomerID"]) };
                }
            }
            catch(Exception ex)
            {
                result = new string[] { "Error", MessageHeader, ex.Message.ToString () };
            }
            return result ;
        }
    }
}
