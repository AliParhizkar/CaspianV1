using Caspian.Common;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class Window: ComponentBase
    {
        MessageBox messageBox;
        bool isOpened;
        WindowStatus oldStatus;
        Dictionary<string, object> attrs = new Dictionary<string, object>();

        public ElementReference window { get; private set; }

        [CascadingParameter(Name = "ParentWindow")]
        public IWindow ParentWindow { get; set; }

        [Parameter,]
        public string Id { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment Content { get; set; }

        [Parameter]
        public IUIService Service { get; set; }

        [Parameter]
        public RenderFragment HeaderTemplate { get; set; }

        [Parameter]
        public bool ShowCustomHeader { get; set; }

        [Parameter]
        public WindowStatus Status { get; set; } = WindowStatus.Close;

        [Parameter]
        public EventCallback<WindowStatus> StatusChanged { get; set; }

        [Parameter]
        public Expression<Func<WindowStatus>> StatusExpression { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback OnOpen { get; set; }

        internal EventCallback OnInternalOpen { get; set; }

        internal EventCallback OnInternalClose { get; set; }

        [Parameter]
        public bool Modal { get; set; } = true;

        [Parameter]
        public bool Resizable { get; set; }

        [Parameter]
        public bool Draggable { get; set; }

        [Parameter]
        public bool ParentChild { get; set; }

        public async Task Open()
        {
            isOpened = true;
            Status = WindowStatus.Open;
            if (StatusChanged.HasDelegate)
                await StatusChanged.InvokeAsync(WindowStatus.Open);
            if (OnOpen.HasDelegate) 
                await OnOpen.InvokeAsync();
        }

        public async Task SetValue(object value)
        {
            await Close();
        }

        public async Task Close()
        {
            if (Status == WindowStatus.Open)
            {
                Status = WindowStatus.Close;
                if (OnInternalClose.HasDelegate)
                    await OnInternalClose.InvokeAsync();
                if (StatusChanged.HasDelegate)
                    await StatusChanged.InvokeAsync(WindowStatus.Close);
            }
        }

        protected override void OnInitialized()
        {
            if (Service != null)
            {
                var service = Service as IInternalUIService;
                service.WindowInitialize(this);
            }
            Modal = true;
            Draggable = true;
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            if (Style.HasValue())
                attrs["style"] = Style;
            if (Id.HasValue())
                attrs["id"] = Id;
            base.OnParametersSet();
        }

        public MessageBox GetMessageBox()
        {
            return messageBox;
        }

        protected async override Task OnParametersSetAsync()
        {
            if (Status == WindowStatus.Open && oldStatus != WindowStatus.Open)
            {
                isOpened = true;
                PageService.Push(this);
            }
            if (Status != WindowStatus.Open && oldStatus == WindowStatus.Open)
            {
                PageService.Pop();
                messageBox = null;
            }
            oldStatus = Status;
            await base.OnParametersSetAsync();
        }

        protected async override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var dotnet = DotNetObjectReference.Create(this);
                await jsRuntime.InvokeVoidAsync("caspian.common.bindWindow", window, dotnet);
            }
            if (isOpened && OnOpen.HasDelegate)
            {
                isOpened = false;
                await OnOpen.InvokeAsync();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            (Service as IInternalUIService)?.WindowInitialize(null);
        }
    }
}
