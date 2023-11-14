using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("Reports", Schema = "cmn")]
    public class Report
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Report group")]
        public int ReportGroupId { get; set; }

        [ForeignKey(nameof(ReportGroupId))]
        public virtual ReportGroup ReportGroup { get; set; }

        /// <summary>
        /// The name of report design file
        /// </summary>
        public string PrintFileName { get; set; }

        public string FilteringFileName { get; set; }

        /// <summary>
        /// The first level of subreport
        /// </summary>
        [DisplayName("Subreport level")]
        public SubReportLevel? SubReportLevel { get; set; }

        [DisplayName("Description")]
        public string Descript { get; set; }

        [CheckOnDelete("The report has parameter(s) and can not be removed")]
        public virtual ICollection<ReportParam> ReportParams { get; set; }
    }
}