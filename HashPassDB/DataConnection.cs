using System;
using System.Data.OleDb;
using System.Security.Cryptography;
using System.Text;

namespace HashPassDB
{
	static class DataConnection
	{
		const string conString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\HashPassDB\UserPasswords.accdb;Persist Security Info=False";
		
		public static string Login(string username, string password)
		{
			if (checkUser(username, getDropBoxHash(username, password))) return "loged in";
			return "input data error";
		}
		
		public static string CreateUser(string username, string password, string confim)
		{
			if (checkUser(username)) return "user exist";
			if (username != "" && password == confim && password != "")
			{
				using(var connection = new OleDbConnection(conString)) // true programming
				{
					connection.Open();
					string s = "INSERT INTO Users VALUES (@user, @pass)";
					OleDbCommand command = new OleDbCommand(s, connection);
					command.Parameters.AddWithValue("user", username);
					command.Parameters.AddWithValue("pass", getDropBoxHash(username, password));
					OleDbTransaction transaction = connection.BeginTransaction(); // true programming
					command.Transaction = transaction;
					
					command.ExecuteNonQuery();
					transaction.Commit(); // do not forget
					connection.Close();
					return "new user created";
				}
			}
			return "error in new values";
		}
		
		static string getDropBoxHash(string username, string password)
		{
			HashSteps.ClearSteps();
			HashSteps.WriteStep(password);
			
			// DropBox chelenge
			SHA1CryptoServiceProvider provider = new SHA1CryptoServiceProvider();
			
			// hash password
			byte[] bytes = Encoding.Unicode.GetBytes(password);
			bytes = provider.ComputeHash(bytes);
			
			HashSteps.WriteStep(Convert.ToBase64String(bytes));
			
			// BScript = 10 * (hash-pass + salt)
			string s = Convert.ToBase64String(bytes) + username;
			
			HashSteps.WriteStep("hash+salt " + s);
			
			bytes = Encoding.Unicode.GetBytes(s);
			for (int i = 0; i < 10; i++)
			{
				bytes = provider.ComputeHash(bytes);
				HashSteps.WriteStep("bscript" + i + " " + Convert.ToBase64String(bytes));
			}
			
			#region AES
			AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
			aes.KeySize = 128;
			aes.Key = Encoding.Unicode.GetBytes("HashPassDataBase"); // 16 chars // TODO get key from txt
			aes.IV = Encoding.ASCII.GetBytes("HashPassDataBase"); // 16 chars // TODO get key from txt
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.PKCS7;
			
			ICryptoTransform transform = aes.CreateEncryptor();
			bytes = transform.TransformFinalBlock(bytes, 0, bytes.Length);
			#endregion
			
			HashSteps.WriteStep("AES " + Convert.ToBase64String(bytes));
			return Convert.ToBase64String(bytes);
		}
		static bool checkUser(string username)
		{
			using(var connection = new OleDbConnection(conString))
			{
				connection.Open();
				string s = "SELECT Username FROM Users WHERE Username = @name";
				OleDbCommand command = new OleDbCommand(s, connection);
				command.Parameters.AddWithValue("name", username);
				var result = command.ExecuteScalar();
				connection.Close();
				if (result != null) return true;
			}
			return false;
		}
		static bool checkUser(string username, string password)
		{
			using(var connection = new OleDbConnection(conString))
			{
				connection.Open();
				string s = "SELECT * FROM Users WHERE Username = @name And Password = @pass";
				OleDbCommand command = new OleDbCommand(s, connection);
				command.Parameters.AddWithValue("name", username);
				command.Parameters.AddWithValue("pass", password);
				var result = command.ExecuteScalar();
				connection.Close();
				if (result != null) return true;
			}
			return false;
		}
	}
}
