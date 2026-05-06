using Caspian.Common.Attributes;
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
        Quantitative,

        [Display(Name = "کیفی")]
        Qualitative
    }

    public enum EffectingType: byte
    {
        [Display(Name = "اولیه")]
        Primary,

        [Display(Name = "با مبناء")]
        WithBasis
    }

    public enum EvaluationBaseType: byte
    {
        [Display(Name = "رسید انبار")]
        Receipt,

        [Display(Name = "تحویل")]
        Delivery,

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
        Great
    }

    public enum ProcurementItemType: byte
    {
        [Display(Name = "کالا")]
        Goods,

        [Display(Name = "خدمات")]
        Service
    }

    public enum PurchaseProcessType :byte
    {
        [Display(Name = "جزئی")]
        Detail,

        [Display(Name = "استعلامی")]
        Inquiry
    }

    public enum CodingLevels: byte
    {
        [Display(Name = "سطح اول")]
        Level1,

        [Display(Name = "سطح دوم")]
        Level2,

        [Display(Name = "سطح سوم")]
        Level3,

        [Display(Name = "سطح چهارم")]
        Level4
    }

    public enum PolicyParameterProperty: byte
    {
        [Display(Name = "منبع سند")]
        DocumentSource,

        Item2,

        [Display(Name = "واحد/رمز تامین")]
        SupplyingUnit,


        [Display(Name = "روش پرداخت")]
        PaymentMethod,

        [Display(Name = "قلم خریدینی")]
        ProcurementItem,

        [Display(Name = "منبع قلم")]
        ItemSource,

        [Display(Name = "تامین کننده")]
        Supplier,

        [Display(Name = "نوع فعالیت تامین کننده")]
        SupplierActiveType
    }

    public enum PolicyKind: byte
    {

    }

    public enum EffectingLevel: byte
    {
        [Display(Name = "قلم سند تدارکات")]
        DocumentItem,

        [Display(Name = "سند تدارکات")]
        Document
    }

    public enum CalculatingMethod: byte
    {
        [Display(Name = "درصدی")]
        Percent,

        [Display(Name = "مبلغی")]
        Amount
    }

    public enum ParticipatoryApproach: byte
    {
        [Display(Name = "پیشنهادی")]
        Voluntary,

        [Display(Name = "اجباری")]
        Compulsory,

        [Display(Name = "اختیاری")]
        Optional,
    }

    public enum Definiteness: byte
    {
        [Display(Name = "قطعی")]
        Definite,

        [Display(Name = "غیرقطعی")]
        Indefinite
    }

    [EnumType(IsBitwise = true)]
    public enum DocumentKind
    {
        [Display(Name = "پیش فاکتور")]
        preFactor = 1,

        [Display(Name = "فاکتور")]
        Factor = 2,

        [Display(Name = "دستور خرید")]
        PurchaseOrder = 4,

        [Display(Name = "سفارش خرید")]
        Order = 8,

        [Display(Name = "درخواست خرید")]
        PurchaseRequest = 16,

        [Display(Name = "قرارداد خرید")]
        PurchaseContract = 32
    }

    public enum Citizenship: byte
    {
        [Display(Name = "ایرانی")]
        Iranian,

        [Display(Name = "خارجی")]
        Foreigner
    }
}
