namespace caspian {
    export class common {
        static RightToLeft(): boolean {
            return document.getElementsByTagName('body')[0].classList.contains('t-rtl');
        }

        static infoTimer;

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
                        animate.style.height = `${loc.height + 6}px`;
                        setTimeout(() => element.style.bottom = '3px', 20);
                    }
                    else {
                        animate.classList.add('c-animation-down');
                        animate.style.height = `${loc.height + 3}px`;
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

        public static async bindSlider(element: HTMLElement, dotnet: dotnetInvoker) {
            let containerWidth = element.getBoundingClientRect().width;
            let slide = element.getElementsByClassName('c-slider-slide')[0];
            let slideWidth = slide.getBoundingClientRect().width;
            (element.getElementsByClassName('c-slider-body')[0] as HTMLElement).style.width = `${slideWidth}px`;
            (element.getElementsByClassName('c-slider-content')[0] as HTMLElement).style.left = `${-slideWidth}px`;
            await dotnet.invokeMethodAsync('SetData', containerWidth, slideWidth);
            //window.addEventListener('resize', async () => {

            //});
            //window.addEventListener('locationchange', () => window.removeEventListener('loc'))
        }

        public static bindTree(tree: HTMLElement) {
            debugger;
        }

        public static async onWindowResizeHandler(element: HTMLElement, dotnet: dotnetInvoker) {
            let containerWidth = element.getBoundingClientRect().width;
            let slide = element.getElementsByClassName('c-slider-slide')[0];
            let slideWidth = slide.getBoundingClientRect().width;
            await dotnet.invokeMethodAsync('SetData', containerWidth, slideWidth);
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
            new DatePicker(element, dotnet);
        }

        public static showErrorMessage(element: HTMLElement) {
            let error = element.getElementsByClassName('errorMessage')[0];
            if (error)
                error.remove();
            let msg = element.attributes['error-message'];
            if (msg) {
                let htmlString = '<div class="errorMessage"></div>'
                var messageBox = document.createElement('div');
                messageBox.classList.add('errorMessage');
                messageBox.innerHTML = '<span class="c-icon"><i class="fa fa-info" aria-hidden="true"></i></span><Span class="c-content">'
                    + msg.value + '</Span><span class="c-pointer"></span>';
                element.append(messageBox);
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
            content.style.height = `${height}px`;
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
                (tabpanel.getElementsByClassName('c-selected-panel')[0] as HTMLElement).style.top = `${pos.top - basePos.top + 8}px`;
            else {
                let seledtedPanel = tabpanel.getElementsByClassName('c-selected-panel')[0] as HTMLElement;
                seledtedPanel.style.left = `${pos.left - basePos.left + 3}px`;
                seledtedPanel.style.width = `${pos.width - 8}px`;
            }
        }

        public static enableDefaultShortKey(status: boolean, dotnet: dotnetInvoker) {
            if (status) {
                document.body.onkeyup = async e => {
                    let key = e.keyCode;
                    if (key == 13 || key == 27)
                        await dotnet.invokeMethodAsync("HideConfirm", key == 13)
                }
            }
            else
                document.body.onkeyup = null;
        }

        public static bindDataGrid(grid: HTMLElement) {
            new DataGrid(grid);
        }

        public static bindBox() {

        }

        public static bindWindow(win: HTMLElement) {
            new Window(win);
        }

        public static bindLookup(input: HTMLElement, dotnet: dotnetInvoker) {
            new Lookup(input, dotnet);
        }

        public static bindTimepicker(element: HTMLElement, dotnet: dotnetInvoker) {
            new TimePicker(element, dotnet);
        }

        public static bindColorPicker(element: HTMLElement) {
            new ColorPicker(element);
        }

        public static bindInputCollorPicker(element: HTMLElement, dotnet: dotnetInvoker) {
            new InputCollorPicker(element, dotnet);
        }

        public static bindDropdownList(element: HTMLElement, dotnet: dotnetInvoker) {
            new DropdownList(element, dotnet);
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
                    document.body.onclick = null;
                    await dotnet.invokeMethodAsync('Close');
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

        public static bindPopupWindow(element: HTMLInputElement, target: HTMLElement, json: string, dotnet: dotnetInvoker) {
            new PopupWindow(element, target, json, dotnet);
        }

        public static bindComboBox(input: HTMLElement, pageable: boolean, dotnet: dotnetInvoker) {
            new ComboBox(input, pageable, dotnet);
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
                            document.body.onmousedown = null;
                            await dotnet.invokeMethodAsync("Close");
                        }
                    }
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

        public static bindMenu() {
            new Accordion(document.getElementById('accordion'), false);
        }

        public static bindTextBox(input: HTMLInputElement) {
            new TextBox(input, 'numeric');
        }

        public static bindStringbox(input: HTMLInputElement) {
            new TextBox(input, 'string');
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

        public static async bindImage(pic: HTMLImageElement, imageStream) {
            if (imageStream) {
                const arrayBuffer = await imageStream.arrayBuffer();
                const blob = new Blob([arrayBuffer]);
                pic.src = URL.createObjectURL(blob);
            }
            else
                pic.src = '';
        }
    }

    export interface dotnetInvoker {
        invokeMethodAsync(methodName: string);

        invokeMethodAsync(methodName: string, argr: any);
        invokeMethodAsync(methodName: string, argr: any, arg1:any);
    }
}

interface HTMLCollection {
    indexOf(element: HTMLElement): number;
}

interface HTMLElement {
    getPosition(): DOMRect;
}


// Implement the Extension
HTMLCollection.prototype.indexOf = function (element: HTMLElement): number {
    let items = this as HTMLCollection;
    for (let index = 0; index < items.length; index++)
        if (items[index] == element)
            return index;
    return -1;
}

HTMLElement.prototype.getPosition = function () {
    let parent: HTMLElement = this, left = 0, top = 0;
    let rect = parent.getBoundingClientRect();
    while (parent != document.body) {
        left += parent.offsetLeft;
        top += parent.offsetTop;
        parent = parent.offsetParent as HTMLElement;
    }
    return new DOMRect(left, top, rect.width, rect.height);
}

interface position {
    left: number; top: number;
}