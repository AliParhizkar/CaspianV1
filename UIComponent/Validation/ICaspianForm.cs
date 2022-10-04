using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Caspian.UI
{
    internal interface ICaspianForm
    {
        void AddControl(IControl control);

        EditContext EditContext { get; set; }

        EventCallback<EditContext> OnInternalInvalidSubmit { get; set; }

        CaspianValidationValidator ValidationValidator { get; set; }

        string MasterIdName { get; set; }
        
        bool IgnoreOnValidSubmit { get; set; }
    }
}
