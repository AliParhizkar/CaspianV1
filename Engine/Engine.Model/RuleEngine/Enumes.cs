using Caspian.Common.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Caspian.Engine
{
    /// <summary>
    /// نوع توکن
    /// </summary>
    public enum TokenKind: byte
    {
        /// <summary>
        /// دراینصورت
        /// </summary>
        QuestionMark = 1,
        
        /// <summary>
        /// در غیراینصورت
        /// </summary>
        Colon,
        
        /// <summary>
        /// پرانتز بسته
        /// </summary>
        CloseBracket,
        
        /// <summary>
        /// عملگرهای مقایسه ای
        /// </summary>
        Compareable,
        
        /// <summary>
        /// عملگرهای منطقی
        /// </summary>
        Logical,
        
        /// <summary>
        /// عملگرهای ریاضی
        /// </summary>
        Math,
        
        /// <summary>
        /// کورشه باز
        /// </summary>
        OpenBracket,
        
        /// <summary>
        /// اگر
        /// </summary>
        If,

        /// <summary>
        /// عملوند یا مقدار ثابت
        /// </summary>
        Parameter
    }

    /// <summary>
    /// نوع عملگر
    /// </summary>
    public enum OperatorType: byte
    {
        /// <summary>
        /// دراینصورت
        /// </summary>
        [Display(Name = "عملگر شرطی اگر ")]
        QuestionMark = 1,
        
        /// <summary>
        /// درغیراینصورت
        /// </summary>
        Colon,
        
        /// <summary>
        /// پرانتز بسته
        /// </summary>
        CloseBracket = 3,
        
        /// <summary>
        /// بزرگتر از
        /// </summary>
        [Display(Name = "بزرگتر از")]
        BT,

        /// <summary>
        /// بزرگتر مساوی با
        /// </summary>
        [Display(Name = "بزرگتر مساوی با")]
        BTE,
        
        /// <summary>
        /// کوچکتر از
        /// </summary>
        [Display(Name = "کوچکتر از")]
        LT = 6,
        
        /// <summary>
        /// کوچکتر مساوی با
        /// </summary>
        [Display(Name = "کوچکتر مساوی با")]
        LTE,

        /// <summary>
        /// مساوی با
        /// </summary>
        [Display(Name = "مساوی با")]
        Equ = 8,
        
        /// <summary>
        /// مخالف(نامساوی با)
        /// </summary>
        [Display(Name = "مخالف(نامساوی با)")]
        NotEqu,

        /// <summary>
        /// یای منطقی
        /// </summary>
        [Display(Name = "یای منطقی")]
        Or,

        /// <summary>
        /// وی منطقی
        /// </summary>
        [Display(Name = "وی منطقی")]
        And = 11,
        
        /// <summary>
        /// عملگر ریاضی جمع
        /// </summary>
        [Display(Name = "جمع")]
        Plus,
        
        /// <summary>
        /// عملگر ریاضی تفریق
        /// </summary>
        [Display(Name = "تفریق")]
        Minus = 13,
        
        /// <summary>
        /// عملگر ریاضی ضرب
        /// </summary>
        [Display(Name = "ضرب")]
        Stra,
        
        /// <summary>
        /// عملگر ریاضی تقسیم
        /// </summary>
        [Display(Name = "تقسیم")]
        Slash,
        
        /// <summary>
        /// عملگر منطقی Not
        /// </summary>
        Not = 16,
        
        /// <summary>
        /// کورشه باز
        /// </summary>
        OpenBracket,
        
        /// <summary>
        /// اگر
        /// </summary>
        If
    }

    /// <summary>
    /// نوع توکن شامل(عملگر عملوند مقدار ثابت)
    /// </summary>
    public enum TokenType: byte
    {
        /// <summary>
        /// عملگر
        /// </summary>
        Oprator = 1,
        
        /// <summary>
        /// عملوند
        /// </summary>
        Oprand = 2,
        
        /// <summary>
        /// مقدار ثابت
        /// </summary>
        ConstValue = 3
    }

    /// <summary>
    /// وضعیت Stateها(شروع، پایانی، میانی)
    /// </summary>
    public enum StateStatus: byte
    { 
        /// <summary>
        /// شروع
        /// </summary>
        Start,

        /// <summary>
        /// پایانی
        /// </summary>
        Final,

        /// <summary>
        /// میانی
        /// </summary>
        Middle
    }

    public enum ImplementType: byte
    {
        [Display(Name = "فرمول")]
        Rule = 1,

        [Display(Name = "فرم")]
        Form
    }

    public enum ValueTypeKind: byte
    { 
        /// <summary>
        /// عدد صحیح
        /// </summary>
        [Display(Name = "عدد صحیح")]
        Int = 1,

        [Display(Name = "اعداد اعشاری")]
        Double = 2,

        [Display(Name = "تاریخ")]
        Date = 4,

        [Display(Name = "بولین")]
        Bool = 16,

        [Display(Name = "نوع شمارشی")]
        Enum = 32,

        [Display(Name = "زمان")]
        Time = 64,

        [Display(Name = "رشته ای")]
        String = 128
    }

    /// <summary>
    /// نوع پارامتر توکن
    /// </summary>
    public enum ParameterType: byte
    {
        /// <summary>
        /// پارامتر اصلی
        /// </summary>
        [Display(Name = "پارامتر اصلی")]
        MainParameter = 1,

        /// <summary>
        /// پارامتر پویا
        /// </summary>
        [Display(Name = "پارامتر پویا")]
        DaynamicParameter,

        /// <summary>
        /// قانون
        /// </summary>
        [Display(Name = "قانون")]
        RuleParameter
    }
}
