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
        protected string _FieldName;
        ValidationMessageStore _messageStore;
        bool reseting;
        EditContext oldContext;
        protected bool disabled, focused, search;
        protected string id;

        string GetId()
        {
            var id = Id;
            if (!Id.HasValue() && ValueExpression != null)
            {
                id = (ValueExpression.Body as MemberExpression).Member.Name;
                if (EntitySearch != null)
                    id = $"Search.{id}";
                if (CaspianForm != null)
                    id = $"Form.{id}";
            }
            return id;
        }

        protected string GetTitle()
        {
            if (Title != null)
            {
                return Title;
            }
            if (ValueExpression != null)
            {
                var member = (ValueExpression.Body as MemberExpression).Member;
                return member.DeclaringType.GetTitle(member.Name) ?? member.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? 
                    member.Name;
            }
            throw new NotImplementedException();
        }

        protected IDictionary<string, object> LabelAttributes
        {
            get
            {
                if (PageData?.OnClick != null && Title.HasValue())
                {
                    var attrs = new Dictionary<string, object>();
                    attrs["onclick"] = new Action(() =>
                    {
                        PageData.OnClick(id, Title);
                    });
                    attrs["class"] = "c-supervisor";
                    return attrs;
                }
                return null;
            }
        }

        protected override void OnInitialized()
        {
            id = GetId();
            base.OnInitialized();
        }

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

        [CascadingParameter]
        internal PageData PageData { get; set; }

        protected string GetControlCSSClassName()
        {
            var str = $"col-md-{ColSpan} ";
            return str + (PageData?.RightToLeft == true ? "ps-2" : "pe-2");
        }

        protected string GetLabelCSSClassName()
        {
            if (TotalSpan.HasValue)
            {
                var className = disabled ? "c-disabled " : "";
                className += PageData?.RightToLeft == true ? "pe-2" : "ps-2";
                className += " col-md-";
                return className + (TotalSpan.Value - ColSpan);
            }
            var container = EntitySearch as ICaspianContainer ?? CaspianForm as ICaspianContainer ?? CaspianContainer;
            return (disabled ? "c-disabled " : "") + container.GetLabelContainerCSSClassName(ColSpan.Value);
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
                if (ValueChanged.HasDelegate)
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
            CaspianContainer?.SetControl(this);
            if (CaspianContainer == null)
                disabled = Disabled;
            else
                disabled = CaspianContainer?.Disabled == true;
            CaspianForm?.AddControl(this);

            if (InputAttributes == null)
                InputAttributes = new Dictionary<string, object>();
            if (!ValueChanged.HasDelegate)
                InputAttributes["readonly"] = true;
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
                    {
                        if (str.Length == 0)
                            str = memberExpr.Member.Name;
                        break;
                    }
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
            if (_FieldName != null && EntitySearch == null)
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
            if (OnDispose.HasDelegate)
                await OnDispose.InvokeAsync(this);
            CaspianForm?.ClearFirstControl(this);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender && !disabled )
                CaspianForm?.SetFirstControl(this);
            base.OnAfterRender(firstRender);
        }

        [CascadingParameter]
        public CaspianContainer CaspianContainer { get; set; }

    }
}
