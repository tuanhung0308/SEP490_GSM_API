namespace Alpha_API.Models
{
	public class Payment
	{
		public int PaymentId { get; set; }
		public int UserId { get; set; }
		public DateTime PaymentDate { get; set; }
		public int PaymentMethodId { get; set; }
		public string PaymentQR { get; set; }
	}
}
