using Caspian.Engine;
using Caspian.Engine.Model;
using System.ComponentModel;

namespace ReportUiModels
{
    public class ReportSelectUi
    {
        public int ReportId { get; set; }

        public string EnTitle { get; set; }

        public string ReportTitle { get; set; }

        public string FaTitle { get; set; }

        public CompositionMethodType? Type { get; set; }

        public int? RuleId { get; set; }
    }


    /// <summary>
    /// <see cref="View Model"/> گزارش
    /// </summary>
    public abstract class ReportUi 
    {
        /// <summary>
        /// کد زیرسیستم
        /// </summary>
        //public int SubSystemId { get; set; }

        /// <summary>
        /// شماره <see cref=" node "/>در درخت گزارش
        /// </summary>
        public int? NodeId { get; set; } 

        /// <summary>
        /// شماره گزارش
        /// </summary>
        public int ReportId { get; set; }

        /// <summary>
        /// عنوان گزارش
        /// </summary>
        //public string ReportTitle { get; set; }

        /// <summary>
        /// نوعی که می خواهیم برساس آن گزارش تهیه نمائیم
        /// </summary>
        //public Type ReportType { get; set; }

        /// <summary>
        /// تعداد ستونهای کنترل نوع شمارشی
        /// </summary>
        //public int? EnumColumnsCount { get; set; }
    }

    /// <summary>
    /// <see cref="View Model"/> فرم نمایش گزارش ثبت شده
    /// </summary>
    public class ReportShowUi : ReportUi
    {
        /// <summary>
        /// لیست تمامی گزارشهائی که عنوان آنها برای انتخاب کاربر نمایش داده می شود.
        /// </summary>
        public IEnumerable<Report> Reports { get; set; }

        /// <summary>
        /// لیستی از داده هائی که در سمت کلاینت توسط کاربر پر شده اند
        /// </summary>
        public IEnumerable<ReportValue> Values { get; set; }

        /// <summary>
        /// لیست وابستگی های بین کنترلهای گزارش
        /// </summary>
        public IEnumerable<ReportControlDependency> ReportControlDependencies { get; set; }

        /// <summary>
        /// فیلد اول مرتب سازی 
        /// </summary>
        [DisplayName("مرتب سازی")]
        public string OrderBy1 { get; set; }

        /// <summary>
        /// فیلد دوم مرتب سازی
        /// </summary>
        [DisplayName("مرتب سازی")]
        public string OrderBy2 { get; set; }

        /// <summary>
        /// فیلد اول گروه بندی
        /// </summary>
        [DisplayName("گروهبندی براساس")]
        public int? GroupBy1 { get; set; }

        /// <summary>
        /// فیلد دوم گروه بندی
        /// </summary>
        [DisplayName("گروهبندی براساس")]
        public int? GroupBy2 { get; set; }

        /// <summary>
        /// مرتب سازی معکوس برای فیلد اول
        /// </summary>
        [DisplayName("نزولی")]
        public bool DescOrderBy1 { get; set; }

        /// <summary>
        /// مرتب سازی معکوس برای فیلد دوم
        /// </summary>
        [DisplayName("نزولی")]
        public bool DescOrderBy2 { get; set; }

        /// <summary>
        /// گروه بندی معکوس برای فیلد اول
        /// </summary>
        [DisplayName("نزولی")]
        public bool DescGroupBy1 { get; set; }

        /// <summary>
        /// گروه بندی معکوس برای فیلد دوم
        /// </summary>
        [DisplayName("نزولی")]
        public bool DescGroupBy2 { get; set; }

        public Type ReportType { get; set; }

        /// <summary>
        /// مشخصات تمامی فیلدهائی که برای انتخاب فیلد مرتب سازی به کاربر نمایش داده می شوند.
        /// </summary>
        //public IEnumerable<SelectListItem> OrderByList { get; set; }

        /// <summary>
        /// فرمت گزارش
        /// </summary>
        public ReportFormat? ReportFormat { get; set; }

        /// <summary>
        /// مشخصات تمامی پنلهای گزارش
        /// </summary>
        public IList<TabPanelModel> Panels { get; set; }
    }

    /// <summary>
    /// <see cref="View Model"/> فرم ایجاد بخش <see cref="Where"/>
    /// </summary>
    public class ReportWhereUi : ReportUi
    {
        public Type ReportType { get; set; }

        /// <summary>
        /// مشخصات نودهائی که کاربر می تواند براساس آنها شرط گذاری نماید.
        /// </summary>
        public IList<ReportNode> Nodes { get; set; }

        /// <summary>
        /// مشخصات پنلهائی که مشخصات آنها باید نمایش داده شود.
        /// </summary>
        public IList<TabPanel> Panels { get; set; }

        public int? RuleId { get; set; }

        /// <summary>
        /// کد پنل جاری
        /// </summary>
        public int? TabPanelId { get; set; }

        /// <summary>
        /// پنل جاری گزارش
        /// </summary>
        public TabPanel Panel { get; set; }

        public ReportNode Node { get; set; }

        /// <summary>
        /// عنوان فارسی فیلدهای نوع شمارشی
        /// </summary>
        public IEnumerable<string> EnumFields { get; set; }

        /// <summary>
        /// مشخصات کنترلهای پنل جاری
        /// </summary>
        public IList<ReportControl> Controls { get; set; }

        public string EnTitle { get; set; }
    }

    /// <summary>
    /// فرمت خروجی گزارش
    /// </summary>
    public enum ReportFormat
    { 

        Word = 1,
        Pdf,
        Excel
    }
}