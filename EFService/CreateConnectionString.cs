using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFService
{
    class CreateConnectionString
    {
       public string GetCompanyDbString(string companyId, string strModuleName)
       {
           if (strModuleName.Equals("Master"))
           {
               return ConfigurationManager.ConnectionStrings["conString"].ConnectionString.ToString();
           }
           else
           {
               using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["conString"].ConnectionString.ToString()))
               {
                   try
                   {
                       using (SqlCommand cmd = new SqlCommand("SELECT ConnectionString FROM CustomerModules WHERE companyId='" + companyId + "' AND ModuleVersionsID IN (select ModuleVersionsID FROM ModuleVersions WHERE ModuleID IN (SELECT ModuleID FROM Modules WHERE ModuleName = '" + strModuleName + "'))", con))
                       {
                           con.Open();
                           SqlDataReader reader;
                           cmd.CommandType = CommandType.Text;
                           reader = cmd.ExecuteReader();
                           if (reader.HasRows)
                           {
                               reader.Read();
                               String connectionString = reader["ConnectionString"].ToString();
                               reader.Close();
                               con.Close();
                               String[] constr = connectionString.Split(';');
                               String[] constr1 = constr[0].Split('=');
                               String dbname = constr1[1];
                               String[] constr2 = constr[1].Split('=');
                               String server = constr2[1];
                               String[] constr3 = constr[2].Split('=');
                               String user = constr3[1];
                               String[] constr4 = constr[3].Split('=');
                               String pass = constr4[1];

                               connectionString = "Data Source=" + server + ";Initial Catalog=" + dbname + ";Persist Security Info=true; User ID=" + user + ";Password=" + pass;
                               //connectionString = "Data Source=" + dbname + ";Initial Catalog=" + server + ";Integrated Security=True";                           
                               return connectionString;
                           }
                           else
                           {
                               if (con.State == ConnectionState.Open)
                                   con.Close();
                               return null;
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
}
