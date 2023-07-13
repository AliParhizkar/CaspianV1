using Caspian.UI;
using Caspian.Common;
using Caspian.Engine.Service;
using Microsoft.AspNetCore.Components;

namespace Caspian.Engine.WorkflowEngine
{
    public partial class AssemblyGenerator: BasePage 
    {
        WorkflowForm form;
        Type DynamicType;
        string errorMessage;

        protected override async Task OnInitializedAsync()
        {
            using var service = CreateScope().GetService<WorkflowFormService>();
            form = await service.GetWorkflowForm(WorkflowFormId);
            try
            {
                var userCode = await form.GetSourceFile(Environment.ContentRootPath);
                userCode += form.CreateEnumesCode();

                var strSource = form.WorkflowGroup.GetCode(form, userCode);
                DynamicType = form.WorkflowGroup.CreateAssembly(form.Name, strSource);
                errorMessage = null;
            }
            catch (CaspianException ex)
            {
                errorMessage = ex.Message;
            }
            await base.OnInitializedAsync();
        }
        
        [Parameter]
        public int WorkflowFormId { get; set; }
    }
}
