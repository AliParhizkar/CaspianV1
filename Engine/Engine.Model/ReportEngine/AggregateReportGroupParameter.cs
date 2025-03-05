using Caspian.Common;
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
        public AggregateReportGroupParameter ParentParameter { get; set; }

        public AggregateParameterType AggregateParameterType { get; set; }

        public AggregateFunctionType? AggregateFunctionType { get; set; }

        public int ReportGroupId {  get; set; }

        [ForeignKey(nameof(ReportGroupId))]
        public ReportGroup ReportGroup { get; set; }

        [CheckOnDelete("پارامتر دارای پارامتر فرعی می باشد و امکان حذف آن وجود ندارد")]
        public IList<AggregateReportGroupParameter> Parameters { get; set; }

        [CheckOnDelete("این پارامتر در گزارش استفاده شده و امکان حذف آن وجود ندارد.")]
        public ICollection<AggregateReportParameter> AggregateReportParameters { get; set; }
    }
}
