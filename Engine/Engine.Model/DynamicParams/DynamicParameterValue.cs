using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// یک کلاس 
    /// </summary>
    [Table("DynamicParametersValues", Schema = "cmn")]
    public class DynamicParameterValue
    {
        [Key]
        public int Id { get; set; }

        public int DynamicParameterId { get; set; }

        [ForeignKey(nameof(DynamicParameterId))]
        public virtual DynamicParameter DynamicParameter { get; set; }

        public int? DynamicParameterOptionId { get; set; }

        [ForeignKey(nameof(DynamicParameterOptionId))]
        public virtual DynamicParameterOption DynamicParameterOption { get; set; }

        public decimal? Value { get; set; }
    }
}
