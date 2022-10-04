using System;
using System.Linq;
using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class CTreeView<TEntity>: ComponentBase where TEntity: class
    {
        ElementReference tree;
        IList<TEntity> Items;

        [CascadingParameter(Name = "TreeViewCascadeData")]
        public TreeViewCascadeData CascadeData { get; set; }

        [Parameter]
        public RenderFragment<TreeViewItem> Template { get; set; }

        [Parameter]
        public RenderFragment<TreeViewItem> ChildContent { get; set; }

        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> Attrs { get; set; }

        [Parameter]
        public Func<TEntity, string> TextFunc { get; set; }

        [Parameter]
        public Func<TEntity, bool> FilterFunc { get; set; }

        [Parameter]
        public Expression<Func<TEntity, bool>> ConditionExpression { get; set; }

        [Parameter]
        public Expression<Func<TEntity, object>> OrderByExpression { get; set; }

        [Parameter]
        public bool Selectable { get; set; }

        [Parameter]
        public IList<TreeViewItem> Source { get; set; }

        [Parameter]
        public EventCallback<TreeViewItem> OnExpanded { get; set; }

        [Parameter]
        public EventCallback<TreeViewItem> OnCollapsed { get; set; }

        [Parameter]
        public EventCallback<TreeViewItem> OnSelected { get; set; }

        [Parameter]
        public EventCallback<TreeViewItem> OnChange { get; set; }

        [Parameter]
        public bool SingleSelectOnTree { get; set; }

        [Parameter]
        public Func<TEntity, bool> ParentNodeFilterFunc { get; set; }

        void UnselectTree(IEnumerable<TreeViewItem> nodes)
        {
            foreach (var node in nodes)
            {
                node.Selected = false;
                if (node.Items != null)
                    UnselectTree(node.Items);
            }
        }

        async Task DataBinding()
        {
            if (Source == null && typeof(TEntity) == typeof(TreeViewItem))
                Source = new List<TreeViewItem>();
            if (Source == null)
            {
                using var scope = ServiceScopeFactory.CreateScope();
                var service = new SimpleService<TEntity>(scope);
                var contextType = new AssemblyInfo().GetDbContextType(typeof(TEntity));
                var query = service.GetAll(default(TEntity));
                if (ConditionExpression != null)
                    query = query.Where(ConditionExpression);
                var dataList = await query.ToListAsync();
                Items = dataList.Where(ParentNodeFilterFunc).ToList();
            }
        }

        public async Task Reload()
        {
            await DataBinding();
        }

        void GetSeletcedItems(TreeViewItem item, IList<TreeViewItem> list)
        {
            if (item.Selected == true)
                list.Add(item);
            if (item.Items != null)
            {
                foreach (var item1 in item.Items)
                    GetSeletcedItems(item1, list);

            }
        }

        public IList<TreeViewItem> GetSeletcedItems()
        {
            var list = new List<TreeViewItem>();
            foreach (var item in Source)
                GetSeletcedItems(item, list);
            return list;
        }

        protected override async Task OnInitializedAsync()
        {
            await DataBinding();
            await base.OnInitializedAsync();
        }

        protected override void OnParametersSet()
        {
            if (CascadeData == null)
                CascadeData = new TreeViewCascadeData();
            CascadeData.Template = Template;
            base.OnParametersSet();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await JSRuntime.InvokeVoidAsync("$.telerik.bindTree", tree);
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
