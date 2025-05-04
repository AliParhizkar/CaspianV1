
using Caspian.Common;
using System.Collections;
using System.Linq.Expressions;

namespace Caspian.UI
{
    public interface IUIService
    {
        Task OpenWindow(int? id);

        Task CloseWindow();
    }

    internal interface IInternalUIService: IUIService
    {
        void WindowInitialize();

        void Dispose();

        Window Window { get; set; }
    }

    public interface IEnumSearch<TValue> where TValue : Enum
    {
        void SetValues(params TValue[] values);
    }

    internal interface IInternalSearchService<TEntity>: ISearchService<TEntity> where TEntity : class
    {
        void DataViewInitialize(DataView<TEntity> dataView);
        void SetSearchType(IDictionary<string, SearchType> types);
        void SetEnumFields(IDictionary<string, ICollection> enumFields);
        IDictionary<string, ICollection> GetEnumFields();
        IDictionary<string, SearchType> GetSearchData();
    }

    public interface ISearchService<TEntity> where TEntity : class
    {
        DataView<TEntity> DataView { get; }

        TEntity Search { get; }

        IEnumSearch<TValue> GetEnumField<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : Enum;

        void OnlyForSearch();

        void HideFooter();
    }

    public interface IUIService<TEntity> : IUIService, ISearchService<TEntity> where TEntity : class
    {
        int MasterId { get; set; }

        CaspianForm<TEntity> Form { get;}

        void FormInitialize(CaspianForm<TEntity> form);

        Task FetchAsync();

        Task UpdateChildOfModelAsync(Type childType);

        TEntity UpsertData { get; }

        void ClearForm();

        Type DetailType { get; }

        IEntityTabPanel EntityTabPanel { get; set; }

        Func<IServiceProvider, TEntity, Task<bool>> OnUpsert { get; set; }

        void TabPanelInitialize();

        void StateHasChanged();
    }
}
