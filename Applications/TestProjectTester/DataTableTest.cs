using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectTester
{
	internal class DataTableTest
	{

		private static IDbConnection GetDataConnection()
		{
			return new OleDbConnection()
			{
				ConnectionString = "Provider=SQLNCLI11;Data Source=WS-FFORMENTI\\SQLEXPRESS;Persist Security Info=True;Integrated Security=SSPI;Initial Catalog=TestAnastasia"
			};
		}

		internal static void TestWithReader()
		{
			using var connection = GetDataConnection();
			connection.Open();
			try
			{
				using var command = connection.CreateCommand();
				command.CommandText = "SELECT * FROM NSCompanyVehicles";
				using var reader = command.ExecuteReader();


				var table = new DataTable();
				table.Columns.Add("VehicleID", typeof(int));
				table.Columns.Add("VehicleName", typeof(string));
				while (reader.Read())
				{
					var row = table.NewRow();
					row["VehicleID"] = reader.GetInt32(reader.GetOrdinal("CV_CompanyVehicleID"));
					row["VehicleName"] = reader.GetString(reader.GetOrdinal("CV_VehicleName"));
					table.Rows.Add(row);
				}
			}
			finally
			{
				connection.Close();
			}

		}

		internal static void TestWithAdapter()
		{
			using var connection = GetDataConnection();
			connection.Open();
			try
			{
				using var command = connection.CreateCommand();
				command.CommandText = "SELECT CV_CompanyVehicleID, CV_VehicleName FROM NSCompanyVehicles";
				using var adapter = new OleDbDataAdapter((OleDbCommand)command);
				var table = new DataTable();
				adapter.Fill(table);

				foreach (DataRow row in table.Rows)
				{
					Console.WriteLine($"ID = {row["CV_CompanyVehicleID"]}: {row["CV_VehicleName"]}");
				}
			}
			finally
			{
				connection.Close();
			}

		}

		internal static void TestInsertAndUpdate()
		{
			using var connection = GetDataConnection();
			connection.Open();
			try
			{
				using var transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead);
				using var cmd = connection.CreateCommand();
				try
				{
					cmd.Transaction = transaction;
					cmd.CommandText = "DELETE FROM [dbo].[NSCompanyVehicles] WHERE [CV_VehicleName]='Giulietta bianca'";
					cmd.ExecuteNonQuery();

					cmd.CommandText = @"
INSERT INTO [dbo].[NSCompanyVehicles]
           ([CV_VehicleName]
           ,[CV_VehicleKm]
           ,[CV_VehicleType]
           ,[CV_WorkingHours]
           ,[CV_PurchaseCost]
           ,[CV_PurchaseDate]
           ,[CV_YearOfProduction]
           ,[CV_FuelType]
           ,[CV_VehicleState]
           ,[CV_LoadCapacity]
           ,[CV_VehicleMaxSpeed])
     VALUES
           ('Giulietta bianca'
           ,240000
           ,'C'
           ,0
           ,6000
           ,'2018-05-01'
           ,2011
           ,'Diesel'
           ,'R'
           ,400
           ,220);
SELECT SCOPE_IDENTITY();
";
					var newID = Convert.ToInt32(cmd.ExecuteScalar());
					cmd.CommandText = $"UPDATE [dbo].[NSCompanyVehicles] SET [CV_PurchaseCost] = 9000 WHERE CV_CompanyVehicleID={newID}";
					cmd.ExecuteNonQuery();

					transaction.Commit();
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					throw new Exception($"L'aggiornamento/inserimento del veicolo è fallito: {ex.Message}", ex);
				}
			}
			finally
			{
				connection.Close();
			}

		}


	}
}
