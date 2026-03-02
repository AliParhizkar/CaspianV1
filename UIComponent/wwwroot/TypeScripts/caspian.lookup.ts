namespace caspian {
    export class Lookup {
        public static lookups: Lookup[];
        timerId: number;
        watingForSearch: boolean;
        searchedValue: string;
        input: HTMLInputElement;
        dotnet: dotnetInvoker;

        constructor(input: HTMLInputElement, dotnet: dotnetInvoker) {
            this.input = input
            let lookup = input.closest('.c-lookup') as HTMLElement;
            this.lookupWindow = lookup;
            input.onfocus = () => caspian.common.showErrorMessage(lookup);
            input.onkeydown = e => {
                let code = e.keyCode;
                if (code == 40 || code == 38)
                    e.preventDefault();
                if (code == 13) {
                    let helpWindow = ((e.target as HTMLElement).closest('.c-lookup') as HTMLElement).getElementsByClassName('t-HelpWindow');
                    if (helpWindow.length > 0)
                        e.preventDefault();
                }
            }
            input.onblur = () => caspian.common.hideErrorMessage(lookup);
            input.oninput = async e => {
                if (this.watingForSearch)
                    this.initializeTimer();
                else {
                    this.watingForSearch = true;
                    this.searchedValue = input.value;
                    await dotnet.invokeMethodAsync("SetSearchValue", (e.target as HTMLInputElement).value)
                    this.watingForSearch = false;
                }
            }
            Lookup.lookups ||= [];
            this.dotnetInvoker = dotnet;
            this.bindObserver(lookup);
        }

        initializeTimer() {
            if (this.timerId)
                clearTimeout(this.timerId);
            this.timerId = setTimeout(async () => {
                if (this.watingForSearch)
                    this.initializeTimer();
                else if (this.searchedValue != this.input.value) {
                    this.watingForSearch = true;
                    this.searchedValue = this.input.value;
                    await this.dotnet.invokeMethodAsync('SetSearchValue', this.input.value);
                    this.watingForSearch = false;
                }
            }, 500);
        }

        public dotnetInvoker: dotnetInvoker;
        public lookupWindow: HTMLElement;

        bindObserver(lookup: HTMLElement) {
            let lookupComponenet = this;
            const mutationObserver = new MutationObserver(async list => {
                let sidebar = document.getElementsByClassName('sidebar')[0];
                let sidebarWidth = sidebar == null ?0 : sidebar.getBoundingClientRect().width;
                let target = (list[0].target as HTMLElement).closest('.c-lookup') as HTMLElement;
                let helpWindow = target.getElementsByClassName('t-HelpWindow')[0] as HTMLElement;
                if (helpWindow != null) {
                    lookupComponenet.lookupWindow = helpWindow;
                    window.onkeydown = e => {
                        if (e.keyCode == 13) {
                            e.preventDefault();
                        }
                    }
                    helpWindow.classList.remove('c-advance-search');
                    if (target.closest('.c-lookup').getAttribute('advanceSearch') == null) {
                        let locTarget = target.getBoundingClientRect();
                        let locHelpWindow = helpWindow.getBoundingClientRect();
                        let posTarget = target.getPosition();
                        let scrollTop = helpWindow.getBoundingClientRect().top - locTarget.top - 38;
                        let gridContainer = target.closest('.t-grid-content') as HTMLElement;
                        if (gridContainer) {
                            await waite(10);
                            scrollTop = gridContainer.scrollTop;
                        }
                        if (locTarget.top - 40 >= locHelpWindow.height)
                            helpWindow.style.marginTop = `${-locHelpWindow.height - scrollTop - 40}px`;
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
                        Lookup.lookups.push(this);
                    }, 25);
                    
                    if (lookup.attributes['autoHide']) {
                        window.onclick = async function (e: MouseEvent) {
                            let lookup = Lookup.lookups[Lookup.lookups.length - 1];
                            let target = (e.target as HTMLElement).closest('.t-HelpWindow');
                            if (target == null) {
                                let lookup = (e.target as HTMLElement).closest('.c-lookup');
                                if (lookup)
                                    target = lookup.getElementsByClassName('t-HelpWindow')[0];
                            }
                            if (target == null || target != lookup.lookupWindow)
                                await lookup.dotnetInvoker.invokeMethodAsync('Close');
                        };
                    }
                }
                else {
                    Lookup.lookups.pop();
                    if (Lookup.lookups.length == 0) {
                        window.onclick = null;
                        window.onkeydown = null;
                    }
                }

            });
            mutationObserver.observe(lookup.getElementsByClassName('c-content')[0], {
                attributes: true,
                childList: true,
                subtree: false
            });
        }
    }
}