using System;
using System.Text;
using System.Linq;
using Caspian.Common;
using System.Threading.Tasks;
using Caspian.Common.Service;
using System.Collections.Generic;
using Caspian.Common.Extension;

namespace Caspian.Common.Service
{
    public class DetailService<TEntity> : BaseService<TEntity> , IDetailService<TEntity> where TEntity : class
    {

        IList<ChangedEntity<TEntity>> changedEntities;

        public DetailService(IServiceProvider provider) 
            :base(provider) 
        { 
        
        }

        protected IReadOnlyCollection<TEntity> Source {  get; private set; }

        public void SetDetails(IEnumerable<TEntity> details)
        {
            Source = details.ToList().AsReadOnly();
        }

        public void SetChangedEntities(IList<ChangedEntity<TEntity>> changedEntities)
        {
            this.changedEntities = changedEntities;
        }

        public async Task SaveChangeAsync()
        {
            foreach (var change in this.changedEntities)
            {
                switch(change.ChangeType)
                {
                    case ChangeType.Added:
                        await AddAsync(change.Entity);
                        break;
                    case ChangeType.Updated:
                        break;
                    case ChangeType.Deleted:
                        var pKey = typeof(TEntity).GetPrimaryKey();
                        var id = Convert.ToInt32(pKey.GetValue(change.Entity));
                        await RemoveAsync(id);
                        break;
                }

            }
        }
    }
}
