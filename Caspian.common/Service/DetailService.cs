using System;
using System.Text;
using System.Linq;
using Caspian.Common;
using FluentValidation;
using System.Threading.Tasks;
using Caspian.Common.Service;
using Caspian.Common.Extension;
using FluentValidation.Results;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;

namespace Caspian.Common.Service
{
    public class DetailService<TEntity> : BaseService<TEntity> , IDetailService<TEntity> where TEntity : class
    {
        IList<ChangedEntity<TEntity>> changedEntities;
        int masterId;
        Type masterType;
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

        public override Task<ValidationResult> ValidateAsync(ValidationContext<TEntity> context, CancellationToken cancellation = default)
        {
            context.RootContextData["__ServiceProvider"] = ServiceProvider;
            context.RootContextData["__MasterId"] = masterId;
            context.RootContextData["__MasterType"] = masterType;
            return base.ValidateAsync(context, cancellation);
        }

        public new async virtual Task AddAsync(TEntity entity)
        {
            var result = await ValidateAsync(entity);
            if (!result.IsValid)
                throw new CaspianException(result.Errors.First().ErrorMessage);

        }

        internal async Task SubmitChangesForUpdateAsync()
        {
            foreach (var change in this.changedEntities)
            {
                switch(change.ChangeType)
                {
                    case ChangeType.Added:
                        await AddAsync(change.Entity);
                        var newEntity = Activator.CreateInstance<TEntity>();
                        foreach (var info in typeof(TEntity).GetProperties())
                        {
                            var type = info.PropertyType.GetUnderlyingType();
                            if (type.IsValueType || type == typeof(string) || type == typeof(byte[]))
                            {
                                var value = info.GetValue(change.Entity);
                                info.SetValue(newEntity, value);
                            }
                        }
                        typeof(TEntity).GetPrimaryKey().SetValue(newEntity, 0);
                        await Context.Set<TEntity>().AddAsync(newEntity);
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
