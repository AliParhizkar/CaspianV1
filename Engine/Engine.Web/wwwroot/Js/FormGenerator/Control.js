/// <reference path="Common.js" />
var fControl = function (element) {

};

(function ($) {
    var $f = $.form, status, focused, widthStart, heightStart, leftStart, topStart;
    function getCurentBond(x, y) {
        ///  <summary>Report Bond ی که در مختصات x  و y قرار دارد را برمی گرداند</summary>
        ///  <param name="x" type="Number">مختصات x</param>
        ///  <param name="y" type="Number">مختصات y</param>
        ///  <type='Element' /returns>
        var bond = null;
        ///تمامی Report bondها دارای کلاس 
        ///bond هستند
        $('.bond').each(function () {
            var top = $(this).offset().top, left = $(this).offset().left, width = $(this).width(), height = $(this).height();
            if (y >= top && y <= top + height && x >= left && x <= left + width) {
                bond = this;
                return false;
            }
        });
        return bond;
    }
    function initControl(element, multiLine) {
        if (focused)
            $(element).find('.square').css('display', '');
        else
            $(element).find('.square').css('display', 'none');
        var width = $(element).width(), height = $(element).height();
        $(element).find('.square').each(function (index) {
            if (index == 2 || index == 3)
                $(this).css('left', (width - 6) / 2);
            if (index == 2)
                $(this).css('top', -2);
            if (index == 1)
                $(this).css('left', -2);
            if (index == 1 || index == 0)
                $(this).css('top', (height - 6) / 2);
            if (index == 3)
                $(this).css('top', height - 3);
            if (index == 0)
                $(this).css('left', width - 3);
            if (!multiLine && (index == 2 || index == 3))
                $(this).css('display', 'none');
        });

    }
    function changeWidth(difX, difY, element, obj) {
        var $element = $(element), id = $element.attr('id');
        if (status == statusType.changeWidthFromRight) {
            if (widthStart + difX > 23) 
                obj.width = widthStart + difX;
            else
                obj.width = 24;
            $('#page').find('.reportcontrol').each(function () {
                if ($(this).hasClass('tablecontrol')) {
                    $(this).find('th').each(function () {
                        var right = $(this).offset().left + $(this).width();
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
        var $element = $(element), id = $element.attr('id');
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
        obj.top = topStart + difY;
        obj.left = leftStart + difX;
        var $element = $(element), id = $element.attr('id');
        $element.closest('#bond').find('.reportcontrol').each(function () {
            if ($(this).hasClass('tablecontrol')) {
                $(this).find('th').each(function () {
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
            }else
                if (id != $(this).attr('id')) {

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
    fControl.prototype = { 
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
            var topBond = $('#bond').offset().top, leftBond = $('#bond').offset().left, bottomBond = topBond + $('#bond').height(), rightBond = $('#bond').offset().left + 
                $('.bond').first().width();
            if (status != statusType.changeHeightFromBottom && topStart + difY < topBond)
                difY = topBond - topStart;
            if (status != statusType.changeWidthFromRight && leftStart + difX < leftBond)
                difX = leftBond - leftStart;
            if (status != statusType.changeWidthFromLeft && leftStart + difX + widthStart > rightBond)
                difX = rightBond - leftStart - widthStart;
            var obj = new Object();
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
                if (obj.leftItem) 
                    $f.showLeftRuler(this.element, obj.leftItem);
                else
                    $f.hideRuler('#leftRuler');
                if (obj.rightItem)
                    $f.showRightRuler(this.element, obj.rightItem);
                else
                    $f.hideRuler('#rightRuler');
                if (obj.topItem) 
                    $f.showTopRuler(this.element, obj.topItem);
                else 
                    $f.hideRuler('#topRuler');
                if (obj.bottomItem)
                    $f.showBottomRuler(this.element, obj.bottomItem);
                else
                    $f.hideRuler('#bottomRuler');
            }
        },
        drop: function (x, y) {
            var bond = $('#bond').data('fBond');
            bond.addControlToBond(this.element);
            bond.updateBondHeight(this.element);
            $f.hideRuler();
            initControl(this.element, this.multiLine);
        },
        updateCursor: function (x, y) { 
            var $element = $(this.element);
            var top = $element.offset().top - $(window).scrollTop(), left = $element.offset().left, width = $element.width(), height = $element.height(), bottom = top + height, right = left + width;
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
                    if (this.multiLine && (Math.abs(y - top) < 6 || Math.abs(y - bottom) < 6)) {
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
            initControl(this.element, this.multiLine);
        },
        dragStart: function (xStart, yStart) {
            var $element = $(this.element);
            widthStart = $element.outerWidth();
            heightStart = $element.outerHeight();
            leftStart = $element.offset().left;
            topStart = $element.offset().top;
            $(this.element).find('.square').css('display', 'none');
        },
        move: function (hMovement, vMovement) {
            var offset = $(this.element).offset();
            var $bond = $(this.element).closest('.bond'), left = $bond.offset().left, 
                right = left + $bond.outerWidth(), top = $bond.offset().top;
            hMovement = offset.left + hMovement;
            vMovement = offset.top + vMovement;
            if (hMovement >= left && hMovement <= right - $(this.element).outerWidth())
                $(this.element).css('left', hMovement);
            if (vMovement >= top)
                $(this.element).css('top', vMovement);
        },
        change: function (width, height) {
            var $element = $(this.element);
            $element.width($element.width() + width);
            $element.height($element.height() + height);
            initControl(this.element, this.multiLine);
        },
        blur: function () {
            focused = false;
            $(this.element).find('.square').css('display', 'none');
        },
        getData: function () {
            var $ctr = $(this.element);
            var $bond = $ctr.closest('.bond');
            var control = new Object();
            control.id = this.id;
            control.position = new Object();
            control.position.left = $f.getWidth($ctr.offset().left - $bond.offset().left);
            control.position.top = $f.getHeight($ctr.offset().top - $bond.offset().top);
            control.position.width = $f.getWidth($ctr.width());
            control.position.height = $f.getHeight($ctr.height());
            var member = this.member();
            if (member != undefined)
                control.member = member;
            return control;
        },
        init: function (data) {
            var $element = $(this.element);
            var $parent = $element.closest('.bond');
            if ($element.closest('.column.right').hasClass('column right'))
                $parent = $element.closest('.column.right');
            $element.css('left', $f.getPixelWidth(data.position.left) + $parent.offset().left);
            $element.css('top', $f.getPixelHeight(data.position.top) + $parent.offset().top);
            
            $element.width($f.getPixelWidth(data.position.width));
            $element.height($f.getPixelHeight(data.position.height));
            if (data.id)
                this.id = data.id;
            if (data.member)
                this.member(decodeURIComponent(data.member));
            initControl(this.element, this.multiLine);
        },
    };

    fControl.prototype.constructor = function () {
        widthStart = $(this.element).width();
        leftStart = -widthStart;
        topStart = 0;
        heightStart = $(this.element).outerHeight();
        status = statusType.move;
        $(this.element).find('td').last().append('<span class="square" style="display:none;top:15px;left:-2px"></span><span class="square" style="display:none;top:15px;left:176px"></span>');
        $(this.element).find('td').first().append('<span class="square" style="display:none;top:-2px;left:85px"></span><span class="square" style="display:none;top:31px;left:85px"></span>');
        focused = false;
    };

    $.fn.control = function () {
        return new fControl(this);
    }
    var statusType = {
        none: 0,
        changeWidthFromLeft: 1,
        changeWidthFromRight: 2,
        changeHeightFromTop: 3,
        changeHeightFromBottom: 4,
        move: 5
    }
})(jQuery);
