

namespace caspian {
    export class common {
        static RightToLeft(): boolean {
            return document.getElementsByTagName('body')[0].classList.contains('t-rtl');
        }

        static gridData: gridData = {};

        static infoTimer;

        static timePickerData: timePickerData = {};

        public static bindCheclistDropdown(element: HTMLElement, dotnet: dotnetInvoker) {
            const mutationObserver = new MutationObserver(t => {
                let element = t[0].target as HTMLElement;
                if (element.classList.contains('c-checkbox-list'))
                    element = element.parentElement;
                if (element.classList.contains('t-checkbox-list')) {
                    let loc = element.getBoundingClientRect();
                    let animate = element.closest('.t-animation-container') as HTMLDivElement;
                    if (loc.top > window.outerHeight / 2) {
                        animate.classList.add('c-animation-up');
                        animate.style.height = (loc.height + 6) + 'px';
                        setTimeout(() => element.style.bottom = '3px', 20);
                    }
                    else {
                        animate.classList.add('c-animation-down');
                        animate.style.height = (loc.height + 3) + 'px';
                        setTimeout(() => element.style.top = '0', 20);
                    }

                }
            });
            mutationObserver.observe(element, {
                attributes: false,
                childList: true,
                subtree: true
            });
            let windowElement = element;
            window.addEventListener("click", async (e: Event): Promise<void> => {
                let elem = (e.target as HTMLElement).closest('.t-dropdown');
                if (elem == null || elem != windowElement)
                    await dotnet.invokeMethodAsync('CloseWindow');
            });
        }

        public static bindWindowClick(dotnet: dotnetInvoker) {
            let main = document.getElementsByClassName('c-content-main')[0] as HTMLElement;
            main = main || document.body;
            main.onmousedown = async e => {
                if ((e.target as HTMLElement).closest('.auto-hide') == null)
                    await dotnet.invokeMethodAsync('WindowClick');
            }
            window.addEventListener("locationchange", this.onMousedownHandler);
        }

        public static onMousedownHandler() {
            let main = document.getElementsByClassName('c-content-main')[0] as HTMLElement;
            if (main == null)
                document.body.onmousedown = null;
            else
                main.onmousedown = null;
            window.removeEventListener("locationchange", this.onMousedownHandler);
        }

        public static bindDatePicker(element: HTMLElement, dotnet: dotnetInvoker) {
            element.focus
            let input = element.getElementsByTagName('input')[0] as HTMLInputElement;
            element.onmouseenter = e => {
                let elem = (e.target as HTMLElement).getElementsByClassName('t-picker-wrap')[0] as HTMLElement;
                if (!elem.classList.contains('t-state-selected') || !elem.classList.contains('t-state-disabled'))
                    elem.classList.add('t-state-hover');
            }
            element.onmouseleave = e => {
                let elem = (e.target as HTMLElement).getElementsByClassName('t-picker-wrap')[0] as HTMLElement;
                elem.classList.remove('t-state-hover');
            }
            input.onfocus = e => {
                let elem = (e.target as HTMLElement);
                elem.closest('.t-picker-wrap').classList.replace('t-state-hover', 't-state-selected');
                this.showErrorMessage(elem.closest('.t-widget'));
            }
            input.onblur = e => {
                let elem = (e.target as HTMLElement);
                elem.closest('.t-picker-wrap').classList.remove('t-state-selected');
                this.hideErrorMessage(elem.closest('.t-widget'));
            }
            window.addEventListener('click', async e => {
                let elem = (e.target as HTMLElement).closest('.t-datepicker') as HTMLDivElement;
                if (elem != element && element.getElementsByClassName('t-animation-container').length > 0)
                    await dotnet.invokeMethodAsync('CloseWindow')
            });
            const mutationObserver = new MutationObserver(t => {
                let element = t[0].target as HTMLDivElement, animate = t[0].addedNodes[0] as HTMLElement;
                if (animate) {
                    let calendar = animate.getElementsByClassName('t-datepicker-calendar')[0] as HTMLElement;
                    if (element.getBoundingClientRect().top > window.outerHeight / 2) {
                        animate.classList.add('c-animate-up');
                        animate.style.height = '237px';
                        setTimeout(async () => calendar.style.bottom = '7px', 20);
                    }
                    else {
                        animate.classList.add('c-animate-down');
                        animate.style.height = '242px';
                        setTimeout(async () => calendar.style.top = '0', 20);
                    }
                }
            });
            mutationObserver.observe(element, {
                attributes: false,
                childList: true,
                subtree: false
            });
        }

        public static showErrorMessage(element: HTMLElement) {
            let error = element.getElementsByClassName('errorMessage')[0];
            if (error)
                error.remove();
            let msg = element.attributes['error-message'];
            if (msg) {

                element.append('<div class="errorMessage"><span class="c-icon"><i class="fa fa-info" aria-hidden="true"></i></span><Span class="c-content">'
                    + msg + '</Span><span class="c-pointer"></span></div>');
            }
        }

        public static hideErrorMessage(element: HTMLElement) {
            let ctr = element.getElementsByClassName('errorMessage')[0];
            if (ctr)
                ctr.remove();
        }

