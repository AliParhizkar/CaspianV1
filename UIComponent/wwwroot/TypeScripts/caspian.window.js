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
                let loc = e.target.closest('.t-window').getBoundingClientRect();
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
                    if (mutation.type == 'attributes' && mutation.attributeName == 'status') {
                        let status = mutation.target.attributes['status'].value;
                        let main = document.getElementsByClassName('c-content-main')[0] || document.body;
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
                                main.style.overflow = 'auto';
                        }
                        else
                            main.style.overflow = 'hidden';
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
//# sourceMappingURL=caspian.window.js.map