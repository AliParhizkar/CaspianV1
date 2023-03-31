using System;
using System.Collections;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Caspian.Common.Extension;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class AutoCompleteTree<TValue>: ComponentBase 
    {
        bool show;
        bool multiSelectable;
        ElementReference input;
        string searchText;
        IList<string> selectedNodesValue;
        ITreeView treeView;
        bool valueIsUpdated;

        async Task ShowTree()
        {
            if (show == false)
            {
                show = true;
                if (OnInternalShow.HasDelegate)
                    await OnInternalShow.InvokeAsync();
            }
        }

        protected override void OnInitialized()
        {
            var type = typeof(TValue).GetUnderlyingType();
            if (type.IsArray || type.IsCollectionType())
            {
                selectedNodesValue = new List<string>();
                multiSelectable = true;
            }
            base.OnInitialized();
        }

        async Task setValue(ChangeEventArgs e)
        {
            if (treeView != null)
            {
                searchText = valueIsUpdated ? "" : Convert.ToString(e.Value);
                valueIsUpdated = false;
                if (OnInternalChanged.HasDelegate)
                    await OnInternalChanged.InvokeAsync(searchText);
                if (show == false)
                    await ShowTree();
                if (multiSelectable)
                    treeView.SetSelectedNodesValue(selectedNodesValue);
                await treeView.ReloadAsync();
            }
        }

        public EventCallback<string> OnInternalChanged { get; set; }

        public EventCallback OnInternalShow { get; set; }

        [Parameter]
        public RenderFragment<string> ChildContent { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        public bool MultiSelecable()
        {
            return multiSelectable;
        }

        public IList<string> SelectedNodesValue()
        {
            return selectedNodesValue;
        }

        public void SetTreeView(ITreeView treeView)
        {
            this.treeView = treeView;
            treeView.MultiSelectable= multiSelectable;
            if (valueIsUpdated)
            {
                valueIsUpdated = false;
                searchText = null;
                if (multiSelectable && treeView != null)
                    treeView.SetSelectedNodesValue(selectedNodesValue);
                StateHasChanged();
            }
            treeView.OnInternalCHanged = EventCallback.Factory.Create<TreeViewItem>(this, node =>
            {
                SetSelectedNodesValue(node);
            });

            treeView.OnInternalClicked = EventCallback.Factory.Create<TreeViewItem>(this, async node =>
            {
                await SetValueAsync(node);
            });
        }

        public async Task SetValueAsync(TreeViewItem node)
        {
            if (!multiSelectable)
            {
                var type = typeof(TValue).GetUnderlyingType();
                Value = (TValue)Convert.ChangeType(node.Value, type);
                searchText = node.Text;
                valueIsUpdated = true;
                if (ValueChanged.HasDelegate)
                    await ValueChanged.InvokeAsync(Value);
                await Task.Delay(200);
                show = false;
            }
        }

        public void SetSelectedNodesValue(TreeViewItem node)
        {
            if (node.Selected == true)
                selectedNodesValue.Add(node.Value);
            else if (node.Selected == false)
                selectedNodesValue.Remove(node.Value);
        }

        public async Task SetValuesAsync()
        {
            valueIsUpdated = true;
            var type = typeof(TValue);
            if (type.IsArray)
                type = type.GetElementType();
            type = type.GetUnderlyingType();
            var arrayList = new ArrayList(selectedNodesValue.Count);
            foreach (var value in selectedNodesValue)
                arrayList.Add(Convert.ChangeType(value, type));
            var array = arrayList.ToArray(type);
            Value = (TValue)(Convert.ChangeType(array, typeof(TValue)));
            valueIsUpdated = true;
            if (service.Language == Common.Language.Fa)
                searchText = $"{selectedNodesValue.Count} آیتم انتخاب شده است";
            else
                searchText = $"{selectedNodesValue.Count} item are selected";
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(Value);
        }

        public EventCallback OnInternalClose { get; set; }

        [JSInvokable]
        public async Task HideForm()
        {
            show = false;
            if (multiSelectable)
                await SetValuesAsync();
            else
                StateHasChanged();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                
                var dotnet = DotNetObjectReference.Create(this);
                await jSRuntime.InvokeVoidAsync("$.caspian.bindLookupTree", dotnet, input);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

    }
}
