namespace caspian {
    export class ComboBox {
        constructor(input: HTMLElement, Pageable: boolean, dotnet: dotnetInvoker) {
            this.bindObserver(input, Pageable, dotnet);
            let control = input.closest('.t-combobox').getElementsByClassName('t-inputbox-wrap')[0] as HTMLElement;
            input.parentElement.onkeydown = e => {
                if (e.code == 'Enter' || e.code == 'NumpadEnter') {
                    if ((e.target as HTMLElement).closest('.t-combobox').getElementsByClassName('t-animation-container').length > 0)
                        e.preventDefault();
                }
            }
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
                    this.bindObserverForItems(group.getElementsByClassName('t-reset')[0] as HTMLElement);
                    if (pageable) {
                        group.onscrollend = async () => {
                            await dotnet.invokeMethodAsync('IncPageNumberInvokable');
                        }
                    }
                    let animate = ctr.getElementsByClassName('t-animation-container')[0] as HTMLElement;
                    let leftScroll = animate.parentElement.getPosition().left - animate.parentElement.getBoundingClientRect().left;
                    animate.style.marginRight = `${leftScroll}px`;
                    let height = group.getElementsByClassName('t-reset')[0].getBoundingClientRect().height;
                    height = Math.min(250, height);
                    height = Math.max(height, 35);
                    animate.style.height = `${height + 10}px`;
                    let loc = ctr.getBoundingClientRect();
                    animate.style.width = `${loc.width + 7}px`;
                    if (loc.top > window.innerHeight / 2) {
                        animate.classList.add('c-animate-up');
                        setTimeout(() => group.style.bottom = '0', 10);
                        let dif = animate.getBoundingClientRect().top - loc.top;
                        animate.style.marginTop = `${-height - dif - 12}px`;
                    }
                    else {
                        animate.classList.add('c-animate-down');
                        setTimeout(() => group.style.top = '0', 10);
                        let dif = animate.getBoundingClientRect().top - loc.top - 35;
                        animate.style.marginTop = `${-dif}px`;
                    }
                    document.body.onmousedown = async e => {
                        if ((e.target as HTMLElement).closest('.t-group') == null)
                            await dotnet.invokeMethodAsync('CloseInvokable');
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

        bindObserverForItems(ul: HTMLElement) {
            const mutationObserver = new MutationObserver(t => {
                let ctr = t[0].target as HTMLElement;
                let height = ctr.getBoundingClientRect().height + 10;
                height = Math.min(250, height);
                height = Math.max(35, height);
                let animate = ctr.closest('.t-animation-container') as HTMLElement;

                if (animate.classList.contains('c-animate-up')) {
                    let difHeight = animate.getBoundingClientRect().height - height;
                    let marginTop = animate.style.marginTop;
                    marginTop = marginTop.substring(0, marginTop.length - 2);
                    animate.style.marginTop = `${parseFloat(marginTop) + difHeight}px`;
                }
                animate.style.height = `${height}px`;
            });
            mutationObserver.observe(ul, {
                attributes: false,
                childList: true,
                subtree: false
            });
        }
    }
}