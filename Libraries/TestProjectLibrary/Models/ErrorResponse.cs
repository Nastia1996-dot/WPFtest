using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectLibrary.Models
{
	/// <summary>
	/// Represents a standard error response returned by the API.
	/// </summary>
	public class ErrorResponse
	{

		#region Properties
		/// <summary>
		/// HTTP status code (e.g. 404, 400, 500)
		/// </summary>
		[Required]
		public int Status {  get; set; }

		/// <summary>
		/// Error message explaining what went wrong
		/// </summary>
		[Required]
		public string Message { get; set; } = string.Empty;

		/// <summary>
		/// Optional list of validation errors.
		/// </summary>
		public ValidationError[]? ValidationErrors { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Sets the error as "Not Found" (404)
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public ErrorResponse SetNotFound(string message)
		{
			this.Status = 404;
			this.Message = message;
			this.ValidationErrors = default;
			return this;
		}

		/// <summary>
		/// Sets the error as "Validation Error" (400)
		/// </summary>
		/// <param name="validationErrors"></param>
		/// <returns></returns>
		public ErrorResponse SetValidationErrors(IEnumerable<ValidationError> validationErrors)
		{
			this.Status = 400;
			this.Message = "There were validation errors, please look at the list";
			this.ValidationErrors = validationErrors.ToArray();
			return this;
		}

		/// <summary>
		/// Sets the error as "Internal Server Error" (500).
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public ErrorResponse SetInternalServerErrorInfo(string message)
		{
			this.Status = 500;
			this.Message = "An internal server error occurred";
			this.ValidationErrors = default;
			return this;
		}

		#endregion

	}

}
