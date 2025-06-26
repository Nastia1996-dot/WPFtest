using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectLibrary.Models
{
	/// <summary>
	/// CompanyVehicle
	/// </summary>
	/// <remarks>
	/// Vehicles of the company
	/// </remarks>
	public class CompanyVehicle
	{
		/// <summary>
		/// ID of the vehicle
		/// </summary>
		public int VehicleID { get; set; }

		/// <summary>
		/// Type of the company vehicle
		/// </summary>
		[Required, MaxLength(50)]
		public string VehicleType { get; set; } = "";

		/// <summary>
		/// Year the vehicle was registered
		/// </summary>
		[Required]
		public int VehicleYearOfProduction { get; set; }

		
	}
}
