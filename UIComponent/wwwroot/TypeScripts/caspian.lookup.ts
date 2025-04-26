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
                    helpWindow.classList.remove('c-advance-search');
                    if (target.getAttribute('advanceSearch') == null) {
                        let locTarget = target.getBoundingClientRect();
                        let locHelpWindow = helpWindow.getBoundingClientRect();
                        let posTarget = target.getPosition();
                        if (locTarget.top >= locHelpWindow.height - 30)
                            helpWindow.style.marginTop = `${-locHelpWindow.height - 60}px`;
                        if (caspian.common.RightToLeft()) {
                            let right = 0;
                            if (locHelpWindow.width + 8 > locTarget.right)
                                right = locTarget.right - locHelpWindow.width - 4;
                            helpWindow.style.marginRight = `${right}px`;
                        }
                        else {
                            let left = (locHelpWindow.width - locTarget.width) / 2;
                            if (posTarget.left - left + locHelpWindow.width > window.innerWidth)
                                left = locHelpWindow.width - (window.innerWidth - posTarget.left) + 26;
                            helpWindow.style.marginLeft = `${-left}px`;
                        }

                    } else {
                        let loc = helpWindow.getBoundingClientRect();
                        helpWindow.style.width = `${loc.width}px`;
                        helpWindow.style.height = `${loc.height}px`;
                        helpWindow.classList.add('c-advance-search');
                        helpWindow.style.marginTop = helpWindow.style.marginLeft = helpWindow.style.marginRight =
                            helpWindow.style.marginBottom = 'auto';
                    }
                    helpWindow.style.transform = 'scale(0)';
                    setTimeout(() => {
                        helpWindow.style.transition = '0.2s transform ease';
                        helpWindow.style.transform = 'scale(100%)';
                    }, 25);
                    if (lookup.attributes['autoHide']) {
                        window.onclick = async function (e: MouseEvent) {
                            if ((e.target as HTMLElement).closest('.c-lookup') == null)
                                await dotnet.invokeMethodAsync('Close');
                        };
                    }
                }
                else
                    window.onclick = null;

            });
            mutationObserver.observe(lookup, {
                attributes: true,
                childList: true,
                subtree: false
            });
        }
    }
}