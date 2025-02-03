using Microsoft.EntityFrameworkCore;
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

        public int? DynamicParameterId { get; set; }

        [ForeignKey(nameof(DynamicParameterId))]
        public DynamicParameter DynamicParameter { get; set; }

        public int? DynamicParameterOptionId { get; set; }

        [ForeignKey(nameof(DynamicParameterOptionId))]
        public DynamicParameterOption DynamicParameterOption { get; set; }

        public int? RuleId { get; set; }

        [ForeignKey(nameof(RuleId))]
        public Rule Rule { get; set; }

        [Precision(10, 2)]
        public decimal? Value { get; set; }
    }
}
