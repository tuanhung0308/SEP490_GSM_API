namespace Alpha_API.Models
{
	public class TrainingHistory
	{
		public string HistoryId { get; set; } 
		public string MembershipId { get; set; } 
		public DateTime CheckInTime { get; set; }
		public DateTime CheckOutTime { get; set; }
	}
}
