using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using TestProjectTester.TestProjectAPI;

namespace TestProjectTester
{
	internal class Program
	{
		private static StreamWriter? Log;
		private static StreamWriter? CurrentLog;
		private static readonly object LockObj = new();
		static async Task Main(string[] args)
		{
			using (Log = new StreamWriter("test.log", true, Encoding.UTF8))
			using (CurrentLog = new StreamWriter("currentTest.log", false, Encoding.UTF8))
			{
				//using serve a garantire che lo stream di output venga rilasciato correttamente quando finisce il blocco 
				using var output = Console.Out;

				//HttpClient implementa IDisposable, quindi va chiuso con using per evitare problemi di connessioni lasciate aperte
				using var httpClient = new HttpClient();

				//oggetto che rappresenta la mia Web API lato client
				var client = new TestAPIClient("https://localhost:7213", httpClient);



				try
				{
					//appena running diventa false esco dal ciclo e il programma termina
					while (await MainMenuAsync(client, output))
					{
						BackToMenu(client, output);
					}
				}
				catch (ApiException<ErrorResponse> notFound)
				{
					ManageErrorResponse(notFound, client, output);
				}
				catch (ApiException apiEx)
				{
					ApiFailureErrorMessage(apiEx, client, output);
				}
				catch (Exception ex)
				{
					GlobalFailureErrorMessage(ex, client, output);
				}

			}
			//aspetta che l'utente prema un tasto prima di chiudere la finestra, altrimenti si chiude immediatamente
			Console.ReadKey();

		}

		#region Metodi

		private static async Task<bool> MainMenuAsync(TestAPIClient client, TextWriter output)
		{
			//pulisce la console
			Console.Clear();

			output.WriteLine("==========VEHICLE MENU==========");
			AddSpace();
			output.WriteLine("1. Read vehicle by ID");
			output.WriteLine("2. List all vehicles");
			output.WriteLine("3. Create or update vehicle");
			output.WriteLine("4. Delete vehicle");
			output.WriteLine("5. Multithreaded test");
			output.WriteLine("6. Exit");
			AddSpace();
			SelectAnOptionBetween(output, 1, 6, out var choice);

			AddSpace();

			switch (choice)
			{
				case 1:
					await VehicleGetAsync(client, output);
					break;
				case 2:
					await VehicleListGETAsync(client, output);
					break;
				case 3:
					await VehiclePOSTAsync(client, output);
					break;
				case 4:
					await VehicleDELETEAsync(client, output);
					break;
				case 5:
					await MultiThreadedTestAsync(client, output);
					break;
				case 6:
					output.WriteLine("Goodbye!");
					return false;
				default:
					output.WriteLine("Invalid option: choose another one");
					break;
			}
			return true;
		}

		private static void BackToMenu(TestAPIClient client, TextWriter output)
		{
			AddSpace();
			output.WriteLine("Press any key to go back to the Main menu");
			Console.ReadKey(true);
		}
		private static void PrintTestTitle(TextWriter output, string methodName)
		{
			AddSpace();
			output.WriteLine($"-------Test of {methodName}-------");
			AddSpace();
			AddSpace();
		}

		private static void MethodDescription(TextWriter output, string description)
		{
			output.WriteLine(description);
			AddSpace();
			AddSpace();
		}

		private static void MethodInstructions(TextWriter output, string instructions)
		{
			AddSpace();
			output.Write(instructions);

		}

		private static void SuccessfulMessage(TestAPIClient client, TextWriter output)
		{
			AddSpace();
			output.WriteLine("Successful read test");
			AddSpace();
			AddSpace();
		}

		private static int SelectAnOptionBetween(TextWriter output, int firstOption, int lastOption, out int result)
		{
			output.Write($"Select an option between {firstOption} and {lastOption}: ");
			GetUserInput(out var input);
			bool success = int.TryParse(input, out result);
			if (!success || result < firstOption || result > lastOption)
			{
				output.WriteLine("Invalid input. Please enter a valid number within the range");
			}
			return 0;

		}
		private static void GetUserInput(out string? input)
		{
			input = Console.ReadLine();
		}
		private static void VehicleCreatedMessage(TextWriter output)
		{
			output.WriteLine("New vehicle created successfully:");
		}

		private static bool TryRequestVehicleIDForUpdate(out int? vehicleID)
		{
			vehicleID = null;

			GetUserInput(out var idInput);

			//se il campo id è stato compilato prosegui con le verifiche di SINTASSI (alle altre pensa già il server)
			if (string.IsNullOrWhiteSpace(idInput))
			{
				return false;
			}

			//verifica che il carattere inserito sia numerico
			if (!int.TryParse(idInput, out int parsedID))
			{
				Console.WriteLine("Invalid ID. Please enter a numeric value.");
				return false;
			}

			vehicleID = parsedID;
			return true;
		}

