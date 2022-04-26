/// <reference path="Common.js" />
(function ($) {
    var $r = $.report, status, focused, widthStart, heightStart, leftStart, topStart;
    function initTextBox(element) {
        var height = $(element).height() - 2;
        var $cell = $(element).find('.text td');
        $cell.css('line-height', '');
        var lineHeight = $cell.css('line-height');
        if (lineHeight === 'normal')
            lineHeight = $cell.innerHeight();
        else {
            lineHeight = lineHeight.substr(0, lineHeight.length);
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
        $cell.height(height);
        if (height < $cell.height())
            $cell.css('line-height', (height - borderWidth) + 'px');
        else
            $cell.css('line-height', 'normal');
    }
    var rTextBox = function (element) {
        this.element = element;
        rControl.prototype.constructor.call(this);
        $(element).find('td').last().append('<table cellpadding="0" cellspacing="0" style="width:' + $(element).outerWidth() + 'px" class="text"><tr><td></td></tr></table>');
        initTextBox(element);
        var self_ = this;
        $(this.element).find('.text').dblclick(function (e) {
            //var win = $.telerik.getWindow();
            //win.control = this;
            var data = {};
            data.DataLevel = $(this).closest('.bond').attr('DataLevel');
            if (data.DataLevel) {
                data.DataLevel = parseInt(data.DataLevel);
                data.bondType = 4;
            }
            data.ReportId = $('#ReportId').val();
            data.isSubReport = $('body').data('rPage').isSubReport;
            var id = $(element).closest('.bond').attr('id');
            switch (id) {
                case 'reportTitle':
                    data.bondType = 1;
                    break;
                case 'pageHeader':
                    data.bondType = 2;
                    break;
                case 'dataFooter':
                    data.bondType = 5;
                    break;
                case 'pageFooter':
                    data.bondType = 6;
                    break;
            }
            data.titleFa = self_.text();
            data.SystemVariable = self_.getSystemVariable();
            data.systemFiledType = self_.getsystemFiledType();
            if (!data.SystemVariable && !data.systemFiledType && data.DataLevel) {
                var member = self_.member();
                if (member) {
                    member = member.replace('{', '').replace('}', '');
                    data.titleEn = member;
                }
                else
                    data.titleEn = null;
            }
            $.report.dotNetObjectReference.invokeMethodAsync('ShowWindow', data);
            if (e.ctrlKey) {
                //win.size(630, 275);
                //win.title("فرمول");
                //win.formContentUrl = formolaWindowUrl;
            } else {
                //if (win.selectedObject.DataLevel)
                //    win.size(400, 160);
                //else {
                //    switch (win.selectedObject.bondType) {
                //        case 1:
                //            win.size(400, 130);
                //            break;
                //        case 2:
                //        case 6:
                //            win.size(400, 130);
                //            break;
                //        case 5:
                //            win.size(400, 230);
                //            break;
                //        default:
                //            win.size(400, 130);
                //    }
                //}
                //win.title("متن کنترل");
                //win.formContentUrl = textBoxWindowUrl;
            }
            //win.center();
            //win.open();
        });
    };
    rTextBox.prototype = Object.create(rControl.prototype);
    rTextBox.prototype.drag = function (x, y) {
        rControl.prototype.drag.call(this, x, y);
        initTextBox(this.element);
    }
    rTextBox.prototype.change = function (width, height) {
        rControl.prototype.change.call(this, width, height);
        var $element = $(this.element).find('.text')
        $element.width($element.width() + width);
        $element = $element.find('td');
        $element.width($element.width() + width);
        $element.height($element.height() + height);
        initTextBox(this.element);
    }
    rTextBox.prototype.format = function (value) {
        if (arguments.length == 0)
            return this.formatValue;
        this.formatValue = value;
    }
    rTextBox.prototype.text = function (text) {
        if (arguments.length == 0)
            return $(this.element).find('.text td').text();
        $(this.element).find('.text td').text(text);
    }
    rTextBox.prototype.getElement = function () {
        return $(this.element).find('.text td')[0];
    }
    rTextBox.prototype.color = function (color) {
        if (arguments.length == 0)
            return $(this.element).find('.text td').css('color');
        $(this.element).find('.text td').css('color', color);
    }
    rTextBox.prototype.backGroundColor = function (color) {
        var $item = $(this.element).find('.text td');
        if (arguments.length == 0)
            return $item.css('background-color');
        if (color == '#0000ffff')
            color = 'Transparent';
        $item.css('background-color', color);
    }
    rTextBox.prototype.align = function (alignment) {
        if (!alignment)
            return new rAlign().getAlign(this.getElement(), report.controlKind.textBox);
        alignment.initElement(this.getElement(), report.controlKind.textBox);
    };
    rTextBox.prototype.border = function (border) {
        if (!border)
            return new rBorder().getBorder(this.getElement());
        border.initElement(this.getElement());
        initTextBox(this.element);
    }
    rTextBox.prototype.font = function (font) {
        if (arguments.length == 0)
            return new rFont().getFont(this.getElement());
        font.initElement(this.getElement());
    }
    rTextBox.prototype.getData = function () {
        var data = rControl.prototype.getData.call(this), $element = $(this.getElement());
        data.type = 3;
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
    rTextBox.prototype.init = function (data) {
        var $element = $(this.getElement());
        rControl.prototype.init.call(this, data);
        if (data.text)
            $element.text(decodeURIComponent(data.text));
        var align = new rAlign();
        $.extend(align, data);
        align.initElement($element[0], report.controlKind.textBox);
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
        this.format(data.format);
        initTextBox(this.element);
    }
    $.fn.rTextBox = function () {
        var item = new rTextBox(this);
        item.controlType = report.controlKind.textBox;
        $(this).data('rTextBox', item); 
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
