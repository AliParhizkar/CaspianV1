using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class CheckedDropdownList<TEntity, TDetails> where TEntity : class
    {
        CheckedListbox<TEntity, TDetails> checkboxList;
        WindowStatus status;
        ElementReference element;
        string text;

        async Task ValuesChangedHandler(IList<TDetails> details)
        {
            text = checkboxList.SelectedItemsText();
            if (ValuesChanged.HasDelegate)
                await ValuesChanged.InvokeAsync(details);
        }

        protected override async Task OnInitializedAsync()
        {
            if (Service?.MasterId > 0)
            {
                var masterType = Service.GetType().GetGenericArguments()[0];
                var masterIdInfo = typeof(TDetails).GetForeignKey(masterType);
                var param = Expression.Parameter(typeof(TDetails), "t");
                Expression expr = Expression.Property(param, masterIdInfo);
                expr = Expression.Equal(expr, Expression.Constant(Service.MasterId));
                var lambda = Expression.Lambda(expr, param);
                using var service = ScopeFactory.CreateScope().GetService<IBaseService<TDetails>>();
                var query = service.GetAll().Where(lambda);
                var detailIdInfo = typeof(TDetails).GetForeignKey(typeof(TEntity));
                Expression selectExpr = Expression.Property(param, detailIdInfo);
                lambda = Expression.Lambda(selectExpr, param);
                var list = await query.Select(lambda).ToDynamicListAsync();
                await SetText(list);
            }
            await base.OnInitializedAsync();
        }

        async Task SetText(IList<dynamic> values)
        {
            if (Service?.MasterId > 0)
            {
                using var service = ScopeFactory.CreateScope().GetService<IBaseService<TEntity>>();
                var query = service.GetAll(default(TEntity));
                if (ConditionExpression != null)
                    query = query.Where(ConditionExpression);
                if (OrderByExpression != null)
                    query = query.OrderBy(OrderByExpression);
                var list = new ExpressionSurvey().Survey(TextExpression);
                var type = typeof(TEntity);
                var parameter = Expression.Parameter(type, "t");
                list = list.Select(t => parameter.ReplaceParameter(t)).ToList();
                var pkey = type.GetPrimaryKey();
                var pKeyExpr = Expression.Property(parameter, pkey);
                var pkeyAdded = false;
                foreach (var expr1 in list)
                {
                    if (expr1.Member == pkey)
                        pkeyAdded = true;
                }
                if (!pkeyAdded)
                    list.Add(pKeyExpr);
                var dataList = await query.GetValuesAsync(list);
                var displayFunc = TextExpression.Compile();
                var info = typeof(TEntity).GetPrimaryKey();
                var items = new List<SelectListItem>();
                string str = string.Empty;
                foreach (var item in dataList)
                {
                    var value = info.GetValue(item);
                    if (values.Contains(value))
                    {
                        if (str != string.Empty)
                            str += ", ";
                        str += Convert.ToString(displayFunc.DynamicInvoke(item));
                    }
                }
                text = str;
            }
        }

        void Open()
        {
            status = WindowStatus.Open;
        }

        [Parameter]
        public Expression<Func<TEntity, string>> TextExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, bool>> OrderByExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, bool>> ConditionExpression { get; set; }

        [Parameter]
        public string PlaceHolder { get; set; } = "جستجو";

        [Parameter]
        public bool Filterable { get; set; }

        [Parameter]
        public bool ShowSelectAll { get; set; }

        [Parameter]
        public string SelectAllText { get; set; } = "انتخاب همه";

        [Parameter]
        public ISimpleBatchService<TDetails> Service { get; set; }

        [Parameter]
        public IList<TDetails> Values { get; set; }

        [Parameter]
        public EventCallback<IList<TDetails>> ValuesChanged { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotnet = DotNetObjectReference.Create(this);
                await jSRuntime.InvokeVoidAsync("caspian.common.bindCheclistDropdown", element, dotnet);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable]
        public void CloseWindow()
        {
            status = WindowStatus.Close;
            StateHasChanged();
        }

    }
}
