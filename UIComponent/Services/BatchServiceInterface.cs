using Caspian.Common;
using System.Reflection;
using System.Linq.Expressions;

namespace Caspian.UI
{
    public interface ISimpleBatchService<TDetail> 
    {
        int MasterId { get; }

        IList<ChangedEntity<TDetail>> ChangedEntities { get; set; }
    }

    internal interface IInternalBatchService<TDetail>: IBatchService<TDetail> where TDetail : class
    {
        void DetailDataViewInitialize(DataView<TDetail> dataView);
        void DetailTypeWindowInitialize(TypeWindow<TDetail> window);

        void DetailFormInitialize(CaspianForm<TDetail> caspianForm);

        void SetDetails(IList<TDetail> details);

        Expression GetDetailsFilterExpression();

        PropertyInfo ThirdLevelProperty { get; set; }
    }

    public interface IBatchService<TDetail> : ISimpleBatchService<TDetail> where TDetail : class
    {
        DataView<TDetail> DetailDataView { get; }

        TypeWindow<TDetail> TypeWindow { get; }

        CaspianForm<TDetail> DetailForm { get; }
    }
}
