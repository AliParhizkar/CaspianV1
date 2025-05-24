using System.Data;
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
    public class UIService<TEntity>: IInternalUIService<TEntity> where TEntity : class
    {
        protected IJSRuntime jSRuntime;
        protected BaseComponentService baseComponentService;
        protected BasePageService basePageService;
        protected CaspianDataService CaspianDataService;
        IDictionary<string, SearchType> searchData;
        IDictionary<string, ICollection> enumValues;
        protected bool hideFooter;

        public UIService(IServiceProvider serviceProvider)
        {
            baseComponentService = serviceProvider.GetService<BaseComponentService>();
            basePageService = serviceProvider.GetService<BasePageService>();
            CaspianDataService = serviceProvider.GetService<CaspianDataService>();
            jSRuntime = serviceProvider.GetService<IJSRuntime>();
            this.ServiceProvider = serviceProvider;
            Search = Activator.CreateInstance<TEntity>();
            UpsertData = Activator.CreateInstance<TEntity>();
            UserId = ServiceProvider.GetService<CaspianDataService>().UserId;
            if (UpsertData is BaseEntity baseEntity)
                baseEntity.UpsertUserId = UserId;
        }

        Type IInternalUIService<TEntity>.OtherType { get; set; }

        internal int UserId { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; }

        public int MasterId { get; set; }

        internal Type MasterType { get; set; }

        public Window Window { get; private set; }

        public bool Is1To1RelationshipService { get; private set; }

        public IEntityTabPanel EntityTabPanel { get; private set; }

        public DataView<TEntity> DataView { get; private set; }

        protected bool HideInsertIcon { get; set; }

        public CaspianForm<TEntity> Form { get; private set; }

        public TEntity UpsertData { get; protected set; }

        public TEntity Search { get; protected set; }

        public Action<TEntity> OnCreate { get; set; }

        public Func<IServiceProvider, TEntity, Task<bool>> OnUpsert { get; set; }

        internal IBaseService<TEntity> BaseService { get; private set; }

        IDictionary<string, SearchType> IInternalSearchService<TEntity>.GetSearchData()
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

        #region Methods for override on subclass 


        /// <summary>
        /// This Method Execute after form valid-submit(form submitted and data is valid). 
        /// It call Insert-Update methods of Validator-Service(BaseService)) to upsert and then initialize Data & Components (UpsertData, Form, DataView, ...)
        /// </summary>
        protected virtual async Task InitializeAfterValidate(TEntity entity)
        {
            var result = true;
            var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(entity));
            using var scope = CreateScope();
            scope.SetUserId(UserId);
            if (OnUpsert != null)
                result = await OnUpsert.Invoke(scope.ServiceProvider, UpsertData);
            if (!result)
                return;
            var service = scope.GetService<IBaseService<TEntity>>();
            (service as BaseService<TEntity>).CheckValidation = false;
            service.OtherTypeIn1To1Relationship = (this as IInternalUIService<TEntity>).OtherType;
            string message = null;
            var isAdd = false;
            IList<PropertyInfo> infos = null;
            TEntity tempEntity = default;
            if (Is1To1RelationshipService)
            {
                infos = typeof(TEntity).GetOneToOnePropertyInfos();
                service.OtherTypeIn1To1Relationship = (this as IInternalUIService<TEntity>).OtherType != typeof(TEntity) ? (this as IInternalUIService<TEntity>).OtherType : null;
                tempEntity = Activator.CreateInstance<TEntity>();
                foreach (var info in infos)
                {
                    var value = info.GetValue(entity);
                    info.SetValue(tempEntity, value);
                    if (info.PropertyType != (this as IInternalUIService<TEntity>).OtherType)
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
            UpsertData.ClearEntityProperties();

            if (infos != null)
            {
                foreach (var info in infos)
                {
                    var value = info.GetValue(tempEntity);
                    info.SetValue(entity, value);
                }
            }
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
                {
                    if (OnCreate != null)
                        OnCreate(UpsertData);
                    StateHasChanged();
                }
                else
                    await Window.Close();
            }
        }

        protected virtual async Task InitializeAfterRemove(TEntity entity)
        {
            await DataView.ReloadAsync();
        }
        #endregion

        #region Methods for Initialize Components. This Methods Call from components(DataView, Form, TypeWindow, ...) For intialize
        public void ChildTabPanelItemInitialize(Type detailType) => (this as IInternalUIService<TEntity>).OtherType = detailType;
        
        void IInternalUIService<TEntity>.TabPanelInitialize(IEntityTabPanel tabPanel)
        {
            EntityTabPanel = tabPanel;
            Is1To1RelationshipService = true;
        }

        void IInternalUIService<TEntity>.FormInitialize(CaspianForm<TEntity> form)
        {
            Form = form;

            if (UpsertData == null)
            {
                UpsertData = Activator.CreateInstance<TEntity>();
                if (UpsertData is BaseEntity baseEntity)
                    baseEntity.UpsertUserId = UserId;

            }
            if (OnCreate != null)
                OnCreate(UpsertData);
            Form.Model = UpsertData;
            Form.OnInternalReset = EventCallback.Factory.Create(this, async () =>
            {
                if (Window != null)
                    await Window?.Close();
                StateHasChanged();
            });
            Form.OnInternalSubmit = EventCallback.Factory.Create<TEntity>(this, async entity =>
            {
                //await InitializeBeforeValidate(entity);
            });
            Form.OnInternalValidSubmit = EventCallback.Factory.Create<TEntity>(this, InitializeAfterValidate);
        }

        void IInternalSearchService<TEntity>.DataViewInitialize(DataView<TEntity> dataView)
        {
            DataView = dataView;
            if (DataView == null)
                return;
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
                    if (UpsertData is BaseEntity baseEntity)
                        baseEntity.UpsertUserId = UserId;
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
                        service.Remove(old);
                        await service.SaveChangesAsync();
                        await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", "حذف با موفقیت انجام شد.");
                        await InitializeAfterRemove(old);
                    }
                }
                else
                    await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", result.Errors[0].ErrorMessage);
            });
        }

        void IInternalUIService.WindowInitialize(Window window)
        {
            Window = window;
            if (window != null)
            {
                Window.OnInternalClose = EventCallback.Factory.Create(this, StateHasChanged);
                Window.OnInternalOpen = EventCallback.Factory.Create(this, async () =>
                {
                    await Task.Delay(100);
                    await Form.FocusAsync();
                });
            }
        }
        #endregion

        /// <summary>
        /// Open Window to validate and upsert the entity. In this case window has form that bind to entity
        /// </summary>
        /// <param name="id">key of entity</param>
        /// <returns>Task</returns>
        public async Task OpenWindow(int? id)
        {
            if (id == null)
            {
                UpsertData = Activator.CreateInstance<TEntity>();
                if (UpsertData is BaseEntity baseEntity)
                    baseEntity.UpsertUserId = UserId;
            }
            else
            {
                using var service = CreateScope().GetService<IBaseService<TEntity>>();
                UpsertData = await service.SingleAsync(id.Value);
            }
            await Window.Open();
            StateHasChanged();
            await Task.Delay(100);
            if (Form != null)
                await Form.FocusAsync();
        }

        /// <summary>
        /// This Method Call form Lookup-window (In lookup-window we hide footer of Data-View)
        /// </summary>
        void IInternalSearchService<TEntity>.HideFooter() => hideFooter = true;

        /// <summary>
        /// This Method Call form Lookup-window (In lookup-window we Hide 
        /// </summary>
        void IInternalSearchService<TEntity>.OnlyForSearch() => HideInsertIcon = true;

        void IInternalUIService.Dispose()
        {
            (this as IInternalUIService<TEntity>).OtherType = default;
            Is1To1RelationshipService = default;
            MasterId = default;
            Window = default;
            EntityTabPanel = default;
            DataView = default;
            Form = default;
            Search = Activator.CreateInstance<TEntity>();
            UpsertData = Activator.CreateInstance<TEntity>();
            if (UpsertData is BaseEntity baseEntity)
                baseEntity.UpsertUserId = UserId;
            OnUpsert = default;
        }

        void IInternalSearchService<TEntity>.SetSearchType(IDictionary<string, SearchType> types) => searchData = types;

        /// <summary>
        /// This Method is used In One-To-One relationship to Fetch Other-Entity. 
        /// For Example fetch Address in Employee-Address relationship
        /// </summary>
        async Task IInternalUIService<TEntity>.UpdateChildOfModelAsync(Type childType)
        {
            (this as IInternalUIService<TEntity>).OtherType = childType;
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

        /// <summary>
        /// 
        /// </summary>
        void IInternalSearchService<TEntity>.SetEnumFields(IDictionary<string, ICollection> enumFields) => enumValues = enumFields;

        async Task IInternalUIService<TEntity>.FetchAsync()
        {
            if (MasterId > 0 && MasterType == null)
            {
                using var service = CreateScope().GetService<IBaseService<TEntity>>();
                var old = await service.SingleOrDefaultAsync(MasterId);
                if (old != null)
                    UpsertData.CopyEntity(old);
            }
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

        async Task<bool> Confirm(string message)
        {
            if (baseComponentService.MessageBox == null)
                throw new CaspianException("You must inherits from BasePage and add this code to page: base.BuildRenderTree(__builder);");
            var window = basePageService.Peek();
            if (window !=  null) 
                return await window.GetMessageBox().Confirm(message);
            return await baseComponentService.MessageBox.Confirm(message);
        }

        void IInternalUIService<TEntity>.ClearForm()
        {
            Form = null;
            if (!Is1To1RelationshipService)
                UpsertData = null;
        }

        IDictionary<string, ICollection> IInternalSearchService<TEntity>.GetEnumFields() => enumValues;

        /// <summary>
        /// This Method is used for Initialize Validator-Service. It can be override to initialize service in child class
        /// </summary>
        protected virtual async Task InitializeValidatorService(IBaseService<TEntity> service)
        {
            var service1 = service as BaseService<TEntity>;
            service1.MasterId = MasterId;
            service1.MasterType = typeof(TEntity);
        }

        void IUIService<TEntity>.CaspianValidationValidatorInitialize(CaspianValidationValidator<TEntity> validator)
        {
            validator.OnInternalValidate = EventCallback.Factory.Create<IBaseService<TEntity>>(this, InitializeValidatorService);
        }
    }
}
