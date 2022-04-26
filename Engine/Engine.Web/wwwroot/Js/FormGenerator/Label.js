/// <reference path="Common.js" />
(function ($) {
    var $f = $.form, status, focused, widthStart, heightStart, leftStart, topStart;
    function initLabel(element) {
        var height = $(element).innerHeight();
        var $cell = $(element).find('.text td');
        $cell.css('line-height', '');
        var lineHeight = $cell.css('line-height');
        if (lineHeight == 'normal' || !lineHeight)
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
            $cell.height(height);
        }
    }
    var fLabel = function (element) {
        $(element).css('background-color', 'transparent');
        this.element = element;
        this.multiLine = true;
        fControl.prototype.constructor.call(this);
        $(element).find('td').last().append('<table cellpadding="0" cellspacing="0" style="width:' + $(element).outerWidth() + 'px" class="text"><tr><td>&emsp;</td></tr></table>');
        initLabel(element);
        var self = this;
        $(this.element).find('.text').dblclick(function (e) {
            let data = self.getData();
            $.form.dotNetObject.invokeMethodAsync('ShowLableWindow', data);
            //var win = $.telerik.getWindow();
            //win.control = self;
            //win.selectedObject = self.getData();
            //win.title("متن برچسب");
            //win.size(400, 250);
            //win.formContentUrl = labelWindowUrl;
            //win.center();
            //win.open();
        });
    };
    fLabel.prototype = Object.create(fControl.prototype);
    fLabel.prototype.drag = function (x, y) {
        fControl.prototype.drag.call(this, x, y);
        initLabel(this.element);
    };
    fLabel.prototype.setDefaultName = function (name) {
        $(this.element).find('.text td').text(name);
    };
    fLabel.prototype.change = function (width, height) {
        fControl.prototype.change.call(this, width, height);
        var $element = $(this.element).find('.text')
        $element.width($element.width() + width);
        $element = $element.find('td');
        $element.width($element.width() + width);
        $element.height($element.height() + height);
        initLabel(this.element);
    }
    fLabel.prototype.text = function (text) {
        if (arguments.length == 0)
            return $(this.element).find('.text td').text();
        $(this.element).find('.text td').text(text);
    };
    fLabel.prototype.getElement = function () {
        return $(this.element).find('.text td')[0];
    };
    fLabel.prototype.color = function (color) {
        if (arguments.length == 0)
            return $(this.element).find('.text td').css('color');
        $(this.element).find('.text td').css('color', color);
    };
    fLabel.prototype.backGroundColor = function (color) {
        var $item = $(this.element).find('.text td');
        if (arguments.length == 0)
            return $item.css('background-color');
        if (color == '#0000ffff')
            color = 'Transparent';
        $item.css('background-color', color);
    };
    fLabel.prototype.align = function (alignment) {
        if (!alignment)
            return new rAlign().getAlign(this.getElement(), controlKind.label);
        alignment.initElement(this.getElement(), controlKind.label);
    };
    fLabel.prototype.border = function (border) {
        if (arguments.length == 0)
            return new rBorder().getBorder(this.getElement());
        border.initElement(this.getElement());
        initLabel(this.element);
    };
    fLabel.prototype.font = function (font) {
        if (arguments.length == 0)
            return new rFont().getFont(this.getElement());
        font.initElement(this.getElement());
    };
    fLabel.prototype.getData = function () {
        var data = fControl.prototype.getData.call(this), $element = $(this.getElement());
        data.formControlType = 1;
        data.specialField = this.specialField
        var border = new Object();
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
        return data;
    };
    fLabel.prototype.init = function (data) {
        var $element = $(this.getElement());
        fControl.prototype.init.call(this, data);
        if (data.text)
            $element.text(decodeURIComponent(data.text));
        var align = new rAlign();
        $.extend(align, data);
        align.initElement($element[0], controlKind.label);
        ///Font&ForeGround Color
        $element.css('color', data.color.colorString);
        var font = new rFont();
        $.extend(font, data.font);
        font.initElement($element[0]);
        ///Border
        var border = new rBorder();
        $.myExtend(border, data.border);
        border.initElement($element[0]);
        ///BackGroundColor
        this.backGroundColor(data.backGroundColor.colorString);
        this.specialField = data.specialField;
        initLabel(this.element);
    };
    $.fn.fLabel = function () {
        var item = new fLabel(this);
        item.controlType = controlKind.label;
        $(this).data('fLabel', item);
        return item;
    };
})(jQuery);

var statusType = {
    none: 0,
    changeWidthFromLeft: 1,
    changeWidthFromRight: 2,
    changeHeightFromTop: 3,
    changeHeightFromBottom: 4,
    move: 5
};
