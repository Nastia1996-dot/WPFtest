using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Runtime.Versioning;
using TestProjectLibrary.Localization.Models;
using TestProjectLibrary.Models;
using TestProjectLibrary.Models.Enums;
using TestProjectLibrary.Services;

namespace TestProjectLibrary.ServiceImplementations
{

	/// <summary>
	/// Implementation of <see cref="IStoreService{TModel}"/> for <see cref="CompanyVehicle"/> on database store
	/// </summary>
	[SupportedOSPlatform("windows")]
	public class CompanyVehicleStoreServiceOnDb(IDatabaseConnectionSettings databaseConnectionSettings) : IStoreService<CompanyVehicle>
	{

		#region Properties
		private IDatabaseConnectionSettings DatabaseConnectionSettings { get; } = databaseConnectionSettings;

		private static bool UseQueryParameters { get; set; } = true;

		#endregion

		#region ICompanyVehicleStoreService

		//Oledb è legata strettamente a Windows, su linux o macOS non funziona
		private static IDbConnection GetDataConnection()
		{
			return new OleDbConnection()
			{
				ConnectionString = "Provider=SQLNCLI11;Data Source=WS-FFORMENTI\\SQLEXPRESS;Persist Security Info=True;Integrated Security=SSPI;Initial Catalog=TestAnastasia"
			};
		}

		/// <inheritdoc cref="IStoreService{TModel}.GetList"/>
		public IEnumerable<CompanyVehicle> GetList()
		{
			//apre una connessione al database
			using var connection = GetDataConnection();
			connection.Open(); //da questo momento in poi posso eseguire comandi SQL perché apro la connessione
			try
			{
				using var command = connection.CreateCommand();
				command.Parameters.Clear();
				command.CommandText = "SELECT CV_CompanyVehicleID, CV_VehicleType, CV_YearOfProduction, CV_VehicleState, CV_VehicleKm, CV_WorkingHours FROM NSCompanyVehicles";
				using var reader = command.ExecuteReader(); //esegue il comando e restituisce un lettore per leggere riga per riga i dati
				while (reader.Read())
				{
					yield return this.ReadCompanyVehicle(reader);
				}
			}
			finally
			{
				connection.Close();
			}
		}

