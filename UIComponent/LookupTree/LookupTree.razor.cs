using Caspian.Common;
using System.Reflection;
using System.Collections;
using Microsoft.JSInterop;
using System.ComponentModel;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public partial class LookupTree<TValue>: ComponentBase, ILookupTree, IControl
    {
        bool show;
        bool multiSelectable;
        
        string searchText, title, _FieldName;
        IList<string> selectedNodesValue;
        ITreeView treeView;
        bool valueIsUpdated;
        EditContext oldContext;
        ValidationMessageStore _messageStore;

        async Task ShowTree()
        {
            if (show == false && !Disabled)
            {
                show = true;
                if (OnInternalShow.HasDelegate)
                    await OnInternalShow.InvokeAsync();
            }
        }

        public async Task ResetAsync()
        {
            Value = default(TValue);
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(Value);
        }

        public bool HasError()
        {
            return ErrorMessage.HasValue();
        }

        public void Dispose()
        {
            InputElement = null;
        }

        [Parameter]
        public bool Disabled { get; set; }

        public ElementReference? InputElement {  get; private set; }

        public string ErrorMessage { get; set; }

        public async Task FocusAsync()
        {
            if (InputElement.HasValue)
                await InputElement.Value.FocusAsync();
        }

        protected override void OnInitialized()
        {
            var type = typeof(TValue).GetUnderlyingType();
            if (type.IsArray || type.IsCollectionType())
            {
                selectedNodesValue = new List<string>();
                multiSelectable = true;
            }
            if (ValueExpression != null)
            {
                var info = (ValueExpression.Body as MemberExpression).Member;
                title = info.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? info.Name;
            }
            base.OnInitialized();
        }

        [CascadingParameter(Name = "ParentForm")]
        internal ICaspianForm CaspianForm { get; set; }

        [CascadingParameter]
        internal CaspianContainer CaspianContainer { get; set; }

        [CascadingParameter]
        internal IEntitySearch EntitySearch { get; set; }

        [CascadingParameter]
        internal PageData PageData { get; set; }

        string GetControlCSSClassName()
        {
            var str = $"col-md-{ColSpan} ";
            return str + (PageData?.RightToLeft == true ? "ps-2" : "pe-2");
        }

        string GetLabelCSSClassName()
        {
            if (TotalSpan.HasValue)
            {
                var className = PageData?.RightToLeft == true ? "ps-2" : "pe-2";
                className += " col-md-";
                return className + (TotalSpan.Value - ColSpan);
            }
            var container = EntitySearch as ICaspianContainer ?? CaspianForm as ICaspianContainer ?? CaspianContainer;
            return container.GetLabelContainerCSSClassName(ColSpan.Value);
        }

        [CascadingParameter]
        public EditContext CurrentEditContext { get; set; }

        protected override void OnParametersSet()
        {
            CaspianForm?.AddControl(this);
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
                CurrentEditContext.OnValidationStateChanged -= CurrentEditContext_OnValidationStateChanged;
                CurrentEditContext.OnValidationStateChanged += CurrentEditContext_OnValidationStateChanged;
                oldContext = CurrentEditContext;
            }
            base.OnParametersSet();
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

                            var identifier = CurrentEditContext.Field(propertyName);
                            var errorMessage = CurrentEditContext.GetValidationMessages(identifier).FirstOrDefault();
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
                    var identifire = CurrentEditContext.Field(_FieldName);
                    ErrorMessage = CurrentEditContext.GetValidationMessages(identifire).FirstOrDefault();
                }
                if (ErrorMessage != null && FormAppState.AllControlsIsValid)
                {
                    FormAppState.AllControlsIsValid = false;
                    FormAppState.Control = this;
                    FormAppState.ErrorMessage = ErrorMessage;
                }
            }
        }

        [Parameter]
        public int? ColSpan { get; set; }

        [Parameter]
        public int? TotalSpan { get; set; }

        [Parameter]
        public string Title { get; set; }

        async Task Search(ChangeEventArgs e)
        {
            if (treeView != null)
            {
                searchText = valueIsUpdated ? "" : Convert.ToString(e.Value);
                valueIsUpdated = false;
                if (OnInternalChanged.HasDelegate)
                    await OnInternalChanged.InvokeAsync(searchText);
                if (ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(default);
                if (show == false)
                    await ShowTree();
                await treeView.ReloadAsync();
            }
        }

        //protected override void OnAfterRender(bool firstRender)
        //{
        //    if (firstRender)
        //        CaspianForm?.SetFirstControl(this);
        //    base.OnAfterRender(firstRender);
        //}

        [Parameter]
        public EventCallback OnChange { get; set; }

        public EventCallback<string> OnInternalChanged { get; set; }

        public EventCallback OnInternalShow { get; set; }

        [Parameter]
        public RenderFragment<string> ChildContent { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter]
        public Expression<Func<TValue>> ValueExpression { get; set; }

        public bool MultiSelectable()
        {
            return multiSelectable;
        }

        public IList<string> SelectedNodesValue()
        {
            return selectedNodesValue;
        }

        public void SetTreeView(ITreeView treeView)
        {
            this.treeView = treeView;
            treeView.MultiSelectable= multiSelectable;
            if (valueIsUpdated)
            {
                valueIsUpdated = false;
                searchText = null;
                if (multiSelectable && treeView != null)
                    treeView.SetSelectedNodesValue(selectedNodesValue);
                StateHasChanged();
            }
            if (multiSelectable)
                treeView.OnInternalCHanged = EventCallback.Factory.Create<NodeView>(this, SetSelectedNodesValue);
            else
                treeView.OnInternalClicked = EventCallback.Factory.Create<NodeView>(this, SetValueAsync);

        }

        public async Task SetValueAsync(NodeView node)
        {
            var type = typeof(TValue).GetUnderlyingType();
            Value = (TValue)Convert.ChangeType(node.Value, type);
            searchText = node.Text;
            valueIsUpdated = true;
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(Value);
            if (OnChange.HasDelegate)
                await OnChange.InvokeAsync();
            show = false;
            if (CurrentEditContext != null && _FieldName != null)
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

        public void SetSelectedNodesValue(NodeView node)
        {
            if (node.Selected == true)
                selectedNodesValue.Add(node.Value);
            else if (node.Selected == false)
                selectedNodesValue.Remove(node.Value);
        }

        public async Task SetValuesAsync()
        {
            valueIsUpdated = true;
            var type = typeof(TValue);
            if (type.IsArray)
                type = type.GetElementType();
            type = type.GetUnderlyingType();
            var arrayList = new ArrayList(selectedNodesValue.Count);
            foreach (var value in selectedNodesValue)
                arrayList.Add(Convert.ChangeType(value, type));
            var array = arrayList.ToArray(type);
            Value = (TValue)(Convert.ChangeType(array, typeof(TValue)));
            valueIsUpdated = true;
            if (service.Language == Common.Language.Fa)
                searchText = $"{selectedNodesValue.Count} آیتم انتخاب شده است";
            else
                searchText = $"{selectedNodesValue.Count} item are selected";
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(Value);
        }

        [JSInvokable]
        public async Task Close()
        {
            show = false;
            if (multiSelectable)
                await SetValuesAsync();
            else
                StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotnet = DotNetObjectReference.Create(this);
                await jSRuntime.InvokeVoidAsync("caspian.common.bindLookupTree", InputElement, dotnet);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

    }
}
