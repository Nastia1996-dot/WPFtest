using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using TestProjectLibrary.Localization.Models;
using TestProjectLibrary.Models;
using TestProjectLibrary.Models.Enums;

namespace TestProjectWebAPI.Controllers
{

	/// <summary>CompanyVehicle</summary>
	/// <response code="500">An internal server error has occurred</response>
	[ApiController]
	[Route("vehicle")]
	[ProducesResponseType<ErrorResponse>(500, "application/json")]

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


		//private static object VehicleLocker = new();

		private static ConcurrentDictionary<int, CompanyVehicle> VehicleDictionary = new ConcurrentDictionary<int, CompanyVehicle>(new Dictionary<int, CompanyVehicle>()
		{
			{ 996, new CompanyVehicle() { VehicleID = 996, VehicleType = VehicleTypes.Car, VehicleYearOfProduction = 2005, VehicleisActive = true, VehicleKm = 12500 } },
			{ 997, new CompanyVehicle() { VehicleID = 997, VehicleType = VehicleTypes.Truck, VehicleYearOfProduction = 1999, VehicleisActive = true, VehicleKm = 100000 } },
			{ 998, new CompanyVehicle() { VehicleID = 998, VehicleType = VehicleTypes.Cruise, VehicleYearOfProduction = 2010, VehicleisActive = false, VehicleWorkingHours = 2500 } },
			{ 999, new CompanyVehicle() { VehicleID = 999, VehicleType = VehicleTypes.Tractor, VehicleYearOfProduction = 1995, VehicleisActive = true, VehicleWorkingHours = 3450 } },
		});


