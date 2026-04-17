using System.ComponentModel.DataAnnotations;

namespace Procurement.Model
{
    public enum ActivityType: byte
    {
        [Display(Name = "داخلی")]
        Interna,

        [Display(Name = "خارجی")]
        External
    }

    public enum IndicatorType: byte
    {
        [Display(Name = "کمی")]
        Kammi,

        [Display(Name = "کیفی")]
        Kifi
    }

    public enum EffectingType: byte
    {
        [Display(Name = "اولیه")]
        Avalieh,

        [Display(Name = "با مبناء")]
        BaMabna
    }

    public enum EvaluationBaseType: byte
    {
        [Display(Name = "رسید انبار")]
        Rasid,

        [Display(Name = "تحویل")]
        Tahvi,

        [Display(Name = "سفارش")]
        Order
    }

    public enum SupplierRank: byte
    {
        [Display(Name = "ضعیف")]
        Weak,

        [Display(Name = "متوسط")]
        Medium,

        [Display(Name = "خوب")]
        Good,

        [Display(Name = "عالی")]
        Fentastek
    }
}
