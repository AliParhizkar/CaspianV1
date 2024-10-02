namespace caspian {
    export class TimePicker {
        dotnet: dotnetInvoker;

        constructor(element: HTMLElement, dotnet: dotnetInvoker) {
            this.dotnet = dotnet;
            let input = element.getElementsByTagName('input')[0];
            caspian.common.bindMask(input, '__:__');
            element.onmouseenter = () => {
                let wrap = element.getElementsByClassName('t-picker-wrap')[0];
                if (!wrap.classList.contains('t-state-selected') && !wrap.classList.contains('t-state-disabled'))
                    wrap.classList.add('t-state-hover');
            }
            element.onmouseleave = () => {
                element.getElementsByClassName('t-picker-wrap')[0].classList.remove('t-state-hover')
            }
            input.onfocus = e => {
                (e.target as HTMLElement).closest('.t-picker-wrap').classList.add('t-state-selected');
            }
            input.onblur = e => {
                (e.target as HTMLElement).closest('.t-picker-wrap').classList.remove('t-state-selected');
            }
            this.bindMutationObserver(element);
        }

        bindTimePanel(elm: HTMLElement) {
            (elm.getElementsByClassName('c-time-minutes')[0] as HTMLDivElement).onmousedown = e => {
                let loc = elm.getElementsByTagName('circle')[2].getBoundingClientRect();
                let r = Math.pow(e.pageX - loc.left - 2, 2) + Math.pow(e.pageY - loc.top - 2, 2);
                if (r > 6400) {
                    this.setMinutLocation(elm.getElementsByClassName('c-time-minutes')[0] as HTMLElement, e, 0)
                    document.body.onmousemove = e => {
                        this.setMinutLocation(elm.getElementsByClassName('c-time-minutes')[0] as HTMLElement, e, 1);
                    }
                    document.body.onmouseup = async e => {
                        document.body.onmousemove = document.body.onmouseup = null;
                        if (elm.getElementsByClassName('c-time-footer').length == 0) {
                            this.setTime(elm.closest('.t-timepicker'))
                            await this.dotnet.invokeMethodAsync('Close');
                        }
                    }
                }
            }
        }

        setMinutLocation(elem: HTMLElement, e: MouseEvent, type: number) {
            let from = elem.attributes['from'], to = elem.attributes['to'];
            let loc = elem.getElementsByTagName('circle').item(2).getBoundingClientRect();
            let difX = e.pageX - loc.left, difY = e.pageY - loc.top;
            let deg = Math.atan(difY / difX);
            if (difX < 0)
                deg += Math.PI;
            let minute = Math.round(deg / Math.PI * 30 + 15);
            if (minute == 60)
                minute = 0;
            if (from != null && minute < from.value)
                return false;
            if (to != null && minute > to.value)
                return false;
            let text = minute.toString();
            if (minute < 10)
                text = '0' + text;
            elem.closest('.c-timepicker').getElementsByClassName('c-span-minutes')[0].textContent = text;
            let x2 = null, y2 = null;
            if (type == 1) {
                x2 = Math.cos(deg) * 110;
                y2 = Math.sin(deg) * 110;
            }
            else {
                let factor = (minute - 15) / 30 * Math.PI;
                x2 = Math.cos(factor) * 110;
                y2 = Math.sin(factor) * 110;
            }
            let svg = elem.getElementsByTagName('svg')[0];
            let circles = svg.getElementsByTagName('circle');
            circles[0].attributes['cx'].value = circles[1].attributes['cx'].value = x2;
            circles[0].attributes['cy'].value = circles[1].attributes['cy'].value = y2;
            let line = svg.getElementsByTagName('line')[0];
            line.attributes['x2'].value = x2;
            line.attributes['y2'].value = y2;
            return true;
        }

        setTime(timepicker: HTMLElement) {
            let hours = timepicker.getElementsByClassName('c-span-hours')[0].textContent;
            let minutes = timepicker.getElementsByClassName('c-span-minutes')[0].textContent;
            let input = timepicker.getElementsByTagName('input')[0];
            input.value = hours + ':' + minutes;
            let event = new Event('change');
            input.dispatchEvent(event);
        }

        bindMutationObserver(element: HTMLElement) {
            const mutationObserver = new MutationObserver(t => {
                let panel = (t[0].target as HTMLElement).getElementsByClassName('c-timepicker')[0] as HTMLDivElement;
                if (panel == null) {
                    window.onclick = null;
                    return;
                }
                let panelLoc = panel.getBoundingClientRect();
                let elem = panel.closest('.t-timepicker');
                let animate = panel.closest('.t-animation-container') as HTMLDivElement;
                let height = panelLoc.height + 2;
                if (elem.getBoundingClientRect().top > height) {
                    animate.classList.add('c-animate-up');
                    animate.style.marginTop = `${-height - 33}px`
                    setTimeout(() => {
                        panel.style.bottom = '0';
                    }, 10);
                }
                else {
                    animate.classList.add('c-animate-down');
                    setTimeout(() => {
                        panel.style.top = '0';
                    }, 10);
                    height += 8;
                }
                animate.style.width = `${panelLoc.width + 8}px`;
                animate.style.height = `${height}px`;
                this.bindTimePanel(panel);
                window.onclick = async e => {
                    if ((e.target as HTMLElement).closest('.c-timepicker') == null)
                        await this.dotnet.invokeMethodAsync('Close');
                }
                let okButton = panel.getElementsByClassName('c-ok')[0] as HTMLButtonElement;
                if (okButton != null) {
                    okButton.onclick = async e => {
                        this.setTime(panel.closest('.t-timepicker'));
                        await this.dotnet.invokeMethodAsync('Close');
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