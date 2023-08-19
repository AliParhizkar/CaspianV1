using Microsoft.Extensions.DependencyInjection;

namespace Capian.Dynamicform.Component
{
    public class BasePage
    {
        protected IServiceScope CreateScope()
        {
            return null;
        }

        protected void ShowMessage(string message)
        {

        }

        public void Alert(string message)
        {

        }

        public async Task<bool> Confirm(string message)
        {
            return await Task.FromResult(true);
        }
    }
}
