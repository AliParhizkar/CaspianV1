using Employment.Model;
using Employment.Service;
using System.Threading.Tasks;
using Capian.Dynamicform.Component;

namespace Caspian.Dynamic.WorkflowForm
{
	public partial class Husband
	{
		public void Initialize()
		{
            cmbMajor.TextExpression = t => t.Title;
            cmbEducationDegree.TextExpression = t => t.Title;
			cmbEmploymentOrderType.TextExpression = t => t.Title + t.Code;
			cmbMajor.ConditionExpression = t => t.EducationDegreeId == EducationDegreeId;
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
            if (EducationDegreeId == null)
                cmbMajor.Disable();
            else
                cmbMajor.Enable();
            cmbMajor.EnableLoading();
		}
    }
}