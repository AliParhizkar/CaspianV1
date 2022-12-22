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
    public partial class CTreeView<TEntity>: ComponentBase, ITreeView where TEntity: class
    {
        ElementReference tree;
        IList<TreeViewItem> treeNodes;

        public EventCallback<TreeViewItem> OnInternalCHanged { get; set; }

        public EventCallback<TreeViewItem> OnInternalClicked { get; set; }

        [CascadingParameter(Name = "TreeViewCascadeData")]
        public TreeViewCascadeData CascadeData { get; set; }

        [CascadingParameter]
        internal IAutoCompleteTree AutoComplete { get; set; }

        [Parameter]
        public RenderFragment<TreeViewItem> Template { get; set; }

        [Parameter]
        public IList<string> SelectedNodesValue { get; set; }

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

        async Task DataBindingAsync()
        {
            if (Source == null && typeof(TEntity) == typeof(TreeViewItem))
                Source = new List<TreeViewItem>();
            if (Source == null)
            {
                using var scope = ServiceScopeFactory.CreateScope();
                var service = new SimpleService<TEntity>(scope.ServiceProvider);
                var contextType = new AssemblyInfo().GetDbContextType(typeof(TEntity));
                var query = service.GetAll(default(TEntity));
                if (ConditionExpression != null)
                    query = query.Where(ConditionExpression);
                var dataList = await query.ToListAsync();
                var items = dataList.Where(ParentNodeFilterFunc).ToList();
                var tree = new HierarchyTree<TEntity>();
                tree.TextFunc = TextFunc;
                if (FilterFunc == null)
                    treeNodes = tree.CreateTree(items, Selectable);
                else
                {
                    tree.FilterFunc = FilterFunc;
                    treeNodes = tree.FilterTree(items, Selectable);
                }
                if (SelectedNodesValue != null)
                    tree.UpdateSelectedState(treeNodes, SelectedNodesValue);
            }
        }

        async Task OnNodeChanged(TreeViewItem node)
        {
            if (node.Items != null)
            {
                foreach(var item in node.Items)
                {
                    if (item.Depth == null)
                        item.Depth = (byte)(node.Depth.Value + 1);
                }
            }
            await OnChange.InvokeAsync(node);
            await OnInternalCHanged.InvokeAsync(node);
        }

        async Task SelectStateChanged(TreeViewItem node)
        {
            await OnInternalCHanged.InvokeAsync(node);
        }

        async Task NodeClicked(TreeViewItem node)
        {
            await OnInternalClicked.InvokeAsync(node);
        }

        protected override void OnInitialized()
        {
            AutoComplete?.SetTreeView(this);
            base.OnInitialized();
        }

        public void RemoveFromTree(TEntity entity)
        {
            var value = typeof(TEntity).GetPrimaryKey().GetValue(entity).ToString();
            var tree = new HierarchyTree<TEntity>();
            var node = tree.FindNodeByValue(value, treeNodes);
            if (node != null)
            {
                if (node.Parent == null)
                    treeNodes.Remove(node);
                else
                    node.Parent.Items.Remove(node);
            }
        }

        public void SetSelectedNodesValue(IList<string> values)
        {
            SelectedNodesValue = values;
        }

        public void UpsertInTree(TEntity entity)
        {
            var value = typeof(TEntity).GetPrimaryKey().GetValue(entity).ToString();
            var tree = new HierarchyTree<TEntity>();
            var node = tree.FindNodeByValue(value, treeNodes);
            if (node == null)
            {
                var info = typeof(TEntity).GetForeignKey(typeof(TEntity));
                var parentValue = Convert.ToString(info.GetValue(entity));
                var parentNode = tree.FindNodeByValue(parentValue, treeNodes);
                node = new TreeViewItem()
                {
                    Collabsable = true,
                    Depth = parentNode?.Depth == null ? (byte)1 : (byte)(parentNode.Depth + 1),
                    Expanded = true,
                    Parent = parentNode,
                    ShowTemplate = true,
                    Text = TextFunc.Invoke(entity),
                    Value = value
                };
                if (parentNode == null)
                    treeNodes.Add(node);
                else
                {
                    parentNode.Expanded = true;
                    if (parentNode.Items == null)
                        parentNode.Items = new List<TreeViewItem>();
                    parentNode.Items.Add(node);
                }
            }
            else
                node.Text = TextFunc.Invoke(entity);//Ä
        }

        public async Task ReloadAsync()
        {
            await DataBindingAsync();
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
            if (Source != null)
            {
                foreach (var item in Source)
                    GetSeletcedItems(item, list);
            }
            else if (treeNodes != null)
            {
                foreach (var item in treeNodes)
                    GetSeletcedItems(item, list);
            }
            return list;
        }

        protected override async Task OnInitializedAsync()
        {
            await DataBindingAsync();
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
