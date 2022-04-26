(function ($) {
    var $f = $.form, guid, hasFocuse;
    function init(element) {
        //var $item = $(element).find('.checklistboxcontent');
        //$item.width($(element).width() - 2);
        //$item.height($(element).height() + 10);
    }

    var fCheckListBox = function (element) {
        this.multiLine = true;
        this.element = element;
        var text = 'کنترل' + $(element).attr('id').substring(2);
        fControl.prototype.constructor.call(this);
        $(element).find('td').first().append('<fieldset class="checklistboxcontent"><legend>' + text + '</legend></fieldset>');
        var $element = $(element).find('.checklistboxcontent');
        var str = '<table cellpadding="0" style="width:100%" cellspacing="0">'
        var isFirst = true;
        for (var i = 1; i < 10; i++) {
            str += '<tr><td style="width:30px;height:29px;"><input type="radio"';
            if (isFirst)
                str += ' checked="checked" '; 
            str += 'disabled="disabled"/></td><td><span>' + 'آیتم' + i + '</span></td></tr>'
            isFirst = false;
        }
        str += '</table>';
        $element.append(str);
        var self = this;
        $element.dblclick(function () {
            let data = self.getData();
            $.form.dotNetObject.invokeMethodAsync('ShowListWindow', data);

            //var win = $.telerik.getWindow();
            //win.formContentUrl = checkListBoxWindowUrl;
            //win.control = self;
            //win.selectedObject = self.getData();
            //win.title('چک لیست');
            //win.size(380, 330);
            //win.open();
        });
        init(element);
    };
    fCheckListBox.prototype = Object.create(fControl.prototype);
    fCheckListBox.prototype.getElement = function () {

    };
    fCheckListBox.prototype.setDefaultName = function (name) {
        $(this.element).find('fieldset legend').text(name);
    };
    fCheckListBox.prototype.drag = function (x, y) {
        fControl.prototype.drag.call(this, x, y);
        init(this.element);
    };
    fCheckListBox.prototype.drop = function (x, y) {
        fControl.prototype.drop.call(this, x, y);
        init(this.element);
    };
    fCheckListBox.prototype.border = function () {
        return null;
    };
    fCheckListBox.prototype.getData = function () {
        var $element = $(this.element).find('.checklistboxcontent');
        var data = fControl.prototype.getData.call(this);
        data.text = $element.find('legend').text();
        data.formControlType = 3;
        data.multiSelect = $element.find('input[type="checkbox"]').first().attr('type') ==
            'checkbox';

        data.isRequired = this.isRequired;
        data.items = [];
        $element.find('tr').each(function () {
            data.items.push($(this).find('td').eq(1).text());
        });
        return data;
    };
    fCheckListBox.prototype.setData = function (data) {
        $(this.element).data('pageData', data);
    };
    fCheckListBox.prototype.init = function (data) {
        fControl.prototype.init.call(this, data);
        var $element = $(this.element).find('.checklistboxcontent');
        var str = '<legend>' + decodeURIComponent(data.text) + '</legend><table cellpadding="0" style="width:100%" cellspacing="0">';
        var isFirst = true;
        data.items.forEach(function (item) {
            str += '<tr><td style="width:30px;height:29px;"><input type="';
            if (data.multiSelect)
                str += 'checkbox"';
            else
                str += 'radio"';
            if (isFirst || data.multiSelect)
                str += ' checked="checked" ';
            str += 'disabled="disabled"/></td><td><span>' + decodeURIComponent(item) +
                '</span></td></tr>';
            isFirst = false;
        });
        str += '</table>';
        $element.html(str);
        this.isRequired = data.isRequired;
        this.setData(data.checkListBox);
    };
    fCheckListBox.prototype.focus = function () {
        fControl.prototype.focus.call(this);
        hasFocuse = true;
    };
    fCheckListBox.prototype.blur = function () {
        fControl.prototype.blur.call(this);
        hasFocuse = false;
    };

    $.fn.fCheckListBox = function () {
        var item = new fCheckListBox(this);
        item.controlType = controlKind.checkListBox;
        $(this).data('fCheckListBox', item);
        return item;
    }
})(jQuery);
