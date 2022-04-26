using System.Collections.Generic;

namespace Page.FormEngine.Models
{
    public class WFForm
    {
        /// <summary>
        /// عنوان فیلدی در آبجکت اصلی که بصورت لیستی از پارامترها می باشد
        /// </summary>
        public string DynamicFieldName { get; set; }

        /// <summary>
        /// عنوان فیلدی که در جدول مقادیر پارامتر ها بعنوان کلید خارجی پارامترها می باشد.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// کد فعالیت
        /// </summary>
        public int ActivityId { get; set; }

        /// <summary>
        /// کد ردیابی
        /// </summary>
        public int? TraceId { get; set; }

        /// <summary>
        /// مشخصات فرم پویا بصورت JSON
        /// </summary>
        public string DynamicForm { get; set; }

        /// <summary>
        /// مشخصات فرم استاتیک بصورت JSON
        /// </summary>
        public string StaticView { get; set; }

        /// <summary>
        /// تمامی اکشن هایی که یک فرم باید داشته باشد.
        /// </summary>
        public Dictionary<string, bool?> Actions { get; set; }

        /// <summary>
        /// وضعیت کنترلهای در فرم ثابت از نظر فعال و یا غیرفعال بودن
        /// </summary>
        public string[] StaticDisabledControls { get; set; }
    }
}