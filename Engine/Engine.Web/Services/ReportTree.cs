using Caspian.UI;
using Caspian.Common;
using Caspian.Engine;
using System.Reflection;
using Caspian.Engine.Model;
using System.ComponentModel;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReportUiModels
{
    /// <summary>
    /// 
    /// </summary>
    public class ReportTree
    {
        public static string[] DateFields = { "Date", "DayOfWeek" };

        public static IDictionary<string, string> DateFieldsText;
        public static string[] TotalDateFields = { "Date", "Year", "Month", "Day", "DayOfWeek" };

        static ReportTree()
        {
            DateFieldsText = new Dictionary<string, string>()
            {
                { "Date", "تاریخ"}, {"Year", "سال"}, {"Month", "ماه"}, {"Day", "روز"}, {"DayOfWeek", "روز هفته"}
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
                        var result = await context.Rules.Where(t => t.TypeName == type.Name).Select(t => new
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
                    case DynamicParameterType.User:
                        result = await context.DynamicParameters
                            .Where(t => t.CalculationType == CalculationType.UserData).Select(t => new
                            {
                                t.Id,
                                Title = t.Title,
                            }).ToListAsync();
                        foreach (var item in result)
                        {
                            list.Add(new ReportNode()
                            {
                                DynamicParameterId = item.Id,
                                DynamicParameterType = DynamicParameterType.User,
                                TitleEn = enTitle,
                                Selected = selectedNodes.Any(u => u.DynamicParameterId == item.Id)
                            });
                        }
                        break;
                    case DynamicParameterType.Form:
                        result = await context.DynamicParameters.Where(t => t.CalculationType == CalculationType.FormData).Select(t => new
                        {
                            t.Id,
                            Title = t.Title,
                        }).ToListAsync();
                        foreach (var item in result)
                        {
                            list.Add(new ReportNode()
                            {
                                DynamicParameterId = item.Id,
                                DynamicParameterType = DynamicParameterType.User,
                                TitleEn = enTitle,
                                Selected = selectedNodes.Any(u => u.DynamicParameterId == item.Id)
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
                    node.Selected = selectedNodes.Any(t => t.ReportGroupParameter.TitleEn == str);
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
            return nodes;
        }

        void ForeignKeyNodes(Type type, IList<NodeView> nodes, string path)
        {
            if (path != null)
                path += '.';
            foreach(var info in type.GetProperties())
            {
                var fKey = info.GetCustomAttribute<ForeignKeyAttribute>();
                if (fKey != null)
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
            foreach(var info  in type.GetProperties())
            {
                var propertyType = info.PropertyType.GetUnderlyingType();
                if (propertyType == typeof(DateTime) || propertyType.IsEnumType())
                {
                    var attr = info.GetCustomAttribute<DisplayNameAttribute>();
                    var node = new NodeView()
                    {
                        Value = path + info.Name,
                        Text = attr?.DisplayName ?? path + info.Name, 
                        Collabsable = false,
                        Selectable = true
                    };
                    //if (propertyType == typeof(DateTime))
                    //{
                    //    node.Children = new List<NodeView>();
                    //    var persianDate = info.DeclaringType.GetProperties().SingleOrDefault(t => t.PropertyType == typeof(PersianDateTable) &&
                    //        t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == info.Name);
                    //    if (persianDate == null)
                    //    {
                    //        foreach (var item in DateFields) 
                    //            node.Children.Add(new NodeView($"{path + info.Name}.{item}", item, false, true));
                    //    }
                    //    else
                    //    {
                    //        foreach (var item in TotalDateFields)
                    //            node.Children.Add(new NodeView($"{path + info.Name}.{item}", item, false, true));
                    //    }
                    //}
                    nodes.Add(node);
                }
                else if (!propertyType.IsValueType && !propertyType.IsCollectionType() && propertyType != typeof(PersianDateTable)) 
                {
                    var fKey = info.GetCustomAttribute<ForeignKeyAttribute>();
                    if (fKey != null)
                        DateAndEnumNodes(info.PropertyType, nodes, info.Name);
                }

            }
        }
    }
}
