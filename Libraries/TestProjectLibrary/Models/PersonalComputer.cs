using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProjectLibrary.Localization.Models;

namespace TestProjectLibrary.Models
{

	/// <summary>
	/// PersonalComputer
	/// </summary>
	/// <remarks>
	/// Personal computers of the company
	/// </remarks>
	public class PersonalComputer : IValidableModel
	{

		#region Properties

		/// <summary>
		/// ID of the personal computer
		/// </summary>
		public int PersonalComputerID { get; set; }

		/// <summary>
		/// Name or model of the computer
		/// </summary>
		[Required]
		public string PcModelName { get; set; } = string.Empty;

		/// <summary>
		/// Installed RAM in gigabytes.
		/// </summary>
		[Required]
		public int PcRamInGB { get; set; }

		/// <summary>
		/// Processor type
		/// </summary>
		[Required]
		public string PcCPU { get; set; } = string.Empty;

		/// <summary>
		/// Type and size of the main storage
		/// </summary>
		[Required]
		public string PcStorage { get; set; } = string.Empty;


		#endregion

		#region Methods

		/// <inheritdoc cref="IValidableModel.TryValidateModel"/>
		public bool TryValidateModel(out ValidationError[] validationErrors)
		{

			var errors = new List<ValidationError>();

			if (this.PcModelName.Length > 100)
			{
				errors.Add(new ValidationError
				{
					PropertyName = nameof(this.PcModelName),
					ErrorMessage = string.Format(PersonalComputerLoc.PcModelNameErrorMessage)
				});
			}
			if (this.PcRamInGB < 0)
			{
				errors.Add(new ValidationError
				{
					PropertyName = nameof(this.PcRamInGB),
					ErrorMessage = string.Format(PersonalComputerLoc.PcRamErrorMessage)
				});
			}
			else if (this.PcRamInGB > 1024)
			{
				errors.Add(new ValidationError
				{
					PropertyName = nameof(this.PcRamInGB),
					ErrorMessage = string.Format(PersonalComputerLoc.PcRam2ErrorMessage)
				});
			}
			if (this.PcCPU.Length > 100)
			{
				errors.Add(new ValidationError
				{
					PropertyName = nameof(this.PcCPU),
					ErrorMessage = string.Format(PersonalComputerLoc.PcCPUErrorMessage)
				});
			}
			if (this.PcStorage.Length > 100)
			{
				errors.Add(new ValidationError
				{
					PropertyName = nameof(this.PcStorage),
					ErrorMessage = string.Format(PersonalComputerLoc.PcStorageErrorMessage)
				});
			}

			validationErrors = errors.ToArray();
			return validationErrors.Length == 0;
		}

		#endregion

		#region IValidableModel

		int IValidableModel.ID
		{
			get => this.PersonalComputerID;
			set => this.PersonalComputerID = value;
		}

		#endregion

	}

}
