using System;
using System.Threading.Tasks;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class LookupWindow<TEntity, TValue> where TEntity:class
    {
        string oldSerachStringValue;
        Expression<Func<TEntity, bool>> SearchExpression;
        internal Expression<Func<TEntity, string>> textExpression;
        protected DataGrid<TEntity> Grid;
        protected TEntity SearchData;

        protected virtual void InitialSearchExpression(Expression<Func<TEntity, bool>> expr)
        {
            SearchExpression = expr;
        }

        protected void InitialTextExpression(Expression<Func<TEntity, string>> expr)
        {
            textExpression = expr;
        }


        protected override void OnInitialized()
        {
            SearchData = Activator.CreateInstance<TEntity>();
            base.OnInitialized();
        }

        [CascadingParameter]
        public AutoComplete<TValue> AutoComplete { get; set; }

        [Parameter]
        public Expression<Func<TEntity, string>> TextExpression { get; set; }   

        [CascadingParameter(Name = "LookupStringSearchValue")]
        public string LookupStringSearchValue { get; set; }

        [CascadingParameter(Name = "AutoComplateState")]
        public SearchState SearchState { get; set; }

        protected override void OnParametersSet()
        {
            if (TextExpression != null)
                textExpression = TextExpression;
            base.OnParametersSet();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Grid != null && SearchState != null)
            {
                if (Grid.InternalConditionExpr == null)
                {
                    
                    Grid.InternalConditionExpr = SearchExpression.Body;
                }
                if (Grid.SelectedRowId == null)
                    Grid.SelectFirstRow();
                SearchState.Grid = Grid;
                if (oldSerachStringValue != LookupStringSearchValue)
                {
                    oldSerachStringValue = LookupStringSearchValue;
                    Grid.EnableLoading();
                    await Grid.DataBind();
                }
            }
            await base.OnParametersSetAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (Grid != null)
            {
                Grid.HideInsertIcon = true;
                if (!Grid.OnInternalRowSelect.HasDelegate)
                {
                    Grid.OnInternalRowSelect = EventCallback.Factory.Create<int>(this, async (int id) =>
                    {
                        if (textExpression == null)
                            throw new InvalidOperationException("خطا: TextExpression is null you must set TextExpression in page");
                        using var scope = CreateScope();
                        var service = new SimpleService<TEntity>(scope.ServiceProvider);
                        var entity = await service.SingleAsync(id);
                        var text = textExpression.Compile().Invoke(entity);
                        AutoComplete.SetText(text);
                        await AutoComplete.SetValue(id);
                        
                        await AutoComplete.CloseHelpForm(true);
                    });
                }
            }
            base.OnAfterRender(firstRender);
        }
    }
}
