using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using System.Linq.Expressions;
using Caspian.Common.Extension;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    internal class BatchService<TMaster, TDetail>: IInternalBatchService<TDetail> where TMaster : class where TDetail : class 
    {
        public BatchService(TMaster master) 
        {
            ChangedEntities = new List<ChangedEntity<TDetail>>();
            Master = master;
        }

        public async Task ReloadForUpdate(int masterId)
        {
            MasterId = masterId;
            ChangedEntities?.Clear();
            if (DataView != null)
            {
                DataView.InternalConditionExpr = GetDetailsFilterExpression();
                await DataView.ReloadAsync();
            }
        }

        public void ClearChangedEntities()
        {
            ChangedEntities?.Clear();
        }

        public void UpdateDetailsByChangedEntity(Type otherType, object otherId, ChangeType changeType)
        {
            switch(changeType)
            {
                case ChangeType.Added:
                    UpdateDetailsByChangedEntity(CreateDetail(otherType, otherId), changeType);
                    break;
                case ChangeType.Deleted:
                    var property = typeof(TDetail).GetForeignKey(otherType);
                    var old = Details.SingleOrDefault(t => property.GetValue(t).Equals(otherId)) ?? 
                        ChangedEntities.SingleOrDefault(t => property.GetValue(t.Entity).Equals(otherId)).Entity;
                    UpdateDetailsByChangedEntity(old, changeType);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        TDetail CreateDetail(Type otherType, object otherId)
        {
            var detail = Activator.CreateInstance<TDetail>();
            typeof(TDetail).GetForeignKey(otherType).SetValue(detail, otherId);
            typeof(TDetail).GetForeignKey(MasterType).SetValue(detail, MasterId);
            return detail;
        }

        public IEnumerable<TDetail> Details
        {
            get
            {
                return typeof(TMaster).GetDetailsProperty(typeof(TDetail)).GetValue(Master) as IEnumerable<TDetail>;
            }
        }

        public void UpdateDetailsByChangedEntity(TDetail detail, ChangeType changeType)
        {
            var property = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            var details = (ICollection<TDetail>)property.GetValue(Master);
            if (details == null)
            {
                details = new List<TDetail>();
                property.SetValue(Master, details);
            }
            switch(changeType)
            {
                case ChangeType.Added:
                    ChangedEntities.Add(new ChangedEntity<TDetail>()
                    {
                        Entity = detail,
                        ChangeType = ChangeType.Added
                    });
                    var pKey = typeof(TDetail).GetPrimaryKey();
                    var minId = 0;
                    foreach (var item in Details)
                    {
                        var minValue = Convert.ToInt32(pKey.GetValue(item));
                        if (minValue < minId)
                            minId = minValue;
                    }
                    pKey.SetValue(detail, minId - 1);
                    details.Add(detail);
                    break;
                case ChangeType.Updated:
                    if (!ChangedEntities.Any(t => t.Entity == detail && t.ChangeType != ChangeType.Deleted))
                    {
                        ChangedEntities.Add(new ChangedEntity<TDetail>()
                        {
                            Entity = detail,
                            ChangeType = ChangeType.Updated
                        });
                    }
                    break;
                case ChangeType.Deleted:
                    var old = ChangedEntities.SingleOrDefault(t => t.Entity == detail);
                    if (old == null)
                    {
                        ChangedEntities.Add(new ChangedEntity<TDetail>()
                        {
                            Entity = detail,
                            ChangeType = ChangeType.Deleted
                        });
                    }
                    else
                        ChangedEntities.Remove(old);
                    details.Remove(detail);
                    break;
            }

        }

        public void InitializeAfterUpsert()
        {
            ChangedEntities.Clear();
            typeof(TMaster).GetDetailsProperty(typeof(TDetail)).SetValue(Master, null);
            if (DataView != null)
            {
                DataView.CancelInternalUpdate();
                DataView.ClearDataView();
            }
        }

        public CaspianValidationValidator<TDetail> DetailValidator { get; set; }

        public IList<ChangedEntity<TDetail>> ChangedEntities { get; private set; }

        public int MasterId { get; set; }

        public TMaster Master { get; private set; }

        public DataView<TDetail> DataView { get; private set; }

        public PropertyInfo ThirdLevelProperty {  get; set; }

        public TypeWindow<TDetail> TypeWindow { get; private set; }

        public CaspianForm<TDetail> DetailForm { get; private set; }

        public Type MasterType { get { return typeof(TMaster); } }

        public void DetailCaspianValidationValidatorInitializer(CaspianValidationValidator<TDetail> validator)
        {
            DetailValidator = validator;
            DetailValidator.OnInternalValidate = EventCallback.Factory.Create<IBaseService<TDetail>>(this, t =>
            {
                //service.SetBatchServiceData(MasterId, typeof(TMaster));
                //if (t is IMasterDetailsService<TMaster, TDetail> detailService)
                //    detailService.SetChangedEntitiesAsync(Master, ChangedEntities);
            });
        }

        public void DetailDataViewInitializer(DataView<TDetail> dataView)
        {
            DataView = dataView;
            if (dataView != null)
            {
                //if (DataView.Inline)
                //    DataView.Batch = true;
                //var value = typeof(TMaster).GetPrimaryKey().GetValue(Master);
                //DataView.InsertIconState(true);
                //DataView.InternalConditionExpr = (this as IInternalBatchService<TDetail>).GetDetailsFilterExpression();
            }
        }

        public void DetailTypeWindowInitialize(TypeWindow<TDetail> window)
        {
            TypeWindow = window;
            DataView.Batch = true;
            DataView.OnInternalUpsert = EventCallback.Factory.Create<TDetail>(this, async detail =>
            {
                if (MasterId > 0)
                {
                    var info = typeof(TDetail).GetForeignKey(typeof(TMaster));
                    var value = Convert.ChangeType(MasterId, info.PropertyType.GetUnderlyingType());
                    info.SetValue(detail, value);
                }
                await TypeWindow.OpenAsync(detail, DataView.GetSource().ToList());
                (this as IInternalUIService<TMaster>).StateHasChanged();
                await Task.Delay(100);
                if (DetailForm != null)
                    await DetailForm.FocusAsync();
            });
        }

        public void DetailFormInitializer(CaspianForm<TDetail> form)
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
                        await DataView.InsertAsync(detail);
                    else
                        await DataView.UpdateAsync(detail);
                    (this as IInternalUIService<TMaster>).StateHasChanged();
                });
            }
;
        }

        public Expression GetDetailsFilterExpression()
        {
            var param = Expression.Parameter(typeof(TDetail), "t");
            var masterInfo = typeof(TDetail).GetForeignKey(typeof(TMaster));
            Expression expr = Expression.Property(param, masterInfo);
            var masterId = Convert.ChangeType(MasterId, masterInfo.PropertyType);
            return Expression.Equal(expr, Expression.Constant(masterId));
        }

        public void SetDetailsProperty(IEnumerable<TDetail> details)
        {
            var masterProperty = typeof(TDetail).GetForeignKey(typeof(TMaster));
            foreach (var detail in details) 
                masterProperty.SetValue(detail, MasterId);
            var property = typeof(TMaster).GetDetailsProperty(typeof(TDetail));
            property.SetValue(Master, details.ToList());
            
        }
    }
}
