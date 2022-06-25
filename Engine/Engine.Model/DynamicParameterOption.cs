using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// dynamic parameters values 
    /// </summary>
    [Table("DynamicParametersOptions", Schema = "cmn")]
    public class DynamicParameterOption
    {
        [Key]
        public int Id { get; set; }

        public int DynamicParameterId { get; set; }

        [ForeignKey(nameof(DynamicParameterId))]
        public virtual DynamicParameter DynamicParameter { get; set; }

        [DisplayName("عنوان فارسی")]
        public string FaTitle { get; set; }

        [DisplayName("عنوان لاتین")]
        public string EnTitle { get; set; }

        [DisplayName("مقدار")]
        public long Value { get; set; }
    }
}
