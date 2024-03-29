﻿/// <reference path="Common.js" />
(function ($) {
    let $r = $.report, $bondForResize, controlls, startHeight, $element, bondsData, hasDataHeader, hasDataFooter, detailCount,
        onlyHeaderBond, bondWidth = 600, hasFocuse, columnsCount = 1, columnsMargin = 2, minDataLevel = 100;
    function getReportBond(id, title, height, bondType) {
        let str = ''; 
        if ($element.find('#' + id).attr('id') != id) {
            str = '<tr><td colspan="' + (detailCount + 1) + '" class="reportheader">' + title + '</td><td id="' + id + '" BondType="' + bondType + '" style="height:' + height +
                'px;width:' + bondWidth + 'px" class="bond"></td></tr><tr><td colspan="' + (detailCount + 1) + '" class="spliter-first" ><hr/></td><td class="spliter"></td></tr>';
        }
        return str;
    }
    function addColumn(item) {
        str = '';
        width = parseInt((bondWidth - (columnsCount - 1) * columnsMargin) / columnsCount);
        for (let index = 0; index < columnsCount; index++) {
            str += '<div class="column '
            if (index == 0)
                str += 'left';
            else if (index + 1 == columnsCount)
                    str += 'right';
                else
                str += 'middel';
            str += '" style="width:' + (width - columnsMargin / 2) + 'px;'
            if (index + 1 != columnsCount && columnsMargin > 0)
                str += 'margin-right:' + columnsMargin + 'px;'
            str += '"></div>';
        }
        $(item).append(str);
    }
    function getMasterRow() {
        let i = 2;
        if (hasDataHeader)
            i += 2;
        if (hasDataFooter)
            i += 2;
        return i;
    }
    function getDataBond(dataBond) {
        let height = 1.5;
        if (dataBond)
            height = dataBond.height;
        height = $r.getPixelHeight(height);
        let str = '';
        if (detailCount == 0)
            str += '<tr style="height:20px">';
        let className = "dataBond data-bond-second" + (detailCount == 2 ? '-2' : ''); 
        if (hasDataHeader) {
            let headerHeight = 50,
              headers  = $.grep(bondsData, function (item) {
                if (item.bondType == 3)
                    return item;
              });
            if (headers.length > 0)
                headerHeight = $r.getPixelHeight(headers[0].height);
            for (let i = 0; i < detailCount; i++) {
                str += '<td class="dataBond data-bond-first">&emsp;</td>';
            }
            
            str += '<td class="' + className + '">Data head</td><td id="dataHeader" style="height:' + headerHeight + 'px" BondType = "3" class="bond"></td></tr><tr style="height:3px"><td colspan="' + (detailCount + 1) + '" class="dataBond">'
            str += '<hr style="width:130px;margin:1px auto 0px auto" /></td><td class="spliter"></td></tr><tr style="height:20px">';
        }
        for (let i = 0; i < detailCount; i++) {
            str += '<td class="dataBond data-bond-first">&emsp;</td>';
        }
        str += '<td class="' + className + '">Data</td><td style="height:' + height + 'px;width:' + bondWidth + 'px" DataLevel="' + dataBond.dataLevel + '" class="bond"></td></tr>';
        if (!hasDataFooter)
            str += '<tr style="height:3px"><td class="spliter-first" colspan="' + (detailCount + 1) + '"><hr/></td><td class="spliter"></td></tr>';
        if (hasDataFooter) {
            let footerHeight = $r.getPixelHeight($.grep(bondsData, function (item) {
                if (item.bondType == 5)
                    return item;
            })[0].height);
            str += '<tr style="height:3px"><td colspan="' + (detailCount + 1) + '" class="dataBond"><hr style="width:130px;margin:1px auto 0px auto"/></td><td class="spliter"></td></tr>';
            str += '<tr style="height:20px">';
            for (let i = 0; i < detailCount; i++) {
                str += '<td class="dataBond data-bond-first">&emsp;</td>';
            }
            str += '<td class="' + className + '">Dada footer</td><td id="dataFooter" style="height:' + footerHeight + 'px;width:' + bondWidth + 'px" BondType="5" class="bond"></td></tr><tr style="height:3px"><td colspan="' + (detailCount + 1) + '" class="spliter-first"><hr/></td><td class="spliter"></td></tr>';
        }
        return str;
    }
    function getDetailDataBond() {
        let bonds = $.grep(bondsData, function (item) {
            if (item.bondType == 4 && checkBondAdd(item.dataLevel))
                return item;
        });
        if (detailCount === 0)
            return getDataBond(bonds[0]);
        
        let str = '<tr style="height:20px"><td class="dataBond"';
        if (detailCount > 0)
            str += ' colspan="' + (detailCount + 1) + '"';
        str += '>Data</td><td style="height:' + $r.getPixelHeight(bonds[0].height) + 'px" DataLevel="' + bonds[0].dataLevel + '" class="bond"></td></tr><tr><td class="spliter-first" colspan="' + (detailCount + 1) + '" ><hr/></td><td class="spliter"></td></tr>';
        for (let i = 1; i <= detailCount; i++) {
            str += '<tr style="height:20px">';
            if (i != detailCount) {
                let height = bonds.length > i ? bonds[i].height : 1.5;
                str += '<td class="dataBond data-bond-first">&emsp;</td>';
                str += '<td colspan="2" class="dataBond data-bond-second">Data</td><td style="width:' + bondWidth + 'px;height:' + $r.getPixelHeight(height) + 'px" DataLevel="' + bonds[i].dataLevel +
                    '" class="bond"></td></tr><tr><td class="spliter-first" colspan="' + (detailCount + 1) + '"><hr/></td><td class="spliter"></td></tr>';
            }
            else
                str += getDataBond(bonds[i]);
        }
        return str;
    }
    function updateBondOnFocuse(bond) {
        if (hasFocuse) {
            let $square = $element.find('.squarebond');
            $square.css('display', 'inline-block');
            let left = $(bond).offset().left, top = $(bond).offset().top, width = $(bond).outerWidth(), height = $(bond).height();
            $square.eq(0).css('left', left - 3);
            $square.eq(0).css('top', top - 3);
            $square.eq(1).css('left', left + width - 3);
            $square.eq(1).css('top', top - 3);
            $square.eq(2).css('left', left - 3);
            $square.eq(2).css('top', top + height - 4);
            $square.eq(3).css('left', left + width - 3);
            $square.eq(3).css('top', top + height - 4);
            $element.find('.bond').removeAttr('selected');
            $(bond).attr('selected', true);
            $element.find('.spliter').removeAttr('selected');
            $(bond).parent().next().find('.spliter').attr('selected', true);
            $(bond).append($square);
            $(document).data('rToolsBar').update();
        }
    }
    function checkBondAdd(dataLevel) {
        let page = $('body').data('rPage');
        if (page.isSubReport) {
            if (parseInt(dataLevel) === 1)
                return true;
            if (parseInt(dataLevel) === 3)
                return false;
            return parseInt(page.report.subReportLevel) === 2;
        } else {
            if (!page.report || !page.report.subReportLevel || page.report.subReportLevel < dataLevel)
                return true;
            return false;
        }
    }

    let rBond = function (element, data) {
        bondsData = data;
        this.controlType = report.controlKind.bond;
        hasFocuse = true;
        this.element = element;
        //onlyHeaderBond = true;
        controlls = [];
        $element = $(element);
    };
    rBond.prototype = {
        width: function (width) {
            if (arguments.length > 0)
                bondWidth = width;
            else
                return $(this.element).find('.bond').first().width();
        },
        getBondProperty: function(){
            let obj = new Object(), $element = $(this.element);
            obj.ReportTitle = $element.find('#reportTitle').is('#reportTitle');
            obj.PageHeader = $element.find('#pageHeader').is('#pageHeader');
            obj.DataHeader = $element.find('#dataHeader').is('#dataHeader');
            obj.DataFooter = $element.find('#dataFooter').is('#dataFooter');
            obj.PageFooter = $element.find('#pageFooter').is('#pageFooter');
            return obj;
        },
        focus: function(){
            hasFocuse = true;
            updateBondOnFocuse($('.bond')[0]);
        },
        getStatus: function(){

        },
        backGroundColor: function (color) {
            $bond = $element.find('.bond[selected="selected"]');
            if (arguments.length == 0)
                return $bond.css('background-color');
            if (color == '#0000ffff')
                color = 'Transparent';
            $bond.css('background-color', color);
        },
        border: function (border) {
            let $bond = $element.find('.bond[selected="selected"]');
            if (arguments.length == 0)
                return new rBorder().getBorder($bond);
            else {
                border.initElement($bond);
            }
        },
        removeBond: function(data, bondType){
            let list = [];
            for (let i in data) {
                let index = parseInt(i), item = data[index];
                if (item.bondType != bondType)
                    list.push(item);
            }
            return list;
        },
        addBond: function(data, bond){
            let list = [];
            for (let i in data) {
                let index = parseInt(i), item = data[index];
                if (bond.bondType === 3 && item.bondType === 4) {
                    if (index + 1 < data.length && data[index + 1].bondType === 4)
                        list.push(item);
                } else {
                    if (item.bondType < bond.bondType)
                        list.push(item);
                }
            }
            list.push(bond);
            for (let i in data) {
                let index = parseInt(i), item = data[index];
                if (bond.bondType === 3 && item.bondType === 4) {
                    if (index + 1 === data.length || data[index + 1].bondType !== 4)
                        list.push(item);
                }
                else
                    if (item.bondType > bond.bondType)
                        list.push(item);
            }
            return list;
        },
        createBond: function (data) {
            if (arguments.length > 0)
                bondsData = data;
            $element.html('');
            hasDataHeader = $.grep(bondsData, function (bond) {
                return bond.bondType === 3;
            }).length > 0;
            hasDataFooter = $.grep(bondsData, function (bond) {
                return bond.bondType === 5;
            }).length > 0;
            let count = 0;
            for (let i = 0; i < bondsData.length; i++) {
                let bond = bondsData[i];
                if (bond.bondType === 4 && checkBondAdd(bond.dataLevel)) {
                    if (bond.columnsCount)
                        columnsCount = bond.columnsCount;
                    let dataLevel = bond.dataLevel;
                    if (dataLevel < minDataLevel)
                        minDataLevel = dataLevel;
                    count++;
                }
            }
            detailCount = count - 1;
            let str = "", height = 0;
            for (let i = 0; i < bondsData.length; i++) {
                let bond = bondsData[i];
                let height = $r.getPixelHeight(bond.height);
                if (parseInt(bond.bondType) === 1)
                    str += getReportBond('reportTitle', 'Report title', height, 1);
                if (parseInt(bond.bondType) === 2 && !onlyHeaderBond) {
                    this.printOn = bond.printOn;
                    str += getReportBond('pageHeader', 'Page head', height, 2);
                }
            }
            if (!onlyHeaderBond)
                str += getDetailDataBond();
            for (let i = 0; i < bondsData.length; i++) {
                let bond = bondsData[i];
                if (bond.bondType === 6)
                    str += getReportBond('pageFooter', 'Page footer', height, 6);
            }
            $element.append(str);
            let isSubReport = $('body').data('rPage').isSubReport;
            if (columnsCount > 1 && !isSubReport) {
                let $temp = $element.find('.bond[datalevel="' + minDataLevel + '"]');
                addColumn($temp.get(0));
                if (hasDataHeader)
                    addColumn($element.find('#dataHeader').get(0));
                if (hasDataFooter)
                    addColumn($element.find('#dataFooter').get(0));
            }
            $('.bond[datalevel],#dataHeader,#dataFooter').dblclick(function (e) {
                if (!$(e.target).is('.column,.bond'))
                    return;
                let index = $('#bond .bond').index($(this));
                $.report.dotNetObjectReference.invokeMethodAsync('ShowColumnWindow', bondsData[index], minDataLevel);
            });
            $element.find('.bond').first().append('<span class="squarebond"></span>');
            $element.find('.bond').first().append('<span class="squarebond"></span>');
            $element.find('.bond').first().append('<span class="squarebond"></span>');
            $element.find('.bond').first().append('<span class="squarebond"></span>');
            $element.find('.bond').click(function () {
                updateBondOnFocuse(this);
            });
            let id = 1;
            for (let i in bondsData) {
                let bond = bondsData[i];
                let $bond = $('.bond').eq(i);
                if ($bond.find('.column.right').hasClass('column right'))
                    $bond = $bond.find('.column.right');
                for (let j in bond.controls) {
                    let control = bond.controls[j], width = $r.getPixelWidth(control.position.width);
                    let height = $r.getPixelHeight(control.position.height); 
                    switch (control.type) {
                        case 3:
                            $bond.append($r.getTextBox("id" + id, width, height));
                            break;
                        case 4:
                            $bond.append($r.getPictureBox("id" + id, width, height));
                            break;
                        case 5:
                            $bond.append($r.getSubReport("id" + id, width, height));
                    }
                    let ctr = null;
                    switch (control.type) {
                        case 3:
                            ctr = $('#id' + id).rTextBox();
                            break;
                        case 4:
                            ctr = $('#id' + id).rPictureBox();
                            break;
                        case 5:
                            ctr = $('#id' + id).rSubReport();
                            break;
                    }
                    ctr.init(control);
                    id++;
                }
                let table = bond.table;
                if (table) {
                    $bond.append('<table cellpadding="0" cellspacing="0" id="id' + id + '" class="reportcontrol tablecontrol" style="position:absolute;">' +
                        '</table>');
                    let tbl = $('#id' + id).rTable();
                    tbl.init(table);
                    id++;
                }
            }
            hasFocuse = true;
        },
        updateCursor: function (x, y) {
            let flag = false;
            $element.find('.spliter[selected="selected"]').each(function () {
                let offset = $(this).offset(), top = offset.top - $(window).scrollTop();
                if (Math.abs(y - top) < 3 && x > offset.left && x < offset.left + $(this).width()) {
                    flag = true;
                    return false;
                }
            });
            if (flag)
                $('#page').css('cursor', 'row-resize');
            else
                $('#page').css('cursor', 'default');
        },
        blur: function(){
            hasFocuse = false;
            let $square = $element.find('.squarebond');
            $square.css('display', 'none');
        },
        dragStart: function (x, y) {
            let index = -1;
            $element.find('.spliter').each(function (i) {
                let top = $(this).offset().top - $(window).scrollTop()
                if (Math.abs(y - top) <= 2) {
                    $bondForResize = $(this).parent().prev();
                    index = i;
                    return false;
                }
            });
            let $square = $element.find('.squarebond');
            $square.css('display', 'none');
            if ($bondForResize) {
                startHeight = $bondForResize.height();
                controlls = [];
                $('#bond').find('.bond').each(function (i) {
                    if (i > index) {
                        $(this).find('.reportcontrol').each(function () {
                            let ctr = { top: $(this).offset().top, control: this };
                            controlls.push(ctr);
                        });
                    }
                });
            }
        },
        drop: function (e) {
            this.updateCursor(e.clientX, y = e.clientY);
        },
        updateBondHeight: function (element) {
            let $bond = $(element).closest('.bond');
            let index = $('#bond').find('.bond').index($bond);
            let difHeight =  $(element).offset().top + $(element).height() - $bond.offset().top - $bond.height();
            if ($(element).hasClass('tablecontrol'))
                $(element).css('top', $bond.offset().top);
            if (difHeight > 0) {
                $('#bond').find('.bond').each(function (i) {
                    if (i > index) {
                        $(this).find('.reportcontrol').each(function () {
                            $(this).css('top', $(this).offset().top + difHeight);
                        });
                    }
                });
                $bond.height($bond.height() + difHeight);
            }
            
        },
        addControlToBond: function(ctr){
            let $ctr = $(ctr), rightCtr = $ctr.offset().left + $ctr.width(), topCtr = $ctr.offset().top;
            $('#bond').find('.bond').each(function () {
                let left = $(this).offset().left, right = left + $(this).width(), top = $(this).offset().top,
                    bottom = top + $(this).height();
                if (rightCtr > left && rightCtr <= right && topCtr >= top && topCtr < bottom) {
                    if ($ctr.closest('.bond').attr('id') != $(this).attr('id')) {
                        $(this).append($ctr);
                    }
                    else
                        if ($ctr.closest('.bond').attr('datalevel') != $(this).attr('datalevel'))
                            $(this).append($ctr);
                    return false;
                }
            });
        },
        getControlType: function(){
            return report.controlKind.bond;
        },
        drag: function (difX, difY) {
            
            if ($bondForResize && $bondForResize.find('.bond').attr('selected')) {
                
                if (difY >= 0) {
                    if (startHeight + difY > 20)
                        $bondForResize.find('.bond').height(startHeight + difY);
                }
                else {
                    let maxBottom = 0;
                    $bondForResize.find('.reportcontrol').each(function () {
                        let bottom = $(this).offset().top + $(this).height() - 1;
                        if (bottom > maxBottom)
                            maxBottom = bottom;
                    });
                    if (startHeight + difY > maxBottom - $bondForResize.offset().top) {
                        $bondForResize.find('.bond').height(startHeight + difY);
                    }
                }
                for (let i = 0; i < controlls.length; i++) 
                    $(controlls[i].control).css('top', controlls[i].top + $bondForResize.find('.bond').height() - startHeight);
            }
        },
        destroyAfterResize: function () {
            $bondForResize = null;
        },
        setColumnProperty: function (colsCount, ColsMargin, newPageAfter, newPageBefore) {
            if (colsCount)
                columnsCount = colsCount;
            if (ColsMargin)
                ColsMargin = ColsMargin;
            let index = null;
            $('#bond .bond').each(function (i) {
                if ($(this).attr('selected') === 'selected')
                    index = i;
            });
            if (index) {
                bondsData[index].newPageAfter = newPageAfter;
                bondsData[index].newPageBefore = newPageBefore;
            }
            let data = this.getData();
            this.createBond(data);
        },
        getData: function () {
            let bonds = [];
            let self = this;
            $(this.element).find('.bond').each(function (index) {
                let bond = new Object();
                bond.newPageAfter = bondsData[index].newPageAfter;
                bond.newPageBefore = bondsData[index].newPageBefore;
                bond.backGroundColor = new Object();
                bond.backGroundColor.colorString = $(this).css('background-color');
                let border = new Object();
                $.myExtend(border, new rBorder().getBorder($(this)));
                bond.border = border;
                if ($(this).find('.tablecontrol').hasClass('tablecontrol')) 
                    bond.height = $r.getHeight($(this).find('.tablecontrol tbody').height());
                else
                    bond.height = $r.getHeight($(this).height());
                bond.bondType = parseInt($(this).attr('BondType'));
                if (bond.bondType == 2)
                    bond.printOn = self.printOn;
                if ($(this).attr('DataLevel'))
                    bond.dataLevel = parseInt($(this).attr('DataLevel'));
                if (bond.dataLevel)
                    bond.bondType = 4;
                if (columnsCount > 1 && minDataLevel == bond.dataLevel) {
                    bond.columnsCount = columnsCount;
                    bond.columnsMargin = columnsMargin;
                }
                bond.controls = [];
                let controlTypes = ['rPictureBox', 'rTextBox', 'rSubReport']
                $(this).find('.reportcontrol').each(function () {
                    let table = $(this).data('rTable');
                    if (table)
                        bond.table = table.getData();
                    else {
                        let ctr = null;
                        for (let i = 0; i < controlTypes.length; i++)
                            if ($(this).data(controlTypes[i])) {
                                ctr = $(this).data(controlTypes[i]);
                                break;
                            }
                        if (ctr)
                            bond.controls.push(ctr.getData());
                    }  
                });
                bonds.push(bond);
            });
            return bonds;
        }

    };
    $.fn.rBond = function (data) {
        let item = new rBond(this, data);
        $(this).data('rBond', item);
        return item;
    }
}
)(jQuery);
