﻿/// <reference path="Common.js" />
(function ($) {
    var cell, otherCell, status, contextMenu, focused, xStart, yStart, tableSelectType, tableWidth, tableHeight, tableLeft, tableTop, $r = $.report;
    function checkForAddRemove(element) {
        var flag = true;
        $(element).find('td').each(function () {
            if ($(this).attr('rowspan') > 1 || $(this).attr('colspan') > 1) {
                flag = false;
                return false;
            }
        });
        if (!flag)
            alert('بعد از "ادغام" سلول های جدول امکان حذف و اضافه کردن سطر یا ستون وجود ندارد');
        return flag;
    }
    function centerAlign(element) {
        let thWidth = $(element).find('th').first().width();
        let $leftColumn = $(element).closest('.column.right');
        if ($leftColumn.hasClass('column')) {
            let left = Math.round($leftColumn.position().left + ($leftColumn.width() + thWidth - $(element).width() + 1) / 2);
            $(element).css('left', left);
        } else {
            let $bond = $(element).closest('.bond');
            let left = Math.round($bond.position().left + ($bond.width() + thWidth - $(element).width() + 1) / 2);
            $(element).css('left', left);
        }
    }

    function getBorder() {
        var border = ':1px solid #000;';
        return 'border-left' + border + 'border-right' + border + 'border-top' + border + 'border-bottom' + border;
    }
    function rowIsSelected(tbl){
        var $tbl = $(tbl), flag = true;
        //تمامی سلولها انتخابی باید در یک ردیف باشند
        var rowIndex = $tbl.find('tr').index($tbl.find('.cellselected').first().closest('tr'));
        $tbl.find('.cellselected').each(function () {
            if (rowIndex != $tbl.find('tr').index($(this).closest('tr')))
                flag = false;
        });
        //تمامی سلولهای یک ردیف باید انتخاب شده باشند
        $tbl.find('tr').eq(rowIndex).find('td').each(function () {
            if (!$(this).hasClass('rowHeader') && !$(this).hasClass('cellselected'))
                flag = false;
        });
        return flag;
    }
    function colIsSelected(tbl) {
        var $tbl = $(tbl), flag = true;
        var $td = $tbl.find('.cellselected').first();
        var colIndex = $td.closest('tr').find('td').index($td);
        ///تمامی سلولهای انتخابی باید در یک ستون باشند
        $tbl.find('.cellselected').each(function () {
            if ($(this).closest('tr').find('td').index($(this)) != colIndex)
                flag = false;
        });
        ///تمامی سلولهای یک ستون باید انتخاب شده باشند.
        $tbl.find('tbody tr').each(function () {
            if (!$(this).find('td').eq(colIndex).hasClass('cellselected')) 
                flag = false;
        });
        return flag;
    }
    function addCell(cell, tr) {
        var $tr = $(tr);
        $tr.append('<td></td>');
        var $td = $tr.find('td').last();
        if (cell.member)
            $td.attr('member', decodeURIComponent(cell.member));
        if (cell.format)
            $td.data('format', cell.format);
        if (cell.colSpan > 1)
            $td.attr('colspan', cell.colSpan);
        if (cell.rowSpan > 1)
            $td.attr('rowspan',cell.rowSpan);
        if (cell.joinHeight > 1)
            $td.attr('rowspan', cell.joinHeight);
        var border = new rBorder();
        $.myExtend(border, cell.border);
        border.initElement($td);
        if (!cell.enable)
            $td.css('display', 'none');
        $td.css('color', cell.color.colorString);
        var align = new rAlign();
        $.myExtend(align, cell);
        align.initElement($td, report.controlKind.table);
        var font = new rFont();
        $.myExtend(font, cell.font);
        font.initElement($td);
        //if (cell.CellType = cellKind.rowHeader) {
        //    $td.height($r.getPixelHeight(cell.position.height));
        //}
        $td.css('background-color', cell.backGroundColor.colorString);
        if (cell.text)
            $td.text(decodeURIComponent(cell.text));
    }
    function getCellData(td, value) {
        var cell = new Object(), $item = $(td);
        if ($item[0].tagName == 'TH') 
            cell.cellType = cellKind.columnHeader;
        else
            if ($item.hasClass('rowHeader'))
                cell.CellType = cellKind.rowHeader;
            else
                cell.CellType = cellKind.normal;
        var format = $item.data('format');
        if (format)
            cell.format = format;
        if (cell.CellType == cellKind.normal) {
            cell.colSpan = parseInt($item.attr('colspan') || '0');
            cell.rowSpan = parseInt($item.attr('rowspan') || '0');
        }
        if (!cell.rowSpan)
            cell.rowSpan = 1;
        if (!cell.colSpan)
            cell.colSpan = 1;
        cell.position = new Object();
        cell.position.left = $r.getWidth($item.offset().left - value);
        cell.position.top = $r.getHeight($item.offset().top - tableTop);
        cell.position.width = $r.getWidth($item.outerWidth());
        cell.position.height = $r.getHeight($item.outerHeight());
        if (cell.CellType == cellKind.normal) {
            $.myExtend(cell, new rAlign().getAlign($item[0], report.controlKind.table));
            if ($item.attr('member'))
                cell.member = encodeURIComponent($item.attr('member'));
            if ($item.text())
                cell.text = encodeURIComponent($item.text());
            cell.backGroundColor = new Object();
            if ($item.css('background-color') == 'rgb(198, 217, 242)')
                cell.backGroundColor.colorString = 'transparent';
            else
                cell.backGroundColor.colorString = $item.css('background-color');
            ///Border
            var border = new Object();
            $.myExtend(border, new rBorder().getBorder($item[0]));
            cell.border = border;
            ///Font&Color
            cell.color = new Object();
            cell.color.colorString = $item.css('color');
            var font = new Object();
            $.myExtend(font, new rFont().getFont($item[0]));
            cell.font = font;
            ///----------------------------
            cell.enable = $item.css('display') != "none";
        }
        return cell;
    }
    var rTable = function (element) {
        this.element = element;
        this.controlType = report.controlKind.table;
        status = statusType.move;
        tableSelectType = tableSelectKind.none;
        this.report = $(document).data('rReport');
        tableLeft = -$(element).width();
        tableTop = 0, tbl = this;
        $(element).find('td').die("mousedown");
        $(element).find('td').live("mousedown", function (e) {
            if (e.button == 0) {
                if ($(this).hasClass('rowHeader')) {
                    if (tbl.focused) {
                        $(element).find('td').removeClass('cellselected');
                        $(this).parent().find('td[class!="cellselected"]').addClass('cellselected');
                    }
                } else {
                    if (!e.ctrlKey) {
                        $(element).find('td').removeClass('cellselected');
                        $(this).addClass('cellselected');
                    }
                    else {
                        if ($(this).is('.cellselected'))
                            $(this).removeClass('cellselected');
                        else
                            $(this).addClass('cellselected');
                    }
                }
            }
            $(document).data('rToolsBar').update();
            $('#bond').data('rBond').updateBondHeight(tbl.element);
        });
        $(element).find('td').live('dblclick', function (e) {
            var data = {};
            data.isSubReport = $('body').data('rPage').isSubReport;
            data.DataLevel = $(this).closest('.bond').attr('DataLevel');
            if (data.DataLevel) {
                data.bondType = 4;
                data.DataLevel = parseInt(data.DataLevel);
            }
            
            data.ReportId = $('#ReportId').val();
            if (e.ctrlKey && data.bondType == 4) {
                $.report.dotNetObjectReference.invokeMethodAsync('ShowFormulaWindow', data);
            }
            else {
                var id = $(element).closest('.bond').attr('id');
                switch (id) {
                    case 'dataHeader':
                        data.bondType = 3;
                        break;
                    case 'dataFooter':
                        data.bondType = 5;
                        break;
                }
                data.titleFa = $(this).text();
                data.titleEn = data.bondType == 4 ? $(this).attr('member') : null;
                $.report.dotNetObjectReference.invokeMethodAsync('ShowWindow', data);
            }
            
            return false;
        });
    };
    rTable.prototype = {
        text: function (text) {
            var $element = $(this.element).find('.cellselected').first();
            if (arguments.length == 0)
                return $element.text();
            $element.text(text);
        },
        format: function (value) {
            var $elements = $(this.element).find('.cellselected');
            if ($elements.length > 1)
                alert('لطفا فقط یک ستون را انتخاب نمایید.');
            else {
                if (arguments.length == 0)
                    return $elements.first().data('format');
                else
                    $elements.first().data('format', value);
            }
        },
        member: function(member){
            var $element = $(this.element).find('.cellselected').first();
            if (arguments.length == 0)
                return $element.attr('member');
            $element.attr('member', member);
        },
        getElement: function () {
            return $(this.element).find('.cellselected');
        },
        font: function(font){
            var $elemenets = this.getElement();
            if (arguments.length == 0) {
                var font = new rFont().getFont($elemenets.first()[0]);
                $elemenets.each(function () {
                    var font1 = new rFont().getFont(this);
                    if (font.bold != font1.bold)
                        font.bold = false;
                    if (font.family != font1.family)
                        font.family = null;
                    if (font.italic != font1.italic)
                        font.italic = false;
                    if (font.size != font1.size)
                        font.size = null;
                    if (font.underLine != font1.underLine)
                        font.underLine = false;
                });
                return font;
            }
            $elemenets.each(function () {
                font.initElement(this);
            });
            $('#bond').data('rBond').updateBondHeight(this.element);
        },
        border: function (border) {
            var $elements = this.getElement();
            if (arguments.length == 0) {
                var border = new rBorder().getBorder($elements.first()[0]);
                $elements.each(function () {
                    var border1 = new rBorder($(this)).getBorder(this);
                    if (border.color != border1.color || border.style != border1.style ||
                        border.width != border1.width) {
                        border = null;
                        return false
                    }
                    if (border != null)
                        border.borderKind &= border1.borderKind;
                });
                return border;
            }
            $elements.each(function () {
                border.initElement(this);
            });
        },
        getStatus: function(){

        },
        align: function (align) {
            var $elements = this.getElement();
            if (arguments.length == 0) {
                var align = new rAlign().getAlign($elements.first()[0], report.controlKind.table);
                $elements.each(function () {
                    var align1 = new rAlign().getAlign(this, report.controlKind.table);
                    if (align.horizontalAlign != align1.horizontalAlign)
                        align.horizontalAlign = null;
                    if (align.verticalAlign != align1.verticalAlign)
                        align.verticalAlign = null;
                });
                return align;
            }
            $elements.each(function () {
                align.initElement(this, report.controlKind.table);
            });
        },
        border: function () {
            var border = new Object();
            border.borderKind = 15;
            return border;
        },
        rowSelect: function (y) {
            if (status == statusType.rowSelected) {
                tableSelectType = tableSelectKind.rowSelect;
                $(this.element).find('td').removeClass('cellselected');
                $(this.element).find('td').each(function () {
                    if (!$(this).hasClass('rowHeader') && (!$(this).attr('rowspan') || $(this).attr('rowspan') == 1)) {

                        if ($(this).offset().top < y && $(this).offset().top + $(this).height() > y)
                            $(this).addClass('cellselected');
                    }
                });
            }
        },
        colSelect: function (x) {
            
            if (status == statusType.colSelected) {
                tableSelectType = tableSelectKind.collSelect;
                $(this.element).find('td').removeClass('cellselected');
                $(this.element).find('td').each(function () {
                    if (!$(this).attr('colspan') || $(this).attr('colspan') == 1) {
                        if ($(this).offset().left < x && $(this).offset().left + $(this).width() > x)
                            $(this).addClass('cellselected');
                    }
                });
            }
        },
        addColumnOnRight: function () {
            if (checkForAddRemove(this.element)) {
                var $selectedCell = $(this.element).find('.cellselected').first();
                var colIndex = $selectedCell.closest('tr').find('td').index($selectedCell);
                var $cell = (this.element).find('thead th').eq(colIndex);
                var width = $cell.width();
                $cell.width(parseInt(width / 2));
                $cell.before('<th style="width:' + (width - $cell.width()) + 'px"></th>');
                $(this.element).find('tbody tr').each(function () {
                    $(this).find('td').eq(colIndex).before('<td style="border-style:solid;border-color:#000;border-left-width:1px;border-right-width:1px;border-top-width:1px;border-bottom-width:1px;"></td>');
                });
            }
        },
        addColumnOnLeft: function () {
            if (checkForAddRemove(this.element)) {
                var $selectedCell = $(this.element).find('.cellselected').first();
                var colIndex = $selectedCell.closest('tr').find('td').index($selectedCell);
                var $cell = (this.element).find('thead th').eq(colIndex);
                var width = $cell.width();
                $cell.width(parseInt(width / 2));
                $cell.after('<th style="width:' + (width - $cell.width()) + 'px"></th>');
                $(this.element).find('tbody tr').each(function () {
                    $(this).find('td').eq(colIndex).after('<td style="border-style:solid;border-color:#000;border-left-width:1px;border-right-width:1px;border-top-width:1px;border-bottom-width:1px;"></td>');
                });
            }
        },
        addRowOnTop: function () {
            if (checkForAddRemove(this.element)) {
                var selectedRow = $(this.element).find('.cellselected').first().parent();
                var str = '<tr>'
                for (var i = 0; i < selectedRow.children().length; i++) {
                    if (i == 0)
                        str += '<td style="height:20px;" class="rowHeader"></td>';
                    else
                        str += '<td style="' + getBorder() + '"></td>';
                }
                str += '</tr>'
                selectedRow.before(str);
                $('#bond').data('rBond').updateBondHeight(this.element);
            }
        },
        addRowOnBottom: function(){
            if (checkForAddRemove(this.element)) {
                var selectedRow = $(this.element).find('.cellselected').first().parent();
                var str = '<tr>'
                for (var i = 0; i < selectedRow.children().length; i++) {
                    if (i == 0)
                        str += '<td style="height:20px;" class="rowHeader"></td>';
                    else
                        str += '<td style="' + getBorder() + '"></td>';
                }
                str += '</tr>';
                selectedRow.after(str);
                $('#bond').data('rBond').updateBondHeight(this.element);
            }
        },
        removeColumn: function(){
            if (checkForAddRemove(this.element)) {
                var columnsCount = $(this.element).find('th').length;
                if (columnsCount <= 2)
                    alert("جدول باید حداقل یک ستون داشته باشد."); else {
                        var $selectedCell = $(this.element).find('.cellselected').first();
                        var colIndex = $selectedCell.closest('tr').find('td').index($selectedCell);
                        var $cell = $(this.element).find('th').eq(colIndex);
                        var width = $cell.width();
                        if (colIndex == columnsCount - 1) {
                            var $preCell = $cell.prev();
                            $preCell.width(width + $preCell.width());
                        } else {
                            var $nextCell = $cell.next();
                            $nextCell.width(width + $nextCell.width());
                        }
                        $cell.remove();
                        $(this.element).find('.cellselected').remove();
                    }
            }
        },
        removeRow: function(){
            if (checkForAddRemove(this.element)) {
                var rowCount = $(this.element).find('tbody tr').length;
                if (rowCount <= 1)
                    alert("جدول باید حداقل یک ردیف داشته باشد.");
                else
                    $(this.element).find('td[class="cellselected"]').parent().remove();
            }
        },
        focus: function () {
            if ($(this.element).find('.cellselected').length == 0) {
                $(this.element).find('td').each(function () {
                    if (!$(this).hasClass('rowHeader')) {
                        $(this).addClass('cellselected');
                        return false;
                    }
                });
            }
        },
        blur: function () {
            this.focused = false;
            $(this.element).find('td').removeClass('cellselected');
        },
        dragStart: function (x, y) {
            $element = $(this.element);
            tableWidth = $element.width();
            tableHeight = $element.height();
            tableLeft = $element.offset().left;
            tableTop = $element.offset().top;
            cell = null, otherCell = null;
            $element.find('th').each(function () {
                if (Math.abs($(this).offset().left - xStart) <= 5) {
                    cell = new Object();
                    cell.$cell = $(this);
                    cell.left = $(this).offset().left;
                    cell.width = $(this).width();
                }
                if (Math.abs($(this).offset().left + $(this).outerWidth() - xStart) <= 5) {
                    otherCell = new Object();
                    otherCell.$cell = $(this);
                    otherCell.left = $(this).offset().left;
                    otherCell.width = $(this).width();
                }
            });
            $element.find('.rowHeader').each(function () {
                if (Math.abs($(this).offset().top + $(this).height() - yStart) <= 5) {
                    cell = new Object();
                    cell.$cell = $(this);
                    cell.top = $(this).offset().top;
                    cell.height = $(this).height();
                }
            });
            this.colSelect(x);
            this.rowSelect(y);
        },
        updateCursor: function (x, y) {
            status = statusType.none;
            xStart = x; yStart = y;
            $element = $(this.element);
            //----------------------------------------
            var $tHead = $element.find('thead th').first();
            var top = $tHead.offset().top, bottom = top + $tHead.height();
            var isFirst = true;
            $element.find('th').each(function () {
                if (y > top && y < bottom) {
                    if (Math.abs(x - $(this).offset().left) <= 5 || Math.abs(x - $(this).offset().left - $(this).width()) <= 5) {
                        status = statusType.changeCellWidth;
                        return false;
                    }
                    else {
                        var tLeft = $element.offset().left;
                        if (x > tLeft && x < tLeft + $element.width())
                            status = statusType.colSelected;
                    }
                }
                isFirst = false;
            });
            //---------------------------------------------
            if (status == statusType.none) {
                var $rowHeader = $element.find('th').first();
                var left = $rowHeader.offset().left, right = left + $rowHeader.width();
                top = $element.find('.rowHeader').first().offset().top, bottom = $element.offset().top + $element.width();
                $element.find('.rowHeader').each(function () {
                    if (x > left && x < right) {
                        if (Math.abs($(this).offset().top + $(this).height() - yStart) <= 3) {
                            status = statusType.changeCellHeight;
                            return false;
                        } else {
                            if (y > top && y < bottom)
                                status = statusType.rowSelected;
                        }
                    }
                });
            }
            contextMenu = 0;
            switch (status) {
                case statusType.none:
                    
                    $('#page').css('cursor', 'default');
                    $('.bottomarrow').css('display', 'none');
                    $('.leftArrow').css('display', 'none');
                    break;
                case statusType.changeCellWidth:
                    $('#page').css('cursor', 'col-resize');
                    $('.bottomarrow').css('display', 'none');
                    break;
                case statusType.changeCellHeight:
                    $('#page').css('cursor', 'row-resize');
                    $('.leftArrow').css('display', 'none');
                    $('.bottomarrow').css('display', 'none');
                    break;
                case statusType.colSelected:
                    contextMenu = 1;
                    $('#page').css('cursor', 'none');
                    $('.bottomarrow').css('left', x);
                    $('.bottomarrow').css('display', '');
                    $('.bottomarrow').css('top', y - 10);
                    break;
                case statusType.rowSelected:
                    contextMenu = 2;
                    $('#page').css('cursor', 'none');
                    $('.leftArrow').css('left', x);
                    $('.leftArrow').css('display', '');
                    $('.leftArrow').css('top', y - 10);
                    break;
            }
            

            return status;
        },
        drag: function (difX, difY) {
            var $element = $(this.element);
            var otherTable = $('#bond').find('.tablecontrol').filter(function () {
                if ($(this).attr('id') != $element.attr('id'))
                    return this;
            });
            switch (status)
            {
                case statusType.changeCellWidth:
                    var item1, item2;
                    if (cell != null) {
                        var factor = 1;
                        if (otherCell == null)
                            factor = 2;
                        var width = cell.width - factor * difX, otherWidth = null;
                        if (otherCell)
                            otherWidth = otherCell.width + difX;
                        otherTable.find('th').each(function () {
                            if (Math.abs(xStart + difX - $(this).offset().left) < 6) {
                                item1 = this;
                                item2 = cell.$cell[0];
                                width = cell.width - factor * ($(this).offset().left - cell.left);
                                if (otherCell)
                                    otherWidth = otherCell.width + $(this).offset().left - cell.left;
                            }
                        });
                        cell.$cell.width(width);
                        //cell.$cell.css('max-width', width);
                        if (otherCell != null) {
                            otherWidth = otherCell.width - (cell.$cell.width() - cell.width);
                            otherCell.$cell.width(otherWidth);
                            //otherCell.$cell.css('max-width', otherWidth);
                        }
                    }
                    if (item1) {
                        $r.showLeftRuler(item1, item2);
                        if (factor == 2)
                            $r.showRightRuler(otherTable, item2);
                    }
                    else
                        $r.hideRuler();
                    centerAlign(this.element);
                    break;
                case statusType.changeCellHeight:
                    if (cell) {
                        if (cell.$cell.attr('rowspan') == 1 || !cell.$cell.attr('rowspan')) 
                            cell.$cell.height(cell.height + difY);
                    }
                    centerAlign(this.element);
                    break;
                case statusType.move:
                    $(this.element).css('left', tableLeft + difX);
                    $(this.element).css('top', tableTop + difY);
                    break;
            }
            
        },
        getContextMenu: function () {
            if (contextMenu == 2 && rowIsSelected(this.element))
                return 2;
            if (contextMenu == 1 && colIsSelected(this.element))
                return 1;
            return 0;
        },
        resetContextMenu: function(){
            contextMenu = 0;
        },
        merge: function(){
            var cells = [];
            $(this.element).find('.cellselected').each(function () {
                var cell = new Object();
                cell.top = $(this).offset().top;
                cell.left = $(this).offset().left;
                cell.width = $(this).width();
                cell.outerWidth = $(this).outerWidth();
                cell.height = $(this).height();
                cells.push(cell);
            });
            var isSame = true;
            for (var i = 0; i < cells.length - 1; i++) {
                var cell1 = cells[i];
                var cell2 = cells[i + 1];
                if (cell1.top != cell2.top || cell1.height != cell2.height) {
                    isSame = false;
                    break;
                }
            }
            if (isSame == true) {
                var colSpan = 0;
                $(this.element).find('.cellselected').each(function () {
                    var span = parseInt($(this).attr('colspan'));
                    if (!span)
                        span = 1;
                    if (colSpan > 0) {
                        $(this).css('display', 'none');
                        $(this).removeClass('cellselected');
                    }
                    colSpan += span;
                });
                $(this.element).find('.cellselected').attr('colspan', colSpan);
            }
            else {
                var isSame = true;
                for (var i = 0; i < cells.length - 1; i++) {
                    var cell1 = cells[i];
                    var cell2 = cells[i + 1];
                    if (cell1.left != cell2.left || cell1.width != cell2.width) {
                        isSame = false;
                        break;
                    }
                }
                if (isSame == true) {
                    var rowSpan = 0;
                    $(this.element).find('.cellselected').each(function () {
                        var span = $(this).attr('rowspan');
                        if (!span)
                            span = 1;
                        if (rowSpan > 0)
                            $(this).css('display', 'none');
                        rowSpan += parseInt(span);
                    });
                    $(this.element).find('.cellselected').first().attr('rowspan', rowSpan);
                }
            }
        },
        unmerge: function(){
            $(this.element).find('.cellselected').each(function () {
                var colSpan = $(this).attr('colspan');
                $(this).attr('colspan', 1);
                var $cell = $(this);
                for (var index = 1; index < colSpan; index++) {
                    $cell.attr('colspan', 1);
                    $cell = $cell.next();
                    $cell.css('display', '');
                }
                var rowSpan = $(this).attr('rowspan');
                var colIndex = $(this).parent().find('td').index($(this));
                var $row = $(this).closest('tr');
                $(this).attr('rowspan', 1);
                for (var index = 1; index < rowSpan; index++) {
                    $row = $row.next();
                    $cell = $row.find('td').eq(colIndex);
                    $cell.attr('rowspan', 1);
                    $cell.css('display', '');
                }
            });
        },
        isMergeable: function () {
            return this.getElement().length > 1;
        },
        isUnmergeable: function(){
            var $elements = this.getElement();
            if ($elements.length > 1)
                return false;
            var $td = this.getElement().first(), rowSpan = $td.attr('rowspan');
            if (!rowSpan)
                rowSpan = 1;
            rowSpan = parseInt(rowSpan);
            if (rowSpan > 1)
                return true;
            var colSpan = $td.attr('colspan');
            if (!colSpan)
                colSpan = 1;
            colSpan = parseInt(colSpan);
            return colSpan > 1;
        },
        drop: function () {
            var bond = $('#bond').data('rBond');
            bond.addControlToBond(this.element);
            bond.updateBondHeight(this.element);
            $r.hideRuler();
            centerAlign(this.element);
        },
        backGroundColor: function(color){
            var $elements = this.getElement();
            if (arguments.length == 0) {
                var color = $elements.first().css('background-color');
                $elements.each(function () {
                    if (color != $(this).css('background-color'))
                        color = null;
                });
                return color
            }
            if (color == '#0000ffff')
                color = 'Transparent';
            $elements.css('background-color', color);
        },
        init: function (data) {
            $(this.element).append("<thead><tr></tr></thead><tbody></tbody>");
            for (var col = 0; col < data.columnsCount; col++) {
                var cell = data.cells[col];
                var $headerRow = $(this.element).find('thead tr').first();
                $headerRow.append('<th></th>');
                $headerRow.find('th').last().width($r.getPixelWidth(cell.position.width) - 2)
            }
            var index = data.columnsCount, $tbody = $(this.element).find('tbody');
            for (var row = 1; row <= data.rowsCount; row++) {
                $tbody.append('<tr></tr>');
                var $tr = $tbody.find('tr').last();
                for (var col = 0; col < data.columnsCount; col++) {
                    var cell = data.cells[index];
                    if (col == 0) {
                        $tr.append('<td></td>');
                        var $td = $tr.find('td').first();
                        $td.addClass('rowHeader').height($r.getPixelWidth(cell.position.height - 1));
                    } else 
                        addCell(cell, $tr[0]);
                    index++;
                }
            }
            $(this.element).closest('.bond').height($(this.element).height());
            status = statusType.none;
            centerAlign(this.element);
        },
        getData: function () {
            var table = new Object();
            table.cells = [];
            var $bond = $(this.element).closest('.bond');
            var tableLeft = $bond.offset().left;
            var $firstColumn = $bond.find('.column.right');
            if ($firstColumn.get(0) != null)
                tableLeft = $firstColumn.offset().left;
            tableTop = $(this.element).offset().top ;
            $(this.element).find('tr').each(function () {
                $(this).find('th').each(function () {
                    var cell = getCellData($(this)[0], tableLeft);
                    table.cells.push(cell);
                });
                $(this).find('td').each(function () {
                    $(this).removeClass('cellselected');
                    var cell = getCellData($(this)[0], tableLeft);
                    table.cells.push(cell);
                });
            });
            table.columnsCount = $(this.element).find('tr:first').find('th').length;
            table.rowsCount = $(this.element).find('td[class="rowHeader"]').length;
            table.dataLevel = $(this.element).closest('.bond').attr('DataLevel');
            if (table.dataLevel)
                table.dataLevel = parseInt(table.dataLevel);
            console.log(table);
            return table;
        }
    };

    $.fn.rTable = function () {
        var table = new rTable(this);
        $(this).data('rTable', table);
        return table;
    }
    var tableSelectKind = {
        none:0,
        rowSelect: 1,
        collSelect: 2
    }
    var statusType = {
        none: 0,
        changeCellWidth: 1,
        changeCellHeight: 2,
        rowSelected: 3,
        colSelected: 4,
        move: 5
    }

})(jQuery);
