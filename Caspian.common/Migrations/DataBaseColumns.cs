using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common.Migrations
{
    [Table("COLUMNS", Schema = "INFORMATION_SCHEMA")]
    internal class DataBaseColumns
    {
        [Column("TABLE_SCHEMA")]
        public string SCHEMA { get; set; }

        [Column("TABLE_NAME")]
        public string Table { get; set; }

        [Column("COLUMN_NAME")]
        public string Column {  get; set; }

        [Column("IS_NULLABLE")]
        public string IsNullable { get; set; }

        [NotMapped]
        public bool Nullable { get; set; }

        [Column("DATA_TYPE")]
        public string DataType { get; set; }

        [Column("CHARACTER_MAXIMUM_LENGTH")]
        public int? Maxlength { get; set; }

        [Column("NUMERIC_PRECISION")]
        public int? Precision { get; set; }

        [Column("NUMERIC_SCALE")]
        public int? Scale { get; set; }
    }
}
