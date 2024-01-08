using Microsoft.JSInterop;

namespace Caspian.Report
{
    public static class Extension
    {
        public static async Task<RecData> GetClientRecById(this IJSRuntime jSRuntime, string id)
        {
            return await jSRuntime.InvokeAsync<RecData>("eval", $"getClientRecById('{id}')");
        }

        public static double Floor(this double d, int digits)
        {
            d = d * Math.Pow(10, digits);
            d = Convert.ToInt32(d);
            return d / Math.Pow(10, digits);
        }
    }
}
