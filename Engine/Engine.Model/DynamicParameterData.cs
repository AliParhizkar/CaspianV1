using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// یک کلاس 
    /// </summary>
    public class BaseDynamicParameterData
    {
        [Key]
        public int Id { get; set; }

        public int DynamicParameterId { get; set; }

        [ForeignKey(nameof(DynamicParameterId))]
        public virtual DynamicParameter DynamicParameter { get; set; }

        public int? DynamicParameterValueId { get; set; }

        [ForeignKey(nameof(DynamicParameterValueId))]
        public virtual DynamicParameterValue DynamicParameterValue { get; set; }

        public string Value { get; set; }
    }
}
