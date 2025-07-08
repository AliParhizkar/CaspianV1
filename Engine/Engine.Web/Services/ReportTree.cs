using Caspian.UI;
using Caspian.Common;
using Caspian.Engine;
using System.Reflection;
using Caspian.Engine.Model;
using System.ComponentModel;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportUiModels
{
    /// <summary>
    /// 
    /// </summary>
    public class ReportTree
    {
        public static string[] DateFields = { "PersianDate", "DayOfPersianWeek" };
        public static string[] AggregateFunctionsName = {"Sum", "Average", "Maximum", "Minimum" };
        public static string[] AggregateDateFunctionsName = {"First", "Last"};
        public static IDictionary<string, string> DateFieldsDictionary;
        public static IDictionary<string, string> AggregateFunctionsDictionary;
        public static IDictionary<string, string> AggregateDateFunctionsNameDictionary;
        public static string[] TotalDateFields = { "PersianDate", "Year", "Month", "Day", "DayOfPersianWeek" };

        static ReportTree()
        {
            DateFieldsDictionary = new Dictionary<string, string>()
            {
                { "PersianDate", "تاریخ"}, {"Year", "سال"}, {"Month", "ماه"}, {"Day", "روز"}, {"DayOfPersianWeek", "روز هفته"}
            };
            AggregateFunctionsDictionary = new Dictionary<string, string>()
            {
                {"Sum", "مجموع" }, {"Average", "میانگین"}, {"Maximum", "بیشترین"}, {"Minimum", "کمترین"}
            };
            AggregateDateFunctionsNameDictionary = new Dictionary<string, string>()
            {
                {"First", "اولین" }, {"Last", "آخرین"}
            };
        }

        public async Task<IList<ReportNode>> CreateTreeForSelect(Type type, ReportNode reportNode, IList<ReportParam> selectedNodes)
        {
            var list = new List<ReportNode>();
            var enTitle = reportNode?.TitleEn;
            if (reportNode != null && reportNode.DynamicParameterType.HasValue)
            {
                var parameterType = reportNode.DynamicParameterType.Value;
                using var context = new Context();
                if (enTitle.HasValue())
                    type = type.GetMyProperty(enTitle).PropertyType;
                switch(parameterType)
                {
                    case DynamicParameterType.Rule:
                        var result = await context.Set<Rule>().Where(t => t.TypeName == type.Name).Select(t => new
                        {
                            t.Id,
                            t.Title
                        }).ToListAsync() ;
                        foreach(var item in result)
                        {
                            list.Add(new ReportNode()
                            {
                                RuleId= item.Id,
                                DynamicParameterType = DynamicParameterType.Rule,
                                TitleEn = enTitle,
                                Selected = selectedNodes.Any(u => u.RuleId == item.Id)
                            });
                        }
                        break;
                }
                return list;
            }
            var str = enTitle;
            if (str.HasValue())
                type = type.GetMyProperty(str).PropertyType;
            var dynamicField = type.GetCustomAttribute<DynamicTypeAttribute>();
            if (dynamicField != null)
            {
                list.Add(new ReportNode()
                {
                    DynamicParameterType = DynamicParameterType.User,
                    TitleEn = str,
                    Grouping = true
                });
                list.Add(new ReportNode()
                {
                    DynamicParameterType = DynamicParameterType.Form,
                    TitleEn = str,
                    Grouping = true
                });
                list.Add(new ReportNode()
                {
                    DynamicParameterType = DynamicParameterType.Rule,
                    Grouping = true,
                    TitleEn = str
                });
            }
            var pKey = type.GetPrimaryKey();
            foreach (var info in type.GetProperties().Where(t => t != pKey && (!t.PropertyType.IsCollectionType())))
            {
                var node = new ReportNode();
                str = enTitle;
                if (str == null)
                    str = "";
                if (str != "")
                    str += '.';
                str += info.Name;
                node.TitleEn = str;
                var displayName = info.GetCustomAttribute<DisplayNameAttribute>();
                //if (displayName != null)
                //    node.TitleFa = displayName.DisplayName ?? str;
                var complextypeAttr = info.PropertyType.GetCustomAttribute<ComplexTypeAttribute>();
                //if (complextypeAttr != null)
                //    node.TitleFa = info.PropertyType.GetProperties().Single(t => t.CanWrite).GetCustomAttribute<ReportFieldAttribute>().Title + " " + node.TitleFa + "(*)";
                var complexAttr = info.PropertyType.GetCustomAttribute<ComplexTypeAttribute>();
                bool singleRelation = false;
                var keyInfo = info.PropertyType.GetPrimaryKey(true);
                if (keyInfo != null)
                    singleRelation = keyInfo.GetCustomAttribute<ForeignKeyAttribute>() != null;
                node.Grouping = info.GetCustomAttribute<ForeignKeyAttribute>() != null || complexAttr != null || singleRelation;
                if (!node.Grouping)
                    node.Selected = selectedNodes.Any(t => t.ReportGroupParameter.PropertyPath == str);
                node.IsKey = info.IsForeignKey();
                list.Add(node);
            }
            return list;
        }

        public IList<NodeView> CreateNodesForGroupBySelect(Type type)
        {
            var nodes = new List<NodeView>();
            ForeignKeyNodes(type, nodes, null);
            DateAndEnumNodes(type, nodes, null);
            foreach (var info in type.GetProperties())
            {
                var isDate = info.PropertyType == typeof(DateTime) || info.PropertyType == typeof(DateTime?);
                if (info.PropertyType.IsNumberType() || isDate)
                {
                    var isKey = info.GetCustomAttribute<KeyAttribute>() != null || type.GetProperties().Any(t => t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == info.Name);
                    if (!isKey)
                    {
                        var text = info.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? info.Name;
                        var node = new NodeView()
                        {
                            Collabsable = true,
                            Expanded = true,
                            Text = text,
                            Value = info.Name
                        };
                        if (isDate)
                            node.Children = AggregateDateFunctionsName.Select(t => new NodeView(t, AggregateDateFunctionsNameDictionary[t], false, true)).ToList();
                        else
                            node.Children = AggregateFunctionsName.Select(t => new NodeView(t, AggregateFunctionsDictionary[t], false, true)).ToList();
                        nodes.Add(node);
                    }
                }
            }
            return nodes;
        }

        void ForeignKeyNodes(Type type, IList<NodeView> nodes, string path)
        {
            if (path != null)
                path += '.';
            foreach (var info in type.GetProperties())
            {
                var fKey = info.GetCustomAttribute<ForeignKeyAttribute>();
                if (fKey != null && info.PropertyType != typeof(PersianDateTable))
                {
                    var stringProperties = info.PropertyType.GetProperties().Where(t => t.PropertyType == typeof(string));
                    if (stringProperties.Any())
                    {
                        var displayAttr = info.GetCustomAttribute<DisplayNameAttribute>();
                        if (displayAttr == null)
                            displayAttr = info.DeclaringType.GetProperty(fKey.Name).GetCustomAttribute<DisplayNameAttribute>();
                        var node = new NodeView(path + info.Name, displayAttr?.DisplayName ?? info.Name);
                        node.Children = stringProperties.Select(t => new NodeView()
                        {
                            Value = node.Value + '.' + t.Name,
                            Text = t.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? t.Name,
                            Collabsable = false,
                            Selectable = true
                        }).ToList();
                        node.Expanded = true;
                        nodes.Add(node);
                    }
                    ForeignKeyNodes(info.PropertyType, nodes, path + info.Name);
                }
            }
        }

        void DateAndEnumNodes(Type type, IList<NodeView> nodes, string path) 
        {
            if (path != null)
                path += '.';
            foreach (var info in type.GetProperties())
            {
                var propertyType = info.PropertyType.GetUnderlyingType();
                if (propertyType == typeof(DateOnly) || propertyType.IsEnumType())
                {
                    var attr = info.GetCustomAttribute<DisplayNameAttribute>();
                    var node = new NodeView()
                    {
                        Value = path + info.Name,
                        Text = attr?.DisplayName ?? path + info.Name,
                        Collabsable = false,
                        Selectable = true
                    };
                    nodes.Add(node);
                }
                else if (!propertyType.IsValueType && !propertyType.IsCollectionType() && propertyType != typeof(PersianDateTable))
                {
                    var fKey = info.GetCustomAttribute<ForeignKeyAttribute>();
                    if (fKey != null)
                        DateAndEnumNodes(info.PropertyType, nodes, path + info.Name);
                }

            }
        }
    }
}
