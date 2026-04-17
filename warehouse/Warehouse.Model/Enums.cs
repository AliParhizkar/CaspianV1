using Caspian.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Warehouse.Model
{
    public enum LocationType: byte
    {
        [Display(Name = "کشور")]
        Country,

        [Display(Name = "استان")]
        Province,

        [Display(Name = "شهر")]
        City
    }

    public enum MeasurementDimension: byte
    {
        [Display(Name = "وزن")]
        Weight,

        [Display(Name = "حجم")]
        Volume,

        [Display(Name = "متراژ")]
        Meterage,

        [Display(Name = "سایر(شمارشی)")]
        Other,

        [Display(Name = "زمان")]
        Time,

        [Display(Name = "انرژی")]
        Energy,

        [Display(Name = "دما")]
        Temperature,

        [Display(Name = "فشار")]
        Pressure,

        [Display(Name = "فرکانس")]
        Frequency
    }

    public enum ActiveStatus: byte
    {
        [Display(Name = "فعال")]
        Enable,

        [Display(Name = "غیرفعال")]
        Disable
    }

    public enum PropertyType: byte
    {
        [Display(Name = "عددی")]
        Number,

        [Display(Name = "تاریخ")]
        Date,

        [Display(Name = "بلی/خیر")]
        Boolean,

        [Display(Name = "متنی")]
        String,

        [Display(Name = "لیست ثابت")]
        List
    }

    public enum PricingMethod: byte
    {
        [Display(Name = "میانگین")]
        Average,

        [Display(Name = "FIFO")]
        FIFO,

        [Display(Name = "LIFO")]
        LIFO,

        [Display(Name = "شناسایی ویژه")]
        SpecialIdentification
    }

    public enum GoodsNature: byte
    {
        [Display(Name = "ماده اولیه")]
        RawMaterial,

        [Display(Name = "محصول نهایی")]
        FinalProduct,

        [Display(Name = "نیمه ساخته")]
        HalfMade,

        [Display(Name = "دارایی ثابت")]
        FixedAsset,

        [Display(Name = "سایر")]
        Others
    }

    [EnumType(IsBitwise = true)]
    public enum GoodsType: byte
    {
        [Display(Name = "خریدنی")]
        Purchased = 1,

        [Display(Name = "قابل فروش")]
        Salable = 2,

        [Display(Name = "ساختنی")]
        Construction = 4,

        [Display(Name = "کالای غیرموجودی")]
        OutOfStockItem = 8
    }

    public enum ReservationLevel: byte
    {
        [Display(Name = "رزرو ندارد")]
        hasNotReservation,

        [Display(Name = "انبار")]
        StockRoom,

        [Display(Name = "مرکز نگهداری")]
        KeepCenter
    }

    public enum ReferenceDocumentType : byte
    {
        [Display(Name = "بدون مبناء")]
        WithoutReference,

        [Display(Name = "درخواست کالا")]
        PurchaseRequest
    }

    public enum PurchaseRequestType: byte
    {
        [Display(Name = "مصرف")]
        Consumption
    }

    public enum ReservationBasis: byte
    {
        [Display(Name = "درخواست کالا")]
        PurchaseRequest
    }

    public enum ReservationType:byte
    {
        [Display(Name = "خرید")]
        Purchase,

        [Display(Name = "تولید")]
        Produce,

        [Display(Name = "مصرف")]
        Consumption,

        [Display(Name = "انتقال بین انبار")]
        BetweenStockTransfer,

        [Display(Name = "فروش")]
        Sale,

        [Display(Name = "ضایعات")]
        Waste,

        [Display(Name = "امانی")]
        Consignment,

        [Display(Name = "بلوکه کردن")]
        Blocking 
    }

    public enum GoodsAccessType : byte
    {
        [Display(Name = "طبقا")]
        GoodsClass
    }

    public enum DocumentType: byte
    {
        [Display(Name = "خرید")]
        Purchase,

        [Display(Name = "فروش")]
        Sale,

        [Display(Name = "تولید")]
        Produce,

        [Display(Name = "امانی")]
        Consignment,

        [Display(Name = "مصرف")]
        Consume,

        [Display(Name = "ضایعات")]
        Waste,

        [Display(Name = "انتقال بین انبار")]
        BetweenStockTransfer,

        [Display(Name = "انبارگردانی")]
        Anbargardany,

        [Display(Name = "سایر")]
        Others
    }

    public enum PurchaseType : byte
    {
        [Display(Name = "داخلی")]
        Internal,

        [Display(Name = "خارجی")]
        External
    }

    public enum InOutType:byte
    {
        [Display(Name = "ورودی")]
        Input,

        [Display(Name = "خروجی")]
        Output
    }

    public enum InventoryImpactType: byte
    {
        [Display(Name = "دائم")]
        Persist,

        [Display(Name = "موقت")]
        Temporary,

        [Display(Name = "ضایعات")]
        Waste
    }

    public enum DocumentRelationshipType: byte
    {
        [Display(Name ="عطف")]
        Atff,

        [Display(Name = "برگشتی")]
        Reflex 
    }

    public enum DocumentRelationshipInstance: byte
    {
        [Display(Name = "حواله تبدیل کالا")]
        HavalehTabdilKala,

        [Display(Name = "رسید تبدیل کالا")]
        RasidTabdilKala,

        [Display(Name = "حواله انتقال")]
        HavalehEnteghal,

        [Display(Name = "رسید انتقال")]
        RasidEnteghal,

        [Display(Name = "برگشت به خرید")]
        BargashtBeKharid,

        [Display(Name = "برگشت از مصرف")]
        BargashAzMasraf
    }

    [EnumType(IsBitwise = true)]
    public enum DocumentBases: byte
    {
        [Display(Name = "اسناد موقت")]
        TemporaryDocument = 1,

        [Display(Name = "مجوز ورود")]
        InputPermission = 2
    }

    [EnumType(IsBitwise = true)]
    public enum AvamelMablaghi: short
    {
        [Display(Name = "تخفیف")]
        Discount = 1,

        OtherCost = 2,

        [Display(Name = "سایر اضافات فاکتور خرید")]
        PurchaseFactorEzafat = 4,

        [Display(Name = "کرایه حمل")]
        TransferKerayeh = 8,

        [Display(Name = "مالیات")]
        Tax = 16,

        [Display(Name = "عوارض")]
        Avarez = 32,

        [Display(Name = "مالیات حمل")]
        TransferTax = 64,

        [Display(Name = "عوارض حمل")]
        TransferAvarez = 128,

        [Display(Name = "پیش پرداخت")]
        Prepayment = 256
    }

    public enum OtherPartyType : byte
    {
        [Display(Name = "مرکز هزینه")]
        CostCenter,

        [Display(Name = "شخص/شرکت")]
        PersonOrCompany
    }

    public enum PersonType: byte
    {
        [Display(Name = "حقیقی")]
        Real,

        [Display(Name = "حقوقی")]
        Legal
    }

    public enum StockFlowType: byte
    {
        [Display(Name = "موجودی اول دوره")]
        OpeningInventory,


    }
}
