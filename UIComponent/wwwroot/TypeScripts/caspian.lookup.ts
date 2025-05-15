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
            input.onkeypress = e => {
                if (e.keyCode == 13) {
                    let helpWindow = ((e.target as HTMLElement).closest('.c-lookup') as HTMLElement).getElementsByClassName('t-HelpWindow');
                    if (helpWindow.length > 0)
                        e.preventDefault();
                }
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
                let sidebarWidth = document.getElementsByClassName('sidebar')[0].getBoundingClientRect().width;
                let target = (list[0].target as HTMLElement);
                let helpWindow = target.getElementsByClassName('t-HelpWindow')[0] as HTMLElement;
                if (helpWindow != null) {
                    helpWindow.classList.remove('c-advance-search');
                    if (target.closest('.c-lookup').getAttribute('advanceSearch') == null) {
                        let locTarget = target.getBoundingClientRect();
                        let locHelpWindow = helpWindow.getBoundingClientRect();

                        let posTarget = target.getPosition();
                        if (locTarget.top >= locHelpWindow.height - 30)
                            helpWindow.style.marginTop = `${-locHelpWindow.height - 40}px`;
                        if (caspian.common.RightToLeft()) {
                            let right = (locTarget.width - locHelpWindow.width) / 2;
                            if (window.innerWidth - locTarget.right - sidebarWidth + right < 5)
                                right = locTarget.right + sidebarWidth + 5 - window.innerWidth;
                            if (locTarget.right - right - locHelpWindow.width < 5)
                                right = locTarget.right - locHelpWindow.width - 5;
                            //if (locHelpWindow.width + 12 > locTarget.right)
                            //    right = locTarget.right - locHelpWindow.width - 10;
                            helpWindow.style.marginRight = `${right}px`;
                        }
                        else {
                            let left = (locHelpWindow.width - locTarget.width) / 2;
                            if (posTarget.left - left + locHelpWindow.width > window.innerWidth)
                                left = locHelpWindow.width - (window.innerWidth - posTarget.left) + 26;
                            if (posTarget.left - left < sidebarWidth + 5) {
                                left = posTarget.left - sidebarWidth - 5;
                            }
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
            mutationObserver.observe(lookup.getElementsByClassName('c-content')[0], {
                attributes: true,
                childList: true,
                subtree: false
            });
        }
    }
}