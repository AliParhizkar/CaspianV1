using Caspian.UI;
using System;
using Caspian.Common.Attributes;
using Employment.Model;
using Employment.Service;
using Employment.Web.Pages;
using Microsoft.AspNetCore.Components;
using Caspian.Common;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Components.Rendering;

namespace Caspian.Engine.CodeGenerator
{
    public partial class Husband : BasePage
    {

        //Fields
        EmploymentOrder employmentOrder;
        int? BaseNumber;
        int? EducationDegreeId;
        Checking Checking;

        //Form controls
        ComboBox<EmploymentOrderType, Int32> cmbEmploymentOrderType;
        DropdownList<Checking> ddlChecking;
        StringTextBox txtDescript;

        protected override void OnInitialized()
        {
            employmentOrder = new EmploymentOrder();
            base.OnInitialized();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<MessageBox>(2);
            builder.AddComponentReferenceCapture(1, msg => MessageBox = msg as MessageBox);
            builder.CloseComponent();
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "row");
            builder.CloseElement();
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "row");
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "col-md-4");
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "c-dynamic-form-controls");
            builder.OpenElement(1, "label");
            builder.AddAttribute(3, "class", "pe-3");
            builder.AddContent(1, "نوع حکم");
            builder.CloseElement();
            builder.OpenComponent<ComboBox<EmploymentOrderType, Int32>>(2);
            builder.AddAttribute(3, "Value", employmentOrder.EmploymentOrderTypeId);
            builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<Int32>(this, value => { employmentOrder.EmploymentOrderTypeId = value; }));
            builder.AddAttribute(3, "OnChange", EventCallback.Factory.Create(this, async () => await EmploymentOrderType_OnChange()));
            builder.AddComponentReferenceCapture(1, cmb =>
            {
                cmbEmploymentOrderType = cmb as ComboBox<EmploymentOrderType, Int32>;
                cmbEmploymentOrderType.TextExpression = t => t.Code + "- " + t.Title;
            });
            builder.CloseComponent();
            builder.CloseElement();
            builder.CloseElement();
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "col-md-4");
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "c-dynamic-form-controls");
            builder.OpenElement(1, "label");
            builder.AddAttribute(3, "class", "pe-3");
            builder.AddContent(1, "بررسی ");
            builder.CloseElement();
            builder.OpenComponent<DropdownList<Checking>>(2);
            builder.AddAttribute(3, "Value", Checking);
            builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<Checking>(this, value => {
                Checking = value;
            }));
            builder.AddComponentReferenceCapture(1, ddl =>
            {
                ddlChecking = ddl as DropdownList<Checking>;
            });
            builder.CloseComponent();
            builder.CloseElement();
            builder.CloseElement();
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "col-md-4");
            builder.CloseElement();
            builder.CloseElement();
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "row");
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "col-md-4");
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "c-dynamic-form-controls");
            builder.AddAttribute(3, "style", "height:221px"); builder.OpenElement(1, "label");
            builder.AddAttribute(3, "class", "pe-3");
            builder.AddContent(1, "شرح");
            builder.CloseElement();
            builder.OpenComponent<StringTextBox>(2);
            builder.AddAttribute(3, "style", "height:153px"); builder.AddAttribute(3, "MultiLine", true); builder.AddAttribute(3, "Value", employmentOrder.Descript);
            builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<string>(this, value => { employmentOrder.Descript = value; }));
            builder.CloseComponent();
            builder.CloseElement();
            builder.CloseElement();
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "col-md-4");
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "row");
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "col-md-12");
            builder.CloseElement();
            builder.CloseElement();
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "row");
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "col-md-12");
            builder.CloseElement();
            builder.CloseElement();
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "row");
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "col-md-12");
            builder.CloseElement();
            builder.CloseElement();
            builder.CloseElement();
            builder.OpenElement(1, "div");
            builder.AddAttribute(3, "class", "col-md-4");
            builder.CloseElement();
            builder.CloseElement();
        }
    }

    public partial class Husband
    {
        public void Initialize()
        {
            cmbEmploymentOrderType.TextExpression = t => t.Code + "- " + t.Title;
        }

        public async Task EmploymentOrderType_OnChange()
        {
            var id = employmentOrder.EmploymentOrderTypeId;
            if (id > 0)
            {
                using var service = CreateScope().GetService<EmploymentOrderTypeService>();
                var old = await service.SingleAsync(id);
                if (!employmentOrder.Descript.HasValue() || await Confirm("آیا با تغییر شرح حکم موافقید؟"))
                    employmentOrder.Descript = old.Description;
            }
        }

        public void OrganUnit_OnChange()
        {

        }

        public void EducationDegree_OnChange()
        {

        }

        public void Major_OnChange()
        {

        }
    }
    public enum Checking
    {
        [Display(Name = "تایید")]
        Ok = 1,

        [Display(Name = "رد")]
        Refuse = 2,

        [Display(Name = "برسی بیشتر")]
        MoreChecking = 3
    }
}