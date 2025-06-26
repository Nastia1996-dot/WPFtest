using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;
using TestProjectLibrary.Localization.Models;
using TestProjectLibrary.Models;

namespace TestProjectWebAPI.Controllers
{
	/// <summary>CompanyVehicle</summary>
	/// <response code="500">An internal server error has occurred</response>
	[ApiController]
	[Route("vehicle")]
	[ProducesResponseType<InternalServerErrorInfo>(500, "application/json")]
	public class CompanyVehicleController : ControllerBase
	{

		/// <summary>
		/// CompanyVehicleController
		/// </summary>
		/// <remarks>
		/// Controller
		/// </remarks>
		public CompanyVehicleController()
		{
		}

		private static readonly string[] CompanyVehicleType = CreateVehicleTypes();

		private static string[] CreateVehicleTypes()
		{
			return new[]
			{
				"Car", "Truck", "Cruise", "Tractor"
			};
		}

		private static List<CompanyVehicle> VehicleList =
		[
			new CompanyVehicle(){VehicleID= 1000, VehicleType= "Car", VehicleYearOfProduction= 2005},
			new CompanyVehicle(){VehicleID= 1001, VehicleType= "Truck", VehicleYearOfProduction= 1999},
			new CompanyVehicle(){VehicleID= 1002, VehicleType= "Cruise", VehicleYearOfProduction= 2010},
			new CompanyVehicle(){VehicleID= 1003, VehicleType= "Tractor", VehicleYearOfProduction= 1995},
		];

		/// <summary>
		/// GetVehicleByID
		/// </summary>
		/// <remarks>Get vehicle by its ID</remarks>
		/// <param name="vehicleID">Id of the vehicle to get</param>
		/// <returns></returns>
		/// <response code="200">Get successful</response>
		/// <response code="404">Not found</response>
		[HttpGet("{vehicleID:int}")]
		[ProducesResponseType<CompanyVehicle[]>(200, "application/json")]
		[ProducesResponseType(typeof(NotFoundErrorInfo), 404, "application/json")]

		public IActionResult GetVehicleByID(int vehicleID)
		{
			//veicolo da mostrare
			try
			{
				if (TryFindVehicleByID(vehicleID, out var vehicle))
				{
					return this.Ok(vehicle);
				}

				return this.NotFound(new NotFoundErrorInfo()
				{
					ErrorMessage = string.Format(CompanyVehicleLoc.NotFoundMessageFormat, vehicleID)
				});
			}
			catch
			{
				return this.StatusCode(500, new InternalServerErrorInfo()
				{
					ErrorMessage = string.Format(CompanyVehicleLoc.InternalServerErrorMessage)
				});
			}
		}

		/// <summary>
		/// GetCompanyVehiclesList
		/// </summary>
		/// <remarks>
		/// Get a list of the company vehicles
		/// </remarks>
		/// <response code="200">Get successful</response>
		/// <returns></returns>
		[HttpGet()]
		[ProducesResponseType<CompanyVehicle[]>(200, "application/json")]
		public IActionResult GetCompanyVehiclesList()
		{
			//return this.Ok(Enumerable.Range(1, 4).Select(index => new CompanyVehicle
			//{
			//VehicleID = Random.Shared.Next(1000, 9999),
			//VehicleType = CompanyVehicleType[Random.Shared.Next(CompanyVehicleType.Length)],
			//VehicleYearOfProduction = Random.Shared.Next(1990, 2025)
			//}
			return this.Ok(VehicleList.ToArray());
		}

