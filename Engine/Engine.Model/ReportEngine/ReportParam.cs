using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    /// <summary>
    /// پارامترهای گزارش
    /// </summary>
    [Table("ReportParams", Schema = "cmn")]
    public class ReportParam
    {
        /// <summary>
        /// کد پارامتر گزارش
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// کد گزارش
        /// </summary>
        [DisplayName("گزارش")]
        public int ReportId { get; set; }

        /// <summary>
        /// عنوان لاتین فیلد
        /// </summary>
        [StringLength(100), DisplayName("عنوان لاتین")]
        public string TitleEn { get; set; }

        /// <summary>
        /// سطح گزارش در گزارشهای گروهبندی شده براساس گزارش
        /// </summary>
        public byte? DataLevel { get; set; }

        /// <summary>
        /// ترتیب در مرتب سازی
        /// </summary>
        [DisplayName("ترتیب")]
        public int? Order_ { get; set; }

        /// <summary>
        /// نوع مرتب سازی 
        /// </summary>
        [DisplayName("ترتیب")]
        public SortType? SortType { get; set; }

        /// <summary>
        /// نوع متد در گروهبندی پایگاه داده ای
        /// </summary>
        public CompositionMethodType? CompositionMethodType { get; set; }

        [DisplayName("قانون")]
        public int? RuleId { get; set; }

        [ForeignKey(nameof(RuleId))]
        public virtual Rule Rule { get; set; }

        public bool IsKey { get; set; }

        public int? DynamicParameterId { get; set; }

        [ForeignKey(nameof(DynamicParameterId))]
        public virtual DynamicParameter DynamicParameter { get; set; }

        /// <summary>
        /// مشخصات گزارش
        /// </summary>
        [ForeignKey(nameof(ReportId))]
        public virtual Report Report { get; set; }
    }
}