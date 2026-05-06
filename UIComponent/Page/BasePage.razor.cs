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
        bool shouldRender = true;
        BasePage child;
        ElementReference shadowDiv;

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
        internal PageData PageData { get; set; }

        public string GuId { get; private set; }

        public ElementReference GetElementReference(string id)
        {
            return new ElementReference(id, shadowDiv.Context);
        }

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
            var type = this.GetType();
            var assemblyName = type.Assembly.GetName().Name;
            if (assemblyName != "Engine.Web")
            {
                var hasRout = type.GetCustomAttributes<RouteAttribute>().Any();
                if (!hasRout)
                    throw new CaspianException($"Only Page can inherited from BasePage Class. \"{type.Name}\": is not page(hasn't RouteAttribute)");
            }
            GuId = Guid.NewGuid().ToString();
            ServiceProvider.GetService<CaspianDataService>().UserId = UserId;
            base.OnInitialized();
        }

        public IServiceScope CreateScope()
        {
            var scope = ServiceScopeFactory.CreateScope();
            scope.SetUserId(UserId);
            return scope;
        }

        public async void ShowMessage(string message)
        {
            await jsRuntime.InvokeVoidAsync("caspian.common.showMessage", message);
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

        virtual public void ChangeState()
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
            if (shouldRender)
                return true;
            shouldRender = true;
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

        public virtual void Dispose() => PageService.Clear();
    }
}