		/// <summary>
		/// GetVehicleByID
		/// </summary>
		/// <remarks>Get vehicle by its ID</remarks>
		/// <param name="vehicleID">Id of the vehicle to get</param>
		/// <returns></returns>
		/// <response code="200">Get successful</response>
		/// <response code="404">Not found</response>
		[HttpGet("{vehicleID:int}")]
		[ProducesResponseType<CompanyVehicle>(200, "application/json")]
		[ProducesResponseType<ErrorResponse>(404, "application/json")]
		public IActionResult GetVehicleByID(int vehicleID)
		{
			try
			{
				if (VehicleDictionary.TryGetValue(vehicleID, out var vehicle))
				{
					return this.Ok(vehicle);
				}
				else
				{

					return this.NotFound(new ErrorResponse().SetNotFound(string.Format(CompanyVehicleLoc.NotFoundMessageFormat)));
				}
			}
			catch (Exception ex)
			{
				return this.StatusCode(500, new ErrorResponse().SetInternalServerErrorInfo(ex.Message));

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
			//lock (VehicleLocker)
			{
				return this.Ok(from item in VehicleDictionary.Values
							   orderby item.VehicleID
							   select item);
			}
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
		[ProducesResponseType<CompanyVehicle>(200, "application/json")]
		[ProducesResponseType(typeof(ErrorResponse), 400, "application/json")]
		[ProducesResponseType(typeof(ErrorResponse), 404, "application/json")]
		public IActionResult PostCreateOrUpdateVehicle([FromBody] CompanyVehicle companyVehicle)
		{
			try
			{
				var errors = new List<ValidationError>();
				//validazioni comuni

				if (companyVehicle.VehicleYearOfProduction < 1900 || companyVehicle.VehicleYearOfProduction > DateTime.Now.Year)
				{
					errors.Add(new ValidationError
					{
						PropertyName = nameof(companyVehicle.VehicleYearOfProduction),
						ErrorMessage = string.Format(CompanyVehicleLoc.VehicleYearOfProductionErrorMessageFormat, DateTime.Now.Year)
					});
				}
				if (!companyVehicle.VehicleisActive)
				{
					errors.Add(new ValidationError
					{
						PropertyName = nameof(companyVehicle.VehicleisActive),
						ErrorMessage = string.Format(CompanyVehicleLoc.VehicleIsActiveErrorMessageFormat)
					});
				}
				if (companyVehicle.VehicleType == VehicleTypes.Car || companyVehicle.VehicleType == VehicleTypes.Truck)
				{
					if (!companyVehicle.VehicleKm.HasValue || companyVehicle.VehicleKm <= 0)
					{
						errors.Add(new ValidationError
						{
							PropertyName = nameof(companyVehicle.VehicleKm),
							ErrorMessage = string.Format(CompanyVehicleLoc.VehicleKmErrorMessageFormat)
						});
					}
				}
				if (companyVehicle.VehicleType == VehicleTypes.Cruise || companyVehicle.VehicleType == VehicleTypes.Tractor)
				{
					if (!companyVehicle.VehicleWorkingHours.HasValue || companyVehicle.VehicleWorkingHours <= 0)
					{
						errors.Add(new ValidationError
						{
							PropertyName = nameof(companyVehicle.VehicleWorkingHours),
							ErrorMessage = string.Format(CompanyVehicleLoc.VehicleWorkingHoursErrorMessageFormat)
						});
					}
				}
				//se ci sono errori, li restituisco tutti insieme
				if (errors.Count != 0)
				{
					return this.BadRequest(new ErrorResponse().SetValidationErrors(errors));
				}
				//se non viene inserito l'id allora si procede con l'INSERIMENTO di un nuovo veicolo
				if (companyVehicle.VehicleID == 0)
				{
					//generazione ID fittizia
					var newID = GenerateID();
					companyVehicle.VehicleID = newID;
					//TODO: se non riesce la TryAdd deve assegnare nuovo id e riprovare (all'infinito)
					VehicleDictionary.TryAdd(newID, companyVehicle);
					return this.Ok(companyVehicle);
				}

				// Se il veicolo esiste, viene aggiornato
				if (!VehicleDictionary.TryGetValue(companyVehicle.VehicleID, out var existingVehicle))
				{
					return this.NotFound(new ErrorResponse().SetNotFound(string.Format(CompanyVehicleLoc.NotFoundMessageFormat, companyVehicle.VehicleID)));

				}
				existingVehicle.VehicleType = companyVehicle.VehicleType;
				existingVehicle.VehicleYearOfProduction = companyVehicle.VehicleYearOfProduction;
				existingVehicle.VehicleisActive = companyVehicle.VehicleisActive;
				if (companyVehicle.VehicleType == VehicleTypes.Car || companyVehicle.VehicleType == VehicleTypes.Truck)
				{
					existingVehicle.VehicleKm = companyVehicle.VehicleKm;


				}
				if (companyVehicle.VehicleType == VehicleTypes.Cruise || companyVehicle.VehicleType == VehicleTypes.Tractor)
				{
					existingVehicle.VehicleWorkingHours = companyVehicle.VehicleWorkingHours;
				}
				return this.Ok(existingVehicle);
			}
			catch (Exception ex)
			{
				return this.StatusCode(500, new ErrorResponse()
				{
					Message = ex.Message,
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
		[ProducesResponseType(204)]
		[ProducesResponseType(typeof(ErrorResponse), 404, "application/json")]
		public IActionResult DeleteVehicle(int vehicleID)
		{
			//lock (VehicleLocker)
			{
				if (!VehicleDictionary.TryRemove(vehicleID, out var removedVehicle))
				{
					return this.NotFound(new ErrorResponse().SetNotFound(string.Format(CompanyVehicleLoc.NotFoundMessageFormat)));
				}
				return this.NoContent();
			}
		}

		/// <summary>
		/// ResetVehiclesAndSetLocking
		/// </summary>
		/// <remarks>
		/// Reset vehicles and set Locking type: no lock, lock or interlocked
		/// </remarks>
		/// <response code="204">Reset successful</response>
		[HttpPost("reset/{lockingType}")]
		[ProducesResponseType(204)]
		public IActionResult ResetVehiclesAndSetLocking(LockingTypes lockingType)
		{
			// Pulisce la lista
			VehicleDictionary.Clear();

			// Imposta il tipo di lock scelto
			LockingType = lockingType;

			// Resetta anche il contatore ID, se serve
			newID = 0;

			return this.NoContent();
		}

		#region PrivateMethods

		private static LockingTypes LockingType = LockingTypes.Interlocked;
		private static object newIDLocker = new object();
		private static int newID = VehicleDictionary.Keys.Max();
		private static int GenerateID()
		{
			switch (LockingType)
			{
				case LockingTypes.NoLock:
					return ++newID;
				case LockingTypes.Interlocked:
					//Nessun altro thread può interferire mentre il valore viene letto, incrementato e riscritto.
					//L’intera operazione è eseguita in una singola istruzione a basso livello, in modo sicuro e veloce.
					return Interlocked.Increment(ref newID);
				case LockingTypes.Lock:
					lock (newIDLocker)
					{
						return ++newID;
					}
				default:
					throw new NotSupportedException(LockingType.ToString());
			}
		}
		#endregion

	}

}