		private static async Task ShowAllVehiclesAsync(TextWriter output, TestAPIClient client)
		{
			AddSpace();
			//il metodo client è asincrono, perciò lo converto in sincrono per farlo funzionare qui
			var vehicles = await client.VehicleAllAsync();
			if (vehicles != null)
			{

				foreach (var v in vehicles)
				{
					output.WriteLine($"ID: {v.VehicleID} ");
					output.WriteLine($"Type: {v.VehicleType}");
					output.WriteLine($"Year of production: {v.VehicleYearOfProduction}");
					output.WriteLine($"Is active: {v.VehicleisActive}");
					if (v.VehicleType.IsKmRequired())
					{
						output.WriteLine($"Km: {v.VehicleKm}");
					}
					if (v.VehicleType.IsWorkinHoursRequired())
					{
						output.WriteLine($"Working hours: {v.VehicleWorkingHours}");
					}
					AddSpace();
					AddSpace();
				}
			}
		}

		private static void PrintVehicleDetails(CompanyVehicle result, TextWriter output)
		{
			AddSpace();
			output.WriteLine($"ID: {result.VehicleID} ");
			output.WriteLine($"Type: {result.VehicleType}");
			output.WriteLine($"Year of production: {result.VehicleYearOfProduction}");
			output.WriteLine($"Is active: {result.VehicleisActive}");

			if (result.VehicleType.IsKmRequired())
			{
				output.WriteLine($"Km: {result.VehicleKm}");
			}
			if (result.VehicleType.IsWorkinHoursRequired())
			{
				output.WriteLine($"Working hours: {result.VehicleWorkingHours}");
			}
		}

		private static void VehicleUpdatedMessage(TextWriter output)
		{
			output.WriteLine("Vehicle updated successfully:");
		}

		private static void ApiFailureErrorMessage(ApiException apiEx, TestAPIClient client, TextWriter output)
		{
			AddSpace();
			Console.Error.WriteLine($"Api failure error: {apiEx.Message}");
			AddSpace();
		}

		private static void GlobalFailureErrorMessage(Exception ex, TestAPIClient client, TextWriter output)
		{
			AddSpace();
			Console.Error.WriteLine($"Global failure error: {ex.Message}");
			AddSpace();
		}

		private static void ManageErrorResponse(ApiException<ErrorResponse> errorResponseEx, TestAPIClient client, TextWriter output)
		{
			AddSpace();
			switch (errorResponseEx.Result.Status)
			{
				case 400:
					output.WriteLine("Bad Request: ");
					if (errorResponseEx.Result.ValidationErrors != null && errorResponseEx.Result.ValidationErrors.Count > 0)
					{
						foreach (var validationError in errorResponseEx.Result.ValidationErrors)
						{
							output.WriteLine($" - {validationError.PropertyName}: {validationError.ErrorMessage}");
						}
					}
					else
					{
						output.WriteLine(errorResponseEx.Result.Message);
					}
					break;
				case 404:
					output.WriteLine("Not found:");
					output.WriteLine(errorResponseEx.Result.Message);
					break;
				default:
					output.WriteLine($"API error (status {errorResponseEx.Result.Status}):");
					output.WriteLine(errorResponseEx.Result.Message);
					break;
			}
			AddSpace();
			AddSpace();
		}

		private static void AddSpace()
		{
			Console.WriteLine();
		}

		//-------------GET-----------------------
		private static async Task VehicleGetAsync(TestAPIClient client, TextWriter output)
		{
			AddSpace();
			output.WriteLine("Choose if you want to proceed with:");
			output.WriteLine("1. Manual test");
			output.WriteLine("2. Automatic test");
			output.WriteLine("3. Back");
			AddSpace();
			SelectAnOptionBetween(output, 1, 3, out var result);

			try
			{
				switch (result)
				{
					case 1:
						GetManualTest(client, output);
						break;
					case 2:
						await GetAutomaticTestAsync(client, output);
						break;
					case 3:
						await MainMenuAsync(client, output);
						break;
					default:
						output.WriteLine("Invalid option: choose another one");
						break;
				}
			}
			catch (ApiException<ErrorResponse> notFound)
			{
				ManageErrorResponse(notFound, client, output);
			}
		}

