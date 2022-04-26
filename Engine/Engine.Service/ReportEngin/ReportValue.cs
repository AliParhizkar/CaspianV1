using System.Collections.Generic;

namespace Caspian.Engine
{
    /// <summary>
    /// برای دریافت اطلاعاتی که کاربر در گزارش وارد کرده است مورد استفاده قرار می گیرد
    /// </summary>
    public class ReportValue
    {
        public string EnTitle { get; set; }

        public object From { get; set; }

        public object To { get; set; }

        public int? DynamicItemId { get; set; }

        public IEnumerable<int?> Values { get; set; }
    }
}
