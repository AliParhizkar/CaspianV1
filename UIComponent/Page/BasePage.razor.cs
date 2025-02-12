using Caspian.Common;
using System.Reflection;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Caspian.UI
{
    public partial class BasePage: ComponentBase, IDisposable
    {
        string message;
        protected MessageBox MessageBox;
        bool sholdRender = true;
        BasePage child;
        
        public static bool IsStarted { get; set; }

        [Inject]
        public IServiceScopeFactory ServiceScopeFactory { get; set; }

        [Inject]
        public IServiceProvider ServiceProvider { get; set; }

        [Inject]
        public CaspianDataService DataService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IJSRuntime jsRuntime { get; set; }

        [CascadingParameter]
        public PageData PageData { get; set; }

        public int UserId 
        { 
            get
            {
                if (PageData == null)
                    return 0;
                return PageData.UserId;
            }
                
        }

        protected override void OnInitialized()
        {
            ServiceProvider.GetService<CaspianDataService>().UserId = UserId;
            base.OnInitialized();
        }

        protected IServiceScope CreateScope()
        {
            var scope = ServiceScopeFactory.CreateScope();
            scope.SetUserId(UserId);
            return scope;
        }

        public void ShowMessage(string msg)
        {
            message = msg;
        }

        public async Task Alert(string message)
        {
            var window = PageService.Peek();
            if (window != null)
                await window.GetMessageBox().Alert(message);
            else
                await MessageBox.Alert(message);
        }

        public async Task<bool> Confirm(string message)
        {
            var window = PageService.Peek();
            if (window != null)
                return await window.GetMessageBox().Confirm(message);
            return await MessageBox.Confirm(message);
        }

        virtual internal protected void ChangeState()
        {
            StateHasChanged();
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

        protected override void OnParametersSet()
        {
            ComponentService.Target = this;
            base.OnParametersSet();
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
            ComponentService.Target = this;
            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            foreach(var info in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (info.PropertyType.GetInterfaces().Contains(typeof(IUIService)))
                    (info.GetValue(this) as IUIService).Dispose();
            }
        }
    }
}
