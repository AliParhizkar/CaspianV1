using System.Reflection;
using System.Collections;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Caspian.Common.Client;

namespace Caspian.UI.Client
{
    public partial class ComboBox<TValue>: ComponentBase
    {
        bool setToDefault;
        bool valueChanged;
        string text;
        Expression cascadExpression;
        Dictionary<string, object> attrs;
        WindowStatus? Status = WindowStatus.Close;
        WindowStatus? oldStatus = WindowStatus.Close;
        TValue OldValue;
        string _FieldName;
        IList items;
        bool shouldRender = true;
        bool focused;

        IList<Expression> fieldsExpression;
        bool fieldsAdd;

        internal int SelectedIndex { get; set; }

        public ElementReference? InputElement { get; private set; }

        [Parameter]
        public IEnumerable<SelectListItem> Source { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public bool Pageable { get; set; } = true;

        [Parameter]
        public int PageSize { get; set; } = 30;

        [Parameter]
        public EventCallback OnChanged { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

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

        void ToggelDropdownList()
        {
            if (!Disabled)
            {
                if (Status == WindowStatus.Close)
                {
                    shouldRender = false;
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

        public async Task SetValueAndClose(object item)
        {
            if (item != null)
            {
                var temp = item as SelectListItem;
                await SetValue(temp.Value);
                text = temp.Text;
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
                    var temp = items[SelectedIndex] as SelectListItem;
                    await SetValue(temp.Value);
                    text = temp.Text;
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

        public void Focus()
        {
            focused = true;
        }

        protected override void OnInitialized()
        {
            text = "";
            base.OnInitialized();
        }

        public void AddDataField(Expression expression)
        {
            fieldsAdd = true;
            fieldsExpression.Add(expression);
        }

        protected async override Task OnParametersSetAsync()
        {
            attrs = new Dictionary<string, object>();
            if (Disabled)
                attrs.Add("disabled", "disabled");
            if (Source != null)
                items = Source.ToList();
            SelectedIndex = -1;
            if (items == null)
            {
                if (Value == null)
                {
                    text = "";
                }
                else if (!Value.Equals(OldValue))
                {
                    OldValue = Value;
                    text = Source.SingleOrDefault(t => t.Value == Value.ToString())?.Text;
                }
            }
            else
            {
                var index = 0;
                foreach (var item in items)
                {
                    var value = (item as SelectListItem).Value;
                    if (Value != null && Value.ToString() == value)
                    {
                        SelectedIndex = index;
                        break;
                    }
                    index++;
                }
                if (SelectedIndex >= 0 && SelectedIndex < items.Count)
                    text = (items[SelectedIndex] as SelectListItem).Text;
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

        public async Task ResetAsync()
        {
            await SetValue(default(TValue));
        }

        public async Task FocusAsync()
        {
            if (InputElement.HasValue)
                await InputElement.Value.FocusAsync();
        }

        void OnChangeValue(ChangeEventArgs e)
        {
            if (setToDefault)
                setToDefault = false;
            else
                text = Convert.ToString(e.Value);
            SelectedIndex = 0;
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
            if (valueChanged)
            {
                valueChanged = false;
                if (OnChanged.HasDelegate)
                    await OnChanged.InvokeAsync();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task SetValue(object value)
        {
            if (value != null)
            {
                var type = typeof(TValue);
                if (type.IsNullableType())
                    type = Nullable.GetUnderlyingType(type);
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
            if (valueChanged && OnChange.HasDelegate)
                await OnChange.InvokeAsync();
        }

        [JSInvokable]
        public void CloseInvokable()
        {
            Close();
        }
    }
}
