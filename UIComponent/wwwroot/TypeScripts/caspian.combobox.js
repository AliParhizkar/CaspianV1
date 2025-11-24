var caspian;
(function (caspian) {
    class ComboBox {
        constructor(input, Pageable, dotnet) {
            this.bindObserver(input, Pageable, dotnet);
            let control = input.closest('.t-combobox').getElementsByClassName('t-inputbox-wrap')[0];
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
                    this.bindObserverForSize(group.getElementsByClassName('t-reset')[0]);
                    if (pageable) {
                        group.onscrollend = async () => {
                            await dotnet.invokeMethodAsync('IncPageNumberInvokable');
                        };
                    }
                    let animate = ctr.getElementsByClassName('t-animation-container')[0];
                    let height = group.getElementsByClassName('t-reset')[0].getBoundingClientRect().height;
                    height = Math.min(250, height);
                    height = Math.max(height, 30);
                    animate.style.height = `${height + 7}px`;
                    let loc = ctr.getBoundingClientRect();
                    animate.style.width = `${loc.width + 7}px`;
                    if (loc.top > window.innerHeight / 2) {
                        animate.classList.add('c-animate-up');
                        setTimeout(() => group.style.bottom = '0', 10);
                        let dif = animate.getBoundingClientRect().top - loc.top;
                        animate.style.marginTop = `${-height - dif - 5}px`;
                    }
                    else {
                        animate.classList.add('c-animate-down');
                        setTimeout(() => group.style.top = '0', 10);
                        let dif = animate.getBoundingClientRect().top - loc.top - 35;
                        animate.style.marginTop = `${-dif}px`;
                    }
                    document.body.onmousedown = async (e) => {
                        if (e.target.closest('.t-group') == null)
                            await dotnet.invokeMethodAsync('CloseInvokable');
                    };
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
        bindObserverForSize(ul) {
            const observer = new ResizeObserver(t => {
                let height = t[0].target.getBoundingClientRect().height;
                if (height > 250)
                    height = 250;
                if (height < 30)
                    height = 30;
                let animate = t[0].target.closest('.t-animation-container');
                animate.style.height = `${height + 3}px`;
                if (animate.classList.contains('c-animate-up'))
                    animate.style.marginTop = `${-height - 40}px`;
            });
            observer.observe(ul);
        }
    }
    caspian.ComboBox = ComboBox;
})(caspian || (caspian = {}));
//# sourceMappingURL=caspian.combobox.js.map