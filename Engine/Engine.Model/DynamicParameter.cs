using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// مشخصات فیلدهایی که بصورت پویا و در فرمهای پویا توسط کاربر ایجاد می شوند
    /// </summary>
    [Table("DynamicParameters", Schema = "cmn")]
    public class DynamicParameter
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        public FormControlType FormControlType { get; set; }

        public InputControlType? InputControlType { get; set; }

        public int FormId { get; set; }

        [ForeignKey(nameof(FormId))]
        public virtual Form Form { get; set; }

        [CheckOnDelete("پارامتر دارای مقدار می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<DynamicParameterValue> Values { get; set; }
    }
}
