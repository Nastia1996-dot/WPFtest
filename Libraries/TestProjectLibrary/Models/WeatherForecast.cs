using System.ComponentModel.DataAnnotations;

namespace TestProjectLibrary.Models
{

	/// <summary>
	/// Weather Forecast information
	/// </summary>
	public class WeatherForecast
	{

		/// <summary>
		/// Date of forecast
		/// </summary>
		public DateOnly Date { get; set; }

		/// <summary>
		/// Temperature in C
		/// </summary>
		public int TemperatureC { get; set; }

		/// <summary>
		/// Temperature in F
		/// </summary>
		public int TemperatureF => 32 + (int)(this.TemperatureC / 0.5556);

		/// <summary>
		/// Description of the forecast
		/// </summary>
		[MaxLength(50)]
		public string Summary { get; set; } = "";

	}

}
