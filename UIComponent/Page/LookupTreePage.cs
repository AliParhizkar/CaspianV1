using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class LookupTreePage<TEntity> : BasePage where TEntity : class
    {
        protected string SearchString;
        protected IList<object> SelectedNodesValue;

        protected CTreeView<TEntity> Tree { get; set; }

        [CascadingParameter]
        internal IAutoCompleteTree AutoComplete { get; set; }

        protected override void OnInitialized()
        {
            SelectedNodesValue = AutoComplete.SelectedNodesValue();
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            AutoComplete.OnInternalChanged = EventCallback.Factory.Create<string>(this, async value =>
            {
                SearchString = value;
                await Tree.ReloadAsync();
            });
            await base.OnInitializedAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (AutoComplete.MultiSelecable())
            {
                
                AutoComplete.OnInternalClose = EventCallback.Factory.Create(this, async item =>
                {
                    var nodes = Tree.GetSeletcedItems();
                    await AutoComplete.SetValuesAsync(nodes);

                });
            }
            else if (!Tree.OnInternalSelect.HasDelegate)
            {
                Tree.OnInternalSelect = EventCallback.Factory.Create<TreeViewItem>(this, async item =>
                {
                    await AutoComplete.SetValueAsync(item);
                });
            }
            

            base.OnAfterRender(firstRender);
        }
    }
}