		private static void UpdateLogFile(string message)
		{
			Console.WriteLine(message);
			lock (LockObj)
			{
				Log?.WriteLine(message);
				Log?.Flush();
				CurrentLog?.WriteLine(message);
				CurrentLog?.Flush();
			}
		}
		private static void GetManualTest(TestAPIClient client, TextWriter output)
		{
			AddSpace();
			output.WriteLine("------MANUAL TEST------");

			PrintTestTitle(output, "VehicleGet");

			MethodDescription(output, "User input read test.");

			MethodInstructions(output, "Please insert vehicle ID to read: ");

			var vehicleID = Convert.ToInt32(Console.In.ReadLine()?.TrimEnd());
			try
			{
				;
				var result = client.VehicleGET(vehicleID);
				PrintVehicleDetails(result, output);
				SuccessfulMessage(client, output);
			}
			catch (ApiException<ErrorResponse> notFound)
			{
				ManageErrorResponse(notFound, client, output);
			}
		}

		private static async Task GetAutomaticTestAsync(TestAPIClient client, TextWriter output)
		{
			AddSpace();
			output.WriteLine("------AUTOMATIC TEST------");

			output.WriteLine("Results of Input: 1000, 1001, 1002, 1003");
			AddSpace();

			await ShowAllVehiclesAsync(output, client);

			try
			{
				AddSpace();
				output.WriteLine("Result of Input: 101");
				AddSpace();
				var vehicle = client.VehicleGET(101);
				output.WriteLine($"{vehicle.VehicleID}: {vehicle.VehicleType} - {vehicle.VehicleYearOfProduction}");
				AddSpace();
			}
			catch (ApiException<ErrorResponse> notFound)
			{
				ManageErrorResponse(notFound, client, output);
			}
		}

		//-----------LIST GET----------------
		private static async Task VehicleListGETAsync(TestAPIClient client, TextWriter output)
		{
			PrintTestTitle(output, "VehicleListGET");
			MethodDescription(output, "Find and print all vehicles in the system.");
			MethodInstructions(output, "No input required. The complete list will be printed:");
			AddSpace();

			try
			{
				await ShowAllVehiclesAsync(output, client);
				SuccessfulMessage(client, output);
			}
			catch (ApiException<ErrorResponse> notFound)
			{
				ManageErrorResponse(notFound, client, output);
			}
		}


		//----------POST----------------

		private static async Task VehiclePOSTAsync(TestAPIClient client, TextWriter output)
		{
			PrintTestTitle(output, "VehiclePOST");
			MethodDescription(output, "Create or update a vehicle.");
			MethodInstructions(output, "Leave ID empty to create a new vehicle, or insert an existing ID to update it.");
			AddSpace();
			AddSpace();

			output.Write("Vehicle ID: ");
			bool isUpdate = TryRequestVehicleIDForUpdate(out int? vehicleID);

			AddSpace();
			output.Write("Vehicle's type: ");
			GetUserInput(out var typeInput);

			if (!Enum.TryParse<VehicleTypes>(typeInput, true, out var type))
			{
				output.WriteLine("Invalid vehicle type. Accepted types: Car, Truck, Cruise, Tractor.");
				return;
			}


			AddSpace();
			output.Write("Vehicle's year of production: ");
			GetUserInput(out var yearInput);
			AddSpace();
			if (!int.TryParse(yearInput, out var year))
			{
				output.WriteLine("Invalid year format. Please enter a numeric year.");
				return;
			}
			int currentYear = DateTime.Now.Year;
			if (year < 1900 || year > currentYear)
			{
				output.WriteLine($"Invalid year. Year must be between 1900 and {currentYear}.");
				return;
			}

			AddSpace();
			output.WriteLine("Is vehicle active: ");
			output.WriteLine("1. True ");
			output.WriteLine("2. False ");
			SelectAnOptionBetween(output, 1, 2, out int activeChoice);

			bool isActive = activeChoice == 1;

			if (activeChoice != 1 && activeChoice != 2)
			{
				output.WriteLine("Invalid choice. Choose between 1 and 2");
				return;
			}

			AddSpace();
			double vehicleKm = 0;
			int vehicleWorkingHours = 0;

			if (type.IsKmRequired())
			{
				output.WriteLine("Vehicle's kilometers (must be > 0) ");
				GetUserInput(out var kmInput);
				if (!double.TryParse(kmInput, out vehicleKm) || vehicleKm < 0)
				{
					output.WriteLine("Invalid km. Must be a number greater than 0");
					return;
				}
				//se il veicolo aggiornato ha il campo working hours è incompatibile e viene azzerato in favore del campo km
				if (isUpdate)
				{
					vehicleWorkingHours = 0;
				}
			}
			if (type.IsWorkinHoursRequired())
			{
				output.Write("Enter working hours (must be > 0): ");
				GetUserInput(out var hoursInput);
				if (!int.TryParse(hoursInput, out vehicleWorkingHours) || vehicleWorkingHours <= 0)
				{
					output.WriteLine("Invalid working hours. Must be an integer greater than 0.");
					return;
				}
				if (isUpdate)
				{
					vehicleKm = 0;
				}
			}

			//creo oggetto e chiamo API
			var vehicle = new CompanyVehicle
			{
				VehicleID = vehicleID ?? 0, // se è null, la API lo interpreta come nuovo
				VehicleType = type,
				VehicleYearOfProduction = year,
				VehicleisActive = isActive,
				VehicleKm = vehicleKm,
				VehicleWorkingHours = vehicleWorkingHours,
			};
			try
			{
				var result = await client.VehiclePOSTAsync(vehicle);
				//se il campo ID non è stato compilato prosegui con la creazione di un nuovo veicolo
				if (isUpdate)
				{
					VehicleUpdatedMessage(output);
				}
				else
				{
					VehicleCreatedMessage(output);
				}

				PrintVehicleDetails(result, output);
			}
			catch (ApiException<ErrorResponse> errorEx) when (errorEx.Result.Status == 400 || errorEx.Result.Status == 404)
			{
				ManageErrorResponse(errorEx, client, output);
			}
		}

