using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TestProjectLibrary.Models.Enums;

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
		[Required]
		public VehicleTypes VehicleType { get; set; }

		/// <summary>
		/// Year the vehicle was registered
		/// </summary>
		[Required]
		public int VehicleYearOfProduction { get; set; }

		/// <summary>
		/// The vehicle is currently in use or decommissioned
		/// </summary>
		[Required]
		public bool VehicleisActive { get; set; }

		/// <summary>
		/// Km travelled by the vehicle (if it is a car or truck)
		/// </summary>
		public decimal? VehicleKm {get; set;}

		/// <summary>
		/// Hours worked by the vehicle (if it is a cruise or tractor)
		/// </summary>
		public int? VehicleWorkingHours { get; set; }
	}
}
