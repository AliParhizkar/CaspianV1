/// <reference path="Common.js" />
(function ($) {
    var $f = $.form, status, focused, widthStart, heightStart, leftStart, topStart;
    function initTextBox(element) {
        var height = $(element).height();
        var $cell = $(element).find('.text td');
        $cell.css('line-height', '');
        var lineHeight = $cell.css('line-height');
        if (lineHeight == 'normal')
            lineHeight = $cell.height();
        else {
            lineHeight = lineHeight.substr(0, lineHeight.length - 2);
            lineHeight = parseInt(lineHeight);
        }
        var border = new rBorder().getBorder($cell[0]), borderWidth = 0;
        if (border) {
            ///بردر بالا
            if ((border.borderKind & 1) == 1)
                borderWidth += border.width;
            ///بردر پایین
            if ((border.borderKind & 2) == 2)
                borderWidth += border.width;
        }
        if (height < lineHeight) {
            $cell.css('line-height', (height - borderWidth) + 'px');
            $cell.height(height - borderWidth);
        }
        else {
            $cell.css('line-height', 'normal');
            $cell.height(height - borderWidth - 1);
        }
    }
    var fTextBox = function (element) {
        this.element = element;
        this.multiLine = false;
        fControl.prototype.constructor.call(this);
        $(element).find('td').last().append('<table cellpadding="0" cellspacing="0" style="width:' + $(element).outerWidth() + 'px" class="text"><tr><td style="border-color:#000;border-style:solid;border-width:1px;">&emsp;</td></tr></table>');
        initTextBox(element);
        var self = this;
        $(this.element).find('.text').dblclick(function (e) {
            let data = self.getData();
            $.form.dotNetObject.invokeMethodAsync('ShowTextboxWindow', data);
            //var win = $.telerik.getWindow();
            //win.control = self;
            //win.selectedObject = self.getData();
            //win.title("متن جعبه متن");
            //win.size(350, 110);
            //win.formContentUrl = textBoxWindowUrl + '?text=' + self.text();
            //win.center();
            //win.open();
        });
    };
    fTextBox.prototype = Object.create(fControl.prototype);
    fTextBox.prototype.drag = function (x, y) {
        fControl.prototype.drag.call(this, x, y);
        initTextBox(this.element);
    }
    fTextBox.prototype.change = function (width, height) {
        fControl.prototype.change.call(this, width, height);
        var $element = $(this.element).find('.text')
        $element.width($element.width() + width);
        $element = $element.find('td');
        $element.width($element.width() + width);
        $element.height($element.height() + height);
        initTextBox(this.element);
    }
    fTextBox.prototype.format = function (value) {
        if (arguments.length == 0)
            return this.formatValue;
        this.formatValue = value;
    }
    fTextBox.prototype.text = function (text) {
        if (arguments.length == 0)
            return $(this.element).find('.text td').text();
        $(this.element).find('.text td').text(text);
    }
    fTextBox.prototype.getElement = function () {
        return $(this.element).find('.text td')[0];
    }
    fTextBox.prototype.color = function (color) {
        if (arguments.length == 0)
            return $(this.element).find('.text td').css('color');
        $(this.element).find('.text td').css('color', color);
    }
    fTextBox.prototype.backGroundColor = function (color) {
        var $item = $(this.element).find('.text td');
        if (arguments.length == 0)
            return $item.css('background-color');
        if (color == '#0000ffff')
            color = 'Transparent';
        $item.css('background-color', color);
    }
    fTextBox.prototype.align = function (alignment) {
        if (!alignment)
            return new rAlign().getAlign(this.getElement(), controlKind.label);
        alignment.initElement(this.getElement(), controlKind.textBox);
    };
    fTextBox.prototype.border = function (border) {
        if (!border)
            return new rBorder().getBorder(this.getElement());
        border.initElement(this.getElement());
        initTextBox(this.element);
    }
    fTextBox.prototype.font = function (font) {
        if (arguments.length == 0)
            return new rFont().getFont(this.getElement());
        font.initElement(this.getElement());
    }
    fTextBox.prototype.getData = function () {
        var data = fControl.prototype.getData.call(this), $element = $(this.getElement());
        data.formControlType = 2;
        data.inputControlType = this.inputControlType;
        data.isRequired = this.isRequired;
        data.multiLine = this.multiLine
        var border = new Object()
        $.myExtend(border, this.border());
        data.border = border;
        data.backGroundColor = new Object();
        data.backGroundColor.colorString = $element.css('background-color');
        $.myExtend(data, this.align());
        ////------Font&ForeGround Color
        data.color = new Object();
        data.color.colorString = $element.css('color');
        var font = new Object();
        $.myExtend(font, this.font());
        data.font = font;
        //----------------------------------------------------
        if ($element.text())
            data.text = encodeURIComponent($element.text());
        data.format = this.format();
        return data;
    }
    fTextBox.prototype.init = function (data) {
        this.inputControlType = data.inputControlType;
        this.isRequired = data.isRequired;
        this.multiLine = data.multiLine
        var $element = $(this.getElement());
        fControl.prototype.init.call(this, data);
        if (data.text)
            $element.text(decodeURIComponent(data.text));
        var align = new rAlign();
        $.extend(align, data);
        align.initElement($element[0], controlKind.textBox);
        ///Font&ForeGround Color
        if (data.color)
            $element.css('color', data.color.colorString);
        var font = new rFont();
        $.extend(font, data.font);
        font.initElement($element[0]);
        ///Border
        var border = new rBorder();
        this.multiLine = data.multiLine;
        $.myExtend(border, data.border);
        border.initElement($element[0]);
        ///BackGroundColor
        this.backGroundColor(data.backGroundColor.colorString);
        this.format(data.format);
        initTextBox(this.element);
    }
    fTextBox.prototype.setDefaultName = function (name) {
        $(this.element).find('.text td').text(name);
    }
    $.fn.fTextBox = function () {
        var item = new fTextBox(this);
        item.controlType = controlKind.textBox;
        $(this).data('fTextBox', item);
        return item;
    }
})(jQuery);

var statusType = {
    none: 0,
    changeWidthFromLeft: 1,
    changeWidthFromRight: 2,
    changeHeightFromTop: 3,
    changeHeightFromBottom: 4,
    move: 5
}
