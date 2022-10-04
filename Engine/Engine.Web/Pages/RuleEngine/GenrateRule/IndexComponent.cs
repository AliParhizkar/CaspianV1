using Caspian.UI;
using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using static System.Formats.Asn1.AsnWriter;

namespace Caspian.Engine.RuleGenerator
{
    public partial class IndexComponent
    {
        int? ruleIdValue;
        int? dynamicParameterId;
        Enum enumConstValue;
        bool? boolConstValue;
        DateTime? dateConstValue;
        TimeSpan? timeConstValue;
        decimal? decimalConstValue;
        ParameterType parameterType = ParameterType.MainParameter;
        Type Type;
        Token token;
        int? tokenId;
        string message;
        IList<Token> Tokens;
        TokenType TokenType;
        WindowStatus newStatus;
        IEnumerable<Token> Params;
        IEnumerable<TokenKind> ValidTokensKind;
        IDictionary<string, object> IfButtonAttrs;
        IDictionary<string, object> MathButtonAttrs;
        IDictionary<string, object> ColonButtonAttrs;
        IDictionary<string, object> LogicButtonAttrs;
        IDictionary<string, object> CompareButtonAttrs;
        IDictionary<string, object> OperandButtonAttrs;
        IDictionary<string, object> OpenBracketButtonAttrs;
        IDictionary<string, object> CloseBracketButtonAttrs;
        IDictionary<string, object> QuestionMarkButtonAttrs;
        Caspian.Engine.Service.RuleEngine ruleEngine;
        ValueTypeKind? ValueTypeKind;
        Type enumType;
        ValueTypeKind? RuleTypeKind;
        Rule rule;

        void ClearWindow()
        {
            tokenId = null;
            ruleIdValue = null;
            timeConstValue = null;
            enumConstValue = null;
            boolConstValue = null;
            dateConstValue = null;
            decimalConstValue = null;
            dynamicParameterId = null;
        }

        async Task AddOperand()
        {
            Token tempToken = null;
            using var scope = ServiceScopeFactory.CreateScope();
            var ruleService = new RuleService(scope);
            var tokenService = new TokenService(scope);
            if (TokenType == TokenType.Oprand)
            {
                if (tokenId.HasValue)
                {
                    tempToken = Params.ElementAt(tokenId.Value - 1);
                    tempToken.RuleId = RuleId;
                    if (tempToken.RuleIdValue.HasValue)
                        tempToken.FaTitle = (await ruleService.SingleAsync(tempToken.RuleIdValue.Value)).Title;
                    else
                    {
                        var temp = ruleEngine.GetObjectTokens(Type).Single(t => t.Id == tempToken.Id);
                        tempToken.Id = 0;
                        tempToken.FaTitle = temp.FaTitle;
                        tempToken.EnTitle = temp.EnTitle;
                        await UnFinalState();
                        tempToken.TokenType = TokenType.Oprand;
                        await tokenService.AddAsync(tempToken);
                        await tokenService.SaveChangesAsync();
                        Tokens = await new TokenService(scope).GetAll().Where(t => t.RuleId == RuleId).ToListAsync();
                        Tokens = new RuleEngine().UpdateTokens(Tokens);
                        ValidTokensKind = new Parser(Tokens).ValidTokenKinds();
                        newStatus = WindowStatus.Close;
                    }
                    StateHasChanged();
                }
                else if (ruleIdValue.HasValue)
                {
                    var ruleTitle = (await ruleService.SingleAsync(ruleIdValue.Value)).Title;
                    await tokenService.AddAsync(new Token()
                    {
                        FaTitle = ruleTitle,
                        RuleId = RuleId,
                        RuleIdValue = ruleIdValue.Value,
                        TokenType = TokenType.Oprand,
                    });
                    await tokenService.SaveChangesAsync();
                    Tokens = await new TokenService(scope).GetAll().Where(t => t.RuleId == RuleId).ToListAsync();
                    Tokens = new RuleEngine().UpdateTokens(Tokens);
                    ValidTokensKind = new Parser(Tokens).ValidTokenKinds();
                    newStatus = WindowStatus.Close;
                    StateHasChanged();
                }
                else if (dynamicParameterId.HasValue)
                {
                    var dynamicParameter = await new DynamicParameterService(scope).SingleAsync(dynamicParameterId.Value);
                    await tokenService.AddAsync(new Token()
                    {
                        FaTitle = dynamicParameter.FaTitle,
                        RuleId = RuleId,
                        DynamicParameterId = dynamicParameterId.Value,
                        parameterType = ParameterType.DaynamicParameter,
                        TokenType = TokenType.Oprand,
                    });
                    await tokenService.SaveChangesAsync();
                    Tokens = await new TokenService(scope).GetAll().Where(t => t.RuleId == RuleId).ToListAsync();
                    Tokens = new RuleEngine().UpdateTokens(Tokens);
                    ValidTokensKind = new Parser(Tokens).ValidTokenKinds();
                    newStatus = WindowStatus.Close;
                    StateHasChanged();
                }
                else
                    message = "لطفا عملوند را مشخص نمایید";
            }
            else
            {
                object constValue = null;
                string faTitle = null;
                switch (this.ValueTypeKind)
                {
                    case Caspian.Engine.ValueTypeKind.Bool:
                        if (boolConstValue == null)
                            message = "مقدار ثابت باید مشخص باشد";
                        else
                        {
                            constValue = boolConstValue;
                            faTitle = boolConstValue.Value ? "درست" : "نادرست";
                        }
                        break;
                    case Caspian.Engine.ValueTypeKind.Date:
                        if (dateConstValue == null)
                            message = "تاریخ باید مشخص باشد";
                        else
                        {
                            constValue = dateConstValue;
                            faTitle = dateConstValue.ToPersianDateString();
                        }
                        break;
                    case Caspian.Engine.ValueTypeKind.Double:
                    case Caspian.Engine.ValueTypeKind.Int:
                        if (decimalConstValue == null)
                            message = "مقدار ثابت باید مشخص باشد";
                        else
                        {
                            constValue = decimalConstValue;
                            faTitle = decimalConstValue.Seprate3Digit();
                        }
                        break;
                    case Caspian.Engine.ValueTypeKind.Enum:
                        if (enumConstValue == null)
                            message = "مقدار ثابت باید مشخص باشد";
                        else
                        {
                            constValue = enumConstValue;
                            faTitle = enumConstValue.FaText();
                        }
                        break;
                    case Caspian.Engine.ValueTypeKind.Time:
                        if (timeConstValue == null)
                            message = "زمان باید مشخص باشد";
                        else
                        {
                            constValue = timeConstValue;
                            faTitle = timeConstValue.Value.ShortString();
                        }
                        break;
                }
                if (constValue != null)
                {
                    tempToken = new Token()
                    {
                        constValue = constValue.ToString(),
                        ConstValueType = ValueTypeKind,
                        RuleId = RuleId,
                        TokenType = TokenType.ConstValue,
                        FaTitle = faTitle
                    };
                    await tokenService.AddAsync(tempToken);
                    await tokenService.SaveChangesAsync();
                    Tokens = await new TokenService(scope).GetAll().Where(t => t.RuleId == RuleId).ToListAsync();
                    Tokens = new RuleEngine().UpdateTokens(Tokens);
                    ValidTokensKind = new Parser(Tokens).ValidTokenKinds();
                    newStatus = WindowStatus.Close;
                }
            }
        }

