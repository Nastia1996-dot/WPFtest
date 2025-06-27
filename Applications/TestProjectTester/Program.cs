using System.Text;
using TestProjectTester.TestProjectAPI;

namespace TestProjectTester
{
	internal class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var carType = Enum.Parse<VehicleTypes>("car", true);

				using var output = Console.Out;
				using var httpClient = new HttpClient();
				var client = new TestAPIClient("http://localhost:5000", httpClient);
				TestVehicleGet(client, output);
				output.WriteLine();
				output.WriteLine();
				output.WriteLine();
				output.WriteLine("Press any key to exit");
			}
			catch (ApiException apiEx)
			{
				Console.Error.WriteLine($"Api failure error: {apiEx.Message}");
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine($"Global failure error: {ex.Message}");
			}
			Console.ReadKey();
		}

		#region Metodi

		private static void TestVehicleGet(TestAPIClient client, TextWriter output)
		{
			output.WriteLine("Test of VehicleGET");
			output.WriteLine();
			output.WriteLine();


			output.WriteLine("User input read test");
			output.WriteLine();
			output.WriteLine();
			output.Write("Please insert vechileID to read: ");
			var vehicleID = Convert.ToInt32(Console.In.ReadLine()?.TrimEnd());
			try
			{
				var vehicle = client.VehicleGET(vehicleID);
				output.WriteLine($"{vehicle.VehicleID}: {vehicle.VehicleType} - {vehicle.VehicleYearOfProduction}");
			}
			catch (ApiException<NotFoundErrorInfo> notFound)
			{
				output.WriteLine("Managed error message:");
				output.WriteLine(notFound.Result.ErrorMessage);
				output.WriteLine();
				output.WriteLine();
				output.WriteLine("Generic error message:");
				output.WriteLine(notFound.Message);
			}


			output.WriteLine("Successful read test");
			output.WriteLine();
			output.WriteLine();
			try
			{
				var vehicle = client.VehicleGET(1001);
				output.WriteLine($"{vehicle.VehicleID}: {vehicle.VehicleType} - {vehicle.VehicleYearOfProduction}");
			}
			catch (ApiException<NotFoundErrorInfo> notFound)
			{
				output.WriteLine("Managed error message:");
				output.WriteLine(notFound.Result.ErrorMessage);
				output.WriteLine();
				output.WriteLine();
				output.WriteLine("Generic error message:");
				output.WriteLine(notFound.Message);
			}



			output.WriteLine("NotFound read test");
			output.WriteLine();
			output.WriteLine();
			try
			{
				var vehicle = client.VehicleGET(101);
				output.WriteLine($"{vehicle.VehicleID}: {vehicle.VehicleType} - {vehicle.VehicleYearOfProduction}");
			}
			catch (ApiException<NotFoundErrorInfo> notFound)
			{
				output.WriteLine("Managed error message:");
				output.WriteLine(notFound.Result.ErrorMessage);
				output.WriteLine();
				output.WriteLine();
				output.WriteLine("Generic error message:");
				output.WriteLine(notFound.Message);
			}
		}

		#endregion
	}
}
