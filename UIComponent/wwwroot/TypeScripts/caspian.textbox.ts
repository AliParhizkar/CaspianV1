namespace caspian {
    export class TextBox {
        input: HTMLInputElement;
        total: number;
        numberDigit: number;
        digitGrouping: boolean;
        search: boolean;
        constructor(input: HTMLInputElement, type: string) {
            caspian.common.bindErrorMessage(input.parentElement, input);
            this.input = input;
            this.total ||= 8; 
            input.onmouseenter = () => {
                let list = input.parentElement.classList;
                list.add('t-state-hover');
                list.remove('t-state-default');
            }
            input.onmouseleave = () => {
                let list = input.parentElement.classList;
                list.remove('t-state-hover');
                list.add('t-state-default');
            }
            input.onfocus = () => {
                if (input.getAttribute('disableautoselect') == null)
                    input.select();
                let list = input.parentElement.classList;
                list.add('t-state-focused');
                list.remove('t-state-default');
                caspian.common.showErrorMessage(this.input.closest('.t-widget'));
            }
            input.onblur = () => {
                let list = input.parentElement.classList;
                list.remove('t-state-focused');
                list.add('t-state-default');
                caspian.common.hideErrorMessage(this.input.closest('.t-widget'));
            }
            this.readAttributes();
            this.bindAttributes();
            if (this.search) {
                input.oninput = e => {
                    if (caspian.common.infoTimer != null)
                        clearTimeout(caspian.common.infoTimer);
                    caspian.common.infoTimer = caspian.common.infoTimer = setTimeout(() => {
                        let event = new Event('change');
                        e.target.dispatchEvent(event);
                    }, 300);
                };
            }
            if (type != 'string') {
                input.onkeypress = e => this.bindKeypress(e);
                if (this.digitGrouping) 
                    input.oninput = e => this.bindOnInput(e);
            }
        }

        bindOnInput(e: Event) {
            let input = e.target as HTMLInputElement, start = input.selectionStart, end = input.selectionEnd;
            let count = input.value.substring(0, start).split(',').length - 1;
            input.value = this.sepreate3Digit(input.value);
            let count1 = input.value.substring(0, start).split(',').length - 1;
            if (count != count1) {
                input.selectionStart = start + 1;
                input.selectionEnd = end + 1;
            }
            else {
                input.selectionStart = start;
                input.selectionEnd = end;
            }
        }

        sepreate3Digit(value: String) {
            let isNegativ = value.length > 0 && value[0] == '-'
            if (isNegativ)
                value = value.replace('-', '');
            value = value.replace(/,/g, '');
            let str = isNegativ ? '-' : '', counter = 3 - value.length % 3;
            for (let index in value) {
                str += value[index];
                counter++;
                if (counter % 3 == 0 && counter <= value.length) {
                    str += ',';
                }
            }
            return str;
        }

        bindKeypress(e: KeyboardEvent) {
            let isValid = false, code = e.keyCode, value = this.input.value, start = this.input.selectionStart, end = this.input.selectionEnd;
            if (code == 46 && this.numberDigit) {
                let remain = value.length - end;
                if (remain <= this.numberDigit && value.indexOf('.') == -1)
                    isValid = true;
            }
            if (code >= 48 && code <= 57 && value.substr(end).indexOf('-') == -1)
                isValid = true;
            if (code >= 48 && code <= 57 || code == 13 || code == 45 && start == 0 && value.substr(end).indexOf('-') == -1)
                isValid = true;
            var pointIndex = value.indexOf('.');
            if (pointIndex >= 0 && start == end && end > pointIndex && value.split('.')[1].length == this.numberDigit)
                isValid = false;
            if (start == 0 && end == 0 && value.length > 0 && value[0] == '-' && code >= 48 && code <= 57)
                isValid = false;
            let len = value.replace('-', '').replace('.', '').replace(/,/g, '').length;
            if (len >= this.total && start == end && code != 45 && code != 46)
                isValid = false;
            if (!isValid)
                e.preventDefault();
        }

        bindAttributes() {
            const mutationObserver = new MutationObserver((mutationList) => {
                let name = mutationList[0].attributeName;
                let attrs = (mutationList[0].target as HTMLElement).attributes;
                if (name == 'total')
                    this.total = attrs['total'].value;
                if (name == 'number-digit')
                    this.numberDigit = attrs['number-digit'].value;
            });
            mutationObserver.observe(this.input.closest('.t-widget'), {
                attributes: true,
                childList: false,
                subtree: false
            });
        }

        readAttributes() {
            let attrs = this.input.closest('.t-widget').attributes;
            if (attrs['total'] != null)
                this.total = attrs['total'].value;
            if (attrs['number-digit'] != null)
                this.numberDigit = attrs['number-digit'].value;
            if (attrs['search'] != null)
                this.search = true;
            this.digitGrouping = attrs['digit-grouping'] != null;
        }
    }
}