    /// <reference path="Common.js" />
(function ($) {
    let $r = $.report, focused, stretch;
    function initPictureBox(element) {
        let $img = $(element).find('.imageBoxDiv');
        if ($img.hasClass('imageBoxDiv')) {
            let border = new rBorder().getBorder($img[0]);
            let borderWidth = 0;
            ///بردر راست
            if (border && (border.borderKind & 4) == 4)
                borderWidth += border.width;
            //بردر چپ
            if (border && (border.borderKind & 8) == 8)
                borderWidth += border.width;
            $img.width($(element).width() - borderWidth);
            borderWidth = 0;
            ///بردر بالا
            if (border && (border.borderKind & 1) == 1)
                borderWidth += border.width;
            ///بردر پایین
            if (border && (border.borderKind & 2) == 2)
                borderWidth += border.width;
            $img.height($(element).height() - borderWidth);
        }
    }
    
    let rPictureBox = function (element) {
        this.element = element;
        rControl.prototype.constructor.call(this);
        $(element).find('td').first().append('<span class="imageboxicon"></span>');
        $(element).find('td').last().append('<div style="background-position:50% 50%;" class="imageBoxDiv"></div>');
        initPictureBox(element);
        let self_ = this;
        $(element).dblclick(function () {
            let data = {};
            data.DataLevel = $(this).closest('.bond').attr('DataLevel');
            data.isSubReport = $('body').data('rPage').isSubReport;
            if (data.DataLevel)
                data.DataLevel = parseInt(data.DataLevel);
            data.ReportId = $('#ReportId').val();
            data.imageFileName = self_.imageFileName;
            data.stretch = $(self_.element).find('.imageBoxDiv').css('background-size') == '100% 100%';
            $.report.dotNetObjectReference.invokeMethodAsync('ShowPictureBoxWindow', data);
        });
    };
    rPictureBox.prototype = Object.create(rControl.prototype);
    rPictureBox.prototype.getElement = function(){
        return $(this.element).find('.imageBoxDiv')[0];
    }
    rPictureBox.prototype.drag = function (x, y) {
        rControl.prototype.drag.call(this, x, y);
        initPictureBox(this.element);
    }
    rPictureBox.prototype.change = function(){
        initPictureBox(this.element);
    }
    rPictureBox.prototype.backGroundColor = function(color){
        if (arguments.length == 0)
            return $(this.getElement()).css('background-color');
        $(this.getElement()).css('background-color', color);
    }
    rPictureBox.prototype.text = function (txt) {
        if (arguments.length == 0)
            return $(this.element).find('.imageBoxDiv').text();
        $(this.element).find('.imageBoxDiv').text(txt);
    }
    rPictureBox.prototype.align = function (alignment) {
        if (!alignment) {
            if (stretch == true)
                return null;
            return new rAlign().getAlign(this.getElement(), report.controlKind.pictureBox);
        }
        alignment.initElement(this.getElement(), report.controlKind.pictureBox);
        initPictureBox(this.element);
    }
    rPictureBox.prototype.setImage = function (img, data) {
        let $element = $(this.element).find('.imageBoxDiv');
        $element.css('background-image', 'url(data:image/png;base64,' + img + ')');
        if (data.stretch)
            $element.css('background-size', '100% 100%');
        else
            $element.css('background-size', 'auto');
        this.imageFileName = data.imageFileName;
    }
    rPictureBox.prototype.imageUrl = function (url) {
        if (arguments.length == 0) {
            let url = $(this.element).find('.imageBoxDiv').css('background-image');
            if (url == 'none')
                return null;
            return url;
        }
        $(this.element).find('.imageBoxDiv').css('background-image', url);
        initPictureBox(this.element);
    }
    rPictureBox.prototype.stretch = function (flag) {
        let $imageBox = $(this.element).find('.imageBoxDiv');
        if (arguments.length == 0) {
            if ($.browser.msie) {
                let str = $imageBox.width() + 'px ' + $imageBox.height() + 'px';
                return $imageBox.css('background-size') == str;
            }
            return $imageBox.css('background-size') == '100% 100%';
        }
        if (flag)
            $imageBox.css('background-size', '100% 100%');
        else
            $imageBox.css('background-size', '');
    }
    rPictureBox.prototype.border = function (border) {
        if (!border)
            return new rBorder().getBorder(this.getElement());
        border.initElement(this.getElement());
        initPictureBox(this.element);
    }
    rPictureBox.prototype.getData = function () {
        let data = rControl.prototype.getData.call(this), $element = $(this.getElement());
        data.type = 4;
        let border = new Object();
        $.myExtend(border, this.border());
        data.border = border;
        data.backGroundColor = new Object();
        data.backGroundColor.colorString = $element.css('background-color');
        $.myExtend(data, this.align());
        if (this.imageUrl()) {
            data.imageFileName = this.imageFileName;
        } else
            data.text = this.text();
        if (this.stretch())
            data.stretch = true;
        return data;
    }
    rPictureBox.prototype.init = function (data) {
        let $element = $(this.getElement());
        rControl.prototype.init.call(this, data);
        if (data.uri)
            $(this.element).find('.imageBoxDiv').css('background-image', 'url(data:image/png;base64,' + data.uri + ')');
        if (data.text)
            this.text(decodeURIComponent(data.text));
        let align = new rAlign();
        $.extend(align, data);
        align.initElement($element[0], report.controlKind.pictureBox);
        if (data.border) {
            let border = new rBorder();
            $.myExtend(border, data.border);
            border.initElement(this.getElement());
        }
        this.imageFileName = data.imageFileName;
        if (data.stretch)
            this.stretch(true)
        this.backGroundColor(data.backGroundColor.colorString)
        initPictureBox(this.element);
    }
    $.fn.rPictureBox = function () {
        let item = new rPictureBox(this);
        item.controlType = report.controlKind.pictureBox;
        $(this).data('rPictureBox', item);
        return item;
    }
    let statusType = {
        none: 0,
        changeWidthFromLeft: 1,
        changeWidthFromRight: 2,
        changeHeightFromTop: 3,
        changeHeightFromBottom: 4,
        move: 5
    }
})(jQuery);
