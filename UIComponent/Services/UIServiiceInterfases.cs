
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
        void WindowInitialize(Window window);

        void Dispose();

        Window Window { get; }
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
        void OnlyForSearch();
        void HideFooter();

    }

    public interface ISearchService<TEntity> where TEntity : class
    {
        DataView<TEntity> DataView { get; }

        TEntity Search { get; }

        IEnumSearch<TValue> GetEnumField<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : Enum;
    }

    internal interface IInternalUIService<TEntity> : IUIService<TEntity>  where TEntity : class
    {
        void FormInitialize(CaspianForm<TEntity> form);
        
        Task UpdateChildOfModelAsync(Type childType);
        
        void TabPanelInitialize(IEntityTabPanel tabPanel);
     
        Task FetchAsync();
        void ClearForm();
        void StateHasChanged();
        Type DetailType { get; set; }

    }

    public interface IUIService<TEntity> : IUIService, ISearchService<TEntity> where TEntity : class
    {
        int MasterId { get; set; }

        CaspianForm<TEntity> Form { get;}

        TEntity UpsertData { get; }

        IEntityTabPanel EntityTabPanel { get; }

        Func<IServiceProvider, TEntity, Task<bool>> OnUpsert { get; set; }

    }
}
