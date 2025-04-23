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

                    let locTarget = target.getBoundingClientRect();
                    let locHelpWindow = helpWindow.getBoundingClientRect();
                    let left = (locHelpWindow.width - locTarget.width) / 2;
                    let posTarget = target.getPosition();
                    if (posTarget.left - left + locHelpWindow.width > window.innerWidth)
                        left = locHelpWindow.width - (window.innerWidth - posTarget.left) + 26;
                    if (locTarget.top >= locHelpWindow.height - 30)
                        helpWindow.style.marginTop = `${-locHelpWindow.height - 60}px`;
                    helpWindow.style.marginLeft = `${-left}px`;
                    helpWindow.style.transform = 'scale(0)';
                    setTimeout(() => {
                        helpWindow.style.transition = '0.2s transform ease';
                        helpWindow.style.transform = 'scale(100%)';
                    }, 25);
                    //if (locTarget.bottom + locHelpWindow.height - 30 <= window.innerHeight) {
                    //    //setTimeout(() => helpWindow.style.top = '0', 25);
                    //}
                    //else if (locTarget.top >= locHelpWindow.height - 30) {
                    //    helpWindow.style.marginTop = `${-locHelpWindow.height - 35}px`;
                    //    //setTimeout(() => helpWindow.style.bottom = '0', 25);
                    //}
                    //else 
                    //    helpWindow.style.top = `${-locHelpWindow.height}`;
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