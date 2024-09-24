namespace Alpha_API.Models
{
	public class Message
	{
		public int MessageId { get; set; }
		public int SenderId { get; set; }
		public int ReceiverId { get; set; }
		public string MessageContent { get; set; }
		public DateTime MessageSendAt { get; set; }
	}
}
