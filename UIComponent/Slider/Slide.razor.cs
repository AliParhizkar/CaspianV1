using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class Slide<TEntity> : ComponentBase, IDisposable where TEntity : class
    {
        IList<TEntity> items;
        Timer timer;
        int pageSize = 1;
        IList<Expression> fieldsExpression;
        ElementReference element;
        double? contentWidth, containerWidth;
        int index, shift;
        bool renderd;
        bool shouldRender = true;
        bool enableAnimate = true;
        int shiftCount = 1;

        public void AddDataField(Expression expression)
        {
            fieldsExpression.Add(expression);
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            
            base.OnInitialized();
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
            if (!renderd && element.Id.HasValue())
            {
                renderd = true;
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

        void ShiftRight()
        {
            if (shift != 0)
                return;
            enableAnimate = true;
            shouldRender = false;
            shift = -1;
            var task = Task.Run(async () =>
            {
                await Task.Delay(500);
                index++;
                shift = 0;
                enableAnimate = false;
                items = await Binddata(pageSize);
                await InvokeAsync(StateHasChanged);
            });
            StateHasChanged();
        }

        void ShiftLeft()
        {
            if (shift != 0)
                return;
            enableAnimate = true;
            shouldRender = false;
            shift = 1;
            var task = Task.Run(async () =>
            {
                await Task.Delay(500);
                index--;
                shift = 0;
                enableAnimate = false;
                items = await Binddata(pageSize);
                await InvokeAsync(StateHasChanged);
            });
            StateHasChanged();
        }

        async Task<IList<TEntity>> Binddata(int size)
        {
            using var service = scopeFactory.CreateScope().GetService<BaseService<TEntity>>();
            var query = service.GetAll();
            if (ConditionExpression != null)
                query = query.Where(ConditionExpression);
            var count = query.Count();
            var list = new List<TEntity>();
            if (index <= 0)
            {
                if (index <= -count)
                    index += count;
                var tempList = await query.Skip(count - 1 + index).ToListAsync();
                list.AddRange(tempList);
                if (tempList.Count < size + 2)
                {
                    tempList = await query.Take(size + 2 - tempList.Count).ToListAsync();
                    list.AddRange(tempList);
                }
            }
            else
            {
                var tempQuery = query;
                if (index > count)
                    index -= count;
                if (index > 1)
                    tempQuery = tempQuery.Skip(index - 1);
                var tempList = await tempQuery.Take(size + 2).ToListAsync();
                list.AddRange(tempList);
                if (tempList.Count < size + 2)
                {
                    tempList = await query.Take(size + 2 - tempList.Count).ToListAsync();
                    list.AddRange(tempList);
                }
            }
            return list;
        }

        public void Dispose()
        {
            if (timer != null)
                timer.Dispose();
        }
    }
}
