using Caspian.Common;
using System.Reflection;
using System.Linq.Expressions;

namespace Caspian.UI
{
    public interface ISimpleBatchService<TDetail> where TDetail : class
    {
        int MasterId { get; }

        IList<ChangedEntity<TDetail>> ChangedEntities { get; set; }

        void DetailDataViewInitialize(DataView<TDetail> dataView);
    }

    public interface IDetailBatchService<TDetail> : ISimpleBatchService<TDetail> where TDetail : class
    {
        DataView<TDetail> DetailDataView { get; set; }

        TypeWindow<TDetail> TypeWindow { get; set; }

        CaspianForm<TDetail> DetailForm { get; set; }

        void DetailTypeWindowInitialize();

        void DetailFormInitialize();

        PropertyInfo ThirdLevelProperty { get; }

        void SetDetails(IList<TDetail> details);

        Expression GetDetailsFilterExpression();
    }
}
