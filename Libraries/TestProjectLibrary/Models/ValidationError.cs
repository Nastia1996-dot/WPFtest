using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectLibrary.Models
{
	
	/// <summary>
	/// Represents a validation error for a specific input property.
	/// </summary>
	public class ValidationError
	{

		/// <summary>
		/// The name of the property that failed validation.
		/// </summary>
		public string PropertyName { get; set; } = string.Empty;

		/// <summary>
		/// A human-readable description of the validation error.
		/// </summary>
		public string ErrorMessage { get; set; } = string.Empty;

	}

}
