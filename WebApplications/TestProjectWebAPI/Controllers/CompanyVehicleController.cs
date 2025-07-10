using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using TestProjectLibrary.Localization.Models;
using TestProjectLibrary.Models;
using TestProjectLibrary.Models.Enums;
using TestProjectLibrary.ServiceImplementations;
using TestProjectLibrary.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TestProjectWebAPI.Controllers
{

	/// <summary>CompanyVehicle</summary>
	/// <response code="500">An internal server error has occurred</response>
	[ApiController]
	[Route("vehicle")]
	[ProducesResponseType<ErrorResponse>(500, "application/json")]

	public class CompanyVehicleController(ICompanyVehicleStoreService storeService) : ControllerBase
	{

		#region Properties

		private ICompanyVehicleStoreService StoreService { get; } = storeService;

		#endregion

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
				if (this.StoreService.TryRead(vehicleID, out var vehicle))
				{
					return this.Ok(vehicle);
				}
				else
				{
					return this.NotFound(new ErrorResponse().SetNotFound(string.Format(CompanyVehicleLoc.NotFoundMessageFormat, vehicleID)));
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
			return this.Ok(this.StoreService.GetList());
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
				var validationErros = companyVehicle.GetValidationErrors();
				if (validationErros.Count > 0)
				{
					return this.BadRequest(new ErrorResponse().SetValidationErrors(validationErros));
				}

				//chiamata al servizio
				if (this.StoreService.TryCreateOrUpdate(companyVehicle, out var errors))
				{
					return this.Ok(companyVehicle);
				}
				else
				{
					// Se c'è un errore NotFound, lo restituisco con 404
					if (errors != null && errors.Status == 404)
					{
						return this.NotFound(errors);
					}
					return this.BadRequest(errors);
				}
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
			{
				if (!this.StoreService.TryDelete(vehicleID))
				{
					return this.NotFound(new ErrorResponse().SetNotFound(string.Format(CompanyVehicleLoc.NotFoundMessageFormat, vehicleID)));
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
			this.StoreService.ResetAndSetLocking(lockingType);

			return this.NoContent();
		}

	}

}
