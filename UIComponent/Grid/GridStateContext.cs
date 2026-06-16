using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Caspian.UI
{
    public class GridStateContext
    {
        public Func<string,string, Task> SaveGridStateAsync;
        public Func<string, Task<string>> GetGridStateAsync;
    }
}
