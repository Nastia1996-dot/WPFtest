using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TestProjectLibrary.Localization.Models;
using TestProjectLibrary.Models.Enums;

namespace TestProjectLibrary.Models
{
	/// <summary>
	/// CompanyVehicle
	/// </summary>
	/// <remarks>
	/// Vehicles of the company
	/// </remarks>
	public class CompanyVehicle : IValidableModel
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


		#region Methods

		/// <inheritdoc cref="IValidableModel.TryValidateModel"/>
		public bool TryValidateModel(out ValidationError[] validationErrors)
		{
			//validazioni comuni
			var errors = new List<ValidationError>();

			//se non viene inserito l'id allora si procede con l'INSERIMENTO di un nuovo veicolo
			if (this.VehicleYearOfProduction < 1900 || this.VehicleYearOfProduction > DateTime.Now.Year)
			{
				errors.Add(new ValidationError
				{
					PropertyName = nameof(this.VehicleYearOfProduction),
					ErrorMessage = string.Format(CompanyVehicleLoc.VehicleYearOfProductionErrorMessageFormat, DateTime.Now.Year)
				});
			}
			if (!this.VehicleisActive)
			{
				errors.Add(new ValidationError
				{
					PropertyName = nameof(this.VehicleisActive),
					ErrorMessage = string.Format(CompanyVehicleLoc.VehicleIsActiveErrorMessageFormat)
				});
			}
			if (this.VehicleType == VehicleTypes.Car || this.VehicleType == VehicleTypes.Truck)
			{
				if (!this.VehicleKm.HasValue || this.VehicleKm <= 0)
				{
					errors.Add(new ValidationError
					{
						PropertyName = nameof(this.VehicleKm),
						ErrorMessage = string.Format(CompanyVehicleLoc.VehicleKmErrorMessageFormat)
					});
				}
			}
			if (this.VehicleType == VehicleTypes.Cruise || this.VehicleType == VehicleTypes.Tractor)
			{
				if (!this.VehicleWorkingHours.HasValue || this.VehicleWorkingHours <= 0)
				{
					errors.Add(new ValidationError
					{
						PropertyName = nameof(this.VehicleWorkingHours),
						ErrorMessage = string.Format(CompanyVehicleLoc.VehicleWorkingHoursErrorMessageFormat)
					});
				}
			}
			validationErrors = errors.ToArray();
			return validationErrors.Length == 0;
		}
		#endregion

		#region IValidableModel

		int IValidableModel.ID
		{
			get => this.VehicleID;
			set => this.VehicleID = value;
		}

		#endregion

	}

}
