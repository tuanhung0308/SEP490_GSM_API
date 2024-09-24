namespace Alpha_API.Models
{
	public class ImportEquipment
	{
		public int ImportEquipmentId { get; set; }
		public DateTime ImportDate { get; set; }
		public int ImportQuantity { get; set; }
		public int EquipmentId { get; set; }
	}
}
