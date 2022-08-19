﻿using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common.Extension
{
    public static class TypeExtension
    {
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
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
            if (type.IsEnumType())
            {
                var flag = true;
                foreach(var field in type.GetFields().Where(t => !t.IsSpecialName))
                {
                    var value = (int)field.GetValue(null);
                    if ((value & (value - 1)) != 0)
                    {
                        flag = false;
                        break;
                    }
                }
                return flag;
            }
            return false;
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

        public static Type CreateDynamicType(this Type mainType, IList<MemberExpression> exprList)
        {
            var properties = new List<DynamicProperty>();
            foreach (var expr in exprList)
            {
                var str = expr.ToString();
                str = str.Substring(str.IndexOf('.') + 1);
                properties.Add(new DynamicProperty(str, expr.Type));
            }
            return DynamicClassFactory.CreateType(properties);
        }

        public static bool IsEnumerableType(this Type type)
        {
            return (type.GetInterface(nameof(IEnumerable)) != null);
        }

        public static bool IsCollectionType(this Type type)
        {
            return (type.GetInterface(nameof(ICollection)) != null);
        }
    }
}
