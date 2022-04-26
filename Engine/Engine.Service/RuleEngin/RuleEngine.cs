using Caspian.Common;
using System.Reflection;
using System.Collections;
using Caspian.Common.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine.Service
{
    public class RuleEngine
    {
        public IList<Token> GetObjectTokens(Type type)
        {
            var tokens = new List<Token>();
            GetObjectTokens(tokens, type, "", "");
            int i = 1;
            foreach (var item in tokens)
            {
                item.Id = i;
                i++;
            }
            return tokens;
        }

        private void GetObjectTokens(IList<Token> tokens, Type type, string title, string faTitle)
        {
            foreach (var info in type.GetProperties())
            { 
                var attr = info.GetCustomAttribute<RuleAttribute>();
                if (attr != null)
                {
                    var name = info.Name;
                    if (title.HasValue())
                        name = title + '.' + name;
                    var faName = attr.Title;
                    if (faTitle.HasValue())
                        faName = faTitle + "•" + faName;
                    if (info.PropertyType.IsValueType || typeof(IEnumerable).IsAssignableFrom(info.PropertyType) ||
                        info.PropertyType.CustomAttributes.Any(t => t.AttributeType == typeof(ComplexTypeAttribute)))
                    {
                        var token = new Token();
                        token.TokenType = TokenType.Oprator;
                        token.FaTitle = faName;
                        token.EnTitle = name;
                        tokens.Add(token);
                    }
                    else
                        GetObjectTokens(tokens, info.PropertyType, name, faName);
                }
            }
        }

        public void Check(IList<Token> tokens)
        {
            var stack = new Stack<Token>();
            ///تعداد ؟ و :
            foreach (var token in tokens)
            {
                if (token.OperatorKind == OperatorType.QuestionMark)
                    stack.Push(token);
                else
                    if (token.OperatorKind == OperatorType.Colon)
                    {
                        if (stack.Count == 0)
                            throw new Exception("");
                        stack.Pop();
                    }
            }
            stack = new Stack<Token>();
            foreach (var token in tokens)
            {
                if (token.OperatorKind == OperatorType.OpenBracket)
                    stack.Push(token);
                else
                    if (token.OperatorKind == OperatorType.CloseBracket)
                    {
                        if (stack.Count == 0)
                            throw new Exception("");
                        stack.Pop();
                    }
                if (stack.Count != 0)
                {
                    if (token.OperatorKind == OperatorType.QuestionMark)
                        throw new Exception("");
                    if (token.OperatorKind == OperatorType.Colon)
                        throw new Exception("");
                }
            }
            if (stack.Count > 0)
                throw new Exception("");
        }

        public int GetPeriority(OperatorType operatorType)
        {
            switch (operatorType)
            {
                case OperatorType.QuestionMark: return 1;
                case OperatorType.Colon: return 2;
                case OperatorType.CloseBracket: return 3;
                case OperatorType.BT:
                case OperatorType.BTE:
                case OperatorType.LT:
                case OperatorType.LTE:
                case OperatorType.Equ:
                case OperatorType.NotEqu: return 6;
                case OperatorType.Or: return 4;
                case OperatorType.And: return 5;
                case OperatorType.Plus:
                case OperatorType.Minus: return 7;
                case OperatorType.Stra: 
                case OperatorType.Slash: return 8;
                case OperatorType.Not: return 9;
                case OperatorType.OpenBracket: return 10;
            }
            throw new Exception("خطا:OpratorType is Invalid", null);
        }

        public IList<Token> UpdateTokens(IList<Token> tokens)
        {
            var stack = new Stack<OperatorType>();
            foreach (var token in tokens)
            {
                if (token.OperatorKind == OperatorType.QuestionMark)
                    stack.Push(OperatorType.QuestionMark);
                if (token.OperatorKind == OperatorType.Colon)
                {
                    var topStack = stack.Peek();
                    if (topStack == OperatorType.QuestionMark)
                        stack.Push(OperatorType.Colon);
                    else
                    { 
                        while(stack.Count > 0 && stack.Peek() == OperatorType.Colon)
                        {
                            stack.Pop();
                            stack.Pop();
                        }
                        stack.Push(OperatorType.Colon);
                    }
                }
                token.IfCount = stack.Count(t => t == OperatorType.QuestionMark);
            }
            return tokens;
        }

        //public object Calculate(IList<Token> tokens, object obj, IList<Rule> rules)
        //{
        //    var queue = new Queue<Token>();
        //    foreach (var token in tokens)
        //    {
        //        if (token.TokenKind == TokenKind.If)
        //            queue.Enqueue(new Token() { EnTitle = "(", TokenType = TokenType.Oprator });
        //        else
        //        {
        //            if (token.TokenKind == TokenKind.QuestionMark)
        //                queue.Enqueue(new Token() { EnTitle = ")", TokenType = TokenType.Oprator });
        //            queue.Enqueue(token);
        //        }
        //    }
        //    return queue.Calculate(obj, rules);
        //}
    }

    public static class Extension
    {
        //public static object Calculate(this Queue<Token> queue, object obj, IList<Rule> rules)
        //{
        //    queue.InfixToPrefix();
        //    var stack = new Stack<object>();
        //    while (queue.Count > 0)
        //    {
        //        var item = queue.Dequeue();
        //        object value = null;
        //        if (item.TokenType == TokenType.Oprand)
        //        {
        //            if (item.EnTitle.HasValue())
        //            {
        //                try
        //                {
        //                    value = obj.GetMyValue(item.EnTitle);
        //                }
        //                catch
        //                {
        //                    return null;
        //                }
        //            }
        //            if (value != null && value.GetType().IsEnum)
        //                value = Convert.ToInt32(value);
        //            if (item.DynamicId.HasValue)
        //            {
        //                var list = value as IEnumerable<object>;
        //                var tempItem = list.SingleOrDefault(t => (int?)t.GetMyValue("RuleId") == item.DynamicId.Value);
        //                if (tempItem == null)
        //                {

        //                }
        //                else
        //                { 
        //                    var tempValue = tempItem.GetMyValue("Value");
        //                    var valueTypeKind = (ValueTypeKind?)tempItem.GetMyValue("Rule.ValueTypeKind");
        //                    switch (valueTypeKind.Value)
        //                    {
        //                        //case ValueTypeKind.Sanavat:
        //                        //    break;
        //                        case ValueTypeKind.Date:
        //                            value = new PersianDate(Convert.ToString(tempValue));
        //                            break;
        //                        case ValueTypeKind.Double:
        //                            value = Convert.ToDouble(tempValue);
        //                            break;
        //                        case ValueTypeKind.Int:
        //                            value = Convert.ToInt32(tempValue);
        //                            break;
        //                    }
        //                }
        //            }
        //            if (item.RuleIdValue.HasValue)
        //                value = rules.Single(t => t.Id == item.RuleIdValue.Value).Result;
        //        }
        //        else
        //            if (item.TokenType == TokenType.ConstValue)
        //            {
        //                switch (item.ConstValueType)
        //                { 
        //                    case ValueTypeKind.Date:
        //                        value = new PersianDate(item.constValue);
        //                        break;
        //                    case ValueTypeKind.Double:
        //                        value = Convert.ToDouble(item.constValue);
        //                        break;
        //                    case ValueTypeKind.Int:
        //                    case ValueTypeKind.Enum:
        //                        value = item.constValue.HasValue() ? Convert.ToInt32(item.constValue) : (int?)null;
        //                        break;
        //                    case ValueTypeKind.Bool:
        //                        value = Convert.ToBoolean(item.constValue);
        //                        break;
        //                }
                        
        //            }
        //        if (item.TokenType == TokenType.Oprator)
        //            stack.Operate(item.OperatorKind.Value);
        //        else
        //            stack.Push(value);
        //    }
        //    return stack.Pop();
        //}

        //public static void Operate(this Stack<object> stack, OperatorType operatorType)
        //{
        //    var item1 = stack.Pop();
        //    var item2 = stack.Pop();
        //    bool? item3 = operatorType == OperatorType.QuestionMark ? Convert.ToBoolean(stack.Pop()) : (bool?)null;
        //    object result = null;
        //    switch (operatorType)
        //    { 
        //        case OperatorType.And:
        //            result = Convert.ToBoolean(item2) && Convert.ToBoolean(item1);
        //            break;
        //        case OperatorType.Or:
        //            result = Convert.ToBoolean(item2) || Convert.ToBoolean(item1);
        //            break;
        //        case OperatorType.BT:
        //        case OperatorType.BTE:
        //        case OperatorType.LT:
        //        case OperatorType.LTE:
        //        case OperatorType.Equ:
        //        case OperatorType.NotEqu:
        //            result = Compare(item2, item1, operatorType);
        //            break;
        //        case OperatorType.QuestionMark:
        //            result = item3.Value == true ? item2 : item1;
        //            break;
        //        case OperatorType.Plus:
        //            result = Add(item2, item1);
        //            break;
        //        case OperatorType.Minus:
        //            result = Mines(item2, item1);
        //            break;
        //        case OperatorType.Stra:
        //            result = Star(item2, item1);
        //            break;
        //        case OperatorType.Slash:
        //            result = Slash(item2, item1);
        //            break;
        //    }
        //    stack.Push(result);
        //}

        //private static object Add(object item1, object item2)
        //{
        //    if (item1 == null || item2 == null)
        //        return null;
        //    if (item1 is int)
        //    {
        //        var param1 = Convert.ToInt32(item1);
        //        if (item2 is int)
        //            return param1 + Convert.ToInt32(item2);
        //        if (item2 is double)
        //            return param1 + Convert.ToDouble(item2);
        //        return null;
        //    }
        //    if (item1 is double)
        //        return Convert.ToDouble(item1) + Convert.ToDouble(item2);
        //    return null;
        //}

        //private static object Mines(object item1, object item2)
        //{
        //    if (item1 == null || item2 == null)
        //        return null;
        //    if (item1 is int)
        //    {
        //        var param1 = Convert.ToInt32(item1);
        //        if (item2 is int)
        //            return param1 - Convert.ToInt32(item2);
        //        if (item2 is double)
        //            return param1 - Convert.ToDouble(item2);
        //    }
        //    if (item1 is double)
        //        return Convert.ToDouble(item1) - Convert.ToDouble(item2);

        //    return (new PersianDate(Convert.ToString(item1)) - new PersianDate(Convert.ToString(item2))).Value;
        //}

        //private static object Star(object item1, object item2)
        //{
        //    if (item1 == null || item2 == null)
        //        return null;
        //    if (item1 is int && item2 is int)
        //        return Convert.ToInt32(item1) * Convert.ToInt32(item2);
        //    return Convert.ToDouble(item1) * Convert.ToDouble(item2);
        //}

        //private static object Slash(object item1, object item2)
        //{
        //    return Convert.ToDouble(item1) / Convert.ToDouble(item2);
        //}

        //private static bool Compare(object item1, object item2, OperatorType operatorType)
        //{
        //    if (item1 == null)
        //        return item2 == null;
        //    if (item2 == null)
        //        return item1 == null;
        //    if (item1 is int || item1 is double)
        //    {
        //        double param1 = Convert.ToDouble(item1), param2 = Convert.ToDouble(item2);
        //        switch (operatorType)
        //        {
        //            case OperatorType.BT: return param1 > param2;
        //            case OperatorType.BTE: return param1 >= param2;
        //            case OperatorType.LT: return param1 < param2;
        //            case OperatorType.LTE: return param1 <= param2;
        //            case OperatorType.Equ: return param1 == param2;
        //            case OperatorType.NotEqu: return param1 != param2;
        //        }
        //    }
        //    if (item1 is PersianDate)
        //    {
        //        PersianDate date1 = item1 as PersianDate, date2 = item2 as PersianDate;
        //        switch (operatorType)
        //        {
        //            case OperatorType.BT: return date1.CompareTo(date2) == 1;
        //            case OperatorType.BTE: return date1.CompareTo(date2) >= 0;
        //            case OperatorType.LT: return date1.CompareTo(date2) == -1;
        //            case OperatorType.LTE: return date1.CompareTo(date2) <= 0;
        //            case OperatorType.Equ: return date1 == date2;
        //            case OperatorType.NotEqu: return date1 != date2;
        //        }
        //    }
        //    if (item1 is bool)
        //    {
        //        bool param1 = (bool)item1, param2 = (bool)item2;
        //        switch (operatorType)
        //        {
        //            case OperatorType.Equ: return param1 == param2;
        //            case OperatorType.NotEqu: return param1 != param2;
        //        }
        //    }
        //    throw new Exception("حالت پیش بینی نشده");
        //}

        //public static void InfixToPrefix(this Queue<Token> queue)
        //{
        //    var tempQueue = new Queue<Token>();
        //    var stack = new Stack<Token>();
        //    while (queue.Count > 0)
        //    {
        //        var token = queue.Dequeue();
        //        if (token.TokenType == TokenType.Oprator)
        //            stack.PeriorityPush(tempQueue, token);
        //        else
        //            tempQueue.Enqueue(token);
        //    }
        //    while (stack.Count > 0)
        //        tempQueue.Enqueue(stack.Pop());
        //    while (tempQueue.Count > 0)
        //    {
        //        var item = tempQueue.Dequeue();
        //        if (item.OperatorKind != OperatorType.Colon)
        //            queue.Enqueue(item);
        //    }
        //}

        //public static void PeriorityPush(this Stack<Token> stack, Queue<Token> queue, Token token)
        //{
        //    ///چنانچه استک خالی باشد 
        //    if (stack.Count == 0)
        //        stack.Push(token);
        //    else
        //    {
        //        if (token.OperatorKind == OperatorType.QuestionMark)
        //            stack.Push(token);
        //        else
        //        {
        //            var topStack = stack.Peek().OperatorKind.Value;
        //            if (token.OperatorKind == OperatorType.Colon &&
        //                (topStack == OperatorType.Colon || topStack == OperatorType.QuestionMark))
        //            {
        //                ///اگر بالا پشته ؟ باشد پوش صورت می گیرد
        //                if (topStack == OperatorType.QuestionMark)
        //                    stack.Push(token);
        //                else
        //                {
        //                    ///اگر بالا پشته : باشد حتما توکن قبلی بالا پشته ؟ خواهد بود
        //                    stack.Pop();///:از استک حذف می شود
        //                    queue.Enqueue(stack.Pop());
        //                    PeriorityPush(stack, queue, token);
        //                }
        //            }
        //            else
        //                if (token.OperatorKind == OperatorType.CloseBracket)
        //                {
        //                    ///پرانتز بسته تا وقتی که به پرانتز باز نرسد از استک خالی کرده به صف اضافه می کند
        //                    while (stack.Peek().OperatorKind != OperatorType.OpenBracket)
        //                        queue.Enqueue(stack.Pop());
        //                    ///پرانتز باز باید از استک خارج شود
        //                    stack.Pop();
        //                }
        //                else
        //                {
        //                    var top = stack.Peek().OperatorKind;
        //                    ///پرانتز باز در داخل استک کمترین اولویت را دارد
        //                    var topPeriority = top == OperatorType.OpenBracket ? 0 : new RuleEngine().GetPeriority(top.Value);
        //                    var opratorPeriority = new RuleEngine().GetPeriority(token.OperatorKind.Value);
        //                    if (opratorPeriority <= topPeriority)
        //                    {
        //                        var item = stack.Pop();
        //                        queue.Enqueue(item);
        //                        PeriorityPush(stack, queue, token);
        //                    }
        //                    else
        //                        stack.Push(token);
        //                }
        //        }
        //    }
        //}

        //public static OperatorType? GetLatestConditionalOperator(this Stack<Token> stack)
        //{
        //    OperatorType? operatorType = null;
        //    for (int i = stack.Count - 1; i >= 0; i++)
        //        if (stack.ElementAt(i).OperatorKind == OperatorType.QuestionMark ||
        //            stack.ElementAt(i).OperatorKind == OperatorType.Colon)
        //            return stack.ElementAt(i).OperatorKind;
        //    return operatorType;
        //}
    }
}
