namespace Alpha_API.Models
{
	public class Equipment
	{
		public int EquipmentId { get; set; }
		public string EquipmentName { get; set; }
		public decimal EquipmentPrice { get; set; }
		public int EquipmentStatusId { get; set; }
		public int EquipmentQuantity { get; set; }
		public int TrainingRoomId { get; set; }
	}
}
