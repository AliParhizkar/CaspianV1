using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class DataGrid<TEntity> : ComponentBase
    {
        IList<int> deletedEntities;

        IList<int> updatedEntities;

        [Parameter]
        public IList<TEntity> Source { get; set; }

        public async Task InsertAsync(TEntity entity)
        {
            
        }

        public async Task UpdateAsync(TEntity entity)
        {

        }

    }
}
