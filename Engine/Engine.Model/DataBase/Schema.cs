using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Engine.Model.DataBase
{
    [Table("COLUMNS", Schema = "INFORMATION_SCHEMA")]
    public class Schema
    {
        [Key, Column("schema_id")]
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Table> Tables { get; set; }
    }

    [Table("tables", Schema = "sys")]
    public class Table
    {
        [Key, Column("object_id")]
        public int Id { get; set; }

        public string Name { get; set; }

        [Column("schema_id")]
        public int SchemaId { get; set; }

        [ForeignKey(nameof(SchemaId))]
        public Schema Schema { get; set; }
    }


}
