var caspian;
(function (caspian) {
    class DatePicker {
        constructor(element, dotnet) {
            let input = element.getElementsByTagName('input')[0];
            caspian.common.bindMask(input, '____/__/__');
            element.onmouseenter = e => {
                let elem = e.target.getElementsByClassName('t-inputbox-wrap')[0];
                if (!elem.classList.contains('t-state-selected') || !elem.classList.contains('t-state-disabled'))
                    elem.classList.add('t-state-hover');
            };
            element.onmouseleave = e => {
                let elem = e.target.getElementsByClassName('t-inputbox-wrap')[0];
                elem.classList.remove('t-state-hover');
            };
            input.onfocus = e => {
                let elem = e.target;
                elem.closest('.t-inputbox-wrap').classList.replace('t-state-hover', 't-state-selected');
                caspian.common.showErrorMessage(elem.closest('.t-widget'));
            };
            input.onblur = e => {
                let elem = e.target;
                elem.closest('.t-inputbox-wrap').classList.remove('t-state-selected');
                caspian.common.hideErrorMessage(elem.closest('.t-widget'));
            };
            this.bindObserver(element, dotnet);
        }
        bindObserver(element, dotnet) {
            const mutationObserver = new MutationObserver(t => {
                let element = t[0].target, animate = t[0].addedNodes[0];
                if (animate) {
                    let calendar = animate.getElementsByClassName('t-datepicker-calendar')[0];
                    if (element.getBoundingClientRect().top > window.outerHeight / 2) {
                        animate.classList.add('c-animate-up');
                        animate.style.marginTop = '-275px';
                        setTimeout(async () => calendar.style.bottom = '-0', 20);
                    }
                    else {
                        animate.classList.add('c-animate-down');
                        setTimeout(async () => calendar.style.top = '0', 20);
                    }
                    document.body.onmousedown = async (e) => {
                        if (e.target.closest('.t-animation-container') == null)
                            await dotnet.invokeMethodAsync('CloseWindow');
                    };
                }
                else
                    document.body.onmousedown = null;
            });
            mutationObserver.observe(element, {
                attributes: false,
                childList: true,
                subtree: false
            });
        }
        static bindCalendar(elm, viewType, vNavigation) {
            let toDownState = elm.getElementsByClassName('c-down-to-state')[0];
            let toUpState = elm.getElementsByClassName('c-up-to-state')[0];
            let fromDownState = elm.getElementsByClassName('c-down-from-state')[0];
            let fromUpState = elm.getElementsByClassName('c-up-from-state')[0];
            if (viewType == ViewType.Month) {
                if (vNavigation == VNavigation.Down) {
                    toDownState.style.left = '0';
                    toDownState.style.top = '35px';
                    toDownState.style.width = '212px';
                    toDownState.style.height = '200px';
                    //-----------------------------
                    fromDownState.style.opacity = '0';
                }
            }
            else {
                let locTarget = elm.getBoundingClientRect();
                let left, top;
                if (toUpState) {
                    let loc = toUpState.getElementsByClassName('t-state-selected')[0].getBoundingClientRect();
                    left = loc.left - locTarget.left, top = loc.top - locTarget.top;
                }
                if (vNavigation == VNavigation.Up) {
                    fromUpState.style.left = `${left}px`;
                    fromUpState.style.top = `${top}px`;
                    fromUpState.style.width = '47px';
                    fromUpState.style.height = '60px';
                    toUpState.style.opacity = '1';
                    setTimeout(() => fromUpState.style.display = 'none', 4000);
                }
                else if (vNavigation == VNavigation.Down) {
                    toDownState.style.left = '0';
                    toDownState.style.top = '35px';
                    toDownState.style.width = '212px';
                    toDownState.style.height = '200px';
                    fromDownState.style.opacity = '0';
                }
            }
        }
    }
    caspian.DatePicker = DatePicker;
    let ViewType;
    (function (ViewType) {
        ViewType[ViewType["Month"] = 1] = "Month";
        ViewType[ViewType["Year"] = 2] = "Year";
        ViewType[ViewType["Decade"] = 3] = "Decade";
        ViewType[ViewType["Century"] = 4] = "Century";
    })(ViewType || (ViewType = {}));
    let VNavigation;
    (function (VNavigation) {
        VNavigation[VNavigation["Up"] = 1] = "Up";
        VNavigation[VNavigation["Down"] = 2] = "Down";
    })(VNavigation || (VNavigation = {}));
})(caspian || (caspian = {}));
//# sourceMappingURL=caspian.datepicker.js.map