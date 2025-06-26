using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectLibrary.Models
{

	/// <summary>
	/// Represents the structure of an internal server error response.
	/// </summary>
	public class InternalServerErrorInfo
	{

		/// <summary>
		/// A descriptive error message returned when a 500 Internal Server Error occurs.
		/// </summary>
		public string ErrorMessage { get; set; } = "An internal server error occurred";

	}

}
