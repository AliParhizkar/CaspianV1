using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;

namespace Caspian.UI
{
    public partial class DropdownList<TValue>: CBaseInput<TValue>
    {
        string text;
        Dictionary<string, object> attrs;
        Dictionary<string, object> inputAttrs;
        WindowStatus status;
        List<SelectListItem> Items;
        int? selectedIndex = null;

        bool IsEqual(string value1, TValue value2)
        {
            if (value1 == null)
                return value2 == null;
            if (value2 == null)
                return value1 == null;
            var temp1 = Convert.ToInt32(value1);
            var temp2 = Convert.ChangeType(value2, typeof(TValue).GetUnderlyingType());
            return temp1.Equals(Convert.ToInt32(temp2));
        }

        async Task OnkeyUp(KeyboardEventArgs args)
        {
            if (!disabled)
            {
                switch(args.Code)
                {
                    case "ArrowDown":
                        if (status == WindowStatus.Open)
                        {
                            selectedIndex++;
                            if (selectedIndex > Items.Count)
                                selectedIndex = 1;
                        }
                        else
                            OpenWindow();
                        break;
                    case "ArrowUp":
                        if (status == WindowStatus.Open)
                        {
                            selectedIndex--;
                            if (selectedIndex == 0)
                                selectedIndex = Items.Count;
                        }
                        break;
                    case "Enter":
                    case "NumpadEnter":
                        if (selectedIndex.GetValueOrDefault() > 0)
                        {
                            status = WindowStatus.Close;
                            await SetValueForActiveItem(Items[selectedIndex.Value - 1]);
                        }
                        break;
                }
            }
        }

        void OpenWindow()
        {
            if (status != WindowStatus.Open && !disabled)
            {
                int index = 1;
                foreach (var item in Items.Select(t => t.Value))
                {
                    if (IsEqual(item, Value))
                    {
                        selectedIndex = index;
                        break;
                    }
                    index++;
                }
                selectedIndex = selectedIndex ?? 1;
                status = WindowStatus.Open;
            }
        }

        async Task SetValueForActiveItem(SelectListItem item)
        {
            if (!item.Disabled && !disabled)
            {
                await SetValue(item.Value);
                await Task.Delay(300);
                status = WindowStatus.Close;
            }
        }

        async Task UpdateValue(ChangeEventArgs arg)
        {
            TValue value = default;
            var str = Convert.ToString(arg.Value);
            if (Source == null)
            {
                if (this.DynamicType != null)
                    value = (TValue)Enum.Parse(DynamicType.GetUnderlyingType(), str);
                else if (str.HasValue())
                    value = (TValue)Enum.Parse(typeof(TValue).GetUnderlyingType(), str);
            }
            else
                if (str.HasValue())
                value = (TValue)Convert.ChangeType(str, typeof(TValue).GetUnderlyingType());
            Value = value;
            await ValueChanged.InvokeAsync(Value);
            if (OnChange.HasDelegate)
                await OnChange.InvokeAsync();
        }

        protected override void OnInitialized()
        {
            if (ValueExpression != null)
            {
                var expr = ValueExpression.Body;
                while (expr.NodeType == ExpressionType.MemberAccess)
                {
                    var property = (expr as MemberExpression).Member;
                    if (property.Name == "SearchData")
                    {
                        var type = property.DeclaringType;
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(SimplePage<>))
                            Search = true;
                    }
                    expr = (expr as MemberExpression).Expression;
                }
            }
            base.OnInitialized();
        }

        [Parameter]
        public IList<SelectListItem> Source { get; set; }

        [Parameter]
        public Func<TValue, bool> FilterFunc { get; set; }

        [Parameter]
        public Func<TValue, bool> DisableFunc { get; set; }

        protected override void OnParametersSet()
        {
            if (Source == null)
            {
                //string str = service.Language == Language.Fa ? "لطفا انتخاب نمائید" : "Please select ...";
                string str = "Please select ...";
                Items = new List<SelectListItem>();
                if (typeof(TValue).IsNullableType())
                    Items.Add(new SelectListItem(null, str));
                else if (Search)
                    Items.Add(new SelectListItem("0", str));
                var fields = GetFields();
                foreach (var field in fields.Where(t => !t.IsSpecialName))
                {
                    var attr = field.GetCustomAttribute<DisplayAttribute>();
                    var value = field.GetValue(null);
                    var disable = false;
                    if (DisableFunc != null)
                    {
                        var enumValue = (TValue)Convert.ChangeType(value, typeof(TValue));
                        disable = DisableFunc.Invoke(enumValue);
                    }
                    var intValue = Convert.ToInt32(value);
                    Items.Add(new SelectListItem(intValue.ToString(), attr == null ? field.Name : attr.Name, disable));
                }
            }
            else
                Items = Source.ToList();
            //text = service.Language == Language.Fa ? "لطفا انتخاب نمائید" : "Please select ...";
            text = "Please select ...";
            if (Value != null && !Value.Equals(default(TValue)))
            {
                if (Source == null)
                {
                    var field = GetFields().SingleOrDefault(t => t.GetValue(null).Equals(Value));
                    if (field != null)
                    {
                        var attr = field.GetCustomAttribute<DisplayAttribute>();
                        text = (attr != null) ? attr.Name : field.Name;
                    }
                }
                else
                {
                    string strValue = null;
                    if (typeof(TValue).GetUnderlyingType().IsEnum)
                        strValue = Convert.ToString(Convert.ToInt32(Value));
                    else
                        strValue = Convert.ToString(Convert.ToInt32(Value));
                    text = Source.SingleOrDefault(t => t.Value == strValue)?.Text;
                }
            }
            attrs = new Dictionary<string, object>();
            if (Style.HasValue())
                attrs["style"] = Style;

            inputAttrs = new Dictionary<string, object>();

            if (Id.HasValue())
            {
                inputAttrs["id"] = Id.Replace('.', '_');
                inputAttrs["name"] = Id;
            }
            if (Value == null)
                inputAttrs["value"] = Value;
            else
            {
                if (typeof(TValue) == typeof(string))
                    inputAttrs["value"] = Value;
                else
                    inputAttrs["value"] = Convert.ToInt32(Value).ToString();
            }
            base.OnParametersSet();
            if (!disabled)
                attrs["tabindex"] = TabIndex ?? 0;
        }

        IEnumerable<FieldInfo> GetFields()
        {
            var type = DynamicType ?? typeof(TValue);
            if (type.IsNullableType())
                type = Nullable.GetUnderlyingType(type);
            if (!type.IsEnum)
                throw new Exception("خطا: Type " + type.Name + " is not enum : " + ValueExpression.Body.ToString());
            var fields = type.GetFields().Where(t => !t.IsSpecialName);
            if (FilterFunc != null)
                return fields.Where(t => FilterFunc.Invoke((TValue)t.GetValue(null)));
            return fields;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotnet = DotNetObjectReference.Create(this);
                await jsRuntime.InvokeVoidAsync("$.caspian.bindDropdownList", dotnet, InputElement);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable]
        public void CloseWindow()
        {
            status = WindowStatus.Close;
            StateHasChanged();
        }

    }
}