        async Task UnFinalState()
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var ruleService = new RuleService(scope);
            var rule = await ruleService.SingleAsync(RuleId);
            rule.Priority = null;
            await ruleService.SaveChangesAsync();
        }

        void OpenOperandForm()
        {
            ClearWindow();
            newStatus = WindowStatus.Open;
            TokenType = TokenType.Oprand;
            Params = ruleEngine.GetObjectTokens(Type);
        }

        async Task UpdateValueTypeKind()
        {
            var tokens = Tokens.Reverse().ToList();
            var scope = CreateScope();
            if (tokens.Count > 0)
            {
                var tokenKind = tokens[0].TokenKind;
                if (tokenKind == TokenKind.Compareable)
                {
                    if (tokens[0].OperatorKind.Value != OperatorType.Equ && tokens[1].RuleId.HasValue && tokens[1].DynamicParameterId == null)
                    {
                        
                        var rule = await new RuleService(scope).SingleAsync(tokens[1].RuleId.Value);
                        ValueTypeKind = rule.ResultType;
                    }
                    else
                    {
                        if (tokens[1].DynamicParameterId.HasValue)
                        {
                            var dynamicParameter = await new DynamicParameterService(scope).SingleAsync(tokens[1].DynamicParameterId.Value);
                            switch(dynamicParameter.ControlType.Value)
                            {
                                case ControlType.Numeric:
                                    ValueTypeKind = Caspian.Engine.ValueTypeKind.Int;
                                    break;
                                case ControlType.CheckBox:
                                    ValueTypeKind = Caspian.Engine.ValueTypeKind.Bool;
                                    break;

                            }
                            
                        }
                        else
                        {
                            var type = Type.GetMyProperty(tokens[1].EnTitle).PropertyType;
                            if (type.IsNullableType())
                                type = Nullable.GetUnderlyingType(type);
                            if (type.IsEnum)
                            {
                                ValueTypeKind = Caspian.Engine.ValueTypeKind.Enum;
                                enumType = type;
                            }
                            else if (type == typeof(DateTime))
                                ValueTypeKind = Caspian.Engine.ValueTypeKind.Date;
                            else if (type == typeof(TimeSpan))
                                ValueTypeKind = Caspian.Engine.ValueTypeKind.Time;
                            else if (type == typeof(bool))
                                ValueTypeKind = Caspian.Engine.ValueTypeKind.Bool;
                            else if (type == typeof(int))
                                ValueTypeKind = Caspian.Engine.ValueTypeKind.Int;
                            else
                                ValueTypeKind = Caspian.Engine.ValueTypeKind.Double;
                        }
                    }
                }
                else if (tokenKind == TokenKind.Math)
                    ValueTypeKind = Caspian.Engine.ValueTypeKind.Int;
                else if (tokenKind == TokenKind.QuestionMark || tokenKind == TokenKind.Colon)
                {
                    ValueTypeKind = RuleTypeKind;
                    if (ValueTypeKind == Engine.ValueTypeKind.Enum)
                        enumType = rule.SystemKind.GetEntityAssembly().GetTypes().Single(t => t.Name == rule.EnumTypeName);
                }
            }
            else
                ValueTypeKind = RuleTypeKind;
        }

