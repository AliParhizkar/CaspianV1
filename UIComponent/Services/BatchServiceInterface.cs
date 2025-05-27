using Caspian.Common;
using System.Reflection;
using System.Linq.Expressions;
using Caspian.Common.Service;

namespace Caspian.UI
{
    internal interface ISimpleBatchService
    {
        PropertyInfo ThirdLevelProperty { get; set; }
    }

    public interface ISimpleBatchService<TDetail> 
    {
        int MasterId { get; }

        Type MasterType { get; }

        IList<ChangedEntity<TDetail>> ChangedEntities { get; set; }
    }

    internal interface IInternalBatchService<TDetail>: IBatchService<TDetail>, ISimpleBatchService where TDetail : class
    {
        void DetailCaspianValidationValidatorInitializer(CaspianValidationValidator<TDetail> validator);
        void DetailDataViewInitializer(DataView<TDetail> dataView);
        void DetailTypeWindowInitialize(TypeWindow<TDetail> window);
        void DetailFormInitializer(CaspianForm<TDetail> form);
        Expression GetDetailsFilterExpression();
    }

    public interface IBatchService<TDetail> : ISimpleBatchService<TDetail> where TDetail : class
    {
        DataView<TDetail> DetailDataView { get; }

        TypeWindow<TDetail> TypeWindow { get; }

        CaspianForm<TDetail> DetailForm { get; }
    }

    public interface IBatchService<TMaster, TDetail> where TMaster:class where TDetail: class
    {

    }
}
