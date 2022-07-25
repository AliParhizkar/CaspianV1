using Employment.Model;
using Employment.Service;
using System.Threading.Tasks;
using Capian.Dynamicform.Component;

namespace Caspian.Dynamic.WorkflowForm
{
	public partial class EmploymentOrderPage
	{
	       int[] BaseStudyFactors = {50, 60, 70, 80};
	       int?[, ] BaseNumbers = 
	       {
                    {3700, 3800, 3900, null, null},
                    {4200, 4300, 4400, 4500, null},
                    {4700, 4800, 4900, 5000, 5100},
                    {5100, 5200, 5300, 5400, 5500}
	       };
	       BaseStudy? baseStudy = null;
	       
	       
		public void Initialize()
		{
                    cmbEducationDegree.TextExpression = t => t.Title;
                    cmbEmploymentOrderType.TextExpression = t => t.Title;
		}

		public async Task cmbEducationDegreeOnChange()
		{
                            if (EmploymentOrder.EducationDegreeId > 0)
                            {
                                    using var scope = CreateScope();
                                    var old = await new EducationDegreeService(scope).SingleAsync(EmploymentOrder.EducationDegreeId);
                                    baseStudy = old.BaseStudy;
                                    var index = baseStudy.ConvertToInt()!.Value - 2;
                                    if (index > 0)
                                            EducationDegreeFactor = BaseStudyFactors[index - 1];
                                    else
                                            EducationDegreeFactor = null;
                                    if (Grade.HasValue)
                                            txtBaseNumber.Enable();
                                    else
                                            txtBaseNumber.Disable();
                            }
                            else
                            {
                                    EducationDegreeFactor = null;
                                    txtBaseNumber.Disable();
                            }
		}

		public void ddlGradeOnChange()
		{
                    if (Grade.HasValue && baseStudy.HasValue)
                    {
                            txtBaseNumber.Enable();
                            var index1 = baseStudy.ConvertToInt() .Value- 3;
                            var index2 = Grade.ConvertToInt() .Value- 1;
                            BaseNumber = BaseNumbers[index1, index2];
                    }
                    else
                            BaseNumber = null;
                    if (BaseNumber == null)
                            txtBaseNumber.Disable();
		}

		public async Task cmbEmploymentOrderTypeOnChange()
		{
		      if (EmploymentOrder.EmploymentOrderTypeId > 0)
		      {
                            using var scope = CreateScope();
                            var old = await new EmploymentOrderTypeService(scope).SingleAsync(EmploymentOrder.EmploymentOrderTypeId);
                            if (!EmploymentOrder.Descript.HasValue() || await Confirm("آیا با ثبت موافقید؟"))
                                    EmploymentOrder.Descript = old.Description;
		      }
		}
        }
}