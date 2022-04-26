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

        public ReportControlModel(Type type, XElement element)
        {
            this.type = type;
            ControlType = (FilteringControlType)typeof(FilteringControlType)
                .GetField(element.Attribute("Type").Value).GetValue(null);
            Id = Convert.ToInt32(element.Attribute("Id").Value);
            FaTitle = element.Attribute("Title").Value;
            EnTitle = element.Element("EnTitle").Value;
            Width = Convert.ToInt32(element.Element("Width").Value);
            Height = Convert.ToInt32(element.Element("Height").Value);
            Left = Convert.ToInt32(element.Element("Left").Value);
            var ruleAttr = element.Attribute("RuleId");
            if (ruleAttr != null && ruleAttr.Value.HasValue())
                RuleId = Convert.ToInt32(ruleAttr.Value);
            Top = Convert.ToInt32(element.Element("Top").Value);
            var dependencies = element.Element("Dependencies").Elements();
            Dependencies = dependencies.Select(t => new ReportControlDependency(t)).ToList();
        }

        public int Id { get; set; }

        /// <summary>
        /// مختصات نسبی کنترل از سمت چپ
        /// </summary>
        public int? Left { get; set; }

        /// <summary>
        /// مختصات نسبی کنترل از بالا
        /// </summary>
        public int? Top { get; set; }

        /// <summary>
        /// پهنای کنترل
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// ارتفاع کنترل
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// ***
        /// عنوان لاتین فیلد
        /// </summary>
        public string EnTitle { get; set; }

        public string FaTitle { get; set; }

        /// <summary>
        /// کد TabPanel ی که کنترل به آن اختصاص دارد
        /// </summary>
        public int? TabPanelId { get; set; }

        public int? RuleId { get; set; }

        public FilteringControlType ControlType { get; set; }

        public IList<ReportControlDependency> Dependencies { get; set; }

        private DynamicParameterType GetParameter(Type type)
        {
            var otherType = type.GetCustomAttribute<DynamicFieldAttribute>().OtherType;
            foreach(var info in otherType.GetProperties())
            {
                if (info.PropertyType != type && info.CustomAttributes.Any(t => t.AttributeType == typeof(ForeignKeyAttribute)))
                {
                    type = info.PropertyType;
                    break;
                }
            }
            var tableAttr = type.GetCustomAttribute<TableAttribute>();
            var tableName = tableAttr.Name;
            if (tableAttr.Schema.HasValue())
                tableName = tableAttr.Schema + '.' + tableName;
            throw new NotImplementedException();
            //using (var context = new MyContext())
            //{
            //    return context.DynamicParameterTypes.Single(t => t.Id == RuleId.Value);
            //}
        }

        public string GetMaskedText(Type type)
        {
            if (RuleId.HasValue)
            {
                var parameter = GetParameter(type);
                if (parameter.ControlType == Engine.ControlType.Date)
                    return "13nn/nn/nn";
                return null;
            }
            return type.GetMyProperty(EnTitle).GetCustomAttribute<ReportFieldAttribute>().MaskedText;
        }

        public Caspian.Engine.InputFieldType? GetInputFieldType(Type type)
        {
            if (RuleId.HasValue)
            {
                switch(GetParameter(type).ControlType)
                {
                    case Engine.ControlType.Integer:
                        return InputFieldType.Integer;
                    case Engine.ControlType.Numeric:
                        return InputFieldType.Number;
                    case Engine.ControlType.Date:
                        return InputFieldType.String;
                    default:
                        return null;
                }
            }
            var tempType = type.GetMyProperty(EnTitle).PropertyType;
            if (tempType.IsNullableType())
                tempType = Nullable.GetUnderlyingType(tempType);
            if (tempType == typeof(int) || tempType == typeof(long) || tempType == typeof(short) || tempType == typeof(byte))
                return InputFieldType.Integer;
            if (tempType == typeof(float) || tempType == typeof(decimal) || tempType == typeof(double))
                return InputFieldType.Number;
            if (tempType == typeof(string))
            {
                var complexAttribute = tempType.GetCustomAttributes<ComplexTypeAttribute>();
                if (complexAttribute != null)
                    return InputFieldType.String;
            }
            return null;
        }

        public string GetValueField(Type type)
        {
            return type.GetMyProperty(EnTitle).PropertyType.GetPrimaryKey().Name;
        }

        public string GetDisplayField(Type type)
        {
            return type.GetMyProperty(EnTitle).GetCustomAttribute<ReportFieldAttribute>().DisplayField;
        }

        public string GetUrl(Type type)
        {
            return type.GetMyProperty(EnTitle).GetCustomAttribute<ReportFieldAttribute>().Url;
        }

        public Dictionary<string, string> GetEnumeFields(Type type)
        {
            if (RuleId.HasValue)
            {
                if (EnTitle.HasValue() && EnTitle != "__DynamicField")
                    type = type.GetMyProperty(EnTitle).PropertyType;
                type = type.GetCustomAttribute<DynamicFieldAttribute>().OtherType;
                type = type.GetCustomAttribute<DynamicFieldAttribute>().ParameterValueType;
                var tableAttr = type.GetCustomAttribute<TableAttribute>();
                var tableName = tableAttr.Name;
                if (tableAttr.Schema.HasValue())
                    tableName = tableAttr.Schema + '.' + tableName;
                //using (var context = new MyContext())
                //{
                //    var list = context.DynamicParameterTypes.Where(t => t.ParameterId == RuleId.Value).Select(t => new
                //    {
                //        t.Value,
                //        t.Display
                //    }).ToList();
                //    var dictionary = new Dictionary<string, string>();
                //    foreach (var item in list)
                //        dictionary.Add(item.Display, item.Value);
                //    return dictionary;
                //}
            }
            var tempType = type.GetMyProperty(EnTitle).PropertyType;
            if (tempType.IsNullableType())
                tempType = Nullable.GetUnderlyingType(tempType);
            var value = Activator.CreateInstance(tempType);
            var dic = new Dictionary<string, string>();
            foreach (var field in tempType.GetFields().Where(t => !t.IsSpecialName))
            {
                var attr = field.GetCustomAttribute<EnumFieldAttribute>();
                if (attr != null)
                    dic.Add(attr.DisplayName, Convert.ToInt32(field.GetValue(value)).ToString());
                else
                    dic.Add(field.Name, Convert.ToInt32(field.GetValue(value)).ToString());
            }
            return dic;
        }

        public XElement GetXml()
        {
            var ctr = new XElement("Control").AddAttribute("Type", ControlType).AddAttribute("Id", Id)
                .AddAttribute("RuleId", RuleId).AddAttribute("Title", FaTitle).AddElement("EnTitle", EnTitle)
                .AddElement("Width", Width).AddElement("Height", Height).AddElement("Left", Left)
                .AddElement("Top", Top).AddElement("Dependencies").AddAttribute("Count", Dependencies.Count);
            var dependencies = ctr.Element("Dependencies");
            foreach (var dependent in Dependencies)
                dependencies.AddElement(dependent.GetXml());
            return ctr;
        }

        private DynamicParameterType DynamicParameter(Type type)
        {
            if (EnTitle.HasValue() && EnTitle != "__DynamicField")
                type = type.GetProperty(EnTitle).PropertyType;
            var otherType = type.GetCustomAttribute<DynamicFieldAttribute>().OtherType;
            var attr = otherType.GetCustomAttribute<DynamicFieldAttribute>();
            var tableAttr = attr.ParameterType.GetCustomAttribute<TableAttribute>();
            var tableName = "";
            if (tableAttr.Schema.HasValue())
                tableName = tableAttr.Schema + '.';
            tableName += tableAttr.Name;
            throw new NotImplementedException();
            //using (var context = new MyContext())
            //{
            //    return context.DynamicParameterTypes.Single();
            //}
        }

        public string ToJson(Type type)
        {
            var str = "{\"left\":" + Left + ",\"top\":" + Top;
            if (EnTitle.HasValue())
                str += ",\"enTitle\":\"" + EnTitle + "\"";
            string[] enumes = null;
            string fromValue = null, toValue = null;
            if (RuleId.HasValue)
            {
                str += ",\"ruleId\":" + RuleId.Value;
                var controlType = DynamicParameter(type).ControlType;
                switch (controlType)
                {
                    case Engine.ControlType.Integer:
                        fromValue = "12";
                        toValue = "14";
                        ControlType = FilteringControlType.FromTo;
                        break;
                    case Engine.ControlType.Numeric:
                        fromValue = "12.25";
                        toValue = "14.75";
                        ControlType = FilteringControlType.FromTo;
                        break;
                    case Engine.ControlType.Date:
                        fromValue = "1396/01/01";
                        toValue = "1396/12/29";
                        ControlType = FilteringControlType.FromTo;
                        break;
                    case Engine.ControlType.DropdownList:
                    case Engine.ControlType.CheckListBox:
                        ControlType = FilteringControlType.Enums;
                        enumes = GetEnumeFields(type).Select(t => t.Key).ToArray();
                        break;
                    case Engine.ControlType.CheckBox:
                        break;
                    default:
                            throw new NotImplementedException("خطای عدم پیاده سازی");
                }
            }
            
            if (!FaTitle.HasValue())
                FaTitle = GetFaTitle(EnTitle, RuleId, null);
            if (FaTitle.HasValue())
                str += ",\"faTitle\":\"" + FaTitle + "\"";
            if (EnTitle.HasValue() && !RuleId.HasValue)
            {
                var info = type.GetMyProperty(EnTitle);
                var tempType = info.PropertyType;
                if (tempType.IsNullableType())
                    tempType = Nullable.GetUnderlyingType(tempType);
                if (info.GetCustomAttribute<ForeignKeyAttribute>() != null)
                    ControlType = FilteringControlType.ForeignKey;
                if (info.DeclaringType.GetCustomAttribute<ComplexTypeAttribute>() != null)
                {
                    if (info.PropertyType == typeof(string))
                    {
                        ControlType = FilteringControlType.FromTo;
                        var text = info.GetCustomAttribute<ReportFieldAttribute>().MaskedText.Replace("9", "_");
                        fromValue = text;
                        toValue = text;
                    }
                }
                if (tempType.IsEnum)
                {
                    ControlType = FilteringControlType.Enums;
                    var value = (Enum)Activator.CreateInstance(tempType);
                    enumes = value.DisplayNames().Select(t => t.Key).ToArray();
                }
                else
                    if (tempType.IsValueType)
                    {
                        ControlType = FilteringControlType.FromTo;
                        if (tempType == typeof(int) || tempType == typeof(short))
                        {
                            fromValue = "12";
                            toValue = "14";
                        }
                        else
                        {
                            fromValue = "12.25";
                            toValue = "14.75";
                        }
                    }
            }
            str += ",\"controlType\":" + (int)ControlType;
            switch (ControlType)
            {
                case FilteringControlType.Enums:
                    str += ",\"enumFields\":[";
                    var isFirstTime = true;
                    foreach (var item in enumes)
                    {
                        if (isFirstTime)
                            isFirstTime = false;
                        else
                            str += ",";
                        str += "\"" + item + "\"";
                    }
                    str += "]";
                    break;
                case FilteringControlType.FromTo:
                    str += ",\"fromValue\":\"" + fromValue + "\"";
                    str += ",\"toValue\":\"" + toValue + "\"";
                    break;
                case FilteringControlType.ForeignKey:
                    if (!Width.HasValue)
                        Width = 544;
                    break;
            }
            if (Width.HasValue)
                str += ",\"width\":" + Width.Value;
            if (Height.HasValue)
                str += ",\"height\":" + Height.Value;
            str += "}";
            return str;
        }

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



        public FilteringControlType GetControlType(string enTitle, int? ruleId)
        {
            if (enTitle.HasValue())
            {
                var info = type.GetMyProperty(enTitle);
                var tempType = info.PropertyType;
                if (tempType == typeof(bool) || tempType == typeof(bool?))
                    return FilteringControlType.Boolean;
                if (tempType.IsEnumType())
                    return FilteringControlType.Enums;
                if (tempType.IsValueType)
                {
                    foreach (var info1 in info.DeclaringType.GetProperties())
                    {
                        var foreignKey = info1.GetCustomAttribute<ForeignKeyAttribute>();
                        if (foreignKey != null)
                            if (foreignKey.Name == info.Name)
                                return FilteringControlType.ForeignKey;
                    }
                    return FilteringControlType.FromTo;
                }
                throw new Exception("خطا:field" + info.Name + " is invalid");
            }
            if (RuleId.HasValue)
                return FilteringControlType.FromTo;
            throw new Exception("خطا:enTitle and ruleId are null");
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
