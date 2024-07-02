using Caspian.Common;
using System.Reflection;
using System.Collections;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public partial class ComboBox<TEntity, TValue>: ICascading, IControl, IListValueInitializer, 
        IEnableLoadData where TEntity: class
    {
        bool LoadData;
        int pageNumber = 1;
        bool setToDefault;
        bool valueChanged;
        string className;
        string text;
        Expression cascadExpression;
        Dictionary<string, object> attrs;
        WindowStatus? Status = WindowStatus.Close;
        WindowStatus? oldStatus = WindowStatus.Close;
        TValue OldValue;
        string _FieldName;
        ValidationMessageStore _messageStore;
        IList items;
        bool shouldRender = true;
        bool focused;

        IList<Expression> fieldsExpression;
        bool fieldsAdd;
        EditContext oldContext;

        internal int SelectedIndex { get; set; }

        public ElementReference? InputElement { get; private set; }

        [Parameter]
        public IEnumerable<SelectListItem> Source { get; set; }

        [CascadingParameter]
        public EditContext CurrentEditContext { get; set; }

        public string ErrorMessage { get; set; }

        [Parameter]
        public string Style { get; set; }

        [CascadingParameter(Name = "ParentForm")]
        internal ICaspianForm CaspianForm { get; set; }

        [CascadingParameter]
        public CaspianContainer Container { get; set; }

        [Parameter]
        public bool Pageable { get; set; } = true;

        [Parameter]
        public int PageSize { get; set; } = 30;

        [Parameter]
        public Func<IQueryable<TEntity>, string, IQueryable<TEntity>> OnDataBinding { get; set; }

        [Parameter]
        public EventCallback OnChanged { get; set; }

        [Parameter]
        public EnableLoadContiner EnableLoadContiner { get; set; }

        [Parameter]
        public CascadeService CascadeService { get; set; }

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

        public void Dispose()
        {
            InputElement = null;
        }

        async Task ToggelDropdownList()
        {
            if (!Disabled)
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

        public void Enable()
        {
            Disabled = false;
        }

        public void Disable()
        {
            Disabled = true;
        }

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

            await Task.Delay(300);
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
            text = "";
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            CaspianForm?.AddControl(this);
            Container?.SetControl(this);
            Disabled = Container?.Disabled == true;
            base.OnParametersSet();
        }

        public void AddDataField(Expression expression)
        {
            fieldsAdd = true;
            fieldsExpression.Add(expression);
        }

        protected async override Task OnParametersSetAsync()
        {
            Container?.SetControl(this);
            className = "t-dropdown-wrap";
            if (Disabled)
                className += " t-state-disabled";
            else
            {
                if (ErrorMessage.HasValue())
                    className += " t-state-error";
                else
                    className += " t-state-default";
            }
            if (CurrentEditContext != null && CurrentEditContext != oldContext && ValueExpression != null)
            {
                _FieldName = (ValueExpression.Body as MemberExpression).Member.Name;
                _messageStore = new ValidationMessageStore(CurrentEditContext);
                CurrentEditContext.OnValidationRequested += CurrentEditContext_OnValidationRequested;
                //CurrentEditContext.OnFieldChanged += CurrentEditContext_OnFieldChanged;
                CurrentEditContext.OnValidationStateChanged += CurrentEditContext_OnValidationStateChanged;
                oldContext = CurrentEditContext;
            }
            attrs = new Dictionary<string, object>();
            if (Disabled)
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
                    var query = service.GetAll(default(TEntity));
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
                    var pkey = type.GetPrimaryKey();
                    var pKeyExpr = Expression.Property(parameter, pkey);
                    var pkeyAdded = false;
                    foreach (var expr in list)
                    {
                        if (expr.Member == pkey)
                            pkeyAdded = true;
                    }
                    if (!pkeyAdded)
                        list.Add(pKeyExpr);
                    var lambda = parameter.CreateLambdaExpresion(list);
                    if (cascadExpression != null)
                        query = query.Where(cascadExpression).OfType<TEntity>();
                    shouldRender = false;
                    var dataList = await query.GetValuesAsync(list);
                    shouldRender = true;
                    if (Template == null)
                    {
                        var displayFunc = TextExpression.Compile();
                        var valueFunc = Expression.Lambda(pKeyExpr, parameter).Compile();
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

        public void CascadTo(Type masterType, object value)
        {
            Disabled = value == null;
            if (value != null)
            {
                foreach (var info in typeof(TEntity).GetProperties())
                {
                    if (info.PropertyType == masterType)
                    {
                        var masterIdInfoName = info.GetCustomAttribute<ForeignKeyAttribute>().Name;
                        var masterInfo = typeof(TEntity).GetProperty(masterIdInfoName);
                        var param = Expression.Parameter(typeof(TEntity), "t");
                        Expression expr = Expression.Property(param, masterInfo);
                        expr = Expression.Equal(expr, Expression.Constant(value));
                        cascadExpression = Expression.Lambda(expr, param);
                        LoadData = true;
                        items = null;

                        break;
                    }
                }
            }
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
                await jsRuntime.InvokeVoidAsync("$.caspian.bindComboBox", dotnet, InputElement, Pageable);
            }
            if (focused)
            {
                focused = false;
                await InputElement.Value.FocusAsync();
            }
            if (valueChanged)
            {
                valueChanged = false;
                if (OnChanged.HasDelegate)
                    await OnChanged.InvokeAsync();
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
            EnableLoadContiner?.Control?.EnableLoading();
            if (value != null)
            {
                var type = typeof(TValue);
                if (type.IsNullableType())
                    type = Nullable.GetUnderlyingType(type);
                var convertedValue = (TValue)Convert.ChangeType(value, type);
                CascadeService?.Cascade?.CascadTo(typeof(TEntity), convertedValue);
                Value = convertedValue;
                await ValueChanged.InvokeAsync(convertedValue);
                valueChanged = true;
            }
            else
            {
                CascadeService?.Cascade?.CascadTo(typeof(TEntity), null);
                Value = default(TValue);
                await ValueChanged.InvokeAsync(default(TValue));
                valueChanged = true;
            }
            if (valueChanged && OnChange.HasDelegate)
                await OnChange.InvokeAsync();
            if (CurrentEditContext != null && _FieldName.HasValue())
            {
                var model = CurrentEditContext.Model;
                //FormAppState.AllControlsIsValid = true;
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

        public void EnableLoading()
        {
            LoadData = true;
            items = null;
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
