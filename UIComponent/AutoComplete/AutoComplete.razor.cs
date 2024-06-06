using System;
using System.Linq;
using System.Reflection;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Caspian.Common.Service;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;

namespace Caspian.UI
{
    public partial class AutoComplete<TEntity, TValue> : IControl where TEntity: class
    {
        string Text;
        string oldText;
        TValue Oldvalue;
        bool mustClear;
        string SearchStr;
        bool shouldRender;
        string _FieldName;
        EditContext oldContext;
        SearchState SearchState;
        ValidationMessageStore _messageStore;
        Dictionary<string, object> inputAttrs = new Dictionary<string, object>();
        ElementReference input;
        WindowStatus status;

        void SetSearchValue(ChangeEventArgs e)
        {
            if (status == WindowStatus.Close)
                status = WindowStatus.Open;
            if (mustClear)
            {
                Text = "";
                SearchStr = "";
                mustClear = false;
            }
            else
            {
                Text = e.Value.ToString();
                SearchStr = Text;
            }
        }

        [Parameter]
        public bool HideHeader { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Title { get; set; } = "حستجو ...";

        [Parameter]
        public bool HideIcon { get; set; }

        [Parameter]
        public bool AutoHide { get; set; }

        public void Enable()
        {
            Disabled = false;
        }

        public bool HasError()
        {
            return ErrorMessage != null;
        }

        public void Disable()
        {
            Disabled = true;
        }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public BindingType BindingType { get; set; } = BindingType.OnInput;

        [CascadingParameter]
        public EditContext CurrentEditContext { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter]
        public Expression<Func<TValue>> ValueExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, string>> TextExpression { get; set; }

        public string Id { get; set; }

        [Parameter]
        public bool OpenOnFocus { get; set; }

        [Parameter]
        public bool CloseOnBlur { get; set; }

        public string ErrorMessage { get; set; }

        [Parameter]
        public bool Required { get; set; }

        [Inject]
        public FormAppState FormAppState { get; set; }

        [CascadingParameter]
        public CaspianContainer Container { get; set; }

        [Parameter]
        public EventCallback OnChange { get; set; }

        protected override void OnInitialized()
        {
            SearchState = new SearchState();
            shouldRender = true;
            status = WindowStatus.Close;
            base.OnInitialized();
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
            if (ErrorMessage != null && FormAppState.AllControlsIsValid)
            {
                FormAppState.AllControlsIsValid = false;
                //FormAppState.Control = this;
                FormAppState.ErrorMessage = ErrorMessage;
            }
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
                    var list = CurrentEditContext.GetValidationMessages();
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
                //FormAppState.Control = this;
            }
            if (ErrorMessage != null && FormAppState.AllControlsIsValid)
            {
                FormAppState.AllControlsIsValid = false;
                //FormAppState.Control = this;
            }
        }

        public bool Validate()
        {
            if (Required && (Value == null || Value.ToString() == ""))
            {
                ErrorMessage = "مقدار این فیلد اجباری است.";
                return false;
            }
            return true;
        }

        protected override bool ShouldRender()
        {
            if (shouldRender)
                return true;
            shouldRender = true;
            return false;
        }

        public async Task CloseHelpForm(bool sholdRender = false)
        {
            await Task.Delay(200);
            status = WindowStatus.Close;
            if (sholdRender)
                StateHasChanged();
        }

        [JSInvokable]
        public async Task Close()
        {
            await CloseHelpForm(true);
        }

