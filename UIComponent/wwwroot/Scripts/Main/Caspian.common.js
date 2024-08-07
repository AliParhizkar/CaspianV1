﻿(function ($) {
    let $c = $.caspian = {
        bindSlider: async function (element, dotnet) {
            let containerWidth = $(element).width();
            let ContentWidth = $(element).find('.c-slider-slide').width();
            $(element).find('.c-slider-body').width(ContentWidth);
            $(element).find('.c-slider-content').css('left', -ContentWidth);
            await dotnet.invokeMethodAsync('SetData', containerWidth, ContentWidth);
            $(window).resize(async () => {
                let containerWidth = $(element).width();
                let ContentWidth = $(element).find('.c-slider-slide').width();
                await dotnet.invokeMethodAsync('SetData', containerWidth, ContentWidth);
            });
        },
        bindInputCollorPicker: function (element, dotnet) {
            const mutationObserver = new MutationObserver(() => {
                let $animate = $(element).find('.c-animation-container');
                if ($animate.hasClass('c-animation-container')) {
                    let loc = $(element).offset().top - $(window).scrollTop();
                    if (loc > $(window).height() / 2) {
                        $animate.addClass('c-animation-up');
                        setTimeout(t => {
                            $(element).find('.c-colorpicker-panel').css('bottom', '0');
                        }, 10);
                    }
                    else {
                        $animate.addClass('c-animation-down');
                        setTimeout(t => {
                            $(element).find('.c-colorpicker-panel').css('top', '6px');
                        }, 10);
                    }

                }
            });
            
            mutationObserver.observe($(element)[0], {
                attributes: false,
                childList: true,
                subtree: false
            });
            $(element).mouseenter(() => {
                $(element).find('.c-input-color').addClass('c-state-hover');
            });
            $(element).mouseleave(() => {
                $(element).find('.c-input-color').removeClass('c-state-hover');
            });
            $(element).focus(() => {
                $(element).find('.c-input-color').addClass('c-state-focused');
            });
            $(element).blur(() => {
                $(element).find('.c-input-color').removeClass('c-state-focused').removeClass('c-state-hover');
            });
            $(window).bind('mousedown', async e => {
                if (!$(e.target).closest('.c-animation-container').hasClass('c-animation-container') && $(element).find('.c-animation-container').hasClass('c-animation-container'))
                    await dotnet.invokeMethodAsync("Close");
            });
        },

        bindColorPicker: function (element) {
            let picker = $(element).colorPicker();
            picker.updateColor();
            const mutationObserver = new MutationObserver((mutationList) => {
                if (mutationList[0].attributeName != 'id') {
                    picker.updateColor();
                }
            });
            mutationObserver.observe(element, {
                attributes: true,
                childList: false,
                subtree: false
            });
        },
        bindCodeEditor: function (code, dotnet, readonly, lineNumber, column, tokensData) {
            $.caspian.dotnet = dotnet;
            $.caspian.tokensData = tokensData;
            require.config({ paths: { vs: '/node_modules/monaco-editor/min/vs' } });
            require(['vs/editor/editor.main'], function () {
                registerCsharpProvider();
                var editor = $.caspian.editor = monaco.editor.create(document.getElementById('caspianCodeGenerator'), {
                    value: code,
                    language: 'csharp',
                    readOnly: readonly
                });

                editor.focus();
                if (lineNumber && column) {
                    editor.setPosition({
                        lineNumber: lineNumber,
                        column: column,
                    });
                    editor.revealLineInCenter(lineNumber);
                }
            });
        },
        setEditorCode: function (code) {
            if ($.caspian.editor)
                $.caspian.editor.getModel().setValue(code);
        },
        setListHeaderPadding($list) {
            let $content = $list.find('.c-dataview-content');
            let height = $content.outerHeight();
            $content.css('overflow', 'visible').css('height', 'auto');
            let realHeight = $content.outerHeight();
            $content.css('overflow', 'auto').css('height', height);
            if (realHeight > height)
                $list.find('.c-dataview-header').css('padding-right', '11px');
            else
                $list.find('.c-dataview-header').css('padding-right', '0');
        },
        bindListView(list) {
            $c.setListHeaderPadding($(list));
            const mutationObserver = new MutationObserver(t => {
                if (t.length > 1) {
                    let $list = $(t[t.length - 1].target).closest('.c-widget.c-data-view');
                    $c.setListHeaderPadding($($list));
                }
            });
            mutationObserver.observe($(list).find('.c-dataview-content')[0], {
                attributes: false,
                childList: true,
                subtree: true
            });
        },
        setEditoPosition: function (element, lineNumber, column) {
            //let editor = $.caspian.editor;
            //editor.focus();
            //editor.setPosition({
            //    lineNumber: lineNumber,
            //    column: column,
            //});
        },
        gridData: {
            resize: 0,
            gridStatus: 0,
            mouseState: 0,
            curent: null,
            other: null,
            curentWidth: 0,
            otherWidth: 0,
            xStart: 0,
            grid: null,
        },
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
        showErrorMessage: function (ctr) {
            
            $(ctr).find('.errorMessage').remove();
            let msg = $(ctr).attr('error-message');
            console.log(msg)
            if (msg) {
                $('<div class="errorMessage"><span class="c-icon"><i class="fa fa-info" aria-hidden="true"></i></span><Span class="c-content">'
                    + msg + '</Span><span class="c-pointer"></span></div>').appendTo(ctr);
            }
        },
        hideErrorMessage: function (ctr) {
            $(ctr).find('.errorMessage').remove();
        },
        fitMainToParent: function () {

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
                    $(elm).find('table tr .t-item').unbind('click.daySelect');
                    $(elm).find('table tr .t-item').bind('click.daySelect', () => {
                        let $ctr = $(elm).closest('.t-datepicker');
                        $ctr.find('.c-animate-down .t-datepicker-calendar').css('top', '-100%');
                        $ctr.find('.c-animate-up .t-datepicker-calendar').css('bottom', '-100%');
                    });
                    if (vNavigation == 2) {//down
                        $(elm).find('.c-down-to-state').css('left', 0).css('top', 36).width(212).height(200);
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
                    if ($.caspian.autoHidedotnetObject != null) {
                        await $.caspian.autoHidedotnetObject.invokeMethodAsync('HideForm');
                        $.caspian.autoHidedotnetObject = null;
                    }
                }
            });
        },
        bindTabpanel: function (ctr) {
            var pos = $(ctr).find('.t-state-active').position();
            if ($(ctr).hasClass('t-vertical')) {
                let top = pos.top;
                $(ctr).find('.c-selected-panel').css('top', top + 8);
            } else {
                let left = pos.left + 5;
                let width = ($(ctr).find('.t-state-active').width()) - 3;
                $(ctr).find('.t-tabstrip-items').width() - (left + width);
                $(ctr).find('.c-selected-panel').css('left', left).css('width', width);
            }
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
                $(input).closest('.c-content').addClass('c-state-focus');
            });
            $(input).unbind('blur');
            $(input).bind('blur', function () {
                $(input).closest('.c-content').removeClass('c-state-focus');
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
        bindBox: function () {
            $('.box').mousedown(e => {
                $.xStart = e.clientX;
                $.left = $('.box').offset().left
                $.down = true;
            });
            $('.box').mousemove(e => {
                if ($.down) {
                    let dif = e.clientX - $.xStart;
                    let maxleft = $(window).width() * 0.03;
                    let minLeft = $(window).width() - $('.box').width() - 20;
                    let left = $.left + dif;
                    if (left <= minLeft)
                        left = minLeft;
                    if (left >= maxleft)
                        left = maxleft;
                    $('.box').css('left', left);
                }
            });
            $('body').mouseup(e => {
                $.down = false;
            });
        },
        showMessage: function (message) {
            if ($.caspian.infoTimer)
                clearTimeout($.caspian.infoTimer);
            $("#outMessage").remove();
            var odv = $('<div class="t-widget t-message" id="outMessage"></div>');
            if ($(".c-content-main")[0])
                $(".c-content-main").append(odv);
            else
                $("body").append(odv);
            $("#outMessage").html('<div class="t-window-titlebar"><span class="t-title">Info</span><span class="t-close"><i class="fa fa-close"></i></span></div><div class="c-content">' + message + '</div>');
            $("#outMessage .t-close").click(function () {
                $.caspian.hideMessage();
            });
            setTimeout(() => {
                $("#outMessage").css('top', 30);
            }, 5);
            $.caspian.infoTimer = setTimeout(() => {
                $.caspian.hideMessage();
            }, 4000);
        },
        bindTree: function (tree) {
            //$(tree).find('.c-expand,.c-collaps').each((index, t) => {
            //    $(t).height(parseInt($(t).height()) + 1);
            //});
            //const mutationObserver = new MutationObserver((list) => {
            //    list.every(t => {
            //        if ($(t.target).hasClass('c-collaps') || $(t.target).hasClass('c-expand')) {
            //            if ($(t.target).attr('class') == 'c-expand') {
            //                $(t.target).find('.c-expand,.c-collaps').each((index, t) => {
            //                    $(t).height(parseInt($(t).height()) + 1);
            //                }); 
            //            }
            //            let height1 = $(t.target).find('.c-node-template').height();
            //            let height2 = $(t.target).find('.c-subtree').height();
            //            if (height2 == null)
            //                height2 = 4;
            //            else
            //                height2 += 5;
            //            let height = height1 + height2;
            //            if (height != t.target.offsetHeight) {
            //                let $parent = $(t.target).parent().closest('.c-expand');
            //                while ($parent.hasClass('c-expand')) {
            //                    $parent.height('auto');
            //                    $parent = $parent.parent().closest('.c-expand');
            //                }
            //                if (!$parent.parent().hasClass('c-subtree'))
            //                    $parent.height('auto');
            //                $(t.target).addClass('c-animate-tree');
            //                setTimeout(() => {
            //                    $(t.target).height(height);
            //                    setTimeout(() => {
            //                        $(t.target).removeClass('c-animate-tree');
            //                        let $parent = $(t.target).parent().closest('.c-expand');
            //                        while ($parent.hasClass('c-expand')) {
            //                            $parent.height($parent.height());
            //                            $parent = $parent.parent().closest('.c-expand');
            //                        }
            //                    }, 400);
            //                }, 25);
            //            }
            //            return false;
            //        }
            //        if ($(t.target).hasClass('c-subtree')) {
            //            let $element = $(t.target).closest('.c-expand');
            //            let $parent = $element.parent().closest('.c-expand');
            //            if (!$parent.parent().hasClass('.c-subtree')) 
            //                $parent.height('auto');
            //            let height1 = $element.find('.c-node-template').height();
            //            let height2 = $(t.target).height();
            //            if (height2 == null)
            //                height2 = 4;
            //            else
            //                height2 += 5;
            //            let height = height1 + height2;
            //            if (height != $element.height()) {
            //                $element.addClass('c-animate-tree');
            //                setTimeout(() => {
            //                    $element.height(height);
            //                    setTimeout(() => {
            //                        $element.removeClass('c-animate-tree');

            //                    }, 400);
            //                }, 25);
            //            }
            //            return false;
            //        }
            //    });
            //});
            //mutationObserver.observe(tree, {
            //    attributes: false,
            //    childList: true,
            //    subtree: true,
            //});
        },
        focusAndShowErrorMessage(element) {
            if ($(element).closest('.t-dropdown').hasClass('t-dropdown'))
                $(element).closest('.t-dropdown').focus();
            else
                $(element).focus();
        },
        showMessageBox: function (overlay, box) {
            let item;
            $('.t-widget.t-window').each(function () {
                if ($(this).css('display') == 'block')
                    item = $(this)[0];
            });
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
            $(input).focus(() => {
                $.caspian.showErrorMessage($lookup[0]);
            });
            $(input).keydown(e => {
                let code = e.code || e.keyCode;
                if (code == 40 || code == 38) 
                    e.preventDefault();
            });
            $(input).blur(() => {
                $.caspian.hideErrorMessage($lookup[0]);
            });
            if ($lookup.attr('closeonblur') != undefined) {
                $lookup.attr('tabindex', 0);
                $lookup.focusout(async () => {
                    //let downToUp = top > $(window).height() / 2 - 20;
                    //$.caspian.closeLookupWindow($lookup.find('.t-HelpWindow'), downToUp);

                    //await dotnet.invokeMethodAsync('Close');
                });
            }
            const mutationObserver = new MutationObserver((mutationList) => {
                mutationList.forEach(mutation => {
                    if (mutation.type == 'attributes' && mutation.attributeName == 'status' && $(mutation.target).attr('status') == "2") {

                        let $animat = $(mutation.target).find('.t-animation-container');
                        let $window = $(mutation.target).find('.t-HelpWindow');
                        let left = $(mutation.target).position().left - ($window.width() - $(mutation.target).width()) / 2 - 20;
                        if (left + $window.width() > $('.c-content-main').outerWidth())
                            left = $('.c-content-main').outerWidth() - $window.width() - 35;
                        $animat.css('left', left).width($window.width() + 10).css('right', 'auto');
                        let top = $(mutation.target).position().top;
                        let downToUp = top > $(window).height() / 2 - 20;
                        if (downToUp) {
                            $animat.css('bottom', $('.c-content-main').outerHeight() - top - $('.c-content-main').scrollTop());
                            $window.css('bottom', -$window.height());
                            $animat.height($window.height() + 10);
                        }
                        else {
                            $animat.css('top', top + $('.c-content-main').scrollTop() + 35);
                            $window.css('top', -$window.height());
                            $animat.height($window.height() + 10);
                        }
                        setTimeout(() => {
                            if (top > $(window).height() / 2) {
                                $window.addClass('c-lookup-animate-up');
                                $window.css('bottom', 12);
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
        dadaGridBind: function (grid) {
            const resizeObserver = new ResizeObserver(entries => {
                let grv = grid;
                for (let entry of entries) {
                    if (entry.contentBoxSize && entry.contentBoxSize[0]) {
                        if ($(grv).find('.t-grid-content').height() < $(grv).find('.t-grid-content table').height()) {
                            if ($('body').hasClass('t-rtl'))
                                $(grv).find('.t-grid-header').css('padding-left', 11);
                            else
                                $(grv).find('.t-grid-header').css('padding-right', 11);
                        }
                        else {
                            if ($('body').hasClass('t-rtl'))
                                $(grv).find('.t-grid-header').css('padding-left', 0);
                            else
                                $(grv).find('.t-grid-header').css('padding-right', 0);
                        }
                    }
                }
            });
            resizeObserver.observe($(grid).find('.t-grid-content')[0]);
            $(grid).find('.t-grid-header-wrap th').mousemove(function (e) {
                let left = $(this).offset().left, right = left + $(this).width(), x = e.clientX;
                if ((x - left) < 5 || (right - x) < 5 || $c.gridData.mouseState) {
                    $(this).css('cursor', 'col-resize');
                    $c.gridData.resize = true;
                }
                else {
                    $(this).css('cursor', '');
                    $c.gridData.resize = true;
                }
            });
            //$(grid).find('.t-grid-content').scroll(function () {
            //    //let scrollLeft = $(this).scrollLeft();
            //    //$(grid).find('.t-grid-header').scrollLeft(scrollLeft);
            //});
            $(grid).find('.t-grid-header-wrap th').mousedown(function (e) {
                $c.gridData.grid = $(e.target).closest('.t-grid');
                if ($c.gridData.resize) {
                    let rtl = $('body').hasClass('t-rtl');
                    let left = $(this).offset().left, right = left + $(this).width(), x = e.clientX;
                    if (x - left < 5 && rtl || right - x < 5 && !rtl) {
                        $c.gridData.curent = $(this);
                        $c.gridData.curentWidth = $(this).width()
                        let $next = $(this).next();
                        if ($next[0] == null)
                            $c.gridData.gridStatus = 3;
                        else
                            $c.gridData.gridStatus = 1;
                        $c.gridData.other = $next;
                        $c.gridData.otherWidth = $next.width()
                    }
                    if (right - x < 5 && rtl || x - left < 5 && !rtl) {
                        $c.gridData.curent = $(this);
                        $c.gridData.curentWidth = $(this).width()
                        let $prev = $(this).prev();
                        if ($prev[0] == null)
                            $c.gridData.gridStatus = 3;
                        else
                            $c.gridData.gridStatus = 2;
                        $c.gridData.other = $(this).prev();
                        $c.gridData.otherWidth = $(this).prev().width();
                    }
                    $c.gridData.mouseState = true;
                    $c.gridData.xStart = e.clientX;
                }
            });
            $('body').unbind('click.gridresize');
            $('body').bind('click.gridresize', function (e) {
                $c.gridData.mouseState = false;
            });
            $('body').unbind('mousemove.gridresize');
            $('body').bind('mousemove.gridresize', function (e) {
                let grv = $c.gridData.grid;
                if ($c.gridData.mouseState == true) {
                    if ($c.gridData.gridStatus == 3)
                        return;
                    let dif = $c.gridData.xStart - e.clientX;
                    if ($c.gridData.gridStatus == 2)
                        dif = -dif;
                    if ($('body').hasClass('t-ltr'))
                        dif = -dif;
                    let curentWidth = $c.gridData.curentWidth;
                    let otherWidth = $c.gridData.otherWidth - 1;
                    let curentResult = curentWidth + dif, otherResult = otherWidth - dif;
                    if (curentResult < 30 || otherResult < 30)
                        return;
                    $c.gridData.curent.width(curentResult);
                    let curentIndex = $(grv).find('.t-grid-header-wrap th').index($c.gridData.curent);
                    $(grv).find('.c-grid-items tr').first().children().eq(curentIndex).width(curentResult);
                    $(grv).find('.c-grid-insert tr').first().children().eq(curentIndex).width(curentResult);
                    let otherIndex = $(grv).find('.t-grid-header-wrap th').index($c.gridData.other);
                    $c.gridData.other.width(otherResult);
                    $(grv).find('.c-grid-items tr').first().children().eq(otherIndex).width(otherResult);
                    $(grv).find('.c-grid-insert tr').first().children().eq(otherIndex).width(otherResult);
                    if ($(grv).find('.t-grid-content').height() < $(grv).find('.c-grid-items').height()) {
                        if ($('body').hasClass('t-ltr'))
                            $(grv).find('.t-grid-header').css('padding-right', 11);
                        else
                            $(grv).find('.t-grid-header').css('padding-left', 11);
                    } else
                        $(grv).find('.t-grid-header').css('padding-right', 0).css('padding-left', 0);
                }
            });
            $(grid).find('.t-grid-content').scroll(function () {
                let grv = grid;
                let scrollLeft = $(this).scrollLeft();
                $(grv).find('.t-grid-header-wrap').scrollLeft(scrollLeft);
            });
            $(grid).find('.c-page-size .fa-angle-up').click(function () {
                let grv = grid;
                $(grv).find('.c-page-size ul').css('display', 'block');
            });
            $(grid).find('.c-page-size li').click(function () {
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
        bindDatePicker(dotnetHelper, ctr) {
            $c.bindtomask($(ctr).find('input')[0], '____/__/__');
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
                $.caspian.showErrorMessage($(ctr).closest('.t-widget')[0]);
            });
            $(ctr).find('input').blur(() => {
                $(ctr).find('.t-picker-wrap').removeClass('t-state-selected');
                $.caspian.hideErrorMessage($(ctr).closest('.t-widget')[0]);
            });
            $(window).bind('click', e => {
                if (!$(e.target).closest('.t-calendar').hasClass('t-calendar') &&
                    $(e.target).closest('.t-picker-wrap')[0] != $(ctr).find('.t-picker-wrap')[0]) {
                    $(ctr).find('.c-animate-down .t-datepicker-calendar').css('top', '-100%');
                    $(ctr).find('.c-animate-up .t-datepicker-calendar').css('bottom', '-100%');
                    setTimeout(async () => await dotnetHelper.invokeMethodAsync('CloseWindow'), 200);
                }
            });
            const mutationObserver = new MutationObserver(() => {
                let $animate = $(ctr).find('.t-animation-container');
                let $calendar = $animate.find('.t-datepicker-calendar');
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
        openWindow: function ($window) {
            $content = $window.find('.t-window-content');
            if ($content.attr('status') == "2") {
                $window.css('top', $('.c-content-main').scrollTop() - 20);
                $window.css('display', 'block');
                let $header = $window.find('.t-window-titlebar');
                if ($content.attr('draggable') != undefined) {
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
                let width = $parent.width() || $('body').width();
                let left = (width - $window.width()) / 2;
                $window.css('left', left);
                let $overlay = null;
                if ($content.attr('modal') != undefined) {
                    //$overlay = $('<div class="t-overlay"></div>');
                    //$overlay.appendTo($parent);
                    $window.data('overlay', $overlay);
                }
                setTimeout(function () {
                    $window.addClass('window-animate');
                    if ($overlay)
                        $overlay.css('opacity', .5);
                    $window.css('top', 40 + $c.scrollTop);
                    setTimeout(() => $window.removeClass('window-animate'), 400);
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
                }, 25);
            }
        },
        bindWindow(dotnet, window) {
            $c.openWindow($(window).closest('.t-window'));
            const mutationObserver = new MutationObserver((mutationList) => {
                mutationList.forEach(mutation => {
                    if (mutation.type == 'attributes' && mutation.attributeName == 'status') {
                        let status = $(mutation.target).attr('status');
                        if (status == 1) {
                            let openWindowIsExist = false;
                            $('.t-widget.t-window').each((index, item) => {
                                if ($(item).find('.t-window-content').attr('status') == 2) {
                                    openWindowIsExist = true;
                                    return 0;
                                }
                            });
                            if (!openWindowIsExist)
                                $('.c-content-main').css('overflow', 'auto');
                        }
                        else
                            $('.c-content-main').css('overflow', 'hidden');
                        let $window = $(mutation.target).closest('.t-window');
                        $c.openWindow($window);
                    }
                });
            });
            mutationObserver.observe($(window)[0], {
                attributes: true,
                childList: false,
                subtree: false
            });
        },
        focus(input) {
            $(input).focus();
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
                            $(ddl).find('.t-dropdown-wrap').removeClass('t-state-hover').addClass('t-state-default');
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
            $(ddl).mouseenter(e => {
                $ddl = $(e.target).closest('.t-dropdown').find('.t-dropdown-wrap');
                if (!$ddl.hasClass('t-state-error') && !$ddl.hasClass('t-state-disabled'))
                    $ddl.removeClass('t-state-default').addClass('t-state-hover');
            });
            $(ddl).mouseleave(e => {
                $ddl = $(e.target).closest('.t-dropdown').find('.t-dropdown-wrap');
                if (!$ddl.hasClass('t-state-error'))
                    $ddl.removeClass('t-state-hover').addClass('t-state-default');
            });
            $(ddl).focusin(() => {
                $(ddl).find('.t-dropdown-wrap').removeClass('t-state-default').addClass('t-state-focused');
                $.caspian.showErrorMessage($(ddl)[0]);
            });
            $(ddl).focusout(() => {
                $(ddl).find('.t-dropdown-wrap').removeClass('t-state-focused').addClass('t-state-default');
                $.caspian.hideErrorMessage($(ddl)[0]);
            });
            $(window).bind('mouseup', e => {
                if (!$(e.target).hasClass('t-disable') || !$(e.target).closest('.t-dropdown').hasClass('t-dropdown')) {
                    let $group = $(ddl).find('.c-animate-down .t-group');
                    if ($(ddl).find('.t-group')[0]) {
                        $group.css('top', '-100%');
                        $group = $(ddl).find('.c-animate-up .t-group');
                        $group.css('bottom', '-100%');
                        setTimeout(async () => await dotnetHelper.invokeMethodAsync('CloseWindow'), 200);
                    }
                }
            });
        },
        bindContextMenu(dotnet, elem) {
            const mutationObserver = new MutationObserver((list) => {
                list.every(t => {
                    if (t.addedNodes.length == 1) {
                        let $ctr = $(t.addedNodes[0]);
                        let right = $ctr.parent().offset().left + $ctr.parent().outerWidth() + $ctr.outerWidth() + 10;
                        if (right < $(window).width())
                            $ctr.addClass('c-context-menu-left').removeClass('c-context-menu-right');
                        else
                            $ctr.addClass('c-context-menu-right').removeClass('c-context-menu-left');
                        setTimeout(() => $ctr.css('margin-top', '-28px'), 1);
                    }
                });
            });
            mutationObserver.observe(elem, {
                attributes: false,
                childList: true,
                subtree: true,
            });
            $(window).unbind("mousedown.ContextMenu");
            $(window).bind('mousedown.ContextMenu', async e => {
                if (!$(e.target).closest('.c-context-menu').hasClass('c-context-menu')) 
                    await dotnet.invokeMethodAsync("Close");
            });
        },
        bindComboBox(dotnetHelper, input, pageable) {
            if (pageable) {
                let div = $(input).closest('.t-combobox').find('.t-group');
                div.bind('scroll', async e => {
                    var height = $(this).scrollTop() + $(this).innerHeight();
                    if (height > $(this).find('.t-reset').height() + 1)
                        await dotnetHelper.invokeMethodAsync('IncPageNumberInvokable');
                });
            }
            const mutationObserver = new MutationObserver(() => {

                let $group = $(input).closest('.t-combobox').find('.t-group');
                let $animate = $(input).closest('.t-combobox').find('.t-animation-container');
                let height = $group.find('.t-reset').outerHeight();
                $(input).closest('.t-combobox').find('.t-item').click((e) => {
                    let $group = $(input).closest('.t-combobox').find('.c-animate-down .t-group');
                    $group.css('top', '-100%');
                    $group = $(input).closest('.t-combobox').find('.c-animate-up .t-group');
                    $group.css('bottom', '-100%');
                    $control.removeClass('t-state-hover').addClass('t-state-default');
                });
                if ($group.offset()) {
                    
                    if (height > 250)
                        height = 250;
                    if (height < 30)
                        height = 30;
                    let loc = $group.offset().top + height + 5;
                    if (loc > $(window).height()) {
                        $animate.addClass('c-animate-up').removeClass('c-animate-down');
                        $animate.height(height);
                        $group.css('bottom', 0);
                    }
                    else {
                        $animate.addClass('c-animate-down').removeClass('c-animate-up');
                        $animate.height(height + 7);
                        $group.css('top', 5);
                    }
                }
                if (!$(input).closest('.t-combobox').attr('error-message')) {
                    $.caspian.hideErrorMessage($(input).closest('.t-widget')[0]);
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
            $(input).keyup(e => {
                if (e.keyCode == 38 || e.keyCode == 40) {
                    let $selected = $(e.target).closest('.t-combobox').find('.t-item.t-state-selected');
                    if ($selected[0]) {
                        let $content = $(e.target).closest('.t-combobox').find('.t-popup.t-group');
                        let top = $selected.position().top, bottom = top + $selected.outerHeight();
                        if (bottom > 240 || top < 10) {
                            let scrollTop = $content.scrollTop();;
                            $content.scrollTop(scrollTop + bottom - 240);
                        }
                    }
                }
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
                $control.removeClass('t-state-default').addClass('t-state-focused');
                $.caspian.showErrorMessage($(input).closest('.t-widget')[0]);
            });
            $(input).focusout(() => {
                $control.removeClass('t-state-focused').addClass('t-state-default');
                $.caspian.hideErrorMessage($(input).closest('.t-widget')[0]);
            });
            $(window).bind('click', e => {
                if (!$(e.target).closest('.t-combobox').hasClass('t-combobox')) {
                    let $group = $(input).closest('.t-combobox').find('.c-animate-down .t-group');
                    $group.css('top', '-100%');
                    $group = $(input).closest('.t-combobox').find('.c-animate-up .t-group');
                    $group.css('bottom', '-100%');
                    setTimeout(async () => await dotnetHelper.invokeMethodAsync('CloseInvokable'), 200);
                }
            });
        },
        bindPopupWindow: function (dotnet, data, element, target) {
            let pos = JSON.parse(data);
            $(element).css('display', 'block');
            let className = $(element).attr('class');
            $(element).attr('class', 'auto-hide c-popup-window');
            let width = $(element).outerWidth();
            let height = $(element).outerHeight();
            let top = $('.c-main-head').height();
            let left = $('.sidebar').width();
            $(element).attr('class', className);
            if (target) {
                $(element).attr('class', 'auto-hide c-popup-window');
                let $target = $(target);
                var sum = 0;
                while ($target[0]) {
                    sum += $target.scrollTop();
                    $target = $target.parent();
                }
                let widthT = $(target).outerWidth();
                let heightT = $(target).outerHeight();
                let topT = $(target).offset().top + sum - 26;
                let leftT = $(target).offset().left - $('#mainmenu').outerWidth();
                let offsetLeft = pos.offsetLeft, offsetTop = pos.offsetTop;
                switch (pos.targetHorizontalAnchor) {
                    case 1:
                        leftT += pos.offsetLeft - 1;
                        break;
                    case 2:
                        leftT += widthT / 2;
                        offsetLeft = 0;
                        break;
                    case 3:
                        leftT += widthT + pos.offsetLeft - 2;
                        break;
                }
                switch (pos.targetVerticalAnchor) {
                    case 1:
                        topT += pos.offsetTop - 1;
                        break;
                    case 2:
                        topT += heightT / 2;
                        offsetTop = 0;
                        break;
                    case 3:
                        topT += heightT + pos.offsetTop - 1;
                        break;

                }
                if (pos.horizontalAnchor == 2)
                    leftT -= width / 2 + pos.offsetLeft - offsetLeft;
                else if (pos.horizontalAnchor == 3)
                    leftT -= width - pos.offsetLeft - offsetLeft;
                if (pos.verticalAnchor == 2)
                    topT -= height / 2 + pos.offsetTop + offsetTop;
                else if (pos.verticalAnchor == 3)
                    topT -= height + pos.offsetTop + offsetTop - 1;
                $(element).css({
                    position: 'absolute',
                    top: topT,
                    left: leftT
                });
            }
            else {
                if (pos.left != undefined)
                    $(element).css({ left: pos.left + left, right: 'auto' });
                else if (pos.right != undefined)
                    $(element).css({ left: 'auto', right: pos.right });
                if (pos.top != undefined)
                    $(element).css({ top: pos.top + top, bottom: 'auto' });
                else if (pos.bottom != undefined)
                    $(element).css({ top: 'auto', bottom: pos.bottom });
            }
            $c.enableAutoHide(dotnet);
        },
        bindMultiSelect: function (element, json) {
            $(element).unbind('focus');
            $(element).bind('focus', function () {
                let message = $(element).attr('error-message');
                if (message) {
                    $.caspian.showErrorMessage(element, message);
                    $(element).find('.errorMessage').css('margin-top', '35px')
                }
            });
            $(element).unbind('blur');
            $(element).bind('blur', function () {
                $.caspian.hideErrorMessage(element);
            });
            if (json.focused) {
                $(element).focus();

            }
        },
        bindImage: async function (pic, imageStream) {
            if (imageStream) {
                const arrayBuffer = await imageStream.arrayBuffer();
                const blob = new Blob([arrayBuffer]);
                const url = URL.createObjectURL(blob);
                $(pic).css('background-image', 'url(' + url + ')');
            }
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
        bindStringbox: function (element, focuced) {
            if (focuced) {
                $(element).focus()
                setTimeout(() => {
                    $(element).select();
                }, 1);
            }
            $(element).bind('focus', () => {
                $(element).select();
                $.caspian.showErrorMessage($(element).closest('.t-widget')[0]);
            });
            $(element).bind('blur', () => {
                $.caspian.hideErrorMessage($(element).closest('.t-widget')[0]);
            });
        },

        bindControl: function (control, options) {
            options = JSON.parse(options);
            if (!$(control).data('tTextBox'))
                $(control).tTextBox(options);
            $(control).data('tTextBox').updateState(options);
        },
        blockUI: function () {
            $.blockUI({ message: '<img src="/Content/loading_big.gif" />', css: { backgroundColor: 'transparent', border: '1px none transparent' } });
        },
        unblockUI: function () {
            $.unblockUI();
        },
        bindWindowClick: function (dotnet) {
            $('body').unbind('click.windowClick');
            $('body').bind('click.windowClick', async function (e) {
                if (!$(e.target).closest('.auto-hide').hasClass('auto-hide'))
                    await dotnet.invokeMethodAsync('WindowClick')
            });
        },
        bindtomask: function (el, patern) {
            const pattern = patern,
                slots = new Set(el.dataset.slots || "_"),
                prev = (j => Array.from(pattern, (c, i) => slots.has(c) ? j = i + 1 : j))(0),
                first = [...pattern].findIndex(c => slots.has(c)),
                accept = new RegExp(el.dataset.accept || "\\d", "g"),
                clean = input => {
                    input = input.match(accept) || [];
                    return Array.from(pattern, c =>
                        input[0] === c || slots.has(c) ? input.shift() || c : c
                    );
                },
                format = () => {
                    const [i, j] = [el.selectionStart, el.selectionEnd].map(i => {
                        i = clean(el.value.slice(0, i)).findIndex(c => slots.has(c));
                        return i < 0 ? prev[prev.length - 1] : back ? prev[i - 1] || first : i;
                    });
                    el.value = clean(el.value).join``;
                    el.setSelectionRange(i, j);
                    back = false;
                };
            let back = false;
            el.addEventListener("keydown", (e) => back = e.key === "Backspace");
            el.addEventListener("input", format);
            el.addEventListener("focus", format);
            el.addEventListener("blur", () => el.value === pattern && (el.value = ""));
        }
    }
})(jQuery);
(function ($) {
    $.fn.getSelection = function () {
        let id = $(this).attr('id');
        let el = this[0];
        if (id)
            el = document.getElementById(id);
        el.focus();
        var start = 0, end = 0, normalizedValue, range, textInputRange, len, endRange;
        if (typeof el.selectionStart === "number" && typeof el.selectionEnd === "number") {

            start = el.selectionStart;
            //$('#qwe').text(start + '++' + Date().toString());
            end = el.selectionEnd;
        }
        else {
            range = document.selection.createRange();
            if (range && range.parentElement() === el) {
                len = el.value.length;
                normalizedValue = el.value.replace(/\r\n/g, "\n");
                textInputRange = el.createTextRange();
                textInputRange.moveToBookmark(range.getBookmark());
                endRange = el.createTextRange();
                endRange.collapse(false);
                if (textInputRange.compareEndPoints("StartToEnd", endRange) > -1) {
                    start = end = len;
                }
                else {
                    start = -textInputRange.moveStart("character", -len);
                    start += normalizedValue.slice(0, start).split("\n").length - 1;
                    if (textInputRange.compareEndPoints("EndToEnd", endRange) > -1) {
                        end = len;
                    } else {
                        end = -textInputRange.moveEnd("character", -len);
                        end += normalizedValue.slice(0, end).split("\n").length - 1;
                    }
                }
            }
        }

        return {
            start: start,
            end: end
        };
    };
    $.fn.setCursorPosition = function (start, end) {
        let input = this[0];
        if (input.setSelectionRange) {
            input.focus();
            input.setSelectionRange(start, end);
        } else
            if (input.createTextRange) {
                var range = input.createTextRange();
                range.collapse(true);
                range.moveEnd('character', selectionEnd);
                range.moveStart('character', selectionStart);
                range.select();
            }
    };
})(jQuery);