using Caspian.Common;
using Microsoft.JSInterop;
using Caspian.Common.Service;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Caspian.Common.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class TreeView<TEntity>: ComponentBase, ITreeView where TEntity: class
    {
        ElementReference tree;
        IList<NodeView> treeNodes;
        Func<TEntity, bool> parentNodeFilterFunc;

        public EventCallback<NodeView> OnInternalCHanged { get; set; }

        public EventCallback<NodeView> OnInternalClicked { get; set; }

        [CascadingParameter]
        internal IAutoCompleteTree AutoComplete { get; set; }

        [Parameter]
        public RenderFragment<NodeView> BeforeNodeTemplate { get; set; }

        [Parameter]
        public RenderFragment<NodeView> AfterNodeTemplate { get; set; }

        [Parameter]
        public IList<string> SelectedNodesValue { get; set; }

        [Parameter]
        public RenderFragment<NodeView> ChildContent { get; set; }

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
        public IList<NodeView> Source { get; set; }

        [Parameter]
        public EventCallback<NodeView> OnExpanded { get; set; }

        [Parameter]
        public EventCallback<NodeView> OnCollapsed { get; set; }

        [Parameter]
        public EventCallback<NodeView> OnSelected { get; set; }

        [Parameter]
        public EventCallback<NodeView> OnChange { get; set; }

        [Parameter]
        public bool SingleSelectOnTree { get; set; }

        [Parameter]
        public Func<TEntity, bool> ParentNodeFilterFunc { get; set; }

        [Parameter]
        public bool AutoSelectable { get; set; }

        void UpdateSelectionForChilren(NodeView node)
        {
            if (node.Children != null)
            {
                foreach(var child in node.Children)
                {
                    child.Selected = node.Selected;
                    UpdateSelectionForChilren(child);
                }
            }
        }

        void UpdateSelectionForParent(NodeView node)
        {
            var parent = node.Parent;
            if (parent != null)
            {
                var siblings = parent.Children;
                if (siblings.All(t => t.Selected == true))
                    parent.Selected = true;
                else if (siblings.All(t => t.Selected == false))
                    parent.Selected = false;
                else
                    parent.Selected = null;
            }
        }

        void UnselectTree(IEnumerable<NodeView> nodes)
        {
            foreach (var node in nodes)
            {
                node.Selected = false;
                if (node.Children != null)
                    UnselectTree(node.Children);
            }
        }

        async Task NodeCollapsed(NodeView node)
        {
            if (OnCollapsed.HasDelegate)
                await OnCollapsed.InvokeAsync(node);
        }

        async Task NodeExpanded(NodeView node)
        {
            if (OnExpanded.HasDelegate)
                await OnExpanded.InvokeAsync(node);
            StateHasChanged();
        }

        async Task NodeSelected(NodeView node)
        {
            if (SingleSelectOnTree)
            {
                UnselectTree(Source);
                node.Selected = true;
            } 
            await OnSelected.InvokeAsync(node);
        }

        async Task DataBindingAsync()
        {
            if (Source == null && typeof(TEntity) != typeof(NodeView))
            {
                using var scope = ServiceScopeFactory.CreateScope();
                var service = new BaseService<TEntity>(scope.ServiceProvider);
                var contextType = new AssemblyInfo().GetDbContextType(typeof(TEntity));
                var query = service.GetAll(default(TEntity));
                if (ConditionExpression != null)
                    query = query.Where(ConditionExpression);
                var dataList = await query.ToListAsync();
                var items = dataList.Where(parentNodeFilterFunc).ToList();
                var tree = new HierarchyTree<TEntity>();
                tree.TextFunc = TextFunc;
                var multSelect = false;
                if (AutoComplete != null)
                    multSelect = AutoComplete.MultiSelecable();
                if (FilterFunc == null)
                    treeNodes = tree.CreateTree(items, multSelect);
                else
                {
                    tree.FilterFunc = FilterFunc;
                    treeNodes = tree.FilterTree(items, multSelect);
                }
                if (SelectedNodesValue != null)
                    tree.UpdateSelectedState(treeNodes, SelectedNodesValue);
            }
        }

        public bool MultiSelectable
        {
            get { return Selectable; }
            set { Selectable = value; }
        }

        async Task OnNodeChanged(NodeView node)
        {
            if (node.Children != null)
            {
                foreach(var item in node.Children)
                {
                    if (item.Depth == null)
                        item.Depth = (byte)(node.Depth.Value + 1);
                }
            }
            if (AutoSelectable) 
            {
                UpdateSelectionForChilren(node);
                UpdateSelectionForParent(node);
            }
            await OnChange.InvokeAsync(node);
            await OnInternalCHanged.InvokeAsync(node);
        }

        async Task SelectStateChanged(NodeView node)
        {
            await OnInternalCHanged.InvokeAsync(node);
        }

        async Task NodeClicked(NodeView node)
        {
            await OnInternalClicked.InvokeAsync(node);
        }

        protected override void OnInitialized()
        {
            AutoComplete?.SetTreeView(this);
            if (typeof(TEntity) != typeof(NodeView))
            {
                var info = typeof(TEntity).GetForeignKey(typeof(TEntity));
                if (info != null)
                {
                    parentNodeFilterFunc = entity =>
                    {
                        return info.GetValue(entity) == null;
                    };
                }
            }
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
                    node.Parent.Children.Remove(node);
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
                node = new NodeView()
                {
                    Collabsable = true,
                    Depth = parentNode?.Depth == null ? (byte)1 : (byte)(parentNode.Depth + 1),
                    Expanded = true,
                    Parent = parentNode,
                    Text = TextFunc.Invoke(entity),
                    Value = value
                };
                if (parentNode == null)
                    treeNodes.Add(node);
                else
                {
                    parentNode.Expanded = true;
                    if (parentNode.Children == null)
                        parentNode.Children = new List<NodeView>();
                    parentNode.Children.Add(node);
                }
            }
            else
                node.Text = TextFunc.Invoke(entity);//Ä
        }

        public async Task ReloadAsync()
        {
            await DataBindingAsync();
        }

        void GetSeletcedItems(NodeView node, IList<NodeView> list)
        {
            if (node.Selected == true)
                list.Add(node);
            if (node.Children != null)
            {
                foreach (var child in node.Children)
                    GetSeletcedItems(child, list);
            }
        }

        public IList<NodeView> GetSeletcedItems()
        {
            var nodes = new List<NodeView>();
            if (Source != null)
            {
                foreach (var item in Source)
                    GetSeletcedItems(item, nodes);
            }
            else if (treeNodes != null)
            {
                foreach (var item in treeNodes)
                    GetSeletcedItems(item, nodes);
            }
            return nodes;
        }

        protected override async Task OnInitializedAsync()
        {
            await DataBindingAsync();
            await base.OnInitializedAsync();
        }

        protected override void OnParametersSet()
        {
            if (ParentNodeFilterFunc != null)
                parentNodeFilterFunc = ParentNodeFilterFunc;
            base.OnParametersSet();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await JSRuntime.InvokeVoidAsync("$.caspian.bindTree", tree);
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
