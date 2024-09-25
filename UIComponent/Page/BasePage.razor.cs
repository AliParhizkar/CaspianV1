using Caspian.Common;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using System.Reflection;

namespace Caspian.UI
{
    public partial class BasePage: ComponentBase, IDisposable
    {
        string message;
        protected MessageBox MessageBox;
        bool sholdRender = true;
        BasePage child;
        
        public static bool IsStarted { get; set; }

        [CascadingParameter]
        public ClassNameContainner ClassNameContainner { get; set; }

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
                var path = new Uri(NavigationManager.Uri).AbsolutePath;
                if (path.StartsWith("/Demo", StringComparison.OrdinalIgnoreCase))
                    DataService.Language = Language.En;
                else
                    DataService.Language = Language.Fa;
                DataService.UserId = UserId;
            }
            await base.OnInitializedAsync();
        }

        protected IServiceScope CreateScope()
        {
            var scope = ServiceScopeFactory.CreateScope();
            scope.GetService<CaspianDataService>().UserId = UserId;
            return scope;
        }

        public void ShowMessage(string msg)
        {
            message = msg;
        }

        public async Task Alert(string message)
        {
            var window = Service.Peek();
            if (window != null)
                await window.GetMessageBox().Alert(message);
            else
                await MessageBox.Alert(message);
        }

        public async Task<bool> Confirm(string message)
        {
            var window = Service.Peek();
            if (window != null)
                return await window.GetMessageBox().Confirm(message);
            return await MessageBox.Confirm(message);
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
            await jsRuntime.InvokeVoidAsync("caspian.common.bindFileDownload", fileName, streamRef);
        }

        protected override void OnInitialized()
        {
            if (ClassNameContainner != null)
                ClassNameContainner.ClassName = this.GetType().Name;
            BaseService.Target = this;
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
            this.child = child;
            await jsRuntime.InvokeVoidAsync("caspian.common.bindWindowClick", DotNetObjectReference.Create(child));
        }

        protected virtual void OnWindowClick()
        {

        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (message.HasValue())
            {
                await jsRuntime.InvokeVoidAsync("caspian.common.showMessage", message);
                message = null;
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            
        }
    }
}
