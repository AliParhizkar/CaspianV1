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
        /// <summary>
        /// این متد نوع اصلی را به همراه زیر مسیر گرفته مسیرهای آن را برمی گرداند
        /// </summary>
        /// <param name="type">نوع اصلی</param>
        /// <returns>ریشه درخت ساخته شده توسط تابع</returns>
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

        public IList<ReportNode> CreateTreeForGroupBy(Report report, string path)
        {
            var type = new AssemblyInfo().GetReturnType(report.ReportGroup);
            var type1 = type;
            if (path.HasValue())
                type1 = type.GetMyProperty(path).PropertyType.GetUnderlyingType().GetUnderlyingType();
            var paramsList = new List<ReportNode>();
            if (type1.IsValueType && !type1.IsEnum)
            {
                paramsList.Add(new ReportNode()
                {
                    TitleEn = path,
                    Type = CompositionMethodType.Sum
                });
                paramsList.Add(new ReportNode()
                {
                    TitleEn = path,
                    Type = CompositionMethodType.Avg
                });
                paramsList.Add(new ReportNode()
                {
                    TitleEn = path,
                    Type = CompositionMethodType.Max
                });
                paramsList.Add(new ReportNode()
                {
                    TitleEn = path,
                    Type = CompositionMethodType.Min
                });
            }
            else
            {
                foreach (var info in type1.GetProperties())
                {
                    var displayAttr = info.GetCustomAttribute<DisplayNameAttribute>();
                    var type2 = info.PropertyType.GetUnderlyingType();
                    if (type2.IsValueType)
                    {
                        if (type2 == typeof(DateTime))
                        {
                            paramsList.Add(new ReportNode()
                            {
                                TitleEn = path,
                                Grouping = true
                            });
                            paramsList.Add(new ReportNode()
                            {
                                TitleEn = path,
                                Type = CompositionMethodType.Max
                            });
                            paramsList.Add(new ReportNode()
                            {
                                TitleEn = path,
                                Type = CompositionMethodType.Min
                            });

                        }
                        else if (!type2.IsEnum)
                        {
                            var str = path;
                            if (str.HasValue())
                                str += '.';
                            str += info.Name;
                            var param = new ReportNode();
                            param.TitleEn = str;
                            param.Grouping = true;
                            paramsList.Add(param);
                        }
                    }
                    else if (info.GetCustomAttribute<ForeignKeyAttribute>() != null)
                    {
                        var paramsTitle = report.ReportParams.Where(t => !t.CompositionMethodType.HasValue).Select(t => t.ReportGroupParameter.TitleEn).ToList();
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
                        if (!paramsList.Any(t => t.TitleEn == strPath))
                        {
                            if (TypeHasGroupByField(info.PropertyType))
                            {
                                var node = new ReportNode();
                                node.Grouping = true;
                                node.TitleEn = strPath;
                                paramsList.Add(node);
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
            return false;
        }

        private void GetOrderByList(Type type, string enTitle, IDictionary<string, string> dic)
        {
            //foreach(var info in type.GetProperties())
            //{
            //    var reportAttr = info.GetCustomAttribute<ReportFieldAttribute>();
            //    if (reportAttr != null && reportAttr.OrderBy)
            //    {
            //        string fielName = enTitle, faTitle = reportAttr.Title;
            //        if (!faTitle.HasValue())
            //        {
            //            var displayAttr = info.GetCustomAttribute<DisplayNameAttribute>();
            //            if (displayAttr != null)
            //                faTitle = displayAttr.DisplayName;
            //        }
            //        if (fielName.HasValue())
            //            fielName += '.';
            //        fielName += info.Name;
            //        var foreignKeyAttr = info.GetCustomAttribute<ForeignKeyAttribute>();
            //        if (foreignKeyAttr != null)
            //            GetOrderByList(info.PropertyType, fielName, dic);
            //        else
            //        {
            //            var complexTypeAttr = info.PropertyType.GetCustomAttribute<ComplexTypeAttribute>();
            //            if (complexTypeAttr != null)
            //            {
            //                var complexTypeInfo = info.PropertyType.GetProperties().Single(t => t.CanWrite);
            //                faTitle = complexTypeInfo.GetCustomAttribute<ReportFieldAttribute>().Title + ' ' + faTitle + "(*)";
            //                fielName += '.' + complexTypeInfo.Name;
            //            }
            //        }
            //        dic.Add(fielName, faTitle);
            //    }
            //}
        }
    }
}