        public static setListHeaderPadding(list: HTMLElement) {
            let content = list.getElementsByClassName('c-dataview-content')[0] as HTMLDivElement;
            let height = content.getBoundingClientRect().height;
            content.style.overflow = 'visible';
            content.style.height = 'auto';
            let realHeight = content.getBoundingClientRect().height;;
            content.style.overflow = 'auto';
            content.style.height = height + 'px';
            let header = list.getElementsByClassName('c-dataview-header')[0] as HTMLDivElement;
            if (realHeight > height)
                header.style.paddingRight = '11px';
            else
                header.style.paddingRight = '0';
        }

        public static bindListView(list: HTMLElement) {
            caspian.common.setListHeaderPadding(list);
            const mutationObserver = new MutationObserver(t => {
                if (t.length > 0) {
                    let ctr = (t[t.length - 1].target as HTMLElement).closest('.c-widget.c-data-view') as HTMLElement;
                    caspian.common.setListHeaderPadding(ctr);
                }
            });
            mutationObserver.observe(list, {
                attributes: false,
                childList: true,
                subtree: true
            });
        }

        public static bindTabpanel(tabpanel: HTMLElement) {
            let basePos = tabpanel.getBoundingClientRect();
            let activeTab = tabpanel.getElementsByClassName('t-state-active')[0];
            let pos = activeTab.getBoundingClientRect();
            if (tabpanel.classList.contains('t-vertical'))
                (tabpanel.getElementsByClassName('c-selected-panel')[0] as HTMLElement).style.top = (pos.top - basePos.top + 8) + 'px';
            else {
                let seledtedPanel = tabpanel.getElementsByClassName('c-selected-panel')[0] as HTMLElement;
                seledtedPanel.style.left = (pos.left - basePos.left + 3) + 'px';
                seledtedPanel.style.width = (pos.width - 8) + 'px';
            }
        }

        public static bindDataGrid(grid: HTMLElement) {
            const resizeObserver = new ResizeObserver(entries => {
                let grv = grid;
                for (let entry of entries) {
                    if (entry.contentBoxSize && entry.contentBoxSize[0]) {

                        let contentHeight = grv.getElementsByClassName('t-grid-content')[0].getBoundingClientRect().height;
                        let table = grv.getElementsByClassName('t-grid-content')[0].getElementsByTagName('table')[0];
                        if (table) {
                            let tableHeight = table.getBoundingClientRect().height;
                            let header = grv.getElementsByClassName('t-grid-header')[0] as HTMLElement;
                            if (contentHeight < tableHeight) {
                                if (this.RightToLeft())
                                    header.style.paddingLeft = 11 + 'px';
                                else
                                    header.style.paddingRight = 11 + 'px';
                            }
                            else {
                                if (this.RightToLeft())
                                    header.style.paddingLeft = '0';
                                else
                                    header.style.paddingRight = '0';
                            }
                        }
                    }
                }
            });
            resizeObserver.observe(grid.getElementsByClassName('t-grid-content')[0]);
            /// column resize
            let head = grid.getElementsByClassName('t-grid-header-wrap')[0] as HTMLElement;

            head.onmousemove = e => {
                let element = (e.target as HTMLElement);
                if (element.tagName != 'th')
                    element = element.closest('th');
                if (element) {
                    let loc = element.getBoundingClientRect(), x = e.clientX;
                    if ((x - loc.left) < 5 || (loc.right - x) < 5) {
                        (e.target as HTMLElement).style.cursor = 'col-resize';
                        this.gridData.resize = true;
                    }
                    else {
                        (e.target as HTMLElement).style.cursor = '';
                        this.gridData.resize = false;
                    }
                }
            }
            head.onmousedown = e => {
                this.gridData.grid = (e.target as HTMLElement).closest('.t-grid');
                if (this.gridData.resize) {
                    let rtl = this.RightToLeft();
                    let element = (e.target as HTMLElement);
                    if (element.tagName != 'th')
                        element = element.closest('th');
                    let loc = element.getBoundingClientRect(), x = e.clientX;
                    this.gridData.curent = element;
                    this.gridData.curentWidth = loc.width;
                    let other = null;
                    if (x - loc.left < 5 && rtl || loc.right - x < 5 && !rtl) {
                        other = element.nextSibling as HTMLElement;
                        this.gridData.gridStatus = 1;
                    }
                    if (loc.right - x < 5 && rtl || x - loc.left < 5 && !rtl) {
                        other = element.previousSibling as HTMLElement;
                        this.gridData.gridStatus = 2
                    }
                    this.gridData.other = other;
                    if (other == null)
                        this.gridData.gridStatus = 3;
                    else 
                        this.gridData.otherWidth = other.getBoundingClientRect().width;
                    this.gridData.mouseState = true;
                    this.gridData.xStart = e.clientX;
                }
                window.onclick = this.drop;
                window.onmousemove = this.dragging;
            }
            (grid.getElementsByClassName('t-grid-content')[0] as HTMLDivElement).onscroll = e => {
                let target = e.target as HTMLElement;
                target.closest('.t-grid').getElementsByClassName('t-grid-header-wrap')[0].scrollLeft = target.scrollLeft;
            }
        }

