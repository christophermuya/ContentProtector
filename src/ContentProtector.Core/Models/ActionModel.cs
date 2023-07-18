using NPoco;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace ContentProtector.Core.Models
{
	[TableName("ContentProtector")]
	[PrimaryKey("id", AutoIncrement = false)]
	[ExplicitColumns]
	public class ActionModel
	{
		[Column("id")]
		[PrimaryKeyColumn(AutoIncrement = false)]
		public int Id { get; set; }

		[Column("name")]
		public string Name { get; set; }

		[Column("nodes")]
		public string Nodes { get; set; }

		[Column("disableAction")]
		public bool DisableAction { get; set; }

		[Column("userExceptions")]
		public string UserExceptions { get; set; }

		[Column("lastEdited")]
		[NullSetting(NullSetting = NullSettings.Null)]
		public DateTime? LastEdited { get; set; }

		[Column("lastEditedBy")]
		public string LastEditedBy { get; set; }

		[Column("expansion")]
		[NullSetting(NullSetting = NullSettings.Null)]
		public string? Expansion { get; set; }
	}
}