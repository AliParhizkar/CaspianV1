using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Caspian.Common.Extension
{
    public static class IQueryableSearchExtension
    {
        public static IQueryable Search(this IQueryable source, object obj)
        {
            if (obj == null)
                return source;
            var list = new List<string>();
            GetSearchFieldsName(list, obj, null);
            if (list.Count == 0)
                return source;
            ParameterExpression parameter = Expression.Parameter(source.ElementType, "t");
            Expression left = null;
            foreach (var fieldName in list)
            {
                var type = obj.GetType();
                object value = obj;
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
            return source.Provider.CreateQuery(
                Expression.Call(typeof(Queryable), "Where", new Type[] { source.ElementType },
                    source.Expression, Expression.Quote(lambda)));
        }

        /// <summary>
        /// این متد لیست تمامی فیلدهایی را که باید در جستجو باشند را برمی گرداند
        /// </summary>
        private static void GetSearchFieldsName(IList<string> fieldsName, object obj, string fieldName)
        {
            var type = obj.GetType();
            foreach (var field in type.GetProperties())
            {
                var notMappedAttr = field.GetCustomAttribute<NotMappedAttribute>();
                if (notMappedAttr == null)
                {
                    var value = field.GetValue(obj);
                    string str = field.Name;
                    if (fieldName.HasValue())
                        str = fieldName + '.' + str;

                    if (value != null)
                    {
                        var isEqualToDefault = true;
                        if (field.PropertyType.IsValueType)
                        {
                            object defaltValue = Activator.CreateInstance(field.PropertyType);
                            isEqualToDefault = value.Equals(defaltValue);
                        }
                        if (!field.PropertyType.CustomAttributes.Any(t => t.AttributeType == typeof(ComplexTypeAttribute)))
                        {
                            if (!isEqualToDefault || field.PropertyType.IsNullableType() || field.PropertyType == typeof(string) || field.PropertyType == typeof(bool))
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
