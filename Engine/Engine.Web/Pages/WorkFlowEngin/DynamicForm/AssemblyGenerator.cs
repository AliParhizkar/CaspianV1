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
                DynamicType = await form.GetFormType(Environment.ContentRootPath);
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
