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

        public Type MasterType { get; set; }

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

        /// <summary>
        /// This event Fired After data saving 
        /// </summary>
        public Func<IServiceProvider, TEntity, Task> OnAfterUpsertAsync { get; set; }

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

        protected virtual async Task InitializeAfterUpsert(TEntity tempEntity, UpsertMode upsertMode)
        {
            if (Is1To1RelationshipService)
            {
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
                    if (Window == null)
                        StateHasChanged();
                    if (MasterType != null)
                        DataView.ChangeState();
                }
                else
                    await DataView.ReloadAsync();
            }
            UpsertData = Activator.CreateInstance<TEntity>();
            Form.SetModel(UpsertData);
            if (UpsertData is BaseEntity baseEntity)
                baseEntity.UpsertUserId = UserId;
           


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
            string message = null;
            if (upsertMode == UpsertMode.Insert)
                message = CaspianDataService.Language == Language.En ? "Registration was done successfully" : "ثبت با موفقیت انجام شد.";
            else
                message = CaspianDataService.Language == Language.En ? "Updating was done successfully" : "بروزرسانی با موفقیت انجام شد";
            await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", message);
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
        protected virtual async Task InitializeValidatorService(IBaseService<TEntity> service)
        {
            (service as BaseService<TEntity>).SetBatchServiceData(MasterId, typeof(TEntity));
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
            Form.OnInternalValidSubmit = EventCallback.Factory.Create<TEntity>(this, UpsertAndInitializeAfterValidate);
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

        void IUIService<TEntity>.CaspianValidationValidatorInitialize(CaspianValidationValidator<TEntity> validator)
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

        /// <summary>
        /// This Method Call form Lookup-window (In lookup-window we Hide Insert-Icon)
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
                var pKey = typeof(TEntity).GetPrimaryKey();
                if (old != null)
                {
                    UpsertData.CopyEntity(old);
                    pKey.SetValue(UpsertData, MasterId);
                }
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
    }
}
