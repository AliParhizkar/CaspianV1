using System;
using Caspian.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;


namespace Caspian.Engine.CodeGenerator
{
    public class ChildrenForm: ComponentBase
    {
        string _value = "علی پرهیزکار";
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(1, "div");
            builder.OpenComponent<StringTextBox>(2);
            builder.AddAttribute(3, "Value", _value);
            builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<string>(this, value => { _value = value; }));
            builder.CloseComponent();
            builder.CloseElement();
            builder.OpenElement(4, "span");
            builder.AddContent(4, _value);
            builder.CloseElement();
            base.BuildRenderTree(builder);
        }
    }
}
