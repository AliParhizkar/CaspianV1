using System;
using Caspian.Common;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Rendering;

namespace Caspian.UI
{
    public class BasePage: ComponentBase
    {
        string message;
        bool? block;
        protected MessageBox MessageBox;
        protected bool sholdRender = true;
        
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

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
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