        public static bindCalendar(elm: HTMLElement, viewType: ViewType, vNavigation: VNavigation) {
            let toDownState = elm.getElementsByClassName('c-down-to-state')[0] as HTMLElement;
            let toUpState = elm.getElementsByClassName('c-up-to-state')[0] as HTMLElement;
            let fromDownState = elm.getElementsByClassName('c-down-from-state')[0] as HTMLElement;
            let fromUpState = elm.getElementsByClassName('c-up-from-state')[0] as HTMLElement;
            
            if (viewType == ViewType.Month) {
                if (vNavigation == VNavigation.Down) {
                    toDownState.style.left = '0';
                    toDownState.style.top = '36px';
                    toDownState.style.width = '212px';
                    toDownState.style.height = '200px';
                    //-----------------------------
                    fromDownState.style.opacity = '0';
                }
            }
            else {
                let locTarget = elm.getBoundingClientRect();
                let left, top;
                if (toUpState) {
                    let loc = toUpState.getElementsByClassName('t-state-selected')[0].getBoundingClientRect();
                    left = loc.left - locTarget.left, top = loc.top - locTarget.top;
                }
                if (vNavigation == VNavigation.Up) {
                    fromUpState.style.left = `${left}px`;
                    fromUpState.style.top = `${top}px`;
                    fromUpState.style.width = '47px';
                    fromUpState.style.height = '60px';
                    toUpState.style.opacity = '1';
                    setTimeout(() => fromUpState.style.display = 'none', 4000);
                }
                else if (vNavigation == VNavigation.Down) {
                    toDownState.style.left = '0';
                    toDownState.style.top = '35px';
                    toDownState.style.width = '212px';
                    toDownState.style.height = '200px';
                    fromDownState.style.opacity = '0';
                }
            }
        }

        public static dragging(e: MouseEvent) {
            let data = caspian.common.gridData;
            if (data.mouseState) {
                if (data.other == null)
                    return;
                let dif = data.xStart - e.clientX;
                if (data.gridStatus == 2)
                    dif = -dif;
                let curentWidth = data.curentWidth;
                let otherWidth = data.otherWidth - 1;
                let curentResult = curentWidth - dif, otherResult = otherWidth + dif;
                if (curentResult < 30 || otherResult < 30)
                    return;
                data.curent.style.width = curentResult + 'px';
                data.other.style.width = otherResult + 'px';
                let columns = data.grid.getElementsByClassName('t-grid-header-wrap')[0].getElementsByTagName("tr")[0].children;
                let curentIndex = columns.indexOf(data.curent)
                let otherIndex = columns.indexOf(data.other);
                columns = data.grid.getElementsByClassName('t-grid-content')[0].getElementsByTagName('tr')[0].children;
                (columns.item(curentIndex) as HTMLElement).style.width = curentResult + 'px';
                (columns.item(otherIndex) as HTMLElement).style.width = otherResult + 'px';
                let contentHeight = data.grid.getElementsByClassName('t-grid-content')[0].getBoundingClientRect().height;
                let tableHeight = data.grid.getElementsByClassName('c-grid-items')[0].getBoundingClientRect().height;
                let header = data.grid.getElementsByClassName('t-grid-header')[0] as HTMLElement;
                if (contentHeight < tableHeight) {
                    if (caspian.common.RightToLeft())
                        header.style.paddingLeft = 11 + 'px';
                    else
                        header.style.paddingRight = 11 + 'px';
                }
                else {
                    header.style.paddingLeft = '0';
                    header.style.paddingRight = '0';
                }
            }
        }

        public static drop() {
            caspian.common.gridData.mouseState = false;
            window.onmousemove = null;
            window.onclick = null;
        }

        public static bindDragAndDrop(dragableDom: HTMLElement, header: HTMLElement) {
            header.onmousedown = e => {
                let loc = (e.target as HTMLElement).getBoundingClientRect();
                let xStart = e.clientX, yStart = e.clientY, leftStart = loc.left, topStart = loc.top;
                document.onmouseup = () => {
                    document.onmouseup = null;
                    document.onmousemove = null;
                }
                document.onmousemove = e => {
                    let difX = e.clientX - xStart, difY = e.clientY - yStart;
                    dragableDom.style.left = (leftStart + difX) + 'px';
                    dragableDom.style.top = (topStart + difY) + 'px';
                }
            }
        }

        public static windowOpenClose(win: HTMLElement) {
            let content = win.getElementsByClassName('t-window-content')[0];
            if (content.attributes['status'].value == '2') {
                let main = document.getElementsByClassName('c-content-main')[0] as HTMLElement;
                if (main == null)
                    document.body.style.overflow = 'hidden';
                else
                    main.style.overflow = 'hidden';
                let header = win.getElementsByClassName('t-window-titlebar')[0] as HTMLElement;
                if (content.attributes['draggable']) {
                    caspian.common.bindDragAndDrop(win, header);
                    header.style.cursor = 'move';
                }
                else {
                    header.onmousedown = null;
                    header.style.cursor = 'default';
                }
                win.style.display = 'block';
                let left, top;
                let parent = win.parentElement.closest('.t-window') as HTMLElement;
                if (parent == null) {
                    left = (window.innerWidth - win.getBoundingClientRect().width) / 2;
                    top = -50;
                }
                else {
                    let loc = parent.getBoundingClientRect();
                    left = loc.left + (loc.width - win.getBoundingClientRect().width) / 2;
                    top = loc.top - 45;
                }
                win.style.left = left + 'px';
                win.style.top = top + 'px';
                setTimeout(function () {
                    win.classList.add('window-animate');
                    win.style.top = (top + 80) + 'px';
                    setTimeout(() => win.classList.remove('window-animate'), 250);
                }, 25);
            }
            else 
                win.style.display = 'none';
        }