		/// <inheritdoc cref="IStoreService{TModel}.TryCreateOrUpdate"/>
		public bool TryCreateOrUpdate(CompanyVehicle model, [NotNullWhen(false)] out ErrorResponse? error)
		{
			if (!model.TryValidateModel(out var validationErrors))
			{
				error = new ErrorResponse().SetValidationErrors(validationErrors);
				return false;
			}
			using var connection = GetDataConnection();
			connection.Open();
			//qui parte una transazione, ossia un blocco di operazioni che devono riuscire tutte insieme o nessuna
			using var transaction = connection.BeginTransaction(IsolationLevel.RepeatableRead);
			using var cmd = connection.CreateCommand();
			cmd.Transaction = transaction;

			try
			{

				if (model.VehicleID <= 0) //---INSERT----
				{
					cmd.Parameters.Clear();
					//nuovo veicolo
					if (UseQueryParameters)
					{
						//i ? sono segnaposto (placeholder) usati nelle query parametrizzate, vengono sostituiti 
						cmd.CommandText = @"INSERT INTO NSCompanyVehicles ( CV_VehicleName, CV_VehicleType, CV_YearOfProduction, CV_VehicleState, CV_VehicleKm, CV_WorkingHours) 
VALUES (?, ?, ?, ?, ?, ?);
SELECT SCOPE_IDENTITY()
"; //funzione che permette di ottenere l'ID appena generato dal db

						cmd.Parameters.Add(CreateParam(cmd, FormatVehicleType(model.VehicleType)));
						cmd.Parameters.Add(CreateParam(cmd, FormatVehicleType(model.VehicleType)));
						cmd.Parameters.Add(CreateParam(cmd, model.VehicleYearOfProduction));
						cmd.Parameters.Add(CreateParam(cmd, FormatVehicleIsActive(model.VehicleisActive)));
						cmd.Parameters.Add(CreateParam(cmd, (object?)model.VehicleKm ?? DBNull.Value));
						cmd.Parameters.Add(CreateParam(cmd, (object?)model.VehicleWorkingHours ?? DBNull.Value));
					}
					else
					{
						cmd.CommandText = $@"INSERT INTO NSCompanyVehicles (
	CV_VehicleName
	, CV_VehicleType
	, CV_YearOfProduction
	, CV_VehicleState
	, CV_VehicleKm
	, CV_WorkingHours
) 
VALUES (
	{GetSQLFormattedValue(FormatVehicleType(model.VehicleType))}
	, {GetSQLFormattedValue(FormatVehicleType(model.VehicleType))}
	, {GetSQLFormattedValue(model.VehicleYearOfProduction)}
	, {GetSQLFormattedValue(FormatVehicleIsActive(model.VehicleisActive))}
	, {GetSQLFormattedValue(model.VehicleKm)}
	, {GetSQLFormattedValue(model.VehicleWorkingHours)}
);
SELECT SCOPE_IDENTITY()
"; //funzione che permette di ottenere l'ID appena generato dal db
					}

					var newID = Convert.ToInt32(cmd.ExecuteScalar()); //metodo per leggere il nuovo id
					model.VehicleID = newID;

					transaction.Commit();
					error = null;
					return true;
				}
				else //---UPDATE----
				{

					cmd.Parameters.Clear();
					//aggiorno su db
					cmd.CommandText = @"UPDATE NSCompanyVehicles
SET CV_VehicleType = ? , CV_YearOfProduction = ?, CV_VehicleState= ?, CV_VehicleKm = ?, CV_WorkingHours = ? 
WHERE CV_CompanyVehicleID = ?
";

					//param devono esssere nello stesso ordine!!
					cmd.Parameters.Add(CreateParam(cmd, FormatVehicleType(model.VehicleType)));
					cmd.Parameters.Add(CreateParam(cmd, model.VehicleYearOfProduction));
					cmd.Parameters.Add(CreateParam(cmd, FormatVehicleIsActive(model.VehicleisActive)));
					cmd.Parameters.Add(CreateParam(cmd, (object?)model.VehicleKm ?? DBNull.Value));
					cmd.Parameters.Add(CreateParam(cmd, (object?)model.VehicleWorkingHours ?? DBNull.Value));
					cmd.Parameters.Add(CreateParam(cmd, model.VehicleID));
					if (cmd.ExecuteNonQuery() > 0)
					{
						error = null;
						return true;
					}
					else
					{
						error = new ErrorResponse().SetNotFound(string.Format(CompanyVehicleLoc.NotFoundMessageFormat));
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				transaction.Rollback();
				error = new ErrorResponse().SetInternalServerErrorInfo(ex.Message);
				return false;
			}
			finally
			{
				connection.Close();
			}

		}

		/// <inheritdoc cref="IStoreService{TModel}.TryDelete"/>
		public bool TryDelete(int id)
		{
			using var connection = GetDataConnection();
			connection.Open();
			try
			{

				using var command = connection.CreateCommand();
				command.Parameters.Clear();
				command.CommandText = "DELETE FROM NSCompanyVehicles " +
									  "WHERE CV_CompanyVehicleID = ? ";
				var param = command.CreateParameter();
				param.Value = id;
				command.Parameters.Add(param);

				int affectedRows = command.ExecuteNonQuery();
				if (affectedRows > 0)
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			finally
			{
				connection.Close();
			}
		}

		/// <inheritdoc cref="IStoreService{TModel}.TryRead"/>
		public bool TryRead(int id, [NotNullWhen(true)] out CompanyVehicle? vehicle)
		{

			//crea var dove salvare il risultato letto
			using var connection = GetDataConnection();
			connection.Open();

			try
			{
				using var command = connection.CreateCommand();
				command.Parameters.Clear();
				command.CommandText = $"SELECT CV_CompanyVehicleID, CV_VehicleType, CV_YearOfProduction, CV_VehicleState, CV_VehicleKm, CV_WorkingHours " +
									  $"FROM NSCompanyVehicles " +
									  $"WHERE CV_CompanyVehicleID = ? ";

				var param = command.CreateParameter();
				param.Value = id;
				command.Parameters.Add(param);

				using var reader = command.ExecuteReader();

				if (reader.Read())
				{
					vehicle = this.ReadCompanyVehicle(reader);
					return true;
				}
				else
				{
					vehicle = null;
					return false;
				}
			}
			finally
			{
				connection.Close();
			}
		}

		//L'ID viene generato in SQL (con SCOPE_IDENTITY() o IDENTITY)
		//Il LockingType non ha applicazione concreta: la concorrenza viene gestita dal motore del database(SQL Server).

		/// <inheritdoc cref="IStoreService{TModel}.ResetAndSetLocking"/>
		public void ResetAndSetLocking(LockingTypes lockingType)
		{
			throw new NotSupportedException();
		}
		#endregion

		#region Private Methods
		private static IDataParameter CreateParam(IDbCommand cmd, object value)
		{
			var param = cmd.CreateParameter();
			param.Value = value;
			return param;
		}

		private static VehicleTypes ParseVehicleType(string vehicleType)
		{
			switch (vehicleType.ToUpper())
			{
				case "C":
					return VehicleTypes.Car;
				case "T":
					return VehicleTypes.Truck;
				case "B":
					return VehicleTypes.Cruise;
				case "R":
					return VehicleTypes.Tractor;
				default:
					throw new ArgumentOutOfRangeException(nameof(vehicleType), vehicleType);
			}
		}
		private static string FormatVehicleType(VehicleTypes vehicleType)
		{
			return vehicleType switch
			{
				VehicleTypes.Car => "C",
				VehicleTypes.Truck => "T",
				VehicleTypes.Cruise => "B",
				VehicleTypes.Tractor => "R",
				_ => throw new ArgumentOutOfRangeException(nameof(vehicleType), vehicleType, null)
			};
		}

		private static bool ParseVehicleIsActive(string state)
		{
			switch (state.ToUpper())
			{
				case "O":
					return true;
				case "R":
				case "F":
					return false;
				default:
					throw new ArgumentOutOfRangeException(nameof(state), state);
			}
		}
		private static string FormatVehicleIsActive(bool isActive)
		{
			return isActive ? "O" : "R";
		}
		/// <summary>
		/// Read all vehicle's properties
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public CompanyVehicle ReadCompanyVehicle(IDataReader reader)
		{

			return new CompanyVehicle
			{
				VehicleID = reader.GetInt32(reader.GetOrdinal("CV_CompanyVehicleID")),
				VehicleType = ParseVehicleType(reader.GetString(reader.GetOrdinal("CV_VehicleType"))),
				VehicleYearOfProduction = reader.GetInt32(reader.GetOrdinal("CV_YearOfProduction")),
				VehicleisActive = reader.IsDBNull(reader.GetOrdinal("CV_VehicleState")) ? false : ParseVehicleIsActive(reader.GetString(reader.GetOrdinal("CV_VehicleState"))),
				VehicleKm = reader.IsDBNull(reader.GetOrdinal("CV_VehicleKm")) ? null : reader.GetInt32(reader.GetOrdinal("CV_VehicleKm")),
				VehicleWorkingHours = reader.IsDBNull(reader.GetOrdinal("CV_WorkingHours")) ? null : reader.GetInt32(reader.GetOrdinal("CV_WorkingHours")),
			};

		}

		private static string GetSQLFormattedValue(string? value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return "NULL";
			}
			else
			{
				return $"'{value.Replace("'", "''")}'";
			}
		}

		private static string GetSQLFormattedValue(int? value)
		{
			return value?.ToString(CultureInfo.InvariantCulture) ?? "NULL";
		}

		private static string GetSQLFormattedValue(decimal? value)
		{
			return value?.ToString(CultureInfo.InvariantCulture) ?? "NULL";
		}

		#endregion
	}

}
