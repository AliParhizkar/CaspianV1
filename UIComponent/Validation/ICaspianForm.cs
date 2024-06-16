using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Caspian.UI
{
    internal interface ICaspianForm
    {
        void AddControl(IControl control);

        void SetFirstControl(IControl control);

        EditContext EditContext { get; }

        EventCallback<EditContext> OnInternalInvalidSubmit { get; set; }

        CaspianValidationValidator ValidationValidator { get; set; }

        IControl GetFirstInvalidControl();

        string MasterIdName { get; set; }
        
        bool IgnoreOnValidSubmit { get; set; }
    }
}
