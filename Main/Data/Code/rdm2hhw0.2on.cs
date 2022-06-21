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

		public async Task cmbEmploymentOrderTypeOnChange()
		{
		      if (EmploymentOrder.EmploymentOrderTypeId > 0)
		      {
		              if (!EmploymentOrder.Descript.HasValue() || await Confirm("آیا می خواهید شرح حکم  را جایگزین کنید"))
                            {
                                  using var scope = CreateScope();
                                   var service = new EmploymentOrderTypeService(scope);
                                    var old = await service.SingleAsync(EmploymentOrder.EmploymentOrderTypeId);
		                      EmploymentOrder.Descript = old.Description;       
	                      }
		      }
		}

		public async Task cmbEducationDegreeOnChange()
		{
                    using var scope = CreateScope();
                    var service = new EducationDegreeService(scope);
                    var old = await service.SingleAsync(EmploymentOrder.EducationDegreeId);
                    if (old.BaseStudy < BaseStudy.AssociateDegree)
                    {
                            EmploymentOrder.EducationDegreeFactor = null;
                            txtEducationDegreeFactor.Disable();
                    }
                    else
                    {
                            EmploymentOrder.EducationDegreeFactor = old.BaseStudy.ConvertToInt() * 10 + 20;
                            txtEducationDegreeFactor.Enable();
                    }
                    /*switch(old.BaseStudy)
                    {
                            case BaseStudy.Highschool:
                            case BaseStudy.Diploma:
                                EmploymentOrder.EducationDegreeFactor = null;
                                txtEducationDegreeFactor.Disable();
                            break;
                            case BaseStudy.AssociateDegree:
                                    EmploymentOrder.EducationDegreeFactor = 50;
                                     txtEducationDegreeFactor.Enable();
                            break;
                            case BaseStudy.Bachelor:
                                    EmploymentOrder.EducationDegreeFactor = 60;
                                    txtEducationDegreeFactor.Enable();
                            break;
                            case BaseStudy.Master:
                                    EmploymentOrder.EducationDegreeFactor = 70;
                                    txtEducationDegreeFactor.Enable();
                            break;
                            case BaseStudy.PHD:
                                    EmploymentOrder.EducationDegreeFactor = 80;
                                    txtEducationDegreeFactor.Enable();
                            break;
                    }*/
		}
        }
}