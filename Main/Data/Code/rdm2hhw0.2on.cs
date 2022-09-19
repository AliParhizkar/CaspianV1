using Employment.Model;
using Employment.Service;
using System.Threading.Tasks;
using Capian.Dynamicform.Component;

namespace Caspian.Dynamic.WorkflowForm
{
	public partial class EmploymentOrderPage
	{
	       BaseStudy? baseStudy = null;
	       int[] baseStudyFactors = { 50, 60, 70, 80 };
		public void Initialize()
		{
			cmbEducationDegree.TextExpression = t => t.Title;
		}

		public async Task cmbEducationDegreeOnChange()
		{
                        var scope = CreateScope();
                        if (employmentOrder.EducationDegreeId > 0)
                        {
                                var old = await new EducationDegreeService(scope).SingleAsync(employmentOrder.EducationDegreeId);
                                baseStudy = old.BaseStudy;
                        }
                        else
                                baseStudy = null;
                        if (baseStudy > BaseStudy.Diploma)
                                EducationDegreeFactor = baseStudyFactors[baseStudy.ConvertToInt().Value - 3];
                        else
                                EducationDegreeFactor = null;
                       if (EducationDegreeFactor == null)        
                            txtEducationDegreeFactor.Disable();
                      else
                            txtEducationDegreeFactor.Enable();
		}
        }
}