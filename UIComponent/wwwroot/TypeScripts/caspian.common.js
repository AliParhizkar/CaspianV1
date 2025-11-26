/// <reference path="index.ts" />
var caspian;
(function (caspian) {
    class common {
        static showMessage(message) {
            if (this.infoTimer)
                clearTimeout(this.infoTimer);
            let box = document.getElementById('outMessage');
            if (box)
                box.remove();
            let odv = document.createElement('div');
            odv.id = 'outMessage';
            odv.className = 't-widget t-message';
            odv.innerHTML = `<div class="t-window-titlebar"><span class="t-title">Info</span><span class="t-close"><i class="fa fa-close"></i></span></div><div class="c-content">${message}</div><div class="c-progress"></div>`;
            odv.getElementsByClassName('t-close')[0].onclick = () => {
                this.hideMessage();
            };
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
            }, 5_500);
            if (this.intervalId)
                clearInterval(this.intervalId);
            let value = 250;
            this.intervalId = setInterval(() => {
                let progress = odv.lastElementChild;
                progress.style.width = value + 'px';
                value -= 0.5;
            }, 11);
        }
        static getPixelsPerCentimetre() {
            let div = document.createElement("div");
            div.style.width = "1cm";
            document.body.appendChild(div);
            let result = div.getBoundingClientRect().width;
            document.body.removeChild(div);
            return result;
        }
        static hideMessage() {
            if (this.infoTimer)
                clearTimeout(this.infoTimer);
            setTimeout(() => {
                document.getElementById('outMessage').remove();
            }, 300);
        }
        static BindWindowClickForMainLayout(dotnet) {
            let container = document.getElementsByClassName('c-packages-container')[0];
            container.style.display = '';
            setTimeout(() => {
                debugger;
                let menu = container.getElementsByClassName('c-packages')[0];
                var rect = menu.getBoundingClientRect();
                container.style.width = `${rect.width}px`;
                container.style.height = `${rect.height}px`;
            }, 100);
            let main = document.getElementsByClassName('page')[0];
            main.onmousedown = async (e) => {
                await dotnet.invokeMethodAsync('CloseSubsystem');
                container.style.display = 'none';
                container.style.width = container.style.height = '0';
                main.onmousedown = null;
            };
        }
        static setValueOnClient(input, value) {
            input.value = value;
        }
        static RightToLeft() {
            return document.body.classList.contains('t-rtl');
        }
        static getSelection(input) {
            let data = { start: input.selectionStart, end: input.selectionEnd };
            return data;
        }
        static bindErrorMessage(target, activeElement, top = 0) {
            const mutationObserver = new MutationObserver(t => {
                t.forEach(u => {
                    if (u.type == 'attributes' && u.attributeName == 'error-message') {
                        if (document.activeElement == activeElement)
                            caspian.common.showErrorMessage(target, top);
                    }
                });
            });
            mutationObserver.observe(target, {
                attributes: true,
                childList: false,
                subtree: false
            });
        }
        static setSelection(input, start, end) {
            input.focus();
            input.setSelectionRange(start, end || start);
        }
        static bindCheckbox(element) {
            element.onfocus = e => {
                caspian.common.showErrorMessage(e.target);
            };
            element.onblur = e => {
                caspian.common.hideErrorMessage(e.target);
            };
        }
        static bindCheclistDropdown(element, dotnet) {
            const mutationObserver = new MutationObserver(t => {
                let element = t[0].target;
                let width = element.closest('.t-dropdown').getBoundingClientRect().width;
                if (element.classList.contains('c-checkbox-list'))
                    element = element.parentElement;
                if (element.classList.contains('t-checkbox-list')) {
                    let loc = element.getBoundingClientRect();
                    let animate = element.closest('.t-animation-container');
                    animate.style.height = `${loc.height + 6}px`;
                    animate.style.width = `${width + 6}px`;
                    if (loc.top > window.outerHeight / 2) {
                        animate.classList.add('c-animation-up');
                        animate.style.marginTop = `${-loc.height - 37}px`;
                        setTimeout(() => element.style.bottom = '3px', 20);
                    }
                    else {
                        animate.classList.add('c-animation-down');
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
            window.addEventListener("click", async (e) => {
                let elem = e.target.closest('.t-dropdown');
                if (elem == null || elem != windowElement)
                    await dotnet.invokeMethodAsync('CloseWindow');
            });
        }
        static bindWindowClick(dotnet) {
            let main = document.getElementsByClassName('c-content-main')[0];
            main = main || document.body;
            main.onmousedown = async (e) => {
                if (e.target.closest('.auto-hide') == null)
                    await dotnet.invokeMethodAsync('WindowClick');
            };
            window.addEventListener("locationchange", this.onMousedownHandler);
        }
        static async bindSlider(element, dotnet) {
            let containerWidth = element.getBoundingClientRect().width;
            let slide = element.getElementsByClassName('c-slider-slide')[0];
            let slideWidth = slide.getBoundingClientRect().width;
            element.getElementsByClassName('c-slider-body')[0].style.width = `${slideWidth}px`;
            element.getElementsByClassName('c-slider-content')[0].style.left = `${-slideWidth}px`;
            await dotnet.invokeMethodAsync('SetData', containerWidth, slideWidth);
            //window.addEventListener('resize', async () => {
            //});
            //window.addEventListener('locationchange', () => window.removeEventListener('loc'))
        }
        static scrollIntoViewSelectedRow(grid) {
            let selectedRows = grid.getElementsByClassName('t-state-selected');
            if (selectedRows.length == 1)
                selectedRows[0].scrollIntoView();
        }
        static bindTree(tree) {
        }
        static bindTooltip() {
        }
        static async onWindowResizeHandler(element, dotnet) {
            let containerWidth = element.getBoundingClientRect().width;
            let slide = element.getElementsByClassName('c-slider-slide')[0];
            let slideWidth = slide.getBoundingClientRect().width;
            await dotnet.invokeMethodAsync('SetData', containerWidth, slideWidth);
        }
        static onMousedownHandler() {
            let main = document.getElementsByClassName('c-content-main')[0];
            if (main == null)
                document.body.onmousedown = null;
            else
                main.onmousedown = null;
            window.removeEventListener("locationchange", this.onMousedownHandler);
        }
        static bindDatePicker(element, dotnet) {
            new caspian.DatePicker(element, dotnet);
        }
        static showErrorMessage(element, top = 0) {
            let error = element.getElementsByClassName('errorMessage')[0];
            if (error)
                error.remove();
            let msg = element.attributes['error-message'];
            if (msg) {
                var messageBox = document.createElement('div');
                messageBox.classList.add('errorMessage');
                messageBox.innerHTML = '<span class="c-icon"><i class="fa fa-info" aria-hidden="true"></i></span><Span class="c-content">'
                    + msg.value + '</Span><span class="c-pointer"></span>';
                element.append(messageBox);
                let height = messageBox.getBoundingClientRect().height;
                if (top > 0)
                    messageBox.style.marginTop = `${top + 6}px`;
                //let pointer = messageBox.getElementsByClassName('c-pointer')[0] as HTMLElement;
                //pointer.style.top = `${height - 6}px`;
            }
        }
        static hideErrorMessage(element) {
            let ctr = element.getElementsByClassName('errorMessage')[0];
            if (ctr)
                ctr.remove();
        }
        static setListHeaderPadding(list) {
            let content = list.getElementsByClassName('c-dataview-content')[0];
            let height = content.getBoundingClientRect().height;
            content.style.overflow = 'visible';
            content.style.height = 'auto';
            let realHeight = content.getBoundingClientRect().height;
            ;
            content.style.overflow = 'auto';
            content.style.height = `${height}px`;
            let header = list.getElementsByClassName('c-dataview-header')[0];
            if (realHeight > height)
                header.style.paddingRight = '10px';
            else
                header.style.paddingRight = '0';
        }
        static bindListView(list) {
            caspian.common.setListHeaderPadding(list);
            const mutationObserver = new MutationObserver(t => {
                if (t.length > 0) {
                    let ctr = t[t.length - 1].target.closest('.c-widget.c-data-view');
                    caspian.common.setListHeaderPadding(ctr);
                }
            });
            mutationObserver.observe(list, {
                attributes: false,
                childList: true,
                subtree: true
            });
        }
        static bindTabpanel(tabpanel) {
            let basePos = tabpanel.getBoundingClientRect();
            let activeTab = tabpanel.getElementsByClassName('t-state-active')[0];
            if (activeTab) {
                let pos = activeTab.getBoundingClientRect();
                if (tabpanel.classList.contains('t-vertical'))
                    tabpanel.getElementsByClassName('c-selected-panel')[0].style.top = `${pos.top - basePos.top + 8}px`;
                else {
                    let seledtedPanel = tabpanel.getElementsByClassName('c-selected-panel')[0];
                    if (common.RightToLeft()) {
                        console.log(basePos.left);
                        seledtedPanel.style.left = `${pos.left - basePos.left + 3}px`;
                    }
                    else
                        seledtedPanel.style.left = `${pos.left - basePos.left + 3}px`;
                    seledtedPanel.style.width = `${pos.width - 8}px`;
                }
            }
        }
        static enableDefaultShortKey(status, dotnet) {
            if (status) {
                document.body.onkeyup = async (e) => {
                    let key = e.keyCode;
                    if (key == 13 || key == 27)
                        await dotnet.invokeMethodAsync("HideConfirm", key == 13);
                };
            }
            else
                document.body.onkeyup = null;
        }
        static bindDataGrid(grid) {
            new caspian.DataGrid(grid);
        }
        static bindBox() {
        }
        static bindWindow(win) {
            new caspian.Window(win);
        }
        static bindLookup(input, dotnet) {
            new caspian.Lookup(input, dotnet);
        }
        static bindTimepicker(element, dotnet) {
            new caspian.TimePicker(element, dotnet);
        }
        static bindColorPicker(element) {
            new caspian.ColorPicker(element);
        }
        static bindInputCollorPicker(element, dotnet) {
            new caspian.InputCollorPicker(element, dotnet);
        }
        static bindDropdownList(element, dotnet) {
            new caspian.DropdownList(element, dotnet);
        }
        static bindMultiSelect(element) {
            caspian.common.bindErrorMessage(element, element, 33);
            element.onfocus = () => {
                this.showErrorMessage(element, 33);
            };
            element.onblur = () => {
                this.hideErrorMessage(element);
            };
        }
        static bindContextMenu(element, dotnet) {
            document.body.onclick = async (e) => {
                if (e.target.closest('.c-context-menu-container') == null) {
                    document.body.onclick = null;
                    await dotnet.invokeMethodAsync('Close');
                }
            };
            const mutationObserver = new MutationObserver(list => {
                list.every(t => {
                    if (t.addedNodes.length == 1) {
                        let ctr = t.addedNodes[0];
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
                        setTimeout(() => ctr.style.marginTop = '-28px', 1);
                    }
                });
            });
            mutationObserver.observe(element, {
                attributes: false,
                childList: true,
                subtree: true,
            });
        }
        static bindPopupWindow(element, target, json, dotnet) {
            element.popupWindow = new caspian.PopupWindow(element, target, json, dotnet);
        }
        static updatePopupWindow(element, json) {
            element.popupWindow.updateLocation(element, json);
        }
        static bindComboBox(input, pageable, dotnet) {
            new caspian.ComboBox(input, pageable, dotnet);
        }
        static bindLookupTree(input, dotnet) {
            const mutationObserver = new MutationObserver(t => {
                let target = t[0].target;
                let targetLoc = target.getBoundingClientRect();
                let content = target.getElementsByClassName('c-tree-content')[0];
                if (content != null) {
                    let loc = target.getBoundingClientRect();
                    let tree = content.getElementsByClassName('c-treeview')[0];
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
                    document.body.onmousedown = async (e) => {
                        if (e.target.closest('.auto-hide') == null) {
                            document.body.onmousedown = null;
                            await dotnet.invokeMethodAsync("Close");
                        }
                    };
                }
            });
            mutationObserver.observe(input.closest('.c-lookup-tree'), {
                attributes: false,
                childList: true,
                subtree: false
            });
            let lookup = input.closest('.c-content');
            input.onfocus = e => {
                lookup.classList.add('c-state-focus');
                caspian.common.showErrorMessage(e.target.parentElement.parentElement);
            };
            input.onblur = e => {
                lookup.classList.remove('c-state-focus');
                caspian.common.hideErrorMessage(e.target.parentElement.parentElement);
            };
        }
        static bindMenu() {
            new caspian.Accordion(document.getElementById('accordion'), false);
        }
        static convertToInt(value) {
            if (!value)
                return null;
            return parseFloat(value.substring(0, value.length - 2));
        }
        static bindTextBox(input) {
            new caspian.TextBox(input, 'numeric', null);
        }
        static bindStringbox(input, regularExpression) {
            new caspian.TextBox(input, 'string', regularExpression);
        }
        static async bindFileDownload(fileName, contentStreamReference) {
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
        static bindMask(el, patern) {
            const pattern = patern, slots = new Set(el.dataset.slots || "_"), prev = (j => Array.from(pattern, (c, i) => slots.has(c) ? j = i + 1 : j))(0), first = [...pattern].findIndex(c => slots.has(c)), accept = new RegExp(el.dataset.accept || "\\d", "g"), clean = input => {
                input = input.match(accept) || [];
                return Array.from(pattern, c => input[0] === c || slots.has(c) ? input.shift() || c : c);
            }, format = () => {
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
        static focus(element) {
            element.focus();
        }
        static async bindImage(pic, imageStream) {
            if (imageStream) {
                const arrayBuffer = await imageStream.arrayBuffer();
                const blob = new Blob([arrayBuffer]);
                pic.src = URL.createObjectURL(blob);
            }
            else
                pic.src = '';
        }
    }
    caspian.common = common;
})(caspian || (caspian = {}));
//# sourceMappingURL=caspian.common.js.map