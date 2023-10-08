using System.Threading.Tasks;

namespace Caspian.UI
{
    public interface IListValueInitializer
    {
        Task IncPageNumber();

        void Close();
    }
}
