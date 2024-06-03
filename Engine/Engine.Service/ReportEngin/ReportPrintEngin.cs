using Caspian.Common;
using System.Reflection;
using System.Collections;
using Caspian.Engine.Model;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine.Service
{
    public class ReportPrintEngine
    {
        IServiceProvider ServiceProvider;

        public ReportPrintEngine(IServiceProvider provider)
        {
            ServiceProvider = provider;
        }

        IList GeoupByData3Level(Type type, IList<ReportParam> reportParams, IList list)
        {
            var index = 0;
            var list2 = new List<object>();
            object item = null, obj3 = null, obj4 = null;
            var strArray = reportParams.Where(t => !t.IsKey).OrderByDescending(t => t.DataLevel).Select(t => t.TitleEn).ToArray();
            var keyName = reportParams.SingleOrDefault<ReportParam>(t => ((t.DataLevel == 3) && t.IsKey)).TitleEn;
            var str2 = reportParams.SingleOrDefault<ReportParam>(t => ((t.DataLevel == 2) && t.IsKey)).TitleEn;
            foreach (object obj5 in list)
            {
                if (this.IsNewObject(list, keyName, index))
                {
                    item = Activator.CreateInstance(type);
                    list2.Add(item);
                }
                int num2 = 0;
                foreach (PropertyInfo info in type.GetProperties())
                {
                    object obj7;
                    if (info.PropertyType.IsGenericType && !info.PropertyType.IsValueType)
                    {
                        Type objectType = info.PropertyType.GenericTypeArguments[0];
                        if (info.GetValue(item) == null)
                            info.SetValue(item, Activator.CreateInstance(info.PropertyType));
                        if (this.IsNewObject(list, str2, index))
                        {
                            obj3 = Activator.CreateInstance(objectType);
                            info.PropertyType.GetMethod("Add").Invoke(info.GetValue(item), new object[] { obj3 });
                        }
                        foreach (PropertyInfo info2 in objectType.GetProperties())
                        {
                            if (info2.PropertyType.IsGenericType && !info2.PropertyType.IsValueType)
                            {
                                Type type3 = info2.PropertyType.GenericTypeArguments[0];
                                obj4 = Activator.CreateInstance(type3);
                                foreach (PropertyInfo info3 in type3.GetProperties())
                                {
                                    if (info3.Name != "Item")
                                    {
                                        obj7 = obj5.GetType().GetProperty(strArray[num2]).GetValue(obj5);
                                        info3.SetValue(obj4, obj7);
                                        num2++;
                                    }
                                }
                                if (info2.GetValue(obj3) == null)
                                    info2.SetValue(obj3, Activator.CreateInstance(info2.PropertyType));
                                info2.PropertyType.GetMethod("Add").Invoke(info2.GetValue(obj3), new object[] { obj4 });
                            }
                            else
                            {
                                if (info2.Name != "Item")
                                {
                                    obj7 = obj5.GetType().GetProperty(strArray[num2]).GetValue(obj5);
                                    info2.SetValue(obj3, obj7);
                                    num2++;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (info.Name != "Item")
                        {
                            obj7 = obj5.GetType().GetProperty(strArray[num2]).GetValue(obj5);
                            info.SetValue(item, obj7);
                            num2++;
                        }
                    }
                }
                index++;
            }
            return list2;
        }

        IList GeoupByData2Level(Type type, IList<ReportParam> reportParams, IList list)
        {
            var index = 0;
            var list2 = new List<object>();
            object item = null, obj3 = null;
            var tempList = reportParams.Where(t => !t.IsKey);
            var strArray = new string[tempList.Count()];
            var index1 = 0;
            foreach(var reportParam in tempList.OrderByDescending(t => t.DataLevel))
            {
                var str = reportParam.TitleEn;
                if (reportParam.CompositionMethodType.HasValue)
                {
                    switch(reportParam.CompositionMethodType.Value)
                    {
                        case CompositionMethodType.Sum: str = "Sum_" + str; break;
                        case CompositionMethodType.Avg: str = "Avg_" + str; break;
                        case CompositionMethodType.Max: str = "Max_" + str; break;
                        case CompositionMethodType.Min: str = "Min_" + str; break;
                    }
                }
                strArray[index1] = str;
                index1++;
            }
            var keyName = reportParams.SingleOrDefault<ReportParam>(t => ((t.DataLevel == 2) && t.IsKey)).TitleEn;
            foreach (var obj4 in list)
            {
                if (IsNewObject(list, keyName, index))
                {
                    item = Activator.CreateInstance(type);
                    list2.Add(item);
                }
                int num2 = 0;
                foreach (var info in type.GetProperties())
                {
                    object obj6;
                    if (info.PropertyType.IsGenericType && !info.PropertyType.IsValueType)
                    {
                        Type objectType = info.PropertyType.GenericTypeArguments[0];
                        if (info.GetValue(item) == null)
                            info.SetValue(item, Activator.CreateInstance(info.PropertyType));
                        obj3 = Activator.CreateInstance(objectType);
                        foreach (PropertyInfo info2 in objectType.GetProperties())
                        {
                            if (info2.Name != "Item")
                            {
                                obj6 = obj4.GetType().GetProperty(strArray[num2]).GetValue(obj4);
                                info2.SetValue(obj3, obj6);
                                num2++;
                            }
                        }
                        info.PropertyType.GetMethod("Add").Invoke(info.GetValue(item), new object[] { obj3 });
                    }
                    else
                    {
                        if (info.Name != "Item")
                        {
                            obj6 = obj4.GetType().GetProperty(strArray[num2]).GetValue(obj4);
                            info.SetValue(item, obj6);
                            num2++;
                        }
                    }
                }
                index++;
            }
            return list2;
        }

        public bool IsNewObject(IList list, string keyName, int index)
        {
            if (index == 0)
                return true;
            var obj2 = list[index - 1].GetType().GetProperty(keyName).GetValue(list[index - 1]);
            return !list[index].GetType().GetProperty(keyName).GetValue(list[index]).Equals(obj2);
        }

        public IQueryable AddOrderBy(IQueryable source, IList<ReportParam> list)
        {
            var expr = source.Expression;
            var orderExprList = new List<TempOrderExpr>();
            var tempList = list.Where(t => t.DataLevel > 1 && t.IsKey).OrderByDescending(t => t.DataLevel);
            var type = source.ElementType;
            #region
            foreach (var item in tempList)
            {
                ParameterExpression t = Expression.Parameter(type, "t");
                Expression propertyExpr = t;
                foreach (var str in item.TitleEn.Split('.'))
                    propertyExpr = Expression.Property(propertyExpr, str);
                propertyExpr = Expression.Lambda(propertyExpr, new ParameterExpression[] { t });
                orderExprList.Add(new TempOrderExpr(propertyExpr, item.SortType));
            }
            while (expr.NodeType == ExpressionType.Call)
            {
                var callExpr = expr as MethodCallExpression;
                if (callExpr.Method.Name == "OrderBy" || callExpr.Method.Name == "ThenBy")
                {
                    var quoteExpr = (callExpr.Arguments[1] as UnaryExpression).Operand;
                    orderExprList.Add(new TempOrderExpr(quoteExpr, SortType.Asc));
                }
                if (callExpr.Method.Name == "OrderByDescending" || callExpr.Method.Name == "ThenByDescending")
                {
                    var quoteExpr = (callExpr.Arguments[1] as UnaryExpression).Operand;
                    orderExprList.Add(new TempOrderExpr(quoteExpr, SortType.Decs));
                }
                expr = callExpr.Arguments[0];
            }
            #endregion
            bool firstOrderBy = true;
            foreach (var orderExpr in orderExprList)
            {
                if (orderExpr.SortType == SortType.Asc)
                    if (firstOrderBy)
                    {
                        source = source.OrderBy(orderExpr.Expr as LambdaExpression);
                        firstOrderBy = false;
                    }
                    else
                        source = source.ThenBy(orderExpr.Expr as LambdaExpression);
                else if (firstOrderBy)
                {
                    var lambda = orderExpr.Expr as LambdaExpression;
                    source = source.OrderBy(lambda);
                    firstOrderBy = false;
                }
                else
                    source = source.ThenByDescending(orderExpr.Expr as LambdaExpression);
            }
            return source;
        }

        private IList GroupByData(IQueryable data, IList<ReportParam> reportParams)
        {
            SelectReport report = new SelectReport(data.ElementType);
            data = AddOrderBy(data, reportParams).AsQueryable();

            LambdaExpression lambda = report.SimpleSelect(reportParams);
            data = data.Select(lambda);
            IList values = report.GetValues(data, reportParams);
            if (values.Count == 0)
                throw new Exception("گزارش فاقد داده می باشد و امکان پیش نمایش وجود ندارد.", null);
            Type type = GetTypeOf(reportParams, values[0].GetType(), lambda.Parameters[0].Type.Name);
            if (reportParams.Any<ReportParam>(t => t.DataLevel == 3))
                return this.GeoupByData3Level(type, reportParams, values);
            return this.GeoupByData2Level(type, reportParams, values);
        }

        public IList GetData(int reportId, IQueryable data)
        {
            var service = ServiceProvider.GetCaspianService<ReportParamService>();
            var reportParams = service.GetAll().Include(t => t.DynamicParameter).Include(t => t.Rule).Where(t => t.ReportId == reportId).ToList();
            var report = new SelectReport(data.ElementType);
            if (reportParams.Any(t => t.DataLevel.GetValueOrDefault(1) > 1))
            {
                if (reportParams.Any(t => t.CompositionMethodType.HasValue))
                    data = data.GroupBy(report.GroupBy(reportParams)).Select(report.SelectForGroupBy(reportParams));
                return GroupByData(data, reportParams);
            }
            if (reportParams.Any(t => t.CompositionMethodType.HasValue))
                data = data.GroupBy(report.GroupBy(reportParams)).Select(report.SelectForGroupBy(reportParams));
            else
                data = data.Select(report.SimpleSelect(reportParams));
            return data.ToIList();
            //return report.GetValues(data, reportParams);
        }

        public Type GetTypeOf(IList<ReportParam> reportParams, Type dynamicType, string mainTypeName)
        {
            var dataParam = reportParams.Where(t => t.DataLevel == 1 && t.IsKey != true);
            var propertiesList = new List<DynamicProperty>();
            foreach (var param in dataParam)
            {
                string name = null;
                if (param.RuleId.HasValue || param.DynamicParameterId.HasValue)
                    name = "DynamicParam" + (param.RuleId ?? param.DynamicParameterId.Value);
                else
                    name = param.TitleEn.Replace('.', '_');
                switch(param.CompositionMethodType)
                {
                    case CompositionMethodType.Sum: name = "Sum_" + name; break;
                    case CompositionMethodType.Avg: name = "Avg_" + name; break;
                    case CompositionMethodType.Max: name = "Max_" + name; break;
                    case CompositionMethodType.Min: name = "Min_" + name; break;
                }
                propertiesList.Add(new DynamicProperty(name, dynamicType.GetProperty(param.TitleEn).PropertyType));
            }
            var type = DynamicClassFactory.CreateType(propertiesList, false);
            dataParam = reportParams.Where(t => t.DataLevel == 2 && !t.IsKey);
            if (dataParam.Any())
            {
                propertiesList.Clear();
                foreach (var param in dataParam)
                {
                    var tempType = dynamicType.GetProperty(param.TitleEn).PropertyType;
                    //name = GetGroupingFiledName(param.TitleEn, 2);
                    propertiesList.Add(new DynamicProperty(param.TitleEn.Replace('.', '_'), tempType));
                }
                var listType = typeof(List<>);
                listType = listType.MakeGenericType(type);
                propertiesList.Add(new DynamicProperty(mainTypeName + 's', listType));
                type = DynamicClassFactory.CreateType(propertiesList, false);
                dataParam = reportParams.Where(t => t.DataLevel == 3 && !t.IsKey);
                if (dataParam.Any())
                {
                    propertiesList.Clear();
                    foreach (var param in dataParam)
                    {
                        var tempType = dynamicType.GetProperty(param.TitleEn).PropertyType;
                        //name = GetGroupingFiledName(param.TitleEn, 3);
                        propertiesList.Add(new DynamicProperty(param.TitleEn.Replace('.', '_'), tempType));
                    }
                    listType = typeof(List<>);
                    listType = listType.MakeGenericType(type);
                    var name2 = GetPropertyListName(dataParam.First().TitleEn, 3);
                    propertiesList.Add(new DynamicProperty(name2, listType));
                    type = DynamicClassFactory.CreateType(propertiesList, false);
                }
            }
            return type;
        }

        private string GetPropertyListName(string name, int dataLevel)
        {
            var str = name;
            if (dataLevel > 1)
            {
                var index = str.LastIndexOf('.');
                str = str.Substring(0, index);
            }
            if (dataLevel > 2)
            {
                var index = str.LastIndexOf('.');
                str = str.Substring(0, index);
            }
            return str + 's';
        }

        private string GetGroupingFiledName(string name, int dataLevel)
        {
            var str = name;
            if (dataLevel > 1)
            {
                var index = str.IndexOf('.');
                str = str.Substring(index + 1);
            }
            if (dataLevel > 2)
            {
                var index = str.IndexOf('.');
                str = str.Substring(index + 1);
            }
            return str.Replace('.', '_');
        }
    }

    internal class TempOrderExpr
    {
        public TempOrderExpr(Expression expr, SortType? sortType)
        {
            Expr = expr;
            if (sortType.HasValue)
                SortType = sortType.Value;
            else
                SortType = SortType.Asc;
        }

        public Expression Expr { get; set; }

        public SortType SortType { get; set; }
    }
}
