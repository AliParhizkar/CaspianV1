using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Caspian.UI
{
    internal interface ICaspianForm
    {
        void AddControl(IControl control);

        void SetFirstControl(IControl control);

        EditContext EditContext { get; }

        IControl GetFirstInvalidControl();

        string MasterIdName { get; set; }
        
        bool IgnoreOnValidSubmit { get; set; }
    }

    internal interface ICaspianForm<TEntity>: ICaspianForm where TEntity : class
    {
        CaspianValidationValidator<TEntity> ValidationValidator { get; set; }
    }

    internal interface ICaspianContainer
    {
        string GetLableContainerCSSClassName(int colSpan);

        string GetControlContainerCSSClassName(int colSpan);
    }
}
