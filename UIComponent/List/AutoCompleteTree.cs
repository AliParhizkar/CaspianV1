using System;
using System.Linq;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Caspian.Common.Extension;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Xml.Linq;
using System.Linq.Dynamic.Core;

namespace Caspian.UI
{
    public partial class AutoCompleteTree<TValue>: ComponentBase 
    {
        bool show;
        bool multiSelectable;
        ElementReference input;
        string searchText;
        async Task ShowTree()
        {
            show = true;
            await jSRuntime.InvokeVoidAsync("$.telerik.enableAutoHide", DotNetObjectReference.Create(this));
        }

        protected override void OnInitialized()
        {
            var type = typeof(TValue).GetUnderlyingType();
            if (type.IsArray || type.IsCollectionType())
                multiSelectable = true;
            base.OnInitialized();
        }

        async Task setValue(ChangeEventArgs e)
        {
            searchText = Convert.ToString(e.Value);
            if (OnInternalChanged.HasDelegate)
                await OnInternalChanged.InvokeAsync(searchText);
            if (show == false)
                await ShowTree();
        }

        public EventCallback<string> OnInternalChanged { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public TValue Value { get; set; }

        [Parameter]
        public EventCallback<TValue> ValueChanged { get; set; }

        public bool MultiSelecable()
        {
            return multiSelectable;
        }

        public IList<object> SelectedNodesValue()
        {
            var array = (Array)(Convert.ChangeType(Value, typeof(TValue)));
            if (array == null)
                return null;
            return array.ToDynamicList();
        }

        public async Task SetValueAsync(TreeViewItem node)
        {
            var type = typeof(TValue).GetUnderlyingType();
            Value = (TValue)Convert.ChangeType(node.Value, type);
            searchText = node.Text;
            show = false;
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(Value);
        }

        public async Task SetValuesAsync(IList<TreeViewItem> nodes)
        {
            var type = typeof(TValue);
            if (type.IsArray)
                type = type.GetElementType();
            type = type.GetUnderlyingType();
            var arrayList = new ArrayList(nodes.Count);
            foreach (var node in nodes)
                arrayList.Add(Convert.ChangeType(node.Value, type));
            var array = arrayList.ToArray(type);
            Value = (TValue)(Convert.ChangeType(array, typeof(TValue)));
            searchText = nodes.Count + " آیتم انتخاب شده است";
            show = false;
            if (ValueChanged.HasDelegate)
                await ValueChanged.InvokeAsync(Value);
        }

        public EventCallback OnInternalClose { get; set; }

        [JSInvokable]
        public async Task HideForm()
        {
            show = false;
            if (OnInternalClose.HasDelegate)
                await OnInternalClose.InvokeAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await jSRuntime.InvokeVoidAsync("$.telerik.bindLookupTree", input);
            await base.OnAfterRenderAsync(firstRender);
        }

    }
}
