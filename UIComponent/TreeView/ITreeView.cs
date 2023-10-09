using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}
