/// <reference path="Common.js" />
(function ($) {
    var $f = $.form, $bondForResize, controlls, startHeight, startWidth, startLeft, $element,
        widthResizeable, bondData, bondWidth = 600, hasFocuse;
    function getReportBond(id, title, height, bondType) {
        var str = ''; 
        if ($element.find('#' + id).attr('id') != id) {
            str = '<tr><td id="' + id + '" BondType="' + bondType + '" style="height:' + height +
                'px;width:' + bondWidth + 'px" class="bond"></td></tr><tr><td class="spliter"></td></tr>';
        }
        return str;
    }
    function updateBondOnFocuse(bond) {
        if (hasFocuse) {
            var $square = $element.find('.squarebond');
            $square.css('display', 'inline-block');
            var left = $(bond).offset().left, top = $(bond).offset().top, width = $(bond).outerWidth(), height = $(bond).height();
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
            $(document).data('fToolsBar').update();
        }
    }

    var fBond = function (element, data) {
        bondData = data;
        this.controlType = controlKind.bond;
        hasFocuse = true;
        this.element = element;
        controlls = [];
        $element = $(element);
        widthResizeable = false;
    };
    fBond.prototype = {
        width: function (width) {
            if (arguments.length > 0)
                bondWidth = width;
            else
                return $(this.element).find('.bond').first().width();
        },
        focus: function(){
            hasFocuse = true;
            updateBondOnFocuse($('.bond')[0])
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
            var $bond = $element.find('.bond[selected="selected"]');
            if (arguments.length == 0)
                return new rBorder().getBorder($bond);
            else {
                border.initElement($bond);
            }
        },
        removeBond: function(data, bondType){
            var list = [];
            for (var i in data) {
                var index = parseInt(i), item = data[index];
                if (item.bondType != bondType)
                    list.push(item);
            }
            return list;
        },
        addBond: function(data, bond){
            var list = [];
            for (var i in data) {
                var index = parseInt(i), item = data[index];
                if (bond.bondType == 3 && item.bondType == 4) {
                    if (index + 1 < data.length && data[index + 1].bondType == 4)
                        list.push(item);
                } else {
                    if (item.bondType < bond.bondType)
                        list.push(item);
                }
            }
            list.push(bond);
            for (var i in data) {
                var index = parseInt(i), item = data[index];
                if (bond.bondType == 3 && item.bondType == 4) {
                    if (index + 1 == data.length || data[index + 1].bondType != 4)
                        list.push(item)
                }
                else
                    if (item.bondType > bond.bondType)
                        list.push(item);
            }
            return list;
        },
        createBond: function (data) {
            if (arguments.length > 0)
                bondData = data;
            $element.html('');
            var str = "";
            bondWidth = $f.getPixelWidth(data.width);
            str += getReportBond('reportTitle', 'عنوان گزارش', $f.getPixelHeight(data.height), 1);
            $element.append(str);
            $element.find('.bond').first().append('<span class="squarebond"></span>');
            $element.find('.bond').first().append('<span class="squarebond"></span>');
            $element.find('.bond').first().append('<span class="squarebond"></span>');
            $element.find('.bond').first().append('<span class="squarebond"></span>');
            $element.find('.bond').click(function () {
                updateBondOnFocuse(this);
            });
            var $bond = $('.bond').eq(0);
            var id = 1;
            for (var j in bondData.controls) {
                var control = bondData.controls[j], width = $f.getPixelWidth(control.position.width);
                var height = $f.getPixelHeight(control.position.height);
                switch (control.formControlType) {
                    case 1:
                        $bond.append($f.getLabel("id" + id, width, height));
                        break;
                    case 2:
                        $bond.append($f.getTextBox("id" + id, width, height));
                        break;
                    case 3:
                        $bond.append($f.getCheckListBox("id" + id, width, height));
                        break;
                    case 4:
                        $bond.append($f.getDropdownList("id" + id, width, height));
                        break;
                    case 5:
                        $bond.append($f.getPanel("id" + id, width, height));
                        break;
                }
                var ctr = null;
                switch (control.formControlType) {
                    case 1:
                        ctr = $('#id' + id).fLabel();
                        break;
                    case 2:
                        ctr = $('#id' + id).fTextBox();
                        break;
                    case 3:
                        ctr = $('#id' + id).fCheckListBox();
                        break;
                    case 4:
                        ctr = $('#id' + id).fDropdownList();
                        break;
                    case 5:
                        ctr = $('#id' + id).fPanel();
                        break;
                }
                ctr.init(control);
                id++;
            }
            hasFocuse = true;
        },
        updateCursor: function (x, y) {
            var flag = 1;
            $element = $(this.element);
            $element.find('.spliter').each(function () {
                var offset = $(this).offset(), top = offset.top - $(window).scrollTop();
                if (Math.abs(y - top) < 3 && x > offset.left && x < offset.left + $(this).width()) 
                    flag = 2;
            });
            if (flag != 2) {
                var offset = $element.offset();
                if (x - offset.left >= 0 && x - offset.left <= 3 && y > offset.top)
                    flag = 3;
            }
            switch (flag) {
                case 1:
                    $('#page').css('cursor', 'default');
                    break;
                case 2:
                    $('#page').css('cursor', 'row-resize');
                    break;
                case 3:
                    $('#page').css('cursor', 'col-resize');
                    break
            }
        },
        blur: function(){
            hasFocuse = false;
            var $square = $element.find('.squarebond');
            $square.css('display', 'none');
        },
        dragStart: function (x, y) {
            var index = -1;
            $element.find('.spliter').each(function (i) {
                var top = $(this).offset().top - $(window).scrollTop()
                if (Math.abs(y - top) <= 2) {
                    $bondForResize = $(this).parent().prev();
                    index = i;
                    return false;
                }
            });
            
            var $square = $element.find('.squarebond');
            $square.css('display', 'none');
            if ($bondForResize) {
                startHeight = $bondForResize.height();
                controlls = [];
                $('#bond').find('.bond').each(function (i) {
                    if (i > index) {
                        $(this).find('.reportcontrol').each(function () {
                            var ctr = { top: $(this).offset().top, control: this };
                            controlls.push(ctr);
                        });
                    }
                });
            }
            $element = $(this.element).find('#reportTitle');
            var offset = $element.offset();
            startWidth = $element.width()
            startLeft = offset.left;
            if (Math.abs(x - startLeft) <= 3 && y > offset.top)
                widthResizeable = true;
        },
        drop: function (e) {
            this.updateCursor(e.clientX, e.clientY);
            widthResizeable = false;
            updateBondOnFocuse($('.bond')[0]);
        },
        updateBondHeight: function (element) {
            var $bond = $(element).closest('.bond');
            var index = $('#bond').find('.bond').index($bond);
            var difHeight =  $(element).offset().top + $(element).height() - $bond.offset().top - $bond.height();
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
            var $ctr = $(ctr), rightCtr = $ctr.offset().left + $ctr.width(), topCtr = $ctr.offset().top;
            $('#bond').find('.bond').each(function () {
                var left = $(this).offset().left, right = left + $(this).width(), top = $(this).offset().top,
                    bottom = top + $(this).height();
                if (rightCtr > left && rightCtr <= right && topCtr >= top && topCtr < bottom) {
                    if ($ctr.closest('.bond').parent().attr('id') != $(this).parent().attr('id')) {
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
            return controlKind.bond;
        },
        drag: function (difX, difY) {
            if ($bondForResize && $bondForResize.find('.bond').attr('selected')) {
                if (difY >= 0) {
                    if (startHeight + difY > 20)
                        $bondForResize.find('.bond').height(startHeight + difY);
                }
                else {
                    var maxBottom = 0;
                    $bondForResize.find('.reportcontrol').each(function () {
                        var bottom = $(this).offset().top + $(this).height() - 1;
                        if (bottom > maxBottom)
                            maxBottom = bottom;
                    });
                    if (startHeight + difY > maxBottom - $bondForResize.offset().top) {
                        $bondForResize.find('.bond').height(startHeight + difY);
                    }
                }
                for (var i = 0; i < controlls.length; i++) 
                    $(controlls[i].control).css('top', controlls[i].top + $bondForResize.find('.bond').height() - startHeight);
            }
            if (widthResizeable) {
                var minLeft = null;
                $element.find('.reportcontrol').each(function () {
                    var left = $(this).offset().left;
                    if (minLeft == null || left < minLeft)
                        minLeft = left;
                });
                if (minLeft == null)
                    minLeft = 1000;
                $.form.print(startLeft + difX < minLeft)
                if (startLeft + difX < minLeft)
                    $('#reportTitle').width(startWidth - difX);
            }
        },
        destroyAfterResize: function () {
            $bondForResize = null;
        },
        getData: function () {
            var self = this;
            var bond = new Object();
            $(this.element).find('.bond').each(function () {
                bond.backGroundColor = new Object();
                bond.backGroundColor.colorString = $(this).css('background-color');
                bond.width = $f.getWidth($(this).width());
                bond.height = $f.getHeight($(this).height());
                bond.controls = [];
                var controlTypes = ['fLabel', 'fTextBox', 'fCheckListBox', 'fDropdownList', 'fPanel']
                $(this).find('.reportcontrol').each(function () {
                    var ctr = null;
                    for (var i = 0; i < controlTypes.length; i++)
                        if ($(this).data(controlTypes[i])) {
                            ctr = $(this).data(controlTypes[i]);
                            break;
                        }
                    if (ctr)
                        bond.controls.push(ctr.getData());
                });
            });
            return bond;
        }
    };
    $.fn.fBond = function (data) {
        var item = new fBond(this, data);
        $(this).data('fBond', item);
        return item;
    }
}
)(jQuery);
