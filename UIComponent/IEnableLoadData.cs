namespace Caspian.UI
{
    public interface IEnableLoadData
    {
        void EnableLoading();
    }

    public interface IWindow: IDisposable
    {
        Task Close();
        
    }
}
