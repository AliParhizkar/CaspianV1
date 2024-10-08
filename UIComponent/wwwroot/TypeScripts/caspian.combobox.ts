namespace caspian {
    export class ComboBox {
        constructor(input: HTMLElement, Pageable: boolean, dotnet: dotnetInvoker) {
            this.bindObserver(input, Pageable, dotnet);
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
                caspian.common.showErrorMessage(input.closest('.t-widget'));
            }
            input.onblur = () => {
                let list = control.classList;
                list.add('t-state-default');
                list.remove('t-state-focused');
                caspian.common.hideErrorMessage(input.closest('.t-widget'));
            }
        }

        bindObserver(input: HTMLElement, pageable: boolean, dotnet: dotnetInvoker) {
            const mutationObserver = new MutationObserver(t => {
                let ctr = t[0].target as HTMLElement;
                let group = ctr.getElementsByClassName('t-group')[0] as HTMLElement;
                if (group) {
                    if (pageable) {
                        group.onscrollend = async () => {
                            await dotnet.invokeMethodAsync('IncPageNumberInvokable');
                        }
                    }
                    let animate = ctr.getElementsByClassName('t-animation-container')[0] as HTMLElement;
                    let height = group.getElementsByClassName('t-reset')[0].getBoundingClientRect().height;
                    height = Math.min(250, height);
                    height = Math.max(height, 30);
                    animate.style.height = `${height + 7}px`;
                    let loc = ctr.getBoundingClientRect();
                    animate.style.width = `${loc.width + 7}px`;
                    if (loc.top > window.innerHeight / 2) {
                        animate.classList.add('c-animate-up');
                        setTimeout(() => group.style.bottom = '0', 10);
                        animate.style.marginTop = `${-height - 42}px`;
                    }
                    else {
                        animate.classList.add('c-animate-down');
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
        }
    }
}