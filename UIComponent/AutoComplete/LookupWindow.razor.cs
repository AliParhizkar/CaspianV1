using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class LookupWindow<TEntity, TValue>: BasePage where TEntity:class
    {
        string oldSerachStringValue;
        Expression<Func<TEntity, bool>> SearchExpression;
        protected TEntity SearchData;
        DataGrid<TEntity> grid;

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

        [CascadingParameter]
        public IAutoComplete<TEntity> AutoComplete { get; set; }

        [Inject]
        public SimpleService<TEntity> Service { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (grid != null)
                await UpdateGrid();
            await base.OnParametersSetAsync();
        }

        async Task UpdateGrid()
        {
            if (grid.SelectedRowId == null)
                grid.SelectFirstRow();
            if (oldSerachStringValue != LookupStringSearchValue)
            {
                oldSerachStringValue = LookupStringSearchValue;
                grid.EnableLoading();
                await grid.DataBind();
            }
        }

        void BindGrid()
        {
            grid.InternalConditionExpr = SearchExpression.Body;
            AutoComplete.SetAndInitializeGrid(grid);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (grid == null)
            {
                grid = Service.DataView as DataGrid<TEntity>;
                BindGrid();
                await UpdateGrid();
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
