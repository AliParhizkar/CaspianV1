using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;

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

        public TypeWindow<TDetail1> TypeWindow { get; private set; }

        public CaspianForm<TDetail1> DetailForm { get; private set; }

        void IInternalBatchService<TDetail1>.DetailFormInitialize(CaspianForm<TDetail1> caspianForm)
        {
            DetailForm = caspianForm;
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

        public IList<ChangedEntity<TDetail1>> ChangedEntities { get; set; }

        void IInternalBatchService<TDetail1>.DetailDataViewInitialize(DataView<TDetail1> dataView)
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

        protected override async Task SetChangedEntities()
        {
            if (Form?.ValidationValidator?.Validator != null)
            {
                if (Form.ValidationValidator.Validator is IMasterDetailsService<TMaster, TDetail, TDetail1> service)
                    await service.SetChangedEntities(UpsertData, base.ChangedEntities, ChangedEntities);
            }
        }

        void IInternalBatchService<TDetail1>.DetailCaspianValidationValidatorInitialize(CaspianValidationValidator<TDetail1> validator)
        {
            
        }
    }
}
