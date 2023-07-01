using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caspian.Dynamic.WorkflowForm
{
    internal class Program123
    {
        public void Method1()
        {
        }

        public Task Method2()
        {
            return Task.CompletedTask;
        }

        public async Task Method3()
        {
            await Task.CompletedTask;
        }
    }
}