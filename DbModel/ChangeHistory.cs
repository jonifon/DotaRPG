namespace DotaRPG.DbModel
{
	public class ChangeHistory
	{
		public int Id { get; set; }
		public string EntityName { get; set; }
		public int EntityId { get; set; }
		public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
		public string ChangeType { get; set; } // Create/Update
		public string OldValue { get; set; }
		public string NewValue { get; set; }
		public string PropertyName { get; set; }
	}
}
