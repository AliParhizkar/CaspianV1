using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Procurement.Model
{
    public class Policy
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("گونه")]
        public PolicyKind PolicyKind { get; set; }

        [DisplayName("سطح اثرگذاری")]
        public EffectingLevel EffectingLevel { get; set; }

        [DisplayName("روش محاسبه")]
        public EffectingLevel CalculatingMethod { get; set; }

        [DisplayName("روش مشارمت")]
        public ParticipatoryApproach ParticipatoryApproach { get; set; }

        [DisplayName("قطعیت")]
        public Definiteness Definiteness { get; set; }

        [DisplayName("تاریخ شروع اعتبار")]
        public DateOnly? StartDate { get; set; }

        [CheckOnDelete("سیاست خرید داری پارامتر می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<PolicyParameter> Parameters { get; set; }
    }
}
