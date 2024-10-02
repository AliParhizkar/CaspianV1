namespace caspian {
    export class Lookup {
        constructor(input: HTMLElement, dotnet: dotnetInvoker) {
            let lookup = input.closest('.c-lookup') as HTMLElement;
            input.onfocus = () => {
                caspian.common.showErrorMessage(lookup);
            }
            input.onkeydown = e => {
                let code = e.keyCode;
                if (code == 40 || code == 38)
                    e.preventDefault();
            }
            input.onblur = () => {
                caspian.common.hideErrorMessage(lookup);
            }
            //if (lookup.attributes['closeonblur'].value != undefined)
            //    lookup.attributes['tabindex'].value = '0';
            this.bindObserver(lookup, dotnet);
        }

        bindObserver(lookup: HTMLElement, dotnet: dotnetInvoker) {
            const mutationObserver = new MutationObserver(list => {
                let target = (list[0].target as HTMLElement);
                let helpWindow = target.getElementsByClassName('t-HelpWindow')[0] as HTMLElement;
                if (helpWindow != null) {
                    let animate = target.getElementsByClassName('t-animation-container')[0] as HTMLElement;
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
                        debugger
                        helpWindow.style.top = `${-locHelpWindow.height}`;
                    }
                    if (lookup.attributes['autoHide']) {
                        window.onclick = async function (e: MouseEvent) {
                            if (!(e.target as HTMLElement).closest('.c-lookup'))
                                await dotnet.invokeMethodAsync('Close');
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
}