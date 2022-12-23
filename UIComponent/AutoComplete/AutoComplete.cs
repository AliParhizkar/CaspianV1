using System;
using System.Linq;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Forms;

namespace Caspian.UI
{
    public partial class AutoComplete<TEntity, TValue> : IControl where TEntity: class 
    {
        string Text;
        bool mustClear;
        string SearchStr;
        string _FieldName;
        EditContext oldContext;
        SearchState SearchState;
        ValidationMessageStore _messageStore;
        Dictionary<string, object> inputAttrs = new Dictionary<string, object>();
        bool focuced;

        void SetSearchValue(ChangeEventArgs e)
        {
            if (WindowStatus == WindowStatus.Close)
                WindowStatus = WindowStatus.Open;
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

        [Parameter, JsonIgnore]
        public bool HideHeader { get; set; }

        [Parameter, JsonIgnore]
        public RenderFragment ChildContent { get; set; }

        [Parameter, JsonIgnore]
        public string Title { get; set; } = "حستجو ...";

        [Parameter, JsonIgnore]
        public bool HideIcon { get; set; }

        [Parameter, JsonProperty("autoHide")]
        public bool AutoHide { get; set; }

        [Parameter, JsonIgnore]
        public bool Disabled { get; set; }

        [Parameter, JsonProperty("bindingType")]
        public BindingType BindingType { get; set; } = BindingType.OnInput;

        [Inject, JsonIgnore]
        protected IJSRuntime jsRuntime { get; set; }

        [CascadingParameter, JsonIgnore]
        public EditContext CurrentEditContext { get; set; }

        [JsonIgnore]
        private ElementReference SearchForm;

        [JsonIgnore]
        private ElementReference Input;

        [Parameter, JsonIgnore]
        public TValue Value { get; set; }

        private TValue Oldvalue { get; set; }

        [Parameter, JsonIgnore]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter, JsonIgnore]
        public Expression<Func<TValue>> ValueExpression { get; set; }
        
        [Parameter, JsonIgnore]
        public Expression<Func<TEntity, string>> TextExpression { get; set; }

        [JsonIgnore]
        public string Id { get; set; }

        [Parameter, JsonIgnore]
        public bool OpenOnFocus { get; set; }

        [Parameter, JsonIgnore]
        public bool CloseOnBlur { get; set; }

        [Parameter, JsonProperty("minCharForSearch")]
        public short? MinCharForSearch { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("type")]
        public string TextBoxType { get; private set; } = "string";

        [JsonProperty("status")]
        public WindowStatus WindowStatus { get; set; }

        [Parameter, JsonIgnore]
        public bool Required { get; set; }

        [Inject, JsonIgnore]
        public FormAppState FormAppState { get; set; }

        [CascadingParameter, JsonIgnore]
        public CaspianContainer Container { get; set; }

        [Parameter, JsonIgnore]
        public EventCallback<TValue> OnChange { get; set; }

        protected override void OnInitialized()
        {
            SearchState = new SearchState();
            SearchState.EntityType = typeof(TEntity);
            WindowStatus = WindowStatus.Close;
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
                FormAppState.Element = Input;
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
                FormAppState.Element = Input;
            }
            if (ErrorMessage != null && FormAppState.AllControlsIsValid)
            {
                FormAppState.AllControlsIsValid = false;
                FormAppState.Element = Input;
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

        public void CloseHelpForm()
        {
            WindowStatus = WindowStatus.Close;
            StateHasChanged();
        }

        async Task OnKeyUpHandler(KeyboardEventArgs e)
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
                        await SetValue(SearchState.Grid.SelectedRowId.Value);
                    break;
                case "Escape":
                    WindowStatus = WindowStatus.Close;
                    break;
                case "Backspace":
                    if (!Value.Equals(default(TValue)))
                    {
                        Value = default(TValue);
                        Oldvalue = Value;
                        await ValueChanged.InvokeAsync(default(TValue));
                        if (OnChange.HasDelegate)
                            await OnChange.InvokeAsync(default(TValue));
                        mustClear = true;
                    }
                    SearchState?.Grid?.SelectFirstPage();
                    SearchState?.Grid?.SelectFirstRow();
                    break;
                default:
                    //if (WindowStatus == WindowStatus.Close)
                    //    WindowStatus = WindowStatus.Open;
                    //SearchState?.Grid?.SelectFirstPage();
                    //SearchState?.Grid?.SelectFirstRow();
                    break;
            }
        }

        public async Task ResetAsync()
        {
            await SetValue(0);
            ErrorMessage = null;
        }

        public async Task FocusAsync()
        {
            await Input.FocusAsync();
        }

        public void Focus()
        {
            
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
            inputAttrs["class"] = "t-input";
            inputAttrs["style"] = "direction:rtl";
            Container?.SetControl(this);
            if (OpenOnFocus)
            {
                inputAttrs.Add("onfocus", new Action(() =>
                {
                    WindowStatus = WindowStatus.Open;
                    SearchStr = "";
                }));
            }
            if (CloseOnBlur)
            {
                inputAttrs.Add("onblur", new Action(() =>
                {
                    WindowStatus = WindowStatus.Close;
                }));
            }
            if (Disabled)
                inputAttrs.Add("disabled", true);
            if (HideHeader)
                AutoHide = true;
            base.OnParametersSet();
        }

        protected string ConvertToJson()
        {
            var setting = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(this, setting);
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if ((ErrorMessage != null || !Validate()) && FormAppState.AllControlsIsValid)
            {
                FormAppState.AllControlsIsValid = false;
                FormAppState.Element = Input;
                FormAppState.ErrorMessage = ErrorMessage;
            }
            if (SearchState.Grid != null && !SearchState.Grid.OnInternalRowSelect.HasDelegate)
            {
                SearchState.Grid.OnInternalRowSelect = EventCallback.Factory.Create<int>(this, async (int id) =>
                {
                    await this.SetValue(id);
                });
            }
            var json = this.ConvertToJson();
            if (firstRender)
            {
                var data = new JsLookupValueSetter(this);
                await jsRuntime.InvokeVoidAsync("$.telerik.bindLookupValue", DotNetObjectReference.Create(data), Input);
            }
            await jsRuntime.InvokeVoidAsync("$.telerik.bindLookup", Input, SearchForm, json);

            if (focuced)
            {
                focuced = false;
                if (Input.Id != null)
                    await jsRuntime.InvokeVoidAsync("$.telerik.setFocuc", Input);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task SetValue(long id)
        {
            var type = typeof(TValue);
            if (type.IsNullableType())
                type = Nullable.GetUnderlyingType(type);
            var tempValue = Convert.ChangeType(id, type);
            Value = (TValue)tempValue;
            WindowStatus = WindowStatus.Close;
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(Value);
            if (OnChange.HasDelegate)
                await OnChange.InvokeAsync(Value);
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