		/// <summary>
		/// PostCreateOrUpdateVehicle
		/// </summary>
		/// <remarks>
		/// Create a new vehicle if the ID is not present or update an existing one if it exists.
		/// </remarks>
		/// <param name="companyVehicle">The vehicle to create or update </param>
		/// <response code="200">Post succesful</response>
		/// <response code="400">A structure containing validation errors</response>
		/// <response code="404">Not found</response>
		/// <returns></returns>
		[HttpPost()]
		[ProducesResponseType<CompanyVehicle[]>(200, "application/json")]
		[ProducesResponseType(typeof(ValidationError), 400, "application/json")]
		[ProducesResponseType(typeof(NotFoundErrorInfo), 404, "application/json")]
		public IActionResult PostCreateOrUpdateVehicle([FromBody] CompanyVehicle companyVehicle)
		{
			try
			{
				//validazioni comuni
				if (string.IsNullOrWhiteSpace(companyVehicle.VehicleType) || companyVehicle.VehicleType.Length > 50)
				{
					return this.BadRequest(new ValidationError
					{
						PropertyName = nameof(companyVehicle.VehicleType),
						ErrorMessage = string.Format(CompanyVehicleLoc.VehicleTypeRequiredErrorMessage)
					});

				}

				if (companyVehicle.VehicleYearOfProduction < 1900 || companyVehicle.VehicleYearOfProduction > DateTime.Now.Year)
				{
					return this.BadRequest(new ValidationError
					{
						PropertyName = nameof(companyVehicle.VehicleYearOfProduction),
						ErrorMessage = string.Format(CompanyVehicleLoc.VehicleYearOfProductionErrorMessageFormat, DateTime.Now.Year)
					});
				}
				//se non viene inserito l'id allora si procede con l'INSERIMENTO di un nuovo veicolo
				if (companyVehicle.VehicleID == 0)
				{
					//generazione ID fittizia
					var newID = GenerateID();
					companyVehicle.VehicleID = newID;
					VehicleList.Add(companyVehicle);
					return this.Ok(companyVehicle);

				}
				//se viene inserito l'id e il veicolo esiste allora aggiorna i dati

				// Usa TryFindVehicle per verificare se il veicolo esiste
				if (!TryFindVehicleByID(companyVehicle.VehicleID, out var existingVehicle))
				{
					return this.NotFound(new NotFoundErrorInfo
					{
						ErrorMessage = string.Format(CompanyVehicleLoc.NotFoundMessageFormat, companyVehicle.VehicleID)
					});
				}

				// Se il veicolo esiste, aggiornalo
				existingVehicle.VehicleType = companyVehicle.VehicleType;
				existingVehicle.VehicleYearOfProduction = companyVehicle.VehicleYearOfProduction;
				return this.Ok(existingVehicle);
			}
			catch (Exception ex)
			{
				return this.StatusCode(500, new InternalServerErrorInfo()
				{
					ErrorMessage = ex.Message
				});
			}
		}

		/// <summary>
		/// DeleteVehicle
		/// </summary>
		/// <remarks>
		///	Delete a vehicle from the list
		/// </remarks>
		/// <response code="204">Delete successful</response>
		/// <response code="404">Not found</response>
		/// <param name="vehicleID"></param>
		/// <returns></returns>
		[HttpDelete("{vehicleID:int}")]
		[ProducesResponseType<NoContentResult>(204, "application/json")]
		[ProducesResponseType(typeof(NotFound), 404, "application/json")]

		public IActionResult DeleteVehicle(int vehicleID)
		{
			//var vehicle = FindVehicleByID(vehicleID);
			//try
			//{
			//	if (!VehicleExists(vehicleID))
			//	{
			//		return this.NotFound(new NotFoundErrorInfo
			//		{
			//			ErrorMessage = string.Format(CompanyVehicleLoc.NotFoundMessageFormat, vehicleID)
			//		});
			//	}
			//	VehicleList.Remove(vehicle);
			//	return this.NoContent();
			//}
			//catch
			//{
			//	return this.NotFound(new NotFoundErrorInfo
			//	{
			//		ErrorMessage = string.Format(CompanyVehicleLoc.NotFoundMessageFormat, vehicleID)
			//	});
			//}

			try
			{
				if (TryFindVehicleByID(vehicleID, out var vehicle))
				{
					VehicleList.Remove(vehicle);
				}
					return this.Ok(vehicle);
			}
			catch
			{
				return this.NotFound(new NotFoundErrorInfo
				{
					ErrorMessage = string.Format(CompanyVehicleLoc.NotFoundMessageFormat, vehicleID)
				});
			}
		}

		#region PrivateMethods

		private static int newID = VehicleList.Max(v => v.VehicleID);
		private static int GenerateID()
		{
			//Nessun altro thread può interferire mentre il valore viene letto, incrementato e riscritto.
			//L’intera operazione è eseguita in una singola istruzione a basso livello, in modo sicuro e veloce.
			return Interlocked.Increment(ref newID);
		}

		//private static bool VehicleExists(int vehicleID)
		//{
		//	return VehicleList.Any(v => v.VehicleID == vehicleID);
		//}

		//private static CompanyVehicle FindVehicleByID(int vehicleID)
		//{
		//	if (VehicleExists(vehicleID))
		//	{
		//		return VehicleList.FirstOrDefault(v => v.VehicleID == vehicleID);
		//	}
		//	return null;
		//}

		private static bool TryFindVehicleByID(int vehicleID, out CompanyVehicle vehicleFound)
		{
			vehicleFound = VehicleList.FirstOrDefault(v => v.VehicleID == vehicleID);
			return vehicleFound != null;
		}

		#endregion

	}
}
