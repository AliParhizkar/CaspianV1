(function($) {
    var $c = $.caspian = {
        bindTooltip() {
            let tooltipTriggerList = [];
            $('[data-bs-toggle="tooltip"]').each(function () {
                tooltipTriggerList.push(this);
            });
            tooltipTriggerList.map(function (tooltipTriggerEl) {
                return new bootstrap.Tooltip(tooltipTriggerEl)
            });
            $('body').unbind('click.tooltip');
            $('body').bind('click.tooltip', function () {
                tooltipTriggerList.map(function (tooltipTriggerEl) {
                    $('.tooltip').remove();
                });
            });
        },
        clearTime: function (elm) {
            $(elm).find('line').attr('x2', 0).attr('y2', -110);
            $(elm).find('circle').each((index, cir) => {
                if (index < 2)
                    $(cir).attr('cx', 0).attr('cy', -110);
            });
        },
        setMinute: function (elm, e, type) {
            let from = $(elm).attr('from');
            let to = $(elm).attr('to');
            let pos = $(elm).find('circle').eq(2).position();
            if (!pos)
                return;
            let x = e.pageX - pos.left - 2;
            let y = e.pageY - pos.top - 2;
            let deg = Math.atan(y / x);
            if (x < 0)
                deg += Math.PI;
            let minute = Math.round(deg / Math.PI * 30 + 15);
            if (minute == 60)
                minute = 0;
            if (from != undefined && minute < from)
                return false;
            if (to != undefined && minute > to)
                return false;
            let text = minute.toString();
            if (minute < 10)
                text = '0' + text;
            $(elm).closest('.c-timepicker').find('.c-span-minutes').text(text);
            $(elm).closest('.c-timepicker')
            let x2 = null, y2 = null;
            if (type == 1) {
                x2 = Math.cos(deg) * 110;
                y2 = Math.sin(deg) * 110;
            }
            else {
                let factor = (minute - 15) / 30 * Math.PI;
                x2 = Math.cos(factor) * 110;
                y2 = Math.sin(factor) * 110;
            }
            let $svg = $(elm).find('svg');
            $svg.find('circle').eq(0).attr('cx', x2).attr('cy', y2);
            $svg.find('circle').eq(1).attr('cx', x2).attr('cy', y2);
            $svg.find('line').attr('x2', x2).attr('y2', y2);
            return true;
        },
        getTime: function (elem) {
            return $(elem).find('.c-span-hours').text() + ':' + $(elem).find('.c-span-minutes').text();
        },
        setTime: function (elem, dotnet) {
            let $panel = $(elem).find('.c-timepicker');
            let height = $panel.height() + 2;
            if ($(elem).position().top > height) 
                $panel.css('bottom', -height);
            else
                $panel.css('top', -height);
            let $input = $(elem).find('input');
            $input.val($.caspian.getTime(elem));
            setTimeout(async () => {
                var event = new Event('change');
                $input[0].dispatchEvent(event);
                await dotnet.invokeMethodAsync("CloseWindow");
            }, 200);
        },
        bindTimepicker: function (dotnet, element) {
            $(element).mouseenter(() => {
                let $element = $(element).find('.t-picker-wrap');
                if (!$element.hasClass('t-state-selected') && !$element.hasClass('t-state-disabled'))
                    $element.addClass('t-state-hover');
            });
            $(element).mouseleave(() => {
                $(element).find('.t-picker-wrap').removeClass('t-state-hover');
            });
            $(element).find('input').focus(() => {
                $(element).find('.t-picker-wrap').removeClass('t-state-hover').addClass('t-state-selected');
            });
            $(element).find('input').blur(() => {
                $(element).find('.t-picker-wrap').removeClass('t-state-selected');
            });
            const mutationObserver = new MutationObserver(() => {
                let elem = element;
                let $panel = $(elem).find('.c-timepicker');
                let height = $panel.height() + 2;
                let $container = $(elem).find('.t-animation-container');
                if ($(elem).position().top > height) {
                    $container.addClass('c-animate-up').removeClass('c-animate-down');
                    setTimeout(() => {
                        $container.find('.c-timepicker').css('bottom', 0);
                    }, 10);
                }
                else {
                    $container.addClass('c-animate-down').removeClass('c-animate-up');
                    setTimeout(() => {
                        $container.find('.c-timepicker').css('top', 0);
                    }, 10);
                    height += 8;
                }
                $container.width($panel.width() + 8).height(height);
                $(elem).find('.c-cancel').click(() => {
                    if ($(elem).position().top > height)
                        $panel.css('bottom', -height);
                    else
                        $panel.css('top', -height);
                    setTimeout(async () => {
                        await dotnet.invokeMethodAsync("CloseWindow");
                    }, 200);
                });
                $(elem).find('.c-ok').click(() => {
                    $.caspian.setTime(elem, dotnet);
                });
                $(elem).find('.c-clear').click(() => {
                    $.caspian.clearTime($(elem).find('.c-time-hour')[0]);
                    $.caspian.clearTime($(elem).find('.c-time-minutes')[0]);
                    $(elem).find('.c-span-hours').text('00');
                    $(elem).find('.c-span-minutes').text('00');
                });
                $.caspian.bindTimePanel($(elem).find('.c-timepicker')[0], dotnet); 
            });
            mutationObserver.observe($(element)[0], {
                attributes: false,
                childList: true,
                subtree: false
            });
            $('body').click(async e => {
                let elem = element;
                if ($(e.target).closest('.t-timepicker')[0] != elem) {
                    let $panel = $(elem).find('.c-timepicker');
                    if ($panel.hasClass('c-timepicker')) {
                        let height = $panel.height() + 2;
                        if ($(elem).position().top > height)
                            $panel.css('bottom', -height);
                        else
                            $panel.css('top', -height);
                        setTimeout(async () => {
                            await dotnet.invokeMethodAsync("CloseWindow");
                        }, 200);
                    }
                }
            })
        },
        bindTimePanel: function (elm, dotnet) {
            let isValid = false;
            $(elm).find('.c-time-minutes .c-tick-container').mousedown(e => {
                let pos = $(elm).find('circle').eq(2).position();
                let r = Math.pow(e.pageX - pos.left - 2, 2) + Math.pow(e.pageY - pos.top - 2, 2);
                if (r > 8000) {
                    $.caspian.xStart = e.pageX;
                    $.caspian.yStart = e.pageY;
                    $.caspian.startus = 1;
                    $.caspian.lineX2 = $(elm).find('svg line').attr('x2');
                    $.caspian.lineY2 = $(elm).find('svg line').attr('y2');
                    isValid = $.caspian.setMinute($(elm).find('.c-time-minutes')[0], e);
                }
            }).mouseup(e => {
                setTimeout(() => {
                    if (isValid && !$(elm).find('.c-time-footer').hasClass('c-time-footer')) {
                        $.caspian.setTime($(elm).closest('.t-timepicker')[0], dotnet);
                    }
                }, 100);
            });
            $('body').bind('mousemove.timepicker', e => {
                if ($.caspian.startus == 1) {
                    $.caspian.setMinute($(elm).find('.c-time-minutes')[0], e, 2);
                }
            });
            $('body').bind('mouseup', e => {
                $.caspian.startus = 0;
            });
        },
        bindCalendar: function (elm, index, vNavigation) {
            switch (index) {
                case 1:
                    if (vNavigation == 2) {//down
                        $(elm).find('.c-down-to-state').css('left', 0).css('top', 35).width(212).height(200);
                        $(elm).find('.c-down-from-state').css('opacity', 0);
                    }
                    break;
                case 2:
                    if (vNavigation == 1) {//up
                        let pos = $(elm).find('.c-up-to-state .t-state-selected').position();
                        $(elm).find('.c-up-from-state').width(47).height(60).css('top', pos.top + 38).css('left', pos.left + 3);
                        $(elm).find('.c-up-to-state').css('opacity', 1);
                        setTimeout(() => $(elm).find('.c-up-from-state').css('display', 'none'), 400);
                    }
                    else if (vNavigation == 2) {//down
                        $(elm).find('.c-down-to-state').css('left', 0).css('top', 35).width(212).height(200);
                        $(elm).find('.c-down-from-state').css('opacity', 0);
                    }
                    break;
                case 3:
                    if (vNavigation == 1) {//up
                        let pos = $(elm).find('.c-up-to-state .t-state-selected').position();
                        $(elm).find('.c-up-from-state').width(47).height(60).css('top', pos.top + 38).css('left', pos.left + 3);
                        $(elm).find('.c-up-to-state').css('opacity', 1);
                        setTimeout(() => $(elm).find('.c-up-from-state').css('display', 'none'), 400);
                    }
                    else if (vNavigation == 2) {//down
                        $(elm).find('.c-down-to-state').css('left', 0).css('top', 35).width(212).height(200);
                        $(elm).find('.c-down-from-state').css('opacity', 0);
                    }
                    break;
                case 4:
                    if (vNavigation == 1) {//up
                        let pos = $(elm).find('.c-up-to-state .t-state-selected').position();
                        $(elm).find('.c-up-from-state').width(47).height(60).css('top', pos.top + 38).css('left', pos.left + 3);
                        $(elm).find('.c-up-to-state').css('opacity', 1);
                        setTimeout(() => $(elm).find('.c-up-from-state').css('display', 'none'), 400);
                    }
                    break;
            }
        },
        enableAutoHide: function (dotnet) {
            $.caspian.autoHidedotnetObject = dotnet;
            $('body').unbind('mousedown.autoHidedotnetObject');
            $('body').bind('mousedown.autoHidedotnetObject', async function (e) {
                if (!$(e.target).closest('.auto-hide').hasClass('auto-hide')) {
                    $('body').unbind('mousedown.autoHidedotnetObject');
                    await $.caspian.autoHidedotnetObject.invokeMethodAsync('HideForm');
                    $.caspian.autoHidedotnetObject = null;
                }
            });
        },
        bindTabpanel: function (ctr) {
            let left = $(ctr).find('.t-state-active').position().left + 5;
            let width = ($(ctr).find('.t-state-active').width()) - 10;
            $(ctr).find('.t-tabstrip-items').width() - (left + width);
            $(ctr).find('.c-selected-panel').css('left', left).css('width', width);
        },
        bindLookupTree: function (dotnet, input) {
            const mutationObserver = new MutationObserver(() => {
                let location = $(input).position();
                let $ctr = $(input).closest('.c-lookup-tree').find('.c-tree-content');
                let maxHeight = null;;
                let width = $(input).closest('.c-lookup-tree').width();
                $content = $ctr.find('.c-treeview');
                if ($content.width() > width) {
                    $ctr.css('left', (width - $content.width()) / 2 + location.left);
                    width = $content.width();
                }
                else
                    $ctr.find('.c-treeview').width('calc(100% - 8px)');
                let scrolTop = $('.c-content-main').scrollTop();
                let downToUp = location.top > $(window).height() / 2;
                let $animate = null;
                if (downToUp) {
                    maxHeight = location.top;
                    $animate = $ctr.addClass('c-animate-up').css('bottom', $(window).height() - location.top - 22 - scrolTop);
                }
                else {
                    
                    maxHeight = $(window).height() - location.top - 62;
                    $animate = $ctr.addClass('c-animate-down').css('top', location.top + 34);
                }
                $animate = $animate.find('.c-treeview');
                $ctr.css('height', maxHeight).width(width + 2);
                setTimeout(() => {
                    if (downToUp) 
                        $animate.css('bottom', 0);
                     else
                        $animate.css('top', 0);
                    $ctr.find('.c-node-template').click(() => {
                        if ($(input).closest('.c-lookup-tree').attr('multiselect') == undefined) {
                            let $animate = $(input).closest('.c-lookup-tree').find('.c-animate-down >.c-treeview');
                            $animate.css('top', "-100%");
                            $animate = $(input).closest('.c-lookup-tree').find('.c-animate-up >.c-treeview');
                            $animate.css('bottom', "-100%");
                        }
                    });
                }, 25);
            });
            mutationObserver.observe($(input).closest('.c-lookup-tree')[0], {
                attributes: false,
                childList: true,
                subtree: false
            });
            $('body').bind('click', e => {
                var flag = $(input).closest('.c-lookup-tree').find('.c-tree-content').hasClass('c-tree-content');
                if (!$(e.target).closest('.auto-hide').hasClass('auto-hide') && flag) {
                    let $animate = $(input).closest('.c-lookup-tree').find('.c-animate-down >.c-treeview');
                    $animate.css('top', "-100%");
                    $animate = $(input).closest('.c-lookup-tree').find('.c-animate-up >.c-treeview');
                    $animate.css('bottom', "-100%");
                    setTimeout(async () => {
                        await dotnet.invokeMethodAsync("HideForm");
                    }, 200);

                }
            });
            $(input).unbind('focus');
            $(input).bind('focus', function () {
                $(input).closest('.c-content').addClass('c-state-focus')
            });
            $(input).unbind('blur');
            $(input).bind('blur', function () {
                $(input).closest('.c-content').removeClass('c-state-focus')
            });
        },
        get3Digit: function (num) {
            if (num === null || num === undefined)
                return '';
            var parts = num.toString().split(".");
            parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            return parts.join(".");
        },
        hideMessage: function () {
            if ($.caspian.infoTimer)
                clearTimeout($.caspian.infoTimer);
            $("#outMessage").css('top', -130);
            setTimeout(() => {
                $("#outMessage").remove();
            }, 300);
        },
        showMessage: function (message) {
            if ($.caspian.infoTimer)
                clearTimeout($.caspian.infoTimer);
            $("#outMessage").remove();
            var odv = $('<div class="t-widget t-message" id="outMessage"></div>');
            $(".c-content-main").append(odv);
            $("#outMessage").html('<div class="t-window-titlebar"><span class="t-title">Info</span><span class="t-close"><i class="fa fa-close"></i></span></div><div class="c-content">' + message + '</div>');
            $("#outMessage .t-close").click(function () {
                $.caspian.hideMessage();
            });
            setTimeout(() => {
                $("#outMessage").css('top', 30);
                $.caspian.infoTimer = setTimeout(() => {
                    $.caspian.hideMessage();
                }, 2000)
            }, 10);
        },
        bindTree: function (tree) {
            $(tree).find('.c-expand,.c-collaps').each((index, t) => {
                $(t).height(parseInt($(t).height()) + 1);
            });
            const mutationObserver = new MutationObserver((list) => {
                list.every(t => {
                    if ($(t.target).hasClass('c-collaps') || $(t.target).hasClass('c-expand')) {
                        if ($(t.target).attr('class') == 'c-expand') {
                            $(t.target).find('.c-expand,.c-collaps').each((index, t) => {
                                $(t).height(parseInt($(t).height()) + 1);
                            }); 
                        }
                        let height1 = $(t.target).find('.c-node-template').height();
                        let height2 = $(t.target).find('.c-subtree').height();
                        if (height2 == null)
                            height2 = 4;
                        else
                            height2 += 5;
                        let height = height1 + height2;
                        if (height != t.target.offsetHeight) {
                            let $parent = $(t.target).parent().closest('.c-expand');
                            while ($parent.hasClass('c-expand')) {
                                $parent.height('auto');
                                $parent = $parent.parent().closest('.c-expand');
                            }
                            if (!$parent.parent().hasClass('c-subtree'))
                                $parent.height('auto');
                            $(t.target).addClass('c-animate-tree');
                            setTimeout(() => {
                                $(t.target).height(height);
                                setTimeout(() => {
                                    $(t.target).removeClass('c-animate-tree');
                                    let $parent = $(t.target).parent().closest('.c-expand');
                                    while ($parent.hasClass('c-expand')) {
                                        $parent.height($parent.height());
                                        $parent = $parent.parent().closest('.c-expand');
                                    }
                                }, 400);
                            }, 25);
                        }
                        return false;
                    }
                    if ($(t.target).hasClass('c-subtree')) {
                        let $element = $(t.target).closest('.c-expand');
                        let $parent = $element.parent().closest('.c-expand');
                        if (!$parent.parent().hasClass('.c-subtree')) 
                            $parent.height('auto');
                        let height1 = $element.find('.c-node-template').height();
                        let height2 = $(t.target).height();
                        if (height2 == null)
                            height2 = 4;
                        else
                            height2 += 5;
                        let height = height1 + height2;
                        if (height != $element.height()) {
                            $element.addClass('c-animate-tree');
                            setTimeout(() => {
                                $element.height(height);
                                setTimeout(() => {
                                    $element.removeClass('c-animate-tree');

                                }, 400);
                            }, 25);
                        }
                        return false;
                    }
                });
            });
            mutationObserver.observe(tree, {
                attributes: false,
                childList: true,
                subtree: true,
            });
        },
        getItem: function (element) {
            var item = $(element).data('tTextBox');
            if (!item)
                item = $(element).data('tDatePicker');
            if (!item)
                item = $(element).data('tDropDownList');
            if (!item)
                item = $(element).data('tComboBox');
            if (!item)
                item = $(element).data('tTimePicker');
            if (!item)
                item = $(element).data('tReportControl');
            if (!item)
                item = $(element).data('tPictureBox');
            if (!item)
                item = $(element).data('tCheckBox');
            return item;
        },
        focusAndShowErrorMessage(element, message) {
            let item = $.caspian.getItem(element);
            if (item) {
                item.errorMessage = message;
                item.focus();
            }
            else
                $(element).focus();
        },
        showMessageBox: function (overlay, box) {
            let item;
            $('.t-widget.t-window').each(function () {
                if ($(this).css('display') == 'block')
                    item = $(this)[0];
            });
            debugger;
            if (item) {
                $(item).append(overlay).append(box);
                $(box).css('top', 10);
            }
            $(overlay).show().css('opacity', .5);
        },
        enableDefaultShortKey: function (status, dotnet) {
            $('body').unbind('keyup.confirmMessage');
            if (status) {
                $('.c-messagebox .c-primary').focus();
                $('body').bind('keyup.confirmMessage', async function (e) {
                    var key = e.keyCode;
                    console.log($(e.target).hasClass('c-primary'))
                    if (!$(e.target).hasClass('c-primary')) {
                        if (key == 13 || key == 27) {
                            await dotnet.invokeMethodAsync('HideConfirm', key == 13);
                        }
                    }
                });
            }
        },
        closeLookupWindow: function ($window, topToDown) {
            if (topToDown)
                $window.css('bottom', -$window.height() - 12);
            else
                $window.css('top', -$window.height() - 12);
        },
        bindLookup: function (dotnet, input) {
            let $lookup = $(input).closest('.c-lookup');
            
            const mutationObserver = new MutationObserver((mutationList) => {
                mutationList.forEach(mutation => {
                    if (mutation.type == 'attributes' && mutation.attributeName == 'status' && $(mutation.target).attr('status') == "2") {
                        let $animat = $(mutation.target).find('.t-animation-container');
                        let $window = $(mutation.target).find('.t-HelpWindow');
                        
                        let left = $(mutation.target).position().left - ($window.width() - $(mutation.target).width()) / 2;
                        $animat.css('left', left).width($window.width() + 10);
                        let loc = $animat.offset().top - $(window).scrollTop();
                        let top = $(mutation.target).position().top;
                        let downToUp = loc > $(window).height() / 2;
                        if (downToUp) {
                            $animat.css('bottom', $(window).height() - top - 20);
                            $window.css('bottom', -$window.height());
                            $animat.height($window.height() + 10);
                        }
                        else {
                            $animat.css('top', top + 40);
                            $window.css('top', -$window.height());
                            $animat.height($window.height() + 10);
                        }
                        setTimeout(() => {
                            if (loc > $(window).height() / 2) {
                                $window.addClass('c-lookup-animate-up');
                                $window.css('bottom', 0);
                            }
                            else {
                                $window.addClass('c-lookup-animate-down');
                                $window.css('top', 0);
                            }
                        }, 25);

                        $window.find('.t-window-action').click(() => $.caspian.closeLookupWindow($window, downToUp));
                        $window.find('.t-grid-content').mousedown(() => $.caspian.closeLookupWindow($window, downToUp));
                        let $lookup = $(mutation.target);
                        if ($lookup.attr('autoHide') != undefined) {
                            $('body').unbind('mousedown.lookup');
                            $('body').bind('mousedown.lookup', async function (e) {
                                if (!$(e.target).closest('.c-lookup').hasClass('c-lookup')) {
                                    $.caspian.closeLookupWindow($window, downToUp);
                                    await dotnet.invokeMethodAsync('Close');
                                }
                            });
                        }
                    }
                });
            });
            mutationObserver.observe($lookup[0], {
                attributes: true,
                childList: false,
                subtree: false
            });
        },
        dadaGridBind: function (grv) {
            const resizeObserver = new ResizeObserver(entries => {
                for (let entry of entries) {
                    if (entry.contentBoxSize && entry.contentBoxSize[0]) {
                        if ($(grv).find('.t-grid-content').height() < $(grv).find('.t-grid-content table').height()) {
                            if ($('body').hasClass('t-rtl'))
                                $(grv).find('.t-grid-header').css('padding-left', 11);
                            else
                                $(grv).find('.t-grid-header').css('padding-right', 11);
                        }
                        else
                            $(grv).find('.t-grid-header').css('padding-left', 0);
                    }
                }
            });
            resizeObserver.observe($(grv).find('.t-grid-content')[0]);
            $(grv).find('.t-grid-header-wrap th').mousemove(function (e) {
                let left = $(this).offset().left, right = left + $(this).width(), x = e.clientX;
                if ((x - left) < 5 || (right - x) < 5 || $(grv).data('mouseState')) {
                    $(this).css('cursor', 'col-resize');
                    $(grv).data('resize', true);
                }
                else {
                    $(this).css('cursor', '');
                    $(grv).data('resize', true);
                }
            });
            $(grv).find('.t-grid-content').scroll(function () {
                var scrollLeft = $(this).scrollLeft();
                $(grv).find('.t-grid-header').scrollLeft(scrollLeft);
            });
            $(grv).find('.t-grid-header-wrap th').mousedown(function (e) {
                if ($(grv).data('resize')) {
                    var rtl = $('body').hasClass('t-rtl');
                    let left = $(this).offset().left, right = left + $(this).width(), x = e.clientX;
                    if (x - left < 5 && rtl || right - x < 5 && !rtl) {
                        $(grv).data('curent', $(this));
                        $(grv).data('curentWidth', $(this).width());
                        let $next = $(this).next();
                        if ($next[0] == null)
                            $(grv).data('gridStatus', 3);
                        else
                            $(grv).data('gridStatus', 1);
                        $(grv).data('other', $next);
                        $(grv).data('otherWidth', $next.width());
                    }
                    if (right - x < 5 && rtl || x - left < 5 && !rtl) {
                        $(grv).data('curent', $(this));
                        $(grv).data('curentWidth', $(this).width());
                        let $prev = $(this).prev();
                        if ($prev[0] == null)
                            $(grv).data('gridStatus', 3);
                        else
                            $(grv).data('gridStatus', 2);
                        $(grv).data('other', $(this).prev());
                        $(grv).data('otherWidth', $(this).prev().width());
                    }
                    $(grv).data('mouseState', true);
                    $(grv).data('xStart', e.clientX);
                }
            });
            $('body').unbind('click.gridresize');
            $('body').bind('click.gridresize', function (e) {
                $(grv).data('mouseState', false);
            });
            $('body').unbind('mousemove.gridresize');
            $('body').bind('mousemove.gridresize', function (e) {
                if ($(grv).data('mouseState') == true) {
                    if ($(grv).data('gridStatus') == 3)
                        return;
                    let dif = $(grv).data('xStart') - e.clientX;
                    if ($(grv).data('gridStatus') == 2)
                        dif = -dif;
                    if ($('body').hasClass('t-ltr'))
                        dif = -dif;
                    let curentWidth = $(grv).data('curentWidth');
                    let otherWidth = $(grv).data('otherWidth');
                    let curentResult = curentWidth + dif, otherResult = otherWidth - dif;
                    if (curentResult < 30 || otherResult < 30)
                        return;
                    console.log(curentResult, otherResult)
                    $(grv).data('curent').width(curentResult);
                    let curentIndex = $(grv).find('.t-grid-header-wrap th').index($(grv).data('curent'));
                    $(grv).find('.t-grid-content table tr').first().children().eq(curentIndex).width(curentResult);
                    $(grv).find('.c-grid-inline table tr').first().children().eq(curentIndex).width(curentResult);
                    let otherIndex = $(grv).find('.t-grid-header-wrap th').index($(grv).data('other'));
                    $(grv).data('other').width(otherResult);
                    $(grv).find('.t-grid-content table tr').first().children().eq(otherIndex).width(otherResult);
                    $(grv).find('.c-grid-inline table tr').first().children().eq(otherIndex).width(otherResult);
                    if ($(grv).find('.t-grid-content').height() < $(grv).find('.t-grid-content table').height()) {
                        if ($('body').hasClass('t-ltr'))
                            $(grv).find('.t-grid-header').css('padding-right', 11);
                        else
                            $(grv).find('.t-grid-header').css('padding-left', 11);
                    } else 
                        $(grv).find('.t-grid-header').css('padding-right', 0).css('padding-left', 0);;
                }
            });
            $(grv).find('.t-grid-content').scroll(function () {
                let scrollLeft = $(this).scrollLeft();
                $(grv).find('.t-grid-header-wrap').scrollLeft(scrollLeft);
            });
            $(grv).find('.c-page-size .fa-angle-up').click(function () {
                $(grv).find('.c-page-size ul').css('display', 'block');
            });
            $(grv).find('.c-page-size li').click(function () {
                let value = $(this).text();
                let $input = $(grv).find('.c-page-size input');
                let oldValue = $input.val();
                if (value != oldValue) {
                    $(this).closest('ul').css('display', 'none');
                    $input.val(value);
                    const event = new Event('change');
                    $input[0].dispatchEvent(event);
                }
            });
        },
        serversideCombobox: function (input, errorMessage, diable, status) {
            
            if (status == 2) {
                let $control = $(input).closest('.t-combobox').find('.t-popup');
                $control.css('display', 'block');
                let $continer = $(input).closest('.t-combobox').find('.t-animation-container');
                
                let height = $control.height() + 5;
                if (height > 300)
                    height = 300;
                $continer.css('height', height);
            }
            if (diable)
                $(input).attr("disabled", "disabled").parent().addClass('t-state-disabled').removeClass('t-state-default');
            else
                $(input).removeAttr("disabled").parent().removeClass('t-state-disabled').addClass('t-state-default');

            if (errorMessage) {
                input.errorMessage = errorMessage;
                $(input).parent().removeClass('t-state-default').addClass('t-state-error');
            } else {
                input.errorMessage = null;
                $(input).parent().removeClass('t-state-error').addClass('t-state-default');
            }
            let $continer = $(input).closest('.t-combobox').find('.t-animation-container');
            if ($continer.find('.t-state-selected').hasClass('t-state-selected')) {
                let $control = $(input).closest('.t-combobox').find('.t-popup');
                var bottom = $continer.find('.t-state-selected').position().top + $continer.find('.t-state-selected').height() +
                    $control.scrollTop();
                if (bottom >= 300) {
                    $control.scrollTop(bottom - 300);
                }
            }
            let $control = $(input).closest('.t-combobox').find('.t-popup');
            let height = $control.height() + 5;
            if (height > 300)
                height = 300;
            if (status == 2)
                $continer.css('height', height);
        },
        bindDatePicker(dotnetHelper, ctr) {
            
            $(ctr).mouseenter(() => {
                let $element = $(ctr).find('.t-picker-wrap');
                if (!$element.hasClass('t-state-selected') && !$element.hasClass('t-state-disabled')) 
                    $element.addClass('t-state-hover');
            });
            $(ctr).mouseleave(() => {
                $(ctr).find('.t-picker-wrap').removeClass('t-state-hover');
            });
            $(ctr).find('input').focus(() => {
                $(ctr).find('.t-picker-wrap').removeClass('t-state-hover').addClass('t-state-selected');
            });
            $(ctr).find('input').blur(() => {
                $(ctr).find('.t-picker-wrap').removeClass('t-state-selected');
            });
            $(window).bind('click', function (e) {
                if (!$(e.target).closest('.t-calendar').hasClass('t-calendar') &&
                        $(e.target).closest('.t-picker-wrap')[0] != $(ctr).find('.t-picker-wrap')[0]) {
                    $(ctr).find('.c-animate-down .t-datepicker-calendar').css('top', '-100%');
                    $(ctr).find('.c-animate-up .t-datepicker-calendar').css('bottom', '-100%');
                    setTimeout(() => dotnetHelper.invokeMethodAsync('CloseWindow'), 200);
                }
            });
            const mutationObserver = new MutationObserver(() => {
                let $animate = $(ctr).find('.t-animation-container');
                let $calendar = $animate.find('.t-datepicker-calendar');
                $calendar.find('table tr .t-item').click(() => {
                    $(ctr).find('.c-animate-down .t-datepicker-calendar').css('top', '-100%');
                    $(ctr).find('.c-animate-up .t-datepicker-calendar').css('bottom', '-100%');
                });
                if ($animate.offset()) {
                    let loc = $animate.offset().top - $(window).scrollTop();
                    if (loc > $(window).height() / 2) {
                        $animate.addClass('c-animate-up').removeClass('c-animate-down');
                        $animate.height(237);
                        $calendar.css('bottom', 0);
                    }
                    else {
                        $animate.addClass('c-animate-down').removeClass('c-animate-up');
                        $animate.height(242);
                        $calendar.css('top', 0);
                    }
                }
            });
            mutationObserver.observe($(ctr)[0], {
                attributes: false,
                childList: true,
                subtree: false
            });
        },
        bindDragAndDrop: function (dragableDom, header) {
            //header.style.display =
            //$window.css('display', 'block');
            header.onmousedown = function (e) {
                e = e || window.event;
                pos3 = e.clientX;
                pos4 = e.clientY;
                document.onmouseup = function (e) {
                    document.onmouseup = null;
                    document.onmousemove = null;
                };
                document.onmousemove = function (e) {
                    e = e || window.event;
                    pos1 = pos3 - e.clientX;
                    pos2 = pos4 - e.clientY;
                    pos3 = e.clientX;
                    pos4 = e.clientY;
                    dragableDom.style.top = (dragableDom.offsetTop - pos2) + "px";
                    dragableDom.style.left = (dragableDom.offsetLeft - pos1) + "px";
                }
            }
        },
        bindFileDownload: async function (fileName, contentStreamReference) {
            const arrayBuffer = await contentStreamReference.arrayBuffer();
            const blob = new Blob([arrayBuffer]);
            const url = URL.createObjectURL(blob);
            const anchorElement = document.createElement('a');
            anchorElement.href = url;
            anchorElement.download = fileName ?? '';
            anchorElement.click();
            anchorElement.remove();
            URL.revokeObjectURL(url);
        },
        bindWindow(dotnet, window) {
            const mutationObserver = new MutationObserver((mutationList) => {
                mutationList.forEach(mutation => {
                    if (mutation.type == 'attributes' && mutation.attributeName == 'status') {
                        let $window = $(mutation.target).closest('.t-window');
                        if ($(mutation.target).attr('status') == "2") {
                            $window.css('display', 'block');
                            let $header = $window.find('.t-header');
                            if ($(mutation.target).attr('draggable') != undefined) {
                                $.caspian.bindDragAndDrop($window[0], $header[0]);
                                $header.css('cursor', 'move');
                            }
                            else {
                                $header[0].onmousedown = null;
                                $header.css('cursor', 'default');
                            }
                            let $parent = $window.parent().closest('.t-window');
                            $c.scrollTop = 0;
                            if (!$parent[0]) {
                                $c.scrollTop = $('.c-content-main').scrollTop() - 20;
                                $window.css('top', $c.scrollTop);
                                $parent = $('.c-content-main');
                            }
                            let left = ($parent.width() - $window.width()) / 2;
                            $window.css('left', left);
                            
                            let $overlay = null;
                            if ($(mutation.target).attr('modal') != undefined) {
                                $overlay = $('<div class="t-overlay"></div>');
                                $overlay.appendTo($parent);
                                $window.data('overlay', $overlay);
                            }
                            $window.addClass('window-animate');

                            setTimeout(function () {
                                $window.addClass('window-animate');
                                if ($overlay) 
                                    $overlay.css('opacity', .5);
                                $window.css('top', 40 + $c.scrollTop);
                                setTimeout(() => $window.removeClass('window-animate'), 400)
                            }, 25);
                        }
                        else {
                            let $overlay = $window.data('overlay');
                            $window.addClass('window-animate');
                            setTimeout(function () {
                                if ($overlay)
                                    $overlay.css('opacity', 0);
                                $window.css('top', 0 + $c.scrollTop);
                                setTimeout(function () {
                                    if ($overlay)
                                        $overlay.remove();
                                    $window.css('display', 'none');
                                }, 200);
                            }, 25)
                        }
                    }
                });
            });
            mutationObserver.observe($(window)[0], {
                attributes: true,
                childList: false,
                subtree: false
            });
        },
        bindDropdownList(dotnetHelper, ddl) {
            const mutationObserver = new MutationObserver(() => {
                let $animate = $(ddl).find('.t-animation-container');
                let $group = $animate.find('.t-group');
                let height = $group.outerHeight();
                if ($group.offset()) {
                    $group.find('.t-item').bind('click', (e) => {
                        let $group = $(ddl).find('.c-animate-down .t-group');
                        if (!$(e.target).closest('.t-item').hasClass('t-disable')) {
                            $group.css('top', '-100%');
                            $group = $(ddl).find('.c-animate-up .t-group');
                            $group.css('bottom', '-100%');
                        }
                    });
                    let loc = $group.offset().top - $(window).scrollTop();
                    if (loc > $(window).height() / 2) {
                        $animate.addClass('c-animate-up').removeClass('c-animate-down');
                        $animate.height(height);
                        $group.css('bottom', 0);
                    }
                    else {
                        $animate.addClass('c-animate-down').removeClass('c-animate-up');
                        $animate.height(height + 7);
                        $group.css('top', 0);
                    }
                }
            });
            mutationObserver.observe($(ddl)[0], {
                attributes: false,
                childList: true,
                subtree: false
            });
            $(window).bind('click', function (e) {
                if (!$(e.target).closest('.t-dropdown').hasClass('t-dropdown')) {
                    let $group = $(ddl).find('.c-animate-down .t-group');
                    $group.css('top', '-100%');
                    $group = $(ddl).find('.c-animate-up .t-group');
                    $group.css('bottom', '-100%');
                    setTimeout(() => dotnetHelper.invokeMethodAsync('CloseWindow'), 200);
                }
            });
        },
        bindComboBox(dotnetHelper, input, pageable) {
            if (pageable) {
                let div = $(input).closest('.t-combobox').find('.t-group');
                div.bind('scroll', function (e) {
                    var height = $(this).scrollTop() + $(this).innerHeight();
                    if (height > $(this).find('.t-reset').height() + 1) 
                        dotnetHelper.invokeMethodAsync('IncPageNumberInvokable');
                });
            }
            const mutationObserver = new MutationObserver(() => {
                let $group = $(input).closest('.t-combobox').find('.t-group');
                let $animate = $(input).closest('.t-combobox').find('.t-animation-container');
                let height = $group.outerHeight();
                $(input).closest('.t-combobox').find('.t-item').click((e) => {
                    let $group = $(input).closest('.t-combobox').find('.c-animate-down .t-group');
                    $group.css('top', '-100%');
                    $group = $(input).closest('.t-combobox').find('.c-animate-up .t-group');
                    $group.css('bottom', '-100%');
                });
                if ($group.offset()) {
                    let loc = $group.offset().top - $(window).scrollTop();
                    if (loc > $(window).height() / 2) {
                        $animate.addClass('c-animate-up').removeClass('c-animate-down');
                        $animate.height(height);
                        $group.css('bottom', 0);
                    }
                    else {
                        $animate.addClass('c-animate-down').removeClass('c-animate-up');
                        $animate.height(height + 7);
                        $group.css('top', 0);
                    }
                }
            });
            mutationObserver.observe($(input).closest('.t-combobox')[0], {
                attributes: false,
                childList: true,
                subtree: true
            });
            $(input).keypress(function (e) {
                let $continer = $(input).closest('.t-combobox').find('.t-animation-container');
                if (e.keyCode == 13 && $continer.css('display') == 'block')
                    return false;
            });
            var $control = $(input).closest('.t-combobox').find('.t-dropdown-wrap');
            $(input).closest('.t-combobox').mouseenter(function () {
                if (!$control.hasClass('t-state-disabled'))
                    $control.removeClass('t-state-default').addClass('t-state-hover');
            });
            $(input).closest('.t-combobox').mouseleave(function () {
                if (!$control.hasClass('t-state-disabled'))
                    $control.removeClass('t-state-hover').addClass('t-state-default');
            });
            $(input).focusin(function () {
                let message = $(input).closest('.t-widget').attr('data-bind');
                if (message)
                    $.telerik.showErrorMessage($(input).closest('.t-widget')[0], message);
                $control.removeClass('t-state-hover').removeClass('t-state-default').addClass('t-state-focused');
            });
            $(input).focusout(function () {
                $control.removeClass('t-state-focused').addClass('t-state-default');
                //$.caspian.hideErrorMessage($(input).closest('.t-widget')[0]);
            });
            $(window).bind('click', function (e) {
                if (!$(e.target).closest('.t-combobox').hasClass('t-combobox')) {
                    let $group = $(input).closest('.t-combobox').find('.c-animate-down .t-group');
                    $group.css('top', '-100%');
                    $group = $(input).closest('.t-combobox').find('.c-animate-up .t-group');
                    $group.css('bottom', '-100%');
                    setTimeout(() => dotnetHelper.invokeMethodAsync('CloseInvokable'), 200);
                }
            });
        },
        bindPopupWindow: function (dotnet, pos) {
            pos = JSON.parse(pos);
            $('.c-popup-window').css('display', 'block');
            var className = $('.c-popup-window').attr('class');
            $('.c-popup-window').attr('class', 'auto-hide c-popup-window');
            let width = $('.c-popup-window').width();
            let height = $('.c-popup-window').height();
            let top = $('.c-main-head').height();
            let left = $('.sidebar').width();
            $('.c-popup-window').width(width).height(height).attr('class', className);
            if (pos.left != undefined) 
                $('.c-popup-window').css({ left: pos.left + left, right: 'auto' });
            else if (pos.right != undefined)
                $('.c-popup-window').css({ left: 'auto', right: pos.right });
            if (pos.top != undefined)
                $('.c-popup-window').css({ top: pos.top + top, bottom: 'auto' });
            else if (pos.bottom != undefined)
                $('.c-popup-window').css({ top: 'auto', bottom: pos.bottom });
            $c.enableAutoHide(dotnet);
        },
        bindMultiSelect: function (element, json) {
            json = JSON.parse(json);
            $(element).data('message', json.errorMessage);
            let maxWidth = 0;
            $(element).find('.t-checkbox').each(function () {
                var width = $(this).find('.t-title').width();
                if (width > maxWidth)
                    maxWidth = width;
            });
            $(element).find('.t-checkbox').width(maxWidth + 30);
            if (!$(element).data('bind')) {
                $(element).data('bind', true);
                $(element).unbind('focus');
                $(element).bind('focus', function () {
                    let message = $(element).data('message');
                    if (message)
                        $.telerik.showErrorMessage(element, message);
                });
                $(element).unbind('blur');
                $(element).bind('blur', function () {
                    $.telerik.hideErrorMessage(element);
                });
            }
            if (json.focused) {
                $(element).focus();

            }
        },
        bindImage: function (pic, img, contentType) {
            contentType = contentType ?? 'image/png';
            if (img)
                $(pic).css('background-image', 'url(data:' + contentType + ';base64,' + img + ')');
            else
                $(pic).css('background-image', 'none');
        },
        bindMenu: function (elem) {
            $(elem).closest('.submenu').css('display', 'block');
            $(elem).addClass('selected');
            if (!$('#accordion').data('accordion')) {
                $(function () {
                    var Accordion = function (el, multiple) {
                        this.el = el || {};
                        this.multiple = multiple || false;
                        el.find('.submenu li').click(function () {
                            el.find('.submenu li').removeClass('selected');
                            $(this).addClass('selected');
                        });
                        var links = this.el.find('.link');
                        links.on('click', { el: this.el, multiple: this.multiple }, this.dropdown);
                    };
                    Accordion.prototype.dropdown = function (e) {
                        var flag = $(e.target).closest('.page').hasClass('hideMenu');
                        if (!flag) {
                            var $el = e.data.el, $this = $(this), $next = $this.next();
                            $next.slideToggle();
                            $this.parent().toggleClass('open');
                            if (!e.data.multiple)
                                $el.find('.submenu').not($next).slideUp().parent().removeClass('open');
                        }
                    };
                    var data = new Accordion($('#accordion'), false);
                    $('#accordion').data('accordion', data);
                });

                $('.accordion.m-collapse .link').mouseenter(function (e) {
                    $(this).append('<div class="m-submenu"></div>');
                    var self_ = this;
                    $(this).parent().find('.submenu li').each(function () {
                        $(self_).find('.m-submenu').append('<div><span>' + $(this).text() + '</span></div>');
                    });
                });
                $('.accordion.m-collapse .link').mouseleave(function (e) {
                    $(this).find('.m-submenu').remove();
                });
            }
        },
        bindControl: function (control, options, controlType) {

            options = JSON.parse(options);
            switch (controlType) {
                case 1:
                    if (options.multiLine) {
                        if (!$(control).data('tTextArea'))
                            $(control).tTextArea(options);
                        $(control).data('tTextArea').updateState(options);
                    } else {
                        if (!$(control).data('tTextBox'))
                            $(control).tTextBox(options);
                        $(control).data('tTextBox').updateState(options);
                    }
                    break;
            }
        },
        blockUI: function () {
            $.blockUI({ message: '<img src="/Content/loading_big.gif" />', css: { backgroundColor: 'transparent', border: '1px none transparent' } });
        },
        unblockUI: function () {
            $.unblockUI();
        },
    }
})(jQuery);


