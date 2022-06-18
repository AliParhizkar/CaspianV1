using Employment.Model;
using Employment.Service;
using System.Threading.Tasks;
using Capian.Dynamicform.Component;

namespace Caspian.Dynamic.WorkflowForm
{
	public partial class EmploymentOrderPage
	{
		public void Initialize()
		{
			cmbEmploymentOrderType.TextExpression = t => t.Title;
		}

		public async Task cmbEmploymentOrderTypeOnChange()
		{
                    var scope = CreateScope();
                    var service = new EmploymentOrderTypeService(scope);
                    if (EmploymentOrder.EmploymentOrderTypeId > 0)
                    {
                            var old = await  service.SingleAsync(EmploymentOrder.EmploymentOrderTypeId);
                            if (!EmploymentOrder.Descript.HasValue() || await Confirm("آیا می خواهید شرح حکم با شرح نوع حکم انتخابی جایگزین شود."))
                                    EmploymentOrder.Descript = old.Description;
                    }
		}
        }
}