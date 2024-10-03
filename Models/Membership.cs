namespace Alpha_API.Models
{
	public class Membership
	{
		//public int MembershipId { get; set; }
		public DateTime MembershipStartDate { get; set; }
		public DateTime MembershipEndDate { get; set; }
		public string CourseId { get; set; }
		public string UserId { get; set; }
	}
}