$.caspian.outMessage = function () {

    this.message = '';
    this.title = "خطا";
    this.kind = 1;
    this.timeOut = 2000;

    function create() {
        var odv = $('<div class="t-widget t-message" id="outMessage"></div>');
        $("body").append(odv);
        $("#outMessage").html('<div class="t-window-titlebar"><span class="t-title"></span><span class="t-close"><i class="fa fa-close"></i></span></div><div style="padding:5px 5px 5px 5px;text-align:justify"></div>');
        $("#outMessage .t-close").click(function () {
            hide();
        });
        $("#outMessage .t-close").mouseover(function () {
            if (!$(this).hasClass('t-state-hover'))
                $(this).addClass('t-state-hover');
        });
        $("#outMessage .t-close").mouseout(function () {
            $(this).removeClass('t-state-hover');
        });
    };
    this.show = function (msg) {
        let isltr = $('body').hasClass('t-rtl');
        if (typeof msg == 'string') {
            try {
                msg = eval('(' + msg + ')');
                this.message = msg.Message;
                this.title = msg.kind == 1 ? "خطا" : "پیغام";
            }
            catch (ex) {
                this.message = msg;
                this.title = isltr ? "پیغام" : "Message";
            }
        }
        else {
            this.message = msg.Message;
            this.title = msg.kind == 1 ? "خطا" : "پیغام";
        }
        flag = false;
        $("#outMessage").remove();
        create();
        height = 30;
        $("#outMessage .t-title").text(this.title);
        $("#outMessage").children().eq(1).html(this.message);
        $("#outMessage").css('opacity', 0.1);
        $("#outMessage").show();
        $("#outMessage").css('height', height + "px");
        opa = 1 - $("#outMessage").css('opacity');
        opa = opa / (125 - height);
        flag = true;
        moverItem();
        if (timer != null)
            clearTimeout(timer);
        timer = setTimeout('hide()', 4000);
    };
    return this;
}