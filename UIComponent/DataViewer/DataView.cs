using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using FluentValidation.Results;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public abstract partial class DataView<TEntity>: ComponentBase, IDisposable where TEntity : class
    {
        /// <summary>
        /// Return total count of records base on grid query
        /// </summary>
        public int Total { get; protected set; }

        /// <summary>
        /// Return index of selected row in grid (base on zero) 
        /// </summary>
        public int? SelectedRowIndex { get; protected set; }

        [Parameter]
        public TEntity Search { get; set; }

        [Parameter]
        public bool? HideFooter { get; set; }

        [Parameter]
        public SelectType SelectType { get; set; } = SelectType.Single;

        [Parameter]
        public EventCallback<UpsertMode> OnCancel { get; set; }

        [Parameter]
        public int? ContentHeight { get; set; } = 250;

        [Parameter]
        public string DeleteMessage { get; set; }

        [Parameter]
        public bool? ShowInsertIcon { get; set; }

        [Parameter]
        public ISearchService<TEntity> Service { get; set; }

        [Parameter]
        public IBatchService<TEntity> DetailsService { get; set; }

        internal IInternalBatchService<TEntity> BatchService { get { return DetailsService as IInternalBatchService<TEntity>; } }

        [Parameter]
        public Expression<Func<TEntity, bool>> ConditionExpr { get; set; }

        [Parameter]
        public Func<TEntity, Task<bool>> OnUpsertAsync { get; set; }

        [Parameter]
        public EventCallback<TEntity> OnOpen { get; set; }

        [Parameter]
        public EventCallback OnSave { get; set; }

        [Parameter]
        public bool Inline { get; set; }

        [Parameter]
        public int PageSize { get; set; } = 5;

        /// <summary>
        /// Hide after Upsert
        /// </summary>
        [Parameter]
        public bool AutoHide { get; set; }

        [Parameter]
        public bool Batch { get; set; }

        /// <summary>
        /// On Delete Button Clicked(both simple & Master-Details state)
        /// </summary>
        [Parameter]
        public Func<TEntity, Task<bool>> OnDelete { get; set; }

        /// <summary>
        ///In master-details raise after any change on Data View list (add, update & delete)
        /// </summary>
        [Parameter]
        public EventCallback<ChangedEntity<TEntity>> OnChanged { get; set; }

        [Parameter]
        public EventCallback OnPageChanged { get; set; }

        public abstract Task<TEntity> SelectRowById(int id);

        /// <summary>
        /// This method return selected row data 
        /// </summary>
        /// <returns>Selected row data</returns>
        public TEntity GetSelectedData()
        {
            if (SelectedRowIndex == null || items == null || items.Count < SelectedRowIndex.Value || SelectedRowIndex == -1)
                return null;
            return items.ElementAt(SelectedRowIndex.Value);
        }

        public async Task FocusInsertButtonAsync()
        {
            if (inertButton.HasValue)
                await inertButton.Value.FocusAsync();
        }

        public async Task ResetGrid()
        {
            SelectedRowIndex = 0;
            shouldFetchData = true;
            await ChangePageNumber(1);
        }

        /// <summary>
        /// Return entities in batch state
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TEntity> GetBatchEntities()
        {
            return BatchService.Details;
        }

        public int PageNumber { get { return pageNumber; } }

        public int PageCount { get { return (Total - 1) / PageSize + 1; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ScrollIntoViewSelectedRow()
        {
            StateHasChanged();
            await jsRuntime.InvokeVoidAsync("caspian.common.scrollIntoViewSelectedRow", mainDiv);
        }

        public async Task ValidateAndUpsert(UpsertMode upsertMode)
        {
            if (upsertMode == UpsertMode.Edit)
            {
                if (OnUpsertAsync != null && !await OnUpsertAsync(EditContext.Model as TEntity))
                    return;
                disableInsertIcon = false;
                FormAppState.AllControlsIsValid = true;
                //FormAppState.Control = null;
                FormAppState.ErrorMessage = null;
                EditContext.Validate();
                EditContext.Properties.TryGetValue("AsyncValidationTask", out var asyncValidationTask);
                var result = await (Task<ValidationResult>)asyncValidationTask;
                if (result.IsValid)
                {
                    if (OnChanged.HasDelegate)
                    {
                        var change = new ChangedEntity<TEntity>();
                        change.Entity = selectedEntity;
                        change.ChangeType = ChangeType.Updated;
                        await OnChanged.InvokeAsync(change);
                    }
                    if (DetailsService.Details == null)
                    {
                        using var scope = ServiceScopeFactory.CreateScope();
                        var service =  scope.GetService<BaseService<TEntity>>();
                        await service.UpdateAsync(selectedEntity);
                        await service.SaveChangesAsync();
                        await ReloadAsync();
                    }
                    else
                        await UpdateAsync(EditContext.Model as TEntity);
                    selectedEntity = null;
                    if (OnSave.HasDelegate)
                        await OnSave.InvokeAsync();

                }
            }
            else
            {
                if (OnUpsertAsync != null && !await OnUpsertAsync(InsertContext.Model as TEntity))
                    return;
                FormAppState.AllControlsIsValid = true;
                FormAppState.ErrorMessage = null;
                //FormAppState.Control = null;
                InsertContext.Validate();
                InsertContext.Properties.TryGetValue("AsyncValidationTask", out var asyncValidationTask);
                var result = await (Task<ValidationResult>)asyncValidationTask;
                if (result.IsValid)
                {
                    var newEntity = Activator.CreateInstance<TEntity>();
                    foreach (var info in typeof(TEntity).GetProperties())
                    {
                        var type = info.PropertyType;
                        if (type.IsValueType || type.IsNullableType() || type == typeof(string))
                            info.SetValue(newEntity, info.GetValue(insertedEntity.Data));
                    }
                    await InsertAsync(newEntity);
                    await ReadyToInsert();
                    if (OnSave.HasDelegate)
                        await OnSave.InvokeAsync();
                }
                else
                {
                    if (FormAppState.AllControlsIsValid)
                        FormAppState.ErrorMessage = result.Errors.First().ErrorMessage;
                }
            }
            StateHasChanged();
            await FocusInsertButtonAsync();
        }

        public async Task CancelUpsert(UpsertMode upsertMode)
        {
            if (upsertMode == UpsertMode.Edit)
            {
                disableInsertIcon = false;
                RollBackEntity();
                selectedEntity = null;
                EditContext = null;
                if (Batch && !AutoHide)
                    InsertContext = new EditContext(insertedEntity.Data);
            }
            else
                await ReadyToInsert();
            if (OnCancel.HasDelegate)
                await OnCancel.InvokeAsync(upsertMode);

            StateHasChanged();
        }

        public async Task InsertAsync(TEntity entity)
        {
            BatchService.UpdateDetailsByChangedEntity(entity, ChangeType.Added);
            await UpdateEntityForForeignKey(entity);
            Total = BatchService.Details.Count();
            var index = 1;
            foreach (var item in BatchService.Details)
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
            if (OnChanged.HasDelegate)
            {
                var change = new ChangedEntity<TEntity>()
                {
                    ChangeType = ChangeType.Added,
                    Entity = entity
                };
                await OnChanged.InvokeAsync(change);
            }
        }

        public async Task UpdateAsync(TEntity entity)
        {
            BatchService.UpdateDetailsByChangedEntity(entity, ChangeType.Updated);
            await UpdateEntityForForeignKey(entity);
            
            foreach (var detail in BatchService.Details)
            {
                if (detail == entity)
                {
                    var index = BatchService.Details.ToList().IndexOf(detail);
                    var pageNumber = index / PageSize + 1;
                    SelectedRowIndex = index % PageSize;
                    await ChangePageNumber(pageNumber);
                    if (OnChanged.HasDelegate)
                    {
                        var change = new ChangedEntity<TEntity>()
                        {
                            Entity = entity,
                            ChangeType = ChangeType.Updated,
                        };
                        await OnChanged.InvokeAsync();
                    }
                    break;
                }
            }
        }

        public async Task<bool> RemoveAsync(TEntity entity)
        {
            var pKey = typeof(TEntity).GetPrimaryKey();
            var id = Convert.ToInt32(pKey.GetValue(entity));
            using var scope = ServiceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetService(typeof(IBaseService<TEntity>)) as BaseService<TEntity>;
            if (BatchService?.ThirdLevelProperty != null)
                service.BatchServiceData.DetailPropertiesInfo.Add(BatchService.ThirdLevelProperty);
            var result = await service.ValidateRemoveAsync(entity);
            if (result.IsValid)
            {
                var index = 1;
                foreach (var item in BatchService.Details)
                {
                    if (item == entity)
                        break;
                    index++;
                }

                BatchService.UpdateDetailsByChangedEntity(entity, ChangeType.Deleted);

                Total = BatchService.Details.Count();
                if (index > Total)
                    index = Total;
                var pageNumber = (index - 1) / PageSize + 1;
                SelectedRowIndex = (index - 1) % PageSize;
                await ChangePageNumber(pageNumber);
                foreach (var item in DetailsService.ChangedEntities)
                {
                    var newId = Convert.ToInt32(pKey.GetValue(item.Entity));
                    if (newId == id)
                    {
                        DetailsService.ChangedEntities.Remove(item);
                        break;
                    }
                }
                if (id > 0)
                {
                    var entityChange = new ChangedEntity<TEntity>() { Entity = entity, ChangeType = ChangeType.Deleted };
                    DetailsService.ChangedEntities.Add(entityChange);
                    deletedEntities.Add(entity);
                    if (OnChanged.HasDelegate)
                        await OnChanged.InvokeAsync(entityChange);
                }

            }
            else
                await jsRuntime.InvokeVoidAsync("caspian.common.showMessage", result.Errors.First().ErrorMessage);
            StateHasChanged();
            return result.IsValid;
        }

        /// <summary>
        /// Get the Entities added to source
        /// </summary>
        /// <returns></returns>
        public IList<TEntity> GetInsertedEntities()
        {
            var list = new List<TEntity>();
            var pKey = typeof(TEntity).GetPrimaryKey();
            foreach (var item in BatchService.Details)
            {
                var id = pKey.GetValue(item);
                if (id.Equals(0))
                    list.Add(item.CreateNewSimpleEntity());
            }
            return list;
        }

        public void EnableLoading() => shouldFetchData = true;

        public IList<TEntity> GetDeletedEntities()
        {
            var list = new List<TEntity>();
            foreach (var item in deletedEntities)
                list.Add(item.CreateNewSimpleEntity());
            return list;
        }

        public async Task SelectNextRow()
        {
            if (SelectType == SelectType.Single && SelectedRowIndex.HasValue)
            {
                if (SelectedRowIndex.Value + 1 < PageSize && SelectedRowIndex.Value + 1 < items.Count)
                    SelectRow(SelectedRowIndex.Value + 1);
                else
                {
                    if (pageNumber < PageCount)
                    {
                        SelectRow(0);
                        await ChangePageNumber(pageNumber + 1);
                        StateHasChanged();
                    }
                }
            }
        }

        public void SelectRow(int rowIndex) => SelectedRowIndex = rowIndex;

        public async Task SelectPrevRow()
        {
            if (SelectType == SelectType.Single && SelectedRowIndex.HasValue)
            {
                if (SelectedRowIndex.Value > 0)
                    SelectRow(SelectedRowIndex.Value - 1);
                else
                {
                    if (pageNumber < 1)
                    {
                        await ChangePageNumber(pageNumber - 1);
                        SelectRow(PageSize - 1);
                    }
                }
            }
        }

        public async Task ReloadAsync()
        {
            EnableLoading();
            await DataBind();
            var pageCount = (Total - 1) / PageSize + 1;
            if (pageCount < pageNumber)
            {
                await ChangePageNumber(pageCount);
                SelectedRowIndex = items.Count - 1;
            }
            else if (SelectedRowIndex != null && SelectedRowIndex.Value >= items.Count)
                SelectedRowIndex = items.Count - 1;
            StateHasChanged();
        }

        public void SetSelectedEntity(TEntity entity)
        {
            disableInsertIcon = true;
            RollBackEntity();
            selectedEntity = entity;//.CreateNewEntity();
            EditContext = new EditContext(selectedEntity);
            shouldSetFocus = true;
            shadowEntity = entity.CreateNewEntity();
            StateHasChanged();
        }

        public async Task OpenForInsert(TEntity entity) => await CreateInsert(entity);

        public async Task UpdateEntityAsync()
        {
            var context = EditContext ?? InsertContext;

            if (context?.Model != null)
            { 
                var model = context.Model as TEntity;
                foreach(var info in typeof(TEntity).GetProperties()) /// info: For example "Customer" Property in "Order" type
                {
                    var attr = info.GetCustomAttribute<ForeignKeyAttribute>();
                    if (attr != null)
                    {
                        var fKeyId = typeof(TEntity).GetProperty(attr.Name);///For example "CustomerId" property in "Order" type 
                        var id = fKeyId.GetValue(model);    /// The value of "CustomerId"
                        if (id != null && !id.Equals(0))    /// If CustomerId is not null and is not 0
                        {
                            if (expressionList.ContainsKey(info.Name))
                            {
                                var value = info.GetValue(model); /// The value of "Customer" property in "Order" 
                                ///If "Customer" value is null or value of 
                                ///"Id" (Primary Key) in "Customer" type is not equal by "CustomerId" (Foreign Key) in "Order"
                                if (value == null || info.PropertyType.GetPrimaryKey().GetValue(value) != id) 
                                {
                                    var query = GetQueryForType(info.PropertyType, id);
                                    var selectExpr = expressionList[info.Name];
                                    var list = await query.Select(selectExpr).ToDynamicListAsync();
                                    var result = list.SingleOrDefault();
                                    value = Activator.CreateInstance(info.PropertyType);
                                    info.SetValue(model, value);
                                    foreach (var item in list)
                                    {
                                        foreach (PropertyInfo property in item.GetType().GetProperties())
                                        {
                                            var info1 = info;
                                            foreach (var section in property.Name.Split('.'))
                                            {
                                                info1 = info1.PropertyType.GetProperty(section);
                                                if (info1.GetCustomAttribute<ForeignKeyAttribute>() == null)
                                                {
                                                    var value1 = property.GetValue(result);
                                                    info1.SetValue(value, value1);
                                                }
                                                else
                                                {
                                                    if (info1.GetValue(value) == null)
                                                    {
                                                        var newValue = Activator.CreateInstance(info1.PropertyType);
                                                        info1.SetValue(value, newValue);
                                                        value = newValue;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            (Service as IInternalSearchService<TEntity>)?.DataViewInitializer(null); 
            (DetailsService as IInternalBatchService<TEntity>)?.DetailDataViewInitializer(null);
        }
    }
}
