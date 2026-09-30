using Caspian.UI;
using Caspian.Common;
using Caspian.Engine;
using Syncfusion.Blazor.Data;
using Caspian.Engine.Service;
using Caspian.Common.Extension;
using Caspian.Engine.Extension;
using Caspian.Engine.RuleGenerator;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.Engine
{
    public partial class RuleBuilder<TEntity, TValue> : BasePage
    {
        IDictionary<string, string> ruleItems;
        int ruleId;
        Token token;
        IList<Token> tokens;
        TokenType TokenType;
        WindowStatus newStatus;
        Caspian.Engine.Service.RuleEngine ruleEngine;
        ValueTypeKind? ruleTypeKind;

        [Parameter]
        public RenderFragment<TEntity> ChildContent { get; set; }

        [Parameter]
        public string UniqueId { get; set; }

        internal void AddRuleData(string ruleItemPath, string title)
        {
            if (ruleItems.ContainsKey(ruleItemPath))
                throw new CaspianException($"آیتم :{ruleItemPath} قیلا اضافه شده است");
            ruleItems.Add(ruleItemPath, title);
        }

        async Task LoadRule(Rule rule)
        {
            using var service = CreateScope().GetService<RuleService>();
            var old = await service.GetAll().Include(t => t.Tokens).SingleAsync(rule.Id);
            tokens = old.Tokens;
        }

        void TokenChanged() => StateHasChanged();

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

        void AfterParameterAdded(IEnumerable<TokenKind> tokens, TokenType tokenType)
        {
            newStatus = WindowStatus.Close;
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            token = new Token();
            ruleEngine = new Caspian.Engine.Service.RuleEngine();
            ruleItems = new Dictionary<string, string>();
            ruleTypeKind = typeof(TValue).GetValueTypeKind();
            base.OnInitialized();
        }

        bool OperandIsActive()
        {
            new RuleEngine().UpdateTokens(tokens);
            var validTokensKind = new Parser(tokens).ValidTokenKinds();
            return validTokensKind.Contains(TokenKind.Parameter);
        }

        protected override async Task OnInitializedAsync()
        {
            using var service = CreateScope().GetService<RuleService>();
            var old = await service.GetAll().Include(t => t.Tokens).SingleOrDefaultAsync(2);
            tokens = old.Tokens;

            await base.OnInitializedAsync();
        }
    }
}
