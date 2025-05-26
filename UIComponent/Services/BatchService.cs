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
                    detailService.SetChangedEntitiesAsync(UpsertData, ChangedEntities);
            });
        }

        protected override async Task InitializeValidatorService(IBaseService<TMaster> service)
        {
            (service as MasterDetailsService<TMaster, TDetail>).SetChangedEntitiesAsync(UpsertData, ChangedEntities);
            await base.InitializeValidatorService(service);
        }

        protected virtual async Task SetChangedEntities()
        {
            if (Form?.ValidationValidator?.Validator != null)
            {
                if (Form.ValidationValidator.Validator is IMasterDetailsService<TMaster, TDetail> service)
                    service.SetChangedEntitiesAsync(UpsertData, ChangedEntities);
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
    }
}