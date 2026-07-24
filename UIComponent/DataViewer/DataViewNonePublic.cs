using Caspian.Common;
using Caspian.Common.Extension;
using Caspian.Common.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Caspian.UI
{
    public abstract partial class DataView<TEntity>
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
        protected IList<TEntity> selectedEntities;

        internal EventCallback<TEntity> OnInternalUpsert { get; set; }

        internal Expression InternalConditionExpr { get; set; }

        /// <summary>
        /// Data of Master-Service that initialized in Children Tab panel 
        /// </summary>
        [CascadingParameter]
        internal MasterDetailsCrudServiceData CrudServiceData { get; set; }

        [Inject]
        internal FormAppState FormAppState { get; set; }

        [Inject]
        protected IJSRuntime jsRuntime { get; set; }

        internal Action OnLoaded { get; set; }

        [Inject]
        internal BatchServiceData BatchServiceData { get; set; }

        [Inject]
        internal IServiceScopeFactory ServiceScopeFactory { get; set; }

        internal EventCallback<TEntity> OnInternalDelete { get; set; }

        [CascadingParameter]
        internal PageData PageData { get; set; }

        internal bool EntityIsSelected(TEntity entity)
        {
            if (selectedEntities == null)
                return false;
            return selectedEntities.Contains(entity);
        }

        internal void UpdateSelectedEntities(TEntity entity, bool isChecked)
        {
            if (selectedEntities == null)
                selectedEntities = new List<TEntity>();
            if (isChecked)
                selectedEntities.Add(entity);
            else
                selectedEntities.Remove(entity);
        }

        internal abstract IQueryable<TEntity> GetQuery(IServiceScope scope);

        internal abstract Task DataBind();

        protected override void OnInitialized()
        {
            var type = typeof(IBaseService<TEntity>);
            using var scope = ServiceScopeFactory.CreateScope();
            //BatchServiceData.MasterType = (DetailsService as ISimpleBatchService)?.MasterType;
            scope.SetUserId(PageData);
            serviceType = scope.ServiceProvider.GetService(type)?.GetType();
            if (serviceType == null)
                throw new CaspianException($"Service of type {type} not implemented");
            (Service as IInternalSearchService<TEntity>)?.DataViewInitializer(this);
            (DetailsService as IInternalBatchService<TEntity>)?.DetailDataViewInitializer(this);
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            if (!AutoHide && Inline)
                await CreateInsert();
            await base.OnInitializedAsync();
        }

        internal void ChangeState() => StateHasChanged();

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

        internal IList<TEntity> GetSource() => source;

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
                    if (value?.Equals(0) == false)
                    {
                        if (expressionList?.ContainsKey(info.Name) == true)
                        {
                            var selectExpr = expressionList[info.Name];
                            var query = GetQueryForType(info.PropertyType, value);
                            var list = await query.Select(selectExpr).ToDynamicListAsync();
                            var result = list.SingleOrDefault();
                            var foreignKeyValue = Activator.CreateInstance(info.PropertyType);
                            foreach (PropertyInfo info1 in result.GetType().GetProperties())
                                if (info1.IsCollectible)
                                    IQueryableExtension.UpdateEntity(foreignKeyValue, info1.Name, info1.GetValue(result));
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

        internal void ClearSource()
        {
            source.Clear();
            Total = 0;
            items.Clear();
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
    }
}
