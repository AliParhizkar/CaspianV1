using System.Reflection;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Caspian.Common.Client;

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
                className += " t-stringtextbox";
            if (Style.HasValue())
                attributes["style"] = Style;
            if (disabled)
                className += " t-state-disabled";
            attributes["class"] = className;
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
            return InputAttributes;
        }

        protected override void OnParametersSet()
        {
            disabled = Disabled;
            base.OnParametersSet();
        }

        async Task ChangeValue(ChangeEventArgs arg)
        {
            var readOnly = false;
            if (InputAttributes.ContainsKey("readonly"))
                readOnly = Convert.ToBoolean(InputAttributes["readonly"]);
            if (!readOnly && !disabled)
            {
                Value = arg.Value.ToString();
                if (ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(Value);
                if (OnChange.HasDelegate)
                    await OnChange.InvokeAsync();
            }
        }

        protected override void OnInitialized()
        {
            if (Id == null)
                Id = "";
            base.OnInitialized();
        }

        [Parameter]
        public int? MaxLength { get; set; }

        [Parameter]
        public string MaskedText { get; set; }

        [Parameter]
        public bool MultiLine { get; set; }

        [Parameter]
        public int? Cols { get; set; }

        [Parameter]
        public int? Rows { get; set; }

        public string Type { get; set; } = "string";

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await jsRuntime.InvokeVoidAsync("caspian.common.bindStringbox", InputElement);
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
