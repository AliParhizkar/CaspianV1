using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public interface ITreeView
    {
        Task ReloadAsync();

        /// <summary>
        /// Use For multi select nodes
        /// </summary>
        EventCallback<NodeView> OnInternalCHanged { get; set; }

        /// <summary>
        /// Use For single select nodes
        /// </summary>
        EventCallback<NodeView> OnInternalClicked { get; set; }

        IList<NodeView> GetSeletcedItems();

        void SetSelectedNodesValue(IList<string> values);

        bool MultiSelectable { get; set; }

        TreeNode ClickedNode { get; set; }

        RenderFragment<NodeView> BeforeNodeTemplate { get; set; }

        RenderFragment<NodeView> AfterNodeTemplate { get; set; }
    }
}
