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
                    cmbEducationDegree.TextExpression = t => t.Title;
		}

		public async Task cmbEducationDegreeOnChange()
		{
                        using var scope = CreateScope();
                        var service = new EducationDegreeService(scope);
                        var old = await service.SingleAsync(EmploymentOrder.EducationDegreeId);
                        if (old.BaseStudy > BaseStudy.Diploma)
                        {
                                txtEducationDegreeFactor.Enable();
                                var index = old.BaseStudy.ConvertToInt()!.Value - 3;
                                var factors = new int[]{50, 60, 70, 80};
                                EmploymentOrder.EducationDegreeFactor = factors[index];
                        }
                        else
                        {
                                txtEducationDegreeFactor.Disable();
                                EmploymentOrder.EducationDegreeFactor = null;
                        }
		}

		public async Task cmbEmploymentOrderTypeOnChange()
		{
                    using var scope = CreateScope();
                    var service = new EmploymentOrderTypeService(scope);
                    var old = await service.SingleAsync(EmploymentOrder.EmploymentOrderTypeId);
                    if (!EmploymentOrder.Descript.HasValue() || await Confirm("آیا با تغییر شرح حکم با شرح حکم پیش فرض موافقید؟"))
                            EmploymentOrder.Descript = old.Description;
		}
        }
}