using Caspian.UI;
using Caspian.Common.Attributes;
using Employment.Model;
using Employment.Service;
using Microsoft.AspNetCore.Components;
using Caspian.Common;
using Microsoft.AspNetCore.Components.Rendering;

namespace Caspian.Engine.CodeGenerator
{
	public partial class EmploymentOrderPage : BasePage
	{
		EmploymentOrder EmploymentOrder;
		int? BaseNumber;
		Grade? Grade;
		int? EducationDegreeFactor;
		ComboBox<EducationDegree, Int32> cmbEducationDegree;
		NumericTextBox<int?> txtBaseNumber;
		DropdownList<Grade> ddlGrade;
		StringTextBox txtDescript;
		NumericTextBox<int?> txtEducationDegreeFactor;
		ComboBox<EmploymentOrderType, Int32> cmbEmploymentOrderType;
		protected override void OnInitialized()
		{
			EmploymentOrder = new EmploymentOrder();
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
			builder.AddAttribute(3, "Value", EmploymentOrder.EducationDegreeId);
			builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<Int32>(this, value => { EmploymentOrder.EducationDegreeId = value; }));
			builder.AddAttribute(3, "OnValueChanged", EventCallback.Factory.Create(this, async () => await cmbEducationDegreeOnChange()));
			builder.AddComponentReferenceCapture(1, cmb =>
			{
				cmbEducationDegree = cmb as ComboBox<EducationDegree, Int32>;
				cmbEducationDegree.TextExpression = t => t.Title;
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
			builder.AddContent(1, "عدد مبنا");
			builder.CloseElement();
			builder.OpenComponent<NumericTextBox<int?>>(2);
			builder.AddAttribute(3, "Value", BaseNumber);
			builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<int?>(this, value => {
				BaseNumber = value;
			}));
			builder.AddComponentReferenceCapture(1, txt =>
			{
				txtBaseNumber = txt as NumericTextBox<int?>;
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
			builder.AddContent(1, "رتبه");
			builder.CloseElement();
			builder.OpenComponent<DropdownList<Grade>>(2);
			builder.AddAttribute(3, "Value", Grade);
			builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<Grade>(this, async value => {
				Grade = value;
				await ddlGradeOnChange();
			}));
			builder.AddComponentReferenceCapture(1, ddl =>
			{
				ddlGrade = ddl as DropdownList<Grade>;
			});
			builder.CloseComponent();
			builder.CloseElement();
			builder.CloseElement();
			builder.CloseElement();
			builder.CloseElement();
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "col-md-6");
			builder.OpenElement(1, "fieldset");
			builder.AddAttribute(3, "class", "c-dynamic-form-controls");
			builder.AddAttribute(3, "style", "height:221px"); 
			builder.OpenElement(1, "legend");
			builder.AddContent(1, "شرح حکم");
			builder.CloseElement();
			builder.OpenComponent<StringTextBox>(2);
			builder.AddAttribute(3, "style", "height:190px"); 
			builder.AddAttribute(3, "MultiLine", true); 
			builder.AddAttribute(3, "Value", EmploymentOrder.Descript);
			builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<string>(this, value => { EmploymentOrder.Descript = value; }));
			builder.CloseComponent();
			builder.CloseElement();
			builder.CloseElement();
			builder.CloseElement();
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "row");
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "col-md-6");
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
			builder.OpenElement(1, "div");
			builder.AddAttribute(3, "class", "col-md-6");
			builder.OpenElement(1, "fieldset");
			builder.AddAttribute(3, "class", "c-dynamic-form-controls");
			builder.OpenElement(1, "legend");
			builder.AddContent(1, "نوع جکم");
			builder.CloseElement();
			builder.OpenComponent<ComboBox<EmploymentOrderType, Int32>>(2);
			builder.AddAttribute(3, "Value", EmploymentOrder.EmploymentOrderTypeId);
			builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<Int32>(this, value => { EmploymentOrder.EmploymentOrderTypeId = value; }));
			builder.AddAttribute(3, "OnValueChanged", EventCallback.Factory.Create(this, async () => await cmbEmploymentOrderTypeOnChange()));
			builder.AddComponentReferenceCapture(1, cmb =>
			{
				cmbEmploymentOrderType = cmb as ComboBox<EmploymentOrderType, Int32>;
				cmbEmploymentOrderType.TextExpression = t => t.Title;
			});
			builder.CloseComponent();
			builder.CloseElement();
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

	public partial class EmploymentOrderPage
	{
		int?[,] baseNumbers = new int?[5, 4]
			{
						  {3700, 4200, 4700, 5100},
						  {3800, 4300, 4800, 5200},
						  {3900, 4400, 4900, 5300},
						  {null, 4500, 5000, 5400},
						  {null, null, 5100, 5500}
			};
		public void Initialize()
		{
			cmbEducationDegree.TextExpression = t => t.Title;
			cmbEmploymentOrderType.TextExpression = t => t.Title;

		}

		async Task<BaseStudy> GetBaseStudy()
		{
			var scope = CreateScope();
			return (await new EducationDegreeService(scope).SingleAsync(EmploymentOrder.EducationDegreeId)).BaseStudy;
		}

		public async Task cmbEducationDegreeOnChange()
		{
			if (EmploymentOrder.EducationDegreeId > 0)
			{
				var index = (await GetBaseStudy()).ConvertToInt()!.Value - 3;
				var array = new int?[] { 50, 60, 70, 80 };
				EducationDegreeFactor = index < 0 ? null : array[index];
				if (EducationDegreeFactor == null)
					txtEducationDegreeFactor.Disable();
				else
					txtEducationDegreeFactor.Enable();
				if (Grade.HasValue && index >= 0)
				{
					var gradeIndex = Grade.Value.ConvertToInt().Value - 1;
					BaseNumber = baseNumbers[gradeIndex, index];
					if (BaseNumber.HasValue)
						txtBaseNumber.Enable();
					else
						txtBaseNumber.Disable();
				}
				else
					txtBaseNumber.Disable();
			}
			else
			{
				txtEducationDegreeFactor.Disable();
				txtBaseNumber.Disable();
			}
		}

		public async Task ddlGradeOnChange()
		{
			if (EmploymentOrder.EducationDegreeId > 0 && Grade.HasValue)
			{
				var index = (await GetBaseStudy()).ConvertToInt()!.Value - 3;
				var gradeIndex = Grade.Value.ConvertToInt().Value - 1;
				BaseNumber = baseNumbers[gradeIndex, index];
				if (BaseNumber.HasValue)
					txtBaseNumber.Enable();
				else
					txtBaseNumber.Disable();
			}
			else
				txtBaseNumber.Disable();
		}

		public async Task cmbEmploymentOrderTypeOnChange()
		{
			if (!EmploymentOrder.Descript.HasValue() || await Confirm("آیا با جایگزینی شرح حکم این نوع حکم موافقید؟"))
			{
				using var scope = CreateScope();
				var old = await new EmploymentOrderTypeService(scope).SingleAsync(EmploymentOrder.EmploymentOrderTypeId);
				EmploymentOrder.Descript = old.Description;
			}
		}
	}
	public enum Grade
	{
		[EnumField("مقدماتی")]
		Primary = 1,

		[EnumField("مهارتی")]
		Skills = 2,

		[EnumField("پایه 3")]
		Grade3 = 3,

		[EnumField("پایه 2")]
		Grade2 = 4,

		[EnumField("پایه 1")]
		Grade1 = 5
	}
}