        public static bindWindow(win: HTMLElement) {
            caspian.common.windowOpenClose(win.closest('.t-window'));
            const mutationObserver = new MutationObserver(list => {
                list.forEach(mutation => {
                    console.log(mutation.target)
                    if (mutation.type == 'attributes' && mutation.attributeName == 'status') {
                        let status = (mutation.target as HTMLElement).attributes['status'].value;
                        if (status == '1') {
                            let openWindowIsExist = false;
                            let windows = document.getElementsByClassName('t-window');
                            for (let index = 0; index < windows.length; index++) {
                                if (windows.item(index).getElementsByClassName('t-window-content')[0].attributes['status'].value == '2') {
                                    openWindowIsExist = true;
                                    break;
                                }
                            }
                            if (!openWindowIsExist)
                                (document.getElementsByClassName('c-content-main')[0] as HTMLElement).style.overflow = 'auto';
                        }
                        else
                            (document.getElementsByClassName('c-content-main')[0] as HTMLElement).style.overflow = 'hidden';
                        let mywindow = (mutation.target as HTMLElement).closest('.t-window') as HTMLElement;
                        caspian.common.windowOpenClose(mywindow);
                    }
                });
            });
            mutationObserver.observe(win, {
                attributes: true,
                childList: false,
                subtree: false
            });
        }

        public static bindLookup(input: HTMLElement, dotnet: dotnetInvoker) {
            let lookup = input.closest('.c-lookup') as HTMLElement;
            input.onfocus = () => {
                caspian.common.showErrorMessage(lookup);
            }
            input.onkeydown = e => {
                let code = e.code || e.keyCode;
                if (code == 40 || code == 38)
                    e.preventDefault();
            }
            input.onblur = () => {
                caspian.common.hideErrorMessage(lookup);
            }
            //if (lookup.attributes['closeonblur'].value != undefined) 
            //    lookup.attributes['tabindex'].value = '0';
            const mutationObserver = new MutationObserver(list => {
                list.forEach(t => {
                    if (t.type == 'attributes' && t.attributeName == 'status') {
                        let target = (t.target as HTMLElement);
                        let container = document.getElementsByClassName('c-content-main')[0] as HTMLElement;
                        if (target.attributes['status'].value == '2') {
                            let animate = target.getElementsByClassName('t-animation-container')[0] as HTMLElement;
                            let helpWindow = target.getElementsByClassName('t-HelpWindow')[0] as HTMLElement;
                            let locTarget = target.getBoundingClientRect();
                            let locHelpWindow = helpWindow.getBoundingClientRect();
                            let left = locTarget.left - (locHelpWindow.width - locTarget.width) / 2 - 20;
                            let locContainer = container.getBoundingClientRect();
                            if (left + locHelpWindow.width > locContainer.width)
                                left = locContainer.width - locHelpWindow.width - 35;
                            animate.style.left = left + 'px';
                            animate.style.width = (locHelpWindow.width + 10) + 'px';
                            animate.style.right = 'auto';
                            container.style.overflow = 'hidden';
                            animate.style.height = (locHelpWindow.height - 20) + 'px';
                            let upToDown = true;
                            if (locTarget.bottom + locHelpWindow.height - 30 <= window.innerHeight) {
                                animate.style.top = (locTarget.top + 35) + 'px';
                                helpWindow.style.top = -locHelpWindow.height + 'px';
                            }
                            else if (locTarget.top >= locHelpWindow.height - 30) {
                                upToDown = false;
                                animate.style.bottom = (window.innerHeight - locTarget.top) + 'px';
                                helpWindow.style.bottom = -locHelpWindow.height + 'px';
                            }
                            else {
                                animate.style.bottom = '5px';
                                helpWindow.style.top = -locHelpWindow.height + 'px';
                            }
                            setTimeout(() => {
                                if (upToDown) {
                                    helpWindow.classList.add('c-lookup-animate-down');
                                    helpWindow.style.top = '0';
                                }
                                else {
                                    helpWindow.classList.add('c-lookup-animate-up');
                                    helpWindow.style.bottom = '0';
                                }
                            }, 25);
                            if (lookup.attributes['autoHide']) {
                                window.onclick = async function (e: MouseEvent) {
                                    if (!(e.target as HTMLElement).closest('.c-lookup'))
                                        await dotnet.invokeMethodAsync('Close');
                                };
                            }
                        }
                        else {
                            container.style.overflow = 'auto';
                            window.onclick = null;
                        }
                    }
                })

            });
            mutationObserver.observe(lookup, {
                attributes: true,
                childList: false,
                subtree: false
            });
        }

