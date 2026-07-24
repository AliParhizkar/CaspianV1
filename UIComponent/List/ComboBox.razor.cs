using Caspian.Common;
using System.Collections;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace Caspian.UI
{
    public partial class ComboBox<TEntity, TValue>: CBaseInput<TValue>, IComboBox<TEntity>, IControl, IListValueInitializer, 
        IEnableLoadData where TEntity: class
    {
        bool LoadData, setToDefault, shouldRender = true, fieldsAdd;
        int pageNumber = 1;
        string text;
        Expression cascadeExpression;
        Dictionary<string, object> attrs;
        WindowStatus? Status = WindowStatus.Close;
        WindowStatus? oldStatus = WindowStatus.Close;
        TValue OldValue = default;
        ValidationMessageStore _messageStore;
        IList items;
        IList<Expression> fieldsExpression;
        EditContext oldContext;

        internal int SelectedIndex { get; set; }

        [Parameter]
        public ICascadeService<TEntity> CascadeService { get; set; }

        public Expression<Func<TEntity, bool>> InternalConditionExpression { get; set; }

        [Parameter]
        public IEnumerable<SelectListItem> Source { get; set; }

        [Parameter]
        public bool Pageable { get; set; } = true;

        [Parameter]
        public int PageSize { get; set; } = 30;

        [Parameter]
        public Func<IQueryable<TEntity>, string, IQueryable<TEntity>> OnDataBinding { get; set; }

        [Parameter]
        public Expression<Func<TEntity, object>> OrderByExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, string>> TextExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, bool>> ConditionExpression { get; set; }

        [Parameter]
        public RenderFragment<TEntity> Template { get; set; }

        async Task ToggleDropdownList()
        {
            if (!disabled)
            {
                if (Status == WindowStatus.Close)
                {
                    shouldRender = false;
                    if (items == null)
                    {
                        LoadData = true;
                        await DataBinding();
                    }
                    Status = WindowStatus.Open;
                    focused = true;
                    shouldRender = true;
                }
                else
                    Status = WindowStatus.Close;
            }
        }

        EventCallback<object> IComboBox<TEntity>.OnInternalValueChanged { get; set; }

        public async Task SetValueAndClose(object item)
        {
            if (item != null)
            {
                if (Template == null)
                {
                    var temp = item as SelectListItem;
                    await SetValue(temp.Value);
                    text = temp.Text;
                }
                else
                {
                    var value = typeof(TEntity).GetPrimaryKey().GetValue(item);
                    await SetValue(value);
                    text = TextExpression.Compile().Invoke(item as TEntity);
                }
                if ((this as IComboBox<TEntity>).OnInternalValueChanged.HasDelegate)
                    await (this as IComboBox<TEntity>).OnInternalValueChanged.InvokeAsync(Value);
            }
            Status = WindowStatus.Close;
        }

        async Task NoChangeKeyDown(KeyboardEventArgs e)
        {
            shouldRender = false;
            if (e.Code == "ArrowDown" || e.Code == "ArrowUp")
            {
                if (e.Code == "ArrowDown")
                {
                    if (SelectedIndex == -1)
                        SelectedIndex = 0;
                    if (items.Count - 1 > SelectedIndex)
                        SelectedIndex++;
                    else
                        SelectedIndex = 0;
                }
                else
                {
                    if (SelectedIndex > 0)
                        SelectedIndex--;
                }
                shouldRender = true;
            }
            else if (e.Key == "Enter" || e.Key == "NumpadEnter")
            {
                if (SelectedIndex == -1)
                    SelectedIndex = 0;
                if (items != null && items.Count > SelectedIndex)
                {
                    if (Template == null)
                    {
                        var temp = items[SelectedIndex] as SelectListItem;
                        await SetValue(temp.Value);
                        text = temp.Text;
                    }
                    else
                    {
                        var temp = items[SelectedIndex] as TEntity;
                        await SetValue(typeof(TEntity).GetPrimaryKey().GetValue(temp));
                        text = TextExpression.Compile().Invoke(temp);
                    }
                    if (Status == WindowStatus.Open)
                        Status = WindowStatus.Close;
                }
                shouldRender = true;
            }
            else if (e.Code == "Backspace")
            {
                if (Value != null && !Value.Equals(default(TValue)))
                {
                    text = null;
                    setToDefault = true;
                    await SetValue(default(TValue));
                    pageNumber = 1;
                    shouldRender = true;
                }
            }
            if (e.Key == "Escape")
            {
                Status = WindowStatus.Close;
                shouldRender = true;
            }
            else if (e.Key != "Enter" && e.Key != "NumpadEnter" && e.Key != "Tab")
            {
                if (Status == WindowStatus.Close)
                    Status = WindowStatus.Open;
            }
        }

        protected override bool ShouldRender()
        {
            return shouldRender;
        }

        async Task OnChangeValue(ChangeEventArgs e)
        {
            if (setToDefault)
                setToDefault = false; 
            else
                text = Convert.ToString(e.Value);
            SelectedIndex = 0;
            pageNumber = 1;
            LoadData = true;
            await DataBinding();
        }

        protected override void OnInitialized()
        {
            CascadeService?.Initialize(this);
            text = "";
            base.OnInitialized();
        }

        void IListViewer<TEntity>.AddDataField(Expression expression)
        {
            fieldsAdd = true;
            fieldsExpression.Add(expression);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Value == null || Value.Equals(0))
                text = "";
            else if (!Value.Equals(OldValue))
            {
                OldValue = Value;
                if (Source == null)
                {
                    using var scope = ServiceScopeFactory.CreateScope();
                    var query = new BaseService<TEntity>(scope.ServiceProvider).GetAll();
                    var parameter = Expression.Parameter(typeof(TEntity));
                    Expression expr = Expression.Property(parameter, typeof(TEntity).GetPrimaryKey());
                    expr = Expression.Equal(expr, Expression.Constant(Value));
                    expr = Expression.Lambda(expr, parameter);
                    query = query.Where(expr).OfType<TEntity>();
                    string str = query.ToQueryString();
                    var list = new ExpressionSurvey().Survey(TextExpression);
                    var entity = (await query.GetValuesAsync(list)).FirstOrDefault();
                    if (entity == null)
                        text = null;
                    else
                        text = TextExpression.Compile().Invoke(entity);
                }
                else
                    text = Source.SingleOrDefault(t => t.Value == Value.ToString())?.Text;
            }
            await base.OnParametersSetAsync();
        }

        async Task DataBinding()
        {
            if (LoadData)
            {
                LoadData = false;
                if (Source == null)
                {
                    if (TextExpression == null)
                        throw new CaspianException("Please specify TextExpression parameter");
                    using var scope = ServiceScopeFactory.CreateScope();
                    scope.SetUserId(PageData);
                    var service = scope.GetService<BaseService<TEntity>>();
                    if (service == null)
                        throw new CaspianException($"Service of type IBaseService<{typeof(TEntity).Name}> not imilimented");
                    var query = service.GetAll();
                    var filterExpression = ConditionExpression;
                    if (InternalConditionExpression != null)
                        query = query.Where(InternalConditionExpression);
                    if (ConditionExpression != null)
                        query = query.Where(ConditionExpression);
                    if (OrderByExpression != null)
                    {
                        if (SortType == SortType.Decs)
                            query = query.OrderByDescending(OrderByExpression);
                        else
                            query = query.OrderBy(OrderByExpression);
                    }
                    if (OnDataBinding != null)
                        query = OnDataBinding.Invoke(query, text);
                    if (text.HasValue() && Status == WindowStatus.Open)
                    {
                        var param = TextExpression.Parameters[0];
                        var method = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
                        var expr = Expression.Call(TextExpression.Body, method, Expression.Constant(text));
                        var lambdaExpr = Expression.Lambda(expr, param);
                        query = query.Where(lambdaExpr).OfType<TEntity>();
                    }
                    IList<MemberExpression> list = null;
                    if (Template == null)
                        list = new ExpressionSurvey().Survey(TextExpression);
                    else
                    {
                        list = new List<MemberExpression>();
                        foreach (var fieldExpression in fieldsExpression)
                            list.AddRange(new ExpressionSurvey().Survey(fieldExpression).ToArray());
                    }
                    var type = typeof(TEntity);
                    query = query.Take(PageSize * pageNumber);
                    var parameter = Expression.Parameter(type, "t");
                    list = list.Select(t => parameter.ReplaceParameter(t)).ToList();
                    var primaryKey = type.GetPrimaryKey();
                    var primaryKeyExpr = Expression.Property(parameter, primaryKey);
                    var primaryKeyAdded = false;
                    foreach (var expr in list)
                    {
                        if (expr.Member == primaryKey)
                            primaryKeyAdded = true;
                    }
                    if (!primaryKeyAdded)
                        list.Add(primaryKeyExpr);
                    var lambda = parameter.CreateLambdaExtension(list, false);
                    if (cascadeExpression != null)
                        query = query.Where(cascadeExpression).OfType<TEntity>();
                    shouldRender = false;
                    var dataList = await query.GetValuesAsync(list);
                    shouldRender = true;
                    if (Template == null)
                    {

                        var displayFunc = TextExpression.Compile();
                        var valueFunc = Expression.Lambda(primaryKeyExpr, parameter).Compile();
                        items = new List<SelectListItem>();
                        foreach (var item in dataList)
                        {
                            var text = Convert.ToString(displayFunc.DynamicInvoke(item));
                            var value = Convert.ToString(valueFunc.DynamicInvoke(item));
                            items.Add(new SelectListItem(value, text));
                        }
                    }
                    else
                    {
                        items = dataList as IList;
                    }
                }
                else
                {
                    shouldRender = true;
                    items = Source.Where(t => !text.HasValue() || t.Text.Contains(text, StringComparison.OrdinalIgnoreCase)).Take(PageSize).ToList();
                }

            }
        }

        [Parameter]
        public SortType SortType { get; set; }

        public async Task IncPageNumber()
        {
            if (items.Count >= PageSize)
            {
                pageNumber++;
                LoadData = true;
                await DataBinding();
                StateHasChanged();
            }
        }

        public void Close()
        {
            Status = WindowStatus.Close;
            StateHasChanged();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotnet = DotNetObjectReference.Create(this);
                await jsRuntime.InvokeVoidAsync("caspian.common.bindComboBox", InputElement, Pageable, dotnet);
            }
            //if (focused)
            //{
            //    focused = false;
            //    await InputElement.Value.FocusAsync();
            //}
            //if (ErrorMessage != null && FormAppState.AllControlsIsValid)
            //{
            //    FormAppState.AllControlsIsValid = false;
            //    FormAppState.Control = this;
            //    FormAppState.ErrorMessage = ErrorMessage;
            //}
            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task<int?> UpdateCascadeComboBox()
        {
            LoadData = true;
            await DataBinding();
            if (items.Count == 1)
            {
                var item = items[0] as SelectListItem;
                text = item.Text;
                Value = (TValue)Convert.ChangeType(item.Value, typeof(TValue).GetUnderlyingType());
                if (!Value.Equals(OldValue))
                {
                    OldValue = Value;
                    if (ValueChanged.HasDelegate)
                        await ValueChanged.InvokeAsync(Value);
                    if (OnChange.HasDelegate)
                        await OnChange.InvokeAsync(Value);   
                }
                return Convert.ToInt32(Value);
            }
            text = "";
            Value = default;
            if (OldValue != null && !OldValue.Equals(0))
            {
                OldValue = Value;
                if (ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(Value);
                if (OnChange.HasDelegate)
                    await OnChange.InvokeAsync(Value);
            }
            return null;
        }

        public void EnableLoading()
        {
            LoadData = true;
            items = null; 
        }

        public async void Clear()
        {
            items?.Clear();
            text = "";
            Value = default;
            var isEqual = false;
            if (OldValue ==  null)
            {
                if (Value ==  null) 
                    isEqual = true;
                else 
                    isEqual = false;
            }
            else
                isEqual = OldValue.Equals(Value);
            if (!isEqual) 
            { 
                if (ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(Value);
                if (OnChange.HasDelegate)
                    await OnChange.InvokeAsync();
            }
        }

        [JSInvokable]
        public async Task IncPageNumberInvokable()
        {
            await IncPageNumber();
        }

        [JSInvokable]
        public void CloseInvokable()
        {
            Close();
        }
    }
}
