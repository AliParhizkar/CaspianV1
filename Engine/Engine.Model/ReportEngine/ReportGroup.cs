using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
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

        public SubsystemKind SubSystem { get; set; }

        /// <summary>
        /// عنوان لاتین <see cref="namespace"/>ئی که متد در یکی از کلاسهای آن قرار دارد
        /// </summary>
        [DisplayName("NameSpace")]
        public string NameSpace { get; set; }

        /// <summary>
        /// عنوان لاتین کلاسی که متد در آن قرار دارد
        /// </summary>
        [DisplayName("Class Title")]
        public string ClassTitle { get; set; }

        [DisplayName("Method Name")]
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
        [CheckOnDelete("گروه دارای گزارش می باشد و امکان حذف آن وجود ندارد")]
        public IList<Report> Reports { get; set; }

        [NotMapped, CheckOnDelete("گروه دارای گزارش می باشد و امکان حذف آن وجود ندارد")]
        public IList<ReportGroupParameter> ReportGroupParameters { get; set; }

        [CheckOnDelete("گروه دارای پارامتر تجمیعی می باشد و امکان حذف آن وجود ندارد.")]
        public IList<AggregateReportGroupParameter> AggregateReportGroupParameters { get; set; }
    }
}