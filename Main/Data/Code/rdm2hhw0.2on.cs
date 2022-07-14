using Employment.Model;
using Employment.Service;
using System.Threading.Tasks;
using Capian.Dynamicform.Component;

namespace Caspian.Dynamic.WorkflowForm
{
	public partial class EmploymentOrderPage
	{
	        int?[, ] baseNumbers = new int?[5, 4] 
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
                            var array = new int?[] {50, 60, 70, 80};
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
}