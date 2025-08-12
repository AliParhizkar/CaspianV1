using Caspian.Common;
using System.Collections;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class EntitySearch<TEntity>: IEntitySearch, ICaspianContainer where TEntity: class
    {
        IDictionary<string, SearchType> searchTypes;
        IDictionary<string, ICollection> enumValues;
        IList<ValueTypeContainer> valueTypeValues;

        protected override void OnInitialized()
        {
            searchTypes = new Dictionary<string, SearchType>();
            enumValues = new Dictionary<string, ICollection>();
            valueTypeValues = new List<ValueTypeContainer>();
            if (Service == null)
                throw new CaspianException($"خطا:EntitySearch Component should have a service of type ISearchService<{typeof(TEntity)}>");
            (Service as IInternalSearchService<TEntity>).SearchData = searchTypes;
            base.OnInitialized();
        }

        [CascadingParameter]
        internal ILookup<TEntity> Lookup { get; set; }

        void IEntitySearch.EnableLoadData()
        {
            Service.DataView.EnableLoading();
        }

        ICollection IEntitySearch.GetFieldValues(string path)
        {
            if (enumValues.ContainsKey(path))
                return enumValues[path];
            return null;
        }

        bool IEntitySearch.ChangEnumValues(string path, System.Collections.ICollection values)
        {
            ICollection oldValues = new object[0];
            if (enumValues.ContainsKey(path))
                oldValues = enumValues[path];
            if (values.Count == 0)
                enumValues.Remove(path);
            else
                enumValues[path] = values;
            var isEqual = true;
            var oldCount = oldValues?.Count ?? 0;
            if (oldCount == values.Count)
            {
                if (oldValues != null)
                {
                    foreach (var value in values)
                    {
                        if (!oldValues.ToDynamicList().Contains(value))
                        {
                            isEqual = false;
                            break;
                        }
                    }
                }
            }
            else
                isEqual = false;
            if (!isEqual)
                (Service as IInternalSearchService<TEntity>).EnumFields = enumValues;
            return !isEqual;
        }

        async Task IEntitySearch.SearchAsync()
        {
            await Service.DataView.ReloadAsync();
        }

        async Task IEntitySearch.SelectNextRow()
        {
            var grid = Service.DataView as DataGrid<TEntity>;
            await grid.SelectNextRow();
            grid.ChangeState();
        }

        async Task IEntitySearch.SelectPreRow()
        {
            var grid = Service.DataView as DataGrid<TEntity>;
            await grid.SelectPrevRow();
            grid.ChangeState();
        }

        async Task IEntitySearch.SelectRow()
        {
            await (Service as IInternalSearchService<TEntity>).SelectItemOnLookup();
        }


        [CascadingParameter]
        internal PageData PageData { get; set; }

        string ICaspianContainer.GetLabelContainerCSSClassName(int colSpan)
        {
            var className = PageData?.RightToLeft == true ? "pe-2" : "ps-2";
            className += " col-md-";
            if (ColumnsCount == 1)
                return className + (12 - colSpan);
            if (colSpan < 6)
                className += (6 - colSpan);
            else
                className += (12 - colSpan);
            return className;
        }

        Type IEntitySearch.EntityType
        {
            get
            {
                return typeof(TEntity);
            }
        }

        void IEntitySearch.SetSearchKind(string path, SearchType searchType)
        {
            if (!searchTypes.ContainsKey(path))
                searchTypes.Add(path, searchType);
        }

        [Parameter]
        public int ColumnsCount { get; set; } = 1;

        bool IEntitySearch.IsLookup { get { return (Service as IInternalSearchService<TEntity>).IsLookup(); } }

        [Parameter]
        public ISearchService<TEntity> Service { get; set; }

        [Parameter]
        public RenderFragment<TEntity> ChildContent { get; set; }
    }
}
