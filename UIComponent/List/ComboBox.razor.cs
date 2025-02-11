using Caspian.Common;
using System.Reflection;
using System.Collections;
using Microsoft.JSInterop;
using System.ComponentModel;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public partial class ComboBox<TEntity, TValue>: ComponentBase, IComboBox<TEntity>, IControl, IListValueInitializer, 
        IEnableLoadData where TEntity: class
    {
        bool LoadData, setToDefault, shouldRender = true, focused, fieldsAdd, disabled;
        int pageNumber = 1;
        string text, title, _FieldName;
        Expression cascadeExpression;
        Dictionary<string, object> attrs;
        WindowStatus? Status = WindowStatus.Close;
        WindowStatus? oldStatus = WindowStatus.Close;
        TValue OldValue = default(TValue);
        ValidationMessageStore _messageStore;
        IList items;
        IList<Expression> fieldsExpression;
        EditContext oldContext;

        internal int SelectedIndex { get; set; }

        public ElementReference? InputElement { get; private set; }

        [Parameter]
        public ICascadeService<TEntity> CascadeService { get; set; }

        [CascadingParameter]
        internal IEntitySearch EntitySearch { get; set; }

        public Expression<Func<TEntity, bool>> InternalConditionExpression { get; set; }

        [Parameter]
        public IEnumerable<SelectListItem> Source { get; set; }

        [CascadingParameter]
        public EditContext CurrentEditContext { get; set; }

        public string ErrorMessage { get; set; }

        [Parameter]
        public string Style { get; set; }

        [CascadingParameter(Name = "ParentForm")]
        internal ICaspianForm CaspianForm { get; set; }

        [Parameter]
        public int? ColSpan { get; set; }

        [Parameter]
        public int? TotalSpan { get; set; }

        [Parameter]
        public bool Pageable { get; set; } = true;

        [CascadingParameter]
        public CaspianContainer CaspianContainer { get; set; }

        [Parameter]
        public int PageSize { get; set; } = 30;

        [Parameter]
        public Func<IQueryable<TEntity>, string, IQueryable<TEntity>> OnDataBinding { get; set; }

        [Parameter]
        public EventCallback OnChanged { get; set; }

        [Parameter]
        public Expression<Func<TEntity, object>> OrderByExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, string>> TextExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, bool>> ConditionExpression { get; set; }

        [Parameter]
        public RenderFragment<TEntity> Template { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter]
        public Expression<Func<TValue>> ValueExpression { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public EventCallback OnChange { get; set; }

        [Parameter]
        public string Title { get; set; }

        public void Dispose()
        {
            InputElement = null;
        }

        async Task ToggelDropdownList()
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

        public EventCallback<object> OnInternalValueChanged { get; set; }


        public bool HasError()
        {
            return ErrorMessage != null;
        }

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
            }
            Status = WindowStatus.Close;
        }

        async Task OnNochangeKeyDown(KeyboardEventArgs e)
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
            else if (e.Key == "Enter")
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
            else if (e.Key != "Enter" && e.Key != "Tab")
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

        public void Focus()
        {
            focused = true;
        }

        protected override void OnInitialized()
        {
            CascadeService?.Initialize(this);
            text = "";
            if (ValueExpression != null)
            {
                var info = (ValueExpression.Body as MemberExpression).Member;
                title = info.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? info.Name;
            }
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            if (Title != null)
                title = Title;
            CaspianForm?.AddControl(this);
            CaspianContainer?.SetControl(this);
            disabled = CaspianContainer?.Disabled == true ? true : Disabled;
            base.OnParametersSet();
        }

        public void AddDataField(Expression expression)
        {
            fieldsAdd = true;
            fieldsExpression.Add(expression);
        }

        protected async override Task OnParametersSetAsync()
        {
            CaspianContainer?.SetControl(this);
            if (CurrentEditContext != null && CurrentEditContext != oldContext && ValueExpression != null)
            {
                var expr = ValueExpression.Body;
                string str = "";
                while (expr.NodeType == ExpressionType.MemberAccess)
                {
                    var memberExpr = expr as MemberExpression;
                    if (memberExpr.Member.DeclaringType.GetCustomAttribute<TableAttribute>() == null)
                        break;
                    else
                    {
                        if (str.Length > 0)
                            str = $".{str}";
                        str = memberExpr.Member.Name + str;
                        expr = memberExpr.Expression;
                    }
                }
                _FieldName = str;
                _messageStore = new ValidationMessageStore(CurrentEditContext);
                CurrentEditContext.OnValidationRequested += CurrentEditContext_OnValidationRequested;
                CurrentEditContext.OnFieldChanged += CurrentEditContext_OnFieldChanged;
                CurrentEditContext.OnValidationStateChanged += CurrentEditContext_OnValidationStateChanged;
                oldContext = CurrentEditContext;
            }
            attrs = new Dictionary<string, object>();
            if (disabled)
                attrs.Add("disabled", "disabled");
            if (Source != null)
                items = Source.ToList();
            SelectedIndex = -1;
            if (items == null)
            {
                if (Value == null || Value.Equals(default(TEntity)))
                {
                    text = "";
                }
                else if (!Value.Equals(OldValue))
                {
                    var qqq = typeof(TEntity);
                    if (OnInternalValueChanged.HasDelegate)
                        await OnInternalValueChanged.InvokeAsync(Value);
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
            }
            else
            {
                var index = 0;
                foreach (var item in items)
                {
                    var value = Template == null ? (item as SelectListItem).Value : typeof(TEntity).GetPrimaryKey().GetValue(item).ToString();
                    if (Value != null && Value.ToString() == value)
                    {
                        SelectedIndex = index;
                        break;
                    }
                    index++;
                }
                if (SelectedIndex >= 0 && SelectedIndex < items.Count)
                {
                    if (Template == null)
                        text = (items[SelectedIndex] as SelectListItem).Text;
                    else
                        text = TextExpression.Compile().Invoke(items[SelectedIndex] as TEntity).ToString();
                }
                else
                    text = "";
            }
            attrs["value"] = text;
            if (Id.HasValue())
            {
                attrs.Add("id", Id.Replace('.', '_'));
                attrs.Add("name", Id.Replace('.', '_'));
            }
            await base.OnParametersSetAsync();
        }

        private void CurrentEditContext_OnValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
        {
            _messageStore = new ValidationMessageStore(CurrentEditContext);
            var field = CurrentEditContext.GetType().GetField("_fieldStates", BindingFlags.NonPublic | BindingFlags.Instance);
            var states = (field.GetValue(CurrentEditContext) as System.Collections.IDictionary);
            foreach (dynamic state in states)
            {
                var fieldName = state.Key.FieldName as string;
                if (fieldName == _FieldName)
                {
                    var fieldIdentifier = new FieldIdentifier(CurrentEditContext.Model, fieldName);
                    var list = CurrentEditContext.GetValidationMessages();
                    var result = CurrentEditContext.GetValidationMessages(fieldIdentifier);
                    ErrorMessage = result.FirstOrDefault();
                }
                else if (fieldName.EndsWith("]." + _FieldName))
                {
                    var mainField = fieldName.Substring(0, fieldName.Length - _FieldName.Length);
                    mainField = mainField.Split('[')[0];
                    var model = CurrentEditContext.Model;
                    var details = model.GetType().GetProperty(mainField).GetValue(model) as System.Collections.IEnumerable;
                    var expr = ValueExpression.Body;
                    FieldInfo info = null;
                    while (expr.NodeType != ExpressionType.Constant)
                    {
                        if (expr.NodeType == ExpressionType.MemberAccess)
                        {
                            var member = (expr as MemberExpression).Member;
                            if (member.MemberType == MemberTypes.Field)
                                info = member as FieldInfo;
                            expr = (expr as MemberExpression).Expression;
                        }
                    }
                    var value = info.GetValue((expr as ConstantExpression).Value);
                    if (info.FieldType == typeof(RowData<>).MakeGenericType(details.ToDynamicList()[0].GetType()))
                        value = value.GetType().GetProperty("Data").GetValue(value);
                    var index = 0;
                    foreach (var detail in details)
                    {
                        if (detail == value)
                        {
                            var str = index == 0 ? fieldName : fieldName.Replace("[0]", '[' + index.ToString() + ']');
                            var fieldIdentifier = new FieldIdentifier(CurrentEditContext.Model, str);
                            var result = CurrentEditContext.GetValidationMessages(fieldIdentifier);
                            ErrorMessage = result.FirstOrDefault();
                        }
                        index++;
                    }
                    break;
                }
            }
            if ((ErrorMessage != null || !Validate()) && FormAppState.AllControlsIsValid)
            {
                FormAppState.AllControlsIsValid = false;
                FormAppState.Control = this;
                FormAppState.ErrorMessage = ErrorMessage;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
                CaspianForm?.SetFirstControl(this);
            base.OnAfterRender(firstRender);
        }

        private void CurrentEditContext_OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            
        }

        private void CurrentEditContext_OnValidationRequested(object sender, ValidationRequestedEventArgs e)
        {
            _messageStore = new ValidationMessageStore(CurrentEditContext);
            var field = CurrentEditContext.GetType().GetField("_fieldStates", BindingFlags.NonPublic | BindingFlags.Instance);
            var states = (field.GetValue(CurrentEditContext) as System.Collections.IDictionary);
            foreach (dynamic state in states)
            {
                var fieldName = state.Key.FieldName as string;
                if (fieldName == _FieldName)
                {
                    var fieldIdentifier = new FieldIdentifier(CurrentEditContext.Model, fieldName);
                    var result = CurrentEditContext.GetValidationMessages(fieldIdentifier);
                    ErrorMessage = result.FirstOrDefault();
                    if (ErrorMessage == null)
                        break;
                }
                else if (fieldName.EndsWith("]." + _FieldName))
                {
                    var mainField = fieldName.Substring(0, fieldName.Length - _FieldName.Length);
                    mainField = mainField.Split('[')[0];
                    var model = CurrentEditContext.Model;
                    var details = model.GetType().GetProperty(mainField).GetValue(model) as System.Collections.IEnumerable;
                    var expr = ValueExpression.Body;
                    FieldInfo info = null;
                    while (expr.NodeType != ExpressionType.Constant)
                    {
                        if (expr.NodeType == ExpressionType.MemberAccess)
                        {
                            var member = (expr as MemberExpression).Member;
                            if (member.MemberType == MemberTypes.Field)
                                info = member as FieldInfo;
                            expr = (expr as MemberExpression).Expression;
                        }
                    }
                    var value = info.GetValue((expr as ConstantExpression).Value);
                    var index = 0;
                    foreach (var detail in details)
                    {
                        if (detail == value)
                        {
                            var str = index == 0 ? fieldName : fieldName.Replace("[0]", '[' + index.ToString() + ']');
                            var fieldIdentifier = new FieldIdentifier(CurrentEditContext.Model, str);
                            var result = CurrentEditContext.GetValidationMessages(fieldIdentifier);
                            ErrorMessage = result.FirstOrDefault();
                        }
                        index++;
                    }
                    break;
                }
            }
            if (ErrorMessage == null && !Validate())
            {
                FormAppState.AllControlsIsValid = false;
                FormAppState.Control = this;
            }
            if (ErrorMessage != null && FormAppState.AllControlsIsValid)
            {
                FormAppState.AllControlsIsValid = false;
                FormAppState.Control = this;
            }
        }

        async Task DataBinding()
        {
            if (LoadData)
            {
                LoadData = false;
                
                if (Source == null)
                {
                    using var scope = ServiceScopeFactory.CreateScope();
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

                    if (text.HasValue() && Status == WindowStatus.Open)
                    {
                        if (OnDataBinding == null)
                        {
                            var param = TextExpression.Parameters[0];
                            var method = typeof(string).GetMethod("Contains", new Type[] { typeof(string) });
                            var expr = Expression.Call(TextExpression.Body, method, Expression.Constant(text));
                            var lambdaExpr = Expression.Lambda(expr, param);
                            query = query.Where(lambdaExpr).OfType<TEntity>();
                        }
                        else
                            query = OnDataBinding.Invoke(query, text);
                    }
                    IList<MemberExpression> list = null;
                    if (Template == null)
                        list = new ExpressionSurvey().Survey(TextExpression);
                    else
                    {
                        list = new List<MemberExpression>();
                        foreach (var fieldexpr in fieldsExpression)
                            list.AddRange(new ExpressionSurvey().Survey(fieldexpr).ToArray());
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
                    var lambda = parameter.CreateLambdaExpresion(list);
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

        public async Task ResetAsync()
        {
            await SetValue(default(TValue));
            ErrorMessage = null;
        }

        public bool Validate()
        {
            return true;
        }

        public async Task FocusAsync()
        {
            if (InputElement.HasValue)
                await InputElement.Value.FocusAsync();
        }

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
            if (focused)
            {
                focused = false;
                await InputElement.Value.FocusAsync();
            }
            if (ErrorMessage != null && FormAppState.AllControlsIsValid)
            {
                FormAppState.AllControlsIsValid = false;
                FormAppState.Control = this;
                FormAppState.ErrorMessage = ErrorMessage;
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task SetValue(object value)
        {
            var valueChanged = false;
            if (value != null)
            {
                var type = typeof(TValue).GetUnderlyingType();
                var convertedValue = (TValue)Convert.ChangeType(value, type);
                Value = convertedValue;
                await ValueChanged.InvokeAsync(convertedValue);
                valueChanged = true;
            }
            else
            {
                Value = default(TValue);
                await ValueChanged.InvokeAsync(default(TValue));
                valueChanged = true;
            }
            if (valueChanged)
            {
                if (OnChange.HasDelegate)
                    await OnChange.InvokeAsync();
                if (OnInternalValueChanged.HasDelegate)
                    await OnInternalValueChanged.InvokeAsync(Value);
            }
            if (CurrentEditContext != null && _FieldName.HasValue())
            {
                var model = CurrentEditContext.Model;
                var info = model.GetType().GetProperty(_FieldName);
                if (info == null)
                    FormAppState.AllControlsIsValid = false;
                else if (info != null)
                {
                    var field = new FieldIdentifier(CurrentEditContext.Model, _FieldName);
                    info.SetValue(model, Value);
                    CurrentEditContext.NotifyFieldChanged(field);
                }
            }
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