        public static setMinutLocation(elem: HTMLElement, e: MouseEvent, type: number) {
            let from = elem.attributes['from'], to = elem.attributes['to'];
            let loc = elem.getElementsByTagName('circle').item(2).getBoundingClientRect();
            let difX = e.pageX - loc.left, difY = e.pageY - loc.top;
            let deg = Math.atan(difY / difX);
            if (difX < 0)
                deg += Math.PI;
            let minute = Math.round(deg / Math.PI * 30 + 15);
            if (minute == 60)
                minute = 0;
            if (from != null && minute < from.value)
                return false;
            if (to != null && minute > to.value)
                return false;
            let text = minute.toString();
            if (minute < 10)
                text = '0' + text;
            elem.closest('.c-timepicker').getElementsByClassName('c-span-minutes')[0].textContent = text;
            let x2 = null, y2 = null;
            if (type == 1) {
                x2 = Math.cos(deg) * 110;
                y2 = Math.sin(deg) * 110;
            }
            else {
                let factor = (minute - 15) / 30 * Math.PI;
                x2 = Math.cos(factor) * 110;
                y2 = Math.sin(factor) * 110;
            }
            let svg = elem.getElementsByTagName('svg')[0];
            let circles = svg.getElementsByTagName('circle');
            circles[0].attributes['cx'].value = circles[1].attributes['cx'].value = x2;
            circles[0].attributes['cy'].value = circles[1].attributes['cy'].value = y2;
            let line = svg.getElementsByTagName('line')[0];
            line.attributes['x2'].value = x2;
            line.attributes['y2'].value = y2;
            return true;
        }

        public static bindTimePanel(elm: HTMLElement, dotnet: dotnetInvoker) {
            (elm.getElementsByClassName('c-time-minutes')[0] as HTMLDivElement).onmousedown = e => {
                let loc = elm.getElementsByTagName('circle')[2].getBoundingClientRect();
                let r = Math.pow(e.pageX - loc.left - 2, 2) + Math.pow(e.pageY - loc.top - 2, 2);
                if (r > 6400) {
                    this.setMinutLocation(elm.getElementsByClassName('c-time-minutes')[0] as HTMLElement, e, 0)
                    document.body.onmousemove = e => {
                        this.setMinutLocation(elm.getElementsByClassName('c-time-minutes')[0] as HTMLElement, e, 1);
                    }
                    document.body.onmouseup = async e => {
                        document.body.onmousemove = document.body.onmouseup = null;
                        if (elm.getElementsByClassName('c-time-footer').length == 0) {
                            this.setTime(elm.closest('.t-timepicker'))
                            await dotnet.invokeMethodAsync('Close');
                        }
                    }
                }
            }
        }

        public static setTime(timepicker: HTMLElement) {
            let hours = timepicker.getElementsByClassName('c-span-hours')[0].textContent;
            let minutes = timepicker.getElementsByClassName('c-span-minutes')[0].textContent;
            let input = timepicker.getElementsByTagName('input')[0];
            input.value = hours + ':' + minutes;
            let event = new Event('change');
            input.dispatchEvent(event);
        }

        public static bindTimepicker(element: HTMLElement, dotnet: dotnetInvoker) {
            let input = element.getElementsByTagName('input')[0];
            this.bindMask(input, '__:__');
            element.onmouseenter = () => {
                let wrap = element.getElementsByClassName('t-picker-wrap')[0];
                if (!wrap.classList.contains('t-state-selected') && !wrap.classList.contains('t-state-disabled'))
                    wrap.classList.add('t-state-hover');
            }
            element.onmouseleave = () => {
                element.getElementsByClassName('t-picker-wrap')[0].classList.remove('t-state-hover')
            }
            input.onfocus = e => {
                (e.target as HTMLElement).closest('.t-picker-wrap').classList.add('t-state-selected');
            }
            input.onblur = e => {
                (e.target as HTMLElement).closest('.t-picker-wrap').classList.remove('t-state-selected');
            }
            const mutationObserver = new MutationObserver(t => {
                let panel = (t[0].target as HTMLElement).getElementsByClassName('c-timepicker')[0] as HTMLDivElement;
                if (panel == null) {
                    window.onclick = null;
                    return;
                }
                let panelLoc = panel.getBoundingClientRect();
                let elem = panel.closest('.t-timepicker');
                let animate = panel.closest('.t-animation-container') as HTMLDivElement;
                let height = panelLoc.height + 2;
                if (elem.getBoundingClientRect().top > height) {
                    animate.classList.add('c-animate-up');
                    setTimeout(() => {
                        panel.style.bottom = '0';
                    }, 10);
                }
                else {
                    animate.classList.add('c-animate-down');
                    setTimeout(() => {
                        panel.style.top = '0';
                    }, 10);
                    height += 8;
                }
                animate.style.width = (panelLoc.width + 8) + 'px';
                animate.style.height = height + 'px';
                caspian.common.bindTimePanel(panel, dotnet);
                window.onclick = async e => {
                    if ((e.target as HTMLElement).closest('.c-timepicker') == null)
                        await dotnet.invokeMethodAsync('Close');
                }
                let okButton = panel.getElementsByClassName('c-ok')[0] as HTMLButtonElement;
                if (okButton != null) {
                    okButton.onclick = async e => {
                        this.setTime(panel.closest('.t-timepicker'));
                        await dotnet.invokeMethodAsync('Close');
                    }
                }
            });
            mutationObserver.observe(element, {
                attributes: false,
                childList: true,
                subtree: false
            });
        }

        public static bindColorPicker(element: HTMLElement) {
            let colorpicker = (element as any).colorPicker = new ColorPicker(element);
            colorpicker.updateColor();
            const mutationObserver = new MutationObserver((mutationList) => {
                if (mutationList[0].attributeName != 'id') {
                    let picker = (mutationList[0].target as any).colorPicker as ColorPicker;
                    picker.updateColor();
                }
            });
            mutationObserver.observe(element, {
                attributes: true,
                childList: false,
                subtree: false
            });
        }

