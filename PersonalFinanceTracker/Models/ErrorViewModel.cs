namespace PersonalFinanceTracker.Models
{
	/// <summary>
	/// View model used by the error page to display request diagnostic information.
	/// </summary>
	public class ErrorViewModel
	{
		/// <summary>
		/// The request identifier associated with the current request (if available).
		/// </summary>
		public string? RequestId { get; set; }

		/// <summary>
		/// True if a RequestId is present and should be shown to the user.
		/// </summary>
		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
	}
}
