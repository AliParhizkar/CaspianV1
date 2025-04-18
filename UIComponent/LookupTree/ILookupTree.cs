using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    internal interface ILookupTree
    {
        Task SetValueAsync(NodeView node);

        bool MultiSelectable();

        IList<string> SelectedNodesValue();

        EventCallback OnInternalClose { get; set; }

        EventCallback OnInternalShow { get; set; }

        EventCallback<string> OnInternalChanged { get; set; }

        void SetTreeView(ITreeView treeView);
    }
}
