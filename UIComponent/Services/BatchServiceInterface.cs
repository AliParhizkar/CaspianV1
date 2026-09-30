using Caspian.Common;
using System.Reflection;
using Caspian.Common.Service;
using System.Linq.Expressions;

namespace Caspian.UI
{
    public interface ISimpleBatchService<TDetail> 
    {
        int MasterId { get; set; }

        IList<ChangedEntity<TDetail>> ChangedEntities { get; }
        
        Type MasterType { get; }

        IEnumerable<TDetail> Details { get; }

        void UpdateDetailsByChangedEntity(Type otherType, object otherId, ChangeType changeType);

        void SetDetailsProperty(IEnumerable<TDetail> details);

        void ClearChangedEntities();
    }

    public interface IBatchService<TDetail> : ISimpleBatchService<TDetail> where TDetail : class
    {
        DataView<TDetail> DataView { get; }

        TypeWindow<TDetail> TypeWindow { get; }

        CaspianForm<TDetail> DetailForm { get; }
    }

    internal interface IInternalBatchService<TDetail>: IBatchService<TDetail> where TDetail : class
    {
        PropertyInfo ThirdLevelProperty { get; set; }

        Task ReloadForUpdate(int masterId);

        void InitializeAfterUpsert();
        void DetailCaspianValidationValidatorInitializer(CaspianValidationValidator<TDetail> validator);
        void DetailDataViewInitializer(DataView<TDetail> dataView);
        void DetailTypeWindowInitialize(TypeWindow<TDetail> window);
        void DetailFormInitializer(CaspianForm<TDetail> form);
        Expression GetDetailsFilterExpression();
        
        void UpdateDetailsByChangedEntity(TDetail detail, ChangeType changeType);
    }

    public interface IUIService<TMaster, TDetail>
        : IUIService<TMaster>
        where TMaster : class
        where TDetail : class
    {
        Task ReloadForUpdate(int masterId);

        IBatchService<TDetail> DetailService { get; }
    }

    public interface IUIService<TMaster, TDetail1, TDetail2>
        : IUIService<TMaster, TDetail1>
        where TMaster : class
        where TDetail1 : class
        where TDetail2 : class
    {
        IBatchService<TDetail2> Detail2Service { get; }
    }
}
