using System.Reflection;
using System.Collections;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common.Extension
{
    public static class IQueryableExtension
    {
        public static IQueryable Select(this IQueryable source, IList<MemberExpression> exprList)
        {
            var parameter = Expression.Parameter(source.ElementType, "x");
            var lambda = parameter.CreateLambdaExpresion(exprList);
            return source.Select(lambda);
        }

        public static void ForTest<TEntity, TResult>(this IQueryable<TEntity> query, Expression<Func<TEntity, TResult>> expr)
        {
            var param  = expr.Parameters.Single();
            var body = expr.Body as NewExpression;
            var q = Expression.MemberInit(Expression.New(body.Type));
            //Expression.MemberInit(q, param)
            //var lambda = Expression.Lambda(body, param);
        }

        public async static Task<TEntity> SingleAsync<TEntity>(this IQueryable<TEntity> query, int id) where TEntity : class
        {
            var entity = await query.SingleOrDefaultAsync(id);
            if (entity == null)
                throw new CaspianException("آیتم از سیستم حذف شده است");
            return entity;
        }

        public async static Task<TEntity> SingleOrDefaultAsync<TEntity>(this IQueryable<TEntity> query, int id) where TEntity : class
        {
            var param = Expression.Parameter(typeof(TEntity), "t");
            Expression expr = Expression.Property(param, typeof(TEntity).GetPrimaryKey());
            expr = Expression.Equal(expr, Expression.Constant(id));
            var lambda = Expression.Lambda(expr, param);
            return await query.Where(lambda).OfType<TEntity>().SingleOrDefaultAsync();
        }

        public async static Task<IList<TEntity>> GetValuesAsync<TEntity>(this IQueryable<TEntity> source, IList<MemberExpression> exprList)
        {
            var values = await source.Select(exprList).OfType<object>().ToListAsync();
            var list = new List<TEntity>();
            foreach(var value in values)
            {
                var entity = Activator.CreateInstance<TEntity>();
                foreach(var info in value.GetType().GetProperties().Where(t => t.Name != "Item"))
                    UpdateEntity(entity, info.Name, info.GetValue(value));
                list.Add(entity);
            }
            return list;
        }

        public static IQueryable GroupBy(this IQueryable source, LambdaExpression lambda)
        {
            return source.Provider.CreateQuery(
                Expression.Call(typeof(Queryable), "GroupBy", new Type[] { source.ElementType, lambda.Body.Type },
                    source.Expression, Expression.Quote(lambda)));
        }

        public static IQueryable ThenBy(this IQueryable source, LambdaExpression lambda)
        {
            return source.Provider.CreateQuery(
                Expression.Call(typeof(Queryable), "ThenBy", new Type[] { source.ElementType, lambda.Body.Type },
                source.Expression, Expression.Quote(lambda)));
        }

        public static IQueryable ThenByDescending(this IQueryable source, LambdaExpression lambda)
        {
            return source.Provider.CreateQuery(
                Expression.Call(typeof(Queryable), "ThenByDescending", new Type[] { source.ElementType, lambda.Body.Type },
                source.Expression, Expression.Quote(lambda)));
        }

        static Type CreateType(Type type)
        {
            var list = new List<DynamicProperty>();
            foreach(var info in type.GetProperties().Where(t => t.Name.StartsWith("Info__")))
                list.Add(new DynamicProperty(info.Name, info.PropertyType));
            return DynamicClassFactory.CreateType(list, false);
        }

        public async static Task<Tuple<IList<TEntity>, IList<object>>> AggregateValuesAsync<TEntity>(this IQueryable<TEntity> query, 
            IList<Expression> aggregateExpressions, int pageNumber, int pageSize)
        {
            if (pageNumber > 1)
                query = query.Skip((pageNumber - 1) * pageSize);
            query = query.Take(pageSize);
            var values = await query.CreateAggregateQuery(aggregateExpressions).OfType<object>().ToListAsync();
            var entities = new List<TEntity>();
            var list = new List<object>();
            if (values.Count > 0)
            {
                var type = CreateType(values[0].GetType());
                foreach (var value in values)
                {
                    var obj = Activator.CreateInstance(type);
                    var entity = Activator.CreateInstance<TEntity>();
                    foreach (var info in value.GetType().GetProperties())
                    {
                        if (info.Name.StartsWith("Info__"))
                            type.GetProperty(info.Name).SetValue(obj, info.GetValue(value));
                        else if (info.Name != "Item")
                            UpdateEntity(entity, info.Name, info.GetValue(value));
                    }
                    list.Add(obj);
                    entities.Add(entity);
                }
            }

            return new Tuple<IList<TEntity>, IList<object>>(entities, list);
        }

        public static IQueryable CreateAggregateQuery<TEntity>(this IQueryable<TEntity> query, IList<Expression> aggregateExpressions)
        {
            var exprList = new List<MemberExpression>();
            var aggregateList = new List<MethodCallExpression>();
            var properties = new List<DynamicProperty>();
            foreach (var item in aggregateExpressions)
            {
                var tempExpr = item;
                if (tempExpr.NodeType == ExpressionType.Convert)
                    tempExpr = (tempExpr as UnaryExpression).Operand;
                var isAggregate = false;
                if (tempExpr.NodeType == ExpressionType.Call)
                {
                    var callExpr = tempExpr as MethodCallExpression;
                    var type = callExpr.Arguments[0].Type;
                    if (type == typeof(IGrouping<TEntity, TEntity>))
                    {
                        properties.Add(new DynamicProperty("Info__" + properties.Count, callExpr.Method.ReturnType));
                        isAggregate = true;
                        aggregateList.Add(tempExpr as MethodCallExpression);
                    }
                }
                if (!isAggregate)
                {
                    var tempList = new ExpressionSurvey().Survey(item);
                    foreach (var expr2 in tempList)
                        if (!exprList.Any(t => t.ToString() == expr2.ToString()))
                            exprList.Add(expr2);
                }
            }
            var param = Expression.Parameter(typeof(TEntity), "t");
            var list = new List<MemberExpression>();
            foreach (var expr in exprList)
            {
                var path = expr.ToString();
                var index = path.IndexOf('.');
                path = path.Substring(index + 5);
                var type = expr.Type;
                if (expr.CheckConfilictByNullValue(out _))
                    type = typeof(Nullable<>).MakeGenericType(expr.Type);
                properties.Add(new DynamicProperty(path, type));
                list.Add(param.CreateMemberExpresion(path));
            }
            var lambda = param.CreateLambdaExpresion(list);
            var groupByQuery = query.GroupBy(lambda);
            /// Select Expresion
            var memberExprList = new List<MemberAssignment>();
            param = Expression.Parameter(groupByQuery.ElementType, "t");
            var selectType = DynamicClassFactory.CreateType(properties, false);
            foreach (var expr in exprList)
            {
                var path = expr.ToString();
                var index = path.IndexOf('.');
                path = path.Substring(index + 5);
                Expression infoExpr = Expression.Property(param, "Key");
                infoExpr = Expression.Property(infoExpr, path);
                var info = selectType.GetProperty(path);
                memberExprList.Add(Expression.Bind(info, infoExpr));
            }
            var infoIndex = 0;
            foreach (var aggregateLExpr in aggregateList)
            {
                var callExpr = aggregateLExpr as MethodCallExpression;
                var args = callExpr.Arguments.ToArray();
                args[0] = param;
                var expr = Expression.Call(callExpr.Method, args);
                var info = selectType.GetProperty("Info__" + infoIndex);
                memberExprList.Add(Expression.Bind(info, expr));
                infoIndex++;
            }
            var memberInit = Expression.MemberInit(Expression.New(selectType), memberExprList);
            var lambdaSelect = Expression.Lambda(memberInit, param);
            //var str = groupByQuery.Select(lambdaSelect).ToQueryString();
            return groupByQuery.Select(lambdaSelect);
        }

        public static void UpdateEntity(object entity, string path, object value)
        {
            var index = 1;
            var array = path.Split('.');
            object obj = entity;
            foreach(var str in array)
            {
                var info = obj.GetType().GetProperty(str);
                if (array.Length == index)
                    info.SetValue(obj, value);
                else if (info.GetValue(obj) == null)
                    info.SetValue(obj, Activator.CreateInstance(info.PropertyType));
                obj = info.GetValue(obj);
                index++;
            }
        }

        public static IQueryable Select(this IQueryable source, LambdaExpression lambda)
        {
            return source.Provider.CreateQuery(
                Expression.Call(typeof(Queryable), "Select", new Type[] { source.ElementType, lambda.Body.Type },
                source.Expression, Expression.Quote(lambda)));
        }

        public static IQueryable OrderBy(this IQueryable source, LambdaExpression lambda)
        {
            return source.Provider.CreateQuery(
                Expression.Call(typeof(Queryable), "OrderBy", new Type[] { source.ElementType, lambda.Body.Type },
                source.Expression, Expression.Quote(lambda)));
        }

        public static IQueryable OrderByDescending(this IQueryable source, LambdaExpression lambda)
        {
            return source.Provider.CreateQuery(
                Expression.Call(typeof(Queryable), "OrderByDescending", new Type[] { source.ElementType, lambda.Body.Type },
                source.Expression, Expression.Quote(lambda)));
        }

        //public static IQueryable<T> Where<T>(this IQueryable source, Expression lambda)
        //{
        //    return source.Provider.CreateQuery<T>(Expression.Call(typeof(Queryable), "Where",
        //        new Type[] { source.ElementType }, source.Expression, Expression.Quote(lambda)));
        //}

        public static IList ToIList(this IQueryable source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var list = (IList)Activator.CreateInstance(typeof(ArrayList));
            foreach (var item in source)
                list.Add(item);
            return list;
        }

        internal static IQueryable<TEntity> Search<TEntity>(this IQueryable<TEntity> source, TEntity search) where TEntity : class
        {
            if (search == null)
                return source;
            var list = new List<string>();
            GetSearchFieldsName(list, search, null);
            if (list.Count == 0)
                return source;
            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "t");
            Expression left = null;
            foreach (var fieldName in list)
            {
                var type = search.GetType();
                object value = search;
                Expression propertyExpr = parameter;
                PropertyInfo field = null;
                foreach (var item in fieldName.Split('.'))
                {
                    field = type.GetProperty(item);
                    propertyExpr = Expression.Property(propertyExpr, field);
                    value = field.GetValue(value);
                    type = field.PropertyType;
                }
                var foreignKeyFields = field.DeclaringType.GetProperties().Where(t => t.CustomAttributes
                    .Any(u => u.AttributeType == typeof(ForeignKeyAttribute)))
                    .Select(t => t.GetCustomAttribute<ForeignKeyAttribute>().Name);
                var constant = Expression.Constant(value, field.PropertyType);
                Expression right = null;
                if (foreignKeyFields.Any(t => t == field.Name) || field.PropertyType != typeof(string))
                    right = Expression.Equal(propertyExpr, constant);
                else
                {
                    var method = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
                    right = Expression.Call(propertyExpr, method, constant);
                }
                if (left == null)
                    left = right;
                else
                    left = Expression.AndAlso(left, right);
            }
            var lambda = Expression.Lambda(left, new ParameterExpression[] { parameter });
            return source.Provider.CreateQuery<TEntity>(
                Expression.Call(typeof(Queryable), "Where", new Type[] { source.ElementType },
                    source.Expression, Expression.Quote(lambda)));
        }

        public static IQueryable<TModel> Where<TModel>(this IQueryable<TModel> source, Expression lambda)
        {
            return source.Provider.CreateQuery<TModel>(Expression.Call(typeof(Queryable), "Where",
                new Type[] { source.ElementType }, source.Expression, Expression.Quote(lambda)));
        }

        public static IQueryable Where(this IQueryable source, Expression lambda)
        {
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Where",
                new Type[] { source.ElementType }, source.Expression, Expression.Quote(lambda)));
        }

        //public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source, Expression lambda)
        //{
        //    return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Where",
        //        new Type[] { source.ElementType }, source.Expression, Expression.Quote(lambda))).OfType<TEntity>();
        //}

        public static IQueryable Where_(this IQueryable source, Expression lambda)
        {
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Where",
                new Type[] { source.ElementType }, source.Expression, Expression.Quote(lambda)));
        }

        /// <summary>
        /// این متد لیست تمامی فیلدهایی را که باید در جستجو باشند را برمی گرداند
        /// </summary>
        private static void GetSearchFieldsName(IList<string> fieldsName, object search, string fieldName)
        {
            var type = search.GetType();
            foreach (var field in type.GetProperties())
            {
                var notMappedAttr = field.GetCustomAttribute<NotMappedAttribute>();
                type = field.PropertyType;
                if (notMappedAttr == null && type != typeof(bool) && type != typeof(bool?) && (!type.IsGenericType || type.IsValueType))
                {
                    var value = field.GetValue(search);
                    string str = field.Name;
                    if (fieldName.HasValue())
                        str = fieldName + '.' + str;

                    if (value != null)
                    {
                        var isEqualToDefault = true;
                        if (type.IsValueType)
                        {
                            object defaltValue = Activator.CreateInstance(type);
                            isEqualToDefault = value.Equals(defaltValue);
                        }
                        if (!type.CustomAttributes.Any(t => t.AttributeType == typeof(ComplexTypeAttribute)))
                        {
                            if (!isEqualToDefault || type.IsNullableType() || type == typeof(string))
                                fieldsName.Add(str);
                            else
                                if (!field.PropertyType.IsValueType)
                                GetSearchFieldsName(fieldsName, value, str);
                        }
                    }
                }
            }
        }
    }
}
