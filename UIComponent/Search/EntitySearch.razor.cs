using System.IO;
using Caspian.Common;
using System.Collections;
using System.Threading.Tasks;
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
            
            InternalService.SearchData = searchTypes;
            base.OnInitialized();
        }

        [CascadingParameter]
        internal ILookup<TEntity> Lookup { get; set; }

        public void EnableLoadData() => Service.DataView?.EnableLoading();

        public async Task ReloadData() => await Service.DataView.ReloadAsync();

        public void SetFromValue(string path, object fromValue)
        {
            var old = valueTypeValues.SingleOrDefault(t => t.PropertyPath == path);
            if (old == null)
                valueTypeValues.Add(new ValueTypeContainer(path) {From = fromValue });
            else
            {
                if (fromValue == null)
                    valueTypeValues.Remove(old);
                else
                    old.From = fromValue;
            }
            InternalService.ValueTypes = valueTypeValues;
        }

        public void SetToValue(string path, object toValue)
        {
            var old = valueTypeValues.SingleOrDefault(t => t.PropertyPath == path);
            if (old == null)
                valueTypeValues.Add(new ValueTypeContainer(path) { To = toValue});
            else
            {
                if (toValue == null)
                    valueTypeValues.Remove(old);
                else
                    old.To = toValue;
            }
            InternalService.ValueTypes = valueTypeValues;
        }

        public void SetValue(string path, bool? value)
        {
            var old = valueTypeValues.SingleOrDefault(t => t.PropertyPath == path);
            if (old == null)
            {
                if (value.HasValue)
                    valueTypeValues.Add(new ValueTypeContainer(path) { Value = value });
            }
            else
            {
                if (value == null)
                    valueTypeValues.Remove(old);
                else
                    old.Value = value.Value;
            }
            InternalService.ValueTypes = valueTypeValues;
        }

        public ICollection GetFieldValues(string path)
        {
            if (enumValues.ContainsKey(path))
                return enumValues[path];
            return null;
        }

        public bool ChangEnumValues(string path, System.Collections.ICollection values)
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
                InternalService.EnumFields = enumValues;
            return !isEqual;
        }

        public async Task SearchAsync()
        {
            if (Service.DataView != null)
                await Service.DataView.ReloadAsync();
        }

        public async Task SelectNextRow()
        {
            var grid = Service.DataView as DataGrid<TEntity>;
            await grid.SelectNextRow();
            grid.ChangeState();
        }

        public async Task SelectPreRow()
        {
            var grid = Service.DataView as DataGrid<TEntity>;
            await grid.SelectPrevRow();
            grid.ChangeState();
        }

        public async Task SelectRow() => await InternalService.SelectItemOnLookup();

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

        public Type EntityType { get { return typeof(TEntity); } }

        public void SetSearchKind(string path, SearchType searchType)
        {
            if (!searchTypes.ContainsKey(path))
                searchTypes.Add(path, searchType);
        }

        [Parameter]
        public int ColumnsCount { get; set; } = 1;

        public bool IsLookup { get { return InternalService.IsLookup(); } }

        [Parameter]
        public ISearchService<TEntity> Service { get; set; }

        internal IInternalSearchService<TEntity> InternalService { get { return Service as IInternalSearchService<TEntity>; } }

        [Parameter]
        public RenderFragment<TEntity> ChildContent { get; set; }
    }
}
