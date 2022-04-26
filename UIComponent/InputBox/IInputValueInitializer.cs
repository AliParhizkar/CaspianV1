using System.Threading.Tasks;

namespace Caspian.UI
{
    public interface IInputValueInitializer
    {
        Task SetValue(object value);
    }

    public interface IListValueInitializer
    {
        Task IncPageNumber();

        void Close();
    }
}
