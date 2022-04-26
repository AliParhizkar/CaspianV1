using System;
using System.Threading.Tasks;
using Caspian.Common.Service;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
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

        protected override void OnInitialized()
        {
            SearchData = Activator.CreateInstance<TEntity>();
            base.OnInitialized();
        }

        [CascadingParameter(Name = "LookupStringSearchValue")]
        public string LookupStringSearchValue { get; set; }

        [CascadingParameter(Name = "AutoComplateState")]
        public SearchState SearchState { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (Grid != null)
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
    }
}
