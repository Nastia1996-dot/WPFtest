using Microsoft.AspNetCore.Mvc;
using TestProjectLibrary.Localization.Models;
using TestProjectLibrary.Models;
using TestProjectLibrary.Models.Enums;
using TestProjectLibrary.ServiceImplementations;
using TestProjectLibrary.Services;

namespace TestProjectWebAPI.Controllers
{
	/// <summary>
	/// PersonalComputer
	/// </summary>
	/// <response code="500">An internal server error has occurred</response>
	[ApiController]
	[Route("computer")]
	[ProducesResponseType<ErrorResponse>(500, "application/json")]
	public class PersonalComputerController(IStoreService<PersonalComputer> storeService) : ControllerBase
	{
		#region Properties

		private IStoreService<PersonalComputer> StoreService { get; } = storeService;

		#endregion

		/// <summary>
		/// GetPcByID
		/// </summary>
		/// <remarks>Shows personal computers details by its ID</remarks>
		/// <param name="personalComputerID"></param>
		/// <response code="200">Get successful </response>
		/// <response code="404">Not found</response>
		/// <returns></returns>
		[HttpGet("{personalComputerID:int}")]
		[ProducesResponseType<PersonalComputer>(200, "application/json")]
		[ProducesResponseType<ErrorResponse>(404, "application/json")]
		public IActionResult GetPcById(int personalComputerID)
		{
			try
			{
				if (this.StoreService.TryRead(personalComputerID, out var model))
				{
					return this.Ok(model);

				}
				else
				{
					return this.NotFound(new ErrorResponse().SetNotFound(string.Format(PersonalComputerLoc.NotFoundMessageFormat, personalComputerID)));
				}
			}
			catch (Exception ex)
			{
				return this.StatusCode(500, new ErrorResponse().SetInternalServerErrorInfo(ex.Message));
			}
		}


		/// <summary>
		/// GetPersonalComputersList
		/// </summary>
		/// <remarks>Get a list of all personal computers</remarks>
		/// <response code="200">Get successful</response>
		/// <returns></returns>
		[HttpGet()]
		[ProducesResponseType<PersonalComputer>(200, "application/json")]
		public IActionResult GetPersonalComputersList()
		{
			try
			{
				return this.Ok(this.StoreService.GetList());
			}
			catch (Exception ex)
			{
				return this.StatusCode(500, new ErrorResponse().SetInternalServerErrorInfo(ex.Message));
			}
		}

		/// <summary>
		/// PostCreateOrUpdatePc
		/// </summary>
		/// <remarks>
		/// Create a new personal computer if the ID is not present or update an existing one if it exists.
		/// </remarks>
		/// <param name="model">The personal computer to create or update</param>
		/// <response code="200">Post successful </response>
		/// <response code="400">A structure containing validation errors</response>
		/// <response code="404">Not found</response>
		/// <returns></returns>
		[HttpPost()]
		[ProducesResponseType<PersonalComputer>(200, "application/json")]
		[ProducesResponseType<ErrorResponse>(404, "application/json")]
		[ProducesResponseType<ErrorResponse>(400, "application/json")]
		public IActionResult PostCreateOrUpdatePc([FromBody] PersonalComputer model)
		{
			try
			{
				if (this.StoreService.TryCreateOrUpdate(model, out var errors))
				{
					return this.Ok(model);
				}
				else if (errors.Status == 404)
				{
					return this.NotFound(errors);
				}
				else
				{
					return this.BadRequest(errors);
				}

			}
			catch (Exception ex)
			{
				return this.StatusCode(500, new ErrorResponse().SetInternalServerErrorInfo(ex.Message));
			}
		}

		/// <summary>
		/// DeletePc
		/// </summary>
		/// <remarks>
		/// Deletes a personal computer from the list
		/// </remarks>
		/// <param name="personalComputerId"></param>
		/// <response code="204">Delete successful</response>
		/// <response code="404">Not found</response>
		/// <returns></returns>
		[HttpDelete("{personalComputerID:int}")]
		[ProducesResponseType<PersonalComputer>(204, "application/json")]
		[ProducesResponseType<ErrorResponse>(404, "application/json")]

		public IActionResult DeletePc(int personalComputerId)
		{
			try
			{
				if (this.StoreService.TryDelete(personalComputerId))
				{
					return this.NoContent();
				}
				else
				{
					return this.NotFound(new ErrorResponse().SetNotFound(string.Format(PersonalComputerLoc.NotFoundMessageFormat, personalComputerId)));
				}
			}
			catch (Exception ex)
			{
				return this.StatusCode(500, new ErrorResponse().SetInternalServerErrorInfo(ex.Message));
			}
		}

		/// <summary>
		/// ResetPcAndSetLocking
		/// </summary>
		/// <remarks>Reset the pc's list and sets Locking Type: no lock, lock and interlocked</remarks>
		/// <response code="204">Reset successful</response>
		/// <returns></returns>
		[HttpPost("reset/{lockingType}")]
		[ProducesResponseType<ErrorResponse>(204, "application/json")]
		public IActionResult PostResetPcAndSetLocking(LockingTypes lockingType)
		{
			try
			{
				this.StoreService.ResetAndSetLocking(lockingType);
				return this.NoContent();
			}
			catch (Exception ex)
			{
				return this.StatusCode(500, new ErrorResponse().SetInternalServerErrorInfo(ex.Message));
			}
		}
	}
}
