using System.Reflection;
using System.Collections;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Caspian.Common.Attributes;

namespace Caspian.Common.Extension
{
    public static class TypeExtension
    {
        public static Type[] NumericTypes = {typeof(byte), typeof(byte?), typeof(short), typeof(short?), typeof(int), typeof(int?), typeof(long), 
            typeof(long?), typeof(float?), typeof(float?), typeof(decimal), typeof(decimal?), typeof(double), typeof(double?) };
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsNumberType(this Type type)
        {
            return NumericTypes.Contains(type);
        }

        public static bool Is1To1Relation(this PropertyInfo info)
        {
            var type = info.PropertyType;
            if (type.GetUnderlyingType().IsValueType || type == typeof(string) || type == typeof(byte[]) || type.IsCollectionType())
                return false;
            if (info.GetCustomAttribute<ForeignKeyAttribute>() != null)
                return false;
            return true;
        }

        public static bool IsNullableType(this PropertyInfo info)
        {
            if (info.PropertyType.IsValueType)
                return IsNullableType(info.PropertyType);
            return new NullabilityInfoContext().Create(info).WriteState == NullabilityState.Nullable;
        }

        /// <summary>
        /// If type is enum and all that fields is power of 2 true else false
        /// </summary>
        public static bool IsMultiSelectEnum(this Type type)
        {
            type = type.GetUnderlyingType();
            return type.GetCustomAttribute<EnumTypeAttribute>() != null;
        }

        public static string GetMapPath(this Assembly assembly)
        {
            var path = Assembly.GetExecutingAssembly().Location;
            var index = path.IndexOf("\\bin\\");
            if (index > 0)
                return path.Substring(0, index);
            return path.Substring(0, path.IndexOf("\\Common.dll"));
        }

        public static bool IsEnumType(this Type type)
        {
            if (type.IsNullableType())
                return type.GetUnderlyingType().IsEnum;
            return type.IsEnum;
        }

        public static Type GetUnderlyingType(this Type type)
         {
            if (IsNullableType(type))
                return Nullable.GetUnderlyingType(type);
            return type;
        }

        public static PropertyInfo GetForeignKey(this PropertyInfo info)
        {
            return info.DeclaringType.GetProperties().Single(t => t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == info.Name);
        }

        public static PropertyInfo GetDetailsProperty(this Type type, Type detailType, string InversePropertyName = null)
        {
            var properties = type.GetProperties().Where(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == detailType && t.PropertyType.IsCollectionType());
            if (properties.Count() < 2)
                return properties.SingleOrDefault();
            if (InversePropertyName == null)
                throw new CaspianException($"There are {properties.Count()} properties of type ICollection<{detailType.Name}> and InversePropertyName is null");
            if (properties.Any(t => t.GetCustomAttribute<InversePropertyAttribute>() == null))
                throw new CaspianException($"There are properties of type ICollection<{detailType.Name}> that haven't InversePropertyAttribute please add it");
            return properties.Single(t => t.GetCustomAttribute<InversePropertyAttribute>().Property == InversePropertyName);
        }

        public static PropertyInfo GetDetailsProperty(this PropertyInfo detailInfo, Type type)
        {
            var properties = detailInfo.PropertyType.GetProperties().Where(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == type && t.PropertyType.IsCollectionType());
            if (properties.Count() == 0)
            {

            }
            if (properties.Count() < 2)
                return properties.SingleOrDefault();
            properties = properties.Where(t => t.GetCustomAttribute<InversePropertyAttribute>().Property == detailInfo.Name);
            if (properties.Count() != 1)
                throw new CaspianException($"There is more than a properties of type {detailInfo.PropertyType.Name} in type {type.Name} ");
            return properties.Single();
        }

        public static PropertyInfo GetForeignKey(this Type type, Type foreignKeyType, string inverseProperty = null)
        {
            var infos = type.GetProperties().Where(t => t.PropertyType == foreignKeyType);
            if (infos.Count() == 0)
                throw new CaspianException($"Type {type.Name} must a foreign key of type {foreignKeyType.Name}");
            PropertyInfo info = null;
            if (infos.Count()  > 1)
            {
                if (inverseProperty == null)
                    throw new CaspianException($"Type {type.Name} has {infos.Count()} foreign key of type {foreignKeyType.Name} and hasnt InversePropertyAttribute");
                info = infos.Single(t => t.Name == inverseProperty);
            }
            else
                info = infos.Single();
            var attr = info.GetCustomAttribute<ForeignKeyAttribute>();
            if (attr == null)
                throw new CaspianException($"Property {info.Name} must have  a ForeignKey Attribute of type {foreignKeyType.Name}");
            return type.GetProperty(attr.Name);
        }

        public static PropertyInfo GetOneToOnePropertyInfo(this Type type, Type detailType)
        {
            foreach(var info in type.GetProperties())
            {
                if (info.PropertyType == detailType)
                {
                    if (info.GetCustomAttribute<ForeignKeyAttribute>() == null)
                        return info;
                }
            }
            throw new InvalidProgramException("");
        }

        public static IList<PropertyInfo> GetOneToOnePropertyInfos(this Type entityType)
        {
            var list = new List<PropertyInfo>();
            foreach (var info in entityType.GetProperties())
            {
                var type = info.PropertyType;
                if (!type.IsValueType && !type.IsGenericType && type != typeof(string) && type != typeof(byte[]))
                {
                    if (info.GetCustomAttribute<ForeignKeyAttribute>() == null)
                    {
                        if (info.PropertyType.GetProperties().Any(t => t.PropertyType == entityType))
                            list.Add(info);
                    }
                }
            }
            return list;
        }

        public static PropertyInfo GetPrimaryKey(this Type type, bool checkAnyType = false)
        {
            var keys = type.GetProperties().Where(t => t.CustomAttributes.Any(u => u.AttributeType == typeof(KeyAttribute))).ToList();
            if (keys.Count != 1 && !checkAnyType)
                throw new Exception("خطا:Type " + type.Name + " must be has a key");
            return keys.SingleOrDefault();
        }

        public static PropertyInfo GetMyProperty(this Type type, string strName)
        {
            var array = strName.Split(new char[] { '.' });
            var propertyInfo = type.GetProperty(array[0]);
            type = propertyInfo.PropertyType;
            for (int i = 1; i < array.Length; i++)
            {
                propertyInfo = type.GetProperty(array[i]);
                type = propertyInfo.PropertyType;
            }
            return propertyInfo;
        }

        public static Type CreateDynamicType(this Type mainType, IList<MemberExpression> exprList, bool isReport)
        {
            var properties = new List<DynamicProperty>();
            foreach (var expr in exprList)
            {
                var str = expr.ToString();
                var type = expr.Type;
                if (expr.CheckConfilictByNullValue(out _))
                    type = typeof(Nullable<>).MakeGenericType(expr.Type);
                str = str.Substring(str.IndexOf('.') + 1).NormalizePropertyPath(isReport);
                properties.Add(new DynamicProperty(str, type));
            }
            return DynamicClassFactory.CreateType(properties, false);
        }

        public static bool IsEnumerableType(this Type type)
        {
            return (type.GetInterface(nameof(IEnumerable)) != null);
        }

        public static bool IsCollectionType(this Type type)
        {
            if (type == typeof(string))
                return false;
            return type.GetInterface(nameof(IEnumerable)) != null;
        }

        public static bool IsCollectionType(this Type type, Type collectionType)
        {
            return (type.IsCollectionType() && type.IsGenericType && type.GenericTypeArguments[0] == collectionType) ;

        }
    }
}
