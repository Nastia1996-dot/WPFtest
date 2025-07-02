using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Xml.Linq;
using TestProjectTester.TestProjectAPI;

namespace TestProjectTester
{
	internal class Program
	{
		private static readonly TextWriter output = Console.Out;
		static async Task Main(string[] args)
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
			catch (ApiException apiEx)
			{
				ApiFailureErrorMessage(apiEx, client, output);
			}
			catch (Exception ex)
			{
				GlobalFailureErrorMessage(ex, client, output);
			}
			//aspetta che l'utente prema un tasto prima di chiudere la finestra, altrimenti si chiude immediatamente
			Console.ReadKey();
		}

		#region Metodi

		private static async Task<bool> MainMenuAsync(TestAPIClient client, TextWriter output)
		{
			//pulisce la console
			Console.Clear();

			Console.WriteLine("==========VEHICLE MENU==========");
			AddSpace();
			Console.WriteLine("1. Read vehicle by ID");
			Console.WriteLine("2. List all vehicles");
			Console.WriteLine("3. Create or update vehicle");
			Console.WriteLine("4. Delete vehicle");
			Console.WriteLine("5. Exit");
			AddSpace();
			SelectAnOptionBetween(1, 5, out var choice);

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
					Console.WriteLine("Goodbye!");
					return false;
				default:
					Console.WriteLine("Invalid option: choose another one");
					break;
			}
			return true;
		}

		private static void BackToMenu(TestAPIClient client, TextWriter output)
		{
			output.WriteLine();
			output.WriteLine("Press any key to go back to the Main menu");
			Console.ReadKey(true);
		}
		private static void PrintTestTitle(string methodName)
		{
			output.WriteLine();
			output.WriteLine($"-------Test of {methodName}-------");
			output.WriteLine();
			output.WriteLine();
		}

		private static void MethodDescription(string description)
		{
			output.WriteLine(description);
			output.WriteLine();
			output.WriteLine();
		}

		private static void MethodInstructions(string instructions)
		{
			output.WriteLine();
			output.Write(instructions);
			
		}

		private static void SuccessfulMessage(TestAPIClient client, TextWriter output)
		{
			output.WriteLine();
			output.WriteLine("Successful read test");
			output.WriteLine();
			output.WriteLine();
		}

		private static int SelectAnOptionBetween(int firstOption, int lastOption, out int result)
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
		private static void VehicleCreatedMessage()
		{
			output.WriteLine("New vehicle created successfully:");
		}

		private static bool TryRequestVehicleIDForUpdate(out int? vechicleID)
		{
			vechicleID = null;

			GetUserInput(out var idInput);

			//se il campo id è stato compilato prosegui con le verifiche di SINTASSI (alle altre pensa già il server)
			if (string.IsNullOrWhiteSpace(idInput))
			{
				return false;
			}

			//verifica che il carattere inserito sia numerico
			if (!int.TryParse(idInput, out int parsedID))
			{
				output.WriteLine("Invalid ID. Please enter a numeric value.");
				return false;
			}

			vechicleID = parsedID;
			return true;
		}

		private static async Task ShowAllVehiclesAsync(TestAPIClient client)
		{
			AddSpace();
			//il metodo client è asincrono, perciò lo converto in sincrono per farlo funzionare qui
			var vehicles = await client.VehicleAllAsync();
			if (vehicles != null)
			{

				foreach (var v in vehicles)
				{
					output.WriteLine($"{v.VehicleID}: {v.VehicleType} - {v.VehicleYearOfProduction}");
				}
			}
		}
		private static void VehicleUpdatedMessage()
		{
			output.WriteLine("Vehicle updated successfully:");
		}
		private static void ApiFailureErrorMessage(ApiException apiEx, TestAPIClient client, TextWriter output)
		{
			output.WriteLine();
			Console.Error.WriteLine($"Api failure error: {apiEx.Message}");
			output.WriteLine();
		}

		private static void GlobalFailureErrorMessage(Exception ex, TestAPIClient client, TextWriter output)
		{
			output.WriteLine();
			Console.Error.WriteLine($"Global failure error: {ex.Message}");
			output.WriteLine();
		}

		private static void NotFoundErrorMessage(ApiException<ErrorResponse> notFound, TestAPIClient client, TextWriter output)
		{
			output.WriteLine();
			output.WriteLine("Managed error message:");
			output.WriteLine(notFound.Result.Message);
			output.WriteLine();
			output.WriteLine();
		}

		private static void GenericNotFoundErrorMessage(ApiException<ErrorResponse> notFound, TestAPIClient client, TextWriter output)
		{
			output.WriteLine();
			output.WriteLine("Generic error message:");
			output.WriteLine(notFound.Message);
			output.WriteLine();
			output.WriteLine();
		}

		private static void AddSpace()
		{
			output.WriteLine();
		}

		//-------------GET-----------------------
		private static async Task VehicleGetAsync(TestAPIClient client, TextWriter output)
		{
			output.WriteLine();
			output.WriteLine("Choose if you want to proceed with:");
			output.WriteLine("1. Manual test");
			output.WriteLine("2. Automatic test");
			output.WriteLine("3. Back");
			AddSpace();
			SelectAnOptionBetween(1, 3, out var result);

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
						Console.WriteLine("Invalid option: choose another one");
						break;
				}
			}
			catch (ApiException<ErrorResponse> notFound)
			{
				GenericNotFoundErrorMessage(notFound, client, output);
			}
		}


		private static void GetManualTest(TestAPIClient client, TextWriter output)
		{
			AddSpace();
			output.WriteLine("------MANUAL TEST------");

			PrintTestTitle("VehicleGet");

			MethodDescription("User input read test.");

			MethodInstructions("Please insert vehicle ID to read: ");

			var vehicleID = Convert.ToInt32(Console.In.ReadLine()?.TrimEnd());
			try
			{;
				var vehicle = client.VehicleGET(vehicleID);
				output.WriteLine($"ID: {vehicle.VehicleID} ");
				output.WriteLine($"Type: {vehicle.VehicleType}");
				output.WriteLine($"Year of production: {vehicle.VehicleYearOfProduction}");
				output.WriteLine($"Is active: {vehicle.VehicleisActive}");
				if (vehicle.VehicleType == VehicleTypes.Car || vehicle.VehicleType == VehicleTypes.Truck)
				{
					output.WriteLine($"Km: {vehicle.VehicleKm}");
				}
				if (vehicle.VehicleType == VehicleTypes.Cruise || vehicle.VehicleType == VehicleTypes.Tractor)
				{
					output.WriteLine($"Working hours: {vehicle.VehicleWorkingHours}");
				}
				SuccessfulMessage(client, output);
			}
			catch (ApiException<ErrorResponse> notFound)
			{
				NotFoundErrorMessage(notFound, client, output);
			}
		}

		private static async Task GetAutomaticTestAsync(TestAPIClient client, TextWriter output)
		{
			AddSpace();
			output.WriteLine("------AUTOMATIC TEST------");

			output.WriteLine("Results of Input: 1000, 1001, 1002, 1003");
			AddSpace();

			await ShowAllVehiclesAsync(client);

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
				NotFoundErrorMessage(notFound, client, output);
			}
		}

		//-----------LIST GET----------------
		private static async Task VehicleListGETAsync(TestAPIClient client, TextWriter output)
		{
			PrintTestTitle("VehicleListGET");
			MethodDescription("Find and print all vehicles in the system.");
			MethodInstructions("No input required. The complete list will be printed:");
			AddSpace();

			try
			{
				await ShowAllVehiclesAsync(client);
				SuccessfulMessage(client, output);
			}
			catch (ApiException<ErrorResponse> notFound)
			{
				GenericNotFoundErrorMessage(notFound, client, output);
			}
		}

		//----------POST----------------

		private static async Task VehiclePOSTAsync(TestAPIClient client, TextWriter output)
		{
			PrintTestTitle("VehiclePOST");
			MethodDescription("Create or update a vehicle.");
			MethodInstructions("Leave ID empty to create a new vehicle, or insert an existing ID to update it.");
			AddSpace();
			AddSpace();

			Console.Write("Vehicle ID: ");
			bool isUpdate = TryRequestVehicleIDForUpdate(out int? vehicleID);

			AddSpace();
			Console.Write("Vehicle's type: ");
			GetUserInput(out var typeInput);

			if (!Enum.TryParse<VehicleTypes>(typeInput, true, out var type))
			{
				output.WriteLine("Invalid vehicle type. Accepted types: Car, Truck, Cruise, Tractor.");
				return;
			}

			AddSpace();
			Console.Write("Vehicle's year of production: ");
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
			//creo oggetto e chiamo API
			var vehicle = new CompanyVehicle
			{
				VehicleID = vehicleID ?? 0, // se è null, la API lo interpreta come nuovo
				VehicleType = type,
				VehicleYearOfProduction = year,
			};

			var result = await client.VehiclePOSTAsync(vehicle);
			//se il campo ID non è stato compilato prosegui con la creazione di un nuovo veicolo
			if (isUpdate)
			{
				VehicleUpdatedMessage();
			}
			else
			{
				VehicleCreatedMessage();
			}

			output.WriteLine($"{result.VehicleID}: {result.VehicleType} - {result.VehicleYearOfProduction}");

		}

		private static async Task VehicleDELETEAsync(TestAPIClient client, TextWriter output)
		{
			PrintTestTitle("VehicleDELETE");
			MethodDescription("Delete an existing vehicle.");
			MethodInstructions("Insert an existing ID to delete the vehicle.");
			AddSpace();
			AddSpace();

			Console.Write("Vehicle ID: ");
			GetUserInput(out var idInput);
			if (!int.TryParse(idInput, out int id))
			{
				output.WriteLine("Invalid ID format. Please enter a numeric value.");
				return;
			}
			try
			{
				await client.VehicleDELETEAsync(id);
				output.WriteLine();
				output.WriteLine($"Vehicle with ID {id} deleted successfully.");
				output.WriteLine();
			}
			catch (ApiException<ErrorResponse> notFound)
			{
				NotFoundErrorMessage(notFound, client, output);
			}
		}
	}

}
#endregion
