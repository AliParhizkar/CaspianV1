/// <reference path="Common.js" />
(function ($) {
    var $f = $.form, focused, stretch;
    function initDropdownList(element) {
        var $img = $(element).find('.imageBoxDiv');
        if ($img.hasClass('imageBoxDiv')) {
            var border = new rBorder().getBorder($img[0]);
            var borderWidth = 0;
            ///بردر راست
            if (border && (border.borderKind & 4) == 4)
                borderWidth += border.width;
            //بردر چپ
            if (border && (border.borderKind & 8) == 8)
                borderWidth += border.width;
            $img.width($(element).width() - borderWidth);
        }
    }
    
    var fDropdownList = function (element) {
        this.element = element;
        this.onlyWidthChange = true;
        fControl.prototype.constructor.call(this);
        $(element).find('td').first().append('<span class="imageboxicon"></span>');
        $(element).find('td').last().append('<div style="height:32px;padding-top:5px;text-align:right;border-color:#000000;border-style:solid;border-width:1px;" class="imageBoxDiv"></div>');
        initDropdownList(element);
        var self = this;
        $(element).dblclick(function () {
            let data = self.getData();
            $.form.dotNetObject.invokeMethodAsync('ShowListWindow', data);
            //var win = $.telerik.getWindow();
            //win.formContentUrl = checkListBoxWindowUrl;
            //win.selectedObject = self.getData();
            //win.control = self;
            //win.size(380, 280);
            //win.title("لیست کرکره ای");
            //win.center();
            //win.open();
        });
    };
    fDropdownList.prototype = Object.create(fControl.prototype);
    fDropdownList.prototype.getElement = function () {
        return $(this.element).find('.imageBoxDiv')[0];
    }
    fDropdownList.prototype.setDefaultName = function (name) {
            $(this.element).find('.imageBoxDiv').text(name);
    }
    fDropdownList.prototype.drag = function (x, y) {
        fControl.prototype.drag.call(this, x, y);
        initDropdownList(this.element);
    }
    fDropdownList.prototype.change = function () {
        initDropdownList(this.element);
    }
    fDropdownList.prototype.updateCursor = function (x, y) {
        fControl.prototype.updateCursor.call(this, x, y);
    }
    fDropdownList.prototype.backGroundColor = function (color) {
        if (arguments.length == 0)
            return $(this.getElement()).css('background-color');
        $(this.getElement()).css('background-color', color);
    }
    fDropdownList.prototype.text = function (txt) {
        if (arguments.length == 0)
            return $(this.element).find('.imageBoxDiv').text();
        $(this.element).find('.imageBoxDiv').text(txt);
    }
    fDropdownList.prototype.align = function (alignment) {
        if (!alignment) {
            if (stretch == true)
                return null;
            return new rAlign().getAlign(this.getElement(), controlKind.dropdownList);
        }
        alignment.initElement(this.getElement(), controlKind.dropdownList);
        initDropdownList(this.element);
    }
    fDropdownList.prototype.border = function (border) {
        if (!border)
            return new rBorder().getBorder(this.getElement());
        border.initElement(this.getElement());
        initDropdownList(this.element);
    }
    fDropdownList.prototype.getData = function () {
        var data = fControl.prototype.getData.call(this), $element = $(this.getElement());
        data.formControlType = 4;
        var border = new Object();
        $.myExtend(border, this.border());
        data.border = border;
        data.backGroundColor = new Object();
        data.backGroundColor.colorString = $element.css('background-color');
        $.myExtend(data, this.align());
        data.text = this.text();
        data.items = this.items;
        data.isRequired = this.isRequired;
        return data;
    }
    fDropdownList.prototype.font = function (font) {
        if (arguments.length == 0)
            return new rFont().getFont(this.getElement());
        font.initElement(this.getElement());
    }
    fDropdownList.prototype.init = function (data) {
        var $element = $(this.getElement());
        fControl.prototype.init.call(this, data);
        this.text(decodeURIComponent(data.text));
        var align = new rAlign();
        $.extend(align, data);
        align.initElement($element[0], controlKind.dropdownList);
        if (data.border) {
            var border = new rBorder();
            $.myExtend(border, data.border);
            border.initElement(this.getElement());
        }
        this.backGroundColor(data.backGroundColor.colorString)
        this.items = data.items;
        this.isRequired = data.isRequired;
        initDropdownList(this.element);
    }
    $.fn.fDropdownList = function () {
        var item = new fDropdownList(this);
        item.controlType = controlKind.dropdownList;
        $(this).data('fDropdownList', item);
        return item;
    }
    var statusType = {
        none: 0,
        changeWidthFromLeft: 1,
        changeWidthFromRight: 2,
        changeHeightFromTop: 3,
        changeHeightFromBottom: 4,
        move: 5
    }
})(jQuery);
