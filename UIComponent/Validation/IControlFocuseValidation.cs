using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public interface IControlFocuseValidation
    {
        bool IsFirstInvalidControl { get; set; }
    }
}
