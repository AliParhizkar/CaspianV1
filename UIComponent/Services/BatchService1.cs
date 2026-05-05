using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public class UIService<TMaster, TDetail, TDetail1> : UIService<TMaster, TDetail>, IInternalBatchService<TDetail1>
        where TMaster : class where TDetail : class where TDetail1 : class
    {
        public UIService(IServiceProvider provider):
            base(provider)
        {
            ChangedEntities = new List<ChangedEntity<TDetail1>>();
        }

        void IInternalBatchService<TDetail1>.SetDetailsProperty(IList<TDetail1> details)
        {
            var property = typeof(TMaster).GetDetailsProperty(typeof(TDetail1));
            property.SetValue(UpsertData, details);
        }

        Expression IInternalBatchService<TDetail1>.GetDetailsFilterExpression()
        {
            var param = Expression.Parameter(typeof(TDetail1), "t");
            var masterInfo = param.Type.GetForeignKey(typeof(TMaster));
            Expression expr = Expression.Property(param, masterInfo);
            var masterId = Convert.ChangeType(MasterId, masterInfo.PropertyType);
            return Expression.Equal(expr, Expression.Constant(masterId));
        }

        PropertyInfo ISimpleBatchService.ThirdLevelProperty { get; set; }

        public DataView<TDetail1> DetailDataView { get; private set; }

        public CaspianValidationValidator<TDetail1> DetailValidator { get; private set; }

        public TypeWindow<TDetail1> TypeWindow { get; private set; }

        public CaspianForm<TDetail1> DetailForm { get; private set; }

        public IList<ChangedEntity<TDetail1>> ChangedEntities { get; set; }

        protected override void DisposeResource()
        {
            //ChangedEntities.Clear();
            //(this as ISimpleBatchService).ThirdLevelProperty = null;
            //DetailDataView = null;
            //DetailValidator = null;
            //TypeWindow = null;
            //DetailForm = null;
            //base.DisposeResource();
        }

        protected override async Task SetChangedEntities()
        {
            if (Form?.ValidationValidator?.Validator != null)
            {
                if (Form.ValidationValidator.Validator is IMasterDetailsService<TMaster, TDetail, TDetail1> service)
                    await service.SetChangedEntities(UpsertData, base.ChangedEntities, ChangedEntities);
            }
        }

        protected override IBaseService<TMaster> CreateService(IServiceScope scope)
        {
            
            return scope.GetService<IMasterDetailsService<TMaster, TDetail, TDetail1>>();
        }

        #region
        void IInternalBatchService<TDetail1>.DetailDataViewInitializer(DataView<TDetail1> dataView)
        {
            DetailDataView = dataView;
            if (dataView != null)
            {
                if (DetailDataView.Inline)
                    DetailDataView.Batch = true;
                DetailDataView.InsertIconState(true);
                DetailDataView.InternalConditionExpr = (this as IInternalBatchService<TDetail1>).GetDetailsFilterExpression();
            }
        }

        protected override TMaster InitializeBeforeUpsert(IBaseService<TMaster> service)
        {
            (service as IMasterDetailsService<TMaster, TDetail, TDetail1>).SetChangedEntities(base.ChangedEntities, ChangedEntities);
            return base.InitializeBeforeUpsert(service);
        }

        void IInternalBatchService<TDetail1>.DetailCaspianValidationValidatorInitializer(CaspianValidationValidator<TDetail1> validator)
        {
            DetailValidator = validator;
            DetailValidator.OnInternalValidate = EventCallback.Factory.Create<IBaseService<TDetail1>>(this, t =>
            {
                ///Initialize Validator-Service(BaseService<TDetail1>) Before validation 
                var service = t as BaseService<TDetail1>;
                service.SetBatchServiceData(MasterId, typeof(TMaster));
                if (t is IMasterDetailsService<TMaster, TDetail1> detailService)
                    detailService.SetChangedEntitiesAsync(UpsertData, ChangedEntities);
            }); 
        }

        void IInternalBatchService<TDetail1>.DetailFormInitializer(CaspianForm<TDetail1> form)
        {
            DetailForm = form;
            if (DetailForm != null)
            {
                DetailForm.OnInternalReset = EventCallback.Factory.Create(this, TypeWindow.Close);
                DetailForm.OnInternalValidSubmit = EventCallback.Factory.Create<TDetail1>(this, async detail1 =>
                {
                    TypeWindow.Close();
                    var id = Convert.ToInt32(typeof(TDetail1).GetPrimaryKey().GetValue(detail1));
                    if (id == 0)
                        await DetailDataView.InsertAsync(detail1);
                    else
                        await DetailDataView.UpdateAsync(detail1);
                    (this as IInternalUIService<TMaster>).StateHasChanged();
                });
            }
        }

        void IInternalBatchService<TDetail1>.DetailTypeWindowInitialize(TypeWindow<TDetail1> window)
        {
            TypeWindow = window;
            DetailDataView.Batch = true;
            DetailDataView.OnInternalUpsert = EventCallback.Factory.Create<TDetail1>(this, async detail1 =>
            {
                if (MasterId > 0)
                {
                    var info = typeof(TDetail1).GetForeignKey(typeof(TMaster));
                    var value = Convert.ChangeType(MasterId, info.PropertyType.GetUnderlyingType());
                    info.SetValue(detail1, value);
                }
                await TypeWindow.OpenAsync(detail1, DetailDataView.GetSource().ToList());
                (this as IInternalUIService<TMaster>).StateHasChanged();
                await Task.Delay(100);
                if (DetailForm != null)
                    await DetailForm.FocusAsync();
            });
        }
        #endregion
    }
}
