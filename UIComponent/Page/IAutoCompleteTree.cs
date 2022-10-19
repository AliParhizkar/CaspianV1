using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    internal interface IAutoCompleteTree
    {
        Task SetValueAsync(TreeViewItem node);

        Task SetValuesAsync(IList<TreeViewItem> nodes);

        bool MultiSelecable();

        IList<object> SelectedNodesValue();

        EventCallback OnInternalClose { get; set; }

        EventCallback<string> OnInternalChanged { get; set; }
    }
}
