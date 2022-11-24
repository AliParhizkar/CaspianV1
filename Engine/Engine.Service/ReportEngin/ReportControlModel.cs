using Caspian.Common;
using System.Xml.Linq;
using System.Reflection;
using System.ComponentModel;
using Caspian.Common.Extension;
using Caspian.Common.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    public class ReportControlModel
    {
        private Type type;

        public ReportControlModel()
        {
            Dependencies = new List<ReportControlDependency>();
        }

        public ReportControlModel(Type type)
        {
            this.type = type;
            Dependencies = new List<ReportControlDependency>();
        }

        public int Id { get; set; }

        /// <summary>
        /// ***
        /// عنوان لاتین فیلد
        /// </summary>
        public string EnTitle { get; set; }

        public IList<ReportControlDependency> Dependencies { get; set; }

        public string GetFaTitle(string enTitle, int? ruleId, CompositionMethodType? methodType = null)
        {
            if (ruleId.HasValue)
                return GetFaTitle(ruleId.Value);
            if (enTitle.HasValue())
                return GetFaTitle(enTitle, methodType);
            throw new Exception("خطا:enTitle or RuleId are null");
        }

        private string GetFaTitle(string enTitle, CompositionMethodType? methodType = null)
        {
            var info = type.GetMyProperty(enTitle);
            string complexTypeTitle = null;
            if (info.DeclaringType.GetCustomAttribute<ComplexTypeAttribute>() != null)
            {
                complexTypeTitle = info.GetCustomAttribute<ReportFieldAttribute>().Title;
                var index = enTitle.LastIndexOf('.');
                var str = enTitle.Substring(0, index);
                info = type.GetMyProperty(str);
            }
            var title = info.GetCustomAttribute<ReportFieldAttribute>().Title;
            if (!title.HasValue())
            {
                var display = info.GetCustomAttribute<DisplayNameAttribute>();
                if (display != null)
                    title = display.DisplayName;
            }
            if (complexTypeTitle.HasValue())
                return complexTypeTitle + " " + title;
            switch (methodType)
            {
                case CompositionMethodType.Sum: return "مجموع(" + title + ')';
                case CompositionMethodType.Avg: return "میانگین(" + title + ')';
                case CompositionMethodType.Max: return "ماکزیمم(" + title + ')';
                case CompositionMethodType.Min: return "مینیمم(" + title + ')';
            }
            return title;
        }

        /// <summary>
        /// این متد با گرفتن کد فرمول عنوان آن را برمی گرداند.
        /// </summary>
        /// <param name="ruleId">کد فرمول</param>
        /// <returns>عنوان فارسی فرمول</returns>
        private string GetFaTitle(int ruleId)
        {
            var tempType = type;
            if (EnTitle.HasValue() && EnTitle != "__DynamicField")
                tempType = type.GetMyProperty(EnTitle).PropertyType;
            var otherType = tempType.GetCustomAttributes<DynamicFieldAttribute>().ToArray()[0].OtherType;
            otherType = otherType.GetCustomAttribute<DynamicFieldAttribute>().ParameterType;
            var tableAttr = otherType.GetCustomAttribute<TableAttribute>();
            var tableName = "";
            if (tableAttr.Schema.HasValue())
                tableName = tableAttr.Schema + '.';
            tableName += tableAttr.Name;
            throw new NotImplementedException();
            //using (var context = new MyContext())
            //{
            //    var value = context.DynamicParameterTypes.SingleOrDefault(t => t.Id == RuleId);
            //    if (value == null)
            //        return "";
            //    return value.Title;
            //}
        }
    }
}
