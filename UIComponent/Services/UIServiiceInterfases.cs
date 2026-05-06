
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
        void DataViewInitializer(DataView<TEntity> dataView);
        IDictionary<string, ICollection> EnumFields { get; set; }
        IDictionary<string, SearchType> SearchData { get; set; }
        IList<ValueTypeContainer> ValueTypes { get; set; }
        void OnlyForSearch();
        void HideFooter();
        void LookupInitializer(ILookup<TEntity> lookup);
        bool IsLookup();
        Task SelectItemOnLookup();
    }

    public interface IUIService
    {
        Task OpenWindow(int? id);
        Task CloseWindow();
        int MasterId { get; set; }
    }

    internal interface IInternalUIService : IUIService
    {
        void WindowInitializer(Window window);

        Window Window { get; }

        /// <summary>
        /// It's Used in One-To-One Relationship.For Example in Employee(Entity) and Address(Other Type) 
        /// </summary>
        Type OtherType { get; set; }

        /// <summary>
        /// It's used in Master-Details CRUD when we save only Single detail we use this to set foreign key property
        /// </summary>
        int InternalMasterId { get; set; }

        /// <summary>
        /// It's used in Master-Details CRUD when we save only Single detail we use this to find foreign key property
        /// </summary>
        Type InternalMasterType { get; set; }
    }

    internal interface IInternalUIService<TEntity> : IInternalUIService, IInternalSearchService<TEntity>, IUIService<TEntity>  where TEntity : class
    {
        void FormInitializer(CaspianForm<TEntity> form);

        /// <summary>
        /// In 1 ti 1 relationship if is bigger than 0 (MasterId > 0) it fetch child from database
        /// </summary>
        /// <param name="detailType">Type of detail in 1 to 1 relationship</param>
        /// <returns></returns>
        Task UpdateChildOfModelAsync(Type detailType);
        
        void TabPanelInitializer(IEntityTabPanel tabPanel);
        
        /// <summary>
        /// Its fetch UpsertData from database by using MasterId as primary key
        /// </summary>
        /// <returns></returns>
        Task FetchAsync();
        void StateHasChanged();
    }

    public interface IUIService<TEntity> : IUIService, ISearchService<TEntity> where TEntity : class
    {
        CaspianForm<TEntity> Form { get;}

        TEntity UpsertData { get; }

        IEntityTabPanel EntityTabPanel { get; }

        Func<IServiceProvider, TEntity, Task<bool>> OnUpsert { get; set; }

        void CaspianValidationValidatorInitializer(CaspianValidationValidator<TEntity> validator);
    }
}
