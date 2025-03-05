using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("AggregateReportsParameters", Schema = "cmn")]
    public class AggregateReportParameter
    {
        [Key]
        public int Id { get; set; }

        public int ReportId {  get; set; }

        [ForeignKey(nameof(ReportId))]
        public Report Report { get; set; }

        public int AggregateReportGroupParameterId {  get; set; }

        [ForeignKey(nameof(AggregateReportGroupParameterId))]
        public AggregateReportGroupParameter AggregateReportGroupParameter { get; set; }
    }
}
