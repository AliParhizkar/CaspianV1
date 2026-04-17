using System.Linq.Expressions;
using Caspian.Common;
using Caspian.Engine.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public partial class LookupWindow<TEntity, TValue> where TEntity:class
    {
        string oldSearchStringValue;
        Expression<Func<TEntity, bool>> SearchExpression;
        protected TEntity SearchData;
        DataGrid<TEntity> grid;

        protected virtual void InitialSearchExpression(Expression<Func<TEntity, bool>> expr)
        {
            SearchExpression = expr;
        }

        [CascadingParameter]
        internal PageData PageData { get; set; }

        public IServiceScope CreateScope()
        {
            var scope = ServiceScopeFactory.CreateScope();
            scope.SetUserId(PageData?.UserId ?? 0);
            return scope;
        }

        protected override void OnInitialized()
        {
            SearchData = Activator.CreateInstance<TEntity>();
            var service = Service as IInternalSearchService<TEntity>;
            if (service != null)
            {
                service.HideFooter();
                service.OnlyForSearch();
                service.LookupInitializer(Lookup);
                (Lookup as Lookup<TEntity, TValue>).OnSelect = OnSelect;
            }
            base.OnInitialized();
        }

        protected Action<TEntity> OnSelect { get; set; }

        [CascadingParameter(Name = "LookupStringSearchValue")]
        public string LookupStringSearchValue { get; set; }

        [CascadingParameter]
        ILookup<TEntity> Lookup { get; set; }

        [Inject]
        public ISearchService<TEntity> Service { get; set; }

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
            if (oldSearchStringValue != LookupStringSearchValue)
            {
                oldSearchStringValue = LookupStringSearchValue;
                grid.EnableLoading();
                await grid.DataBind();
                
            }
        }

        void BindGrid()
        {
            grid.InternalConditionExpr = SearchExpression?.Body;
            Lookup.SetAndInitializeGrid(grid);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (grid == null)
            {
                grid = Service.DataView as DataGrid<TEntity> ;
                if (grid != null)
                {
                    BindGrid();
                    await UpdateGrid();
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
