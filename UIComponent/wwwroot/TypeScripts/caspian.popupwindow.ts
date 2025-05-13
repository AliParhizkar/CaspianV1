namespace caspian {
    export class PopupWindow {
        static dotnets: dotnetInvoker[] = [];
        target: HTMLElement;
        constructor(element: HTMLInputElement, target: HTMLElement, json: string, dotnet: dotnetInvoker) {
            caspian.PopupWindow.dotnets.push(dotnet);
            
            this.target = target
            element.style.display = 'block';
            let className = element.className;
            element.className = 'auto-hide c-popup-window';
            element.className = className;
            this.updateLocation(element, json);
            document.body.onmousedown = async e => {
                if ((e.target as HTMLElement).closest('.auto-hide') == null) {
                    document.body.onmousedown = null;
                    caspian.PopupWindow.dotnets.forEach(async dotnet => {
                        await dotnet.invokeMethodAsync('Close');
                    });
                    caspian.PopupWindow.dotnets = [];
                }
            }
        }

        updateLocation(element: HTMLInputElement, json: string) {
            let data = JSON.parse(json) as popupWindowData;
            if (this.target)
                this.bindTarget(element, this.target, data);
            else {
                let sidebarWidth = 0;
                if (document.getElementsByClassName('sidebar').length > 0)
                    sidebarWidth = document.getElementsByClassName('sidebar')[0].getBoundingClientRect().width;
                if (data.left != null) {
                    if (caspian.common.RightToLeft())
                        element.style.left = `${data.left}px`;
                    else
                        element.style.left = `${data.left + sidebarWidth}px`;
                    element.style.right = 'auto';
                }
                else if (data.right != null) {
                    element.style.left = 'auto';
                    if (caspian.common.RightToLeft())
                        element.style.right = `${data.right + sidebarWidth}px`;
                    else
                        element.style.right = `${data.right}px`;
                }
                if (data.top != null) {
                    element.style.top = `${data.top}px`;
                    element.style.bottom = 'auto';
                }
                else if (data.bottom != null) {
                    element.style.top = 'auto';
                    element.style.bottom = `${data.bottom}px`;
                }
            }
        }

        bindTarget(element: HTMLInputElement, target: HTMLElement, data: popupWindowData) {
            element.className = 'auto-hide c-popup-window';
            let targetLoc = target.getBoundingClientRect();
            let leftT = targetLoc.left, topT = targetLoc.top;
            let offsetLeft = data.offsetLeft, offsetTop = data.offsetTop;
            switch (data.targetHorizontalAnchor) {
                case HorizontalAnchor.Left:
                    leftT += data.offsetLeft;
                    break;
                case HorizontalAnchor.Center:
                    leftT += targetLoc.width / 2;
                    offsetLeft = 0;
                    break;
                case HorizontalAnchor.Right:
                    leftT += targetLoc.width + data.offsetLeft;
                    break;
            }
            switch (data.targetVerticalAnchor) {
                case VerticalAnchor.Top:
                    topT += data.offsetTop;
                    break;
                case VerticalAnchor.Middle:
                    topT += targetLoc.height / 2;
                    offsetTop = 0;
                    break;
                case VerticalAnchor.Bottom:
                    topT += targetLoc.height + data.offsetTop;
                    break;
            }
            let loc = element.getBoundingClientRect();
            if (data.horizontalAnchor == HorizontalAnchor.Center)
                leftT -= loc.width / 2 + data.offsetLeft - offsetLeft;
            else if (data.horizontalAnchor == HorizontalAnchor.Right)
                leftT -= loc.width - data.offsetLeft - offsetLeft;
            if (data.verticalAnchor == VerticalAnchor.Middle)
                topT -= loc.height / 2 + data.offsetTop + offsetTop;
            else if (data.verticalAnchor == VerticalAnchor.Bottom)
                topT -= loc.height + data.offsetTop + offsetTop - 1;
            if (topT < 30)
                topT = 30;
            element.style.left = `${leftT}px`;
            element.style.top = `${topT}px`;
        }
    }

    interface popupWindowData {
        top: number;
        left: number;
        right: number;
        bottom: number;
        horizontalAnchor: HorizontalAnchor;
        verticalAnchor: VerticalAnchor;
        targetHorizontalAnchor: HorizontalAnchor;
        targetVerticalAnchor: VerticalAnchor;
        offsetLeft: number;
        offsetTop: number;
    }

    enum HorizontalAnchor {
        Left = 1,
        Center,
        Right
    }

    enum VerticalAnchor {
        Top = 1,
        Middle,
        Bottom
    }
}