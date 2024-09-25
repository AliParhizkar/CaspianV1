(function ($) {
    var $C = $.caspian, mouseisDows, leftStart, topStart, xStart, yStart;
    $C.colorPicker = function (element) {
        this.element = element;
        let target = this;
        $(element).find('.c-color-selector').mousedown(e => {
            let pos = $(e.target).position();
            target.dragStart(e.clientX, e.clientY, pos.left, pos.top);
            $C.__colorpicker = this;
        });
        $(element).find('.c-color-block').bind('mousedown', e => {
            $(element).find('.c-color-selector').css('left', e.offsetX - 7).css('top', e.offsetY - 7);
            let $element = $(e.target), width = $element.width(), height = $element.height();
            target.element = $element.closest('.c-colorpicker-panel')[0];
            target.dragStart(e.clientX, e.clientY, e.offsetX - 7, e.offsetY - 7);
            target.saturation = parseInt((e.offsetX) / width * 100);
            target.value = 100 - parseInt((e.offsetY) / height * 100);
            target.update();
            $C.__colorpicker = this;
        });
        $(element).find('.c-colors-hue input').bind('input', e => {
            target.hue = parseInt(e.target.value);
            target.update();
        });
        $(element).find('.c-transparent-strip input').bind('input', e => {
            target.alpha = parseInt(e.target.value) / 100;
            target.update();
        });
        $('body').bind('mousemove.colorpicker', e => {
            if (mouseisDows) {
                let $element = $(element).find('.c-color-black');
                let width = $element.width(), height = $element.height();
                let difX = e.clientX - xStart;
                let left = leftStart + difX;
                if (left < -7)
                    left = -7;
                if (left > width - 7)
                    left = width - 7;
                let difY = e.clientY - yStart;
                let top = topStart + difY;
                if (top < -7)
                    top = -7;
                if (top > height - 7)
                    top = height - 7;
                $(element).find('.c-color-selector').css('left', left).css('top', top);
                target.saturation = parseInt((left + 7) / width * 100);
                target.value = 100 - parseInt((top + 7) / height * 100);
                target.update();
            }
        });
        $('body').bind('mouseup.colorpicker', e => {
            if (e.target.type == 'range' || mouseisDows || $(e.target).hasClass('c-color')) {
                $C.__colorpicker.bindColor();
            }
            mouseisDows = false;
        });
        $(element).find('.c-colors-palette .c-color').mouseup(e => {
            let [r, g, b, a] = $(e.target).css('background-color').replace('rgba', '').replace('rgb', '').replace('(', '').replace(')', '').split(',');
            $(this.element).attr('red', r).attr('green', g).attr('blue', b).attr('alpha', a);
            target.updateColor(r, g, b, a || 1);
        });
        $(element).find('.c-color-displayer').click(e => {
            let color = $(e.target).css('background-color');
            let $input = $(element).find('input[type="hidden"]').val(color);
            let [r, g, b, a] = $(e.target).css('background-color').replace('rgba', '').replace('rgb', '').replace('(', '').replace(')', '').split(',');
            this.red = r;
            this.green = g;
            this.blue = b;
            this.alpha = a || 1;
            let event = new Event('change');
            $input[0].dispatchEvent(event);
        });
    }

    $C.colorPicker.prototype = {
        
        bindColor: function () {
            if (this.bindingType == 2) {
                let color = $(this.element).find('.c-color-displayer').css('background-color');
                let [r, g, b, a] = color.replace('rgba', '').replace('rgb', '').replace('(', '').replace(')', '').split(',');
                this.red = r;
                this.green = g;
                this.blue = b;
                this.alpha = a || 1;
                let $input = $(this.element).find('input[type="hidden"]').val(color);
                let event = new Event('change');
                $input[0].dispatchEvent(event);
            }
        },
        updateColor: function () {
            this.bindingType = parseInt($(this.element).attr('bindingType'));
            let red = parseInt($(this.element).attr('red'));
            let green = parseInt($(this.element).attr('green'));
            let blue = parseInt($(this.element).attr('blue'));
            let alpha = parseFloat($(this.element).attr('alpha'));
            if (this.red != red || this.green != green || this.blue != blue || this.alpha != alpha) {
                $(this.element).find('.c-transparent-strip').css('background-image',
                    `linear-gradient(15deg, transparent, rgb(${red}, ${green}, ${blue}))`);
                this.red = red;
                this.green = green;
                this.blue = blue;
                this.alpha = alpha || 1;
                $(this.element).find('.c-transparent-strip input').val(100 * this.alpha);
                let color = alpha == 1 ? `rgb(${red}, ${green}, ${blue})` : `rgba(${red}, ${green}, ${blue}, ${alpha})`;
                $(this.element).find('.c-color-number input').val(color);
                $(this.element).find('.c-color-displayer').css('background-color', color);
                let [h, s, v] = this.convertRGBAtoHSVA();
                this.hue = h;
                this.saturation = s;
                this.value = v;
                let [r, g, b] = this.convertHSVtoRGB(h, 100, 100);
                color = `rgb(${r}, ${g}, ${b})`;
                let $element = $(this.element).find('.c-color-block');
                $element.css('background-color', color);
                $(this.element).find('.c-colors-hue input').val(h);
                let left = parseInt($element.width() * s / 100) - 7, top = parseInt($element.height() * (100 - v) / 100) - 7;
                $(this.element).find('.c-color-selector').css('left', left).css('top', top);
            }
        },
        convertRGBAtoHSVA: function () {
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
        },
        dragStart: function (x, y, left, top) {
            mouseisDows = true;
            leftStart = left;
            topStart = top;
            xStart = x;
            yStart = y;
        },
        update: function () {
            let [r, g, b] = this.convertHSVtoRGB(this.hue, 100, 100);
            let color = `rgb(${r}, ${g}, ${b})`;
            $(this.element).find('.c-color-block').css('background-color', color);
            
            [r, g, b] = this.convertHSVtoRGB(this.hue, this.saturation, this.value);
            $(this.element).find('.c-transparent-strip').css('background-image', `linear-gradient(15deg, transparent, rgb(${r}, ${g}, ${b}))`);
            color = this.alpha == 1 ? `rgb(${r}, ${g}, ${b})` : `rgba(${r}, ${g}, ${b}, ${this.alpha})`;
            $(this.element).find('.c-color-displayer').css('background-color', color);
            $(this.element).find('.c-color-number input').val(color);
        },
        convertHSVtoRGB: function (h, s, v) {
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

    $.fn.colorPicker = function () {
        if (!$(this).data('ColorPicker'))
            $(this).data('ColorPicker', new $C.colorPicker(this));
        return $(this).data('ColorPicker');
    };
})(jQuery);