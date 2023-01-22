using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace Caspian.UI
{
    public static class Extension
    {
        public async static Task<byte[]> GetByteArrayAsync(this IBrowserFile file)
        {
            using var stream = file.OpenReadStream(51200);
            var buffer = new byte[stream.Length];
            var sum = 0;
            var remine = (int)stream.Length - sum;
            var count = await stream.ReadAsync(buffer, 0, remine);
            while (count > 0)
            {
                sum += count;
                remine = (int)stream.Length - sum;
                if (remine > 10240)
                    remine = 10240;
                count = await stream.ReadAsync(buffer, sum, remine);
            }
            return buffer;
        }
    }
}
