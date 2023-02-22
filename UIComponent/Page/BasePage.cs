using System;
using System.IO;
using System.Linq;
using Caspian.Common;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Authorization;

namespace Caspian.UI
{
    public class BasePage: ComponentBase
    {
        string message;
        bool? block;
        protected MessageBox MessageBox;
        protected bool sholdRender = true;
        BasePage child;
        
        public static bool IsStarted { get; set; }

        [Inject]
        public IServiceScopeFactory ServiceScopeFactory { get; set; }

        [Inject]
        public CaspianDataService DataService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IJSRuntime jsRuntime { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }

        public int UserId { get; private set; }

        protected async override Task OnInitializedAsync()
        {
            if (authenticationStateTask != null)
            {
                var result = await authenticationStateTask;
                UserId = Convert.ToInt32(result.User.Claims.FirstOrDefault()?.Value);
                DataService.UserId = UserId;
            }
            await base.OnInitializedAsync();
        }

        /// <summary>
        /// Create in OnInitialized and dispose in OnAfterRenderAsync
        /// </summary>
        public IServiceScope GlobalScope { get; private set; }

        protected IServiceScope CreateScope()
        {
            var scope = ServiceScopeFactory.CreateScope();
            scope.ServiceProvider.GetService<CaspianDataService>().UserId = UserId;
            return scope;
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

        public async Task Alert(string message)
        {
            await MessageBox.Alert(message);
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
            if (sholdRender)
                return true;
            sholdRender = true;
            return false;
        }

        protected async Task DownloadFile(string fileName, byte[] fileContent)
        {
            using var memoryStream = new MemoryStream(fileContent);
            await DownloadFile(fileName, memoryStream);
        }

        protected async Task DownloadFile(string fileName, MemoryStream fileContent)
        {
            using var streamRef = new DotNetStreamReference(fileContent);
            await jsRuntime.InvokeVoidAsync("$.telerik.bindFileDownload", fileName, streamRef);
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
            if (child != null)
                child.OnWindowClick();
        }

        /// <summary>
        /// if add auto-hide class to DOM element, then DOM element and all children click not send to server
        /// </summary>
        protected async Task EnableWindowClick(BasePage child)
        {
            await jsRuntime.InvokeVoidAsync("$.telerik.bindWindowClick", DotNetObjectReference.Create(child));
        }

        protected virtual void OnWindowClick()
        {

        }

        protected async Task BindTooltip()
        {
            await jsRuntime.InvokeVoidAsync("$.telerik.bindTooltip");
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
