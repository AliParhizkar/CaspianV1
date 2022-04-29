using System;
using System.Threading.Tasks;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class LookupPage<TEntity> where TEntity:class
    {
        string oldSerachStringValue;
        Expression<Func<TEntity, bool>> SearchExpression;
        protected DataGrid<TEntity> Grid;
        protected TEntity SearchData;

        protected virtual void InitialSearchExpression(Expression<Func<TEntity, bool>> expr)
        {
            SearchExpression = expr;
        }

        protected void InitialTextExpression(Expression<Func<TEntity, string>> expr)
        {
            TextExpression = expr;
        }


        protected override void OnInitialized()
        {
            SearchData = Activator.CreateInstance<TEntity>();
            base.OnInitialized();
        }

        internal Expression<Func<TEntity, string>> TextExpression { get; private set; }   

        [CascadingParameter(Name = "LookupStringSearchValue")]
        public string LookupStringSearchValue { get; set; }

        [CascadingParameter(Name = "AutoComplateState")]
        public SearchState SearchState { get; set; }

        [CascadingParameter(Name = "MultiselectAutocomplete")]
        public MultiselectAutocomplete MultiselectAutocomplete { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (Grid != null && SearchState != null)
            {
                
                if (Grid.InternalConditionExpr == null)
                    Grid.InternalConditionExpr = SearchExpression.Body;

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

        protected void SetSearchExpression(Expression<Func<TEntity, bool>> expr)
        {
            SearchExpression = expr;
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
                        if (TextExpression == null)
                            throw new InvalidOperationException("خطا: TextExpression is null you must set TextExpression in page");
                        using var scope = CreateScope();
                        var service = new SimpleService<TEntity>(scope);
                        var entity = await service.SingleAsync(id);
                        var text = TextExpression.Compile().Invoke(entity);
                        MultiselectAutocomplete.AddToList(new SelectListItem(id.ToString(), text));
                    });
                }
            }
            base.OnAfterRender(firstRender);
        }
    }
}
