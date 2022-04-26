using Caspian.Common;

namespace Caspian.Engine.Service
{
    public class CheckTypeEngin
    {
        private TypeOperationData[] TypeOperationsData = new TypeOperationData[]
        {
            ///int {+, -, *} int
            new TypeOperationData(){FirstType = ValueTypeKind.Int, SecondType = ValueTypeKind.Int, 
                OperatorsType = new OperatorType[]{OperatorType.Plus, OperatorType.Minus, OperatorType.Stra}, Result = ValueTypeKind.Int},
            /// int {/} int
            new TypeOperationData(){FirstType = ValueTypeKind.Int, SecondType = ValueTypeKind.Int, Result = ValueTypeKind.Double, 
                OperatorsType = new OperatorType[]{OperatorType.Slash}},
            /// int {+, -, *, /} double
            new TypeOperationData(){FirstType = ValueTypeKind.Int, SecondType = ValueTypeKind.Double, Result = ValueTypeKind.Double,
                OperatorsType = new OperatorType[]{OperatorType.Plus, OperatorType.Minus, OperatorType.Stra, OperatorType.Slash}},
            ///  Int {+, -} Sanavat
            //new TypeOperationData(){FirstType = ValueTypeKind.Int, SecondType = ValueTypeKind.Sanavat, Result = ValueTypeKind.Sanavat,
               // OperatorsType = new OperatorType[]{OperatorType.Plus, OperatorType.Minus}},            
            /// int {>, >=, <, <=, ==, !=} int
            new TypeOperationData(){FirstType = ValueTypeKind.Int, SecondType = ValueTypeKind.Int, Result = ValueTypeKind.Bool,
                OperatorsType = new OperatorType[]{OperatorType.BT, OperatorType.BTE, OperatorType.LT, OperatorType.LTE, OperatorType.Equ, OperatorType.NotEqu}},
            /// int {>, >=, <, <=, ==, !=} double
            new TypeOperationData(){FirstType = ValueTypeKind.Int, SecondType = ValueTypeKind.Double, Result = ValueTypeKind.Bool,
                OperatorsType = new OperatorType[]{OperatorType.BT, OperatorType.BTE, OperatorType.LT, OperatorType.LTE, OperatorType.Equ, OperatorType.NotEqu}},
            /// Date {-} Date
            new TypeOperationData(){FirstType = ValueTypeKind.Date, SecondType = ValueTypeKind.Date, Result = ValueTypeKind.Int,
                OperatorsType = new OperatorType[]{OperatorType.Minus}},
            /// Date {>, >=, <, <=, ==, !=} Date
            new TypeOperationData(){FirstType = ValueTypeKind.Date, SecondType = ValueTypeKind.Date, Result = ValueTypeKind.Bool,
                OperatorsType = new OperatorType[]{OperatorType.BT, OperatorType.BTE, OperatorType.LT, OperatorType.LTE, OperatorType.Equ, OperatorType.NotEqu}},
            /// Sanavat {+, -} Int
            //new TypeOperationData(){FirstType = ValueTypeKind.Sanavat, SecondType = ValueTypeKind.Int, Result = ValueTypeKind.Sanavat,
            //    OperatorsType = new OperatorType[]{OperatorType.Plus, OperatorType.Minus}},            
            /// Sanavat {>, >=, <, <=, ==, !=} Sanavat
            //new TypeOperationData(){FirstType = ValueTypeKind.Sanavat, SecondType = ValueTypeKind.Sanavat, Result = ValueTypeKind.Bool,
            //    OperatorsType = new OperatorType[]{OperatorType.BT, OperatorType.BTE, OperatorType.LT, OperatorType.LTE, OperatorType.Equ, OperatorType.NotEqu}},
            /// bool {&, |} bool
            new TypeOperationData(){FirstType = ValueTypeKind.Bool, SecondType = ValueTypeKind.Bool, Result = ValueTypeKind.Bool,
                OperatorsType = new OperatorType[]{OperatorType.Or, OperatorType.And}},
        };

        private ValueTypeKind GetResult(ValueTypeKind firstType, ValueTypeKind secondType, OperatorType operatorType)
        {
            var temp = TypeOperationsData.SingleOrDefault(t => t.FirstType == firstType && t.SecondType == secondType &&
                t.OperatorsType.Contains(operatorType));
            if (temp == null)
                throw new Exception("عملگر" + operatorType.FaText() + " برای نوعهای" + firstType.FaText() + " و " + secondType.FaText() + "نامعتبر می باشد", null);
            return temp.Result;
        }

        //async public void CheckType(int ruleId, Type baseType)
        //{
        //    var queue = new Queue<Token>();
        //    using (var context = new MyContext())
        //    {
        //        var a = new RuleService(context);
        //        var rule = await a.SingleAsync(ruleId);
        //        foreach (var token in rule.Tokens)
        //        {
        //            if (token.TokenKind == TokenKind.If)
        //                queue.Enqueue(new Token() { EnTitle = "(", TokenType = TokenType.Oprator });
        //            else
        //            {
        //                if (token.TokenKind == TokenKind.QuestionMark)
        //                    queue.Enqueue(new Token() { EnTitle = ")", TokenType = TokenType.Oprator });
        //                queue.Enqueue(token);
        //            }
        //        }
        //        queue.InfixToPrefix();
        //        var stack = new Stack<ValueTypeKind>();
        //        foreach (var token in queue)
        //        {
        //            if (token.TokenType == TokenType.Oprator)
        //            {
        //                var type2 = stack.Pop();
        //                var type1 = stack.Pop();
        //                if (token.OperatorKind == OperatorType.QuestionMark)
        //                {
        //                    var type3 = stack.Pop();
        //                    if (type3 != ValueTypeKind.Bool)
        //                        throw new Exception("عملگر شرطی اگر فقط از نوع بولین می تواند باشد.", null);
        //                    if (type1 != type2)
        //                        throw new Exception("عملگر شرطی در حالت درست و نادرست باید نوع یکسانی داشته باشد.", null);
        //                    stack.Push(type1);
        //                }
        //                else
        //                    if (token.OperatorKind != OperatorType.Colon)
        //                        stack.Push(GetResult(type1, type2, token.OperatorKind.Value));
        //            }
        //            else
        //                stack.Push((await token.ValueType(baseType)).Value);
        //        }
        //        if (stack.Peek() != rule.ValueTypeKind)
        //            throw new Exception("خروجی فرمول از نوع " + stack.Peek().FaText() + " می باشد در صورتی که " + rule.ValueTypeKind.FaText() + " مشخص شده بود.", null);
        //    }
        //}
    }

    public class TypeOperationData
    {
        public ValueTypeKind FirstType { get; set; }

        public ValueTypeKind SecondType { get; set; }

        public OperatorType[] OperatorsType { get; set; }

        public ValueTypeKind Result { get; set; }
    }
}
