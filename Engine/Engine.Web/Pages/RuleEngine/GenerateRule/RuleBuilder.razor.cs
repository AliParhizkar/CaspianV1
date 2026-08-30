using Caspian.UI;
using Caspian.Common;
using Caspian.Engine;
using Syncfusion.Blazor.Data;
using Caspian.Engine.Service;
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
        ValueTypeKind? RuleTypeKind;

        [Parameter]
        public RenderFragment<TEntity> ChildContent { get; set; }

        [Parameter]
        public string UniqueId { get; set; }

        public void AddRuleData(string ruleItemPath, string title)
        {
            if (ruleItems.ContainsKey(ruleItemPath))
                throw new CaspianException($"آیتم :{ruleItemPath} قیلا اضافه شده است");
            ruleItems.Add(ruleItemPath, title);
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

        void OperatorAdded()
        {
            newStatus = WindowStatus.Close;
            StateHasChanged();
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
            //using var service = CreateScope().GetService<RuleService>();
            //var old = await service.GetAll().Include(t => t.Tokens).SingleOrDefaultAsync(t => t.UniqueId == UniqueId);
            //tokens = old.Tokens;
            //old = old ?? new Rule();
            //old.Title = Title;
            //old.TypeName = typeof(TEntity).Name;
            //old.ResultType = typeof(TValue).GetValueTypeKind();
            //old.UniqueId = UniqueId;
            //var result = await service.ValidateAsync(old);
            //if (result.IsValid)
            //{
            //    if (old.Id == 0)
            //        await service.AddAsync(old);
            //    else
            //        await service.UpdateAsync(old);
            //    await service.SaveChangesAsync();
            //    ruleId = old.Id;
            //}   
            //else
            //    ShowMessage(result.Errors[0].ErrorMessage);
            await base.OnInitializedAsync();
        }
    }
}
