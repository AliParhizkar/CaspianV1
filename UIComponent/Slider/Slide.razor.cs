using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class Slide<TEntity> : ComponentBase where TEntity : class
    {
        IList<TEntity> items;
        int pageSize = 1;
        IList<Expression> fieldsExpression;
        ElementReference element;
        double? contentWidth, containerWidth;
        int index, shift;
        bool shouldRender = true;
        bool enableAnimate = true;

        public void AddDataField(Expression expression)
        {
            fieldsExpression.Add(expression);
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            items = await Binddata(1);
            await base.OnInitializedAsync();
        }

        [Parameter]
        public Expression<Func<TEntity, bool>> ConditionExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, object>> OrderByExpression { get; set; }

        [Parameter]
        public RenderFragment<TEntity> ChildContent { get; set; }

        [Parameter]
        public bool ShiftPageSize { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotnet = DotNetObjectReference.Create(this);
                await JSRuntime.InvokeVoidAsync("$.caspian.bindSlider", element, dotnet);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable]
        public async Task SetData(double containerWidth, double contentWidth)
        {
            var pageSize = (int)((containerWidth - 80) / contentWidth);
            this.contentWidth = contentWidth;
            this.containerWidth = containerWidth;
            if (this.pageSize != pageSize)
            {
                this.pageSize = pageSize;
                items = await Binddata(pageSize);
                StateHasChanged();
            }
        }

        protected override bool ShouldRender()
        {
            if (!shouldRender)
            {
                shouldRender = true;
                return false;
            }
            return base.ShouldRender();
        }

        async Task ShiftRight()
        {
            enableAnimate = true;
            shouldRender = false;
            shift = -1;
            items = await Binddata(pageSize + 2);
            var task = Task.Run(async () =>
            {
                await Task.Delay(500);
                index++;
                shift = 0;
                enableAnimate = false;
                items.RemoveAt(0);
                await InvokeAsync(StateHasChanged);
            });
        }

        async Task ShiftLeft()
        {

        }

        async Task<IList<TEntity>> Binddata(int size)
        {
            using var service = scopeFactory.CreateScope().GetService<BaseService<TEntity>>();
            var query = service.GetAll();
            if (ConditionExpression != null)
                query = query.Where(ConditionExpression);
            var count = query.Count();
            
            //if (OrderByExpression == null)
            //{
            //    var param = Expression.Parameter(typeof(TEntity), "t");
            //    Expression expr = Expression.Property(param, typeof(TEntity).GetPrimaryKey());
            //    var lambda = Expression.Lambda(expr, param);
            //    query = query.OrderBy(lambda).OfType<TEntity>();
            //}
            //else
            //    query = query.OrderBy(OrderByExpression);
            var shiftCount = ShiftPageSize ? pageSize : 1;
            shiftCount = index - shiftCount;
            var list = new List<TEntity>();
            if (shiftCount < 0)
            {
                var items = await query.Skip(count + shiftCount).ToListAsync();
                list.AddRange(items);
            }
            if (index > 0)
                query = query.Skip(index);
            query = query.Take(size);
            list.AddRange(await query.ToListAsync());
            return list;
        }
    }
}
