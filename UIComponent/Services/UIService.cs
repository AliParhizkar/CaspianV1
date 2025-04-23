using Caspian.Common;
using System.Reflection;
using System.Collections;
using Microsoft.JSInterop;
using Caspian.Engine.Model;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations.Schema;

namespace Caspian.UI
{
    public class UIService<TEntity>: IUIService<TEntity>, IInternalUIService where TEntity : class
    {
        protected IJSRuntime jSRuntime;
        BaseComponentService baseComponentService;
        BasePageService basePageService;
        CaspianDataService CaspianDataService;
        IDictionary<string, SearchType> searchData;
        IDictionary<string, ICollection> enumValues;
        protected bool hideFooter;

        public IServiceProvider ServiceProvider { get; private set; }

        public IDictionary<string, SearchType> GetSearchData()
        {
            return searchData;
        }

        public async Task CloseWindow()
        {
            if (Window != null) 
                await Window.Close();
        }

        public IEnumSearch<TValue> GetEnumField<TValue>(Expression<Func<TEntity, TValue>> lambda) where TValue : Enum
        {
            return new EnumSearch<TValue>(lambda.Body, enumValues);
        }

        /// <summary>
        /// Open Window to validate and upsert the entity. In this case window has form that bind to entity
        /// </summary>
        /// <param name="id">key of entity</param>
        /// <returns>Task</returns>
        public async Task OpenWindow(int id)
        {
            if (id == 0)
                UpsertData = Activator.CreateInstance<TEntity>();
            else
            {
                using var service = CreateScope().GetService<IBaseService<TEntity>>();
                UpsertData = await service.SingleAsync(id);
            }
            await Window.Open();
            StateHasChanged();
            await Task.Delay(100);
            if (Form != null)
                await Form.FocusAsync();
        }

        public void HideFooter()
        {
            hideFooter = true;
        }

        public void OnlyForSearch()
        {
            HideInsertIcon = true;
        }

        void IInternalUIService.Dispose()
        {
            DetailType = default;
            Is1To1RelationshipService = default;
            MasterId = default;
            Window = default;
            EntityTabPanel = default;
            DataView = default;
            Form = default;
            Search = Activator.CreateInstance<TEntity>();
            UpsertData = Activator.CreateInstance<TEntity>();
            OnUpsert = default;
        }

        public void SetSearchType(IDictionary<string, SearchType> types)
        {
            searchData = types;
        }

        public Type DetailType { get; private set; }

        internal int UserId { get; private set; }

        public bool Is1To1RelationshipService { get; private set; }

        public int MasterId { get; set; }

        internal Type MasterType { get; set; }

        public async Task UpdateChildOfModelAsync(Type childType)
        {
            DetailType = childType;
            if (childType != typeof(TEntity) && UpsertData != null)
            {
                var info = typeof(TEntity).GetProperties().Single(t => t.PropertyType == childType);
                var detail = info.GetValue(UpsertData);
                if (detail == null)
                {
                    if (MasterId > 0)
                    {
                        using var service = CreateScope().GetService<IBaseService<TEntity>>();
                        var old = await service.GetAll().Include(info.Name).SingleAsync(MasterId);
                        detail = info.GetValue(old);
                    }
                    if (detail == null)
                        detail = Activator.CreateInstance(childType);
                }
                info.SetValue(UpsertData, detail);
            }
        }

        public void SetEnumFields(IDictionary<string, ICollection> enumFields)
        {
            this.enumValues = enumFields;
        }

        public UIService(IServiceProvider serviceProvider) 
        {
            baseComponentService = serviceProvider.GetService<BaseComponentService>();
            basePageService = serviceProvider.GetService<BasePageService>();
            CaspianDataService = serviceProvider.GetService<CaspianDataService>();
            jSRuntime = serviceProvider.GetService<IJSRuntime>();
            this.ServiceProvider = serviceProvider;
            Search = Activator.CreateInstance<TEntity>();
            UpsertData = Activator.CreateInstance<TEntity>();
        }

        public void ChildTabPanelItemInitialize(Type detailType)
        {
            this.DetailType = detailType;
        }

        public Window Window { get; set; }

        public IEntityTabPanel EntityTabPanel { get; set; }

        public DataView<TEntity> DataView { get; set; }

        protected bool HideInsertIcon { get; set; }

        public CaspianForm<TEntity> Form { get; set; }

        public TEntity UpsertData { get; private set; }

        public TEntity Search { get; private set; }

        public async Task FetchAsync()
        {
            if (MasterId > 0 && MasterType == null)
            {
                using var service = CreateScope().GetService<IBaseService<TEntity>>();
                var old = await service.SingleOrDefaultAsync(MasterId);
                if (old != null)
                    UpsertData.CopyEntity(old);
            }
        }

        public void TabPanelInitialize()
        {
            Is1To1RelationshipService = true;
        }

