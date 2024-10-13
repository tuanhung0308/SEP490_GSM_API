namespace Alpha_API.Models
{
	public class Feedback
	{
		public string FeedbackId { get; set; } // Unique identifier for feedback
		public string UserEmail { get; set; } // User who submitted the feedback
		public string Message { get; set; } // The feedback message
		public string Response { get; set; } // Staff/Admin reply to the feedback
		public DateTime SubmittedAt { get; set; } // Time when feedback was submitted
		public DateTime? RespondedAt { get; set; } // Time when response was made (nullable)
		public string RespondedBy { get; set; } // The staff/admin who responded
	}

}
