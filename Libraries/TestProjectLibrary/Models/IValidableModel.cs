using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectLibrary.Models
{
	
	/// <summary>
	/// Interface for a model with validation errors check
	/// </summary>
	public interface IValidableModel
	{

		#region Properties

		/// <summary>
		/// ID of the model
		/// </summary>
		int ID { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Check all the validation errors that may arise
		/// </summary>
		/// <param name="validationErrors"><c>out</c>: list of validation errors if the validation is failed</param>
		/// <returns><c>true</c> if the validation is successful, <c>false</c> otherwise</returns>
		bool TryValidateModel(out ValidationError[] validationErrors);

		#endregion

	}

}
