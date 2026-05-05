using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Procurement.Model
{
    [Table("Policies", Schema = "pcm")]
    public class Policy
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("گونه")]
        public PolicyKind? PolicyKind { get; set; }

        [DisplayName("سطح اثرگذاری")]
        public EffectingLevel EffectingLevel { get; set; }

        [DisplayName("روش محاسبه")]
        public CalculatingMethod CalculatingMethod { get; set; }

        [DisplayName("روش مشارکت")]
        public ParticipatoryApproach ParticipatoryApproach { get; set; }

        [DisplayName("قطعیت")]
        public Definiteness Definiteness { get; set; }

        [DisplayName("تاریخ شروع اعتبار")]
        public DateOnly? StartDate { get; set; }

        [DisplayName("نوع سند")]
        public DocumentKind DocumentKind { get; set; }

        [CheckOnDelete("سیاست خرید دارای شرط می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<PolicyParameterCondition> Conditions{ get; set; }
    }
}
