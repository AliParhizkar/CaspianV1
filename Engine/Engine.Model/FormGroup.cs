using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// مشخصات فرمهایی که توسط برنامه نویس ساخته شده
    /// و در گردش ساز قابل استفاده هستند.
    /// </summary>
    [Table("FormGroups", Schema = "cmn")]
    public class FormGroup
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// نوع سیستم
        /// </summary>
        [DisplayName("زیرسیستم")]
        public SubSystemKind Subsystem { get; set; }

        /// <summary>
        /// عنوان فارسی فرم
        /// </summary>
        [DisplayName("عنوان")]
        public string Title { get; set; }

        /// <summary>
        /// عنوان فضای کاری که کلاس مدل در آن قرار دارد
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// عنوان کلاس مدل
        /// </summary>
        public string ClassName { get; set; }

        public virtual IList<Form> Forms { get; set; }
    }
}
