using System;
using System.Linq;
using Caspian.Common;
using Caspian.Engine;
using System.Reflection;
using System.ComponentModel;
using Caspian.Engine.Service;
using System.Threading.Tasks;
using Caspian.Common.Extension;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportUiModels
{
    /// <summary>
    /// 
    /// </summary>
    public class ReportTree
    {
        /// <summary>
        /// این متد نوع اصلی را به همراه زیر مسیر گرفته مسیرهای آن را برمی گرداند
        /// </summary>
        /// <param name="type">نوع اصلی</param>
        /// <returns>ریشه درخت ساخته شده توسط تابع</returns>
        public async Task<IList<ReportNode>> CreateTreeForSelect(Type type, ReportNode reportNode, IList<string> selectedNodes)
        {
            //var list = new List<ReportNode>();
            //var enTitle = reportNode?.TitleEn;
            //var str = enTitle;
            //if (str.HasValue())
            //{
            //    if (str.EndsWith("___DynamicField"))
            //        str = str.Substring(0, str.Length - 15);
            //    if (str.Length > 0 && str[str.Length - 1] == '.')
            //        str = str.Substring(0, str.Length - 1);
            //    var array = str.Split('.');
            //    if (int.TryParse(array[0], out _))
            //    {
            //        var strTemp = str.Substring(str.IndexOf('.') + 1);
            //        type = type.GetMyProperty(strTemp).PropertyType;
            //    }
            //    else if (str.HasValue())
            //        type = type.GetMyProperty(str).PropertyType;
            //}
            //var dynamicField = type.GetCustomAttribute<DynamicFieldAttribute>();
            //if (dynamicField != null)
            //{
            //    var array = str.Split('.');
            //    if (!int.TryParse(array[0], out _) && (enTitle == null || !enTitle.EndsWith("___DynamicField")))
            //    {
            //        str = "___DynamicField";
            //        if (enTitle.HasValue())
            //            str = enTitle + ".___DynamicField";
            //        list.Add(new ReportNode()
            //        {
            //            TitleEn = str,
            //            TitleFa = "فرمهای پویا",
            //            Grouping = true
            //        });
            //    }
            //    else
            //    {
            //        if (str == "")
            //            str = null;
            //        int? formId = null;
            //        if (int.TryParse(str.Split('.')[0], out _))
            //            formId = Convert.ToInt32(str.Split('.')[0]);
            //         return (await new DynamicFieldEngin().GetDynamicItems(type, formId)).Select(t => new ReportNode() 
            //        {
            //            RuleId = t.Id,
            //            TitleFa = t.Title,
            //            TitleEn =  (t.FormId == 0 ? str : t.FormId.ToString() + '.' + str),
            //            Grouping = formId == null,
            //            Selected = selectedNodes.Contains(t.Id.ToString())
            //         }).ToList();
            //    }
            //}
            //foreach(var info in type.GetProperties())
            //{
            //    var reportFieldAttribute = info.GetCustomAttribute<ReportFieldAttribute>();
            //    if (reportFieldAttribute != null)
            //    {
            //        var node = new ReportNode();
            //        str = enTitle;
            //        if (str == null)
            //            str = "";
            //        if (str != "")
            //            str += '.';
            //        str += info.Name;
            //        node.TitleEn = str;
            //        if (reportFieldAttribute.Title.HasValue())
            //            node.TitleFa = reportFieldAttribute.Title;
            //        else
            //        {
            //            var displayName = info.GetCustomAttribute<DisplayNameAttribute>();
            //            if (displayName != null)
            //                node.TitleFa = displayName.DisplayName;
            //        }
            //        var complextypeAttr = info.PropertyType.GetCustomAttribute<ComplexTypeAttribute>();
            //        if (complextypeAttr != null)
            //            node.TitleFa = info.PropertyType.GetProperties().Single(t => t.CanWrite).GetCustomAttribute<ReportFieldAttribute>().Title + " " + node.TitleFa + "(*)";
            //        node.UseOrderBy = reportFieldAttribute.OrderBy;
            //        var complexAttr = info.PropertyType.GetCustomAttribute<ComplexTypeAttribute>();
            //        bool singleRelation = false;
            //        var keyInfo = info.PropertyType.GetPrimaryKey(true);
            //        if (keyInfo != null)
            //            singleRelation = keyInfo.GetCustomAttribute<ForeignKeyAttribute>() != null;
            //        node.Grouping = info.GetCustomAttribute<ForeignKeyAttribute>() != null || complexAttr != null || singleRelation;
            //        if (!node.Grouping)
            //            node.Selected = selectedNodes.Any(t => t == str);
            //        list.Add(node);
            //    }
            //}
            //return list;
            throw new NotImplementedException("خطای عدم پیاده سازی");
        }

        public IList<ReportNode> CreateTreeForGroupBy(Report report, string path)
        {
            var type = new AssemblyInfo().GetReturnType(report.ReportGroup);
            var type1 = type;
            if (path.HasValue())
                type1 = type.GetMyProperty(path).PropertyType;
            if (type1.IsNullableType())
                type1 = Nullable.GetUnderlyingType(type1);
            var paramsList = new List<ReportNode>();
            if (type1.IsValueType && !type1.IsEnum)
            {
                paramsList.Add(new ReportNode()
                {
                    TitleEn = path,
                    TitleFa = "مجموع",
                    Type = CompositionMethodType.Sum
                });
                paramsList.Add(new ReportNode()
                {
                    TitleEn = path,
                    TitleFa = "میانگین",
                    Type = CompositionMethodType.Avg
                });
                paramsList.Add(new ReportNode()
                {
                    TitleEn = path,
                    TitleFa = "ماکزیمم",
                    Type = CompositionMethodType.Max
                });
                paramsList.Add(new ReportNode()
                {
                    TitleEn = path,
                    TitleFa = "مینیمم",
                    Type = CompositionMethodType.Min
                });
            }
            else
            {
                foreach (var info in type1.GetProperties())
                {
                    var reportField = info.GetCustomAttribute<ReportFieldAttribute>();
                    if (reportField != null)
                    {
                        if (!reportField.Title.HasValue())
                        {
                            var displayAttr = info.GetCustomAttribute<DisplayNameAttribute>();
                            if (displayAttr != null)
                                reportField.Title = displayAttr.DisplayName;
                        }
                        var type2 = info.PropertyType;
                        if (type2.IsNullableType())
                            type2 = Nullable.GetUnderlyingType(type2);
                        if (type2.IsValueType)
                        {
                            if (!type2.IsEnum)
                            {
                                var str = path;
                                if (str.HasValue())
                                    str += '.';
                                str += info.Name;
                                var param = new ReportNode();
                                param.TitleEn = str;
                                param.TitleFa = reportField.Title;
                                param.Grouping = true;
                                paramsList.Add(param);
                            }
                        }
                        else
                            if (info.GetCustomAttribute<ForeignKeyAttribute>() != null)
                            {
                                var paramsTitle = report.ReportParams.Where(t => !t.CompositionMethodType.HasValue).Select(t => t.TitleEn).ToList();
                                var list = new List<string>();
                                foreach (var param in paramsTitle)
                                {
                                    var str = "";
                                    var tempType = type;
                                    foreach (var strItem in param.Split('.'))
                                    {
                                        var info1 = tempType.GetProperty(strItem);
                                        if (info1.GetCustomAttribute<ForeignKeyAttribute>() != null)
                                        {
                                            if (str.HasValue())
                                                str += '.';
                                            str += info1.Name;
                                        }
                                        tempType = info1.PropertyType;
                                    }
                                    if (str.HasValue() && !list.Any(t => t == str))
                                        list.Add(str);
                                }
                                var strPath = path;
                                if (strPath.HasValue())
                                    strPath += '.';
                                strPath += info.Name;
                                if (!list.Any(t => t == strPath))
                                {
                                    if (TypeHasGroupByField(info.PropertyType))
                                    {
                                        var node = new ReportNode();
                                        node.Grouping = true;
                                        node.TitleEn = strPath;
                                        node.TitleFa = reportField.Title;
                                        paramsList.Add(node);
                                    }
                                }
                            }
                    }
                }
            }
            return paramsList;
        }

        private bool TypeHasGroupByField(Type type)
        {
            foreach(var info in type.GetProperties())
            {
                if (info.GetCustomAttribute<ReportFieldAttribute>() != null)
                {
                    var tempType = info.PropertyType;
                    if (tempType.IsNullableType())
                        tempType = Nullable.GetUnderlyingType(tempType);
                    if (tempType.IsValueType && !tempType.IsEnum)
                        return true;
                    if (info.GetCustomAttribute<ForeignKeyAttribute>() != null)
                    {
                        if (TypeHasGroupByField(info.PropertyType))
                            return true;
                    }
                }
            }
            return false;
        }

        public async Task<IList<ReportNode>> CreateTreeForWhere(Type mainType, string path)
        {
            var list = new List<ReportNode>();
            PropertyInfo tempInfo = null;
            var tempType = mainType;
            DynamicFieldAttribute dynamicField = null;
            var str = path;
            if (str.HasValue() && str.EndsWith(".__DynamicField"))
                str = path.Substring(0, str.LastIndexOf('.'));
            if (str.HasValue())
                tempInfo = mainType.GetMyProperty(str);
            if (tempInfo != null)
                tempType = tempInfo.PropertyType;
            dynamicField = tempType.GetCustomAttribute<DynamicFieldAttribute>();
            if (dynamicField != null)
            {
                if (path != null && path.EndsWith("__DynamicField"))
                {
                    return (await new DynamicFieldEngin().GetDynamicItems(tempType, null))
                        .Where(t => t.ControlType != Caspian.Engine.ControlType.String)
                        .Select(t => new ReportNode()
                    {
                        RuleId = t.Id,
                        TitleFa = t.Title,
                        TitleEn = str
                    }).ToList();
                }
                else
                {
                    str = "__DynamicField";
                    if (tempInfo != null)
                        str = tempInfo.Name + '.' + str;
                    list.Add(new ReportNode()
                    {
                        TitleEn = str,
                        Grouping = true,
                        TitleFa = "پارامترهای پویا",
                    });
                }
            }
            var type = mainType;
            if (path.HasValue())
            {
                if (tempInfo.GetCustomAttribute<ForeignKeyAttribute>() != null)
                {
                    var node = new ReportNode();
                    node.TitleEn = path;
                    node.TitleFa = new ReportControlModel(mainType).GetFaTitle(path, null, null);
                    node.FilteringControlType = FilteringControlType.ForeignKey;
                    list.Add(node);
                }
                type = tempInfo.PropertyType;
                var complexTypeAttr = type.GetCustomAttribute<ComplexTypeAttribute>();
                if (complexTypeAttr != null)
                {
                    foreach(var info1 in type.GetProperties())
                    {
                        var reportAttr = info1.GetCustomAttribute<ReportFieldAttribute>();
                        if (reportAttr != null && reportAttr.Where == WhereFieldType.True)
                        {
                            var node = new ReportNode();
                            node.TitleEn = path + '.' + info1.Name;
                            node.TitleFa = reportAttr.Title;
                            tempType = info1.PropertyType;
                            if (tempType.IsEnumType())
                                node.FilteringControlType = FilteringControlType.Enums;
                            else
                                if (tempType == typeof(bool))
                                    node.FilteringControlType = FilteringControlType.Boolean;
                                else
                                    node.FilteringControlType = FilteringControlType.FromTo;
                            list.Add(node);
                        }
                    }
                    return list;
                }
            }
            foreach(var info in type.GetProperties())
            {
                var reportAttr = info.GetCustomAttribute<ReportFieldAttribute>();
                if (reportAttr != null)
                {
                    FilteringControlType? filteringControlType = null;
                    if (reportAttr.Where == WhereFieldType.Buffer)
                        reportAttr.Where = IsWhereField(info);
                    if (reportAttr.Where == WhereFieldType.True)
                    {
                        ///چنانچه فیلد کلید خارجی باشد و یا رابطه یک به (صفر یا یک داشته باشد) باید به لیست اضافه شود
                        var foreignKey = info.GetCustomAttribute<ForeignKeyAttribute>();
                        if (foreignKey == null)
                        {
                            var pKey = info.PropertyType.GetPrimaryKey();
                            if (pKey != null && pKey.GetCustomAttribute<ForeignKeyAttribute>() != null)
                                filteringControlType = FilteringControlType.ForeignKey;
                        }
                        ComplexTypeAttribute complexType = null;
                        if (foreignKey != null)
                            filteringControlType = FilteringControlType.ForeignKey;
                        else
                        {
                            ///چنانچه فیلد از نوع شمارشی باشد باید به لیست اضافه شود.
                            if (info.PropertyType.IsEnumType())
                                filteringControlType = FilteringControlType.Enums;
                            else
                                if (info.PropertyType == typeof(bool))
                                    filteringControlType = FilteringControlType.Boolean;
                                else
                                {
                                    ///چنانچه فیلد از نوع Complex Type باشد باید به لیست اضافه شود
                                    complexType = info.PropertyType.GetCustomAttribute<ComplexTypeAttribute>();
                                    if (complexType != null)
                                        filteringControlType = FilteringControlType.FromTo;
                                    else
                                    {
                                        if (info.PropertyType.IsValueType)
                                        {
                                            var key = info.GetCustomAttribute<KeyAttribute>();
                                            if (key == null)
                                                filteringControlType = FilteringControlType.FromTo;
                                        }
                                    }
                                }
                        }
                        if (filteringControlType.HasValue)
                        {
                            var node = new ReportNode();
                            if (path.HasValue())
                                node.TitleEn = path + '.' + info.Name;
                            else
                                node.TitleEn = info.Name;
                            node.Grouping = filteringControlType == FilteringControlType.ForeignKey || complexType != null;
                            if (reportAttr.Title.HasValue())
                                node.TitleFa = reportAttr.Title;
                            else
                            {
                                var displayName = info.GetCustomAttribute<DisplayNameAttribute>();
                                if (displayName != null)
                                    node.TitleFa = displayName.DisplayName;
                            }
                            node.FilteringControlType = filteringControlType;
                            node.MaskText = reportAttr.MaskedText;
                            list.Add(node);
                        }
                    }
                }
            }
            return list;
        }

        private WhereFieldType IsWhereField(PropertyInfo info)
        {
            if (info.PropertyType.IsEnumType())
                return WhereFieldType.True;
            if (info.GetCustomAttribute<ForeignKeyAttribute>() != null)
                return WhereFieldType.True;
            if (info.PropertyType.IsValueType)
            {
                foreach(var info1 in info.PropertyType.GetProperties())
                {
                    var foreignKey = info1.GetCustomAttribute<ForeignKeyAttribute>();
                    if (foreignKey != null && foreignKey.Name == info.Name)
                        return WhereFieldType.False;
                }
                return WhereFieldType.True;
            }
            if (info.PropertyType.GetCustomAttribute<ComplexTypeAttribute>() != null)
                return WhereFieldType.True;
            var pKey = info.PropertyType.GetPrimaryKey();
            if (pKey != null && pKey.GetCustomAttribute<ForeignKeyAttribute>() != null)
                return WhereFieldType.True;
            return WhereFieldType.False;
        }

        public ReportNode GetNodeproperty(Type mainType, string path)
        {
            var node = new ReportNode();
            node.TitleEn = path;
            var info = mainType.GetMyProperty(path);
            if (info.GetCustomAttribute<ForeignKeyAttribute>() != null)
                node.FilteringControlType = FilteringControlType.ForeignKey;
            else
            {
                if (info.PropertyType.IsEnumType())
                    node.FilteringControlType = FilteringControlType.Enums;
                else
                    if (info.PropertyType == typeof(bool))
                        node.FilteringControlType = FilteringControlType.Boolean;
                    else
                        node.FilteringControlType = FilteringControlType.FromTo;
            }
            var reportFieldAttr = info.GetCustomAttribute<ReportFieldAttribute>();
            if (reportFieldAttr != null)
                node.MaskText = reportFieldAttr.MaskedText;
            return node;
        }

        public ReportNode ReportNodeForWhere(Type type, string enTitle)
        {
            var reportNode = new ReportNode();
            var tempType = type;
            PropertyInfo pInfo = null;
            foreach (var item in enTitle.Split('.'))
            {
                pInfo = tempType.GetProperty(item);
                tempType = pInfo.PropertyType;
            }
            var reportAttribute = pInfo.GetCustomAttribute<ReportFieldAttribute>();
            reportNode.MaskText = reportAttribute.MaskedText;
            reportNode.Url = reportAttribute.Url;
            reportNode.ValueField = type.GetPrimaryKey().Name;
            reportNode.DisplayField = reportAttribute.DisplayField;
            var foreignKey = pInfo.GetCustomAttribute<ForeignKeyAttribute>();
            if (foreignKey != null)
                reportNode.FilteringControlType = FilteringControlType.ForeignKey;
            tempType = pInfo.PropertyType;
            if (tempType.IsNullableType())
                tempType = Nullable.GetUnderlyingType(tempType);
            if (tempType == typeof(bool))
                reportNode.FilteringControlType = FilteringControlType.Boolean;
            if (tempType.IsEnum)
                reportNode.FilteringControlType = FilteringControlType.Enums;
            if (!reportNode.FilteringControlType.HasValue && tempType.IsValueType || reportNode.MaskText.HasValue())
                reportNode.FilteringControlType = FilteringControlType.FromTo;
            return reportNode;
        }

        public IDictionary<string, string> GetOrderByList(Type type)
        {
            var dic = new Dictionary<string, string>();
            GetOrderByList(type, "", dic);
            return dic;
        }

        private void GetOrderByList(Type type, string enTitle, IDictionary<string, string> dic)
        {
            foreach(var info in type.GetProperties())
            {
                var reportAttr = info.GetCustomAttribute<ReportFieldAttribute>();
                if (reportAttr != null && reportAttr.OrderBy)
                {
                    string fielName = enTitle, faTitle = reportAttr.Title;
                    if (!faTitle.HasValue())
                    {
                        var displayAttr = info.GetCustomAttribute<DisplayNameAttribute>();
                        if (displayAttr != null)
                            faTitle = displayAttr.DisplayName;
                    }
                    if (fielName.HasValue())
                        fielName += '.';
                    fielName += info.Name;
                    var foreignKeyAttr = info.GetCustomAttribute<ForeignKeyAttribute>();
                    if (foreignKeyAttr != null)
                        GetOrderByList(info.PropertyType, fielName, dic);
                    else
                    {
                        var complexTypeAttr = info.PropertyType.GetCustomAttribute<ComplexTypeAttribute>();
                        if (complexTypeAttr != null)
                        {
                            var complexTypeInfo = info.PropertyType.GetProperties().Single(t => t.CanWrite);
                            faTitle = complexTypeInfo.GetCustomAttribute<ReportFieldAttribute>().Title + ' ' + faTitle + "(*)";
                            fielName += '.' + complexTypeInfo.Name;
                        }
                    }
                    dic.Add(fielName, faTitle);
                }
            }
        }

        private ReportNode GetReportNode(PropertyInfo property, string preFix, string faTitle, FilteringControlType controlType)
        {
            var reportNode = new ReportNode();
            if (preFix.HasValue())
                reportNode.TitleEn = preFix + '.';
            reportNode.TitleEn += property.Name;

            reportNode.FilteringControlType = controlType;
            return reportNode;
        }
    }
}
