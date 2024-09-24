namespace Alpha_API.Models
{
	public class Membership
	{
		public int MembershipId { get; set; }
		public DateTime MembershipStartDate { get; set; }
		public DateTime MembershipEndDate { get; set; }
		public int CourseId { get; set; }
		public int UserId { get; set; }
	}
}
