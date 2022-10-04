using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("DataParameterValues", Schema = "cmn")]
    public class DataParameterValue
    {
        [Key]
        public int Id { get; set; }

        public int Parameter1Id { get; set; }

        [ForeignKey(nameof(Parameter1Id))]
        public virtual DataParameter Parameter1 { get; set; }

        public int? Parameter2Id { get; set; }

        [ForeignKey(nameof(Parameter2Id))]
        public virtual DataParameter Parameter2 { get; set; }

        public int? Parameter3Id { get; set; }

        [ForeignKey(nameof(Parameter3Id))]
        public virtual DataParameter Parameter3 { get; set; }

        public int? Parameter4Id { get; set; }

        [ForeignKey(nameof(Parameter4Id))]
        public virtual DataParameter Parameter4 { get; set; }

        public int? Parameter5Id { get; set; }

        [ForeignKey(nameof(Parameter5Id))]
        public virtual DataParameter Parameter5 { get; set; }

        public int? Parameter6Id { get; set; }

        [ForeignKey(nameof(Parameter6Id))]
        public virtual DataParameter Parameter6 { get; set; }

        public int Value1 { get; set; }

        public int? Value2 { get; set; }

        public int? Value3 { get; set; }

        public int? Value4 { get; set; }

        public int? Value5 { get; set; }

        public int? Value6 { get; set; }

        [Column("ResultValue", TypeName = "numeric(18, 3)")]
        public decimal ResultValue { get; set; }
    }
}
