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
    public abstract class DataView<TEntity>: ComponentBase, IDisposable where TEntity : class
    {
        protected RowData<TEntity> insertedEntity;
        protected EditContext InsertContext;
        protected bool insertContainerHoldHasFocus;
        protected TEntity selectedEntity;
        protected IList<TEntity> source;
        protected EditContext EditContext;
        protected bool shouldSetFocus;
        protected IList<TEntity> deletedEntities;
        protected bool shouldFetchData = true;
        protected CaspianContainer insertContainer;
        protected CaspianContainer updateContainer;
        protected IDictionary<string, LambdaExpression> expressionList;
        protected int pageNumber = 1;
        protected Type serviceType;
        protected CaspianValidationValidator<TEntity> validator;
        protected IList<TEntity> items;
        protected bool disableInsertIcon;
        protected bool? showInsertIcon;
        protected ElementReference mainDiv;
        protected ElementReference? inertButton;

        internal EventCallback<TEntity> OnInternalUpsert { get; set; }

        internal Expression InternalConditionExpr { get; set; }

        public int Total { get; set; }

        public int? SelectedRowIndex { get; protected set; }

        [Inject]
        public FormAppState FormAppState { get; set; }

        [Inject]
        protected IJSRuntime jsRuntime { get; set; }

        [Inject]
        public CaspianDataService CaspianDataService { get; set; }

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

        [Parameter]
        public Expression<Func<TEntity, bool>> ConditionExpr { get; set; }

        [Parameter]
        public Func<TEntity, Task<bool>> OnUpsertAsync { get; set; }

        [Parameter]
        public EventCallback<TEntity> OnOpen { get; set; }

        internal Action OnLoaded { get; set; }

        [Parameter]
        public EventCallback OnSave { get; set; }

        [Parameter]
        public bool Inline { get; set; }

        [Parameter]
        public int PageSize { get; set; } = 5;

        [Inject]
        public BatchServiceData BatchServiceData { get; set; }

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

        [Parameter]
        public EventCallback OnPageChanged { get; set; }

        [Inject]
        public IServiceScopeFactory ServiceScopeFactory { get; set; }

        internal EventCallback<TEntity> OnInternalDelete { get; set; }

        public abstract Task<TEntity> SelectRowById(int id);

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
        public ICollection<TEntity> GetBatchEntities()
        {
            if (source == null)
                return new List<TEntity>();
            return source;
        }

        internal abstract IQueryable<TEntity> GetQuery(IServiceScope scope);

        public abstract Task DataBind();

        public int PageNumber {get{ return pageNumber; }}

        public int PageCount {get{ return (Total - 1) / PageSize + 1; }}

        [CascadingParameter]
        internal PageData PageData { get; set; }

        protected override void OnInitialized()
        {
            var type = typeof(IBaseService<TEntity>);
            using var scope = ServiceScopeFactory.CreateScope();
            //BatchServiceData.MasterType = (DetailsService as ISimpleBatchService)?.MasterType;
            scope.SetUserId(PageData);
            serviceType = scope.ServiceProvider.GetService(type)?.GetType();
            if (serviceType == null)
                throw new CaspianException($"Service of type {type} not implemented");

            if (Service != null)
                (Service as IInternalSearchService<TEntity>).DataViewInitializer(this);
            if (DetailsService != null)
                (DetailsService as IInternalBatchService<TEntity>).DetailDataViewInitializer(this);
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            if (!AutoHide && Inline)
                await CreateInsert();
            await base.OnInitializedAsync();
        }

        internal void ChangeState() => StateHasChanged();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task ScrollIntoViewSelectedRow()
        {
            StateHasChanged();
            await jsRuntime.InvokeVoidAsync("caspian.common.scrollIntoViewSelectedRow", mainDiv);
        }

        internal void InsertIconState(bool flag)
        {
            if (AutoHide || !Inline)
                showInsertIcon = flag;
        }

        protected override void OnParametersSet()
        {
            if (DeleteMessage == null)
            {
                if (PageData?.Language != Language.En)
                    DeleteMessage = "آیا با حذف موافقید؟";
                else
                    DeleteMessage = "Do you agree to delete?";
            }
            base.OnParametersSet();
        }

        protected void ManageExpressionForUpsert(IList<MemberExpression> list)
        {
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
                            var tempList = new List<MemberExpression>
                            {
                                expression
                            };
                            dic.Add(array[0], tempList.ToList());
                        }
                        else
                            item.Value.Add(expression);
                    }
                }
            }
            foreach (var item in dic)
            {
                var type = typeof(TEntity).GetProperty(item.Key).PropertyType;
                var param = Expression.Parameter(type, "t");
                var exprList = new List<MemberExpression>();
                foreach (var expr in item.Value)
                {
                    var str = expr.ToString();
                    str = str.Substring(str.IndexOf('.') + 1);
                    str = str.Substring(str.IndexOf('.') + 1);
                    var newExpr = param.CreateMemberExpresion(str);
                    exprList.Add(newExpr);
                }
                var lambda = param.CreateLambdaExtension(exprList, false);
                expressionList.Add(item.Key, lambda);
            }
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
                    if (source == null)
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

        internal IList<TEntity>  GetSource() => source;

        internal async Task ReadyToInsert()
        {
            if (Inline)
            {
                if (AutoHide)
                {
                    insertedEntity = null;
                    InsertContext = null;
                }
                else
                {
                    await insertContainer.ResetAsync();
                    if (insertedEntity == null)
                    {
                        insertedEntity = new RowData<TEntity>();
                        insertedEntity.UpsertMode = UpsertMode.Insert;
                    }
                    insertedEntity.Data = Activator.CreateInstance<TEntity>();
                    if (DetailsService.MasterId > 0)
                        typeof(TEntity).GetForeignKey(DetailsService.MasterType).SetValue(insertedEntity.Data, DetailsService.MasterId);
                    InsertContext = new EditContext(insertedEntity.Data);
                }
            }
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

        internal void CancelInternalUpdate()
        {
            InsertContext = null;
            disableInsertIcon = false;
            RollBackEntity();
            selectedEntity = null;
            EditContext = null;
            if (Batch)
            {
                if (AutoHide)
                    insertedEntity = null;
                else if (insertedEntity != null)
                    InsertContext = new EditContext(insertedEntity.Data);
            }
        }

        public async Task InsertAsync(TEntity entity)
        {
            var pKey = typeof(TEntity).GetPrimaryKey();
            pKey.SetValue(entity, 0);
            using var scope = ServiceScopeFactory.CreateScope();
            var services = scope.ServiceProvider.GetServices(typeof(IBaseService<TEntity>));
            BaseService<TEntity> service = null;
            if (services.Count() == 1)
                service = services.Single() as BaseService<TEntity>;
            else
                service = services.Single(t => t.GetType().BaseType.GetGenericArguments().Count() == 1) as BaseService<TEntity>;
            service.SetBatchServiceData(DetailsService.MasterId, DetailsService.MasterType);
            await service.AddAsync(entity);
            DetailsService.ChangedEntities.Add(new ChangedEntity<TEntity>() 
            { 
                Entity = entity, 
                ChangeStatus = ChangeStatus.Added 
            });
            await UpdateEntityForForeignKey(entity);
            var minId = 0;
            foreach(var item in source)
            {
                var minValue = Convert.ToInt32(pKey.GetValue(item));
                if (minValue < minId)
                    minId = minValue;
            }
            pKey.SetValue(entity, minId - 1);
            source.Add(entity);
            Total = source.Count;
            var index = 1;
            foreach (var item in source)
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

        public async Task UpdateAsync(TEntity entity)
        {
            var key = typeof(TEntity).GetPrimaryKey();
            var id = Convert.ToInt32(key.GetValue(entity));
            TEntity old = null;
            foreach (var item in DetailsService.ChangedEntities.Where(t => t.ChangeStatus != ChangeStatus.Deleted))
            {
                var newId = Convert.ToInt32(key.GetValue(item.Entity));
                if (id == newId)
                {
                    old = item.Entity;
                    break;
                }
            }
            if (old != null)
            {
                ///Entity add already and only should updated on memory
                old.CopyEntity(entity);
            }
            else if (id > 0)
            {
                ///Entity exist in database and should be updated on database
                DetailsService.ChangedEntities.Add(new ChangedEntity<TEntity>()
                {
                    ChangeStatus = ChangeStatus.Updated,
                    Entity = entity
                });
            }

            await UpdateEntityForForeignKey(entity);
            for (var index = 0; index < source.Count; index++)
            {
                if (key.GetValue(source[index]).Equals(id))
                {
                    source[index] = entity;
                    var pageNumber = index / PageSize + 1;
                    SelectedRowIndex = index % PageSize;
                    await ChangePageNumber(pageNumber);
                    break;
                }
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
                        if (expressionList?.ContainsKey(info.Name) == true)
                        {

                            var selectExpr = expressionList[info.Name];
                            var query = GetQueryForType(info.PropertyType, value);
                            var list = await query.Select(selectExpr).ToDynamicListAsync();
                            var result = list.SingleOrDefault();
                            var foreignKeyValue = Activator.CreateInstance(info.PropertyType);
                            foreach (PropertyInfo info1 in result.GetType().GetProperties())
                            {
                                if (info1.IsCollectible)
                                    IQueryableExtension.UpdateEntity(foreignKeyValue, info1.Name, info1.GetValue(result));
                            }
                            info.SetValue(entity, foreignKeyValue);
                        }
                    }
                }
            }
        }

        protected IQueryable GetQueryForType(Type type, object value)
        {
            var serviceType = typeof(IBaseService<>).MakeGenericType(type);
            var service = ServiceScopeFactory.CreateScope().ServiceProvider.GetService(serviceType) as IBaseService;
            var query = service.GetAllRecords();
            var param = Expression.Parameter(type, "t");
            Expression expr = Expression.Property(param, type.GetPrimaryKey());
            expr = Expression.Equal(expr, Expression.Constant(value));
            var lambda = Expression.Lambda(expr, param);
            return query.Where(lambda);
        }

        protected IQueryable GetQueryForType(Type type, object[] value)
        {
            var serviceType = typeof(IBaseService<>).MakeGenericType(type);
            var service = ServiceScopeFactory.CreateScope().ServiceProvider.GetService(serviceType) as IBaseService;
            var query = service.GetAllRecords();
            var param = Expression.Parameter(type, "t");
            Expression expr = Expression.Property(param, type.GetPrimaryKey());
            expr = Expression.Equal(expr, Expression.Constant(value));
            var lambda = Expression.Lambda(expr, param);
            return query.Where(lambda);
        }

        public async Task<bool> RemoveAsync(TEntity entity)
        {
            var pKey = typeof(TEntity).GetPrimaryKey();
            var id = Convert.ToInt32(pKey.GetValue(entity));
            using var scope = ServiceScopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetService(typeof(IBaseService<TEntity>)) as BaseService<TEntity>;
            if (DetailsService != null && (DetailsService as IInternalBatchService<TEntity>).ThirdLevelProperty != null)
                service.BatchServiceData.DetailPropertiesInfo.Add((DetailsService as IInternalBatchService<TEntity>).ThirdLevelProperty);
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
                    DetailsService.ChangedEntities.Add(new ChangedEntity<TEntity>() { Entity = entity, ChangeStatus = ChangeStatus.Deleted });
                    deletedEntities.Add(entity);
                }
            }
            else
                await jsRuntime.InvokeVoidAsync("caspian.common.showMessage", result.Errors.First().ErrorMessage);
            StateHasChanged();
            return result.IsValid;
        }

        protected async Task ChangePageNumber(int pageNumber)
        {
            this.pageNumber = pageNumber;
            if (Batch)
                ShowItemsForBatch();
            else
            {
                shouldFetchData = true;
                await DataBind();
            }
            if (OnPageChanged.HasDelegate)
                await OnPageChanged.InvokeAsync();
        }

        /// <summary>
        /// Get the Entities added to source
        /// </summary>
        /// <returns></returns>
        public IList<TEntity> GetInsertedEntities()
        {
            var list = new List<TEntity>();
            var pKey = typeof(TEntity).GetPrimaryKey();
            foreach (var item in source)
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

        internal void ClearSource()
        {
            source .Clear();
            Total = 0;
            items.Clear();
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

        protected void ShowItemsForBatch()
        {
            if (pageNumber > 1)
            {
                var skip = (pageNumber - 1) * PageSize;
                items = source.Skip(skip).Take(PageSize).ToList();
            }
            else
                items = source.Take(PageSize).ToList();
        }

        protected void RollBackEntity()
        {
            if (selectedEntity != null)
            {
                if (source == null)
                {
                    //foreach (var info in typeof(TEntity).GetProperties())
                    //{
                    //    var value = info.GetValue(unchangedEntity);
                    //    info.SetValue(selectedEntity, value);
                    //}
                }
                else 
                {
                    //for (var i = 0; i < source.Count; i++)
                    //{
                    //    var sourceItem = source[i];
                    //    if (sourceItem.Equals(selectedEntity))
                    //    {
                    //        foreach (var info in typeof(TEntity).GetProperties())
                    //        {
                    //            var value = info.GetValue(unchangedEntity);
                    //            info.SetValue(sourceItem, value);
                    //        }
                    //        break;
                    //    }
                    //}
                }
            }
        }

        public void SetSelectedEntity(TEntity entity)
        {
            disableInsertIcon = true;
            RollBackEntity();
            selectedEntity = entity.CreateNewEntity();
            EditContext = new EditContext(selectedEntity);
            shouldSetFocus = true;
            //unchangedEntity = entity.CreateNewEntity();
            StateHasChanged();
        }

        public async Task OpenForInsert(TEntity entity) => await CreateInsert(entity);

        internal async Task CreateInsert(TEntity entity = default)
        {
            if (!disableInsertIcon)
            {
                insertedEntity = new RowData<TEntity>();
                insertedEntity.UpsertMode = UpsertMode.Insert;
                insertedEntity.Data = entity ?? Activator.CreateInstance<TEntity>();
                if (DetailsService.MasterId > 0)
                    typeof(TEntity).GetForeignKey(DetailsService.MasterType).SetValue(insertedEntity.Data, DetailsService.MasterId);
                if (OnOpen.HasDelegate)
                    await OnOpen.InvokeAsync(insertedEntity.Data);
                InsertContext = new EditContext(insertedEntity.Data);
                insertContainerHoldHasFocus = AutoHide;
                StateHasChanged();
            }
        }

        public void Dispose()
        {
            if (Service != null)
                (Service as IInternalSearchService<TEntity>).DataViewInitializer(null); 
            if (DetailsService != null)
                (DetailsService as IInternalBatchService<TEntity>).DetailDataViewInitializer(null);
        }
    }
}