        public static bindInputCollorPicker(element: HTMLElement, dotnet: dotnetInvoker) {
            const mutationObserver = new MutationObserver(t => {
                let elem = t[0].target as HTMLElement;
                let animate = elem.getElementsByClassName('c-animation-container')[0];
                if (animate != null) {
                    
                    let top = elem.getBoundingClientRect().top;
                    let picker = elem.getElementsByClassName('c-colorpicker-panel')[0] as HTMLDivElement;
                    if (top > window.innerHeight / 2) {
                        animate.classList.add('c-animation-up');
                        setTimeout(t => {
                            picker.style.bottom = '0';
                        }, 10);
                    }
                    else {
                        animate.classList.add('c-animation-down');
                        setTimeout(t => {
                            picker.style.top = '6px';
                        }, 10);
                    }
                    document.body.onclick = async e => {
                        if ((e.target as HTMLDivElement).closest('.c-animation-container') == null)
                            await dotnet.invokeMethodAsync("Close");
                    }
                }
                else 
                    document.body.onclick = null;
            });

            mutationObserver.observe(element, {
                attributes: false,
                childList: true,
                subtree: false
            });

            element.onmouseenter = () => {
                element.getElementsByClassName('c-input-color')[0].classList.add('c-state-hover');
            }
            element.onmouseleave = () => {
                element.getElementsByClassName('c-input-color')[0].classList.remove('c-state-hover');
            }
            element.onfocus = () => {
                element.getElementsByClassName('c-input-color')[0].classList.add('c-state-focused');
            }
            element.onblur = () => {
                element.getElementsByClassName('c-input-color')[0].classList.remove('c-state-focused');
            }
        }

        public static bindDropdownList(element: HTMLElement, dotnet: dotnetInvoker) {
            const mutationObserver = new MutationObserver(t => {
                let ddl = t[0].target as HTMLElement;
                let animate = ddl.getElementsByClassName('t-animation-container')[0] as HTMLElement;
                if (animate != null) {
                    let group = animate.getElementsByClassName('t-group')[0] as HTMLElement;
                    if (ddl.getBoundingClientRect().top > window.innerHeight / 2) {
                        animate.classList.add('c-animate-up');
                        animate.style.height = `${group.getBoundingClientRect().height}px`;
                        group.style.bottom = '0';
                    }
                    else {
                        animate.classList.add('c-animate-down');
                        animate.style.height = `${group.getBoundingClientRect().height + 7}px`;
                        group.style.top = '0';
                    }
                    document.body.onclick = async e => {
                        if ((e.target as HTMLElement).closest('t-animation-container') == null)
                            await dotnet.invokeMethodAsync('CloseWindow');
                    }
                }
                else 
                    document.body.onclick = null;
            });
            mutationObserver.observe(element, {
                attributes: false,
                childList: true,
                subtree: false
            });

            element.onmouseenter = e => {
                let ddl = (e.target as HTMLElement).getElementsByClassName('t-dropdown-wrap')[0];
                if (!ddl.classList.contains('t-state-disabled')) {
                    ddl.classList.remove('t-state-default');
                    ddl.classList.add('t-state-hover');
                }
            }
            element.onmouseleave = e => {
                let ddl = (e.target as HTMLElement).getElementsByClassName('t-dropdown-wrap')[0];
                ddl.classList.remove('t-state-hover');
                ddl.classList.add('t-state-default');
            }
            element.onfocus = e => {
                let ddl = (e.target as HTMLElement).getElementsByClassName('t-dropdown-wrap')[0];
                ddl.classList.remove('t-state-default');
                ddl.classList.add('t-state-focused');
                this.showErrorMessage(e.target as HTMLElement);
            }
            element.onblur = () => {
                let ddl = element.getElementsByClassName('t-dropdown-wrap')[0];
                ddl.classList.remove('t-state-focused');
                ddl.classList.add('t-state-default');
                this.hideErrorMessage(element);
            }
        }

        public static async bindImage(pic: HTMLImageElement, imageStream) {
            if (imageStream) {
                const arrayBuffer = await imageStream.arrayBuffer();
                const blob = new Blob([arrayBuffer]);
                pic.src = URL.createObjectURL(blob);
            }
            else
                pic.src = '';
        }

        public static bindMultiSelect(element: HTMLElement) {
            element.onfocus = () => {
                this.showErrorMessage(element);
            }
            element.onblur = () => {
                this.hideErrorMessage(element);
            }
        }

        public static bindContextMenu(element: HTMLElement, dotnet: dotnetInvoker) {
            document.body.onclick = async e => {
                if ((e.target as HTMLElement).closest('.c-context-menu-container') == null) {
                    await dotnet.invokeMethodAsync('Close');
                    document.onclick = null;
                }
            }
            const mutationObserver = new MutationObserver((list) => {
                list.every(t => {
                    if (t.addedNodes.length == 1) {
                        let ctr = t.addedNodes[0] as HTMLElement;
                        let parentLoc = ctr.parentElement.getBoundingClientRect();
                        let right = parentLoc.left + parentLoc.width + ctr.getBoundingClientRect().width + 10;
                        let list = ctr.classList;
                        if (right < window.innerWidth) {
                            list.add('c-context-menu-left');
                            list.remove('c-context-menu-right');
                        }
                        else {
                            list.add('c-context-menu-right');
                            list.remove('c-context-menu-left');
                        }
                        setTimeout(() => ctr.style.marginTop = '-28px' , 1);
                    }
                });
            });
            mutationObserver.observe(element, {
                attributes: false,
                childList: true,
                subtree: true,
            });
        }

