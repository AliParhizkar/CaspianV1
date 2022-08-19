using Caspian.Engine.Model;
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

        [ForeignKey(nameof(ResultParameterId))]
        public virtual DynamicParameter ResultParameter { get; set; }

        [DisplayName("نوع پارامتر")]
        public DataParameterType ParameterType { get; set; }

        [DisplayName("نام خصوصیت")]
        public string? PropertyName { get; set; }

        [DisplayName("پارامتر")]
        public int? DynamicParameterId { get; set; }

        [ForeignKey(nameof(DynamicParameterId))]
        public virtual DynamicParameter DynamicParameter { get; set; }
    }
}
