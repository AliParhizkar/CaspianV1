using Caspian.Common.Attributes;

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
        [EnumField("عملگر شرطی اگر ")]
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
        [EnumField("بزرگتر از")]
        BT,

        /// <summary>
        /// بزرگتر مساوی با
        /// </summary>
        [EnumField("بزرگتر مساوی با")]
        BTE,
        
        /// <summary>
        /// کوچکتر از
        /// </summary>
        [EnumField("کوچکتر از")]
        LT = 6,
        
        /// <summary>
        /// کوچکتر مساوی با
        /// </summary>
        [EnumField("کوچکتر مساوی با")]
        LTE,

        /// <summary>
        /// مساوی با
        /// </summary>
        [EnumField("مساوی با")]
        Equ = 8,
        
        /// <summary>
        /// مخالف(نامساوی با)
        /// </summary>
        [EnumField("مخالف(نامساوی با)")]
        NotEqu,

        /// <summary>
        /// یای منطقی
        /// </summary>
        [EnumField("یای منطقی")]
        Or,

        /// <summary>
        /// وی منطقی
        /// </summary>
        [EnumField("وی منطقی")]
        And = 11,
        
        /// <summary>
        /// عملگر ریاضی جمع
        /// </summary>
        [EnumField("جمع")]
        Plus,
        
        /// <summary>
        /// عملگر ریاضی تفریق
        /// </summary>
        [EnumField("تفریق")]
        Minus = 13,
        
        /// <summary>
        /// عملگر ریاضی ضرب
        /// </summary>
        [EnumField("ضرب")]
        Stra,
        
        /// <summary>
        /// عملگر ریاضی تقسیم
        /// </summary>
        [EnumField("تقسیم")]
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
        [EnumField("فرمول")]
        Rule = 1,

        [EnumField("فرم")]
        Form
    }

    public enum ValueTypeKind: byte
    { 
        /// <summary>
        /// عدد صحیح
        /// </summary>
        [EnumField("عدد صحیح")]
        Int = 1,

        [EnumField("اعداد اعشاری")]
        Double = 2,

        [EnumField("تاریخ")]
        Date = 4,

        [EnumField("بولین")]
        Bool = 16,

        [EnumField("نوع شمارشی")]
        Enum = 32,

        [EnumField("زمان")]
        Time = 64,

        [EnumField("رشته ای")]
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
        [EnumField("پارامتر اصلی")]
        MainParameter = 1,

        /// <summary>
        /// پارامتر پویا
        /// </summary>
        [EnumField("پارامتر پویا")]
        DaynamicParameter,

        /// <summary>
        /// قانون
        /// </summary>
        [EnumField("قانون")]
        RuleParameter
    }
}