        public static showMessage(message: string) {
            if (this.infoTimer)
                clearTimeout(this.infoTimer);
            let box = document.getElementById('outMessage');
            if (box)
                box.remove();
            let odv = document.createElement('div');
            odv.id = 'outMessage';
            odv.className = 't-widget t-message';
            odv.innerHTML = `<div class="t-window-titlebar"><span class="t-title">Info</span><span class="t-close"><i class="fa fa-close"></i></span></div><div class="c-content">${message}</div>`;
            (odv.getElementsByClassName('t-close')[0] as HTMLElement).onclick = () => {
                this.hideMessage();
            }
            let main = document.getElementsByClassName('c-content-main')[0];
            if (main == null)
                document.body.appendChild(odv);
            else
                main.appendChild(odv);
            setTimeout(() => {
                odv.style.top = '30px';
            }, 5);
            this.infoTimer = setTimeout(() => {
                this.hideMessage();
            }, 4_000);
        }

        public static hideMessage() {
            if (this.infoTimer)
                clearTimeout(this.infoTimer);
            let box = 
            document.getElementById('outMessage').style.top = '-130px';
            setTimeout(() => {
                document.getElementById('outMessage').remove();
            }, 300);
        }

        public static bindPopupWindow(element: HTMLInputElement, target: HTMLElement, json: string, dotnet: dotnetInvoker) {
            let data = JSON.parse(json) as popupWindowData;
            element.style.display = 'block';
            let className = element.className;
            element.className = 'auto-hide c-popup-window';
            let loc = element.getBoundingClientRect();
            let mainLoc = document.getElementsByClassName('c-content-main')[0].getBoundingClientRect();
            element.className = className;
            if (target) {
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
                element.style.left = `${leftT}px`;
                element.style.top = `${topT}px`;
            }
            else {
                if (data.left != null) {
                    element.style.left = `${data.left}px`;
                    element.style.right = 'auto';
                }
                else if (data.right != null) {
                    element.style.left = 'auto';
                    element.style.right = `${data.right}px`;
                }
                else if (data.top != null) {
                    element.style.top = `${data.top}px`;
                    element.style.bottom = 'auto';
                }
                else if (data.bottom != null) {
                    element.style.top = 'auto';
                    element.style.bottom = `${data.bottom}px`;
                }
            }
            document.body.onmousedown = async e => {
                if ((e.target as HTMLElement).closest('.auto-hide') == null) {
                    document.body.onmousedown = null;
                    await dotnet.invokeMethodAsync('Close');
                }
            }
        }

        public static bindMask(el: HTMLInputElement, patern: string) {
            const pattern = patern,
                slots = new Set(el.dataset.slots || "_"),
                prev = (j => Array.from(pattern, (c, i) => slots.has(c) ? j = i + 1 : j))(0),
                first = [...pattern].findIndex(c => slots.has(c)),
                accept = new RegExp(el.dataset.accept || "\\d", "g"),
                clean = input => {
                    input = input.match(accept) || [];
                    return Array.from(pattern, c =>
                        input[0] === c || slots.has(c) ? input.shift() || c : c
                    );
                },
                format = () => {
                    const [i, j] = [el.selectionStart, el.selectionEnd].map(i => {
                        i = clean(el.value.slice(0, i)).findIndex(c => slots.has(c));
                        return i < 0 ? prev[prev.length - 1] : back ? prev[i - 1] || first : i;
                    });
                    el.value = clean(el.value).join('');
                    el.setSelectionRange(i, j);
                    back = false;
                };
            let back = false;
            el.addEventListener("keydown", (e) => back = e.key === "Backspace");
            el.addEventListener("input", format);
            el.addEventListener("focus", format);
            el.addEventListener("blur", () => el.value === pattern && (el.value = ""));
        }

