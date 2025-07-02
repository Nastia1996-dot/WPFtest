using Microsoft.AspNetCore.Mvc;
using TestProjectLibrary.Models;

namespace TestProjectWebAPI.Controllers
{
	/// <summary>WeatherForecast</summary>
	/// <response code="500">An internal server error has occurred</response>
	[ApiController]
	[Route("weather")]
	[ProducesResponseType<ErrorResponse>(500, "application/json")]
	public class WeatherForecastController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;

		/// <summary>
		/// WeatherForecastController
		/// </summary>
		/// <remarks>
		/// Controller
		/// </remarks>
		/// <param name="logger"></param>
		public WeatherForecastController(ILogger<WeatherForecastController> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// GetForecast
		/// </summary>
		/// <remarks>
		/// Post a forecast
		/// </remarks>
		/// <response code="200">Returns a list of `WeatherForecast` </response>
		/// <returns></returns>
		[HttpGet("forecast")]
		[ProducesResponseType<WeatherForecast[]>(200, "application/json")]
		public IActionResult GetForecast()
		{
			return this.Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
			{
				Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
				TemperatureC = Random.Shared.Next(-20, 55), 
				Summary = Summaries[Random.Shared.Next(Summaries.Length)]
			})
			.ToArray());
		}

		/// <summary>
		/// PostForecast
		/// </summary>
		/// <remarks>
		/// Post a forecast
		/// </remarks>
		/// <param name="weatherForecast">Forecast of the weather</param>
		/// <response code="200">Post successful</response>
		/// <returns></returns>
		[HttpPost("forecast")]
		public IActionResult PostForecast([FromBody] WeatherForecast weatherForecast)
		{
			return this.Ok(weatherForecast);
		}

		/// <summary>
		/// PostForecastInline
		/// </summary>
		/// <remarks>
		/// Post a forecast with **inline data**
		/// </remarks>
		/// <param name="date">Date of the forecast</param>
		/// <param name="temperatureC">Temperature in C</param>
		/// <param name="summary">Description of the forecast</param>
		/// <response code="200">Post successful</response>
		/// <response code="400">A string containing validation errors</response>
		[HttpPost("forecast/inline")]
		[ProducesResponseType<WeatherForecast>(200, "application/json")]
		[ProducesResponseType<ValidationError>(400, "application/json")]
		public IActionResult PostForecastInline(DateOnly date, int temperatureC, string summary)
		{
			try
			{
				if (temperatureC < -273)
				{
					throw new ArgumentException("must be greater than -273");
				}
				if (temperatureC <= 0)
				{
					return this.BadRequest(new ValidationError() { PropertyName = nameof(temperatureC), ErrorMessage = "the value is mandatory" });
				}
				return this.Ok(new WeatherForecast() { Date = date, TemperatureC = temperatureC, Summary = summary });
			}
			catch (Exception ex)
			{
				return this.StatusCode(500, new ErrorResponse() { Message = ex.Message });
			}
		}
	}
}