        void OpenConstantForm()
        {
            ClearWindow();
            newStatus = WindowStatus.Open;
            TokenType = TokenType.ConstValue;
        }

        async void AddOperator(string enTitle)
        {
            var token = new Token()
            {
                TokenType = TokenType.Oprator,
                EnTitle = enTitle,
                RuleId = RuleId
            };
            switch (token.EnTitle)
            {
                case "*":
                    token.FaTitle = "×";
                    break;
                case "/":
                    token.FaTitle = "÷";
                    break;
                case "==":
                    token.FaTitle = "=";
                    break;
                case "?":
                    token.FaTitle = "دراینصورت";
                    break;
                case ":":
                    token.FaTitle = "در غیراینصورت";
                    break;
                case "if":
                    token.FaTitle = "اگر";
                    break;
                case ">=":
                    token.FaTitle = "≥";
                    break;
                case "<=":
                    token.FaTitle = "≤";
                    break;
                default:
                    token.FaTitle = enTitle;
                    break;
            };
            using var scope = CreateScope();
            var tokenService = new TokenService(scope);
            await tokenService.AddAsync(token);
            await tokenService.SaveChangesAsync();
            Tokens = await new TokenService(scope).GetAll().Where(t => t.RuleId == RuleId).ToListAsync();
            Tokens = new RuleEngine().UpdateTokens(Tokens);
            ValidTokensKind = new Parser(Tokens).ValidTokenKinds();
            await UpdateValueTypeKind();
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            token = new Token();
            ruleEngine = new RuleEngine();
            MathButtonAttrs = new Dictionary<string, object>();
            LogicButtonAttrs = new Dictionary<string, object>();
            CompareButtonAttrs = new Dictionary<string, object>();
            IfButtonAttrs = new Dictionary<string, object>();
            QuestionMarkButtonAttrs = new Dictionary<string, object>();
            ColonButtonAttrs = new Dictionary<string, object>();
            OpenBracketButtonAttrs = new Dictionary<string, object>();
            CloseBracketButtonAttrs = new Dictionary<string, object>();
            OperandButtonAttrs = new Dictionary<string, object>();
            base.OnInitialized();
        }

        async protected override Task OnInitializedAsync()
        {
            using var scope = CreateScope();
            rule = await new RuleService(scope).SingleAsync(RuleId);
            RuleTypeKind = rule.ResultType;
            Tokens = await new TokenService(scope).GetAll().Where(t => t.RuleId == RuleId).ToListAsync();
            new RuleEngine().UpdateTokens(Tokens);
            ValidTokensKind = new Parser(Tokens).ValidTokenKinds();

            await base.OnInitializedAsync();
        }

        async Task ClearAsync()
        {
            using var scope = CreateScope();
            await new TokenService(scope).RemoveAsync(RuleId);
            Tokens = await new TokenService(scope).GetAll().Where(t => t.RuleId == RuleId).ToListAsync();
            new RuleEngine().UpdateTokens(Tokens);
            ValidTokensKind = new Parser(Tokens).ValidTokenKinds();
            await UpdateValueTypeKind();
        }

        [Parameter]
        public int RuleId { get; set; }

        async protected override Task OnParametersSetAsync()
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var rule = await new RuleService(scope).SingleAsync(RuleId);
            Type = new AssemblyInfo().GetModelType(rule.SystemKind, rule.TypeName);
            await UpdateValueTypeKind();
            await base.OnParametersSetAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (message.HasValue())
            {
                await jSRuntime.InvokeVoidAsync("$.telerik.showMessage", message);
                message = null;
            }
            if (firstRender)
                await jSRuntime.InvokeVoidAsync("$.telerik.fitMainToParent");
            await base.OnAfterRenderAsync(firstRender);
        }

    }
}
