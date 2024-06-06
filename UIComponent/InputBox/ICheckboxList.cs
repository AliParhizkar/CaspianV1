namespace Caspian.UI
{
    public interface IControl
    {
        Task FocusAsync();

        Task ResetAsync();

        bool HasError();
    }
}
