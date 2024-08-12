using Caspian.Common;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class RadioItem<TValue> : ComponentBase
    {
        async void OnMouseClick()
        {
            if (!Disabled)
            {
                await ValueChanged.InvokeAsync(Value);
                await RadioLis.ChangeValueAsync(Value);
            }
        }

        [CascadingParameter(Name = "RadioLis")]
        public IRadioList RadioLis { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public bool HideTitle { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        protected override void OnParametersSet()
        {
            if (!Title.HasValue())
            {
                if (typeof(TValue).GetUnderlyingType().IsEnum && Value != null)
                    Title = (Value as Enum).EnumText();
            }
            base.OnParametersSet();
        }
    }
}
