/// <reference path="Common.js" />
(function ($) {
    var $f = $.form, status, focused, widthStart, heightStart, leftStart, topStart;
    function initPanel(element) {
        var height = $(element).innerHeight();
        var $cell = $(element).find('.text td');
        $cell.css('line-height', '');
        var lineHeight = $cell.css('line-height');
        if (lineHeight == 'normal')
            lineHeight = $cell.innerHeight();
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
            $cell.height(height - 6);
        }
    }
    var fPanel = function (element) {
        this.element = element;
        this.multiLine = true;
        fControl.prototype.constructor.call(this);
        $(element).find('td').last().append('<table cellpadding="0" cellspacing="0" style="width:' + $(element).outerWidth() + 'px" class="text"><tr><td style="background-color:#c6d9f0;border-color:rgb(128, 100, 162);border-style:solid;border-width:2px;">&emsp;</td></tr></table>');
        $(element).css('z-index', 1);
        initPanel(element);
    };
    fPanel.prototype = Object.create(fControl.prototype);
    fPanel.prototype.setDefaultName = function (name) {
        $(this.element).find('table td').text(name);
    }
    fPanel.prototype.drag = function (x, y) {
        fControl.prototype.drag.call(this, x, y);
        initPanel(this.element);
    }
    fPanel.prototype.getStatus = function () {
        var status = fControl.prototype.getStatus.call(this);
        if (status == 5)
            return 0
        return status;
    }
    fPanel.prototype.change = function (width, height) {
        fControl.prototype.change.call(this, width, height);
        var $element = $(this.element).find('.text')
        $element.width($element.width() + width);
        $element = $element.find('td');
        $element.width($element.width() + width);
        $element.height($element.height() + height);
        initPanel(this.element);
    }
    fPanel.prototype.format = function (value) {
        if (arguments.length == 0)
            return this.formatValue;
        this.formatValue = value;
    }
    fPanel.prototype.text = function (text) {
        if (arguments.length == 0)
            return $(this.element).find('.text td').text();
        $(this.element).find('.text td').text(text);
    }
    fPanel.prototype.getElement = function () {
        return $(this.element).find('.text td')[0];
    }
    fPanel.prototype.color = function (color) {
        if (arguments.length == 0)
            return $(this.element).find('.text td').css('color');
        $(this.element).find('.text td').css('color', color);
    }
    fPanel.prototype.backGroundColor = function (color) {
        var $item = $(this.element).find('.text td');
        if (arguments.length == 0)
            return $item.css('background-color');
        if (color == '#0000ffff')
            color = 'Transparent';
        $item.css('background-color', color);
    }
    fPanel.prototype.align = function (alignment) {
        if (!alignment)
            return new rAlign().getAlign(this.getElement(), controlKind.label);
        alignment.initElement(this.getElement(), controlKind.panel);
    };
    fPanel.prototype.border = function (border) {
        if (!border)
            return new rBorder().getBorder(this.getElement());
        border.initElement(this.getElement());
        initPanel(this.element);
    }
    fPanel.prototype.font = function (font) {
        if (arguments.length == 0)
            return new rFont().getFont(this.getElement());
        font.initElement(this.getElement());
    }
    fPanel.prototype.getData = function () {
        var data = fControl.prototype.getData.call(this), $element = $(this.getElement());
        data.formControlType = 5;
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
    fPanel.prototype.init = function (data) {
        this.inputControlType = data.inputControlType;
        this.isRequired = data.isRequired;
        this.multiLine = data.multiLine
        var $element = $(this.getElement());
        fControl.prototype.init.call(this, data);
        if (data.text)
            $element.text(decodeURIComponent(data.text));
        var align = new rAlign();
        $.extend(align, data);
        align.initElement($element[0], controlKind.panel);
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
        initPanel(this.element);
    }
    $.fn.fPanel = function () {
        var item = new fPanel(this);
        item.controlType = controlKind.panel;
        $(this).data('fPanel', item);
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
