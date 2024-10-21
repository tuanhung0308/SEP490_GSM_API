namespace Alpha_API.Models
{
	public class Payment
	{
		public int PaymentId { get; set; }
		public int Amount { get; set; }
		public bool Status { get; set; }
		public DateTime PaymentDate { get; set; }
		public int PaymentMethodId { get; set; }
	}
}
