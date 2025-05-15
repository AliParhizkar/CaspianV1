
using Caspian.Common;
using System.Collections;
using System.Linq.Expressions;

namespace Caspian.UI
{
    public interface IEnumSearch<TValue> where TValue : Enum
    {
        void SetValues(params TValue[] values);
    }

    public interface ISearchService<TEntity> where TEntity : class
    {
        DataView<TEntity> DataView { get; }

        TEntity Search { get; }

        IEnumSearch<TValue> GetEnumField<TValue>(Expression<Func<TEntity, TValue>> expression) where TValue : Enum;
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

    public interface IUIService
    {
        Task OpenWindow(int? id);

        Task CloseWindow();
    }

    internal interface IInternalUIService : IUIService
    {
        void WindowInitialize(Window window);

        void Dispose();

        Window Window { get; }
    }

    internal interface IInternalUIService<TEntity> : IInternalUIService, IInternalSearchService<TEntity>, IUIService<TEntity>  where TEntity : class
    {
        void FormInitialize(CaspianForm<TEntity> form);

        /// <summary>
        /// In 1 ti 1 relationship if is bigger tan 0 (MasterId > 0) itt fetch child from database
        /// </summary>
        /// <param name="detailType">Type of detail in 1 to 1 relationship</param>
        /// <returns></returns>
        Task UpdateChildOfModelAsync(Type detailType);
        
        void TabPanelInitialize(IEntityTabPanel tabPanel);
        
        /// <summary>
        /// Its fetch UpsertData from database by using MasterId as primary key
        /// </summary>
        /// <returns></returns>
        Task FetchAsync();
        void ClearForm();
        void StateHasChanged();
        
        /// <summary>
        /// It's Used in 1 To 1 Relationship for example Entity Type (1) <--> Detail Type (1)
        /// </summary>
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
