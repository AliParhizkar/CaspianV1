using Caspian.Common;
using Microsoft.JSInterop;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class Window: ComponentBase
    {
        MessageBox messageBox;
        bool isOpend;
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
        public ISimpleService Service { get; set; }

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
        public bool ParentChaild { get; set; }

        [CascadingParameter]
        internal ICrudComponent CrudComponent { get; set; }

        public async Task Open()
        {
            isOpend = true;
            Status = WindowStatus.Open;
            if (StatusChanged.HasDelegate)
                await StatusChanged.InvokeAsync(WindowStatus.Open);
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
            }
            if (StatusChanged.HasDelegate)
                await StatusChanged.InvokeAsync(WindowStatus.Close);
        }

        protected override void OnInitialized()
        {
            if (CrudComponent != null)
                CrudComponent.SetWindow(this);
            if (Service != null)
            {
                Service.Window = this;
                Service.WindowInitialize();
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
                isOpend = true;
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
                await jsRuntime.InvokeVoidAsync("$.caspian.bindWindow", DotNetObjectReference.Create(this), window);
            }
            if (isOpend)
            {
                isOpend = false;
                if (OnInternalOpen.HasDelegate)
                    await OnInternalOpen.InvokeAsync();
                if (OnOpen.HasDelegate)
                    await OnOpen.InvokeAsync();
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public void Dispose()
        {
            Service?.ClearWindow();
        }
    }
}
