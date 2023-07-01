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
			cmbEmploymentOrderType.TextExpression = t => t.Title + t.Code;
			cmbEducationDegree.TextExpression = t => t.Title;
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

		public async Task EducationDegree_OnChange()
		{
			await Confirm("سلام خوبی");
		}
    }
}