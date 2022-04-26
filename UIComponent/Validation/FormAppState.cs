
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public class FormAppState
    {
        public bool AllControlsIsValid { get; set; }

        public ElementReference? Element { get; set; }

        public string ErrorMessage { get; set; }
    }
}
