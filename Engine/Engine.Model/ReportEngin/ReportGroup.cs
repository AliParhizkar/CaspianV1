using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Caspian.Common;

namespace Caspian.Engine
{
    /// <summary>
    /// مشخصات گروه گزارش ها
    /// </summary>
    [Table("ReportGroups", Schema = "cmn")]
    public class ReportGroup
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// عنوان گزارش
        /// </summary>
        [DisplayName("عنوان")]
        public string Title { get; set; }

        public SubSystemKind SubSystem { get; set; }

        /// <summary>
        /// عنوان لاتین <see cref="namespace"/>ئی که متد در یکی از کلاسهای آن قرار دارد
        /// </summary>
        public string NameSpace { get; set; }

        /// <summary>
        /// عنوان لاتین کلاسی که متد در آن قرار دارد
        /// </summary>
        public string ClassTitle { get; set; }

        /// <summary>
        /// عنوان لاتین متد
        /// </summary>
        [DisplayName("عنوان متد")]
        public string MethodName { get; set; }

        /// <summary>
        /// غیرفعال
        /// </summary>
        [DisplayName("غیرفعال")]
        public bool Disable { get; set; }

        /// <summary>
        /// شرح گزارش
        /// </summary>
        [DisplayName("شرح")]
        public string Descript { get; set; }

        /// <summary>
        /// گزارش های این گروه
        /// </summary>
        [CheckOnDelete("گزارش دارای گروه می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<Report> Params { get; set; }
    }
}