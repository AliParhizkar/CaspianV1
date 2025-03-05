using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("DataParameters", Schema = "cmn")]
    public class DataParameter
    {
        [Key]
        public int Id { get; set; }

        public int ResultParameterId { get; set; }

        [DisplayName("نوع پارامتر")]
        public DataParameterType ParameterType { get; set; }

        [DisplayName("نام خصوصیت")]
        public string PropertyName { get; set; }

        public int? RuleId { get; set; }

        [ForeignKey(nameof(RuleId))]
        public Rule Rule { get; set; }
    }
}
