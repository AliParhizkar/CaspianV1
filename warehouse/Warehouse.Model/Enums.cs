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

    public enum GoodsType: byte
    {
        [Display(Name = "خریدنی")]
        Purchased,

        [Display(Name = "قابل فروش")]
        Salable,

        [Display(Name = "ساختنی")]
        Construction,

        [Display(Name = "کالای غیرموجودی")]
        OutOfStockItem
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
        WithoutReference
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
        [Display(Name = "مصرف")]
        Consumption
    }
}
