using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caspian.UI
{
    public interface IControl
    {
        Task FocusAsync();

        Task ResetAsync();
    }
}
