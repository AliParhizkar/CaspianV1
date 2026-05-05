using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("SupplierEvaluationDetails", Schema = "pcm")]
    public class SupplierEvaluationDetail
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("ارزیابی تامین کننده")]
        public int SupplierEvaluationId { get; set; }

        [ForeignKey(nameof(SupplierEvaluationId))]
        public SupplierEvaluation SupplierEvaluation { get; set; }

        [DisplayName("شاخص ارزیابی")]
        public int EvaluationIndicatorId { get; set; }

        [ForeignKey(nameof(EvaluationIndicatorId))]
        public EvaluationIndicator EvaluationIndicator { get; set; }

        [DisplayName("امتیاز")]
        public int Privilege { get; set; }
    }
}
