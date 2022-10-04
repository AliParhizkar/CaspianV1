using Employment.Model;
using Employment.Service;
using System.Threading.Tasks;
using Capian.Dynamicform.Component;

namespace Caspian.Dynamic.WorkflowForm
{
    public partial class Children
    {
               BaseStudy?  baseStudy = null;
               int[] degreesFactor = {50, 60, 70, 80};
               int?[,] baseNumbers = 
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

                public async Task cmbEmploymentOrderTypeOnChange()
                {
                      if (employmentOrder.EmploymentOrderTypeId > 0)
                      {
                                    if (!employmentOrder.Descript.HasValue() || (await Confirm("آیا با تغییر شرح حکم موافقید؟")) == true)
                                    {
                                            using var scope = CreateScope();
                                            var old = await new EmploymentOrderTypeService(scope).SingleAsync(employmentOrder.EmploymentOrderTypeId );
                                            employmentOrder.Descript = old.Description;
                                    }
                      }
                }

                public async Task cmbEducationDegreeOnChange()
                {
                          if (employmentOrder.EducationDegreeId > 0)
                          {
                                        using var scope = CreateScope();
                                  var old = await new EducationDegreeService(scope).SingleAsync(employmentOrder.EducationDegreeId);
                                  baseStudy = old.BaseStudy;
                                  if (baseStudy > BaseStudy.Diploma)
                                          EducationDegreeFactor = degreesFactor[baseStudy.ConvertToInt().Value - 3];  
                                  else
                                          EducationDegreeFactor = null;
                          }
                          else
                                  EducationDegreeFactor = null;
                          if (EducationDegreeFactor == null)
                                  txtEducationDegreeFactor.Disable();
                         else
                                  txtEducationDegreeFactor.Enable();
                        ddlBaseTypeOnChange();
                }

                public void ddlBaseTypeOnChange()
                {
                                if (baseStudy > BaseStudy.Diploma && employmentOrder.BaseType > 0)
                                        BaseNumber = baseNumbers[employmentOrder.BaseType.ConvertToInt().Value - 1, baseStudy.ConvertToInt().Value - 3];
                                else
                                       BaseNumber = null;
                                if (BaseNumber == null)
                                        txtBaseNumber.Disable();
                               else
                                        txtBaseNumber.Enable();
                }
        }
}
