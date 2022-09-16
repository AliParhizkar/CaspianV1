using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common.Extension
{
    public static class ExpressionExtension
    {
        public static LambdaExpression CreateLambdaExpresion(this ParameterExpression param,params MemberExpression[] array)
        {
            return CreateLambdaExpresion(param, array.ToList());
        }



        public static LambdaExpression CreateLambdaExpresion(this ParameterExpression param, IList<MemberExpression> list)
        {
            var type = param.Type.CreateDynamicType(list);
            var memberExprList = new List<MemberAssignment>();
            foreach (var expr in list)
            {
                var str = expr.ToString();
                str = str.Substring(str.IndexOf('.') + 1);
                var info = type.GetProperty(str);
                Expression memberExpr = param.CreateMemberExpresion(str);
                if (expr.CheckConfilictByNullValue())
                {
                    Expression expr1 = param.CreateMemberExpresion("CustomerId");
                    var test = Expression.Equal(expr1, Expression.Constant(null));
                    var nullableType = typeof(Nullable<>).MakeGenericType(memberExpr.Type);
                    var ifTrue = Expression.Convert(Expression.Constant(null), nullableType);
                    var ifFalse = Expression.Convert(memberExpr, nullableType);
                    memberExpr = Expression.Condition(test, ifTrue, ifFalse);
                }
                var bindingExpr = Expression.Bind(info, memberExpr);
                memberExprList.Add(bindingExpr);
            }
            var memberInit = Expression.MemberInit(Expression.New(type), memberExprList);
            return Expression.Lambda(memberInit, param);
        }

        public static bool CheckConfilictByNullValue(this MemberExpression expr)
        {
            var type = expr.Type;
            if (type.IsValueType && !type.IsNullableType())
            {
                while(expr != null)
                {
                    var attr = expr.Member.GetCustomAttribute<ForeignKeyAttribute>();
                    if (attr != null)
                    {
                        var tempType = expr.Member.DeclaringType.GetProperty(attr.Name).PropertyType;
                        if (tempType.IsNullableType())
                            return true;
                    }
                    expr = expr.Expression as MemberExpression;
                }
            }
            return false;
        }

        /// <summary>
        /// این متد با توجه به مسیر ورودی خود یک <see cref="MemberExpression" تولید می کند/>
        /// </summary>
        public static MemberExpression CreateMemberExpresion(this ParameterExpression param, string path)
        {
            Expression expr = param;
            var type = param.Type;
            foreach (var str in path.Split('.'))
            {
                var info = type.GetProperty(str);
                if (info == null)
                    return null;
                type = info.PropertyType;
                expr = Expression.Property(expr, info);
            }
            return expr as MemberExpression;
        }

        public static MemberExpression ReplaceParameter(this ParameterExpression param, MemberExpression expr)
        {
            var path = expr.ToString();
            var index = path.IndexOf('.');
            path = path.Substring(index + 1);
            return param.CreateMemberExpresion(path);
        }

        public static Expression ReplaceParameter(this ParameterExpression param, Expression expr)
        {
            switch(expr.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                case ExpressionType.Add:
                    var binary = expr as BinaryExpression;
                    var left = param.ReplaceParameter(binary.Left);
                    var right = param.ReplaceParameter(binary.Right);
                    switch(expr.NodeType)
                    {
                        case ExpressionType.Equal:
                            return Expression.Equal(left, right);
                        case ExpressionType.AndAlso:
                            return Expression.AndAlso(left, right);
                        case ExpressionType.OrElse:
                            return Expression.OrElse(left, right);
                        case ExpressionType.Add:
                            if (left.Type == typeof(string) || right.Type == typeof(string))
                            {
                                var method = typeof(string).GetMethod("Concat", new Type[] { left.Type, right.Type });
                                return Expression.Call(null, method, left, right);
                            }
                            return Expression.Add(left, right);
                    }
                    throw new System.NotImplementedException("خطای عدم پیاده سازی");
                case ExpressionType.Convert:
                case ExpressionType.Not:
                    var convert = expr as UnaryExpression;
                    var operand = param.ReplaceParameter(convert.Operand);
                    if (expr.NodeType == ExpressionType.Convert) 
                        return Expression.Convert(operand, convert.Type);
                    return Expression.Not(operand);
                case ExpressionType.MemberAccess:
                    var member = expr as MemberExpression;
                    var memberExpr = param.ReplaceParameter(member.Expression);
                    return Expression.PropertyOrField(memberExpr, member.Member.Name);
                case ExpressionType.Parameter:
                    return param;
                case ExpressionType.Constant:
                    return expr;
                case ExpressionType.Call:
                    var call = expr as MethodCallExpression;
                    var args = call.Arguments.Select(t => param.ReplaceParameter(t));
                    Expression instatnse = call.Object == null ? null : param.ReplaceParameter(call.Object);
                    return Expression.Call(instatnse, call.Method, args);
                case ExpressionType.Lambda:
                    return expr;
            }
            throw new System.NotImplementedException("خطای عدم پیاده سازی");
        }

        public static LambdaExpression ReplaceParameter(this ParameterExpression param, LambdaExpression expr)
        {
            var body = param.ReplaceParameter(expr.Body as MemberExpression);
            return Expression.Lambda(body, param);
        }

        public static bool CompareMemberExpression(this MemberExpression expression1, MemberExpression expression2)
        {
            var str1 = expression1.ToString();
            var index = str1.IndexOf('.');
            str1 = str1.Substring(index + 1);
            var str2 = expression2.ToString();
            index = str2.IndexOf('.');
            str2 = str2.Substring(index + 1);
            return str1 == str2;
        }

        public static void CreateExpresion(this MemberExpression expr, Type type)
        {
            if (!(expr.Member as PropertyInfo).IsNullableType())
            {
                var path = expr.ToString();
                path = path.Substring(path.IndexOf('.') + 1);
                foreach(var str in path.Split('.'))
                {
                    var info = type.GetProperty(str);
                    var foreignKeyAttr = info.GetCustomAttribute<ForeignKeyAttribute>();
                    if (foreignKeyAttr != null)
                    {
                        if (type.GetProperty(foreignKeyAttr.Name).PropertyType.IsNullableType())
                        {

                        }

                    }
                    type = info.PropertyType;
                }
            }
        }
    }
}
