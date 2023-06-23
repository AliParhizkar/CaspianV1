using FluentValidation;
using System.Reflection;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.Common.Service
{
    public class MasterDetailsService<TMaster, TDetails>: BaseService<TMaster> where TMaster : class 
    {
        public MasterDetailsService(IServiceProvider provider)
            : base(provider)
        {

        }

        public async override Task<TMaster> AddAsync(TMaster entity)
        {
            PropertyInfo detailsInfo = null;
            foreach (var info in typeof(TMaster).GetProperties())
            {
                var type = info.PropertyType;
                if ( info.GetCustomAttribute<ForeignKeyAttribute>() != null)
                    info.SetValue(entity, default);
                else if (type.IsCollectionType())
                {
                    if (type != typeof(string))
                    {
                        if (info.PropertyType.GetInterfaces().Contains(typeof(IEnumerable<TDetails>)))
                            detailsInfo= info;
                        else
                            info.SetValue(entity, default);
                    }
                }
            }
            var context = new ValidationContext<TMaster>(entity);
            context.RootContextData["__IgnorePropertyInfo"] = typeof(TDetails).GetForeignKey(typeof(TMaster));
            context.RootContextData["__MasterId"] = 0;
            var result = await ValidateAsync(context);
            if (result.Errors.Count > 0)
                throw new CaspianException(result.Errors[0].ErrorMessage);
            var newEntity = entity.CreateNewSimpleEntity();
            var details = new List<TDetails>();
            detailsInfo.SetValue(newEntity, details);
            foreach (var detail in detailsInfo.GetValue(entity) as IEnumerable<TDetails>)
            {
                var item = Activator.CreateInstance<TDetails>();
                foreach (var info in typeof(TDetails).GetProperties())
                {
                    var type = info.PropertyType;
                    if (type.IsValueType || type.IsNullableType() || type == typeof(string) || type == typeof(byte[]))
                        info.SetValue(item, info.GetValue(detail));
                }
                details.Add(item);
            }
            var result1 = await Context.Set<TMaster>().AddAsync(newEntity);
            return result1.Entity;
        }

        public async Task UpdateAsync(TMaster entity, IEnumerable<int> deletedIds)
        {
            var result = await ValidateAsync(entity);
            if (result.Errors.Count > 0)
                throw new CaspianException(result.Errors[0].ErrorMessage);
            var id = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(entity));
            var detailsInfo = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetails));
            var details = detailsInfo.GetValue(entity) as IEnumerable<TDetails>;
            var query = GetAll();
            TMaster old = default(TMaster);
            if (details != null && details.Count() > 0 || deletedIds != null && deletedIds.Count() > 0) 
            {
                old = await query.Include(detailsInfo.Name).SingleAsync(id);
                var detailKey = typeof(TDetails).GetPrimaryKey();
                var items = detailsInfo.GetValue(old) as ICollection<TDetails>;
                if (details != null && details.Count() > 0)
                {
                    foreach (var detail in details)
                    {
                        id = Convert.ToInt32(typeof(TDetails).GetPrimaryKey().GetValue(detail));
                        if (id == 0)
                            items.Add(detail);
                        else
                        {
                            var item = items.Single(t => Convert.ToInt32(detailKey.GetValue(t)) == id);
                            foreach (var info in typeof(TDetails).GetProperties())
                            {
                                var type = info.PropertyType;
                                if (type.IsValueType || type.IsNullableType() || type == typeof(string) || type == typeof(byte[]))
                                    info.SetValue(item, info.GetValue(detail));
                            }
                        }
                    }
                }
                else
                {
                    foreach (var deletedId in deletedIds)
                    {
                        var item = items.Single(t => Convert.ToInt32(detailKey.GetValue(t)) == deletedId);
                        items.Remove(item);
                    }
                }
            }
            
        }
    }
}
