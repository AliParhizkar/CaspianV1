using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Model
{
    [Table("AggregateReportGroupsParameter", Schema = "cmn")]
    public class AggregateReportGroupParameter
    {
        [Key]
        public int Id { get; set; }

        public string Path { get; set; }

        public string Allis { get; set; }

        public int? ParentParameterId { get; set; }

        [ForeignKey(nameof(ParentParameterId))]
        public virtual AggregateReportGroupParameter ParentParameter { get; set; }

        public AggregateParameterType AggregateParameterType { get; set; }

        public int ReportGroupId {  get; set; }

        [ForeignKey(nameof(ReportGroupId))]
        public virtual ReportGroup ReportGroup { get; set; }

        public virtual IList<AggregateReportGroupParameter> Parameters { get; set; }
    }
}
