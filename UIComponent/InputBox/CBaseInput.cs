using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using System.ComponentModel;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public class CBaseInput<TValue>: ComponentBase, IValidate, IControl
    {
        string _FieldName;
        ValidationMessageStore _messageStore;
        bool valueIsChanged, reseting;
        EditContext oldContext;
        protected bool disabled, focused, search;
        protected string title;

        public ElementReference? InputElement { get; protected set; }

        [Inject]
        public FormAppState FormAppState { get; set; }

        [CascadingParameter]
        internal IEntitySearch EntitySearch { get; set; }

        [Parameter]
        public int? ColSpan { get; set; }

        [Parameter]
        public int? TotalSpan { get; set; }

        public string ErrorMessage { get; set; }

        [Parameter]
        public BindingType BindingType { get; set; } = BindingType.OnChange;

        [Parameter]
        public Type DynamicType { get; set; }

        [Parameter]
        public bool Required { get; set; }

        [CascadingParameter]
        public EditContext CurrentEditContext { get; set; }

        [Inject]
        protected IJSRuntime jsRuntime { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public Dictionary<string, object> InputAttributes { get; set; }

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter]
        public Expression<Func<TValue>> ValueExpression { get; set; }

        [Parameter]
        public int? TabIndex { get; set; }

        [CascadingParameter(Name = "ParentForm")]
        internal ICaspianForm CaspianForm { get; set; }

        [Parameter]
        public EventCallback<CBaseInput<TValue>> OnDispose { get; set; }

        protected override void OnInitialized()
        {
            if (ValueExpression != null)
            {
                var member = (ValueExpression.Body as MemberExpression).Member;
                title = member.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? member.Name;
            }
            base.OnInitialized();
        }


        [CascadingParameter]
        internal PageData PageData { get; set; }

        protected string GetLabelCSSClassName()
        {
            if (TotalSpan.HasValue)
            {
                var className = PageData?.RightToLeft == true ? "ps-2" : "pe-2";
                className += " col-md-";
                return className + (TotalSpan.Value - ColSpan);
            }
            var container = CaspianForm as ICaspianContainer ?? CaspianContainer as ICaspianContainer ?? EntitySearch as ICaspianContainer;
            return container.GetLabelContainerCSSClassName(ColSpan.Value);
        }

        protected string GetControlCSSClassName()
        {
            var str = $"col-md-{ColSpan} ";
            return str + (PageData?.RightToLeft == true ? "ps-2" : "pe-2");
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

        public bool HasError()
        {
            return ErrorMessage != null;
        }

        public virtual async Task SetValue(object obj, bool isMultiselect = false)
        {
            var readOnly = false;
            if (InputAttributes.ContainsKey("readonly"))
                readOnly = Convert.ToBoolean(InputAttributes["readonly"]);
            if (!disabled && !readOnly)
            {
                var type = DynamicType ?? typeof(TValue);
                if (type.IsNullableType() && obj != null)
                    type = Nullable.GetUnderlyingType(type);
                Value = default(TValue);
                valueIsChanged = true;
                if (obj != null)
                {
                    if (type.IsEnum && !isMultiselect)
                    {
                        object value = default(TValue);
                        Enum.TryParse(type, obj.ToString(), out value);
                        Value = (TValue)value;
                    }
                    else
                        Value = (TValue)Convert.ChangeType(obj, type);
                }
                if (CurrentEditContext != null && !reseting)
                {
                    var model = CurrentEditContext.Model;
                    FormAppState.AllControlsIsValid = true;
                    FormAppState.ErrorMessage = null;
                    if (_FieldName.HasValue())
                    {
                        var info = model.GetType().GetProperty(_FieldName);
                        if (info == null && !Validate())
                        {
                            FormAppState.AllControlsIsValid = false;
                            FormAppState.Control = this;
                        }
                        else if (info != null)
                        {
                            var field = new FieldIdentifier(CurrentEditContext.Model, _FieldName);
                            info.SetValue(model, Value);
                            CurrentEditContext.NotifyFieldChanged(field);
                        }
                    }
                }
                reseting = false;
                await ValueChanged.InvokeAsync(Value);
                if (OnChange.HasDelegate)
                    await OnChange.InvokeAsync();
                EntitySearch?.EnableLoadData();
            }
        }

        [Parameter]
        public EventCallback OnChange { get; set; }

        [Parameter]
        public string Title { get; set; }

        protected override void OnParametersSet()
        {
            if (Title != null)
                title = Title;
            CaspianContainer?.SetControl(this);
            if (CaspianContainer == null)
                disabled = Disabled;
            else
                disabled = CaspianContainer?.Disabled == true;
            CaspianForm?.AddControl(this);
            if (InputAttributes == null)
                InputAttributes = new Dictionary<string, object>();
            if (TabIndex.HasValue)
                InputAttributes["tabindex"] = TabIndex;
            if (CurrentEditContext != null && CurrentEditContext != oldContext && ValueExpression != null)
            {
                var expr = ValueExpression.Body;
                string str = "";
                while(expr.NodeType == ExpressionType.MemberAccess)
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
                CurrentEditContext.OnValidationStateChanged -= CurrentEditContext_OnValidationStateChanged;
                CurrentEditContext.OnValidationStateChanged += CurrentEditContext_OnValidationStateChanged;
            }
            base.OnParametersSet();
        }

        public async Task FocusAsync()
        {
            if (InputElement.HasValue)
                await InputElement.Value.FocusAsync();
        }

        public virtual async Task ResetAsync()
        {
            reseting = true;
            await SetValue(default(TValue));
            ErrorMessage = null;
        }

        private void CurrentEditContext_OnValidationStateChanged(object sender, ValidationStateChangedEventArgs e)
        {
            if (_FieldName != null)
            {
                if (CurrentEditContext.Properties["ValidationType"].ToString() == "FieldChanged")
                {
                    object obj;
                    if (CurrentEditContext.Properties.TryGetValue("PropertyName", out obj))
                    {
                        var propertyName = obj.ToString();
                        if (propertyName != null && propertyName == _FieldName)
                        {
                            var identifire = CurrentEditContext.Field(propertyName);
                            var errorMessage = CurrentEditContext.GetValidationMessages(identifire).FirstOrDefault();
                            if (ErrorMessage != errorMessage)
                            {
                                ErrorMessage = errorMessage;
                                StateHasChanged();
                            }
                        }
                    }
                }
                else
                {
                    var identifyer = CurrentEditContext.Field(_FieldName);
                    ErrorMessage = CurrentEditContext.GetValidationMessages(identifyer).FirstOrDefault();
                }
            }


            //var field = CurrentEditContext.GetType().GetField("_fieldStates", BindingFlags.NonPublic | BindingFlags.Instance);
            //var states = field.GetValue(CurrentEditContext) as System.Collections.IDictionary;
            //foreach (dynamic state in states)
            //{
            //    var fieldName = state.Key.FieldName as string;
            //    if (fieldName == _FieldName)
            //    {
            //        var fieldIdentifier = new FieldIdentifier(CurrentEditContext.Model, fieldName);
            //        var list = CurrentEditContext.GetValidationMessages();
            //        var result = CurrentEditContext.GetValidationMessages(fieldIdentifier);
            //        ErrorMessage = result.FirstOrDefault();
            //    }
            //    else if (fieldName.EndsWith("]." + _FieldName))
            //    {
            //        var mainField = fieldName.Substring(0, fieldName.Length - _FieldName.Length);
            //        mainField = mainField.Split('[')[0];
            //        var model = CurrentEditContext.Model;
            //        var details = model.GetType().GetProperty(mainField).GetValue(model) as System.Collections.IEnumerable;
            //        var expr = ValueExpression.Body;
            //        FieldInfo info = null;
            //        while (expr.NodeType != ExpressionType.Constant)
            //        {
            //            switch (expr.NodeType)
            //            {
            //                case ExpressionType.MemberAccess:
            //                    var member = (expr as MemberExpression).Member;
            //                    if (member.MemberType == MemberTypes.Field)
            //                        info = member as FieldInfo;
            //                    expr = (expr as MemberExpression).Expression;
            //                    break;
            //                case ExpressionType.Call:
            //                    expr = (expr as MethodCallExpression).Arguments[0];
            //                    break;
            //                default:
            //                    throw new NotImplementedException("خطای عدم پیاده سازی");
            //            }
            //        }
            //        var value = info.GetValue((expr as ConstantExpression).Value);
            //        if (info.FieldType == typeof(RowData<>).MakeGenericType(details.ToDynamicList()[0].GetType()))
            //            value = value.GetType().GetProperty("Data").GetValue(value);
            //        else
            //        {
            //            var str = $"{mainField}[{value}].{_FieldName}";
            //            if (fieldName == str)
            //            {
            //                var fieldIdentifier = new FieldIdentifier(CurrentEditContext.Model, str);
            //                var result = CurrentEditContext.GetValidationMessages(fieldIdentifier);
            //                ErrorMessage = result.FirstOrDefault();

            //            }
            //        }
            //    }
            //}
            if ((ErrorMessage != null || !Validate()) && FormAppState.AllControlsIsValid)
            {
                if (InputElement != null && !Disabled)
                {
                    FormAppState.AllControlsIsValid = false;
                    FormAppState.Control = this;
                }
                else
                    FormAppState.ErrorMessage = ErrorMessage;
            }
        }

        public virtual async void Dispose()
        {
            InputElement = null;
            if (CurrentEditContext != null) 
                CurrentEditContext.OnValidationStateChanged -= CurrentEditContext_OnValidationStateChanged;
            if (OnDispose.HasDelegate)
                await OnDispose.InvokeAsync(this);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
                CaspianForm?.SetFirstControl(this);
            base.OnAfterRender(firstRender);
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
                        switch (expr.NodeType)
                        {
                            case ExpressionType.MemberAccess:
                                var member = (expr as MemberExpression).Member;
                                if (member.MemberType == MemberTypes.Field)
                                    info = member as FieldInfo;
                                expr = (expr as MemberExpression).Expression;
                                break;
                            case ExpressionType.Call:
                                expr = (expr as MethodCallExpression).Arguments[0];
                                break;
                            default:
                                throw new NotImplementedException("عدم پیاده سازی");
                        }
                    }
                    var value = info.GetValue((expr as ConstantExpression).Value);
                    if (info.FieldType == typeof(RowData<>).MakeGenericType(details.ToDynamicList()[0].GetType()))
                        value = value.GetType().GetProperty("Data").GetValue(value);
                    else
                    {
                        var str = fieldName.Replace("[0]", '[' + value.ToString() + ']');
                        var fieldIdentifier = new FieldIdentifier(CurrentEditContext.Model, str);
                        var result = CurrentEditContext.GetValidationMessages(fieldIdentifier);
                        ErrorMessage = result.FirstOrDefault();
                    }
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

        [CascadingParameter]
        public CaspianContainer CaspianContainer { get; set; }

    }
}
