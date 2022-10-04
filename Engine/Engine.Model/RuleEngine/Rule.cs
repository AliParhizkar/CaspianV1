using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("Rules", Schema = "cmn")]
    public class Rule
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// عنوان قانوان یا پارامتر
        /// </summary>
        [DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// شرح قانون و یا پارامتر
        /// </summary>
        [DisplayName("شرح")]
        public string Descript { get; set; }

        /// <summary>
        /// آیا قانون کاملا معتبر می باشد*
        /// </summary>
        [DisplayName("معتبر")]
        public bool IsValid { get; set; }

        [DisplayName("فرمول واسط فرمی")]
        public bool FormRule { get; set; }

        /// <summary>
        /// مشخص می کند که خروجی این <see cref="Rule"/> از چه نوعی می باشد
        /// </summary>
        [DisplayName("نوع")]
        public ValueTypeKind ResultType { get; set; }

        [DisplayName("نوع شمارشی خروجی")]
        public string EnumTypeName { get; set; }

        /// <summary>
        /// زیر سیستمی که قانون برای آن تعریف شده است.
        /// </summary>
        public SubSystemKind SystemKind { get; set; }

        /// <summary>
        /// نوعی که برای آن قانون تعریف شده است
        /// </summary>
        [DisplayName("انتخاب نوع")]
        public string TypeName { get; set; }

        /// <summary>
        /// اولویت محاسبه
        /// </summary>
        [DisplayName("اولویت محاسبه")]
        public int? Priority { get; set; }

        /// <summary>
        /// لیست توکن های این قانون
        /// </summary>
        [CheckOnDelete("این Rule دارای توکن است و امکان حذف آن وجود ندارد")]
        [InverseProperty("Rule")]
        public virtual IList<Token> Tokens { get; set; }
    }
}
