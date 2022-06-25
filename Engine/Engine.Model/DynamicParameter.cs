using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// dynamic parameters that is created by user
    /// </summary>
    [Table("DynamicParameters", Schema = "cmn")]
    public class DynamicParameter
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string FaTitle { get; set; }

        public string EnTitle { get; set; }

        public ControlType ControlType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [CheckOnDelete("پارامتر دارای چندین انتخاب می باشد")]
        public virtual IList<DynamicParameterOption> Options { get; set; }

        /// <summary>
        /// مقادیر پارامتر
        /// </summary>
        [DisplayName("مقادیری که پارامتر می تواند داشته باشد.")]
        public virtual IList<DynamicParameterValue> Values { get; set; }
    }
}
