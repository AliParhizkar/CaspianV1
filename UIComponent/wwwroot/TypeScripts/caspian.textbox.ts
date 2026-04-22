namespace caspian {
    export class TextBox {
        input: HTMLInputElement;
        total: number;
        numberDigit: number;
        digitGrouping: boolean;
        search: boolean;
        private maskedText: string;
        dotnet: dotnetInvoker;
        watingForSearch: boolean;
        timerId: number;
        searchedValue: string;

        constructor(input: HTMLInputElement, type: string, dotnet: dotnetInvoker) {
            this.dotnet = dotnet;
            caspian.common.bindErrorMessage(input.parentElement, input);
            let attr = input.parentElement.attributes['masked-text'];
            this.input = input;
            if (attr) {
                this.maskedText = attr.value;
                caspian.common.bindMaskedText(this.input, this.maskedText);
            }
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
            if (type != 'string') {
                input.onkeypress = e => this.bindKeypress(e);
                if (this.digitGrouping)
                    input.oninput = async e => await this.bindOnInput(e);
            }
            else if (this.search)
                input.oninput = async e => await this.bindOnInput(e);
            if (input.style.direction == 'ltr' && input.closest('.t-rtl')) 
                (input.closest('.t-textbox') as HTMLElement).style.direction= 'rtl'
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

        async bindOnInput(e: Event) {
            let input = e.target as HTMLInputElement, start = input.selectionStart, end = input.selectionEnd;
            if (this.search) {
                if (this.watingForSearch) {
                    this.initializeTimer();
                }
                else {
                    this.watingForSearch = true;
                    this.searchedValue = input.value;
                    await this.dotnet.invokeMethodAsync('SetSearchValue', input.value);
                    this.watingForSearch = false;
                }
            }
            else {
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

            if (pointIndex >= 0 && start == end && end > pointIndex && value.split('.')[1].length >= this.numberDigit)
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
                if (name == 'masked-text') {
                    if (attrs['masked-text'].value != this.maskedText) {
                        this.maskedText = attrs['masked-text'].value;
                        caspian.common.bindMaskedText(this.input, this.maskedText);
                    }
                }
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
            if (this.maskedText != attrs['masked-text']) {
                this.maskedText = attrs['masked-text'].value;
                caspian.common.bindMaskedText(this.input, this.maskedText);
            }
            this.digitGrouping = attrs['digit-grouping'] != null;
        }
    }
}