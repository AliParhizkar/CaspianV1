using Caspian.Common;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;
using System.Reflection;

namespace Caspian.UI
{
    public interface ISimpleBatchService<TDetail>
    {
        int MasterId { get; }

        IList<ChangedEntity<TDetail>> ChangedEntities { get; set; }

        void DetailDataViewInitialize();
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
