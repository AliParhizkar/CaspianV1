using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// مقادیر فیلدهای پویا که توسط کاربر و هنگام ساخت فرم تولید می شوند
    /// </summary>
    [Table("DynamicParametersValues", Schema = "cmn")]
    public class DynamicParameterValue
    {
        [Key]
        public int Id { get; set; }

        public int DynamicParameterId { get; set; }

        [ForeignKey(nameof(DynamicParameterId))]
        public virtual DynamicParameter DynamicParameter { get; set; }

        public string Value { get; set; }
    }
}
