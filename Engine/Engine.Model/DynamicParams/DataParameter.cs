using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Caspian.Engine
{
    [Table("DataParameters", Schema = "cmn")]
    public class DataParameter
    {
        [Key]
        public int Id { get; set; }

        public int ResultParameterId { get; set; }

        [ForeignKey(nameof(ResultParameterId))]
        public DynamicParameter ResultParameter { get; set; }

        [DisplayName("نوع پارامتر")]
        public DataParameterType ParameterType { get; set; }

        [DisplayName("نام خصوصیت")]
        public string PropertyName { get; set; }

        [DisplayName("پارامتر")]
        public int? DynamicParameterId { get; set; }

        [ForeignKey(nameof(DynamicParameterId))]
        public DynamicParameter DynamicParameter { get; set; }

        public int? RuleId { get; set; }

        [ForeignKey(nameof(RuleId))]
        public Rule Rule { get; set; }

        [InverseProperty("Parameter1")]
        public IList<DataParameterValue> Values1 { get; set; }

        [InverseProperty("Parameter2")]
        public IList<DataParameterValue> Values2 { get; set; }

        [InverseProperty("Parameter3")]
        public IList<DataParameterValue> Values3 { get; set; }

        [InverseProperty("Parameter4")]
        public IList<DataParameterValue> Values4 { get; set; }

        [InverseProperty("Parameter5")]
        public IList<DataParameterValue> Values5 { get; set; }

        [InverseProperty("Parameter6")]
        public IList<DataParameterValue> Values6 { get; set; }
    }
}
