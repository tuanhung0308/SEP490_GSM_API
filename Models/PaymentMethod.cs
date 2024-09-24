namespace Alpha_API.Models
{
	public class PaymentMethod
	{
		public int PaymentMethodId { get; set; }
		public string PaymentMethodName { get; set; }
	}
















	









	public class Schedule
	{
		public int ScheduleId { get; set; }
		public string ScheduleName { get; set; }
		public DateTime ScheduleTime { get; set; }
		public string ScheduleDescription { get; set; }
		public int UserId { get; set; }
		public int MembershipId { get; set; }
	}

	public class Sale
	{
		public int SaleId { get; set; }
		public string SaleContent { get; set; }
		public int SalePrice { get; set; }
	}
}
