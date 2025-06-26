namespace TestProjectWebAPI
{
	/// <summary>
	/// Represents the structure of a not found error response.
	/// </summary>
	public class NotFoundErrorInfo
	{
		/// <summary>
		///  A descriptive error message returned when a 404 Not Found Error occurs.
		/// </summary>
		public string ErrorMessage { get; set; } = string.Empty;
	}
}
