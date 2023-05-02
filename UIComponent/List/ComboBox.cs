using System;
using System.Linq;
using Caspian.Common;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public partial class ComboBox<TEntity, TValue>: ICascading, IControl, IInputValueInitializer, IListValueInitializer, IEnableLoadData where TEntity: class
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
        IList<SelectListItem> Items;
        bool shouldRender = true;
        bool focused;
        protected ElementReference input;

        [Inject, JsonIgnore]
        protected IJSRuntime jsRuntime { get; set; }

        [JsonIgnore]
        internal int SelectedIndex { get; set; }

        [Parameter, JsonIgnore]
        public IEnumerable<SelectListItem> Source { get; set; }

        [CascadingParameter, JsonIgnore]
        public EditContext CurrentEditContext { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Inject, JsonIgnore]
        public FormAppState FormAppState { get; set; }

        [CascadingParameter(Name = "ParentForm"), JsonIgnore]
        internal ICaspianForm CaspianForm { get; set; }

        [CascadingParameter, JsonIgnore]
        public CaspianContainer Container { get; set; }

        [Parameter, JsonIgnore]
        public bool Pageable { get; set; } = true;

        [Parameter, JsonIgnore]
        public int PageSize { get; set; } = 30;

        [Parameter, JsonIgnore]
        public RenderFragment<TEntity> ChildContent { get; set; }

        [Parameter, JsonIgnore]
        public Func<IQueryable<TEntity>, string, IQueryable<TEntity>> OnDataBinding { get; set; }

        [Parameter, JsonIgnore]
        public EventCallback OnValueChanged { get; set; }

        [Parameter, JsonIgnore]
        public EnableLoadContiner EnableLoadContiner { get; set; }

        [Parameter, JsonIgnore]
        public CascadeService CascadeService { get; set; }

        [Parameter, JsonIgnore]
        public Expression<Func<TEntity, object>> OrderByExpression { get; set; }

        [Parameter, JsonIgnore]
        public Expression<Func<TEntity, string>> TextExpression { get; set; }

        [Parameter, JsonIgnore]
        public Expression<Func<TEntity, bool>> ConditionExpression { get; set; }

        [Parameter, JsonIgnore]
        public RenderFragment Template { get; set; }

        [Parameter, JsonIgnore]
        public TValue Value { get; set; }

        [Parameter, JsonIgnore]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter, JsonIgnore]
        public Expression<Func<TValue>> ValueExpression { get; set; }

        [Parameter, JsonIgnore]
        public string Id { get; set; }

        [Parameter, JsonProperty("disabled")]
        public bool Disabled { get; set; }

        [Parameter, JsonIgnore]
        public EventCallback OnChange { get; set; }

        async Task ToggelDropdownList()
        {
            if (!Disabled)
            {
                if (Status == WindowStatus.Close)
                {
                    shouldRender = false;
                    if (Items == null)
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

        public async Task SetValueAndClose(SelectListItem item)
        {
            if (!item.Disabled)
            {
                await SetValue(item.Value);
                text = item.Text;
                await Task.Delay(300);
                Status = WindowStatus.Close;
            }
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
                    if (Items.Count - 1 > SelectedIndex)
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
                if (Items != null && Items.Count > SelectedIndex)
                {
                    await SetValue(Items[SelectedIndex].Value);
                    text = Items[SelectedIndex].Text;
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
            CaspianForm?.AddControl(this);
            //LoadData = true;
            text = "";
            if (CurrentEditContext != null && ValueExpression != null)
            {
                _FieldName = (ValueExpression.Body as MemberExpression).Member.Name;
                _messageStore = new ValidationMessageStore(CurrentEditContext);
                CurrentEditContext.OnValidationRequested += CurrentEditContext_OnValidationRequested;
                //CurrentEditContext.OnFieldChanged += CurrentEditContext_OnFieldChanged;
                CurrentEditContext.OnValidationStateChanged += CurrentEditContext_OnValidationStateChanged;
            }
            base.OnInitialized();
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
                
            attrs = new Dictionary<string, object>();
            if (Disabled)
                attrs.Add("disabled", "disabled");
            if (Source != null)
                Items = Source.ToList();
            SelectedIndex = -1;
            if (Items == null)
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
                foreach (var item in Items)
                {
                    if (Value != null && Value.ToString() == item.Value)
                    {
                        SelectedIndex = index;
                        break;
                    }
                    index++;
                }
                if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
                    text = Items[SelectedIndex].Text;
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
                FormAppState.Element = input;
                FormAppState.ErrorMessage = ErrorMessage;
            }
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
                FormAppState.Element = input;
            }
            if (ErrorMessage != null && FormAppState.AllControlsIsValid)
            {
                FormAppState.AllControlsIsValid = false;
                FormAppState.Element = input;
            }
        }

        [Inject, JsonIgnore]
        public IServiceProvider Provider { get; set; }

        async Task DataBinding()
        {
            if (LoadData)
            {
                LoadData = false;
                
                if (Source == null)
                {
                    using var scope = ServiceScopeFactory.CreateScope();
                    var service = new BaseService<TEntity>(scope.ServiceProvider);
                    var query = service.GetAll(default(TEntity));
                    if (ConditionExpression != null)
                        query = query.Where(ConditionExpression);
                    //if (multiSelect)
                    //    total = query.Count();
                    if (OrderByExpression != null)
                    {
                        if (SortType == SortType.Decs)
                            query = query.OrderByDescending(OrderByExpression);
                        else
                            query = query.OrderBy(OrderByExpression);
                    }

                    if (text.HasValue())
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
                                        var list = new ExpressionSurvey().Survey(TextExpression);
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
                    var displayFunc = TextExpression.Compile();
                    var valueFunc = Expression.Lambda(pKeyExpr, parameter).Compile();
                    Items = new List<SelectListItem>();
                    foreach (var item in dataList)
                    {
                        var text = Convert.ToString(displayFunc.DynamicInvoke(item));
                        var value = Convert.ToString(valueFunc.DynamicInvoke(item));
                        Items.Add(new SelectListItem(value, text));
                    }
                }
                else
                {
                    Items = Source.Where(t => !text.HasValue() || t.Text.Contains(text)).Take(PageSize).ToList();
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
                        Items = null;

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
            await input.FocusAsync();
        }

        protected string ConvertToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            //Cascade?.Cascade?.CascadTo(typeof(TEntity), Value);
            base.OnAfterRender(firstRender);
        }

        public async Task IncPageNumber()
        {
            if (Items.Count >= PageSize)
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
                await jsRuntime.InvokeVoidAsync("$.caspian.bindComboBox", dotnet, input, Pageable);
            }
            if (focused)
            {
                focused = false;
                await input.FocusAsync();
            }
            //await jsRuntime.InvokeVoidAsync("$.caspian.serversideCombobox", input, ErrorMessage, Disabled, Status);
            if (valueChanged)
            {
                valueChanged = false;
                if (OnValueChanged.HasDelegate)
                    await OnValueChanged.InvokeAsync();
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
            Items = null;
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
