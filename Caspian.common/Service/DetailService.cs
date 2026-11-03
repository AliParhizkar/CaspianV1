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
    public class DetailService<TEntity> : BaseService<TEntity> where TEntity : class
    {
        IList<ChangedEntity<TEntity>> changedEntities;
        int masterId;
        Type masterType;
        public DetailService(IServiceProvider provider) 
            :base(provider) 
        { 

        }

        protected IReadOnlyCollection<TEntity> Source {  get; private set; }

        internal void SetDetails(IEnumerable<TEntity> details)
        {
            Source = details?.ToList()?.AsReadOnly() ?? (new List<TEntity>()).AsReadOnly();
        }

        internal void SetMasterType(Type masterType) => this.masterType = masterType;

        internal void SetMasterData(int masterId, Type masterType)
        {
            this.masterId = masterId;
            this.masterType = masterType;
        }

        internal void SetChangedEntities(IList<ChangedEntity<TEntity>> changedEntities)
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

        public override async Task<TEntity> AddAsync(TEntity entity)
        {
            /// For Base Service masterType is null
            if (masterType == null) 
                return await base.AddAsync(entity);
            ///In Master-Details service for 'Detail' Service 'masterType' is not null 
            ///Note: In Master-Details details added with master together
            var result = await ValidateAsync(entity);
            if (!result.IsValid)
                throw new CaspianException(result.Errors.First().ErrorMessage);
            return entity.ClearEntityProperties();
        }

        internal async Task<IEnumerable<TEntity>> GetDetailsForInsertAsync(Type masterType)
        {
            masterId = 0;
            this.masterType = masterType;
            var list = new List<TEntity>();
            var pKey = typeof(TEntity).GetPrimaryKey();
            foreach (var change in changedEntities)
            {
                await AddAsync(change.Entity);
                pKey.SetValue(change.Entity, 0);
                list.Add(change.Entity);
            }
            return list;
        }

        internal async Task RemoveDetailsAsync(IList<TEntity> entities)
        {
            var pKey = typeof(TEntity).GetPrimaryKey();
            foreach (var entity in entities)
            {
                var value = Convert.ToInt32(pKey.GetValue(entity));
                await RemoveAsync(value);
            }
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
