using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public class UIService<TMaster, TDetail>: UIService<TMaster>, IInternalBatchService<TDetail> where TMaster : class where TDetail : class
    {
        public UIService(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            ChangedEntities = new List<ChangedEntity<TDetail>>();
        }

        PropertyInfo ISimpleBatchService.ThirdLevelProperty { get; set; }

        public DataView<TDetail> DetailDataView { get; set; }

        public TypeWindow<TDetail> TypeWindow { get; set; }

        public CaspianForm<TDetail> DetailForm { get; set; }

        public CaspianValidationValidator<TDetail> DetailValidator { get; set; }

        public Type MasterType => typeof(TMaster);

        public IList<ChangedEntity<TDetail>> ChangedEntities { get; set; }

        Expression IInternalBatchService<TDetail>.GetDetailsFilterExpression()
        {
            var param = Expression.Parameter(typeof(TDetail), "t");
            var masterInfo = typeof(TDetail).GetForeignKey(typeof(TMaster));
            Expression expr = Expression.Property(param, masterInfo);
            var masterId = Convert.ChangeType(MasterId, masterInfo.PropertyType);
            return Expression.Equal(expr, Expression.Constant(masterId));
        }

        protected override IBaseService<TMaster> CreateService(IServiceScope scope)
        {
            return scope.GetService<IMasterDetailsService<TMaster, TDetail>>();
        }

        protected override TMaster InitializeBeforeUpsert(IBaseService<TMaster> service)
        {
            (service as MasterDetailsService<TMaster, TDetail>).SetChangedEntities(ChangedEntities);
            return base.InitializeBeforeUpsert(service);
        }

        protected override async Task InitializeAfterUpsert(TMaster tempEntity, UpsertMode upsertMode)
        {
            ChangedEntities.Clear();
            if (DetailDataView != null)
            {
                DetailDataView.ClearSource();
                DetailDataView.CancelInternalUpdate();
            }
            await base.InitializeAfterUpsert(tempEntity, upsertMode);

        }

        public void ThirdDataLevelToIgnoreOnRemove<TProperty>(Expression<Func<TDetail, ICollection<TProperty>>> expression) => (this as IInternalBatchService<TDetail>).ThirdLevelProperty = (expression.Body as MemberExpression).Member as PropertyInfo;

        /// <summary>
        /// This method reload data for update
        /// </summary>
        /// <param name="masterId">The id of "TMaster"</param>
        public async Task ReloadForUpdate(int masterId)
        {
            MasterId = masterId;
            ChangedEntities?.Clear();
            if (DetailDataView != null)
            {
                DetailDataView.InternalConditionExpr = (this as IInternalBatchService<TDetail>).GetDetailsFilterExpression();
                await DetailDataView.ReloadAsync();
            }
            await (this as IInternalUIService<TMaster>).FetchAsync();
        }

        //void IInternalUIService.Dispose()
        //{
        //    (this as IInternalUIService<TMaster>).DetailType = default;
        //    /// On Master Details Service We Set MasterId on OnInitialized but it reset here because page disposed 
        //    /// on another page created

        //    //MasterId = default;
        //    (this as IInternalUIService).WindowInitialize(null);
        //    DetailDataView = default;
        //}

        void IInternalBatchService<TDetail>.DetailFormInitialize(CaspianForm<TDetail> caspianForm)
        {
            DetailForm = caspianForm;
            if (DetailForm != null)
            {
                DetailForm.OnInternalReset = EventCallback.Factory.Create(this, TypeWindow.Close);
                //DetailDataView.
                DetailForm.OnInternalValidSubmit = EventCallback.Factory.Create<TDetail>(this, async detail => 
                {
                    TypeWindow.Close();
                    var id = Convert.ToInt32(typeof(TDetail).GetPrimaryKey().GetValue(detail));
                    if (id == 0)
                        await DetailDataView.InsertAsync(detail);   
                    else
                        await DetailDataView.UpdateAsync(detail);
                    (this as IInternalUIService<TMaster>).StateHasChanged();
                });
            }
;        }

        void IInternalBatchService<TDetail>.DetailCaspianValidationValidatorInitialize(CaspianValidationValidator<TDetail> validator)
        {
            DetailValidator = validator;
            DetailValidator.OnInternalValidate = EventCallback.Factory.Create<IBaseService<TDetail>>(this, async t => 
            {
                ///Initialize Validator-Service Before validation 
                var service = t as BaseService<TDetail>;
                service.SetBatchServiceData(MasterId, typeof(TMaster));
                if (t is MasterDetailsService<TMaster, TDetail> detailService)
                {
                    detailService.SetChangedEntitiesAsync(UpsertData, ChangedEntities);

                }
            });
        }
        protected override async Task InitializeValidatorService(IBaseService<TMaster> service)
        {
            (service as MasterDetailsService<TMaster, TDetail>).SetChangedEntitiesAsync(UpsertData, ChangedEntities);
            await base.InitializeValidatorService(service);
        }

        //async Task IInternalUIService<TMaster>.FetchAsync()
        //{
        //    batchServiceData.MasterId = MasterId;
        //    if (MasterId > 0)
        //    {
        //        using var service = CreateScope().GetService<IBaseService<TMaster>>();
        //        var propertyName = typeof(TMaster).GetDetailsProperty(typeof(TDetail)).Name;
        //        UpsertData = await service.GetAll().Include(propertyName).SingleAsync(MasterId);
        //        Form?.SetModel(UpsertData);
        //    }
        //}




        //protected virtual async Task UpdateDatabaseAsync(TMaster master)
        //{
        //    var id = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(master));
        //    using var scope = CreateScope();

        //    if (OnUpsert != null)
        //    {
        //        if (!await OnUpsert.Invoke(scope.ServiceProvider, master))
        //            return;
        //    }
        //    var service = scope.GetService<IMasterDetailsService<TMaster, TDetail>>();
        //    service.SetChangedEntities(ChangedEntities);
        //    TMaster result = default;
        //    if (id == 0)
        //        result = await service.AddAsync(UpsertData);
        //    else
        //    {
        //        await service.UpdateAsync(UpsertData);
        //        result = UpsertData;
        //    }
        //    await service.SaveChangesAsync();
        //    if (OnAfterOpsertAsync != null)
        //        await OnAfterOpsertAsync(scope.ServiceProvider, result);
        //    ChangedEntities.Clear();
        //    if (service.Context?.Database?.CurrentTransaction != null)
        //    {
        //        Console.ForegroundColor = ConsoleColor.Yellow;
        //        Console.WriteLine("Warning: Transaction Rollbacked by Caspian infrastructure");
        //        Console.ResetColor();
        //        service.Context.Database.CurrentTransaction.Rollback();
        //    }
        //    DetailDataView?.ClearSource();
        //    UpsertData = Activator.CreateInstance<TMaster>();
        //    if (UpsertData is BaseEntity baseEntity)
        //        baseEntity.UpsertUserId = UserId;
        //    Form.SetModel(UpsertData);
        //    if (OnCreate != null)
        //        OnCreate.Invoke(UpsertData);
        //    if (DataView != null)
        //    {
        //        var newId = (int)typeof(TMaster).GetPrimaryKey().GetValue(result);
        //        await DataView.SelectRowById(newId);
        //    }
        //    string message = null;
        //    if (id == 0)
        //        message = CaspianDataService.Language == Language.Fa ? "ثبت با موفقیت انجام شد." : "Registration was done successfully";
        //    else
        //        message = CaspianDataService.Language == Language.Fa ? "بروزرسانی با موفقیت انجام شد." : "Updating was done successfully";
        //    await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", message);

        //    if (DetailDataView != null)
        //        DetailDataView.CancelInternalUpdate();
        //    if ((this as IInternalUIService).Window != null)
        //        await (this as IInternalUIService).Window?.Close();
        //    (this as IInternalUIService<TMaster>).StateHasChanged();
        //}

        protected virtual async Task SetChangedEntities()
        {
            //if (UpsertData is BaseEntity entity)
            //{
            //    entity.UpsertUserId = CaspianDataService.UserId;
            //    entity.UpsertDate = DateTime.Now;
            //}
            if (Form?.ValidationValidator?.Validator != null)
            {
                if (Form.ValidationValidator.Validator is IMasterDetailsService<TMaster, TDetail> service)
                    service.SetChangedEntitiesAsync(UpsertData, ChangedEntities);
            }
        }

        //void IInternalUIService<TMaster>.FormInitialize(CaspianForm<TMaster> form)
        //{
        //    Form = form;
        //    var detailsProperty = typeof(TMaster).GetProperties().Single(t => t.PropertyType.IsGenericType && t.PropertyType.GenericTypeArguments[0] == typeof(TDetail));
        //    if (UpsertData == null)
        //        UpsertData = Activator.CreateInstance<TMaster>();
        //    //if (UpsertData is BaseEntity baseEntity)
        //    //{
        //    //    baseEntity.UpsertUserId = CaspianDataService.UserId;
        //    //    baseEntity.UpsertDate = DateTime.Now;
        //    //}
        //    if (OnCreate != null)
        //        OnCreate.Invoke(UpsertData);   
        //    Form.SetModel(UpsertData);
        //    Form.OnInternalReset = EventCallback.Factory.Create(this, async () =>
        //    {
        //        DetailDataView?.ClearSource();
        //        if ((this as IInternalUIService).Window != null)
        //        {
        //            await (this as IInternalUIService).Window?.Close();
        //            (this as IInternalUIService<TMaster>).StateHasChanged();
        //        }
        //    });
        //    form.OnBeforeValidate = EventCallback.Factory.Create(this,  SetChangedEntities);
        //    Form.OnInternalValidSubmit = EventCallback.Factory.Create<TMaster>(this, UpdateDatabaseAsync);
        //}

        //void IInternalUIService<TMaster>.StateHasChanged()
        //{
        //    baseComponentService = serviceProvider.GetService<BaseComponentService>();
        //    if (baseComponentService.Target == null)
        //        throw new CaspianException("You must inherits from BasePage or configure page manually");
        //    (baseComponentService.Target as BasePage).ChangeState();
        //}

        //void IInternalSearchService<TMaster>.DataViewInitialize(DataView<TMaster> dataView)
        //{
        //    DataView = dataView;
        //    if (dataView == null)
        //        return;
        //    DataView.Search = Search;
        //    DataView.HideFooter = DataView.HideFooter ?? hideFooter;
        //    DataView.InsertIconState(!onlyForSearch);
        //    DataView.OnInternalUpsert = EventCallback.Factory.Create<TMaster>(this, async master =>
        //    {
        //        if ((this as IInternalUIService).Window != null)
        //        {
        //            var value = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(master));
        //            if (value != 0)
        //            {
        //                var detailsName = typeof(TMaster).GetDetailsProperty(typeof(TDetail)).Name;
        //                using var service = CreateScope().GetService<MasterDetailsService<TMaster, TDetail>>();
        //                UpsertData = await service.GetAll().Include(detailsName).SingleAsync(value);
        //            }
        //            else
        //            {
        //                UpsertData = Activator.CreateInstance<TMaster>();
        //                ChangedEntities.Clear();
        //            }
        //            MasterId = value;
        //            await (this as IInternalUIService).Window.Open();
        //            (this as IInternalUIService<TMaster>).StateHasChanged();
        //            await Task.Delay(100);
        //            if (Form != null)
        //                await Form.FocusAsync();
        //        }
        //    });
        //    DataView.OnInternalDelete = EventCallback.Factory.Create<TMaster>(this, async master =>
        //    {
        //        using var scope = CreateScope();
        //        using var service = scope.GetService<MasterDetailsService<TMaster, TDetail>>();
        //        var id = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(master));
        //        var detailsProperty = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
        //        var old = await service.GetAll().Include(detailsProperty.Name).SingleAsync(id);
        //        var result = await service.ValidateRemoveAsync(old);
        //        if (result.IsValid)
        //        {
        //            var details = detailsProperty.GetValue(old) as IEnumerable<TDetail>;
        //            string errorMessage = null;
        //            var detailService = scope.GetService<IBaseService<TDetail>>();
        //            foreach (var item in details) 
        //            {
        //                var result1 = await detailService.ValidateRemoveAsync(item);
        //                if (!result1.IsValid)
        //                {
        //                    errorMessage = result1.Errors.First().ErrorMessage;
        //                    break;
        //                }
        //            }
        //            if (errorMessage == null)
        //            {
        //                if (!DataView.DeleteMessage.HasValue() || await Confirm(DataView.DeleteMessage))
        //                {
        //                    service.Remove(old);
        //                    await service.SaveChangesAsync();
        //                    await DataView.ReloadAsync();
        //                    await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", "حذف با موفقیت انجام شد.");
        //                }
        //            }
        //            else
        //                await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", errorMessage);
        //        }
        //        else
        //            await jSRuntime.InvokeVoidAsync("caspian.common.showMessage", result.Errors[0].ErrorMessage);
        //    });
        //}

        void IInternalBatchService<TDetail>.DetailTypeWindowInitialize(TypeWindow<TDetail> window)
        {
            TypeWindow = window;
            DetailDataView.Batch = true;
            DetailDataView.OnInternalUpsert = EventCallback.Factory.Create<TDetail>(this, async detail => 
            {
                if (MasterId > 0)
                {
                    var info = typeof(TDetail).GetForeignKey(typeof(TMaster));
                    var value = Convert.ChangeType(MasterId, info.PropertyType.GetUnderlyingType());
                    info.SetValue(detail, value);
                }
                await TypeWindow.OpenAsync(detail, DetailDataView.GetSource().ToList());
                (this as IInternalUIService<TMaster>).StateHasChanged();
                await Task.Delay(100);
                if (DetailForm != null)
                    await DetailForm.FocusAsync();
            });
        }

        void IInternalBatchService<TDetail>.DetailDataViewInitialize(DataView<TDetail> dataView)
        {
            DetailDataView = dataView;
            if (dataView != null)
            {
                if (DetailDataView.Inline)
                    DetailDataView.Batch = true;
                DetailDataView.InsertIconState(true);
                DetailDataView.InternalConditionExpr = (this as IInternalBatchService<TDetail>).GetDetailsFilterExpression();
            }
        }

        //void IInternalUIService.WindowInitialize(Window window)
        //{
        //    Window = window;
        //    if (window != null)
        //    {
        //        (this as IInternalUIService).Window.OnInternalOpen = EventCallback.Factory.Create(this, () =>
        //        {
        //            MasterId = Convert.ToInt32(typeof(TMaster).GetPrimaryKey().GetValue(UpsertData));
        //            batchServiceData.MasterId = MasterId;
        //        });
        //    }
        //}

        //void IInternalUIService<TMaster>.ClearForm()
        //{
        //    //Form = null;
        //    //UpsertData = null;
        //    //ChangedEntities = null;
        //}
    }
}