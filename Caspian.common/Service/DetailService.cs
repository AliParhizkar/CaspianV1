using System;
using System.Text;
using System.Linq;
using Caspian.Common;
using System.Threading.Tasks;
using Caspian.Common.Service;
using Caspian.Common.Extension;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

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
            Source = details?.ToList()?.AsReadOnly() ?? (new List<TEntity>()).AsReadOnly();
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
                        var newEntity = Activator.CreateInstance<TEntity>();
                        foreach(var info in typeof(TEntity).GetProperties())
                        {
                            var type = info.PropertyType.GetUnderlyingType();
                            if (type.IsValueType || type == typeof(string) || type == typeof(byte[]))
                            {
                                var value = info.GetValue(change.Entity);
                                info.SetValue(newEntity, value);
                            }
                        }
                        typeof(TEntity).GetPrimaryKey().SetValue(newEntity, 0);
                        await AddAsync(newEntity);
                        break;
                    case ChangeType.Updated:
                        await UpdateAsync(change.Entity);
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
