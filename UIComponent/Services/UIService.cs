using Caspian.Common;
using System.Reflection;
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
    public class UIService<TEntity>: IUIService<TEntity> where TEntity : class
    {
        protected IJSRuntime jSRuntime;
        BaseComponentService baseComponentService;
        BasePageService basePageService;
        IDictionary<string, SearchType> searchData;

        public IServiceProvider ServiceProvider { get; private set; }

        public IDictionary<string, SearchType> GetSearchData()
        {
            return searchData;
        }

        public void OnlyForSearch()
        {

        }

        public void Dispose()
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

        public UIService(IServiceProvider serviceProvider) 
        {
            baseComponentService = serviceProvider.GetService<BaseComponentService>();
            basePageService = serviceProvider.GetService<BasePageService>();
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
                var isAdd = false   ;
                if (Is1To1RelationshipService)
                {
                    service.DetailType = DetailType != typeof(TEntity) ? DetailType : null;
                    var pKeyName = typeof(TEntity).GetPrimaryKey().Name;
                    if (typeof(TEntity).GetProperties().Any(t => t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == pKeyName))
                    {
                        var old = await service.SingleOrDefaultAsync(id);
                        isAdd = old == null;
                    }
                }
                if (id == 0 || isAdd)
                {
                    await service.AddAsync(UpsertData);
                    message = "Registration was done successfully";
                }
                else
                {
                    await service.UpdateAsync(UpsertData);
                    message = "Updating was done successfully";
                }
                await service.SaveChangesAsync();
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
                throw new CaspianException("Caspian Exception: You must inherits from BasePage or configure page manioaly");
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
            DataView.ShowInsertIcon = DataView.ShowInsertIcon ?? true;
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
                        var pkeyName = type.GetPrimaryKey().Name;
                        if (type.GetProperties().Single(t => t.PropertyType == typeof(TEntity) && t.GetCustomAttribute<ForeignKeyAttribute>()?.Name == pkeyName) != null)
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
                throw new CaspianException("لطفا صفحه ی پایه را به این صفحه اضافه کنید");
            var window = basePageService.Peek();
            if (window !=  null) 
                return await window.GetMessageBox().Confirm(message);
            return await baseComponentService.MessageBox.Confirm(message);
        }

        public void WindowInitialize()
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
    }
}
