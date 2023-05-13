using System;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class LookupWindow<TEntity, TValue> where TEntity:class
    {
        string oldSerachStringValue;
        Expression<Func<TEntity, bool>> SearchExpression;
        protected DataGrid<TEntity> Grid;
        protected TEntity SearchData;

        protected virtual void InitialSearchExpression(Expression<Func<TEntity, bool>> expr)
        {
            SearchExpression = expr;
        }

        protected override void OnInitialized()
        {
            SearchData = Activator.CreateInstance<TEntity>();
            base.OnInitialized();
        }

        [CascadingParameter]
        public AutoComplete<TValue, TEntity> AutoComplete { get; set; }

        [CascadingParameter(Name = "LookupStringSearchValue")]
        public string LookupStringSearchValue { get; set; }

        [CascadingParameter(Name = "AutoComplateState")]
        public SearchState SearchState { get; set; }

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
                        var text = await AutoComplete.GetText(id);
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
