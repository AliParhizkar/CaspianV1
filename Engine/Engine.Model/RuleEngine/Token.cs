using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Engine
{
    [Table("Tokens", Schema = "cmn")]
    public class Token
    {
        [Key]
        public int Id { get; set; }

        public TokenType TokenType { get; set; }

        [StringLength(100)]
        public string EnTitle { get; set; }

        [StringLength(50)]
        public string FaTitle { get; set; }

        public string constValue { get; set; }

        public ValueTypeKind? ConstValueType { get; set; }

        public int? RuleId { get; set; }

        public int? RuleIdValue { get; set; }

        public ParameterType parameterType { get; set; }

        public int? DynamicParameterId { get; set; }

        [ForeignKey(nameof(DynamicParameterId))]
        public virtual DynamicParameter DynamicParameter { get; set; }

        [ForeignKey(nameof(RuleId))]
        public virtual Rule Rule { get; set; }

        [ForeignKey(nameof(RuleIdValue))]
        public virtual Rule RuleValue { get; set; }

        [NotMapped]
        public TokenKind TokenKind
        {
            get
            {
                if (TokenType == TokenType.Oprand || TokenType == TokenType.ConstValue)
                    return TokenKind.Parameter;
                switch (this.OperatorKind)
                {
                    case OperatorType.And:
                    case OperatorType.Or:
                    case OperatorType.Not: return TokenKind.Logical;
                    case OperatorType.BT:
                    case OperatorType.BTE:
                    case OperatorType.LT:
                    case OperatorType.LTE:
                    case OperatorType.Equ:
                    case OperatorType.NotEqu: return TokenKind.Compareable;
                    case OperatorType.CloseBracket: return TokenKind.CloseBracket;
                    case OperatorType.Colon: return TokenKind.Colon;
                    case OperatorType.If: return TokenKind.If;
                    case OperatorType.Plus:
                    case OperatorType.Minus:
                    case OperatorType.Stra:
                    case OperatorType.Slash: return TokenKind.Math;
                    case OperatorType.OpenBracket: return TokenKind.OpenBracket;
                    case OperatorType.QuestionMark: return TokenKind.QuestionMark;
                }
                return Engine.TokenKind.CloseBracket;
            }
        }

        public string GetEnTitle(OperatorType operatorType)
        {
            switch (operatorType)
            {
                case OperatorType.And: return "&";
                case OperatorType.BT: return ">";
                case OperatorType.BTE: return ">=";
                case OperatorType.CloseBracket: return ")";
                case OperatorType.Colon: return ":";
                case OperatorType.Equ: return "=";
                case OperatorType.LT: return "<";
                case OperatorType.LTE: return "<=";
                case OperatorType.Minus: return "-";
                case OperatorType.NotEqu: return "!=";
                case OperatorType.OpenBracket: return "(";
                case OperatorType.Or: return "|";
                case OperatorType.Plus: return "+";
                case OperatorType.QuestionMark: return "?";
                case OperatorType.Slash: return "/";
                case OperatorType.Stra: return "*";
                case OperatorType.If: return "if";
            }
            throw new NotImplementedException();
        }

        public string GetFaTitle(OperatorType operatorType)
        {
            switch (operatorType)
            {
                case OperatorType.And: return "&";
                case OperatorType.BT: return ">";
                case OperatorType.BTE: return "≥";
                case OperatorType.CloseBracket: return ")";
                case OperatorType.Colon: return "درغیراینصورت";
                case OperatorType.Equ: return "=";
                case OperatorType.LT: return "<";
                case OperatorType.LTE: return "≤";
                case OperatorType.Minus: return "-";
                case OperatorType.NotEqu: return "≠";
                case OperatorType.OpenBracket: return "(";
                case OperatorType.Or: return "|";
                case OperatorType.Plus: return "+";
                case OperatorType.QuestionMark: return "در اینصورت";
                case OperatorType.Slash: return "÷";
                case OperatorType.Stra: return "×";
                case OperatorType.If: return "اگر";
            }
            throw new NotImplementedException();
        }

        [NotMapped]
        public int IfCount { get; set; }

        [NotMapped]
        public bool IsSpecalToken
        {
            get
            {
                return (new TokenKind[] { TokenKind.If, TokenKind.QuestionMark, TokenKind.Colon }.Contains(TokenKind));
            }
        }

        [NotMapped]
        public OperatorType? OperatorKind
        {
            get
            {
                if (TokenType != Engine.TokenType.Oprator)
                    return null;

                switch (EnTitle)
                {
                    case "(": return OperatorType.OpenBracket;
                    case ")": return OperatorType.CloseBracket;
                    case "+": return OperatorType.Plus;
                    case "-": return OperatorType.Minus;
                    case "*": return OperatorType.Stra;
                    case "/": return OperatorType.Slash;
                    case "?": return OperatorType.QuestionMark;
                    case ":": return OperatorType.Colon;
                    case "&": return OperatorType.And;
                    case "|": return OperatorType.Or;
                    case "!": return OperatorType.Not;
                    case ">": return OperatorType.BT;
                    case "<": return OperatorType.LT;
                    case ">=": return OperatorType.BTE;
                    case "<=": return OperatorType.LTE;
                    case "==": return OperatorType.Equ;
                    case "!=": return OperatorType.NotEqu;
                    case "if": return OperatorType.If;
                }
                throw new NotImplementedException();
            }
        }


        //async public Task<ValueTypeKind?> ValueType(Type objectType)
        //{
        //    if (TokenType == TokenType.ConstValue)
        //        return ConstValueType.Value;
        //    if (RuleIdValue.HasValue)
        //    {
        //        using (var context = new MyContext())
        //        {
        //            return (await new RuleService(context).SingleAsync(RuleIdValue.Value)).ValueTypeKind;
        //        }
        //    }
        //    if (TokenType == TokenType.Oprand)
        //    {
        //        var type = objectType.GetMyProperty(EnTitle).PropertyType;
        //        if (type.IsNullableType())
        //            type = Nullable.GetUnderlyingType(type);
        //        if (type.IsEnum)
        //            return ValueTypeKind.Enum;
        //        if (type == typeof(int))
        //            return ValueTypeKind.Int;
        //        if (type == typeof(float) || type == typeof(double))
        //            return ValueTypeKind.Double;
        //        if (type == typeof(PersianDate))
        //            return ValueTypeKind.Date;
        //        throw new Exception("خطا:Type not suported");
        //    }
        //    return null;
        //}
    }
}
