using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponent.Grid
{
    public class GridStateContext
    {
        public Func<string,string, Task> SaveGridStateAsync;
        public Func<string, Task<string>> GetGridStateAsync;
    }
}
