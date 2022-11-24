using Caspian.UI;
using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;

namespace Caspian.Engine.RuleGenerator
{
    public partial class IndexComponent
    {
        ParameterComponent parameterComponent;
        ConstValueComponent constValueComponent;
        OperatorComponent operatorComponent;

        Type Type;
        Token token;
        IList<Token> Tokens;
        TokenType TokenType;
        WindowStatus newStatus;
        IEnumerable<TokenKind> ValidTokensKind;
        IDictionary<string, object> OperandButtonAttrs;
        Caspian.Engine.Service.RuleEngine ruleEngine;
        ValueTypeKind? RuleTypeKind;
        Rule rule;

        void OpenOperandForm()
        {
            TokenType = TokenType.Oprand;
            newStatus = WindowStatus.Open;
        }

        void OpenConstantForm()
        {
            TokenType = TokenType.ConstValue;
            newStatus = WindowStatus.Open;
        }

        void OpratorAdded()
        {
            Tokens = operatorComponent.Tokens;
            ValidTokensKind = operatorComponent.ValidTokensKind;
            newStatus = WindowStatus.Close;
            StateHasChanged();
        }

        void AfterParameterAdded(IEnumerable<TokenKind> tokens, TokenType tokenType)
        {
            if (tokenType == TokenType.ConstValue)
                Tokens = constValueComponent.Tokens;
            else
                Tokens = parameterComponent.Tokens;
            ValidTokensKind = tokens;
            newStatus = WindowStatus.Close;
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            token = new Token();
            ruleEngine = new RuleEngine();
            OperandButtonAttrs = new Dictionary<string, object>();
            base.OnInitialized();
        }

        async protected override Task OnInitializedAsync()
        {
            using var scope = CreateScope();
            rule = await new RuleService(scope).SingleAsync(RuleId);
            RuleTypeKind = rule.ResultType;
            Tokens = await new TokenService(scope).GetAll().Include(t => t.RuleValue).Where(t => t.RuleId == RuleId).ToListAsync();
            new RuleEngine().UpdateTokens(Tokens);
            ValidTokensKind = new Parser(Tokens).ValidTokenKinds();

            await base.OnInitializedAsync();
        }

        [Parameter]
        public int RuleId { get; set; }

        async protected override Task OnParametersSetAsync()
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var rule = await new RuleService(scope).SingleAsync(RuleId);
            Type = new AssemblyInfo().GetModelType(rule.SystemKind, rule.TypeName);
            await base.OnParametersSetAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await jSRuntime.InvokeVoidAsync("$.telerik.fitMainToParent");
            await base.OnAfterRenderAsync(firstRender);
        }

    }
}
