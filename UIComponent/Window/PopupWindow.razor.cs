﻿using System.Text.Json;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace Caspian.UI
{
    public partial class PopupWindow: ComponentBase
    {
        string className;
        ElementReference element;

        protected override void OnInitialized()
        {
            Status = WindowStatus.Close;

            base.OnInitialized();
        }

        [Parameter]
        public WindowStatus Status { get; set; }

        [Parameter]
        public EventCallback<WindowStatus> StatusChanged { get; set; }

        [Parameter]
        public VerticalAlign VerticalAlign { get; set; } = VerticalAlign.Top;

        [Parameter]
        public HorizontalAlign HorizontalAlign { get; set; } = HorizontalAlign.Center;

        [Parameter]
        public VerticalAnchor VerticalAnchor { get; set; } = VerticalAnchor.Top;

        [Parameter]
        public HorizontalAnchor HorizontalAnchor { get; set; } = HorizontalAnchor.Center;

        [Parameter]
        public VerticalAnchor TargetVerticalAnchor { get; set; } = VerticalAnchor.Bottom;

        [Parameter]
        public HorizontalAnchor TargetHorizontalAnchor { get; set; } = HorizontalAnchor.Center;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public int? Left { get; set; }

        [Parameter]
        public int? Right { get; set; }

        [Parameter]
        public int? Top { get; set; }

        [Parameter]
        public int? Bottom { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public bool AutoHide { get; set; } = true;

        [Parameter]
        public int OffsetLeft { get; set; }

        [Parameter]
        public int OffsetTop { get; set; }

        [Parameter]
        public ElementReference? TargetElement { get; set; }

        [JSInvokable]
        public async Task Close()
        {
            if (AutoHide && StatusChanged.HasDelegate)
            {
                //Delay to Value of control set in binding after change
                await Task.Delay(100);
                await StatusChanged.InvokeAsync(WindowStatus.Close);
            }
        }

        protected override void OnParametersSet()
        {
            className = "auto-hide c-popup-window";
            switch (VerticalAlign)
            {
                case VerticalAlign.Top:
                    className += " c-top";
                    break;
                case VerticalAlign.Bottom:
                    className += " c-bottom";
                    break;
                case VerticalAlign.Middle:
                    className += " c-top c-bottom";
                    break;
            }
            switch (HorizontalAlign)
            {
                case HorizontalAlign.Left:
                    className += " c-left";
                    break;
                case HorizontalAlign.Right:
                    className += " c-right";
                    break;
                case HorizontalAlign.Center:
                    className += " c-left c-right";
                    break;
            }
            base.OnParametersSet();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (Status == WindowStatus.Open)
            {
                string json = JsonSerializer.Serialize(new
                {
                    left = Left,
                    right = Right,
                    top = Top,
                    bottom = Bottom,
                    targetHorizontalAnchor = TargetHorizontalAnchor,
                    targetVerticalAnchor = TargetVerticalAnchor,
                    horizontalAnchor = HorizontalAnchor,
                    verticalAnchor = VerticalAnchor,
                    offsetLeft = OffsetLeft,
                    offsetTop = OffsetTop
                });
                await jSRuntime.InvokeVoidAsync("caspian.common.bindPopupWindow", element, TargetElement, json, DotNetObjectReference.Create(this));
            }
            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