        public static bindComboBox(input: HTMLElement, Pageable: boolean, dotnet: dotnetInvoker) {

            const mutationObserver = new MutationObserver(t => {
                let ctr = t[0].target as HTMLElement;
                let group = ctr.getElementsByClassName('t-group')[0] as HTMLElement;
                if (group) {
                    if (Pageable) {
                        group.onscrollend = async () => {
                            await dotnet.invokeMethodAsync('IncPageNumberInvokable');
                        }
                    }
                    let animate = ctr.getElementsByClassName('t-animation-container')[0] as HTMLElement;
                    let height = group.getElementsByClassName('t-reset')[0].getBoundingClientRect().height;
                    height = Math.min(250, height);
                    height = Math.max(height, 30);
                    let loc = group.getBoundingClientRect().top + height + 5;
                    if (loc > window.innerHeight) {
                        let list = animate.classList;
                        list.add('c-animate-up');
                        list.remove('c-animate-down');
                        animate.style.height = `${height}px`;
                        setTimeout(() => group.style.bottom = '0', 10);
                    }
                    else {
                        let list = animate.classList;
                        list.add('c-animate-down');
                        list.remove('c-animate-up');
                        animate.style.height = `${height + 7}px`;
                        setTimeout(() => group.style.top = '0', 10)
                    }
                    document.body.onmousedown = async e => {
                        if ((e.target as HTMLElement).closest('.t-group') == null)
                            await dotnet.invokeMethodAsync('CloseInvokable')
                    }
                }
                else 
                    document.body.onmousedown = null;
            });
            mutationObserver.observe(input.closest('.t-combobox'), {
                attributes: false,
                childList: true,
                subtree: false
            });
            let control = input.closest('.t-combobox').getElementsByClassName('t-dropdown-wrap')[0] as HTMLElement;
            input.onkeyup = e => {
                if (e.key == 'ArrowDown' || e.key == 'ArrowUp') {
                    let selected = (e.target as HTMLElement).closest('.t-combobox').getElementsByClassName('t-state-selected')[0];
                    if (selected) {
                        let content = (e.target as HTMLElement).closest('.t-combobox').getElementsByClassName('t-group')[0];
                        let loc = selected.getBoundingClientRect();
                        let top = loc.top - content.getBoundingClientRect().top, bottom = top + loc.height;
                        if (bottom > 240 || top < 10) {
                            let scrollTop = content.scrollTop;
                            content.scrollTop = (scrollTop + bottom - 240);
                        }
                    }
                }
            };

            (input.closest('.t-combobox') as HTMLElement).onmouseenter = () => {
                if (!control.classList.contains('t-state-disabled')) {
                    let list = control.classList;
                    list.add('t-state-hover');
                    list.remove('t-state-default');
                }
            };
            (input.closest('.t-combobox') as HTMLElement).onmouseleave = () => {
                if (!control.classList.contains('t-state-disabled')) {
                    let list = control.classList;
                    list.add('t-state-default');
                    list.remove('t-state-hover');
                }
            };
            input.onfocus = () => {
                let list = control.classList;
                list.add('t-state-focused');
                list.remove('t-state-default');
                this.showErrorMessage(input.closest('.t-widget'));
            }
            input.onblur = () => {
                let list = control.classList;
                list.add('t-state-default');
                list.remove('t-state-focused');
                this.hideErrorMessage(input.closest('.t-widget'));
            }
        }

        public static bindLookupTree(input: HTMLElement, dotnet: dotnetInvoker) {
            const mutationObserver = new MutationObserver(t => {
                let target = t[0].target as HTMLElement;
                let targetLoc = target.getBoundingClientRect();
                let content = target.getElementsByClassName('c-tree-content')[0] as HTMLElement;
                if (content != null) {
                    let loc = target.getBoundingClientRect();
                    let tree = content.getElementsByClassName('c-treeview')[0] as HTMLElement;
                    content.style.width = `${loc.width + 3}px`;
                    if (targetLoc.top > window.innerHeight / 2) {
                        content.style.height = `${targetLoc.top - 25}px`;
                        content.style.marginTop = `${-targetLoc.top - 9}px`;
                        content.classList.add('c-animate-up');
                        setTimeout(() => tree.style.bottom = '1px', 10);
                    }
                    else {
                        content.style.height = `${window.innerHeight - targetLoc.bottom - 5}px`;
                        content.classList.add('c-animate-down');
                        setTimeout(() => tree.style.top = '0', 10);
                    }
                    document.body.onmousedown = async e => {
                        if ((e.target as HTMLElement).closest('.auto-hide') == null) {
                            await dotnet.invokeMethodAsync("Close");
                        }
                    }
                }
                else {
                    document.body.onmousedown = null;
                }
            });
            mutationObserver.observe(input.closest('.c-lookup-tree'), {
                attributes: false,
                childList: true,
                subtree: false
            });
            let lookup = input.closest('.c-content');
            input.onfocus = () => {
                lookup.classList.add('c-state-focus')
            }
        }

        public static async bindFileDownload(fileName: string, contentStreamReference) {
            const arrayBuffer = await contentStreamReference.arrayBuffer();
            const blob = new Blob([arrayBuffer]);
            const url = URL.createObjectURL(blob);
            const anchorElement = document.createElement('a');
            anchorElement.href = url;
            anchorElement.download = fileName ?? '';
            anchorElement.click();
            anchorElement.remove();
            URL.revokeObjectURL(url);
        }

        public static bindMenu() {
            new Accordion(document.getElementById('accordion'), false);
        }
    }
    export interface dotnetInvoker {
        invokeMethodAsync(methodName: string);
    }

    class popupWindowData {
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

    enum ViewType {
        Month = 1,

        Year,

        Decade,

        Century
    }

    enum VNavigation {
        Up = 1,

        Down
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

    class gridData {
        grid?: HTMLDivElement;
        mouseState?: boolean;
        resize?: boolean;
        curent?: HTMLElement;
        curentWidth?: number;
        other?: HTMLElement;
        otherWidth?: number;
        gridStatus?: number;
        xStart?: number;
    }

    class timePickerData {
        mouseState?: boolean;
    }
}


class Accordion {

}

interface HTMLCollection {
    indexOf(element: HTMLElement): number;
}


// Implement the Extension
HTMLCollection.prototype.indexOf = function (element: HTMLElement): number {
    let items = this as HTMLCollection;
    for (let index = 0; index < items.length; index++)
        if (items[index] == element)
            return index;
    return -1;
}