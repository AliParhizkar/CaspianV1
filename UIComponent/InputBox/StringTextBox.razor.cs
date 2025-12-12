using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Caspian.UI
{
    public partial class StringTextBox: CBaseInput<string>
    {
        IDictionary<string, object> GetAttributes()
        {
            var attributes = new Dictionary<string, object>();
            var className = "t-widget";
            if (MultiLine)
                className += " t-multitextbox";
            else
                className += " t-textbox";
            
            if (ErrorMessage.HasValue())
                attributes["error-message"] = ErrorMessage;
            
            if (Style.HasValue())
                attributes["style"] = Style;
            if (disabled)
                className += " t-state-disabled";
            else if (ErrorMessage.HasValue())
                className += " t-state-error";
            attributes["class"] = className;
            if (search)
                attributes["search"] = true;
            if (MaskedText.HasValue())
                attributes["masked-text"] = MaskedText;
            return attributes;
        }

        IDictionary<string, object> GetInputAttributes() 
        {
            if (MultiLine)
            {
                if (Cols.HasValue)
                    InputAttributes["cols"] = Cols;
                if (Rows.HasValue)
                    InputAttributes["rows"] = Rows;
            }
            else if (!InputAttributes.ContainsKey("autocomplete"))
                InputAttributes["autocomplete"] = "off";
            if (Style.HasValue())
                InputAttributes["style"] = Style;
            if (MaxLength.HasValue)
                InputAttributes["maxlength"] = MaxLength;
            if (disabled)
                InputAttributes["disabled"] = "disabled";
            else
                InputAttributes.Remove("disabled");
            
            InputAttributes["class"] = "t-input";
            if (Id.HasValue())
            {
                var id = Id.Replace('.', '_');
                InputAttributes["id"] = id;
                InputAttributes["name"] = id;
            }
            if (DisableAutoSelect)
                InputAttributes["disableAutoSelect"] = true;
            if (EntitySearch?.IsLookup == true)
            {
                InputAttributes["onkeydown"] = new Action<KeyboardEventArgs>(async e =>
                {
                    switch (e.Code)
                    {
                        case "ArrowUp":
                            await EntitySearch.SelectPreRow();
                            break;
                        case "ArrowDown":
                            await EntitySearch.SelectNextRow();
                            break;
                        case "Enter":
                        case "NumpadEnter":
                            await EntitySearch.SelectRow();
                            break;
                    }
                });
            }
            return InputAttributes;
        }

        [Parameter]
        public bool DisableAutoSelect { get; set; }

        public async Task SetValueOnClientAsync(string value)
        {
            Value = value;
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(value);
            if (InputElement != null)
                await jsRuntime.InvokeVoidAsync("caspian.common.setValueOnClient", InputElement, value);
        }

        async Task ChangeValue(ChangeEventArgs arg)
        {
            var readOnly = false;
            if (InputAttributes.ContainsKey("readonly"))
                readOnly = Convert.ToBoolean(InputAttributes["readonly"]);
            if (!readOnly)
                await base.SetValue(arg.Value);
        }

        protected override void OnInitialized()
        {
            if (Id == null)
                Id = "";
            if (EntitySearch != null)
            {
                search = true;
                if (ValueExpression != null)
                {
                    var expr = ValueExpression.Body;
                    var path = "";
                    while (expr.NodeType == ExpressionType.MemberAccess)
                    {

                        var property = (expr as MemberExpression).Member as PropertyInfo;
                        if (expr.Type == EntitySearch.EntityType)
                            break;
                        else if (property !=  null)
                        {
                            if (path.HasValue())
                                path = $".{path}";
                            path = property.Name + path;
                        }
                        expr = (expr as MemberExpression).Expression;
                    }
                    EntitySearch.SetSearchKind(path, SearchType);
                }
            }
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            if (search)
                BindingType = BindingType.OnInput;
            base.OnParametersSet();
        }

        public async Task<Selection> GetSelectionAsync() => await jsRuntime.InvokeAsync<Selection>("caspian.common.getSelection", InputElement);

        public async Task SetSelection(int start, int? end = null) =>  await jsRuntime.InvokeVoidAsync("caspian.common.setSelection", InputElement, start, end);

        [Parameter]
        public int? MaxLength { get; set; }

        [Parameter]
        public string MaskedText { get; set; }

        [Parameter]
        public bool MultiLine { get; set; }

        [Parameter]
        public int? Cols { get; set; }

        [Parameter]
        public SearchType SearchType { get; set; }

        [Parameter]
        public int? Rows { get; set; }

        public string Type { get; set; } = "string";

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await jsRuntime.InvokeVoidAsync("caspian.common.bindStringbox", InputElement);
            if (focused)
            {
                focused = false;
                await jsRuntime.InvokeVoidAsync("caspian.common.focus", InputElement);
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
