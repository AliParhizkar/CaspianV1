using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caspian.UI
{
    public interface ITreeView
    {
        Task ReloadAsync();

        EventCallback<TreeViewItem> OnInternalCHanged { get; set; }

        EventCallback<TreeViewItem> OnInternalClicked { get; set; }

        IList<TreeViewItem> GetSeletcedItems();

        void SetSelectedNodesValue(IList<string> values);

        bool MultiSelectable { get; set; }
    }
}
