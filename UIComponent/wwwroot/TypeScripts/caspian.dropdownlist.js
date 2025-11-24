var caspian;
(function (caspian) {
    class DropdownList {
        constructor(element, dotnet) {
            this.bindObserver(element, dotnet);
            element.onmouseenter = e => {
                let ddl = e.target.getElementsByClassName('t-inputbox-wrap')[0];
                if (!ddl.classList.contains('t-state-disabled')) {
                    ddl.classList.remove('t-state-default');
                    ddl.classList.add('t-state-hover');
                }
            };
            element.onmouseleave = e => {
                let ddl = e.target.getElementsByClassName('t-inputbox-wrap')[0];
                ddl.classList.remove('t-state-hover');
                ddl.classList.add('t-state-default');
            };
            element.onfocus = e => {
                let ddl = e.target.getElementsByClassName('t-inputbox-wrap')[0];
                ddl.classList.remove('t-state-default');
                ddl.classList.add('t-state-focused');
                caspian.common.showErrorMessage(e.target);
            };
            element.onblur = () => {
                let ddl = element.getElementsByClassName('t-inputbox-wrap')[0];
                ddl.classList.remove('t-state-focused');
                ddl.classList.add('t-state-default');
                caspian.common.hideErrorMessage(element);
            };
            caspian.common.bindErrorMessage(element, element);
        }
        bindObserver(element, dotnet) {
            const mutationObserver = new MutationObserver(t => {
                let ddl = t[0].target;
                let animate = ddl.getElementsByClassName('t-animation-container')[0];
                if (animate != null) {
                    animate.style.width = `${ddl.getBoundingClientRect().width + 4}px`;
                    let group = animate.getElementsByClassName('t-group')[0];
                    let height = group.getBoundingClientRect().height;
                    animate.style.height = `${height + 5}px`;
                    if (ddl.getBoundingClientRect().top > window.innerHeight / 2) {
                        animate.classList.add('c-animate-up');
                        animate.style.marginTop = `${-height - 38}px`;
                        setTimeout(() => group.style.bottom = '0', 30);
                    }
                    else {
                        animate.classList.add('c-animate-down');
                        setTimeout(() => group.style.top = '0', 30);
                    }
                    document.body.onmousedown = async (e) => {
                        let dropdown = e.target.closest('.t-dropdown');
                        if (dropdown == null || dropdown != element) {
                            document.body.onmousedown = null;
                            await dotnet.invokeMethodAsync('CloseWindow');
                        }
                    };
                }
            });
            mutationObserver.observe(element, {
                attributes: false,
                childList: true,
                subtree: false
            });
        }
    }
    caspian.DropdownList = DropdownList;
})(caspian || (caspian = {}));
//# sourceMappingURL=caspian.dropdownlist.js.map