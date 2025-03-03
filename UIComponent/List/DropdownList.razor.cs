using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Caspian.UI
{
    public partial class DropdownList<TValue>: CBaseInput<TValue>
    {
        string text;
        Dictionary<string, object> attrs;
        Dictionary<string, object> inputAttrs;
        WindowStatus status;
        List<SelectListItem> items;
        int? selectedIndex = null;
        IList<object> values;
        string propertyPath;

        protected override void OnInitialized()
        {
            values = new List<object>();
            base.OnInitialized();
        }

        void UpdateValues(bool flag, object value)
        {
            if (flag)
                values.Add(value);
            else
                values.Remove(value);
        }

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
                            if (selectedIndex > items.Count)
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
                                selectedIndex = items.Count;
                        }
                        break;
                    case "Enter":
                    case "NumpadEnter":
                        if (selectedIndex.GetValueOrDefault() > 0 && EntitySearch == null)
                        {
                            status = WindowStatus.Close;
                            await SetValueForActiveItem(items[selectedIndex.Value - 1]);
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
                foreach (var item in items.Select(t => t.Value))
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
                await base.SetValue(item.Value);
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
            else if (str.HasValue())
                value = (TValue)Convert.ChangeType(str, typeof(TValue).GetUnderlyingType());
            Value = value;
            await ValueChanged.InvokeAsync(Value);
            if (OnChange.HasDelegate)
                await OnChange.InvokeAsync();
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
                string str = "Please Select ...";
                items = new ();
                if (typeof(TValue).IsNullableType())
                    items.Add(new SelectListItem(null, str));
                else if (search)
                    items.Add(new SelectListItem("0", str));
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
                    items.Add(new SelectListItem(intValue.ToString(), attr == null ? field.Name : attr.Name, disable));
                }
            }
            else
            {
                if (FilterFunc == null)
                    items = Source.ToList();
                else
                {
                    items = new List<SelectListItem>();
                    foreach (var item in Source)
                    {
                        var value = (TValue)Convert.ChangeType(item.Value, typeof(TValue));
                        if (FilterFunc.Invoke(value))
                            items.Add(item);
                    }
                }
            }
            //text = service.Language == Language.Fa ? "لطفا انتخاب نمائید" : "Please select ...";
            text = "Please Select ...";
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
            if (EntitySearch != null)
            {
                var path = GetProppertyPath();
                if (path != null)
                {
                    var values = EntitySearch.GetFieldValues(path);
                    if (values != null && values.Count() > 0)
                    {
                        var str = string.Empty;
                        foreach ( var value in values)
                        {
                            if (str != string.Empty)
                                str += ", ";
                            str += items.Single(t => t.Value == value.ToString()).Text;
                        }
                        text = str;
                    }
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
                await jsRuntime.InvokeVoidAsync("caspian.common.bindDropdownList", InputElement, dotnet);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        [JSInvokable]
        public async Task CloseWindow()
        {
            status = WindowStatus.Close;
            if (EntitySearch != null)
            {
                text = string.Empty;
                foreach (var value in values)
                {
                    if (text != string.Empty)
                        text += ", ";
                    text += items.Single(t => t.Value == value.ToString()).Text;
                }
            }
            if (text == string.Empty)
                text = "Please Select ...";
            if (EntitySearch != null)
            {
                var path = GetProppertyPath();
                var type = typeof(TValue).GetUnderlyingType();
                await EntitySearch.UpsertEnumValues(path, values.Select(t => Enum.Parse(type, t.ToString())).ToArray());
            }
            StateHasChanged();
        }

        string GetProppertyPath()
        {
            if (propertyPath == null)
            {
                var path = "";
                var expr = ValueExpression.Body;
                while (expr.NodeType == ExpressionType.MemberAccess)
                {

                    var property = (expr as MemberExpression).Member as PropertyInfo;
                    if (expr.Type == EntitySearch.EntityType)
                        break;
                    else if (property != null)
                    {
                        if (path.HasValue())
                            path = $".{path}";
                        path = property.Name + path;
                    }
                    expr = (expr as MemberExpression).Expression;
                }
                propertyPath = path;
            }
            return propertyPath;
        }

    }
}
