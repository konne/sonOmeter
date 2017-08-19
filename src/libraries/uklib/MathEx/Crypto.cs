using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;

namespace UKLib.MathEx
{
	public class Crypto
	{

		private static byte[] KEY_64 = {42, 16, 93, 156, 78, 4, 218, 32};
		private static byte[] IV_64 = {55, 103, 246, 79, 36, 99, 167, 3};
		private static byte[] KEY_192 = {42, 16, 93, 156, 78, 4, 218, 32, 15, 167, 44, 80, 26, 250, 155, 112, 2, 94, 11, 204, 119, 35, 184, 197};
		private static byte[] IV_192 = {55, 103, 246, 79, 36, 99, 167, 3, 42, 5, 62, 83, 184, 7, 209, 13, 145, 23, 200, 58, 173, 10, 121, 222};

		private static DESCryptoServiceProvider cpDES = null;
		private static TripleDESCryptoServiceProvider cpTripleDES = null;

		public static void ResetDES()
		{
			if (cpDES == null)
				return;

			try { cpDES.Clear(); }
			finally { cpDES = null; }
		}

		public static void ResetTripleDES()
		{
			if (cpTripleDES == null)
				return;

			try { cpTripleDES.Clear(); }
			finally { cpTripleDES = null; }
		}

		public static string Encrypt(string value)
		{
			if (value != "") 
			{
				if (cpDES == null)
					cpDES = new DESCryptoServiceProvider();

				MemoryStream ms = new MemoryStream();
				CryptoStream cs = new CryptoStream(ms, cpDES.CreateEncryptor(KEY_64, IV_64), CryptoStreamMode.Write);
				StreamWriter sw = new StreamWriter(cs);
				sw.Write(value);
				sw.Flush();
				cs.FlushFinalBlock();
				ms.Flush();
				return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
			}

			return "";
		}

		public static string Decrypt(string value)
		{
			if (value != "") 
			{
				if (cpDES == null)
					cpDES = new DESCryptoServiceProvider();

				byte[] buffer = Convert.FromBase64String(value);
				MemoryStream ms = new MemoryStream(buffer);
				CryptoStream cs = new CryptoStream(ms, cpDES.CreateDecryptor(KEY_64, IV_64), CryptoStreamMode.Read);
				StreamReader sr = new StreamReader(cs);
				return sr.ReadToEnd();
			}

			return "";
		}

		public static string EncryptTripleDES(string value)
		{
			if (value != "") 
			{
				if (cpTripleDES == null)
					cpTripleDES = new TripleDESCryptoServiceProvider();

				MemoryStream ms = new MemoryStream();
				CryptoStream cs = new CryptoStream(ms, cpTripleDES.CreateEncryptor(KEY_192, IV_192), CryptoStreamMode.Write);
				StreamWriter sw = new StreamWriter(cs);
				sw.Write(value);
				sw.Flush();
				cs.FlushFinalBlock();
				ms.Flush();
				return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
			}

			return "";
		}

		public static string DecryptTripleDES(string value)
		{
			if (value != "") 
			{
				if (cpTripleDES == null)
					cpTripleDES = new TripleDESCryptoServiceProvider();

				byte[] buffer = Convert.FromBase64String(value);
				MemoryStream ms = new MemoryStream(buffer);
				CryptoStream cs = new CryptoStream(ms, cpTripleDES.CreateDecryptor(KEY_192, IV_192), CryptoStreamMode.Read);
				StreamReader sr = new StreamReader(cs);
				return sr.ReadToEnd();
			}

			return "";
		}
	}
}