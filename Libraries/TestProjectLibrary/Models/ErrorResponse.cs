using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProjectLibrary.Models
{

	public class ErrorResponse
	{

		#region Properties

		public int Status {  get; set; }

		public string Message { get; set; }

		public ValidationError[]? ValidationErrors { get; set; }

		#endregion

		#region Methods

		public ErrorResponse SetNotFound(string message)
		{
			this.Status = 404;
			this.Message = message;
			this.ValidationErrors = default;
			return this;
		}

		public ErrorResponse SetValidationErrors(IEnumerable<ValidationError> validationErrors)
		{
			this.Status = 400;
			this.Message = "There were validation errors, please look at the list";
			this.ValidationErrors = validationErrors.ToArray();
			return this;
		}

		#endregion

	}

}