		private static async Task VehicleDELETEAsync(TestAPIClient client, TextWriter output)
		{
			PrintTestTitle(output, "VehicleDELETE");
			MethodDescription(output, "Delete an existing vehicle.");
			MethodInstructions(output, "Insert an existing ID to delete the vehicle.");
			AddSpace();
			AddSpace();

			output.Write("Vehicle ID: ");
			GetUserInput(out var idInput);
			if (!int.TryParse(idInput, out int id))
			{
				output.WriteLine("Invalid ID format. Please enter a numeric value.");
				return;
			}
			try
			{
				await client.VehicleDELETEAsync(id);
				AddSpace();
				output.WriteLine($"Vehicle with ID {id} deleted successfully.");
				AddSpace();
			}
			catch (ApiException<ErrorResponse> notFound)
			{
				ManageErrorResponse(notFound, client, output);
			}
		}

		//far scegliere all'utente quanti thread lanciare e quanti tentativi per ciascuno.
		//in output devo avere i risultati.
		//dunque i risultati devono comparire sia in console che su file.log usando la classe stream.writer.
		private static async Task MultiThreadedTestAsync(TestAPIClient client, TextWriter output)
		{
			Console.WriteLine("How many thread do you want to launch?");
			GetUserInput(out string? threadInput);
			if (!int.TryParse(threadInput, out int numOfThreads) || numOfThreads <= 0)
			{
				Console.WriteLine("Invalid input format. Please enter a numeric value greater than 0.");
				return;
			}
			Console.WriteLine("How many attempts per thread do you want to set?");
			GetUserInput(out string? attemptsInput);
			if (!int.TryParse(attemptsInput, out int numOfAttempts) || numOfAttempts <= 0)
			{
				Console.WriteLine("Invalid input format. Please enter a numeric value greater than 0.");
				return;
			}

			UpdateLogFile($"Starting current test: {numOfThreads} threads, {numOfAttempts} attempts");

			//crea una lista che conterrà i thread
			var tasks = new List<Task>();

			var startedAt = DateTime.Now;
			var timer = new Stopwatch();
			timer.Start();
			for (int i = 0; i < numOfThreads; i++)
			{
				tasks.Add(DoSomeWorkAsync(client, i, numOfAttempts));
			}
			//aspetta che tutti i thread abbiano finito di lavorare prima di proseguire
			Task.WaitAll(tasks.ToArray());
			timer.Stop();
			UpdateLogFile($"All threads completed in DateTime: {DateTime.Now.Subtract(startedAt)}");
			UpdateLogFile($"All threads completed in DateTime: {timer.Elapsed}");
			var vehicles = await client.VehicleAllAsync();
			UpdateLogFile($"Number of vehicles present: {vehicles.Count}");
		}

		private static async Task DoSomeWorkAsync(TestAPIClient client, int threadId, int attempts)
		{
			//ogni thread fa un ciclo pari al numero di tentativi
			for (int i = 0; i < attempts; i++)
			{
				var vehicle = new CompanyVehicle
				{
					VehicleID = 0,
					VehicleType = VehicleTypes.Car,
					VehicleYearOfProduction = 2006,
					VehicleisActive = true,
					VehicleKm = 1200,
					VehicleWorkingHours = null,
				};

				try
				{
					var result = await client.VehiclePOSTAsync(vehicle);
					string message = $"Thread {threadId} - Attempt {i + 1} - Vehicle ID created: {result.VehicleID}";
					UpdateLogFile(message);
				}
				catch (ApiException<ErrorResponse> ex)
				{
					string errorMsg = $"Thread {threadId} - Attempt {i + 1} - Error: {ex.Result.Message}";
					UpdateLogFile(errorMsg);
				}
			}
		}



	}

}
#endregion