        async Task OnKeyUp(KeyboardEventArgs e)
        {
            if (e.Code == "Enter" || e.Code == "NumpadEnter")
            {
                if (ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(Value);
                if (OnChange.HasDelegate)
                    await OnChange.InvokeAsync();
            }
            else
                shouldRender = false;
        }

        internal async Task<string> GetText(int value)
        {
            using var scope = ServiceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetService(typeof(IBaseService<TEntity>)) as BaseService<TEntity>;
            var t = Expression.Parameter(typeof(TEntity), "t");
            Expression expr = Expression.Property(t, typeof(TEntity).GetPrimaryKey());
            expr = Expression.Equal(expr, Expression.Constant(value)); 
            var result = await service.GetAll().Where(Expression.Lambda(expr, t)).Select(TextExpression).FirstAsync();
            return result;
        }

        async Task OnKeyDownHandler(KeyboardEventArgs e)
        {
            
            switch (e.Code)
            {
                case "ArrowUp":
                    SearchState?.Grid?.SelectPrevRow();
                    break;
                case "ArrowDown":
                    SearchState?.Grid?.SelectNextRow();
                    break;
                case "Enter":
                case "NumpadEnter":
                    if (SearchState?.Grid?.SelectedRowId != null)
                    {
                        var value = SearchState.Grid.SelectedRowId.Value;
                        Text = await GetText(value);
                        await SetValue(value, false);
                        await CloseHelpForm();
                    }
                    break;
                case "Escape":
                    status = WindowStatus.Close;
                    break;
                case "Backspace":
                    if (!Value.Equals(default(TValue)))
                    {
                        Value = default(TValue);
                        Oldvalue = Value;
                        await ValueChanged.InvokeAsync(default(TValue));
                        if (OnChange.HasDelegate)
                            await OnChange.InvokeAsync();
                        mustClear = true;
                    }
                    SearchState?.Grid?.SelectFirstPage();
                    SearchState?.Grid?.SelectFirstRow();
                    break;
                default:
                    shouldRender = false;
                    break;
            }
        }

        public async Task ResetAsync()
        {
            ErrorMessage = null;
            await SetValue(0);
        }

        public async Task FocusAsync()
        {
            await input.FocusAsync();
        }

        protected override void OnParametersSet()
        {
            SearchState.Value = Value;
            if (CurrentEditContext != null && CurrentEditContext != oldContext && ValueExpression != null)
            {
                _FieldName = (ValueExpression.Body as MemberExpression).Member.Name;
                _messageStore = new ValidationMessageStore(CurrentEditContext);
                CurrentEditContext.OnValidationRequested -= CurrentEditContext_OnValidationRequested;
                CurrentEditContext.OnValidationRequested += CurrentEditContext_OnValidationRequested;
                CurrentEditContext.OnValidationStateChanged -= CurrentEditContext_OnValidationStateChanged;
                CurrentEditContext.OnValidationStateChanged += CurrentEditContext_OnValidationStateChanged;
                oldContext = CurrentEditContext;
            }
            inputAttrs = new Dictionary<string, object>();
            inputAttrs["class"] = AutoHide ? "t-input auto-hide" : "t-input";
            Container?.SetControl(this);
            if (OpenOnFocus)
            {
                inputAttrs.Add("onfocus", new Action(() =>
                {
                    status = WindowStatus.Open;
                    SearchStr = "";
                }));
            }
            //if (CloseOnBlur)
            //{
            //    inputAttrs.Add("onblur", new Action(async () =>
            //    {
            //        await Task.Delay(200);
            //        status = WindowStatus.Close;
            //    }));
            //}
            if (Disabled)
                inputAttrs.Add("disabled", true);
            if (HideHeader)
                AutoHide = true;
            base.OnParametersSet();
        }

        protected override async Task OnParametersSetAsync()
        {
            await SetText();
            await base.OnParametersSetAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if ((ErrorMessage != null || !Validate()) && FormAppState.AllControlsIsValid)
            {
                FormAppState.AllControlsIsValid = false;
                //FormAppState.Control = this;
                FormAppState.ErrorMessage = ErrorMessage;
            }
            if (SearchState.Grid != null && !SearchState.Grid.OnInternalRowSelect.HasDelegate)
            {
                SearchState.Grid.OnInternalRowSelect = EventCallback.Factory.Create<int>(this, async (int id) =>
                {
                    await this.SetValue(id);
                });
            }
            if (firstRender)
            {
                var dotnet = DotNetObjectReference.Create(this);
                await jsRuntime.InvokeVoidAsync("$.caspian.bindLookup", dotnet, input);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        async Task SetText()
        {
            if (Value == null || Value.Equals(0))
                Text = "";
            else if (!Value.Equals(Oldvalue))
            {
                Oldvalue = Value;
                Text = await GetText(Convert.ToInt32(Value));
                oldText = Text;
            }
            else
                Text = oldText;
        }

        public async Task SetValue(long id, bool fireEvent = true)
        {
            if (!Disabled)
            {
                var type = typeof(TValue);
                if (type.IsNullableType())
                    type = Nullable.GetUnderlyingType(type);
                var tempValue = Convert.ChangeType(id, type);
                Value = (TValue)tempValue;
                await SetText();
                Value = (TValue)tempValue;
                if (fireEvent)
                {
                    if (ValueChanged.HasDelegate)
                        await ValueChanged.InvokeAsync(Value);
                    if (OnChange.HasDelegate)
                        await OnChange.InvokeAsync();
                }
            }
        }

        public void SetText(string text)
        {
            Text = text;
        }

        public void SetSearchStringValue(string searchStr)
        {
            SearchStr = searchStr;
            StateHasChanged();
        }
    }
}
