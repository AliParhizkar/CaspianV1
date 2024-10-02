namespace caspian {
    export class TextBox {
        input: HTMLInputElement;
        total: number;
        numberDigit: number;
        constructor(input: HTMLInputElement, type: string) {
            this.input = input;
            this.total ||= 8; 
            input.onmouseenter = () => {
                input.parentElement.classList.add('t-state-hover');
            }
            input.onmouseleave = () => {
                input.parentElement.classList.remove('t-state-hover');
            }
            this.readAttributes();
            this.bindAttributes();
            if (type != 'string')
                input.onkeypress = e => this.bindKeypress(e);
            input.onfocus = () => {
                setTimeout(() => {
                    this.input.select();
                }, 100);
                caspian.common.showErrorMessage(this.input.closest('.t-widget'));
            }
            input.onblur = () => {
                caspian.common.hideErrorMessage(this.input.closest('.t-widget'));
            }
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
            let len = value.replace('-', '').replace('.', '').length;
            if (len == this.total && start == end && code != 45 && code != 46)
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
        }
    }
}