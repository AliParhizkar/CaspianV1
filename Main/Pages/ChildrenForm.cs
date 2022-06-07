using Caspian.UI;
using Employment.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Caspian.Engine.CodeGenerator
{
	public partial class Husband : ComponentBase
	{
		ChildrenProperties childrenProperty;
		protected override void OnInitialized()
		{
			childrenProperty = new ChildrenProperties();
			base.OnInitialized();
		}

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent<ComboBox<City, int>>(1);
			builder.AddComponentReferenceCapture(1, cmb =>
			{
				(cmb as ComboBox<City, int>)!.TextExpression = t => t.Title;
			});
			builder.CloseComponent();
		}
	}
}