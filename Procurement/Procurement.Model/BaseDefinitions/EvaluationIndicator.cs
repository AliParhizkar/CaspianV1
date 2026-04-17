using Caspian.Common;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("EvaluationIndicators", Schema = "pcm")]
    public class EvaluationIndicator
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("نوع شاخص")]
        public IndicatorType IndicatorType { get; set; }

        [DisplayName("نوع اثرگذاری")]
        public EffectingType EffectingType { get; set; }

        [DisplayName("حداقل امتیاز")]
        public int Minimum { get; set; }

        [DisplayName("حداکثر امتیاز")]
        public int Maximum { get; set; }

        [DisplayName("وزن"), Precision(10, 3)]
        public decimal Factor { get; set; }

        [CheckOnDelete("این شاخص برای ارزیابی تامین کنندگان مورد استفاده قرار گرفته و امکان حذف آن وجود ندارد")]
        public ICollection<SupplierEvaluationDetail> Details { get; set; }
    }
}
