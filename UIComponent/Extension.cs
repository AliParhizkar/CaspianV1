using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace Caspian.UI
{
    public static class Extension
    {
        public async static Task<byte[]> GetByteArrayAsync(this IBrowserFile file)
        {
            var stream = file.OpenReadStream();
            var buffer = new byte[file.Size];
            await stream.ReadAsync(buffer);
            return buffer;
        }
    }
}
