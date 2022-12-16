/// <reference path="Common.js" />
(function ($) {
    var $r = $.report;
    var page = null;
    function getCurentElement() {
        page = $('body').data('rPage');
        var ctr = page.getCurentControl();
        if (ctr && ctr.getElement)
            return ctr.getElement();
        return null;
    }
    function getBorder() {
        var index = $('.borderwidthdiv').data('rDropDownList').getSelectedIndex(), border = new rBorder();
        border.borderKind = 0;
        if (page.getCurentControl().border() != null)
            border.borderKind = page.getCurentControl().border().borderKind;
        switch (index) {
            case 1: border.width = 1; break;
            case 2: border.width = 2; break;
            case 3: border.width = 3; break;
            case 4: border.width = 4; break;
            case 5: border.width = 5; break;
            case 6: border.width = 6; break;
            case 7: border.width = 7; break;
            case 8: border.width = 8; break;
        }
        index = $('.borderstylediv').data('rDropDownList').getSelectedIndex();
        switch (index) {
            case 1: border.style = borderStyleKind.solid; break;
            case 2: border.style = borderStyleKind.dashed; break;
            case 3: border.style = borderStyleKind.dotted; break;
            case 4: border.style = borderStyleKind.double; break;
        }
        border.color = new rColor();
        border.color.colorString = $('#borderColorpicker').colorpicker("val");
        return border;
    }
    function enable(controlsId) {
        for (var i = 1; i < 32; i++) {
            var $ctr = $('.toolsbar#_' + i);
            var str = $ctr.css('background-position').split(' ')[0] + ' 0px';
            $ctr.css('background-position', str);
        }
        for (var i = 0; i < controlsId.length; i++) {
            var $ctr = $('.toolsbar#' + controlsId[i]);
            var str = $ctr.css('background-position').split(' ')[0] + ' -27px';
            $ctr.css('background-position', str);
            if (controlsId[i] == '_14') {
                var size = $(getCurentElement()).first().css('font-size');
                $(getCurentElement()).each(function () {
                    if ($(this).css('font-size') != size)
                        size = null;
                });
                if (size) {
                    size = size.substring(0, size.length - 2);
                    $ctr.parent().find('input').val($r.convertPxToPt(size));
                }
                else
                    $ctr.parent().find('input').val('');
            }
            if (controlsId[i] == '_15') {
                var ctr = page.getCurentControl();
                if (ctr && ctr.font) {
                    var family = ctr.font().family;
                    $ctr.parent().find('input').val(family);
                }
            }
        }
    }
    function selecte(controlsId) {
        for (var i = 0; i < controlsId.length; i++) {
            var $ctr = $('.toolsbar#' + controlsId[i]);
            var str = $ctr.css('background-position').split(' ')[0] + ' -81px';
            $ctr.css('background-position', str);
        }
    }
    function updateToolsBar() {
        var data = [];
        data.push('_34');
        data.push('_35');
        data.push('_36');
        data.push('_38');
        var ctr = page.getCurentControl();
        if (!ctr) {
            enable(data);
            return data;
        }
        data.push('_31');
        switch (ctr.controlType) {
            case report.controlKind.textBox:
                for (let i = 1; i < 29; i++)
                    data.push('_' + i);
                break;
            case report.controlKind.pictureBox:
                var stretch = ctr.stretch();
                for (let i = 4; i < 11; i++) {
                    if (i !== 7 && !stretch)
                        data.push('_' + i);
                }
                for (let i = 16; i < 29; i++)
                    data.push('_' + i);
                break;
            case report.controlKind.table:
                for (let i = 1; i < 29; i++)
                    data.push('_' + i);
                break;
            case report.controlKind.bond:
                for (let i = 16; i < 29; i++)
                    data.push('_' + i);
                break;
        }
        if (ctr.controlType === report.controlKind.table) {
            if (ctr.isUnmergeable())
                data.push('_29');
            if (ctr.isMergeable())
                data.push('_30');
        }
        enable(data);
        data = [];
        if (ctr.controlType === report.controlKind.textBox || ctr.controlType === report.controlKind.table) {
            var alignment = ctr.align();
            if (alignment) {
                if (alignment.horizontalAlign)
                    data.push('_' + (alignment.horizontalAlign + 6));
                if (alignment.verticalAlign)
                    data.push('_' + (alignment.verticalAlign + 3));
            }
            var font = ctr.font();
            if (font) {
                if (font.underLine)
                    data.push('_11');
                if (font.italic)
                    data.push('_12');
                if (font.bold)
                    data.push('_13');
            }
        }
        if (ctr.controlType === report.controlKind.textBox || ctr.controlType === report.controlKind.pictureBox || ctr.controlType === report.controlKind.bond || ctr.controlType == report.controlKind.table) {
            var border = ctr.border();
            if (!border)
                data.push('_23');
            else {
                if (border.borderKind == 15)
                    data.push('_24');
                if ((border.borderKind & 8) == 8)
                    data.push('_25');
                if ((border.borderKind & 4) == 4)
                    data.push('_26');
                if ((border.borderKind & 2) == 2)
                    data.push('_27');
                if ((border.borderKind & 1) == 1)
                    data.push('_28');
            }
        }
        if (ctr.controlType == report.controlKind.pictureBox) {
            var stretch = ctr.stretch();
            if (!stretch) {
                var alignment = ctr.align();
                if (alignment) {
                    if (alignment.verticalAlign)
                        data.push('_' + (alignment.verticalAlign + 3));
                    if (alignment.horizontalAlign)
                        data.push('_' + (alignment.horizontalAlign + 6));
                }
            }
        }
        selecte(data);
    }

    var rToolsBarIcon = function (element) {

    };
    rToolsBarIcon.prototype = {
        enable: function () {

        },
        disable: function () {

        },
        click: function () {

        }
    }

    var rToolsBar = function (element) {
        this.element = element;
        page = $('body').data('rPage');
        $('.toolsbar-list-input').mouseover(function () {
            var ctr = $(this).parent().find('.toolsbar');
            ctr.css('background-position', ctr.css('background-position').split(' ')[0] + ' -27px');
        });
        $('.toolsbar-list-input').mouseout(function () {
            var ctr = $(this).parent().find('.toolsbar');
            
            ctr.css('background-position', ctr.css('background-position').split(' ')[0] + ' -0px');
        });
        $('.toolsbar-list-input').mousedown(function () {
            var ctr = $(this).parent().find('.toolsbar');
            ctr.css('background-position', ctr.css('background-position').split(' ')[0] + ' -54px');
        });
        $('.toolsbar-list-input').click(function () {
            var ctr = $(this).parent().find('.toolsbar');
            ctr.css('background-position', ctr.css('background-position').split(' ')[0] + ' -27px');
        });
        $('.toolsbar').mouseover(function () {
            var str = $(this).css('background-position').split(' ')[1];
            if (str != '0px' && str != '0%') {
                var str = $(this).css('background-position').split(' ')[0] + ' -54px';
                $(this).css('background-position', str);
                var id = $(this).attr('id');
                if (id)
                    id = parseInt(id.substr(1));
                if (id == 14 || id == 15)
                    $(this).parent().find('input').css('border-color', '#dbce99');
                if (id == 2 || id == 18 || id == 20) {
                    var $next = $('#_' + (id + 1));
                    str = $next.css('background-position').split(' ')[0] + ' -54px';
                    $next.css('background-position', str);
                }
                if (id == 3 || id == 19 || id == 21) {
                    var $prev = $('#_' + (id - 1));
                    str = $prev.css('background-position').split(' ')[0] + ' -54px';
                    $prev.css('background-position', str);
                }
            }
        });
        $('.toolsbar').mouseout(function () {
            var str = $(this).css('background-position').split(' ')[1];
            if (str != '0px' && str != '0%') {
                str = $(this).css('background-position').split(' ')[0] + ' -27px';
                $(this).css('background-position', str);
                var id = $(this).attr('id');
                if (id == '_14' || id == '_15')
                    $(this).parent().find('input').css('border-color', '#B7B7B7');
                if (id == '_2' || id == '_18' || id == '_20') {
                    //str = $(this).next().css('background-position').split(' ')[0] + ' -27px';
                    //$(this).next().css('background-position', str);
                }
                if (id == '_3' || id == '_19' || id == '_21') {
                    //str = $(this).prev().css('background-position').split(' ')[0] + ' -27px';
                    //$(this).prev().css('background-position', str);
                }
                updateToolsBar();
            }
        });
        $('.toolsbar').mousedown(function () {
            if ($(this).css('background-position').split(' ')[1] != '0px') {
                var str = $(this).css('background-position').split(' ')[0] + ' -108px';
                $(this).css('background-position', str);
                if ($(this).attr('id') == '_14' || $(this).attr('id') == '_15')
                    $(this).parent().find('input').css('border-color', '#9a8f63');
            }
        });
        var obj = this;
        var data = ['_34', '_35', '_36', '_37', '_38'];
        enable(data);
        $('.toolsbar').click(async function (evt) {
            if ($(this).css('background-position').split(' ')[1] == '0px')
                return;
            var str = $(this).css('background-position').split(' ')[0] + ' -27px ';
            $(this).css('background-position', str);
            if ($(this).attr('id') == '_14' || $(this).attr('id') == '_15')
                $(this).parent().find('input').css('border-color', '#dbce99');
            var id = $(this).attr('id'), curentControl = page.getCurentControl();
            if (curentControl && id) {
                id = parseInt(id.substr(1)), ctr = getCurentElement(), ctrType = curentControl.controlType;
                let alignment = null, font = null, $ctr = null, val = null, t = null, border = null, win = null;
                switch (id) {
                    case 1:
                        break;
                    case 2:
                        evt.stopImmediatePropagation();
                        $('#fontColorpicker').colorpicker("showPalette");
                        break;
                    case 3:
                        curentControl.color($('#fontColorpicker').colorpicker("val"));
                        break;
                    case 4:
                        alignment = curentControl.align();
                        alignment.verticalAlign = verticalAlignKind.bottom;
                        curentControl.align(alignment);
                        break;
                    case 5:
                        alignment = curentControl.align();
                        alignment.verticalAlign = verticalAlignKind.middel;
                        curentControl.align(alignment);
                        break;
                    case 6:
                        alignment = curentControl.align();
                        alignment.verticalAlign = verticalAlignKind.top;
                        curentControl.align(alignment);
                        break;
                    case 7:
                        alignment = curentControl.align();
                        alignment.horizontalAlign = horizontalAlignKind.justify;
                        curentControl.align(alignment);
                        break;
                    case 8:
                        alignment = curentControl.align();
                        alignment.horizontalAlign = horizontalAlignKind.right;
                        curentControl.align(alignment);
                        break;
                    case 9:
                        alignment = curentControl.align();
                        alignment.horizontalAlign = horizontalAlignKind.center;
                        curentControl.align(alignment);
                        break;
                    case 10:
                        alignment = curentControl.align();
                        alignment.horizontalAlign = horizontalAlignKind.left;
                        curentControl.align(alignment);
                        break;
                    case 11:
                        font = curentControl.font();
                        font.underLine ? font.underLine = false : font.underLine = true;
                        curentControl.font(font);
                        break;
                    case 12:
                        font = curentControl.font();
                        font.italic ? font.italic = false : font.italic = true;
                        curentControl.font(font);
                        break;
                    case 13:
                        font = curentControl.font();
                        font.bold ? font.bold = false : font.bold = true;
                        curentControl.font(font);
                        break;
                    case 14:
                        $ctr = $('.toolsbar-dropdowndiv').first();
                        val = $(this).parent().find('input').first().val();
                        $ctr.find('span').each(function () {
                            if ($(this).text() == val) {
                                $(this).css('border', '1px solid #c2AA79');
                                $(this).css('background-image', 'url(/Areas/ReportEngin/Content/Images/ddlpan.png)');
                            } else {
                                $(this).css('border', '1px solid #fff');
                                $(this).css('background-image', '');
                            }
                        });
                        $ctr.css('display', '');
                        break;
                    case 15:
                        $ctr = $('.toolsbar-dropdowndiv').last();
                        $ctr.css('display', '');
                        val = $(this).parent().find('input').first().val();
                        $ctr.find('span').each(function () {
                            if ($(this).text() == val) {
                                $(this).css('border', '1px solid #c2AA79');
                                $(this).css('background-image', 'url(/Areas/ReportEngin/Content/Images/ddlpan.png)');
                            }else{
                                $(this).css('border', '1px solid #fff');
                                $(this).css('background-image', '');
                            }
                        });
                        break;
                    case 16:
                        t = $(this).offset();
                        var borderStyle = $('.borderstylediv').data('rDropDownList');
                        border = curentControl.border();
                        if (border)
                            borderStyle.selectedValue = curentControl.border().style - 1;
                        else
                            borderStyle.selectedValue = -1;
                        borderStyle.show(t.left - 50, t.top + 28);
                        borderStyle.change = function () {
                            border = getBorder();
                            curentControl.border(border);
                        };
                        break;
                    case 17:
                        t = $(this).offset();
                        var borderWidth = $('.borderwidthdiv').data('rDropDownList');
                        border = curentControl.border();
                        if (border)
                            borderWidth.selectedValue = curentControl.border().width - 1;
                        else
                            borderWidth.selectedValue = -1;
                        borderWidth.show(t.left - 50, t.top + 28);
                        borderWidth.change = function () {
                            border = getBorder();
                            curentControl.border(border);
                        };
                        break;
                    case 18:
                        evt.stopImmediatePropagation();
                        $('#borderColorpicker').colorpicker("showPalette");
                        break;
                    case 19:
                        border = getBorder();
                        curentControl.border(border);
                        break;
                    case 20:
                        evt.stopImmediatePropagation();
                        $('#contentColorpicker').colorpicker("showPalette");
                        break;
                    case 21:
                        curentControl.backGroundColor($('#contentColorpicker').colorpicker("val"));
                        break;
                    case 22:
                        if (curentControl && curentControl.format) {
                            var data = curentControl.format() || {};
                            $.report.dotNetObjectReference.invokeMethodAsync('ShowDigitsFormatWindow', data);
                        }
                        break;
                    case 23:
                        border = getBorder();
                        border.borderKind = 0;
                        curentControl.border(border);
                        break;
                    case 24:
                        border = getBorder();
                        border.borderKind = 15;
                        curentControl.border(border);
                        break;
                    case 25:
                        border = getBorder();
                        if ((border.borderKind & 8) == 8)
                            border.borderKind &= 7;
                        else
                            border.borderKind |= 8;
                        curentControl.border(border);
                        break;
                    case 26:
                        border = getBorder();
                        if ((border.borderKind & 4) == 4)
                            border.borderKind &= 11;
                        else
                            border.borderKind |= 4;
                        curentControl.border(border);
                        break;
                    case 27:
                        border = getBorder();
                        if ((border.borderKind & 2) == 2)
                            border.borderKind &= 13;
                        else
                            border.borderKind |= 2;
                        curentControl.border(border);
                        break;
                    case 28:
                        border = getBorder();
                        if ((border.borderKind & 1) == 1)
                            border.borderKind &= 14;
                        else
                            border.borderKind |= 1;
                        curentControl.border(border);
                        break;
                    case 29:
                        curentControl.unmerge();
                        break;
                    case 30:
                        curentControl.merge();
                        break;
                    case 31:
                        if (confirm("آیا با حذف موافقید؟")) {
                            var ctrType = curentControl.controlType;
                            if (ctrType === 1 || ctrType === 2 || ctrType === 3) {
                                id = $(curentControl.element).attr('id');
                                $(curentControl.element).remove();
                            }else{
                                var tbl = $(curentControl.element).data('rTable');
                                if (tbl) 
                                    $(curentControl.element).remove();
                            }
                            page.resetCurentControl();
                        }
                        break;
                    case 34:
                        page.resetCurentControl();
                        var data = page.getPageProperty();
                        $.report.dotNetObjectReference.invokeMethodAsync('ShowSettingWindow', data);
                        break;
                    case 35:
                        var reportId = $('#ReportId').val();
                        open(preViewUrl + '?reportid=' + reportId);
                        break;
                    case 36:

                        break;
                    case 38:
                        if (confirm("آیا با ثبت موافقید؟")) {
                            var data = page.getPageData();
                            if (page.isSubReport) {
                                var parentPage = parent.$('body').data('rPage').getCurentControl().setData(data);
                                parent.$.telerik.getWindow().close();
                            } else {
                                $.telerik.blockUI();
                                await $.report.dotNetObjectReference.invokeMethodAsync('SaveData', data);
                                $.telerik.showMessage("ثبت با موفقیت انجام شد.");
                                $.telerik.unblockUI();
                            }
                        }
                        break;
                }
                updateToolsBar();
            }
        });
        page.chageControl = function () {
            var border = page.getCurentControl().border();
            if (border) {
                if (border.color)
                    $('#borderColorpicker').colorpicker("val", border.color.colorString);
                if (border.style)
                    $('.borderstylediv').data('rDropDownList').selectedValue = border.style - 1;
            }
            updateToolsBar();
        }
    };
    rToolsBar.prototype = {
        update: function () {
            var data = updateToolsBar();
            //enable(data);
        },
        updateCurentControlCSS: function(key, value){
            var ctr = getCurentElement();
            if (ctr)
                $(ctr).css(key, value);
        },
        toggleCurentControlCSS: function (key, val1, val2) {
            var ctr = getCurentElement();
            if (ctr) {
                if ($(ctr).css(key) == val1)
                    $(ctr).css(key, val2);
                else
                    $(ctr).css(key, val1);
            }
        },
        createDropDownList: function (element, data) {
            var left = $(element).offset().left - 16, top = $(element).offset().top + $(element).height() + 3, width = $(element).width() + 15;
            var str = '<div dir="ltr" class="toolsbar-dropdowndiv" style="display:none;width:' + width + 'px;left:' + left + 'px;top:' + top + 'px">';
            for (var key in data)
                str += '<span class="toolsbar-dropdownspan">' + data[key] + '</span>';
            str += '</div>';
            $('body').append(str);
            var obj = this;
            var id = $(element).parent().find('.toolsbar').attr('id');
            $('.toolsbar-dropdowndiv').last().find('span').click(function () {
                if (id == '_14') {
                    var ctr = $('body').data('rPage').getCurentControl();
                    if (ctr){
                        var font = ctr.font();
                        font.size = $(this).text();
                        ctr.font(font);
                        $('.toolsbar-list-input').first().val($(this).text());
                    }
                }
                if (id == '_15') {
                    var ctr = $('body').data('rPage').getCurentControl();
                    if (ctr) {
                        var font = ctr.font();
                        font.family = $(this).text();
                        ctr.font(font);
                        $('.toolsbar-list-input').last().val($(this).text());
                    }
                }
            });
        }
    };
    $.fn.rToolsBar = function () {
        var item = new rToolsBar(this);
        $(this).data('rToolsBar', item);
        return item;
    }
})(jQuery);
