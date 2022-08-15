using Caspian.UI;
using System;
using Caspian.Common.Attributes;
using Employment.Model;
using Employment.Service;
using Microsoft.AspNetCore.Components;
using Caspian.Common;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Rendering;

namespace Caspian.Engine.CodeGenerator
{
	public partial class EmploymentOrderPage : BasePage
	{
		EmploymentOrder employmentOrder;
		string ForTest;
		DateTime? forTest1;

		//Dynamic parameters
		int? EducationDegreeFactor;
		ComboBox<EducationDegree, Int32> cmbEducationDegree;
		NumericTextBox<int?> txtEducationDegreeFactor;
		StringTextBox txtForTest;

		protected override void OnInitialized()
		{
			employmentOrder = new EmploymentOrder();
			base.OnInitialized();
		}

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent<MessageBox>(2);
			builder.AddComponentReferenceCapture(1, msg => { MessageBox = msg as MessageBox; });
			builder.CloseComponent(); builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "row");
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "col-md-6");
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "row");
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "col-md-12");
			builder.OpenElement(1, "fieldset");
			builder.AddAttribute(3, "class", "c-dynamic-form-controls");
			builder.OpenElement(1, "legend");
			builder.AddContent(1, "مدرک تحصیلی");
			builder.CloseElement();
			builder.OpenComponent<ComboBox<EducationDegree, Int32>>(2);
			builder.AddAttribute(3, "Value", employmentOrder.EducationDegreeId);
			builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<Int32>(this, value => { employmentOrder.EducationDegreeId = value; }));
			builder.AddComponentReferenceCapture(1, cmb =>
			{
				cmbEducationDegree = cmb as ComboBox<EducationDegree, Int32>;
			});
			builder.CloseComponent();
			builder.CloseElement();
			builder.CloseElement();
			builder.CloseElement();
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "row");
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "col-md-12");
			builder.OpenElement(1, "fieldset");
			builder.AddAttribute(3, "class", "c-dynamic-form-controls");
			builder.OpenElement(1, "legend");
			builder.AddContent(1, "ضریب مدرک تحصیلی");
			builder.CloseElement();
			builder.OpenComponent<NumericTextBox<int?>>(2);
			builder.AddAttribute(3, "Value", EducationDegreeFactor);
			builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<int?>(this, value => {
				EducationDegreeFactor = value;
			}));
			builder.AddComponentReferenceCapture(1, txt =>
			{
				txtEducationDegreeFactor = txt as NumericTextBox<int?>;
			});
			builder.CloseComponent();
			builder.CloseElement();
			builder.CloseElement();
			builder.CloseElement();
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "row");
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "col-md-12");
			builder.OpenElement(1, "fieldset");
			builder.AddAttribute(3, "class", "c-dynamic-form-controls");
			builder.OpenElement(1, "legend");
			builder.AddContent(1, "برای تست");
			builder.CloseElement();
			builder.OpenComponent<StringTextBox>(2);
			builder.AddAttribute(3, "Value", ForTest);
			builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<string>(this, value => {
				ForTest = value;
			}));
			builder.AddComponentReferenceCapture(1, txt =>
			{
				txtForTest = txt as StringTextBox;
			});
			builder.CloseComponent();
			builder.CloseElement();
			builder.CloseElement();
			builder.CloseElement();
			builder.CloseElement();
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "col-md-6");
			builder.CloseElement();
			builder.CloseElement();
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "row");
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "col-md-6");
			builder.CloseElement();
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "col-md-6");
			builder.CloseElement();
			builder.CloseElement();
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "row");
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "col-md-6");
			builder.CloseElement();
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "col-md-6");
			builder.CloseElement();
			builder.CloseElement();
		}
	}
}