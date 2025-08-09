using System.ComponentModel.DataAnnotations;

namespace Warehouse.Model
{
    public enum StockRoomType:byte
    {
        [Display(Name = "سایر")]
        Others,

        [Display(Name = "دارایی ثابت")]
        FixedAsset,

        [Display(Name = "مواد")]
        Material,

        [Display(Name = "نمیه ساخت")]
        SemiFinished,

        [Display(Name = "محصول")]
        Product
    }

    public enum SimpleDataType:byte
    {
        [Display(Name = "واحد مالی")]
        FinancialUnit,

        [Display(Name = "واحد بودجه")]
        BudgetUnit,
    }

    public enum PricingMethodType:byte
    {
        [Display(Name = "آخرین وارده(LIFO)")]
        LIFO,

        [Display(Name = "میانگین موزون")]
        Average,

        [Display(Name = "اولین وارده(FIFO)")]
        FIFO
    }

    public enum MeasureType: byte
    {
        [Display(Name = "سایر")]
        Others,

        [Display(Name = "شمارشی")]
        Numerical,

        [Display(Name = "حجم")]
        Volume,

        [Display(Name = "وزن")]
        Weight,

        [Display(Name = "طول")]
        Length,

        [Display(Name = "سطج")]
        Surface
    }

    public enum SellerType: byte
    {
        [Display(Name = "حقیقی")]
        Real,
        
        [Display(Name = "حقوقی")]
        Legal
    }

    public enum Citizenship: byte
    {
        [Display(Name = "ایرانی")]
        Iranian,

        [Display(Name = "خارجی")]
        Foreigner
    }

    public enum ProductType: byte
    {
        [Display(Name = "مصرفی")]
        Consumable,

        [Display(Name = "اموالی")]
        State,

        [Display(Name = "خدماتی")]
        Service
    }

    public enum DescriptionType: byte
    {
        [Display(Name = "حرفی")]
        String,

        [Display(Name = "عددی")]
        Numerical,

        [Display(Name = "تاریخ")]
        Date
    }
}
