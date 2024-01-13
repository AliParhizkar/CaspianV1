using Caspian.Engine;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ReportUiModels
{
    public class PrintParam
    {
        public int? ReportId { get; set; }

        public int? DataLevel { get; set; }

        public bool IsSubReport { get; set; }

        public bool Stretch { get; set; }

        public string ImageFileName { get; set; }

        public BondType? BondType { get; set; }

        /// <summary>
        /// متغیرهای سیستمی 
        /// </summary>
        [DisplayName("متغیرهای سیستمی")]
        public SystemVariable? SystemVariable { get; set; }

        /// <summary>
        /// نوع متدی 
        /// </summary>
        [DisplayName("عنوان متد")]
        public TotalFuncType? TotalFuncType { get; set; }

        [DisplayName("اسم مستعار")]
        public string EqualName { get; set; }

        public string TitleEn { get; set; }

        [DisplayName("متن کنترل")]
        public string TitleFa { get; set; }

        [DisplayName("فیلد داده")]
        public int? DataField { get; set; }

        [DisplayName("فیلد سیستمی")]
        public SystemFiledType? SystemFiledType { get; set; }

        /// <summary>
        /// جداسازی سه رقم سه رقم ارقام
        /// </summary>
        [DisplayName("گروهبندی ارقام")]
        public bool DigitGroup { get; set; }

        /// <summary>
        /// تعداد ارقام اعشاری
        /// </summary>
        [DisplayName("تعداد ارقام اعشاری"), Range(0, 5, ErrorMessage = "فقط مقاریر 0 تا 5 مجاز هستند")]
        public int? NumberDigits { get; set; }

        /// <summary>
        /// کاراکتر جداساز ارقام
        /// </summary>
        [DisplayName("کاراکتر گروهبندی")]
        public GroupNumberChar? GroupNumberChar { get; set; }

        /// <summary>
        /// کاراکتر ممیز شناور
        /// </summary>
        [DisplayName("کاراکتر ممیز")]
        public DecimalChar? DecimalChar { get; set; }

        /// <summary>
        /// معادل برخی از فیلدها 
        /// </summary>
        public string Member { get; set; }

        public IList<Token> Tokens { get; set; }

        public PrintParam CopyData()
        {
            return new PrintParam()
            {
                BondType = BondType,
                DataField = DataField,
                DataLevel = DataLevel,
                DecimalChar = DecimalChar,
                DigitGroup = DigitGroup,
                EqualName = EqualName,
                GroupNumberChar = GroupNumberChar,
                IsSubReport = IsSubReport,
                NumberDigits = NumberDigits,
                ReportId = ReportId,
                SystemFiledType = SystemFiledType,
                SystemVariable = SystemVariable,
                TitleEn = TitleEn,
                TitleFa = TitleFa,
                TotalFuncType = TotalFuncType
            };
        }
    }
}