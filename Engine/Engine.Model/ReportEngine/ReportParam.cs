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
        /// The Level of report in multi levels reports
        /// </summary>
        public byte DataLevel { get; set; }

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
        public Rule Rule { get; set; }

        public int ReportGroupParameterId {  get; set; }

        [ForeignKey(nameof(ReportGroupParameterId))]
        public ReportGroupParameter ReportGroupParameter { get; set; }

        public int? DynamicParameterId { get; set; }

        [ForeignKey(nameof(DynamicParameterId))]
        public DynamicParameter DynamicParameter { get; set; }

        [DisplayName("گزارش")]
        public int ReportId { get; set; }

        [ForeignKey(nameof(ReportId))]
        public Report Report { get; set; }
    }
}