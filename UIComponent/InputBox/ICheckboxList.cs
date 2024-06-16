using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public interface IControl: IDisposable
    {
        Task FocusAsync();

        ElementReference? InputElement { get; }

        Task ResetAsync();

        bool HasError();
    }
}
