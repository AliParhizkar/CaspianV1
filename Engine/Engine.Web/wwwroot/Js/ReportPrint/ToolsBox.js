/// <reference path="Common.js" />
(function ($) {
    var $r = $.report, addedToForm, creatingControlId;

    function getTable(id) {
        var str = '<table cellpadding="0" cellspacing="0" id="' + id + '" class="reportcontrol tablecontrol" style="position:absolute;">';
        str += '<thead>';
        for (var col = 0; col < 4; col++) {
            str += '<th style="width:';
            if (col == 0)
                str += '10';
            else
                str += '100';
            str += 'px"></th>';
        }
        str += '</thead>';
        for (var row = 0; row < 1; row++) {
            str += '<tr>';
            str += '<td class="rowHeader" style="height:23px;" ></td>';
            for (var col = 0; col < 3; col++) {
                str += '<td style="border-style:solid;border-color:#000;border-left-width:1px;border-right-width:1px;border-top-width:1px;border-bottom-width:1px;"></td>';
            }
            str += '</tr>'
        }
        str += '</table>';
        return str;
    }
    function getCheckBox(id) {

    }
    function getChart(id) {
        alert("این کنترل در نسخه بعدی اضافه خواهد شد.");
        return null;
    }
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
            case report.controlKind.textBox:
                return $(element).rTextBox();
            case report.controlKind.checkBox:
                return $(element).rCheckBox();
                //case report.controlKind.chart:
                //    return $(element).ch;
            case report.controlKind.pictureBox:
                return $(element).rPictureBox();
            case report.controlKind.subReport:
                return $(element).rSubReport();
            case report.controlKind.table:
                return $(element).rTable();
        }
    }
    function appentControl(controlType, bond) {
        creatingControlId = getId();
        var str = null;
        switch (controlType) {
            case report.controlKind.textBox:
                str  = $r.getTextBox(creatingControlId, 180, 35);
                break;
            case report.controlKind.pictureBox:
                str = $r.getPictureBox(creatingControlId, 80, 80);
                break;
            case report.controlKind.subReport:
                str = $r.getSubReport(creatingControlId, 350, 150);
                break;
            case report.controlKind.table:
                str = getTable(creatingControlId);
                break;
            case report.controlKind.checkBox:
                str = getCheckBox(creatingControlId);
                break;
            case report.controlKind.chart:
                str = $r.getChart(creatingControlId, 200, 100);
                break;
        }
        if (str != null) {
            if (controlType == report.controlKind.table && $(bond).find('.column').hasClass('column'))
                $(bond).find('.column').first().append(str);
            else
                $(bond).append(str);
            addedToForm = true;
        }
        else
            creatingControlId = null;
    }
    var rToolsBox = function (element) {
        this.element = element;
        var obj = this;
        this.controlType = report.controlKind.none;
        
        $(this.element).mousedown(function (e) {
            addedToForm = false;
            $item = $(e.target);
            if ($item.hasClass('textbox'))
                obj.controlType = report.controlKind.textBox;
            if ($item.hasClass('picture'))
                obj.controlType = report.controlKind.pictureBox;
            if ($item.hasClass('subreport'))
                obj.controlType = report.controlKind.subReport;
            if ($item.hasClass('table'))
                obj.controlType = report.controlKind.table;
            if ($item.hasClass('checkbox'))
                obj.controlType = report.controlKind.checkBox;
            if ($item.hasClass('chart'))
                obj.controlType = report.controlKind.chart;
        });
    };

    rToolsBox.prototype = {
        createAndDrag: function (x, y) {
            if (this.controlType != report.controlKind.none) {
                if (addedToForm) {
                    var control = $('#' + creatingControlId).data('rControl');
                    if (control)
                        control.drag(x, y);
                    this.controlType = report.controlKind.none;
                }
                else {
                    var bond = getCurentBond(x, y);
                    $('body').data('rPage').resetCurentControl();
                    if (bond) {
                        appentControl(this.controlType, bond);
                        switch (this.controlType) {
                            case report.controlKind.table:
                                return $('#' + creatingControlId).rTable();
                            case report.controlKind.textBox:
                                return $('#' + creatingControlId).rTextBox();
                            case report.controlKind.pictureBox:
                                return $('#' + creatingControlId).rPictureBox();
                            case report.controlKind.subReport:
                                return $('#' + creatingControlId).rSubReport();
                            case report.controlKind.chart:
                                return $('#' + creatingControlId).rChart();
                        }
                    }
                }
            }
        },
        dragStart: function (xStart, yStart) {
        }
    };
    $.fn.rToolsBox = function () {
        var item = new rToolsBox(this);
        $(this).data('rToolsBox', item);
        return item;
    }
})(jQuery);
