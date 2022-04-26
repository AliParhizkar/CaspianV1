/// <reference path="Common.js" />
(function ($) {
    var $f = $.form, addedToForm, creatingControlId;

    function getCurentBond(x, y) {
        var bond = null;
        $('.bond').each(function () {
            var top = $(this).offset().top, left = $(this).offset().left, width = $(this).width(), height = $(this).height();
            if (y > top && y < top + height && x > left && x < left + width) {
                bond = this;
                return false;
            }
        });
        return bond;
    }

    function getId() {
        var max = 0;
        $('#page .reportcontrol').each(function () {
            var id = parseInt($(this).attr('id').substr(2));
            if (id > max)
                max = id;
        });
        return 'id' + (max + 1);
    }
    function getControl(element, controlType) {
        switch (controlType) {
            case controlKind.label:
                return $(element).label();
            case controlKind.checkBox:
                return $(element).checkBox();
            case controlKind.textBox:
                return $(element).textBox();
            case controlKind.checkListBox:
                return $(element).checkListBox();
            case controlKind.dropdownList:
                return $(element).dropdownList();
        }
    }
    function appentControl(controlType, bond) {
        creatingControlId = getId();
        var str = null;
        switch (controlType) {
            case controlKind.label:
                str = $f.getLabel(creatingControlId, 180, 32);
                break;
            case controlKind.textBox:
                str = $f.getTextBox(creatingControlId, 180, 32);
                break;
            case controlKind.checkListBox:
                str = $f.getCheckListBox(creatingControlId, 200, 100);
                break;
            case controlKind.dropdownList:
                str = $f.getDropdownList(creatingControlId, 180, 32);
                break;
            case controlKind.checkBox:
                str = $f.getCheckBox(creatingControlId);
                break;
            case controlKind.panel:
                str = $f.getPanel(creatingControlId, 200, 150);
                break;
        }
        if (str != null) {
            $(bond).append(str);
            addedToForm = true;
        }
        else
            creatingControlId = null;
    }
    function controlName(controlType) {
        var index = 0, str = '';
        $('#reportTitle').find('.reportcontrol').each(function () {
            switch (controlType) {
                case controlKind.label:
                    index++;
                    str = 'برچسب';
                    break;
                case controlKind.textBox:
                    index++;
                    str = 'جعبه متن';
                    break;
                case controlKind.checkListBox:
                    index++;
                    str = 'جعبه لیست';
                    break;
                case controlKind.dropdownList:
                    index++;
                    str = 'لیست کشویی';
                    break;
                case controlKind.panel:
                    index++;
                    str = 'پنل';
                    break;
            }
        });
        return str + ' ' + index;
    }
    var fToolsBox = function (element) {
        this.element = element;
        var obj = this;
        this.controlType = controlKind.none;
        
        $(this.element).mousedown(function (e) {
            addedToForm = false;
            $item = $(e.target);
            if ($item.hasClass('label'))
                obj.controlType = controlKind.label;
            if ($item.hasClass('textbox'))
                obj.controlType = controlKind.textBox;
            if ($item.hasClass('checklistbox'))
                obj.controlType = controlKind.checkListBox;
            if ($item.hasClass('dropdownlist'))
                obj.controlType = controlKind.dropdownList;
            if ($item.hasClass('checkbox'))
                obj.controlType = controlKind.checkBox;
            if ($item.hasClass('panel'))
                obj.controlType = controlKind.panel;
        });
    };

    fToolsBox.prototype = {
        createAndDrag: function (x, y) {
            if (this.controlType != controlKind.none) {
                if (addedToForm) {
                    var control = $('#' + creatingControlId).data('fControl');
                    if (control)
                        control.drag(x, y);
                    this.controlType = controlKind.none;
                }
                else {
                    var bond = getCurentBond(x, y);
                    $('body').data('fPage').resetCurentControl();
                    if (bond) {
                        appentControl(this.controlType, bond);
                        var control;
                        switch (this.controlType) {
                            case controlKind.dropdownList:
                                control = $('#' + creatingControlId).fDropdownList();
                                break;
                            case controlKind.label:
                                control = $('#' + creatingControlId).fLabel();
                                break;
                            case controlKind.textBox:
                                control = $('#' + creatingControlId).fTextBox();
                                break;
                            case controlKind.checkListBox:
                                control = $('#' + creatingControlId).fCheckListBox();
                                break;
                            case controlKind.panel:
                                control = $('#' + creatingControlId).fPanel();
                                break;
                            case controlKind.checkBox:
                                control = $('#' + creatingControlId).fCheckBox();
                                break;
                        }
                        control.setDefaultName(controlName(this.controlType));
                        return control;
                    }
                }
            }
        },
        dragStart: function (xStart, yStart) {
        }
    };
    $.fn.fToolsBox = function () {
        var item = new fToolsBox(this);
        $(this).data('fToolsBox', item);
        return item;
    }
})(jQuery);
