
using Caspian.Common;
using System.Collections;
using System.Linq.Expressions;

namespace Caspian.UI
{
    public interface IUIService
    {
        Window Window { get; set; }

        void WindowInitialize();

        void Dispose();

        Task OpenWindow(int id);
    }

    internal interface IInternalUIService: IUIService
    {

    }

    public interface IEnumSearch<TValue> where TValue : Enum
    {
        void SetValues(params TValue[] values);
    }


    public interface ISearchService<TEntity> where TEntity : class
    {
        DataView<TEntity> DataView { get; set; }


        TEntity Search { get; }

        void DataViewInitialize();

        void SetSearchType(IDictionary<string, SearchType> types);

        void SetEnumFields(IDictionary<string, ICollection> enumFields);

        IDictionary<string, ICollection> GetEnumFields();

        IEnumSearch<TValue> GetEnumField<TValue>(Expression<Func<TEntity, TValue>> expression)where TValue:Enum;

        IDictionary<string, SearchType> GetSearchData();

        void OnlyForSearch();

        void HideFooter();
    }

    internal interface IUIService<TEntity>
    {

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

        void StateHasChanged();
    }
}
