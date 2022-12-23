using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public partial class DataGrid<TEntity> : ComponentBase
    {
        IList<TEntity> deletedEntities;
        IList<int> updatedEntitiesId;
        IList<TEntity> source;
        IDictionary<string, LambdaExpression> expressionList;

        [Parameter]
        public bool Batch { get; set; }

        IQueryable GetQueryForType(Type type, object value)
        {
            var serviceType = typeof(ISimpleService<>).MakeGenericType(type);
            var service = ServiceScopeFactory.CreateScope().ServiceProvider.GetService(serviceType) as ISimpleService;
            var query = service.GetAllRecords();
            var param = Expression.Parameter(type, "t");
            Expression expr = Expression.Property(param, type.GetPrimaryKey());
            expr = Expression.Equal(expr, Expression.Constant(value));
            var lambda = Expression.Lambda(expr, param);
            return query.Where(lambda);
        }

        internal void ClearSource()
        {
            source = new List<TEntity>();
            Total = 0;
            Items = new List<TEntity>();
        }

        public IList<TEntity> AllRecords()
        {
            return source;
        }

        public async Task<string> PostToDatabaseAsync()
        {
            var serviceType = typeof(ISimpleService<TEntity>);
            var service = ServiceScopeFactory.CreateScope().ServiceProvider.GetService(serviceType) as SimpleService<TEntity>;
            var insertedEntities = GetInsertedEntities();
            foreach(var entity in insertedEntities)
            {
                var result = await service.ValidateAsync(entity);
                if (!result.IsValid)
                    return await Task.FromResult(result.Errors[0].ErrorMessage);
            }
            var updatedEntities = GetUpdatedEntities();
            foreach (var entity in updatedEntities)
            {
                var result = await service.ValidateAsync(entity);
                if (!result.IsValid)
                    return await Task.FromResult(result.Errors[0].ErrorMessage);
            }
            var deletedEntities = GetDeletedEntities();
            foreach (var entity in deletedEntities)
            {
                var result = await service.ValidateRemoveAsync(entity);
                if (!result.IsValid)
                    return await Task.FromResult(result.Errors[0].ErrorMessage);
            }
            var insertedList = new List<TEntity>();
            foreach(var item in insertedEntities)
            {
                var newItem = Activator.CreateInstance<TEntity>();
                foreach (var info in typeof(TEntity).GetProperties())
                {
                    if (info.GetCustomAttribute<ForeignKeyAttribute>() == null && !info.PropertyType.IsEnumerableType())
                        info.SetValue(newItem, info.GetValue(item));
                }
                insertedList.Add(newItem);
            }
            var updateList = new List<TEntity>();
            foreach (var item in updatedEntities)
            {
                var newItem = Activator.CreateInstance<TEntity>();
                foreach (var info in typeof(TEntity).GetProperties())
                {
                    if (info.GetCustomAttribute<ForeignKeyAttribute>() == null && !info.PropertyType.IsEnumerableType())
                        info.SetValue(newItem, info.GetValue(item));
                }
                updateList.Add(newItem);
            }
            var deletedList = new List<TEntity>();
            foreach(var item in deletedEntities)
            {
                var newItem = Activator.CreateInstance<TEntity>();
                foreach (var info in typeof(TEntity).GetProperties())
                {
                    if (info.GetCustomAttribute<ForeignKeyAttribute>() == null && !info.PropertyType.IsEnumerableType())
                        info.SetValue(newItem, info.GetValue(item));
                }
                deletedList.Add(newItem);
            }
            using var transaction = service.Context.Database.BeginTransaction();
            if (insertedList.Count > 0)
            {
                await service.AddRangeAsync(insertedList);
            }
            if (updateList.Count > 0)
                service.UpdateRange(updateList);

            if (deletedEntities.Count > 0)
                service.RemoveRange(deletedList);
            await service.SaveChangesAsync();
            await transaction.CommitAsync();
            await Reload();
            return null;
        }

        public async Task InsertAsync(TEntity entity)
        {
            var type = typeof(TEntity);
            type.GetPrimaryKey().SetValue(entity, 0);
            await UpdateEntityForForeignKey(entity);
            source.Add(entity);
            Total = source.Count;
            var index = 1;
            foreach(var item in source)
            {
                if (item == entity)
                {
                    var pageNumber = (index - 1) / PageSize + 1;
                    SelectedRowIndex = (index - 1) % PageSize;
                    await ChangePageNumber(pageNumber);
                    break;
                }
                index++;
            }
        }

        async Task UpdateEntityForForeignKey(TEntity entity)
        {
            var type = typeof(TEntity);
            foreach (var info in type.GetProperties())
            {
                var attr = info.GetCustomAttribute<ForeignKeyAttribute>();
                if (attr != null)
                {
                    var foreignKeyInfo = type.GetProperty(attr.Name);
                    var value = foreignKeyInfo.GetValue(entity);
                    if (value != null && !value.Equals(0))
                    {
                        var selectExpr = expressionList.SingleOrDefault(t => t.Key == info.Name).Value;
                        if (selectExpr != null)
                        {
                            var query = GetQueryForType(info.PropertyType, value);
                            var list = await query.Select(selectExpr).ToDynamicListAsync();
                            var result = list.SingleOrDefault();
                            var foreignKeyValue = Activator.CreateInstance(info.PropertyType);
                            foreach (PropertyInfo info1 in result.GetType().GetProperties())
                                if (info1.Name != "Item")
                                    IQueryableExtension.UpdateEntity(foreignKeyValue, info1.Name, info1.GetValue(result));
                            info.SetValue(entity, foreignKeyValue);
                        }
                    }
                }
            }
        }

        public async Task UpdateAsync(TEntity entity)
        {
            var pkey = typeof(TEntity).GetPrimaryKey();
            var id = Convert.ToInt32(pkey.GetValue(entity)); 
            if (id > 0 && !updatedEntitiesId.Contains(id))
                updatedEntitiesId.Add(id);
            await UpdateEntityForForeignKey(entity);
            for (var index = 0; index < source.Count; index++)
            {
                if (Convert.ToInt32(pkey.GetValue(source[index])) == id)
                {
                    source[index] = entity;
                    var pageNumber = index / PageSize + 1;
                    SelectedRowIndex = index % PageSize;
                    await ChangePageNumber(pageNumber);
                    break;
                }
            }
        }

        public IList<TEntity> GetUpdatedEntities()
        {
            var list = new List<TEntity>();
            var pKey = typeof(TEntity).GetPrimaryKey();
            foreach (var id in updatedEntitiesId)
            {
                var item = source.Single(t => pKey.GetValue(t).Equals(id));
                list.Add(item.CreateNewSimpleEntity());
            }
            return list;
        }

        public IList<TEntity> GetDeletedEntities()
        {
            var list = new List<TEntity>();
            foreach(var item in deletedEntities)
                list.Add(item.CreateNewSimpleEntity());
            return list;
        }

        /// <summary>
        /// This method used for insert records to database
        /// </summary>
        /// <returns></returns>
        public IList<TEntity> GetInsertedEntities()
        {
            var list = new List<TEntity>();
            var pKey = typeof(TEntity).GetPrimaryKey();
            foreach(var item in source)
            {
                var id = pKey.GetValue(item);
                if (id.Equals(0))
                    list.Add(item.CreateNewSimpleEntity());
            }
            return list;
        }

        public async Task RemoveAsync(TEntity entity)
        {
            var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(entity));
            using var scope = ServiceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetService(typeof(ISimpleService<TEntity>)) as SimpleService<TEntity>;
            var result = await service.ValidateRemoveAsync(entity);
            if (result.IsValid)
            {
                var index = 1;
                foreach (var item in source)
                {
                    if (item == entity)
                        break;
                    index++;
                }
                source.Remove(entity);
                Total = source.Count;
                if (index > Total)
                    index = Total;
                var pageNumber = (index - 1) / PageSize + 1;
                SelectedRowIndex = (index - 1) % PageSize;
                await ChangePageNumber(pageNumber);
                if (id > 0)
                {
                    updatedEntitiesId.Remove(id);
                    deletedEntities.Add(entity);
                }
            }
            else
                errorMessage = result.Errors[0].ErrorMessage;
            StateHasChanged();
        }

        void ShowItemsForBatch()
        {
            if (PageNumber > 1)
            {
                var skip = (PageNumber - 1) * PageSize;
                Items = source.Skip(skip).Take(PageSize).ToList();
            }
            else
                Items = source.Take(PageSize).ToList();
        }

        void ManageExpressionForUpsert(IList<MemberExpression> list)
        {
            updatedEntitiesId = new List<int>();
            deletedEntities = new List<TEntity>();
            expressionList = new Dictionary<string, LambdaExpression>();
            var dic = new Dictionary<string, IList<MemberExpression>>();
            foreach (var expression in list)
            {
                var str = expression.ToString();
                var index = str.IndexOf('.');
                str = str.Substring(index + 1);
                var type = typeof(TEntity);
                var array = str.Split('.');
                if (array.Length > 1)
                {
                    var info = type.GetProperty(array[0]);
                    var attr = info.GetCustomAttributes<ForeignKeyAttribute>();
                    if (attr != null)
                    {
                        var item = dic.SingleOrDefault(t => t.Key == array[0]);
                        if (item.Key == null)
                        {
                            var tempList = new List<MemberExpression>();
                            tempList.Add(expression);
                            dic.Add(array[0], tempList.ToList());
                        }
                        else
                            item.Value.Add(expression);
                    }
                }
            }
            foreach(var item in dic)
            {
                var type = typeof(TEntity).GetProperty(item.Key).PropertyType;
                var param = Expression.Parameter(type, "t");
                var exprList = new List<MemberExpression>();
                foreach(var expr in item.Value)
                {
                    var str = expr.ToString();
                    str = str.Substring(str.IndexOf('.') + 1);
                    str = str.Substring(str.IndexOf('.') + 1);
                    var newExpr = param.CreateMemberExpresion(str);
                    exprList.Add(newExpr);
                }
                var lambda = param.CreateLambdaExpresion(exprList);
                expressionList.Add(item.Key, lambda);
            }
        }
    }
}
