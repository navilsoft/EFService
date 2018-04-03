using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EFService
{
  public static class EncryptDecrypt
    {
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

        public static string Decrypt(string txtEncrpt)
        {
            string SecretKey = "_$!9_?#_)";
            // any 8 char
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

            if (txtEncrpt == null | txtEncrpt.Trim() == string.Empty)
            {
                return null;
            }
            string encryptKey = SecretKey;
            byte[] inputByte = new byte[txtEncrpt.Trim().Length + 2];
            try
            {
                key = System.Text.Encoding.UTF8.GetBytes(encryptKey.ToCharArray(), 0, 8);
                //Dim des As SHA256CryptoServiceProvider = New SHA256CryptoServiceProvider()
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByte = Convert.FromBase64String(txtEncrpt.Trim());
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, b4), CryptoStreamMode.Write);
                cs.Write(inputByte, 0, inputByte.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception err)
            {
                return err.Message;
            }
        }

      
    }
}
