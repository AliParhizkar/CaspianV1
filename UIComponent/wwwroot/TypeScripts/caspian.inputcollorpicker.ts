namespace caspian {
    export class InputCollorPicker {
        constructor(element: HTMLElement, dotnet: dotnetInvoker) {
            this.bindObserver(element, dotnet);
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

        bindObserver(element: HTMLElement, dotnet: dotnetInvoker) {
            const mutationObserver = new MutationObserver(t => {
                let elem = t[0].target as HTMLElement;
                let animate = elem.getElementsByClassName('t-animation-container')[0] as HTMLElement;
                if (animate != null) {

                    let top = elem.getBoundingClientRect().top;
                    let picker = elem.getElementsByClassName('c-colorpicker-panel')[0] as HTMLDivElement;
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
                    document.body.onclick = async e => {
                        if ((e.target as HTMLDivElement).closest('.t-animation-container') == null)
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
        }
    }
}