        public Func<IServiceProvider, TEntity, Task<bool>> OnUpsert { get; set; }

        protected Action<TEntity> OnFormSubmit { get; set; }

        protected Action<TEntity> OnFormValidSubmit { get; set; }

        protected Action<TEntity> OnAfterDelete { get; set; }

        public void FormInitialize()
        {
            UserId = ServiceProvider.GetService<CaspianDataService>().UserId;
            if (UpsertData == null)
                UpsertData = Activator.CreateInstance<TEntity>();
            Form.Model = UpsertData;
            Form.OnInternalReset = EventCallback.Factory.Create(this, async () =>
            {
                if (Window != null)
                    await Window?.Close();
                StateHasChanged();
            });
            Form.OnInternalSubmit = EventCallback.Factory.Create<TEntity>(this, entity =>
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.UpsertUserId = UserId;
                    baseEntity.UpsertDate = DateTime.Now;
                }
                OnFormSubmit?.Invoke(entity);
            });
            Form.OnInternalValidSubmit = EventCallback.Factory.Create<TEntity>(this, async entity =>
            {
                var result = true;
                var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(entity));
                using var scope = CreateScope();
                scope.SetUserId(UserId);
                var service = scope.GetService<IBaseService<TEntity>>();
                if (OnUpsert != null)
                    result = await OnUpsert.Invoke(scope.ServiceProvider, UpsertData);
                if (!result)
                    return;

                service.DetailType = DetailType;
                string message = null;
                var isAdd = false;
                IList<PropertyInfo> infos = null;
                TEntity tempEntity = default;
                if (Is1To1RelationshipService)
                {
                    infos = typeof(TEntity).GetOneToOnePropertyInfos();
                    service.DetailType = DetailType != typeof(TEntity) ? DetailType : null;
                    tempEntity = Activator.CreateInstance<TEntity>();
                    foreach (var info in infos)
                    {
                        var value = info.GetValue(entity);
                        info.SetValue(tempEntity, value);
                        if (info.PropertyType != DetailType)
                            info.SetValue(entity, null);
                    }
                }
                if (id == 0 || isAdd)
                {
                    await service.AddAsync(UpsertData);
                    if (CaspianDataService.Language == Language.En)
                        message = "Registration was done successfully";
                    else
                        message = "ثبت با موفقیت انجام شد.";
                }
                else
                {
                    await service.UpdateAsync(UpsertData);
                    if (CaspianDataService.Language == Language.En)
                        message = "Updating was done successfully";
                    else
                        message = "بروزرسانی با موفقیت انجام شد";
                }
                await service.SaveChangesAsync();
                if (infos != null)
                {
                    foreach (var info in infos)
                    {
                        var value = info.GetValue(tempEntity);
                        info.SetValue(entity, value);
                    }
                }
                OnFormValidSubmit?.Invoke(entity);

                if (DataView != null)
                {
                    if (id == 0)
                    {
                        id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(UpsertData));
                            await DataView.SelectRowById(id);
                        if (Window == null)
                            StateHasChanged();
                        if (MasterType != null)
                            DataView.ChangeState();
                    }
                    else
                        await DataView.ReloadAsync();
                }
                await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", message);
                if (Is1To1RelationshipService)
                    EntityTabPanel.ChangeState();
                else
                {
                    if (Window == null)
                        await Form.ResetAsync();
                    else
                        await Window.Close();
                }
            });
        }

        public void StateHasChanged()
        {
            var page = baseComponentService.Target as BasePage;
            if (page == null)
                throw new CaspianException("You must inherits from BasePage and add this code to page: base.BuildRenderTree(__builder);");
            page.ChangeState();
        }

        protected IServiceScope CreateScope()
        {
            var scope = ServiceProvider.CreateScope();
            UserId = ServiceProvider.GetService<CaspianDataService>().UserId;
            scope.SetUserId(UserId);
            return scope;
        }

        public void DataViewInitialize()
        {
            DataView.Search = Search;
            DataView.ShowInsertIcon = DataView.ShowInsertIcon ?? !HideInsertIcon;
            DataView.HideFooter = DataView.HideFooter ?? hideFooter;
            if (MasterType != null && MasterId > 0)
            {
                var param = Expression.Parameter(typeof(TEntity), "t");
                var foreignKey = typeof(TEntity).GetForeignKey(MasterType);
                Expression expr = Expression.Property(param, foreignKey);
                var foreignKeyType = foreignKey.PropertyType;

                if (foreignKey.PropertyType.IsNullableType())
                {
                    expr = Expression.Property(expr, "Value");
                    foreignKeyType = foreignKeyType.GetUnderlyingType();
                }
                expr = Expression.Equal(expr, Expression.Constant(Convert.ChangeType(MasterId, foreignKeyType)));
                DataView.InternalConditionExpr = expr;
            }
            DataView.OnInternalUpsert = EventCallback.Factory.Create<TEntity>(this, async entity =>
            {
                var value = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(entity));
                if (MasterType == null)
                    MasterId = value;
                if (value != 0)
                {
                    using var service = CreateScope().GetService<BaseService<TEntity>>();
                    UpsertData = await service.GetAll().SingleAsync(value);
                }
                else
                {
                    UpsertData = Activator.CreateInstance<TEntity>();
                    if (MasterType != null)
                    {
                        var foreignKey = typeof(TEntity).GetForeignKey(MasterType);
                        foreignKey.SetValue(UpsertData, MasterId);
                    }
                }
                if (Form != null)
                    Form.Model = UpsertData;
                if (Window != null)
                    await Window.Open();
                StateHasChanged();
                await Task.Delay(100);
                if (Form != null)
                    await Form.FocusAsync();
            });

            DataView.OnInternalDelete = EventCallback.Factory.Create<TEntity>(this, async entity =>
            {
                using var scope = CreateScope();
                scope.SetUserId(UserId);
                var service = scope.GetService<IBaseService<TEntity>>();
                var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(entity));
                ///For 1to1 relationship we should include all 1to1 relationship to cascade remove 
                var list = new List<string>();
                /// find relationship and add them to list
                foreach (var info in typeof(TEntity).GetProperties())
                {
                    var type = info.PropertyType.GetUnderlyingType();
                    if (!type.IsValueType && type != typeof(string) && type != typeof(byte[]) && info.GetCustomAttribute<ForeignKeyAttribute>() == null && !type.IsEnumerableType())
                    {
                        var pKeyName = type.GetPrimaryKey().Name;
                        if (type.GetProperties().Single(t => t.PropertyType == typeof(TEntity) && t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == pKeyName) != null)
                            list.Add(info.Name);
                    }
                }
                TEntity old = null;
                if (list.Count > 0)
                {
                    ///Include all relationship 
                    var query = service.GetAll();
                    foreach (var item in list)
                        query = query.Include(item);
                    old = await query.SingleAsync(id);
                }
                else
                    old = await service.SingleAsync(id);
                    
                var result = await service.ValidateRemoveAsync(old);
                if (result.IsValid)
                {
                    if (!DataView.DeleteMessage.HasValue() || await Confirm(DataView.DeleteMessage))
                    {
                        await service.RemoveAsync(old);
                        await service.SaveChangesAsync();
                        await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", "حذف با موفقیت انجام شد.");
                        await DataView.ReloadAsync();
                        OnAfterDelete?.Invoke(old);
                    }
                }
                else
                    await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", result.Errors[0].ErrorMessage);
            });
        }

        async Task<bool> Confirm(string message)
        {
            if (baseComponentService.MessageBox == null)
                throw new CaspianException("You must inherits from BasePage and add this code to page: base.BuildRenderTree(__builder);");
            var window = basePageService.Peek();
            if (window !=  null) 
                return await window.GetMessageBox().Confirm(message);
            return await baseComponentService.MessageBox.Confirm(message);
        }

        void IInternalUIService.WindowInitialize()
        {
            Window.OnInternalClose = EventCallback.Factory.Create(this, StateHasChanged);
            Window.OnInternalOpen = EventCallback.Factory.Create(this, async () => 
            {
                await Task.Delay(100);
                await Form.FocusAsync();
            });
        }

        public void ClearForm()
        {
            Form = null;
            if (!Is1To1RelationshipService)
                UpsertData = null;
        }

        IDictionary<string, ICollection> ISearchService<TEntity>.GetEnumFields()
        {
            return enumValues;
        }
    }

    public class EnumSearch<TValue>:IEnumSearch<TValue> where TValue:Enum
    {
        string propertyPath;
        IDictionary<string, ICollection> dictionary;
        public EnumSearch(Expression expression, IDictionary<string, ICollection> dictionary)
        {
            this.dictionary = dictionary;
            var propertyPath = "";
            var expr = expression;
            while (expr.NodeType == ExpressionType.MemberAccess)
            {
                var memberExpr = expr as MemberExpression;
                if (propertyPath.HasValue())
                    propertyPath = $".{propertyPath}";
                if (!memberExpr.Member.DeclaringType.IsNullableType())
                    propertyPath += memberExpr.Member.Name;
                expr = memberExpr.Expression;
            }
            this.propertyPath = propertyPath;
        }

        public void SetValues(params TValue[] values)
        {
            if (dictionary == null || !dictionary.ContainsKey(propertyPath))
            {
                if (values != null && values.Length > 0) 
                    dictionary.Add(propertyPath, values);
            }
            else
            {
                if (values == null || values.Length == 0)
                    dictionary.Remove(propertyPath);
                else
                    dictionary[propertyPath] = values;
            }
        }
    }
}
