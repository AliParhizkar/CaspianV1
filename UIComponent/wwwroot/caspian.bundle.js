var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var caspian;
(function (caspian) {
    class ColorPicker {
        constructor(element) {
            this.element = element;
            this.transparentStrip = element.getElementsByClassName('c-transparent-strip')[0];
            this.alphaInput = this.transparentStrip.getElementsByTagName('input')[0];
            this.colorInput = element.querySelector('.c-color-number input');
            this.displayer = element.getElementsByClassName('c-color-displayer')[0];
            this.colorBlock = element.getElementsByClassName('c-color-block')[0];
            this.hueInput = element.querySelector('.c-colors-hue input');
            this.selector = element.getElementsByClassName('c-color-selector')[0];
            this.colorBlock.onmousedown = e => {
                this.startDrag(e.clientX, e.clientY, e.layerX, e.layerY);
                this.selector.style.left = `${e.layerX - 7}px`;
                this.selector.style.top = `${e.layerY - 7}px`;
            };
            this.selector.onmousedown = e => {
                let loc = this.element.getBoundingClientRect();
                let locSelector = this.selector.getBoundingClientRect();
                let left = locSelector.left - loc.left + 7, top = locSelector.top - loc.top + 7;
                this.startDrag(e.clientX, e.clientY, left, top);
            };
            element.getElementsByClassName('c-color-bar')[0].oninput = e => {
                this.hue = parseInt(e.target.value);
                this.update();
            };
            this.alphaInput.oninput = e => {
                this.alpha = parseInt(e.target.value) / 100;
                this.update();
            };
            this.element.querySelectorAll('.c-colors-palette .c-color').forEach(t => {
                t.onclick = e => {
                    let color = e.target.style.backgroundColor;
                    let [r, g, b, a] = color.replace('rgba', '').replace('rgb', '').replace('(', '').replace(')', '').split(',');
                    let attrs = this.element.attributes;
                    attrs['red'].value = r;
                    attrs['green'].value = g;
                    attrs['blue'].value = b;
                    attrs['alpha'].value = a || 1;
                    this.updateColor();
                };
            });
            this.updateColor();
            this.bindAttributes(element);
        }
        bindColor() {
            let color = this.displayer.style.backgroundColor;
            let [r, g, b, a] = color.replace('rgba', '').replace('rgb', '').replace('(', '').replace(')', '').split(',');
            this.red = parseInt(r);
            this.green = parseInt(g);
            this.blue = parseInt(b);
            ;
            this.alpha = parseFloat(a) || 1;
            let input = this.element.querySelector('input[type="hidden"]');
            input.value = color;
            let event = new Event('change');
            input.dispatchEvent(event);
            this.displayer.onclick = () => {
                this.bindColor();
            };
        }
        update() {
            let [r, g, b] = this.convertHSVtoRGB(this.hue, 100, 100);
            let color = `rgb(${r}, ${g}, ${b})`;
            this.colorBlock.style.backgroundColor = color;
            [r, g, b] = this.convertHSVtoRGB(this.hue, this.saturation, this.value);
            this.transparentStrip.style.backgroundImage = `linear-gradient(15deg, transparent, rgb(${r}, ${g}, ${b}))`;
            color = this.alpha == 1 ? `rgb(${r}, ${g}, ${b})` : `rgba(${r}, ${g}, ${b}, ${this.alpha})`;
            this.displayer.style.backgroundColor = color;
            this.colorInput.value = color;
            if (this.bindingType == 2)
                this.bindColor();
        }
        startDrag(clientX, clientY, x, y) {
            this.xStart = clientX;
            this.yStart = clientY;
            this.leftStart = x - 7;
            this.topStart = y - 7;
            document.body.onmousemove = e => {
                let loc = this.colorBlock.getBoundingClientRect();
                let difX = e.clientX - this.xStart, difY = e.clientY - this.yStart;
                let left = this.leftStart + difX;
                if (left < -7)
                    left = -7;
                if (left > loc.width - 7)
                    left = loc.width - 7;
                this.selector.style.left = `${left}px`;
                let top = this.topStart + difY;
                if (top <= -7)
                    top = -7;
                if (top > loc.height - 7)
                    top = loc.height - 7;
                this.selector.style.top = `${top}px`;
                this.saturation = Math.floor((left + 7) / loc.width * 100);
                this.value = 100 - Math.floor((top + 7) / loc.height * 100);
                this.update();
            };
            document.body.onmouseup = () => {
                document.body.onmouseup = document.body.onmousemove = null;
            };
        }
        updateColor() {
            let attrs = this.element.attributes;
            this.bindingType = parseInt(attrs['bindingType'].value);
            let red = parseInt(attrs['red'].value);
            let green = parseInt(attrs['green'].value);
            let blue = parseInt(attrs['blue'].value);
            let alpha = parseFloat(attrs['alpha'].value);
            if (this.red != red || this.green != green || this.blue != blue || this.alpha != alpha) {
                this.transparentStrip.style.backgroundImage = `linear-gradient(15deg, transparent, rgb(${red}, ${green}, ${blue}))`;
                this.red = red;
                this.green = green;
                this.blue = blue;
                this.alpha = alpha || 1;
                this.alphaInput.value = (100 * this.alpha).toString();
                let color = alpha == 1 ? `rgb(${red}, ${green}, ${blue})` : `rgba(${red}, ${green}, ${blue}, ${alpha})`;
                this.colorInput.value = color;
                this.displayer.style.backgroundColor = color;
                let [h, s, v] = this.convertRGBAtoHSVA();
                this.hue = h;
                this.saturation = s;
                this.value = v;
                let [r, g, b] = this.convertHSVtoRGB(h, 100, 100);
                color = `rgb(${r}, ${g}, ${b})`;
                this.colorBlock.style.backgroundColor = color;
                this.hueInput.value = h.toString();
                let loc = this.colorBlock.getBoundingClientRect();
                let left = Math.floor(loc.width * s / 100) - 7, top = Math.floor(loc.height * (100 - v) / 100) - 7;
                this.selector.style.left = `${left}px`;
                this.selector.style.top = `${top}px`;
            }
        }
        convertRGBAtoHSVA() {
            const red = this.red / 255;
            const green = this.green / 255;
            const blue = this.blue / 255;
            const xmax = Math.max(red, green, blue);
            const xmin = Math.min(red, green, blue);
            const chroma = xmax - xmin;
            const value = xmax;
            let hue = 0;
            let saturation = 0;
            if (chroma) {
                if (xmax === red) {
                    hue = ((green - blue) / chroma);
                }
                if (xmax === green) {
                    hue = 2 + (blue - red) / chroma;
                }
                if (xmax === blue) {
                    hue = 4 + (red - green) / chroma;
                }
                if (xmax) {
                    saturation = chroma / xmax;
                }
            }
            hue = Math.floor(hue * 60);
            return [hue < 0 ? hue + 360 : hue, Math.round(saturation * 100), Math.round(value * 100)];
        }
        convertHSVtoRGB(h, s, v) {
            const saturation = s / 100;
            const value = v / 100;
            let chroma = saturation * value;
            let hueBy60 = h / 60;
            let x = chroma * (1 - Math.abs(hueBy60 % 2 - 1));
            let m = value - chroma;
            chroma = (chroma + m);
            x = (x + m);
            const index = Math.floor(hueBy60) % 6;
            const red = [chroma, x, m, m, x, chroma][index];
            const green = [x, chroma, chroma, x, m, m][index];
            const blue = [m, m, x, chroma, chroma, x][index];
            return [Math.round(red * 255), Math.round(green * 255), Math.round(blue * 255)];
        }
        bindAttributes(element) {
            const mutationObserver = new MutationObserver((mutationList) => {
                if (mutationList[0].attributeName != 'id')
                    this.updateColor();
            });
            mutationObserver.observe(element, {
                attributes: true,
                childList: false,
                subtree: false
            });
        }
    }
    caspian.ColorPicker = ColorPicker;
})(caspian || (caspian = {}));
var caspian;
(function (caspian) {
    class ComboBox {
        constructor(input, Pageable, dotnet) {
            this.bindObserver(input, Pageable, dotnet);
            let control = input.closest('.t-combobox').getElementsByClassName('t-dropdown-wrap')[0];
            input.onkeyup = e => {
                if (e.key == 'ArrowDown' || e.key == 'ArrowUp') {
                    let selected = e.target.closest('.t-combobox').getElementsByClassName('t-state-selected')[0];
                    if (selected) {
                        let content = e.target.closest('.t-combobox').getElementsByClassName('t-group')[0];
                        let loc = selected.getBoundingClientRect();
                        let top = loc.top - content.getBoundingClientRect().top, bottom = top + loc.height;
                        if (bottom > 240 || top < 10) {
                            let scrollTop = content.scrollTop;
                            content.scrollTop = (scrollTop + bottom - 240);
                        }
                    }
                }
            };
            input.closest('.t-combobox').onmouseenter = () => {
                if (!control.classList.contains('t-state-disabled')) {
                    let list = control.classList;
                    list.add('t-state-hover');
                    list.remove('t-state-default');
                }
            };
            input.closest('.t-combobox').onmouseleave = () => {
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
                caspian.common.showErrorMessage(input.closest('.t-widget'));
            };
            input.onblur = () => {
                let list = control.classList;
                list.add('t-state-default');
                list.remove('t-state-focused');
                caspian.common.hideErrorMessage(input.closest('.t-widget'));
            };
        }
        bindObserver(input, pageable, dotnet) {
            const mutationObserver = new MutationObserver(t => {
                let ctr = t[0].target;
                let group = ctr.getElementsByClassName('t-group')[0];
                if (group) {
                    if (pageable) {
                        group.onscrollend = () => __awaiter(this, void 0, void 0, function* () {
                            yield dotnet.invokeMethodAsync('IncPageNumberInvokable');
                        });
                    }
                    let animate = ctr.getElementsByClassName('t-animation-container')[0];
                    let height = group.getElementsByClassName('t-reset')[0].getBoundingClientRect().height;
                    height = Math.min(250, height);
                    height = Math.max(height, 30);
                    let loc = group.getBoundingClientRect().top + height + 5;
                    animate.style.height = `${height + 7}px`;
                    animate.style.width = `${ctr.getBoundingClientRect().width + 7}px`;
                    if (loc > window.innerHeight / 2) {
                        animate.classList.add('c-animate-up');
                        setTimeout(() => group.style.bottom = '0', 10);
                        animate.style.marginTop = `${-height - 42}px`;
                    }
                    else {
                        animate.classList.add('c-animate-down');
                        setTimeout(() => group.style.top = '0', 10);
                    }
                    document.body.onmousedown = (e) => __awaiter(this, void 0, void 0, function* () {
                        if (e.target.closest('.t-group') == null)
                            yield dotnet.invokeMethodAsync('CloseInvokable');
                    });
                }
                else
                    document.body.onmousedown = null;
            });
            mutationObserver.observe(input.closest('.t-combobox'), {
                attributes: false,
                childList: true,
                subtree: false
            });
        }
    }
    caspian.ComboBox = ComboBox;
})(caspian || (caspian = {}));
var caspian;
(function (caspian) {
    class common {
        static RightToLeft() {
            return document.getElementsByTagName('body')[0].classList.contains('t-rtl');
        }
        static bindCheclistDropdown(element, dotnet) {
            const mutationObserver = new MutationObserver(t => {
                let element = t[0].target;
                if (element.classList.contains('c-checkbox-list'))
                    element = element.parentElement;
                if (element.classList.contains('t-checkbox-list')) {
                    let loc = element.getBoundingClientRect();
                    let animate = element.closest('.t-animation-container');
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
            window.addEventListener("click", (e) => __awaiter(this, void 0, void 0, function* () {
                let elem = e.target.closest('.t-dropdown');
                if (elem == null || elem != windowElement)
                    yield dotnet.invokeMethodAsync('CloseWindow');
            }));
        }
        static bindWindowClick(dotnet) {
            let main = document.getElementsByClassName('c-content-main')[0];
            main = main || document.body;
            main.onmousedown = (e) => __awaiter(this, void 0, void 0, function* () {
                if (e.target.closest('.auto-hide') == null)
                    yield dotnet.invokeMethodAsync('WindowClick');
            });
            window.addEventListener("locationchange", this.onMousedownHandler);
        }
        static bindSlider(element, dotnet) {
            return __awaiter(this, void 0, void 0, function* () {
                let containerWidth = element.getBoundingClientRect().width;
                let slide = element.getElementsByClassName('c-slider-slide')[0];
                let slideWidth = slide.getBoundingClientRect().width;
                element.getElementsByClassName('c-slider-body')[0].style.width = `${slideWidth}px`;
                element.getElementsByClassName('c-slider-content')[0].style.left = `${-slideWidth}px`;
                yield dotnet.invokeMethodAsync('SetData', containerWidth, slideWidth);
                //window.addEventListener('resize', async () => {
                //});
                //window.addEventListener('locationchange', () => window.removeEventListener('loc'))
            });
        }
        static bindTree(tree) {
            debugger;
        }
        static onWindowResizeHandler(element, dotnet) {
            return __awaiter(this, void 0, void 0, function* () {
                let containerWidth = element.getBoundingClientRect().width;
                let slide = element.getElementsByClassName('c-slider-slide')[0];
                let slideWidth = slide.getBoundingClientRect().width;
                yield dotnet.invokeMethodAsync('SetData', containerWidth, slideWidth);
            });
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
        static showErrorMessage(element) {
            let error = element.getElementsByClassName('errorMessage')[0];
            if (error)
                error.remove();
            let msg = element.attributes['error-message'];
            if (msg) {
                let htmlString = '<div class="errorMessage"></div>';
                var messageBox = document.createElement('div');
                messageBox.classList.add('errorMessage');
                messageBox.innerHTML = '<span class="c-icon"><i class="fa fa-info" aria-hidden="true"></i></span><Span class="c-content">'
                    + msg.value + '</Span><span class="c-pointer"></span>';
                element.append(messageBox);
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
                header.style.paddingRight = '11px';
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
            let pos = activeTab.getBoundingClientRect();
            if (tabpanel.classList.contains('t-vertical'))
                tabpanel.getElementsByClassName('c-selected-panel')[0].style.top = `${pos.top - basePos.top + 8}px`;
            else {
                let seledtedPanel = tabpanel.getElementsByClassName('c-selected-panel')[0];
                seledtedPanel.style.left = `${pos.left - basePos.left + 3}px`;
                seledtedPanel.style.width = `${pos.width - 8}px`;
            }
        }
        static enableDefaultShortKey(status, dotnet) {
            if (status) {
                document.body.onkeyup = (e) => __awaiter(this, void 0, void 0, function* () {
                    let key = e.keyCode;
                    if (key == 13 || key == 27)
                        yield dotnet.invokeMethodAsync("HideConfirm", key == 13);
                });
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
            element.onfocus = () => {
                this.showErrorMessage(element);
            };
            element.onblur = () => {
                this.hideErrorMessage(element);
            };
        }
        static bindContextMenu(element, dotnet) {
            document.body.onclick = (e) => __awaiter(this, void 0, void 0, function* () {
                if (e.target.closest('.c-context-menu-container') == null) {
                    document.body.onclick = null;
                    yield dotnet.invokeMethodAsync('Close');
                }
            });
            const mutationObserver = new MutationObserver((list) => {
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
            new caspian.PopupWindow(element, target, json, dotnet);
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
                    document.body.onmousedown = (e) => __awaiter(this, void 0, void 0, function* () {
                        if (e.target.closest('.auto-hide') == null) {
                            document.body.onmousedown = null;
                            yield dotnet.invokeMethodAsync("Close");
                        }
                    });
                }
            });
            mutationObserver.observe(input.closest('.c-lookup-tree'), {
                attributes: false,
                childList: true,
                subtree: false
            });
            let lookup = input.closest('.c-content');
            input.onfocus = () => {
                lookup.classList.add('c-state-focus');
            };
        }
        static bindMenu() {
            new caspian.Accordion(document.getElementById('accordion'), false);
        }
        static bindTextBox(input) {
            new caspian.TextBox(input, 'numeric');
        }
        static bindStringbox(input) {
            new caspian.TextBox(input, 'string');
        }
        static bindFileDownload(fileName, contentStreamReference) {
            return __awaiter(this, void 0, void 0, function* () {
                const arrayBuffer = yield contentStreamReference.arrayBuffer();
                const blob = new Blob([arrayBuffer]);
                const url = URL.createObjectURL(blob);
                const anchorElement = document.createElement('a');
                anchorElement.href = url;
                anchorElement.download = fileName !== null && fileName !== void 0 ? fileName : '';
                anchorElement.click();
                anchorElement.remove();
                URL.revokeObjectURL(url);
            });
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
        static showMessage(message) {
            if (this.infoTimer)
                clearTimeout(this.infoTimer);
            let box = document.getElementById('outMessage');
            if (box)
                box.remove();
            let odv = document.createElement('div');
            odv.id = 'outMessage';
            odv.className = 't-widget t-message';
            odv.innerHTML = `<div class="t-window-titlebar"><span class="t-title">Info</span><span class="t-close"><i class="fa fa-close"></i></span></div><div class="c-content">${message}</div>`;
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
            }, 4000);
        }
        static hideMessage() {
            if (this.infoTimer)
                clearTimeout(this.infoTimer);
            let box = document.getElementById('outMessage').style.top = '-130px';
            setTimeout(() => {
                document.getElementById('outMessage').remove();
            }, 300);
        }
        static bindImage(pic, imageStream) {
            return __awaiter(this, void 0, void 0, function* () {
                if (imageStream) {
                    const arrayBuffer = yield imageStream.arrayBuffer();
                    const blob = new Blob([arrayBuffer]);
                    pic.src = URL.createObjectURL(blob);
                }
                else
                    pic.src = '';
            });
        }
    }
    caspian.common = common;
})(caspian || (caspian = {}));
// Implement the Extension
HTMLCollection.prototype.indexOf = function (element) {
    let items = this;
    for (let index = 0; index < items.length; index++)
        if (items[index] == element)
            return index;
    return -1;
};
HTMLElement.prototype.getPosition = function () {
    let parent = this, left = 0, top = 0;
    let rect = parent.getBoundingClientRect();
    while (parent != document.body) {
        left += parent.offsetLeft;
        top += parent.offsetTop;
        parent = parent.offsetParent;
    }
    return new DOMRect(left, top, rect.width, rect.height);
};
var caspian;
(function (caspian) {
    class DataGrid {
        constructor(grv) {
            this.grid = grv;
            this.bindObserver();
            grv.getElementsByClassName('t-grid-content')[0].onscroll = e => {
                let target = e.target;
                target.closest('.t-grid').getElementsByClassName('t-grid-header-wrap')[0].scrollLeft = target.scrollLeft;
            };
        }
        bindObserver() {
            const resizeObserver = new ResizeObserver(entries => {
                let grv = this.grid;
                for (let entry of entries) {
                    if (entry.contentBoxSize && entry.contentBoxSize[0]) {
                        let contentHeight = grv.getElementsByClassName('t-grid-content')[0].getBoundingClientRect().height;
                        let table = grv.getElementsByClassName('t-grid-content')[0].getElementsByTagName('table')[0];
                        if (table) {
                            let tableHeight = table.getBoundingClientRect().height;
                            let header = grv.getElementsByClassName('t-grid-header')[0];
                            if (contentHeight < tableHeight) {
                                if (caspian.common.RightToLeft())
                                    header.style.paddingLeft = '11px';
                                else
                                    header.style.paddingRight = '11px';
                            }
                            else {
                                if (caspian.common.RightToLeft())
                                    header.style.paddingLeft = '0';
                                else
                                    header.style.paddingRight = '0';
                            }
                        }
                    }
                }
            });
            resizeObserver.observe(this.grid.getElementsByClassName('t-grid-content')[0]);
        }
        columnResize() {
            let head = this.grid.getElementsByClassName('t-grid-header-wrap')[0];
            head.onmousemove = e => {
                let element = e.target;
                if (element.tagName != 'th')
                    element = element.closest('th');
                if (element) {
                    let loc = element.getBoundingClientRect(), x = e.clientX;
                    if ((x - loc.left) < 5 || (loc.right - x) < 5) {
                        e.target.style.cursor = 'col-resize';
                        this.resize = true;
                    }
                    else {
                        e.target.style.cursor = '';
                        this.resize = false;
                    }
                }
            };
            head.onmousedown = e => {
                if (this.resize) {
                    let rtl = caspian.common.RightToLeft();
                    let element = e.target;
                    if (element.tagName != 'th')
                        element = element.closest('th');
                    let loc = element.getBoundingClientRect(), x = e.clientX;
                    this.curent = element;
                    this.curentWidth = loc.width;
                    let other = null;
                    if (x - loc.left < 5 && rtl || loc.right - x < 5 && !rtl) {
                        other = element.nextSibling;
                        this.gridStatus = 1;
                    }
                    if (loc.right - x < 5 && rtl || x - loc.left < 5 && !rtl) {
                        other = element.previousSibling;
                        this.gridStatus = 2;
                    }
                    this.other = other;
                    if (other == null)
                        this.gridStatus = 3;
                    else
                        this.otherWidth = other.getBoundingClientRect().width;
                    this.xStart = e.clientX;
                }
                window.onclick = this.drop;
                window.onmousemove = this.dragging;
            };
        }
        dragging(e) {
            if (this.other == null)
                return;
            let dif = this.xStart - e.clientX;
            if (this.gridStatus == 2)
                dif = -dif;
            let curentWidth = this.curentWidth;
            let otherWidth = this.otherWidth - 1;
            let curentResult = curentWidth - dif, otherResult = otherWidth + dif;
            if (curentResult < 30 || otherResult < 30)
                return;
            this.curent.style.width = `${curentResult}px`;
            this.other.style.width = `${otherResult}px`;
            let columns = this.grid.getElementsByClassName('t-grid-header-wrap')[0].getElementsByTagName("tr")[0].children;
            let curentIndex = columns.indexOf(this.curent);
            let otherIndex = columns.indexOf(this.other);
            columns = this.grid.getElementsByClassName('t-grid-content')[0].getElementsByTagName('tr')[0].children;
            columns.item(curentIndex).style.width = `${curentResult}px`;
            columns.item(otherIndex).style.width = `${otherResult}px`;
            let contentHeight = this.grid.getElementsByClassName('t-grid-content')[0].getBoundingClientRect().height;
            let tableHeight = this.grid.getElementsByClassName('c-grid-items')[0].getBoundingClientRect().height;
            let header = this.grid.getElementsByClassName('t-grid-header')[0];
            if (contentHeight < tableHeight) {
                if (caspian.common.RightToLeft())
                    header.style.paddingLeft = '11px';
                else
                    header.style.paddingRight = '11px';
            }
            else {
                header.style.paddingLeft = '0';
                header.style.paddingRight = '0';
            }
        }
        drop() {
            window.onmousemove = null;
            window.onclick = null;
        }
    }
    caspian.DataGrid = DataGrid;
})(caspian || (caspian = {}));
var caspian;
(function (caspian) {
    class DatePicker {
        constructor(element, dotnet) {
            element.focus;
            let input = element.getElementsByTagName('input')[0];
            element.onmouseenter = e => {
                let elem = e.target.getElementsByClassName('t-picker-wrap')[0];
                if (!elem.classList.contains('t-state-selected') || !elem.classList.contains('t-state-disabled'))
                    elem.classList.add('t-state-hover');
            };
            element.onmouseleave = e => {
                let elem = e.target.getElementsByClassName('t-picker-wrap')[0];
                elem.classList.remove('t-state-hover');
            };
            input.onfocus = e => {
                let elem = e.target;
                elem.closest('.t-picker-wrap').classList.replace('t-state-hover', 't-state-selected');
                caspian.common.showErrorMessage(elem.closest('.t-widget'));
            };
            input.onblur = e => {
                let elem = e.target;
                elem.closest('.t-picker-wrap').classList.remove('t-state-selected');
                caspian.common.hideErrorMessage(elem.closest('.t-widget'));
            };
            this.bindObserver(element, dotnet);
        }
        bindObserver(element, dotnet) {
            const mutationObserver = new MutationObserver(t => {
                let element = t[0].target, animate = t[0].addedNodes[0];
                if (animate) {
                    let calendar = animate.getElementsByClassName('t-datepicker-calendar')[0];
                    if (element.getBoundingClientRect().top > window.outerHeight / 2) {
                        animate.classList.add('c-animate-up');
                        animate.style.marginTop = '-275px';
                        setTimeout(() => __awaiter(this, void 0, void 0, function* () { return calendar.style.bottom = '-0'; }), 20);
                    }
                    else {
                        animate.classList.add('c-animate-down');
                        setTimeout(() => __awaiter(this, void 0, void 0, function* () { return calendar.style.top = '0'; }), 20);
                    }
                    document.body.onmousedown = (e) => __awaiter(this, void 0, void 0, function* () {
                        if (e.target.closest('.t-animation-container') == null)
                            yield dotnet.invokeMethodAsync('CloseWindow');
                    });
                }
                else
                    document.body.onmousedown = null;
            });
            mutationObserver.observe(element, {
                attributes: false,
                childList: true,
                subtree: false
            });
        }
        static bindCalendar(elm, viewType, vNavigation) {
            let toDownState = elm.getElementsByClassName('c-down-to-state')[0];
            let toUpState = elm.getElementsByClassName('c-up-to-state')[0];
            let fromDownState = elm.getElementsByClassName('c-down-from-state')[0];
            let fromUpState = elm.getElementsByClassName('c-up-from-state')[0];
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
    }
    caspian.DatePicker = DatePicker;
    let ViewType;
    (function (ViewType) {
        ViewType[ViewType["Month"] = 1] = "Month";
        ViewType[ViewType["Year"] = 2] = "Year";
        ViewType[ViewType["Decade"] = 3] = "Decade";
        ViewType[ViewType["Century"] = 4] = "Century";
    })(ViewType || (ViewType = {}));
    let VNavigation;
    (function (VNavigation) {
        VNavigation[VNavigation["Up"] = 1] = "Up";
        VNavigation[VNavigation["Down"] = 2] = "Down";
    })(VNavigation || (VNavigation = {}));
})(caspian || (caspian = {}));
var caspian;
(function (caspian) {
    class DropdownList {
        constructor(element, dotnet) {
            this.bindObserver(element, dotnet);
            element.onmouseenter = e => {
                let ddl = e.target.getElementsByClassName('t-dropdown-wrap')[0];
                if (!ddl.classList.contains('t-state-disabled')) {
                    ddl.classList.remove('t-state-default');
                    ddl.classList.add('t-state-hover');
                }
            };
            element.onmouseleave = e => {
                let ddl = e.target.getElementsByClassName('t-dropdown-wrap')[0];
                ddl.classList.remove('t-state-hover');
                ddl.classList.add('t-state-default');
            };
            element.onfocus = e => {
                let ddl = e.target.getElementsByClassName('t-dropdown-wrap')[0];
                ddl.classList.remove('t-state-default');
                ddl.classList.add('t-state-focused');
                caspian.common.showErrorMessage(e.target);
            };
            element.onblur = () => {
                let ddl = element.getElementsByClassName('t-dropdown-wrap')[0];
                ddl.classList.remove('t-state-focused');
                ddl.classList.add('t-state-default');
                caspian.common.hideErrorMessage(element);
            };
        }
        bindObserver(element, dotnet) {
            const mutationObserver = new MutationObserver(t => {
                let ddl = t[0].target;
                let animate = ddl.getElementsByClassName('t-animation-container')[0];
                if (animate != null) {
                    animate.style.width = `${ddl.getBoundingClientRect().width + 4}px`;
                    let group = animate.getElementsByClassName('t-group')[0];
                    let height = group.getBoundingClientRect().height;
                    animate.style.height = `${height + 5}px`;
                    if (ddl.getBoundingClientRect().top > window.innerHeight / 2) {
                        animate.classList.add('c-animate-up');
                        animate.style.marginTop = `${-height - 38}px`;
                        setTimeout(() => group.style.bottom = '0', 50);
                    }
                    else {
                        animate.classList.add('c-animate-down');
                        setTimeout(() => group.style.top = '0', 50);
                    }
                    document.body.onclick = (e) => __awaiter(this, void 0, void 0, function* () {
                        if (e.target.closest('t-animation-container') == null) {
                            document.body.onclick = null;
                            yield dotnet.invokeMethodAsync('CloseWindow');
                        }
                    });
                }
            });
            mutationObserver.observe(element, {
                attributes: false,
                childList: true,
                subtree: false
            });
        }
    }
    caspian.DropdownList = DropdownList;
})(caspian || (caspian = {}));
var caspian;
(function (caspian) {
    class InputCollorPicker {
        constructor(element, dotnet) {
            this.bindObserver(element, dotnet);
            element.onmouseenter = () => {
                element.getElementsByClassName('c-input-color')[0].classList.add('c-state-hover');
            };
            element.onmouseleave = () => {
                element.getElementsByClassName('c-input-color')[0].classList.remove('c-state-hover');
            };
            element.onfocus = () => {
                element.getElementsByClassName('c-input-color')[0].classList.add('c-state-focused');
            };
            element.onblur = () => {
                element.getElementsByClassName('c-input-color')[0].classList.remove('c-state-focused');
            };
        }
        bindObserver(element, dotnet) {
            const mutationObserver = new MutationObserver(t => {
                let elem = t[0].target;
                let animate = elem.getElementsByClassName('t-animation-container')[0];
                if (animate != null) {
                    let top = elem.getBoundingClientRect().top;
                    let picker = elem.getElementsByClassName('c-colorpicker-panel')[0];
                    animate.style.width = '245px';
                    let height = picker.getBoundingClientRect().height;
                    animate.style.height = `${height + 6}px`;
                    if (top > window.innerHeight / 2) {
                        animate.classList.add('c-animation-up');
                        animate.style.marginTop = `${-height - 41}px`;
                        setTimeout(t => {
                            picker.style.bottom = '0';
                        }, 10);
                    }
                    else {
                        animate.classList.add('c-animation-down');
                        setTimeout(t => {
                            picker.style.top = '0px';
                        }, 10);
                    }
                    document.body.onclick = (e) => __awaiter(this, void 0, void 0, function* () {
                        if (e.target.closest('.t-animation-container') == null)
                            yield dotnet.invokeMethodAsync("Close");
                    });
                }
                else
                    document.body.onclick = null;
            });
            mutationObserver.observe(element, {
                attributes: false,
                childList: true,
                subtree: false
            });
        }
    }
    caspian.InputCollorPicker = InputCollorPicker;
})(caspian || (caspian = {}));
var caspian;
(function (caspian) {
    class Lookup {
        constructor(input, dotnet) {
            let lookup = input.closest('.c-lookup');
            input.onfocus = () => {
                caspian.common.showErrorMessage(lookup);
            };
            input.onkeydown = e => {
                let code = e.keyCode;
                if (code == 40 || code == 38)
                    e.preventDefault();
            };
            input.onblur = () => {
                caspian.common.hideErrorMessage(lookup);
            };
            //if (lookup.attributes['closeonblur'].value != undefined)
            //    lookup.attributes['tabindex'].value = '0';
            this.bindObserver(lookup, dotnet);
        }
        bindObserver(lookup, dotnet) {
            const mutationObserver = new MutationObserver(list => {
                let target = list[0].target;
                let helpWindow = target.getElementsByClassName('t-HelpWindow')[0];
                if (helpWindow != null) {
                    let animate = target.getElementsByClassName('t-animation-container')[0];
                    let locTarget = target.getBoundingClientRect();
                    let locHelpWindow = helpWindow.getBoundingClientRect();
                    let left = (locHelpWindow.width - locTarget.width) / 2;
                    let posTarget = target.getPosition();
                    if (posTarget.left - left + locHelpWindow.width > window.innerWidth)
                        left = locHelpWindow.width - (window.innerWidth - posTarget.left) + 20;
                    animate.style.marginLeft = `${-left}px`;
                    animate.style.width = `${locHelpWindow.width + 10}px`;
                    animate.style.height = `${locHelpWindow.height - 20}px`;
                    if (locTarget.bottom + locHelpWindow.height - 30 <= window.innerHeight) {
                        animate.classList.add('c-animate-down');
                        setTimeout(() => helpWindow.style.top = '0', 25);
                    }
                    else if (locTarget.top >= locHelpWindow.height - 30) {
                        animate.style.marginTop = `${-locHelpWindow.height - 15}px`;
                        animate.classList.add('c-animate-up');
                        setTimeout(() => helpWindow.style.bottom = '0', 25);
                    }
                    else {
                        debugger;
                        helpWindow.style.top = `${-locHelpWindow.height}`;
                    }
                    if (lookup.attributes['autoHide']) {
                        window.onclick = function (e) {
                            return __awaiter(this, void 0, void 0, function* () {
                                if (!e.target.closest('.c-lookup'))
                                    yield dotnet.invokeMethodAsync('Close');
                            });
                        };
                    }
                }
                else
                    window.onclick = null;
            });
            mutationObserver.observe(lookup, {
                attributes: false,
                childList: true,
                subtree: false
            });
        }
    }
    caspian.Lookup = Lookup;
})(caspian || (caspian = {}));
var caspian;
(function (caspian) {
    class Accordion {
        constructor(el, multiple) {
            this.el = el;
            this.multiple = multiple || false;
            el.querySelectorAll('.submenu li').forEach(li => {
                li.onclick = e => {
                    el.querySelector('.submenu .selected').classList.remove('selected');
                    e.target.closest('li').classList.add('selected');
                };
            });
            el.querySelectorAll('.default .link').forEach(elem => {
                elem.onclick = e => {
                    this.setOpenMenusHeight();
                    let target = e.target.closest('.default');
                    let height = null;
                    if (!target.classList.contains('open')) {
                        let submenu = target.querySelector('.submenu');
                        submenu.style.height = 'auto';
                        height = submenu.getBoundingClientRect().height;
                        submenu.style.height = '0';
                    }
                    setTimeout(() => {
                        this.toggleSubmen(target, height);
                    }, 1);
                };
            });
        }
        setOpenMenusHeight() {
            this.el.querySelectorAll('.default.open').forEach(elem => {
                let open = elem.querySelector('.submenu');
                let height = open.getBoundingClientRect().height;
                open.style.height = `${height}px`;
            });
        }
        toggleSubmen(menu, height) {
            if (this.multiple) {
            }
            else {
                let opendMenu = this.el.querySelector('.default.open');
                if (opendMenu != menu)
                    this.openSubmenu(menu, height);
                if (opendMenu != null)
                    this.closeSubmenu(opendMenu);
            }
        }
        closeSubmenu(category) {
            category.classList.remove('open');
            category.querySelector('.submenu').style.height = '0';
        }
        openSubmenu(category, height) {
            category.classList.add('open');
            let submenu = category.querySelector('.submenu');
            submenu.style.height = `${height}px`;
        }
    }
    caspian.Accordion = Accordion;
})(caspian || (caspian = {}));
var caspian;
(function (caspian) {
    class PopupWindow {
        constructor(element, target, json, dotnet) {
            let data = JSON.parse(json);
            element.style.display = 'block';
            let className = element.className;
            element.className = 'auto-hide c-popup-window';
            let loc = element.getBoundingClientRect();
            let mainLoc = document.getElementsByClassName('c-content-main')[0].getBoundingClientRect();
            element.className = className;
            if (target)
                this.bindTarget(element, target, data);
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
            document.body.onmousedown = (e) => __awaiter(this, void 0, void 0, function* () {
                if (e.target.closest('.auto-hide') == null) {
                    document.body.onmousedown = null;
                    yield dotnet.invokeMethodAsync('Close');
                }
            });
        }
        bindTarget(element, target, data) {
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
    }
    caspian.PopupWindow = PopupWindow;
    let HorizontalAnchor;
    (function (HorizontalAnchor) {
        HorizontalAnchor[HorizontalAnchor["Left"] = 1] = "Left";
        HorizontalAnchor[HorizontalAnchor["Center"] = 2] = "Center";
        HorizontalAnchor[HorizontalAnchor["Right"] = 3] = "Right";
    })(HorizontalAnchor || (HorizontalAnchor = {}));
    let VerticalAnchor;
    (function (VerticalAnchor) {
        VerticalAnchor[VerticalAnchor["Top"] = 1] = "Top";
        VerticalAnchor[VerticalAnchor["Middle"] = 2] = "Middle";
        VerticalAnchor[VerticalAnchor["Bottom"] = 3] = "Bottom";
    })(VerticalAnchor || (VerticalAnchor = {}));
})(caspian || (caspian = {}));
var caspian;
(function (caspian) {
    class TextBox {
        constructor(input, type) {
            this.input = input;
            this.total || (this.total = 8);
            input.onmouseenter = () => {
                input.parentElement.classList.add('t-state-hover');
            };
            input.onmouseleave = () => {
                input.parentElement.classList.remove('t-state-hover');
            };
            this.readAttributes();
            this.bindAttributes();
            if (type != 'string')
                input.onkeypress = e => this.bindKeypress(e);
            input.onfocus = () => {
                setTimeout(() => {
                    this.input.select();
                }, 100);
                caspian.common.showErrorMessage(this.input.closest('.t-widget'));
            };
            input.onblur = () => {
                caspian.common.hideErrorMessage(this.input.closest('.t-widget'));
            };
        }
        bindKeypress(e) {
            let isValid = false, code = e.keyCode, value = this.input.value, start = this.input.selectionStart, end = this.input.selectionEnd;
            if (code == 46 && this.numberDigit) {
                let remain = value.length - end;
                if (remain <= this.numberDigit && value.indexOf('.') == -1)
                    isValid = true;
            }
            if (code >= 48 && code <= 57 && value.substr(end).indexOf('-') == -1)
                isValid = true;
            if (code >= 48 && code <= 57 || code == 13 || code == 45 && start == 0 && value.substr(end).indexOf('-') == -1)
                isValid = true;
            var pointIndex = value.indexOf('.');
            if (pointIndex >= 0 && start == end && end > pointIndex && value.split('.')[1].length == this.numberDigit)
                isValid = false;
            if (start == 0 && end == 0 && value.length > 0 && value[0] == '-' && code >= 48 && code <= 57)
                isValid = false;
            let len = value.replace('-', '').replace('.', '').length;
            if (len == this.total && start == end && code != 45 && code != 46)
                isValid = false;
            if (!isValid)
                e.preventDefault();
        }
        bindAttributes() {
            const mutationObserver = new MutationObserver((mutationList) => {
                let name = mutationList[0].attributeName;
                let attrs = mutationList[0].target.attributes;
                if (name == 'total')
                    this.total = attrs['total'].value;
                if (name == 'number-digit')
                    this.numberDigit = attrs['number-digit'].value;
            });
            mutationObserver.observe(this.input.closest('.t-widget'), {
                attributes: true,
                childList: false,
                subtree: false
            });
        }
        readAttributes() {
            let attrs = this.input.closest('.t-widget').attributes;
            if (attrs['total'] != null)
                this.total = attrs['total'].value;
            if (attrs['number-digit'] != null)
                this.numberDigit = attrs['number-digit'].value;
        }
    }
    caspian.TextBox = TextBox;
})(caspian || (caspian = {}));
var caspian;
(function (caspian) {
    class TimePicker {
        constructor(element, dotnet) {
            this.dotnet = dotnet;
            let input = element.getElementsByTagName('input')[0];
            caspian.common.bindMask(input, '__:__');
            element.onmouseenter = () => {
                let wrap = element.getElementsByClassName('t-picker-wrap')[0];
                if (!wrap.classList.contains('t-state-selected') && !wrap.classList.contains('t-state-disabled'))
                    wrap.classList.add('t-state-hover');
            };
            element.onmouseleave = () => {
                element.getElementsByClassName('t-picker-wrap')[0].classList.remove('t-state-hover');
            };
            input.onfocus = e => {
                e.target.closest('.t-picker-wrap').classList.add('t-state-selected');
            };
            input.onblur = e => {
                e.target.closest('.t-picker-wrap').classList.remove('t-state-selected');
            };
            this.bindMutationObserver(element);
        }
        bindTimePanel(elm) {
            elm.getElementsByClassName('c-time-minutes')[0].onmousedown = e => {
                let loc = elm.getElementsByTagName('circle')[2].getBoundingClientRect();
                let r = Math.pow(e.pageX - loc.left - 2, 2) + Math.pow(e.pageY - loc.top - 2, 2);
                if (r > 6400) {
                    this.setMinutLocation(elm.getElementsByClassName('c-time-minutes')[0], e, 0);
                    document.body.onmousemove = e => {
                        this.setMinutLocation(elm.getElementsByClassName('c-time-minutes')[0], e, 1);
                    };
                    document.body.onmouseup = (e) => __awaiter(this, void 0, void 0, function* () {
                        document.body.onmousemove = document.body.onmouseup = null;
                        if (elm.getElementsByClassName('c-time-footer').length == 0) {
                            this.setTime(elm.closest('.t-timepicker'));
                            yield this.dotnet.invokeMethodAsync('Close');
                        }
                    });
                }
            };
        }
        setMinutLocation(elem, e, type) {
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
        setTime(timepicker) {
            let hours = timepicker.getElementsByClassName('c-span-hours')[0].textContent;
            let minutes = timepicker.getElementsByClassName('c-span-minutes')[0].textContent;
            let input = timepicker.getElementsByTagName('input')[0];
            input.value = hours + ':' + minutes;
            let event = new Event('change');
            input.dispatchEvent(event);
        }
        bindMutationObserver(element) {
            const mutationObserver = new MutationObserver(t => {
                let panel = t[0].target.getElementsByClassName('c-timepicker')[0];
                if (panel == null) {
                    window.onclick = null;
                    return;
                }
                let panelLoc = panel.getBoundingClientRect();
                let elem = panel.closest('.t-timepicker');
                let animate = panel.closest('.t-animation-container');
                let height = panelLoc.height + 2;
                if (elem.getBoundingClientRect().top > height) {
                    animate.classList.add('c-animate-up');
                    animate.style.marginTop = `${-height - 33}px`;
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
                animate.style.width = `${panelLoc.width + 8}px`;
                animate.style.height = `${height}px`;
                this.bindTimePanel(panel);
                window.onclick = (e) => __awaiter(this, void 0, void 0, function* () {
                    if (e.target.closest('.c-timepicker') == null)
                        yield this.dotnet.invokeMethodAsync('Close');
                });
                let okButton = panel.getElementsByClassName('c-ok')[0];
                if (okButton != null) {
                    okButton.onclick = (e) => __awaiter(this, void 0, void 0, function* () {
                        this.setTime(panel.closest('.t-timepicker'));
                        yield this.dotnet.invokeMethodAsync('Close');
                    });
                }
            });
            mutationObserver.observe(element, {
                attributes: false,
                childList: true,
                subtree: false
            });
        }
    }
    caspian.TimePicker = TimePicker;
})(caspian || (caspian = {}));
var caspian;
(function (caspian) {
    class Window {
        constructor(win) {
            this.windowOpenClose(win.closest('.t-window'));
            this.bindObserver(win);
        }
        windowOpenClose(win) {
            let content = win.getElementsByClassName('t-window-content')[0];
            if (content.attributes['status'].value == '2') {
                let main = document.getElementsByClassName('c-content-main')[0];
                if (main == null)
                    document.body.style.overflow = 'hidden';
                else
                    main.style.overflow = 'hidden';
                let header = win.getElementsByClassName('t-window-titlebar')[0];
                if (content.attributes['draggable']) {
                    this.bindDragAndDrop(win, header);
                    header.style.cursor = 'move';
                }
                else {
                    header.onmousedown = null;
                    header.style.cursor = 'default';
                }
                win.style.display = 'block';
                let left, top;
                let parent = win.parentElement.closest('.t-window');
                if (parent == null) {
                    left = (window.innerWidth - win.getBoundingClientRect().width) / 2;
                    top = -50;
                }
                else {
                    let loc = parent.getBoundingClientRect();
                    left = loc.left + (loc.width - win.getBoundingClientRect().width) / 2;
                    top = loc.top - 45;
                }
                win.style.left = `${left}px`;
                win.style.top = `${top}px`;
                setTimeout(function () {
                    win.classList.add('window-animate');
                    win.style.top = `${top + 80}px`;
                    setTimeout(() => win.classList.remove('window-animate'), 250);
                }, 25);
            }
            else
                win.style.display = 'none';
        }
        bindDragAndDrop(dragableDom, header) {
            header.onmousedown = e => {
                let loc = e.target.getBoundingClientRect();
                let xStart = e.clientX, yStart = e.clientY, leftStart = loc.left, topStart = loc.top;
                document.onmouseup = () => {
                    document.onmouseup = null;
                    document.onmousemove = null;
                };
                document.onmousemove = e => {
                    let difX = e.clientX - xStart, difY = e.clientY - yStart;
                    dragableDom.style.left = `${leftStart + difX}px`;
                    dragableDom.style.top = `${topStart + difY}px`;
                };
            };
        }
        bindObserver(win) {
            const mutationObserver = new MutationObserver(list => {
                list.forEach(mutation => {
                    console.log(mutation.target);
                    if (mutation.type == 'attributes' && mutation.attributeName == 'status') {
                        let status = mutation.target.attributes['status'].value;
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
                                document.getElementsByClassName('c-content-main')[0].style.overflow = 'auto';
                        }
                        else
                            document.getElementsByClassName('c-content-main')[0].style.overflow = 'hidden';
                        let mywindow = mutation.target.closest('.t-window');
                        this.windowOpenClose(mywindow);
                    }
                });
            });
            mutationObserver.observe(win, {
                attributes: true,
                childList: false,
                subtree: false
            });
        }
    }
    caspian.Window = Window;
})(caspian || (caspian = {}));
//# sourceMappingURL=caspian.bundle.js.map