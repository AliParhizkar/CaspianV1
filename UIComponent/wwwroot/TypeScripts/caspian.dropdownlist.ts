namespace caspian {
    export class DropdownList {
        constructor(element: HTMLElement, dotnet: dotnetInvoker) {
            this.bindObserver(element, dotnet);
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
                caspian.common.showErrorMessage(e.target as HTMLElement);
            }
            element.onblur = () => {
                let ddl = element.getElementsByClassName('t-dropdown-wrap')[0];
                ddl.classList.remove('t-state-focused');
                ddl.classList.add('t-state-default');
                caspian.common.hideErrorMessage(element);
            }
        }

        bindObserver(element: HTMLElement, dotnet: dotnetInvoker) {
            const mutationObserver = new MutationObserver(t => {
                let ddl = t[0].target as HTMLElement;
                let animate = ddl.getElementsByClassName('t-animation-container')[0] as HTMLElement;
                if (animate != null) {
                    animate.style.width = `${ddl.getBoundingClientRect().width + 4}px`;
                    let group = animate.getElementsByClassName('t-group')[0] as HTMLElement;
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
                    document.body.onclick = async e => {
                        if ((e.target as HTMLElement).closest('t-animation-container') == null) {
                            document.body.onclick = null;
                            await dotnet.invokeMethodAsync('CloseWindow');
                        }
                    }
                }
            });
            mutationObserver.observe(element, {
                attributes: false,
                childList: true,
                subtree: false
            });
        }
    }
}