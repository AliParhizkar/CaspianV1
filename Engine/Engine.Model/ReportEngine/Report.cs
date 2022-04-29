using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    /// <summary>
    /// مشخصات گزارش
    /// </summary>
    [Table("Reports", Schema = "cmn")]
    public class Report
    {
        /// <summary>
        /// شماره گزارش
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// عنوان گزارش
        /// </summary>
        [DisplayName("عنوان گزارش")]
        public string Title { get; set; }

        [DisplayName("گروه گزارش")]
        public int ReportGroupId { get; set; }

        [ForeignKey(nameof(ReportGroupId))]
        public virtual ReportGroup ReportGroup { get; set; }

        /// <summary>
        /// عنوان فایلی که برای طراحی گزارش تعیین شده است.
        /// </summary>
        public string PrintFileName { get; set; }

        public string FilteringFileName { get; set; }

        /// <summary>
        /// اولین سطح داده ای زیرگزارش را مشخص می کند
        /// </summary>
        [DisplayName("سطح زیرگزارش")]
        public SubReportLevel? SubReportLevel { get; set; }

        /// <summary>
        /// شرح گزارش
        /// </summary>
        [DisplayName("شرح")]
        public string Descript { get; set; }

        /// <summary>
        /// پارامترهای گزارش
        /// </summary>
        [CheckOnDelete("گزارش دارای پارامتر می باشد و امکان حذف آن وجود ندارد.")]
        public virtual ICollection<ReportParam> ReportParams { get; set; }
    }
}