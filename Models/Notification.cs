namespace Alpha_API.Models
{
	public class Notification
	{
		public int NotificationId { get; set; }
		public string NotificationContent { get; set; }
		public string NotificationTitle { get; set; }
		public int UserId { get; set; }
		public DateTime NotificationSendAt { get; set; }
	}
}
