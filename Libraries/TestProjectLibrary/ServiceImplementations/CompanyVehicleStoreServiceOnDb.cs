using System.Data;
using System.Data.OleDb;
using System.Diagnostics.CodeAnalysis;
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
	public class CompanyVehicleStoreServiceOnDb(IDatabaseConnectionSettings databaseConnectionSettings) : IStoreService<CompanyVehicle>
	{

		#region Properties
		private IDatabaseConnectionSettings DatabaseConnectionSettings { get; } = databaseConnectionSettings;

		//Oledb è legata strettamente a Windows, su linux o macOS non funziona
		[SupportedOSPlatform("windows")]
		#endregion

		#region ICompanyVehicleStoreService

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
			//crea una lista vuota che conterrà tutti i veicoli 
			var result = new List<CompanyVehicle>();
			//apre una connessione al database
			using var connection = GetDataConnection();
			connection.Open(); //da questo momento in poi posso eseguire comandi SQL perché apro la connessione
			try
			{
				using var command = connection.CreateCommand();
				command.Parameters.Clear();
				command.CommandText = "SELECT * FROM NSCompanyVehicles";
				using var reader = command.ExecuteReader(); //esegue il comando e restituisce un lettore per leggere riga per riga i dati

				while (reader.Read())
				{
					var vehicle = new CompanyVehicle
					{
						VehicleID = reader.GetInt32(reader.GetOrdinal("CV_CompanyVehicleID")),
						VehicleType = Enum.Parse<VehicleTypes>(reader.GetString(reader.GetOrdinal("CV_VehicleType"))),
						VehicleYearOfProduction = reader.GetInt32(reader.GetOrdinal("CV_YearOfProduction")),
						VehicleisActive = reader.GetBoolean(reader.GetOrdinal("CV_VehicleState")),
						VehicleKm = reader.IsDBNull(reader.GetOrdinal("CV_VehicleKm")) ? null : reader.GetDecimal(reader.GetOrdinal("CV_VehicleKm")),
						VehicleWorkingHours = reader.IsDBNull(reader.GetOrdinal("CV_VehicleWorkingHours")) ? null : reader.GetInt32(reader.GetOrdinal("CV_VehicleWorkingHours"))
					};
					result.Add(vehicle);
				}
			}
			finally
			{
				connection.Close();

			}
			return result;
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

				if (model.VehicleID == 0)
				{
					cmd.Parameters.Clear();
					//nuovo veicolo
					//i ? sono segnaposto (placeholder) usati nelle query parametrizzate, vengono sostituiti 
					cmd.CommandText = @"INSERT INTO NSCompanyVehicles ( CV_VehicleType, CV_YearOfProduction, CV_VehicleState, CV_VehicleKm, CV_VehicleWorkingHours) " +
						"VALUES (?, ?, ?, ?, ?);" +
						"SELECT SCOPE_IDENTITY()"; //funzione che permette di ottenere l'ID appena generato dal db

					cmd.Parameters.Add(CreateParam(cmd, model.VehicleType));
					cmd.Parameters.Add(CreateParam(cmd, model.VehicleYearOfProduction));
					cmd.Parameters.Add(CreateParam(cmd, model.VehicleisActive));
					cmd.Parameters.Add(CreateParam(cmd, (object?)model.VehicleKm ?? DBNull.Value));
					cmd.Parameters.Add(CreateParam(cmd, (object?)model.VehicleWorkingHours ?? DBNull.Value));

					var newID = Convert.ToInt32(cmd.ExecuteScalar()); //metodo per leggere il nuovo id
					model.VehicleID = newID;

					transaction.Commit();
					error = null;
					return true;
				}
				else
				{
					cmd.CommandText = "SELECT CV_CompanyVehicleID FROM NSCompanyVehicles " +
						$"WHERE CV_CompanyVehicleID = {model.VehicleID}";
					
					using var reader = cmd.ExecuteReader();

					//verifico che l'id esista
					if (reader.Read())
					{
						reader.Close();
						cmd.Parameters.Clear();
						//aggiorno su db
						cmd.CommandText = @"UPDATE NSCompanyVehicles" +
							"SET CV_VehicleType = ? , CV_YearOfProduction = ?, CV_VehicleState= ?, CV_VehicleKm = ?, CV_VehicleWorkingHours = ? " +
							"WHERE CV_CompanyVehicleID = ?";

						//param devono esssere nello stesso ordine!!
						cmd.Parameters.Add(CreateParam(cmd, model.VehicleType));
						cmd.Parameters.Add(CreateParam(cmd, model.VehicleYearOfProduction));
						cmd.Parameters.Add(CreateParam(cmd, model.VehicleisActive));
						cmd.Parameters.Add(CreateParam(cmd, (object?)model.VehicleKm ?? DBNull.Value));
						cmd.Parameters.Add(CreateParam(cmd, (object?)model.VehicleWorkingHours ?? DBNull.Value));
						cmd.Parameters.Add(CreateParam(cmd, model.VehicleID));

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
		public bool TryRead(int id, [NotNullWhen(true)] out CompanyVehicle? model)
		{

			//crea var dove salvare il risultato letto
			using var connection = GetDataConnection();
			connection.Open();

			try
			{
				using var command = connection.CreateCommand();
				command.Parameters.Clear();
				command.CommandText = $"SELECT CV_CompanyVehicleID, CV_VehicleType, CV_YearOfProduction, CV_VehicleState, CV_VehicleKm, CV_VehicleWorkingHours " +
									  $"FROM NSCompanyVehicles " +
									  $"WHERE CV_CompanyVehicleID = ? ";

				var param = command.CreateParameter();
				param.Value = id;
				command.Parameters.Add(param);

				using var reader = command.ExecuteReader();

				if (reader.Read())
				{
					model = new CompanyVehicle {
						VehicleType = Enum.Parse<VehicleTypes>(reader.GetString(reader.GetOrdinal("CV_VehicleType"))),
						VehicleYearOfProduction = reader.GetInt32(reader.GetOrdinal("CV_YearOfProduction")),
						VehicleisActive = reader.GetBoolean(reader.GetOrdinal("CV_VehicleState")),
						VehicleKm = reader.IsDBNull(reader.GetOrdinal("CV_VehicleKm")) ? null : reader.GetInt32(reader.GetOrdinal("CV_VehicleKm")),
						VehicleWorkingHours = reader.IsDBNull(reader.GetOrdinal("CV_VehicleWorkingHours")) ? null : reader.GetInt32(reader.GetOrdinal("CV_VehicleWorkingHours"))
					};
					return true;
				}
				else
				{
					model = null;
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
		#endregion
	}

}
