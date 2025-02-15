
using Caspian.Common;

namespace Caspian.UI
{
    public interface IUIService
    {
        Window Window { get; set; }

        void WindowInitialize();

        void Dispose();
    }

    public interface ISearchService<TEntity> where TEntity : class
    {
        DataView<TEntity> DataView { get; set; }

        TEntity Search { get; }

        void DataViewInitialize();

        void SetSearchType(IDictionary<string, SearchType> types);

        IDictionary<string, SearchType> GetSearchData();
        void OnlyForSearch();

        void HideFooter();
    }

    public interface IUIService<TEntity> : IUIService, ISearchService<TEntity> where TEntity : class
    {
        int MasterId { get; set; }

        CaspianForm<TEntity> Form { get; set; }

        void FormInitialize();

        Task FetchAsync();

        Task UpdateChildOfModelAsync(Type childType);

        TEntity UpsertData { get; }

        void ClearForm();

        Type DetailType { get; }

        IEntityTabPanel EntityTabPanel { get; set; }

        Func<IServiceProvider, TEntity, Task<bool>> OnUpsert { get; set; }

        void TabPanelInitialize();
    }
}
