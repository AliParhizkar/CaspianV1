///هنگام نمایش فرم راهنما برای کنترلهائی که از نوع کلید خارجی هستند باید براساس کنترلهای موجود در گزارش رکوردهای فرم راهنما 
///فیلتر شود این کلاس برای پیدا کردن روابط بین کنترلهائی از نوع کلید خارجی با سایر کنترلهای موجود در گزارش طراحی و پیاده سازی شده است
using Caspian.Common;
using Caspian.Common.Extension;

namespace Caspian.Engine
{
    /// <summary>
    /// این کلاس برای پیدا کردن وابستگی بین کلاسها مورد استفاده قرار می گیرد
    /// </summary>
    public class DependencyEngin
    {
        private Type mainType;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">نوعی که گزارش براساس آن ساخته می شود</param>
        public DependencyEngin(Type type)
        {
            mainType = type;
        }

        /// <summary>
        /// این متد لیستی از کنترلها را گرفته و وابستگی میان آنها را برمی گرداند
        /// </summary>
        /// <param name="controls">لیست کنترلهائی که وابستگی بین آنها باید پیدا شود</param>
        /// <returns>لیستی از وابستگی های بین کنترلها</returns>
        public IList<ReportControlDependency> GetDependency(IEnumerable<ReportControlModel> controls)
        {
            ///مسیرهای غیر نهائی
            var foreigenKeyList = controls.Where(t => mainType.GetExtend(t.EnTitle) == null);
            var list = new List<ReportControlDependency>();
            foreach (var control in foreigenKeyList)
                list.AddRange(GetDependency(control, controls));
            return list;
        }

        /// <summary>
        /// این متد وابستگی بین یک کنترل از نوع کلید خارجی با سایر کنترلها را برمی گرداند
        /// </summary>
        /// <param name="mainControl">کنترلی که حتما از نوع کلید خارجی می باشد و باید وابستگی آن با سایر کنترلها پیدا شود.</param>
        /// <param name="controls">تمامی کنترلهای موجود در گزارش</param>
        /// <returns>وابستگی های بین یک کنترل از نوع کلید خارجی با سایر کنترلها</returns>
        public IList<ReportControlDependency> GetDependency(ReportControlModel mainControl, IEnumerable<ReportControlModel> controls)
        {
            var list = new List<ReportControlDependency>();
            var controlType = mainType.GetMyProperty(mainControl.EnTitle).PropertyType;
            foreach (var control in controls)
            {
                if (mainControl.EnTitle != control.EnTitle)
                {
                    var dependency = mainType.GetDependencyOfType(controlType, control.EnTitle);
                    if (dependency.HasValue())
                    {
                        var controlDependency = new ReportControlDependency();
                        controlDependency.DependControlId = control.Id;
                        controlDependency.ControlId = mainControl.Id;
                        controlDependency.Dependency = dependency;
                        list.Add(controlDependency);
                    }
                }
            }
            return list;
        }
    }
}
