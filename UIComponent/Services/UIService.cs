using System.Data;
using Caspian.Common;
using System.Collections;
using Microsoft.JSInterop;
using Caspian.Engine.Model;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public class UIService<TEntity>: IInternalUIService<TEntity> where TEntity : class
    {
        protected IJSRuntime jSRuntime;
        protected BaseComponentService baseComponentService;
        protected BasePageService basePageService;
        protected CaspianDataService CaspianDataService;
        protected bool hideFooter, isDevelopment;
        protected ILogger logger;
        ILookup<TEntity> lookup;

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
            logger = serviceProvider.GetService<ILogger>();
            if (UpsertData is BaseEntity baseEntity)
                baseEntity.UpsertUserId = UserId;
        }

        public IEnumSearch<TValue> GetEnumField<TValue>(Expression<Func<TEntity, TValue>> lambda) where TValue : Enum
        {
            return new EnumSearch<TValue>(lambda.Body, (this as IInternalSearchService<TEntity>).EnumFields);
        }

        int IInternalUIService.InternalMasterId { get; set; }

        Type IInternalUIService.InternalMasterType { get; set; }

        Type IInternalUIService.OtherType { get; set; }

        IList<ValueTypeContainer> IInternalSearchService<TEntity>.ValueTypes { get; set; }

        internal int UserId { get; private set; }

        public IServiceProvider ServiceProvider { get; private set; }

        public int MasterId { get; set; }

        public Window Window { get; private set; }

        internal bool Is1To1RelationshipService { get; private set; }

        public IEntityTabPanel EntityTabPanel { get; private set; }

        public DataView<TEntity> DataView { get; private set; }

        protected bool HideInsertIcon { get; set; }

        public CaspianForm<TEntity> Form { get; private set; }

        public TEntity UpsertData { get; protected set; }

        public TEntity Search { get; protected set; }

        public Action<TEntity> OnCreate { get; set; }

        public Func<IServiceProvider, TEntity, Task<bool>> OnUpsert { get; set; }

        /// <summary>
        /// This event Fired After data saving 
        /// </summary>
        public Func<IServiceProvider, TEntity, Task> OnAfterUpsertAsync { get; set; }

        internal IBaseService<TEntity> BaseService { get; private set; }

        IDictionary<string, SearchType> IInternalSearchService<TEntity>.SearchData { get; set; }

        async Task IInternalSearchService<TEntity>.SelectItemOnLookup()
        {
            await lookup.SelectOnLookup(true);
        }

        public async Task CloseWindow()
        {
            if (Window != null) 
                await Window.Close();
        }

        #region Methods for override on subclass 

        /// <summary>
        /// This Method create Validator & CRUD Service. it's can be override and create Master-Details service in sub class
        /// </summary>
        /// <returns>Service is Used for validation and CRUD</returns>
        protected virtual IBaseService<TEntity> CreateService(IServiceScope scope)
        {
            return scope.GetService<IBaseService<TEntity>>();
        }

        protected virtual TEntity InitializeBeforeUpsert(IBaseService<TEntity> service)
        {
            ///Before Calling this method Validation is done. an we don't need to do it(validation) again 
            (service as BaseService<TEntity>).CheckValidation = false;
            service.OtherTypeIn1To1Relationship = (this as IInternalUIService<TEntity>).OtherType;
            /// In One-To-One Relationship Before Upsert maybe we have many One-To-One Relationship data, But we save only one of them.
            /// We clear other One-To-One relationship data and keep this data in tempEntity object to use it to reset data after Upsert
            TEntity tempEntity = default;
            if (Is1To1RelationshipService)
            {
                var properties = typeof(TEntity).GetOneToOneProperties();
                service.OtherTypeIn1To1Relationship = (this as IInternalUIService<TEntity>).OtherType != typeof(TEntity) ? (this as IInternalUIService<TEntity>).OtherType : null;
                tempEntity = Activator.CreateInstance<TEntity>();
                foreach (var property in properties)
                {
                    var value = property.GetValue(UpsertData);
                    property.SetValue(tempEntity, value);
                    if (property.PropertyType != (this as IInternalUIService<TEntity>).OtherType)
                        property.SetValue(UpsertData, null);
                }
            }
            return tempEntity;
        }

        protected virtual string GetUpsertMessage(UpsertMode upsertMode)
        {
            if (upsertMode == UpsertMode.Insert)
                return CaspianDataService.Language == Language.En ? "Registration was done successfully" : "ثبت با موفقیت انجام شد.";
            return CaspianDataService.Language == Language.En ? "Updating was done successfully" : "بروزرسانی با موفقیت انجام شد";

        }

        protected virtual async Task InitializeAfterUpsert(TEntity tempEntity, UpsertMode upsertMode)
        {
            if (Is1To1RelationshipService)
            {
                var value1 = typeof(TEntity).GetPrimaryKey().GetValue(UpsertData);
                MasterId = Convert.ToInt32(value1);
                var properties = typeof(TEntity).GetOneToOneProperties();
                if (properties != null)
                {
                    foreach (var info in properties)
                    {
                        var value = info.GetValue(tempEntity);
                        info.SetValue(UpsertData, value);
                    }
                }
            }
            if (DataView != null)
            {
                if (upsertMode == UpsertMode.Insert)
                {
                    var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(UpsertData));
                    await DataView.SelectRowById(id);
                    if ((this as IInternalUIService).InternalMasterType != null)
                        DataView.ChangeState();
                    if (Window == null)
                        StateHasChanged();
                }
                else
                    await DataView.ReloadAsync();
            }
            if (Is1To1RelationshipService)
               EntityTabPanel.ChangeState();
            else
            {
                UpsertData = Activator.CreateInstance<TEntity>();
                Form.SetModel(UpsertData);
                if (UpsertData is BaseEntity baseEntity)
                    baseEntity.UpsertUserId = UserId;
                if (Window == null)
                {
                    if (OnCreate != null)
                        OnCreate(UpsertData);
                    StateHasChanged();
                    await Form.FocusAsync();
                }
                else
                    await Window.Close();
            }
            var message = GetUpsertMessage(upsertMode);
            if (message != null)
                await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", message);
        }

        /// <summary>
        /// This Method Execute on form submit(form submitted and data is valid). 
        /// We Use this method to Initialize entity before of validate 
        /// Note: This method override in sub class **DON'T CLEAR IT**
        /// </summary>
        protected virtual void InitializeBeforeValidation(TEntity entity)
        {
            var service = this as IInternalUIService;
            if (service.InternalMasterType != null)
            {
                var info = typeof(TEntity).GetForeignKey(service.InternalMasterType);
                info.SetValue(entity, Convert.ChangeType(service.InternalMasterId, info.PropertyType.GetUnderlyingType()));
            }
        }

        /// <summary>
        /// This Method Execute after form valid-submit(form submitted and data is valid). 
        /// It call Insert-Update methods of Validator-Service(BaseService)) to upsert and then initialize Data & Components (UpsertData, Form, DataView, ...)
        /// </summary>
        protected virtual async Task UpsertAndInitializeAfterValidate(TEntity entity)
        {
            var result = true;
            var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(UpsertData));
            using var scope = CreateScope();
            scope.SetUserId(UserId);
            if (OnUpsert != null)
                result = await OnUpsert.Invoke(scope.ServiceProvider, UpsertData);
            if (!result)
                return;
            var service = CreateService(scope);
            var tempEntity = InitializeBeforeUpsert(service);
           
            if (id == 0)
                await service.AddAsync(UpsertData);
            else
                await service.UpdateAsync(UpsertData);
            await service.SaveChangesAsync();
            if (OnAfterUpsertAsync != null)
                await OnAfterUpsertAsync(scope.ServiceProvider, UpsertData);
            if (service.Context?.Database?.CurrentTransaction != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Warning: Transaction Rollbacked by Caspian infrastructure");
                Console.ResetColor();
                service.Context.Database.CurrentTransaction.Rollback();
            }
            await InitializeAfterUpsert(tempEntity, id == 0 ? UpsertMode.Insert : UpsertMode.Edit);
        }

        protected virtual async Task InitializeAfterRemove(TEntity entity)
        {
            await DataView.ReloadAsync();
        }

        /// <summary>
        /// This Method is used for Initialize Validator-Service. It can be override to initialize service in child class
        /// </summary>
        protected virtual Task InitializeValidatorService(IBaseService<TEntity> service)
        {
            (service as BaseService<TEntity>).SetBatchServiceData(MasterId, typeof(TEntity));
            return Task.CompletedTask;
        }

        protected virtual void DataViewInitializer()
        {
            DataView.Search = Search;
            
            DataView.ShowInsertIcon = DataView.ShowInsertIcon ?? !HideInsertIcon;
            DataView.HideFooter = DataView.HideFooter ?? hideFooter;
            
            DataView.OnInternalUpsert = EventCallback.Factory.Create<TEntity>(this, async entity =>
            {
                var value = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(entity));
                if (value != 0)
                {
                    using var service = CreateScope().GetService<BaseService<TEntity>>();
                    UpsertData = await service.SingleAsync(value);
                }
                else
                {
                    UpsertData = Activator.CreateInstance<TEntity>();
                    var otherType = (this as IInternalUIService).OtherType;
                    if (otherType != null)
                    {
                        var info = typeof(TEntity).GetForeignKey(otherType);
                        info.SetValue(UpsertData, MasterId);
                    }
                    if (UpsertData is BaseEntity baseEntity)
                        baseEntity.UpsertUserId = UserId;
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
                var service = CreateService(scope);
                var id = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(entity));
                var old = await service.SingleAsync(id);
                var result = await service.ValidateRemoveAsync(old);
                if (result.IsValid)
                {
                    if (!DataView.DeleteMessage.HasValue() || await Confirm(DataView.DeleteMessage))
                    {
                        await service.RemoveAsync(id);
                        await service.SaveChangesAsync();
                        await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", "حذف با موفقیت انجام شد.");
                        await InitializeAfterRemove(old);
                    }
                }
                else
                    await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", result.Errors[0].ErrorMessage);
            });
        }

        #endregion

        #region Methods for Initialize Components. This Methods Call from components(DataView, Form, TypeWindow, ...) For intialize
        public void ChildTabPanelItemInitialize(Type detailType) => (this as IInternalUIService<TEntity>).OtherType = detailType;
        
        void IInternalUIService<TEntity>.TabPanelInitializer(IEntityTabPanel tabPanel)
        {
            EntityTabPanel = tabPanel;
            Is1To1RelationshipService = true;
        }

        void IInternalUIService<TEntity>.FormInitializer(CaspianForm<TEntity> form)
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
            Form.OnInternalSubmit = EventCallback.Factory.Create<TEntity>(this, InitializeBeforeValidation);
            Form.OnInternalValidSubmit = EventCallback.Factory.Create<TEntity>(this, UpsertAndInitializeAfterValidate);
        }

        void IInternalSearchService<TEntity>.DataViewInitializer(DataView<TEntity> dataView)
        {
            DataView = dataView;
            if (DataView == null)
                return;
            DataViewInitializer();
        }

        void IInternalUIService.WindowInitializer(Window window)
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

        void IUIService<TEntity>.CaspianValidationValidatorInitializer(CaspianValidationValidator<TEntity> validator)
        {
            validator.OnInternalValidate = EventCallback.Factory.Create<IBaseService<TEntity>>(this, InitializeValidatorService);
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
        void IInternalSearchService<TEntity>.LookupInitializer(ILookup<TEntity> lookup)
        {
            this.lookup = lookup;
        }

        bool IInternalSearchService<TEntity>.IsLookup()
        {
            return lookup != null;
        }


        /// <summary>
        /// This Method Call form Lookup-window (In lookup-window we Hide Insert-Icon)
        /// </summary>
        void IInternalSearchService<TEntity>.OnlyForSearch() => HideInsertIcon = true;

        /// <summary>
        /// This Method is used In One-To-One relationship to Fetch Other-Entity. 
        /// For Example fetch Address in Employee-Address relationship
        /// </summary>
        async Task IInternalUIService<TEntity>.UpdateChildOfModelAsync(Type childType)
        {
            (this as IInternalUIService<TEntity>).OtherType = childType;
            
            if (childType != typeof(TEntity) && UpsertData != null && !childType.IsCollectionType()) 
            {
                MasterId = Convert.ToInt32(typeof(TEntity).GetPrimaryKey().GetValue(UpsertData));
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

        async Task IInternalUIService<TEntity>.FetchAsync()
        {
            if (MasterId > 0)
            {
                using var service = CreateScope().GetService<IBaseService<TEntity>>();
                if (service == null)
                    throw new CaspianException($"Service of type IBaseService<{typeof(TEntity)} not 'Implemented' and 'Injected'");
                var old = await service.SingleOrDefaultAsync(MasterId);
                var pKey = typeof(TEntity).GetPrimaryKey();
                if (old != null)
                {
                    UpsertData.CopyEntity(old);
                    if (pKey.PropertyType == typeof(int))
                        pKey.SetValue(UpsertData, MasterId);
                    else
                        pKey.SetValue(UpsertData, Convert.ChangeType(MasterId, pKey.PropertyType));
                }
            }
        }

        public void StateHasChanged()
        {
            var page = baseComponentService.Target as BasePage;
            if (page == null)
                throw new CaspianException("You must inherits from BasePage and add this code to page: base.BuildRenderTree(__builder);");
            string guid = page.GuId;
            //if (page.GetType() != this.GetType().DeclaringType)
            //    logger.LogInformation("You Inject Service in the component(in this case we can't call StateHasChanged method of page). so maybe all of page not changed");
            page.ChangeState();
        }

        protected IServiceScope CreateScope()
        {
            var scope = ServiceProvider.CreateScope();
            UserId = ServiceProvider.GetService<CaspianDataService>().UserId;
            scope.SetUserId(UserId);
            return scope;
        }

        protected async Task<bool> Confirm(string message)
        {
            if (baseComponentService.MessageBox == null)
                throw new CaspianException("You must inherits from BasePage and add this code to page: base.BuildRenderTree(__builder);");
            var window = basePageService.Peek();
            if (window !=  null) 
                return await window.GetMessageBox().Confirm(message);
            
            return await baseComponentService.MessageBox.Confirm(message);
        }

        IDictionary<string, ICollection> IInternalSearchService<TEntity>.EnumFields { get; set; }
    }
}
