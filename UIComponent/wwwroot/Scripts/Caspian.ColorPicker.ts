namespace caspian {
    export class ColorPicker {
        element: HTMLElement;
        bindingType: number;
        red: number;
        green: number;
        blue: number;
        alpha: number;
        hue: number;
        saturation: number;
        value: number;
        transparentStrip: HTMLDivElement;
        colorBlock: HTMLElement;
        selector: HTMLElement;
        displayer: HTMLElement;
        alphaInput: HTMLInputElement;
        colorInput: HTMLInputElement;
        hueInput: HTMLInputElement;
        xStart: number;
        yStart: number;
        leftStart: number;
        topStart: number;
        constructor(element: HTMLElement) {
            this.element = element;
            this.transparentStrip = element.getElementsByClassName('c-transparent-strip')[0] as HTMLDivElement;
            this.alphaInput = this.transparentStrip.getElementsByTagName('input')[0] as HTMLInputElement;
            this.colorInput = element.querySelector('.c-color-number input') as HTMLInputElement;
            this.displayer = element.getElementsByClassName('c-color-displayer')[0] as HTMLElement;
            this.colorBlock = element.getElementsByClassName('c-color-block')[0] as HTMLElement;
            this.hueInput = element.querySelector('.c-colors-hue input') as HTMLInputElement;
            this.selector = element.getElementsByClassName('c-color-selector')[0] as HTMLElement;
            this.colorBlock.onmousedown = e => {
                this.startDrag(e.clientX, e.clientY, e.layerX, e.layerY);
                this.selector.style.left = `${e.layerX - 7}px`;
                this.selector.style.top = `${e.layerY - 7}px`;
            };
            this.selector.onmousedown = e => {
                let loc = this.element.getBoundingClientRect();
                let locSelector = this.selector.getBoundingClientRect();
                let left = locSelector.left - loc.left + 7, top = locSelector.top - loc.top + 7;
                this.startDrag(e.clientX, e.clientY, left, top);
            };
            (element.getElementsByClassName('c-color-bar')[0] as HTMLInputElement).oninput = e => {
                this.hue = parseInt((e.target as HTMLInputElement).value);
                this.update();
            }
            this.alphaInput.oninput = e => {
                this.alpha = parseInt((e.target as HTMLInputElement).value) / 100;
                this.update();
            }
            this.element.querySelectorAll('.c-colors-palette .c-color').forEach(t => {
                (t as HTMLElement).onclick = e => {
                    let color = (e.target as HTMLElement).style.backgroundColor;
                    let [r, g, b, a] = color.replace('rgba', '').replace('rgb', '').replace('(', '').replace(')', '').split(',');
                    let attrs = this.element.attributes;
                    attrs['red'].value = r;
                    attrs['green'].value = g;
                    attrs['blue'].value = b;
                    attrs['alpha'].value = a || 1;
                    this.updateColor();
                }
            });
        }

        bindColor() {
            let color = this.displayer.style.backgroundColor;
            let [r, g, b, a] = color.replace('rgba', '').replace('rgb', '').replace('(', '').replace(')', '').split(',');
            this.red = parseInt(r);
            this.green = parseInt(g);
            this.blue = parseInt(b);;
            this.alpha = parseFloat(a) || 1;
            let input = this.element.querySelector('input[type="hidden"]') as HTMLInputElement;
            input.value = color;
            let event = new Event('change');
            input.dispatchEvent(event);
            this.displayer.onclick = () => {
                this.bindColor();
            }
        }

        update() {
            let [r, g, b] = this.convertHSVtoRGB(this.hue, 100, 100);
            let color = `rgb(${r}, ${g}, ${b})`;
            this.colorBlock.style.backgroundColor = color;
            [r, g, b] = this.convertHSVtoRGB(this.hue, this.saturation, this.value);
            this.transparentStrip.style.backgroundImage = `linear-gradient(15deg, transparent, rgb(${r}, ${g}, ${b}))`;
            color = this.alpha == 1 ? `rgb(${r}, ${g}, ${b})` : `rgba(${r}, ${g}, ${b}, ${this.alpha})`;
            this.displayer.style.backgroundColor = color;
            this.colorInput.value = color;
            if (this.bindingType == 2) 
                this.bindColor();
        }

        startDrag(clientX: number, clientY: number, x: number, y: number) {
            this.xStart = clientX;
            this.yStart = clientY;
            this.leftStart = x - 7;
            this.topStart = y - 7;
            document.body.onmousemove = e => {
                let loc = this.colorBlock.getBoundingClientRect();
                let difX = e.clientX - this.xStart, difY = e.clientY - this.yStart;
                let left = this.leftStart + difX;
                if (left < - 7)
                    left = -7;
                if (left > loc.width - 7)
                    left = loc.width - 7;
                this.selector.style.left = `${left}px`;
                let top = this.topStart + difY;
                if (top <= -7)
                    top = -7;
                if (top > loc.height - 7)
                    top = loc.height - 7;
                this.selector.style.top = `${top}px`;
                this.saturation = Math.floor((left + 7) / loc.width * 100);
                this.value = 100 - Math.floor((top + 7) / loc.height * 100);
                this.update();
            }

            document.body.onmouseup = () => {
                document.body.onmouseup = document.body.onmousemove = null;
            }
        }

        public updateColor() {
            let attrs = this.element.attributes;
            this.bindingType = parseInt(attrs['bindingType'].value);
            let red = parseInt(attrs['red'].value);
            let green = parseInt(attrs['green'].value);
            let blue = parseInt(attrs['blue'].value);
            let alpha = parseFloat(attrs['alpha'].value);
            if (this.red != red || this.green != green || this.blue != blue || this.alpha != alpha) {
                this.transparentStrip.style.backgroundImage = `linear-gradient(15deg, transparent, rgb(${red}, ${green}, ${blue}))`;
                this.red = red;
                this.green = green;
                this.blue = blue;
                this.alpha = alpha || 1;
                this.alphaInput.value = (100 * this.alpha).toString();
                let color = alpha == 1 ? `rgb(${red}, ${green}, ${blue})` : `rgba(${red}, ${green}, ${blue}, ${alpha})`;
                this.colorInput.value = color;
                this.displayer.style.backgroundColor = color;
                let [h, s, v] = this.convertRGBAtoHSVA();
                this.hue = h;
                this.saturation = s;
                this.value = v;
                let [r, g, b] = this.convertHSVtoRGB(h, 100, 100);
                color = `rgb(${r}, ${g}, ${b})`;
                this.colorBlock.style.backgroundColor = color;
                this.hueInput.value = h.toString();
                let loc = this.colorBlock.getBoundingClientRect();
                let left = Math.floor(loc.width * s / 100) - 7, top = Math.floor(loc.height * (100 - v) / 100) - 7;
                this.selector.style.left = `${left}px`;
                this.selector.style.top = `${top}px`;
            }
        }

        public convertRGBAtoHSVA():number[] {
            const red = this.red / 255;
            const green = this.green / 255;
            const blue = this.blue / 255;
            const xmax = Math.max(red, green, blue);
            const xmin = Math.min(red, green, blue);
            const chroma = xmax - xmin;
            const value = xmax;
            let hue = 0;
            let saturation = 0;

            if (chroma) {
                if (xmax === red) { hue = ((green - blue) / chroma); }
                if (xmax === green) { hue = 2 + (blue - red) / chroma; }
                if (xmax === blue) { hue = 4 + (red - green) / chroma; }
                if (xmax) { saturation = chroma / xmax; }
            }

            hue = Math.floor(hue * 60);
            return [hue < 0 ? hue + 360 : hue, Math.round(saturation * 100), Math.round(value * 100)];
        }

        public convertHSVtoRGB(h, s, v):number[] {
            const saturation = s / 100;
            const value = v / 100;
            let chroma = saturation * value;
            let hueBy60 = h / 60;
            let x = chroma * (1 - Math.abs(hueBy60 % 2 - 1));
            let m = value - chroma;
            chroma = (chroma + m);
            x = (x + m);
            const index = Math.floor(hueBy60) % 6;
            const red = [chroma, x, m, m, x, chroma][index];
            const green = [x, chroma, chroma, x, m, m][index];
            const blue = [m, m, x, chroma, chroma, x][index];
            return [Math.round(red * 255), Math.round(green * 255), Math.round(blue * 255)];
        }
    }
}
