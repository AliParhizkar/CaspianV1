/// <reference path="Common.js" />
let rControl = function (element) {

};

(function ($) {
    let $r = $.report, status, focused, widthStart, heightStart, leftStart, topStart;
    function getCurentBond(x, y) {
        ///  <summary>Report Bond ی که در مختصات x  و y قرار دارد را برمی گرداند</summary>
        ///  <param name="x" type="Number">مختصات x</param>
        ///  <param name="y" type="Number">مختصات y</param>
        ///  <type='Element' /returns>
        let bond = null;
        ///تمامی Report bondها دارای کلاس 
        ///bond هستند
        $('.bond').each(function () {
            let top = $(this).offset().top, left = $(this).offset().left, width = $(this).width(), height = $(this).height();
            if (y >= top && y <= top + height && x >= left && x <= left + width) {
                bond = this;
                return false;
            }
        });
        return bond;
    }
    function initControl(element) {
        $(element).find('.square').css('display', '');
        let width = $(element).width(), height = $(element).height();
        $(element).find('.square').each(function (index) {
            if (index == 0 || index == 1)
                $(this).css('left', (width - 6) / 2);
            if (index == 0)
                $(this).css('top', -1);
            if (index == 2)
                $(this).css('left', -1);
            if (index == 2 || index == 3)
                $(this).css('top', (height - 3) / 2);
            if (index == 1) 
                $(this).css('top', height - 2);
            if (index === 3) 
                $(this).css('left', width - 2);
        });
    }
    function changeWidth(difX, difY, element, obj) {
        let $element = $(element), id = $element.attr('id');
        if (status == statusType.changeWidthFromRight) {
            if (widthStart + difX > 23) 
                obj.width = widthStart + difX;
            else
                obj.width = 24;
            $('#page').find('.reportcontrol').each(function () {
                if ($(this).hasClass('tablecontrol')) {
                    $(this).find('th').each(function () {
                        let right = $(this).offset().left + $(this).width();
                        if (Math.abs(right - widthStart - difX - leftStart) < 5) {
                            obj.rightItem = this;
                            obj.width = right - $element.offset().left;
                        }
                    });
                }else
                if (id != $(this).attr('id'))
                    if (Math.abs($(this).offset().left + $(this).width() - widthStart - difX - leftStart) < 5) {
                        obj.rightItem = this;
                        obj.width = $(this).offset().left + $(this).width() - $element.offset().left;
                    }
            });
        }
        else {
            if (widthStart - difX > 17) {
                obj.width = widthStart - difX;
                obj.left = leftStart + difX;
            } else {
                obj.width = 18;
                obj.left = leftStart + widthStart - 18;
            }
            $element.closest('#bond').find('.reportcontrol').each(function () {
                if ($(this).hasClass('tablecontrol')) {
                    $(this).find('th').each(function () {
                        if (Math.abs($(this).offset().left - difX - leftStart) < 5) {
                            obj.leftItem = this;
                            obj.left = $(this).offset().left;
                            obj.width = leftStart + widthStart - obj.left;
                        }
                    });
                }else
                if (id != $(this).attr('id'))
                    if (Math.abs($(this).offset().left - difX - leftStart) < 5) {
                        obj.leftItem = this;
                        obj.left = $(this).offset().left;
                        obj.width = leftStart + widthStart - obj.left;
                    }
            });
        }
        return obj;
    }
    function changeHeight(difX, difY, element, obj) {
        let $element = $(element), id = $element.attr('id');
        if (status == statusType.changeHeightFromTop) {
            obj.height = heightStart - difY;
            obj.top = topStart + difY;
            $element.closest('.bond').find('.reportcontrol').each(function () {
                if (id != $(this).attr('id'))
                    if (Math.abs($(this).offset().top - difY - topStart) < 5) {
                        obj.top = $(this).offset().top;
                        obj.height = topStart + heightStart - obj.top;
                        obj.topItem = this;
                    }
            });
        }
        else {
            obj.height = heightStart + difY;
            $element.closest('.bond').find('.reportcontrol').each(function () {
                if (id != $(this).attr('id'))
                    if (Math.abs($(this).offset().top + $(this).height() - heightStart - difY - topStart) < 5) {
                        obj.bottomItem = this;
                        obj.height = $(this).offset().top + $(this).height() - $element.offset().top;
                    }
            });
           
        }
        return obj;
    }
    function move(difX, difY, element, obj) {
        let $element = $(element), id = $element.attr('id');
        obj.top = topStart + difY;
        obj.left = leftStart + difX;
        $element.closest('#bond').find('.reportcontrol').each(function () {
            if ($(this).hasClass('tablecontrol')) {
                $(this).find('th').each(function () {
                    debugger;
                    if (Math.abs($(this).offset().left - leftStart - difX) < 5) {
                        obj.leftItem = this
                        obj.left = $(this).offset().left;
                    }
                    if (Math.abs($(this).offset().left + $(this).width() - leftStart - difX - widthStart) < 5) {
                        obj.rightItem = this;
                        obj.left = $(this).offset().left + $(this).width() - $element.width();
                    }
                    if (Math.abs($(this).offset().top - topStart - difY) < 5) {
                        obj.topItem = this;
                        obj.top = $(this).offset().top;
                    }
                    if (Math.abs($(this).offset().top + $(this).height() - topStart - difY - heightStart) < 5) {
                        obj.bottomItem = this;
                        obj.top = $(this).offset().top + $(this).height() - $element.height();
                    }
                });
            } else if (id != $(this).attr('id')) {
                debugger
                    if (Math.abs($(this).offset().left - leftStart - difX) < 5) {
                        obj.leftItem = this
                        obj.left = $(this).offset().left;
                    }
                    if (Math.abs($(this).offset().left + $(this).width() - leftStart - difX - widthStart) < 5) {
                        obj.rightItem = this;
                        obj.left = $(this).offset().left + $(this).width() - $element.width();
                    }
                    if (Math.abs($(this).offset().top - topStart - difY) < 5) {
                        obj.topItem = this;
                        obj.top = $(this).offset().top;
                    }
                    if (Math.abs($(this).offset().top + $(this).height() - topStart - difY - heightStart) < 5) {
                        obj.bottomItem = this;
                        obj.top = $(this).offset().top + $(this).height() - $element.height();
                    }
                }
        });
        return obj;
    }
    rControl.prototype = {
        getSystemVariable: function () {
            let member = this.member();
            switch (member) {
                case '{Date}': return 1;
                case '{FName}': return 2;
                case '{LName}': return 3;
                case '{FLName}': return 4;
                case '{UserId}': return 5;
                case '{CodeMelli}': return 6;
            }
            return null;
        },
        getsystemFiledType: function () {
            let member = this.member();
            switch (member) {
                case '{Line}': return 1;
                case '{PageNumber}': return 2;
                case '{PageNofM}': return 3;
                case '{TotalPageCount}': return 4;
            }
            return null;
        },
        member: function (member) {
            if (arguments.length == 0) {
                return $(this.element).attr('member');
            }
            $(this.element).attr('member', member);
        },
        getStatus: function(){
            return status;
        },
        drag: function (difX, difY) {
            let topBond = $('#bond').offset().top, leftBond = $('#bond').offset().left, bottomBond = topBond + $('#bond').height(), rightBond = $('#bond').offset().left + 
                $('.bond').first().width();
            if (status != statusType.changeHeightFromBottom && topStart + difY < topBond)
                difY = topBond - topStart;
            let configWidth = $('#bond tr td').first().width();
            if (status != statusType.changeWidthFromRight && leftStart + difX < leftBond + configWidth)
                difX = leftBond - leftStart + configWidth;
            let toolsboxWidth = $('#toolsBox').width();
            if (status != statusType.changeWidthFromLeft && leftStart + difX + toolsboxWidth > rightBond)
                difX = rightBond - leftStart - toolsboxWidth;
            
            let obj = new Object();
            obj.left = leftStart;
            obj.top = topStart;
            obj.width = widthStart;
            obj.height = heightStart;
            switch (status) {
                case statusType.move:
                    obj = move(difX, difY, this.element, obj);
                    break;
                case statusType.changeWidthFromLeft:
                case statusType.changeWidthFromRight:
                    obj = changeWidth(difX, difY, this.element, obj);
                    break;
                case statusType.changeHeightFromTop:
                case statusType.changeHeightFromBottom:
                    obj = changeHeight(difX, difY, this.element, obj);
                    break;
            }
            if (obj) {
                $(this.element).css('left', obj.left);
                $(this.element).css('top', obj.top);
                $(this.element).width(obj.width);
                $(this.element).find('.text').width(obj.width);
                $(this.element).css('height', obj.height);
                let ruler = $('#cnvRuler').data('rRuler')
                ruler.hideStatus();
                ruler.showStatus(obj.left, $(this.element).width() + $(this.element).offset().left);
                if (obj.leftItem) 
                    $r.showLeftRuler(this.element, obj.leftItem);
                else
                    $r.hideRuler('#leftRuler');
                if (obj.rightItem)
                    $r.showRightRuler(this.element, obj.rightItem);
                else
                    $r.hideRuler('#rightRuler');
                if (obj.topItem) 
                    $r.showTopRuler(this.element, obj.topItem);
                else 
                    $r.hideRuler('#topRuler');
                if (obj.bottomItem)
                    $r.showBottomRuler(this.element, obj.bottomItem);
                else
                    $r.hideRuler('#bottomRuler');
            }
        },
        drop: function (x, y) {
            let bond = $('#bond').data('rBond');
            bond.addControlToBond(this.element);
            bond.updateBondHeight(this.element);
            $r.hideRuler();
            initControl(this.element);
        },
        updateCursor: function (x, y) { 
            let $element = $(this.element);
            let top = $element.offset().top - $(window).scrollTop(), left = $element.offset().left, width = $element.width(), height = $element.height(), bottom = top + height, right = left + width;
            
            status = statusType.none;
            if (y > top && y < bottom) {
                if (Math.abs(x - left) < 6 || Math.abs(x - right) < 6) {
                    if (Math.abs(x - left) < 6)
                        status = statusType.changeWidthFromLeft;
                    else
                        status = statusType.changeWidthFromRight;
                    $('#page').css('cursor', 'e-resize');
                }
                else {
                    
                    if (x > left && x < right) {
                        status = statusType.move;
                        $('#page').css('cursor', 'move');
                    }
                }
            } else {
                if (x > left && x < right) {
                    if (Math.abs(y - top) < 6 || Math.abs(y - bottom) < 6) {
                        if (Math.abs(y - top) < 6)
                            status = statusType.changeHeightFromTop;
                        else
                            status = statusType.changeHeightFromBottom;
                        $('#page').css('cursor', 's-resize');
                    }
                    else {
                        if (y > top && y < bottom) {
                            status = statusType.move;
                            $('#page').css('cursor', 'move');
                        }
                    }
                }
            }
            if (status == statusType.none)
                $('#page').css('cursor', 'default');
        },
        focus: function () {
            focused = true;
            status = statusType.move;
            let left = $(this.element).offset().left;
            $('#cnvRuler').data('rRuler').showStatus(left, left + $(this.element).width());
            $(this.element).find('.square').css('display', '');
        },
        dragStart: function (xStart, yStart) {
            let $element = $(this.element);
            widthStart = $element.outerWidth();
            heightStart = $element.outerHeight();
            leftStart = $element.offset().left;
            topStart = $element.offset().top;
            $(this.element).find('.square').css('display', 'none');
        },
        move: function (hMovement, vMovement) {
            let offset = $(this.element).offset();
            let $bond = $(this.element).closest('.bond'), left = $bond.offset().left, 
                right = left + $bond.outerWidth(), top = $bond.offset().top;
            hMovement = offset.left + hMovement;
            vMovement = offset.top + vMovement;
            let ruler = $('#cnvRuler').data('rRuler');
            if (hMovement >= left && hMovement <= right - $(this.element).outerWidth()) {
                $(this.element).css('left', hMovement);
                ruler.showStatus(hMovement, hMovement + $(this.element).outerWidth());
            }
            if (vMovement >= top)
                $(this.element).css('top', vMovement);
        },
        change: function (width, height) {
            let $element = $(this.element);
            $element.width($element.width() + width);
            $element.height($element.height() + height);
            initControl(this.element);
        },
        blur: function () {
            focused = false;
            $('#cnvRuler').data('rRuler').hideStatus();
            $(this.element).find('.square').css('display', 'none');
        },
        getData: function () {
            let $ctr = $(this.element);
            let control = new Object();
            let $bond = $ctr.closest('.bond');
            control.position = new Object();
            let $firstColumn = $bond.find('.column.right');
            if ($firstColumn.get(0) != null)
                control.position.left = $r.getWidth($ctr.offset().left - $firstColumn.offset().left);
            else
                control.position.left = $r.getWidth($ctr.offset().left - $bond.offset().left);
            control.position.top = $r.getHeight($ctr.offset().top - $bond.offset().top);
            control.position.width = $r.getWidth($ctr.width());
            control.position.height = $r.getHeight($ctr.height());
            let member = this.member();
            if (member != undefined)
                control.member = member;
            return control;
        },
        init: function (data) {
            let $element = $(this.element);
            let $parent = $element.closest('.bond');
            if ($element.closest('.column').hasClass('column'))
                $parent = $element.closest('.column');
            $element.css('left', $r.getPixelWidth(data.position.left) + $parent.offset().left);
            $element.css('top', $r.getPixelHeight(data.position.top) + $parent.offset().top);
            
            $element.width($r.getPixelWidth(data.position.width));
            $element.height($r.getPixelHeight(data.position.height));
            if (data.member)
                this.member(decodeURIComponent(data.member));
            if (this.control && this.control.imageUri) {
                this.control.setText(decodeURIComponent(data.text));
                
                this.control.imageUri(data.uri);
                this.control.stretch(data.stretch);
            }
        },
        getDataLevel: function () {
            let dataLevel = $(this.element).closest('.bond').attr('DataLevel');
            if (dataLevel)
                return dataLevel;
            return null;
        }
    };

    rControl.prototype.constructor = function () {
        widthStart = $(this.element).width();
        leftStart = 0;
        topStart = 0;
        heightStart = $(this.element).outerHeight();
        status = statusType.move;
        $(this.element).find('td').first().append('<span class="square" style="display:none;top:-2px;left:85px"></span><span class="square" style="display:none;top:31px;left:85px"></span>');
        $(this.element).find('td').last().append('<span class="square" style="display:none;top:15px;left:-2px"></span><span class="square" style="display:none;top:15px;left:176px"></span>');
        focused = true;
    };

    $.fn.control = function () {
        return new rControl(this);
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
