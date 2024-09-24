namespace Alpha_API.Models
{
	public class BMI
	{
		public int BmiId { get; set; }
		public decimal Height { get; set; }
		public decimal Weight { get; set; }
		public decimal Calories { get; set; }
		public int MembershipId { get; set; }
	}
}
