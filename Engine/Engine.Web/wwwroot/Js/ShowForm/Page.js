///// <reference path="../../../ReportEngin/Scripts/ReportPrint/Common.js" />
///// <reference path="Control.js" />
//(function ($) {
    
//    var formsData = null;
//    function addLabelOrPanel(form, label) {
//        var str = '<table class="label" cellpadding="0" cellspacing="0" style="position:absolute;"><tr><td>';
//        if (label.formControlType == 1)
//            str += decodeURIComponent(label.text);
//        str += '</td></tr></table>';
//        $(form).append(str);
//        var $table = $(form).find('.label').last();
//        var $element = $table.find('td');
//        new rPosition().initElement($table[0], label);
//        if (label.formControlType == 1) {
//            var align = new rAlign();
//            $.extend(align, label);
//            align.initElement($element[0], controlKind.textBox);
//        }
//        ///Font&ForeGround Color
//        $element.css('color', label.color.colorString).css('background-color',
//            label.backGroundColor.colorString);
//        if (label.formControlType == 1) {
//            var font = new rFont();
//            $.extend(font, label.font);
//            font.initElement($element[0]);
//        }
//        ///Border
//        var border = new rBorder();
//        $.myExtend(border, label.border);
//        border.initElement($element[0]);
//    }
//    function addInputControl(form, input, disabled) {
//        var str = '<div style="position:absolute;" class="t-widget ';
//        var id = '_' + input.id;
//        switch (input.inputControlType) {
//            case 1:
//            case 2:
//            case 4:
//                input.position.left -= $.report.getWidth(12);
//                if (!input.multiLine) 
//                    str += 't-numerictextbox"><input style="width:100%" class="t-input" id="' + id + '" name="' + id + '"/></div>';
//                else
//                    str += 't-multitextbox"><textarea style="height:100%" class="t-input" id="' + id + '" name="' + id + '"></textarea>';
//                break; 
//            case 3:
//                str += 't-datepicker"><div class="t-picker-wrap" style="float:right"><input autocomplete="off" class="t-input" id="' + id + '" name="' + id + '"/><span class="t-select" style="left:inherit"><span class="t-icon t-icon-calendar" title="Open the calendar">Open the calendar</span></span></div></div>';
//                break;
//        }
//        $(form).append(str);
//        new rPosition().initElement($('#' + id).closest('.t-widget')[0], input);
//        if (input.inputControlType == 3)
//            $('#' + id).width($.report.getPixelWidth(input.position.width) - 30);
//        var data = { minValue: -2147483648, maxValue: 2147483647, digits: 0, type: 'numeric' };
//        var ctr = null;
//        switch (input.inputControlType) {
//            case 1:
//                $('#' + id).tTextBox(data);
//                ctr = $('#' + id).data('tTextBox');
//                break;
//            case 2:
//                data.digits = 2;
//                $('#' + id).tTextBox(data);
//                ctr = $('#' + id).data('tTextBox');
//                break;
//            case 3:
//                $('#' + id).tDatePicker();
//                ctr = $('#' + id).data('tDatePicker');
//                break;
//            case 4:
//                $('#' + id).tTextBox({ type: 'string' });
//                ctr = $('#' + id).data('tTextBox');
//                break;
//        }
//        if (input.value)
//            ctr.value(decodeURIComponent(input.value));
//        if (disabled)
//            ctr.disable();
//    }
//    function addDropdownList(form, ctr) {
//        var id = '_' + ctr.id;
//        var str = '<div style="position:absolute;" class="t-widget t-dropdown t-header">' +
//            '<div class="t-dropdown-wrap t-state-default"><span class="t-input"> </span><span class="t-select"><span class="t-icon t-arrow-down">select</span></div>' +
//            '<input type="text" style="display:none" name="' + id + '" id="' + id + '" autocomplete="off"/></div>';
//        ctr.position.height = $.report.getHeight(36);
//        ctr.position.width += $.report.getWidth(12);
//        ctr.position.left -= $.report.getWidth(12);
//        $(form).append(str);
//        var data = { data: [] };
//        var value = 1;
//        data.data.push({ Text: 'عدم انتخاب', Value: '' });
//        ctr.items.forEach(function (item) {
//            data.data.push({
//                Text: item,
//                Value:value
//            });
//            value++;
//        });
//        $('#' + id).tDropDownList(data);
//        $('#' + id).data('tDropDownList').value(ctr.value);
//        new rPosition().initElement($('#' + id).closest('.t-widget')[0], ctr);
//    }
//    function addCheckListBox(form, ctr) {
//        var id = '_' + ctr.id;
//        var str = '<fieldset style="background-color:white;overflow:hidden;border-radius:7px;position:absolute;margin:-10px 0 0 0;padding:0" id="' + id + '" class="t-widget t-Enumes"><legend style="color:blue;margin-right:10px">' +
//            decodeURIComponent(ctr.text) + '</legend><input class=".t-input" type="hidden" value=""><div style="margin-top:-5px" class="t-inner">';
//        var index = 1;
//        ctr.items.forEach(function (item) {
//            str += '<div';
//            if (index % 2 == 0)
//                str += ' class="t-alt"';
//            var value = index;
//            str += ' style="height:29px;padding-right:4px;"><input style="margin-top:3px;" name="' + id + '" type="'
//            if (ctr.multiSelect) {
//                value = Math.pow(2, index - 1);
//                str += 'checkbox';
//            }
//            else
//                str += 'radio';
//            if (ctr.value == index)
//                str += '" checked="checked'
//            str += '" value="' + value + '">';
//            str += '<label for="' + id + '-' + index + '">' + decodeURIComponent(item) + '</span>'
//            str += '</div>';
//            index++;
//        });
//        str += '</div></fieldset>';
//        $(form).append(str);
//        new rPosition().initElement($('#' + id).closest('.t-widget')[0], ctr);
//    }
//    var fPage = function (element, data) {
//        formsData = eval('(' + data + ')');
//        this.element = element;
//    }
//    fPage.prototype = {
//        init: function () {
//            var self = this;
//            var index = 1;
//            formsData.forEach(function (form) {
//                var bond = form.bond;
//                $(self.element).append('<div style="margin:0 auto" class="form bond"></div>');
//                var $form = $(self.element).find('.form').last();
//                $form.width($.report.getPixelWidth(bond.width))
//                    .height($.report.getPixelHeight(bond.height));
//                $('#action').width($form.width());
//                bond.controls.forEach(function (ctr) {
//                    switch (ctr.formControlType) {
//                        case 1:
//                        case 5:
//                            addLabelOrPanel($form[0], ctr);
//                            break;
//                        case 2:
//                            addInputControl($form[0], ctr, formsData.length != index);
//                            break;
//                        case 3:
//                            addCheckListBox($form[0], ctr)
//                            break;
//                        case 4:
//                            addDropdownList($form[0], ctr);
//                            break;
//                    }
//                });
//                index++;
//            });
//        },
//        getData: function () {
//            var data = [];
//            var self = this;
//            var value, id;
//            $(this.element).find('.t-widget').each(function () {
//                if ($(this).hasClass('t-numerictextbox') || $(this).hasClass('t-datepicker') || $(this).hasClass('t-dropdown')) {
//                    id = $(this).find('input').attr('id');
//                    value = $.telerik.getValueOfItem(id);
//                }
//                if ($(this).hasClass('t-multitextbox')) {
//                    id = $(this).find('textarea').attr('id');
//                    value = $(this).find('textarea').val();
//                }
//                if ($(this).hasClass('t-Enumes')) {
//                    id = $(this).attr('id');
//                    value = 0;
//                    $(this).find('input:checked').each(function () {
//                        value |= $(this).val();
//                    });
//                }
//                if (value && value != '') {
//                    id = id.substr(1);
//                    var obj = new Object();
//                    obj[self.fieldName] = id;
//                    obj.value = value;
//                    data.push(obj);
//                }
//            });
//            return data;
//        }
//    }
//    $.fn.fPage = function (data) {
//        var item = new fPage(this, data);
//        $(this).data('fPage', item);
//        return item;
//    }
//}  
//)(jQuery);