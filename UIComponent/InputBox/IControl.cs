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

    public class Selection
    {
        public int Start { get; set; }

        public int End { get; set; }
    }
}
