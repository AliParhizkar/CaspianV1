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

        void IInternalBatchService<TDetail>.SetDetailsProperty(IList<TDetail> details)
        {
            var property = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            property.SetValue(UpsertData, details);
        }

        PropertyInfo ISimpleBatchService.ThirdLevelProperty { get; set; }

        public DataView<TDetail> DetailDataView { get; private set; }

        public TypeWindow<TDetail> TypeWindow { get; private set; }

        public CaspianForm<TDetail> DetailForm { get; private set; }

        public CaspianValidationValidator<TDetail> DetailValidator { get; set; }

        public Type MasterType => typeof(TMaster);

        public IList<ChangedEntity<TDetail>> ChangedEntities { get; set; }

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

        /// <summary>
        /// In Master-Details page we should filter the detail data-view (grid or list view) by MasterId. 
        /// This method create the expression for filter
        /// </summary>
        /// <returns></returns>
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
            (service as IMasterDetailsService<TMaster, TDetail>).SetChangedEntities(ChangedEntities);
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

        protected virtual Task SetChangedEntities()
        {
            if (Form?.ValidationValidator?.Validator != null)
            {
                if (Form.ValidationValidator.Validator is IMasterDetailsService<TMaster, TDetail> service)
                    service.SetChangedEntitiesAsync(UpsertData, ChangedEntities);
            }

            return Task.CompletedTask;
        }

        protected override async Task InitializeValidatorService(IBaseService<TMaster> service)
        {
            (service as MasterDetailsService<TMaster, TDetail>).SetChangedEntitiesAsync(UpsertData, ChangedEntities);
            await base.InitializeValidatorService(service);
        }

        #region Methods for Initialize Components. This Methods Call from components(DataView, Form, TypeWindow, ...) For intialize
        void IInternalBatchService<TDetail>.DetailDataViewInitializer(DataView<TDetail> dataView)
        {
            DetailDataView = dataView;
            if (dataView != null)
            {
                if (DetailDataView.Inline)
                    DetailDataView.Batch = true;
                var value = typeof(TMaster).GetPrimaryKey().GetValue(base.UpsertData);
                /// Why this code should be exist
                //MasterId = Convert.ToInt32(value);
                DetailDataView.InsertIconState(true);
                DetailDataView.InternalConditionExpr = (this as IInternalBatchService<TDetail>).GetDetailsFilterExpression();
            }
        }

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

        void IInternalBatchService<TDetail>.DetailFormInitializer(CaspianForm<TDetail> form)
        {
            DetailForm = form;
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
;
        }

        void IInternalBatchService<TDetail>.DetailCaspianValidationValidatorInitializer(CaspianValidationValidator<TDetail> validator)
        {
            DetailValidator = validator;
            DetailValidator.OnInternalValidate = EventCallback.Factory.Create<IBaseService<TDetail>>(this, t =>
            {
                ///Initialize Validator-Service(BaseService<TDetail>) Before validation 
                var service = t as BaseService<TDetail>;
                service.SetBatchServiceData(MasterId, typeof(TMaster));
                if (t is IMasterDetailsService<TMaster, TDetail> detailService)
                    detailService.SetChangedEntitiesAsync(UpsertData, ChangedEntities);
            });
        }
        #endregion
    }
}