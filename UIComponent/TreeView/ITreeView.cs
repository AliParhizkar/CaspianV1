using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public interface ITreeView
    {
        Task ReloadAsync();

        EventCallback<NodeView> OnInternalCHanged { get; set; }

        EventCallback<NodeView> OnInternalClicked { get; set; }

        IList<NodeView> GetSeletcedItems();

        void SetSelectedNodesValue(IList<string> values);

        bool MultiSelectable { get; set; }

        RenderFragment<NodeView> BeforeNodeTemplate { get; set; }

        RenderFragment<NodeView> AfterNodeTemplate { get; set; }
    }
}
