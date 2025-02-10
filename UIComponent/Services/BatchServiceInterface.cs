using Caspian.Common;

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

        void DetailTypwWindowInitialize();

        void DetailFormInitialize();
    }
}
