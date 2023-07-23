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
			cmbEmploymentOrderType.TextExpression = t => t.Code + " " +  t.Title;
			
		}

		public async Task EmploymentOrderType_OnChange()
		{
            var id = employmentOrder.EmploymentOrderTypeId;
            if (id > 0 && (employmentOrder.Descript == null || await Confirm("Do you change the value of ...")))
            {
                using var service = CreateScope().GetService<EmploymentOrderTypeService>();
                var old = await service.SingleAsync(id);
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

		public void Checking_OnChange()
		{
			
		}
    }
}