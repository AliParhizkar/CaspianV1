using Employment.Model;
using Capian.Dynamicform.Component;

namespace Caspian.Dynamic.WorkflowForm
{
	public partial class Husband
	{
		public void Initialize()
		{
                    cmbEducationDegree.TextExpression =  t => t.Title;
			cmbEducationDegree.ConditionExpression = t => t.BaseStudy == Marriage.BaseStudy;			
		}

		public void ddlBaseStudyOnChange()
		{
                    if (Marriage.BaseStudy >= BaseStudy.AssociateDegree)
                            cmbEducationDegree.Enable();
                    else
                            cmbEducationDegree.Disable();
			cmbEducationDegree.EnableLoading();
		}
    }
}