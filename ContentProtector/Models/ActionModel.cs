using NPoco;
using System;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace ContentProtector.App_Plugins.ContentProtector.Models {
    [TableName("ContentProtector")]
    [PrimaryKey("id",AutoIncrement = false)]
    [ExplicitColumns]
    public class ActionModel {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        public int id { get; set; }

        [Column("name")]
        public string name { get; set; }        

        [Column("nodes")]
        public string nodes { get; set; }

        [Column("disableAction")]
        public bool disableAction { get; set; }

        [Column("userExceptions")]
        public string userExceptions { get; set; }

        [Column("lastEdited")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime? lastEdited { get; set; }

        [Column("lastEditedBy")]
        public string lastEditedBy { get; set; }

        [Column("expansion")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string expansion { get; set; }
    }
}