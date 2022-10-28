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

        [DisplayName("زیرسیستم")]
        public SubSystemKind SubSystem { get; set; }

        [DisplayName("نظام پرداخت حقوق")]
        public string EntityName { get; set; }

        [DisplayName("اولویت")]
        public int? Priority { get; set; }

        [DisplayName("عنوان")]
        public string FaTitle { get; set; }

        [DisplayName("عنوان لاتین")]
        public string EnTitle { get; set; }

        [DisplayName("روش محاسبه")]
        public CalculationType CalculationType { get; set; }

        [DisplayName("نوع کنترل")]
        public ControlType? ControlType { get; set; }

        /// <summary>
        /// نوع خروجی در حالت فرم و فرمول
        /// </summary>
        [DisplayName("نوع خروجی")]
        public ResultType? ResultType { get; set; }

        [DisplayName("تعداد ارقام اعشار")]
        public byte? DecimalNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [CheckOnDelete("پارامتر دارای چندین انتخاب می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<DynamicParameterOption> Options { get; set; }

        /// <summary>
        /// مقادیر پارامتر
        /// </summary>
        [CheckOnDelete("پارامتر دارای مقدار می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<DynamicParameterValue> Values { get; set; }

        [InverseProperty("DynamicParameter")]
        [CheckOnDelete("پارامتر دارای پارامتر داده ای می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<DataParameter> DataParameters { get; set; }

        [InverseProperty("ResultParameter")]
        [CheckOnDelete("پارامتر بعنوان پارامتر داده ای می باشد وامکان حذف آن وجود ندارد")]
        public virtual IList<DataParameter> ResultParameters { get; set; }
    }
}
