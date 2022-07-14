using System;
using Caspian.Common;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Rendering;
using System.Collections.Generic;

namespace Caspian.UI
{
    public class BasePage: ComponentBase
    {
        string message;
        bool? block;
        protected MessageBox MessageBox;
        protected bool sholdRender = true;
        bool enableWindowClick;
        BasePage child;
        
        public static bool IsStarted { get; set; }

        [Inject]
        public IServiceScopeFactory ServiceScopeFactory { get; set; }

        [Inject]
        protected IJSRuntime jsRuntime { get; set; }

        /// <summary>
        /// Create in OnInitialized and dispose in OnAfterRenderAsync
        /// </summary>
        public IServiceScope GlobalScope { get; private set; }

        protected IServiceScope CreateScope()
        {
            return ServiceScopeFactory.CreateScope();
        }

        public void ShowMessage(string msg)
        {
            message = msg;
        }

        public void BlockUI()
        {
            block = true;
        }

        public void UnblockUI()
        {
            block = false;
        }

        public void Alert(string message)
        {
            MessageBox.Alert(message);
        }

        public async Task<bool> Confirm(string message)
        {
            return await MessageBox.Confirm(message);
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent(1, typeof(MessageBox));
            builder.AddComponentReferenceCapture(1, capturedRef => 
            { 
                MessageBox = capturedRef as MessageBox; 
            });
            builder.CloseComponent();
            base.BuildRenderTree(builder);
        }

        protected IList<SelectListItem> GetSelectList(params string[] array)
        {
            var list = new List<SelectListItem>();
            var index = 1;
            foreach (var item in array)
            {
                list.Add(new SelectListItem(index.ToString(), item));
                index++;
            }
            return list;
        }

        protected override bool ShouldRender()
        {
            return sholdRender;
        }

        protected override void OnInitialized()
        {
            if (GlobalScope == null)
                GlobalScope = CreateScope();
            base.OnInitialized();
        }

        [JSInvokable]
        public void WindowClick()
        {
            child.OnWindowClick();
        }

        /// <summary>
        /// if add auto-hide class to DOM element, then DOM element and all children click not send to server
        /// </summary>
        protected void EnableWindowClick(BasePage child)
        {
            this.child = child;
            enableWindowClick = true;
        }

        protected virtual void OnWindowClick()
        {

        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && enableWindowClick)
            {
                enableWindowClick = false;
                await jsRuntime.InvokeVoidAsync("$.telerik.bindWindowClick", DotNetObjectReference.Create(this));
            }
            if (message.HasValue())
            {
                await jsRuntime.InvokeVoidAsync("$.telerik.showMessage", message);
                message = null;
            }
            if (block.HasValue)
            {
                var tempBlock = block.Value;
                await jsRuntime.InvokeVoidAsync("$.telerik.blockManagement", tempBlock);
            }
            if (block == true)
                block = false;
            else if (block == false)
                block = null;
            if (GlobalScope != null)
            {
                GlobalScope.Dispose();
                GlobalScope = CreateScope();
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }

    internal enum MessageType
    {
        Info = 1,

        Quession,
    }
}
