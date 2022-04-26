/// <reference path="Common.js" />
/// <reference path="Control.js" />
(function ($) {
    var curentControl;
    function getId() {
        var max = 0;
        $('fieldset').each(function () {
            var id = $(this).attr('id');
            if (id && id.length > 2) {
                id = parseInt(id.substr(2));
                if (id > max)
                    max = id;
            }
        });
        return "id" + (max + 1);
    }
    function getData(element) {
        var $element = $(element);
        var panel = new Object();
        panel.controls = [];
        var $link = $element.find('a');
        var array = $link.attr('href').split('-');
        panel.id = parseInt(array[array.length - 1]);
        panel.title = $link.text();
        $element.closest('.t-tabstrip').find($link.attr('href')).find('.filterControl').each(function () {
            panel.controls.push($(this).data('fControl').getData());
        });
        return panel;
    }

    var fPage = function (element, data) {
        this.element = element;
        $(element).mousedown(function (e) {
            $.filtering.mouseDown = true;
            if (curentControl) 
                curentControl.dragStart(e.pageX, e.pageY);
        });
        $(element).mousemove(function (e) {
            if (curentControl) {
                if ($.filtering.mouseDown)
                    curentControl.draging(e.pageX, e.pageY);
                else
                    curentControl.updateCursor(e.pageX, e.pageY);
            }
        });
        $(element).mouseup(function (e) {
            $.filtering.mouseDown = false;
            var newControl = $(e.target).closest('fieldset').data('fControl');
            if (newControl) {
                curentControl.blur();
                curentControl = newControl;
                curentControl.focus();
                curentControl.updateCursor(e.pageX, e.pageY);
            }
            if (curentControl)
                curentControl.drop(e.pageX, e.pageY);
        });
        $('body').keyup(function (e) {
            var key = e.keyCode | e.which, type = 1, hChange = 0, vChange = 0, factor = 1;
            if (e.ctrlKey)
                type = 2;
            if (e.shiftKey)
                factor = 5;
            else
                factor = 1;
            switch (key) {
                case 37: hChange = -factor; break;
                case 38: vChange = -factor; break;
                case 39: hChange = factor; break;
                case 40: vChange = factor; break;
            }
            if (curentControl && (hChange != 0 || vChange != 0)) {
                if (type == 1)
                    curentControl.move(hChange, vChange);
                else
                    curentControl.change(hChange, vChange);
            }
        })
    }

    fPage.prototype = {
        addControl: function (data, $page) {
            if (arguments.length == 1)
                $page = $('.t-content.t-state-active .PanelContent');
            var id = getId();
            if (data.controlType == filterControlKind.boolean) {
                var str = '<table id="' + id + '" class="t-checkBox"><tr><td><span></span></td><td><input '
                str += ' disabled="disabled" /></td></tr></table>';
                $page.append(str);
            }

            $page.append('<fieldset id="' + id + '"></fieldset>');
            var $element = $page.find('#' + id);
            $element.width(data.width);
            $element.height(data.height);
            var offset = $('.t-widget.t-tabstrip.t-header').offset();
            $element.css('left', parseInt(parseInt(data.left) + offset.left  + 6));
            $element.css('top', parseInt(parseInt(data.top) + offset.top + 29));
            $element.addClass('filterControl');
            $element.append('<legend>' + data.faTitle + '</legend>');
            $element.find('legend').append('<span title="حذف"></span>');
            $element.find('legend span').click(function () {
                if (confirm('آیا با حذف موافقید؟'))
                    $element.remove();
            });
            var obj = $.telerik.toJson({ enTitle: data.enTitle, ruleId: data.ruleId });
            $element.append('<input type="hidden" />');
            $element.find('input').val(obj);
            $element.append('<div class="t-inner"></div>');
            $inner = $element.find('.t-inner');
            switch (parseInt(data.controlType)) {
                case filterControlKind.enums:
                    data.enumFields.forEach(function (value) {
                        var str = '<div><input type="checkbox" disabled="disabled" /><span>' + value + '</span></div>';
                        $inner.append(str)
                    });
                    break;
                case filterControlKind.foreignKey:
                    for (var i = 0; i < 8; i++) 
                        $inner.append('<div><span>"آیتم"' + i + '</span></div>');
                    break;
                case filterControlKind.fromTo:
                    $element.css('max-width', '150px');
                    $element.css('max-height', '110px');
                    $element.css('min-height', '100px');
                    let str = '<table><tr><td><label>از:</label></td><td><span>' + data.fromValue + '</span></td></tr>' +
                        '<tr><td><label>تا:</label></td><td><span>' + data.toValue + '</span></td></tr></table>';
                    $inner.append(str);
                    break;
            }
            if (curentControl)
                curentControl.blur();
            curentControl = $('#' + id).fControl(data);
            curentControl.fromValue = data.fromValue;
            curentControl.toValue = data.toValue;
            curentControl.controlType = data.controlType;
            curentControl.enumFields = data.enumFields;
            curentControl.focus();
        },
        getData: function () {
            return getData($(this.element).find('.t-item.t-state-active'));
        },
        getAllData: function () {
            var panels = [];
            var obj = this;
            $(this.element).find('ul li:not(.item-add)').each(function () {
                $('#TabPanels').data('tTabStrip').activateTab($(this), false);
                panels.push(obj.getData());
            });
            return panels;
        }
    }
    $.fn.fPage = function (data) {
        var item = new fPage(this, data);
        $(this).data('fPage', item);
        return item;
    }
}    
)(jQuery);