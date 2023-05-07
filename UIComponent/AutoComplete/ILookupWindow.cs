using System.Threading.Tasks;

namespace Caspian.UI
{
    internal interface ILookupWindow
    {
        Task<string> GetText(int id);
    }
}
