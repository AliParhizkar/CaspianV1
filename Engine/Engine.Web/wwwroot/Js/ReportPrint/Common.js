(function ($) {
    let factor = 1000;
    let $r = $.report = {
        mouseDown: false,
        print: function (a1, a2, a3, a4, a5) {
            let str = a1;
            if (arguments.length > 1)
                str += ',' + a2;
            if (arguments.length > 2)
                str += ',' + a3;
            if (arguments.length > 3)
                str += ',' + a4;
            if (arguments.length > 4)
                str += ',' + a5;
            $('#lbl').text(str);
            
        },
        printf: function (txt) {
            let temp = $('#fortest1').text() + ',' + txt;
            if (temp.length > 150)
                temp = txt
            $('#fortest1').text(temp);
        },
        convertPtToPx: function (pt) {
            return pt * 1.33;
        },
        convertPxToPt: function (px) {
            return Math.round(px / 1.33);
        },
        updateColumnsData: function (count, margin, newPageAfter, newPageBefore) {
            $('body').data('rPage').getCurentControl().setColumnProperty(count, margin, newPageAfter, newPageBefore);
        },
        showLeftRuler: function (item1, item2) {
            let $body = $('body');
            let $item1 = $(item1), $item2 = $(item2);
            let left1 = Math.floor($item1.offset().left), left2 = Math.floor($item2.offset().left);
            if (Math.abs(left1 - left2) <= 1) {
                if (!$body.find('#leftRuler').hasClass('ruler'))
                    $body.append('<span id="leftRuler" class="ruler"></span>');
                $('#leftRuler').width(0).css('display', '').height($('#bond').height() + 40).css('left', left1)
                    .css('top', $('#bond').offset().top - 10);
            }
            else
                $('#leftRuler').css('display', 'none');
        },
        
        reportBind: function (dotNetObjectReference, data) {
            console.log(data);
            $('#cnvRuler').rRuler();
            $.report.dotNetObjectReference = dotNetObjectReference;
            let page = $('body').rPage(data);
            let toolBar = $(document).rToolsBar();
            let setting1 = {
                submenuLeftOffset: -1,
                onShow: function (e) {
                    let ctr = page.getCurentControl();
                    if (ctr && ctr.getContextMenu && ctr.getContextMenu() == 1) {
                        $('#page').css('cursor', 'default');
                        $('.bottomarrow').css('display', 'none');
                    }
                    else
                        return false;
                },
                autoHide: false,
                onSelect: function (e, context) {
                    let flag = $(this).attr('id');
                    let ctr = page.getCurentControl();
                    switch (flag) {
                        case '_1':
                            ctr.addColumnOnRight(); 
                            break;
                        case '_2':
                            ctr.addColumnOnLeft();
                            break;
                        case '_3':
                            ctr.removeColumn();
                            break;
                    }
                }
            };
            let setting2 = {
                submenuLeftOffset: -1,
                onShow: function (e) {
                    let ctr = page.getCurentControl();
                    if (ctr && ctr.getContextMenu && ctr.getContextMenu() == 2) {
                        $('#page').css('cursor', 'default');
                        $('.leftArrow').css('display', 'none');
                    }
                    else
                        return false;
                },
                autoHide: false,
                onSelect: function (e, context) {
                    let flag = $(this).attr('id');
                    let ctr = page.getCurentControl();
                    switch (flag) {
                        case '_1':
                            ctr.addRowOnTop();
                            break;
                        case '_2':
                            ctr.addRowOnBottom();
                            break;
                        case '_3':
                            ctr.removeRow();
                            break;
                    }
                }
            };
            $('#page').jeegoocontext("columnOperation", setting1);
            $('#page').jeegoocontext("rowOperation", setting2);
            let colorSetting = {
                strings: 'Context colors,Standard colors,Web colors,Context colors',
                showOn: 'none',
                hideButton: true,
                color: '#000000',
                history: false
            };
            $('#fontColorpicker').colorpicker(colorSetting).on('change.color', function (evt, color) {
                toolBar.updateCurentControlCSS('color', color);
            });
            $('#borderColorpicker').colorpicker(colorSetting).on('change.color', function (evt, color) {
                let ctr = $('body').data('rPage').getCurentControl();
                if (ctr) {
                    let border = ctr.border();
                    if (border && border.color) {
                        border.color.colorString = color;
                        ctr.border(border);
                    }
                }
            });
            colorSetting.color = '#FFFFFF';
            colorSetting.transparentColor = true;
            $('#contentColorpicker').colorpicker(colorSetting).on('change.color', function (evt, color) {
                let ctr = $('body').data('rPage').getCurentControl();
                if (ctr)
                    ctr.backGroundColor(color);
            });
            $('.borderwidthdiv').rDropDownList();
            $('.borderstylediv').rDropDownList();
            $('body').click(function (e) {
                let id = $(e.target).attr('id');
                if (id != '_14')
                    $('.toolsbar-dropdowndiv').first().fadeOut(500);
                if (id != '_15')
                    $('.toolsbar-dropdowndiv').last().fadeOut(500);
                if (id != '_16' && $('.borderstylediv').data('rDropDownList'))
                    $('.borderstylediv').data('rDropDownList').hide();
                if (id != '_17' && $('.borderwidthdiv').data('rDropDownList'))
                    $('.borderwidthdiv').data('rDropDownList').hide();
            });
            let data1 = [7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 28, 36, 42, 72];
            let data2 = ['sans-serif', 'B Nazanin', 'Tahoma', 'Times New Roman'];
            toolBar.createDropDownList($('.toolsbar-list-input')[0], data1);
            toolBar.createDropDownList($('.toolsbar-list-input')[1], data2);
        },
        addBonds(json) {
            json = JSON.parse(json);
            let page = $('body').data('rPage');
            page.size($.report.getPixelWidth(json.pageWidth), json.pageHeight);
            page.pageType = json.pageType;
            page.printOn = json.printOn;
            let bond = $('#bond').data('rBond');
            let data = bond.getData();
            if (json.reportTitle) {
                if (!$('#bond #reportTitle').is('#reportTitle')) {
                    let bondValue = new Object();
                    bondValue.bondType = 1;
                    bondValue.height = 1;
                    data = bond.addBond(data, bondValue);
                }
            } else {
                if ($('#bond #reportTitle').is('#reportTitle')) {
                    let count = $('#bond #reportTitle').find('.reportcontrol').length;
                    let str = count == 0 ? "آیا با حذف سرگزارش موافید؟" : "با حذف عنوان گزارش کنترلهای آن نیز حذف می شود آیا با حذف موافقید؟";
                    if (confirm(str))
                        data = bond.removeBond(data, 1);
                }
            }
            if (json.pageHeader) {
                if (!$('#bond #pageHeader').is('#pageHeader')) {
                    let bondValue = new Object();
                    bondValue.bondType = 2;
                    bondValue.height = 1;
                    data = bond.addBond(data, bondValue);
                }
            } else {
                if ($('#bond #pageHeader').is('#pageHeader')) {
                    let count = $('#bond #pageHeader .reportcontrol').length;
                    let str = count == 0 ? "آیا با حدف سرصفحه موافقید؟" : "با حذف سربرگ کنترلهای آن نیز حذف می شود آیا با حذف موافقید؟";
                    if (confirm(str))
                        data = bond.removeBond(data, 2);
                }
            }
            if (json.dataHeader) {
                if (!$('#bond #dataHeader').is('#dataHeader')) {
                    let bondValue = new Object();
                    bondValue.bondType = 3;
                    bondValue.height = 1;
                    data = bond.addBond(data, bondValue);
                }
            } else {
                if ($('#bond #dataHeader').is('#dataHeader')) {
                    let count = $('#bond #dataHeader .reportcontrol').length;
                    let str = count == 0 ? "آیا با حدف سرداده موافقید؟" : "با حذف سرداده کنترلهای آن نیز حذف می شود آیا با حذف موافقید؟";
                    if (confirm(str))
                        data = bond.removeBond(data, 3);
                }
            }

            if (json.dataFooter) {
                if (!$('#bond #dataFooter').is('#dataFooter')) {
                    let bondValue = new Object();
                    bondValue.bondType = 5;
                    bondValue.height = 1;
                    data = bond.addBond(data, bondValue);
                }
            } else {
                if ($('#bond #dataFooter').is('#dataFooter')) {
                    let count = $('#bond #dataFooter .reportcontrol').length;
                    let str = count == 0 ? "آیا با حذف ته داده موافقید؟" : "با حذف ته داده کنترلهای آن نیز حذف می شود آیا با حذف موافقید؟";
                    if (confirm(str))
                        data = bond.removeBond(data, 5);
                }
            }
            if (json.pageFooter) {
                if (!$('#bond #pageFooter').is('#pageFooter')) {
                    let bondValue = new Object();
                    bondValue.bondType = 6;
                    bondValue.height = 1;
                    data = bond.addBond(data, bondValue);
                }
            } else {
                if ($('#bond #pageFooter').is('#pageFooter')) {
                    let count = $('#bond #pageFooter .reportcontrol').length;
                    let str = count == 0 ? "آیا با حذف ته صفحه موافقید؟" : "با حذف ته برگ کنترلهای آن نیز حذف می شود آیا با حذف موافقید؟";
                    if (confirm(str))
                        data = bond.removeBond(data, 6);
                }
            }
            let width = $.report.getPixelWidth(json.pageWidth) - $('#bond .bond').first().width();
            data.forEach(function (bond, index) {
                if (bond.controls) {
                    bond.controls.forEach(function (control) {
                        control.position.left += $.report.getWidth(width);
                    });
                }
            });
            bond.createBond(data);
        },
        setFormat: function (format) {
            let curentControl = $('body').data('rPage').getCurentControl();
            curentControl.format(format);
        },
        setImage: function (img, data) {
            let curentControl = $('body').data('rPage').getCurentControl();
            curentControl.setImage(img, data);
        },
        updateTextWindowData: function (json) {
            let curentControl = $('body').data('rPage').getCurentControl();
            curentControl.member(json.member);
            curentControl.text(json.titleFa);
        },
        showRightRuler: function (item1, item2) {
            let $body = $('body');
            let $item1 = $(item1), $item2 = $(item2);
            let right1 = Math.floor($item1.offset().left) + Math.floor($item1.width());
            let right2 = Math.floor($item2.offset().left) + Math.floor($item2.width());
            if (right1 == right2) {
                if (!$body.find('#rightRuler').hasClass('ruler'))
                    $body.append('<span id="rightRuler" class="ruler"></span>');
                $('#rightRuler').css('display', '').width(0).height($('#bond').height() + 40).css('left', right1)
                    .css('top', $('#bond').offset().top - 10);
            }
            else
                $('#rightRuler').css('display', 'none');
        },
        showTopRuler: function (item1, item2) {
            let $body = $('body');
            let $item1 = $(item1), $item2 = $(item2);
            let top1 = Math.floor($item1.offset().top), top2 = Math.floor($item2.offset().top);
            if (top1 == top2) {
                if (!$body.find('#topRuler').hasClass('ruler'))
                    $body.append('<span id="topRuler" class="ruler"></span>');
                let $Ruler = $('#topRuler'), $bond = $('.bond').first();
                $Ruler.width($bond.width() - 20);
                $Ruler.height(0);
                $Ruler.css('display', '');
                $Ruler.css('left', $bond.offset().left + 10);
                $Ruler.css('top', top1);
            }
            else
                $('#topRuler').css('display', 'none');
        },
        getLength: function(len){
            len = len.substr(0, len.length - 2);
            return parseInt(len);
        },
        showBottomRuler: function (item1, item2) {
            let $body = $('body');
            let $item1 = $(item1), $item2 = $(item2);
            let bottom1 = Math.floor($item1.offset().top) + Math.floor($item1.height());
            let bottom2 = Math.floor($item2.offset().top) + Math.floor($item2.height());
            if (bottom1 == bottom2) {
                if (!$body.find('#bottomRuler').hasClass('ruler'))
                    $body.append('<span id="bottomRuler" class="ruler"></span>');
                let $Ruler = $('#bottomRuler'), $bond = $('.bond').first();
                $Ruler.width($bond.width() - 20);
                $Ruler.height(0);
                $Ruler.css('left', $bond.offset().left + 10);
                $Ruler.css('top', bottom1 - 1);
                $Ruler.css('display', '');
            }
            else
                $('#bottomRuler').css('display', 'none');
        },
        hideRuler: function (item) {
            if (item)
                $(item).css('display', 'none');
            else
                $('.ruler').css('display', 'none');
        },
        getWidth: function (width) {
            width = width * factor / 37.8;
            return Math.round(width) / factor;
        },
        getHeight: function (height) {
            height = height * factor / 37.8;
            return Math.round(height) / factor;
        },
        getPixelWidth: function (width) {
            return Math.round(width * 37.8);
        },
        getPixelHeight: function (height) {
            return Math.round(height * 37.8);
        },
        getTextBox: function(id, width, height) {
            return '<table class="reportcontrol" cellspacing="0" cellpadding="0" style="width:' + width + 'px;height:' + height + 'px;" id="' + id + '">' +
                '<tr><td style="vertical-align:top"><span class="topcell leftcell"></span><span class="topcell rightcell"></span></td></tr>' +
                '<tr><td style="vertical-align:bottom"><span class="bottomcell leftcell"></span><span class="bottomcell rightcell"></span>' +
                '</td></tr></table>';
        },
        getPictureBox: function (id, width, height) {
            return '<table class="reportcontrol" cellspacing="0" cellpadding="0" style="width:' + width + 'px; height:'+ height + 'px;" id="' + id + '">' +
                '<tr><td style="vertical-align: top;"><span class="topcell leftcell"></span><span class="topcell rightcell"></span></td></tr>' +
                '<tr><td style="vertical-align: bottom;"><span class="bottomcell leftcell"></span><span class="bottomcell rightcell"></span></td></tr></table';
        },
        getSubReport: function (id, width, height) {
            new rBorder()
            return '<table class="reportcontrol" cellspacing="0" cellpadding="0" style="width:' + width + 'px;height:' + height + 'px;" id="' + id + '">' +
            '<tr><td style="vertical-align:top"><span class="topcell leftcell"></span><span class="topcell rightcell"></span></td></tr>' +
            '<tr><td style="vertical-align:bottom"><span class="bottomcell leftcell"></span><span class="bottomcell rightcell"></span>' +
            '</td></tr></table>';
        },
        getChart: function (id, width, height) {
            new rBorder()
            return '<table class="reportcontrol" cellspacing="0" cellpadding="0" style="width:' + width + 'px;height:' + height + 'px;" id="' + id + '">' +
            '<tr><td style="vertical-align:top"><span class="topcell leftcell"></span><span class="topcell rightcell"></span></td></tr>' +
            '<tr><td style="vertical-align:bottom"><span class="bottomcell leftcell"></span><span class="bottomcell rightcell"></span>' +
            '</td></tr></table>';
        }
    }
})(jQuery);
let report = {
    controlKind: {
        none: 0,
        textBox: 1,
        pictureBox: 2,
        subReport: 3,
        table: 4,
        checkBox: 5,
        chart: 6,
        bond: 7
    }
}
borderStyleKind = {
    none: 0,
    solid: 1,
    dashed: 2,
    dotted: 3,
    double: 4
}
let rPosition = function () {

}
rPosition.prototype = {
    left: null,
    top: null,
    width: null,
    height: null,
    getPosition: function (element) {
        let pos = new rPosition();
        let $element = $(element), $bond = $element.closest('.bond');
        pos.left = $.report.getWidth($element.offset().left - $bond.offset().left);
        pos.top = $.report.getHeight($element.offset().top - $bond.offset().top);
        pos.width = $.report.getWidth($element.width());
        pos.height = $.report.getHeight($element.height());
        return pos;
    },
    initElement: function (element, data) {
        let $element = $(element), $bond = $element.closest('.bond');
        $element.css('left', $.report.getPixelWidth(data.position.left) + $bond.offset().left);
        $element.css('top', $.report.getPixelHeight(data.position.top) + $bond.offset().top);
        $element.width($.report.getPixelWidth(data.position.width));
        $element.height($.report.getPixelHeight(data.position.height));
    }
}

//مشخصات Border
let rBorder = function(element) {
    
};
rBorder.prototype = {
    /// <field name="borderKind" type="Number">عددی بین صفر تا 15 که مشخص می کند که آیا المان دارای هر کدام ار بردرهای بالا، چپ و ... است یا خیر</field>
    borderKind: 0,
    /// <field name="width" type="Number">پهنای بردر</field>
    width: 1,
    /// <field name="style" type="borderStyleKind">نوع بردر</field>
    style: borderStyleKind.solid,
    /// <field name="color" type="rColor">رنگ بردر</field>
    color: null,
    getBorder: function (element) {
        let borderStyle = 'solid';
        let borderWidth = '1px', color = '#000000', borderKind = 0;
        if ($(element).css('border-top-style') && $(element).css('border-top-style') != 'none' && $(element).css('border-top-width') != '0px') {
            borderStyle = $(element).css('border-top-style');
            borderWidth = $(element).css('border-top-width');
            color = $(element).css('border-top-color');
            borderKind |= 1;
        }
        if ($(element).css('border-bottom-style') && $(element).css('border-bottom-style') != 'none' && $(element).css('border-bottom-width') != '0px') {
            borderStyle = $(element).css('border-bottom-style');
            borderWidth = $(element).css('border-bottom-width');
            borderWidth = $(element).css('border-bottom-width');
            color = $(element).css('border-bottom-color');
            borderKind |= 2;
        }
        if ($(element).css('border-left-style') && $(element).css('border-left-style') != 'none' && $(element).css('border-left-width') != '0px') {
            borderStyle = $(element).css('border-left-style');
            borderWidth = $(element).css('border-left-width');
            color = $(element).css('border-left-color');
            borderKind |= 4;
        }
        if ($(element).css('border-right-style') && $(element).css('border-right-style') != 'none' && $(element).css('border-right-width') != '0px') {
            borderStyle = $(element).css('border-right-style');
            borderWidth = $(element).css('border-right-width');
            color = $(element).css('border-right-color');
            borderKind |= 8;
        }
        if (borderKind == 0)
            return null;
        let border = new rBorder();
        switch (borderStyle) {
            case 'solid':
                border.style = borderStyleKind.solid; break;
            case 'dashed':
                border.style = borderStyleKind.dashed; break;
            case 'dotted':
                border.style = borderStyleKind.dotted; break;
            case 'double':
                border.style = borderStyleKind.double; break;
        }
        border.color = new rColor();
        
        border.color.colorString = color;
        borderWidth = borderWidth.substr(0, borderWidth.length - 2);
        border.width = parseFloat(borderWidth);
        border.borderKind = borderKind;
        return border;
    },
    initElement: function (element) {
        ///   <summary>المان ورودی را با مشخصات کلاس مقدرادهی می کند</summary>
        ///   <param name="element" type="Element">المان ورودی</param>
        if (this.borderKind != 0) {
            let border = this.width + 'px' + ' ';
            switch (this.style) {
                case borderStyleKind.solid: border += 'solid'; break;
                case borderStyleKind.dashed: border += 'dashed'; break;
                case borderStyleKind.dotted: border += 'dotted'; break;
                case borderStyleKind.double: border += 'double'; break;
            }
            border += ' ' + this.color.colorString;
            (this.borderKind & 1) == 1 ? $(element).css('border-top', border) : $(element).css('border-top', 'none');
            (this.borderKind & 2) == 2 ? $(element).css('border-bottom', border) : $(element).css('border-bottom', 'none');
            (this.borderKind & 4) == 4 ? $(element).css('border-left', border) : $(element).css('border-left', 'none');
            (this.borderKind & 8) == 8 ? $(element).css('border-right', border) : $(element).css('border-right', 'none');
        } else {
            if ($(element).css('border-top-style') != 'none')
                $(element).css('border-top-style', 'none');
            if ($(element).css('border-right-style') != 'none')
                $(element).css('border-right-style', 'none');
            if ($(element).css('border-bottom-style') != 'none')
                $(element).css('border-bottom-style', 'none');
            if ($(element).css('border-left-style') != 'none')
                $(element).css('border-left-style', 'none');
        }
    },
    showAllBorder: function(){

    },
    anyBorder: function(flag){
        if (arguments.length == 0)
            return this.borderKind == 0;
        this.borderKind = 0;
    },
    allBorder: function(flag){
        if (arguments.length == 0)
            return this.borderKind == 15;
        this.borderKind = 15;
    },
    topBorder: function (flag) {
        if (arguments.length == 0)
            return (this.borderKind & 1) == 1;
        this.borderKind |= 1;
    },
    bottomBorder: function () {
        if (arguments.length == 0)
            return (this.borderKind & 2) == 2;
        this.borderKind |= 2;
    },
    leftBorder: function () {
        if (arguments.length == 0)
            return (this.borderKind & 4) == 4;
        this.borderKind |= 4;
    },
    rightBorder: function(){
        if (arguments.length == 0)
            return (this.borderKind & 8) == 8;
        this.borderKind |= 8;
    },
    toggelBorder: function () {
        if (this.borderKind == 0)
            this.borderKind = 15;
        else
            this.borderKind = 0;
    },
    toggelTopBorder: function () {
        if ((this.borderKind & 1) == 1)
            this.borderKind |= 14;
        else
            this.borderKind |= 1;
    },
    toggelBottomBorder: function () {
        if ((this.borderKind & 2) == 2)
            this.borderKind |= 13;
        else
            this.borderKind |= 13;
    },
    toggelLeftBorder: function () {
        if ((this.borderKind & 4) == 4)
            this.borderKind |= 11;
        else
            this.borderKind |= 11;
    },
    toggelRightBorder: function () {
        if ((this.borderKind & 8) == 8)
            this.borderKind |= 7;
        else
            this.borderKind |= 7;
    }
}

rFont = function () {
    
};
rFont.prototype = {
    bold: false,
    italic: false,
    underLine: false,
    family: '',
    size: 12,
    getFont: function (element) {
        ///   <summary>مشخصات فونت المان ورودی را یرمی گرداند</summary>
        ///   <param name="element" type="Element">المان ورودی</param>
        ///   <returns type="rFont" />
        let font = new rFont();
        font.bold = $(element).css('font-weight') == 700 || $(element).css('font-weight') == 'bold';
        font.family = $(element).css('font-family');
        if (font.family && (font.family.charAt(0) == "'" || font.family.charAt(0) == "\""))
            font.family = font.family.substr(1, font.family.length - 2);
        font.italic = $(element).css('font-style') == 'italic';
        let size = $(element).css('font-size');
        if (size)
            font.size = $.report.convertPxToPt(parseFloat(size.substr(0, size.length - 2)));
        font.underLine = $(element).css('text-decoration-line') == 'underline';
        return font;
    },
    initElement: function (element) {
        ///   <summary>المان ورودی را با مشخصات کلاس مقدرادهی می کند</summary>
        ///   <param name="element" type="Element">المان ورودی</param>
        this.bold ? $(element).css('font-weight', 700) : $(element).css('font-weight', 400);
        $(element).css('font-family', this.family);
        this.italic ? $(element).css('font-style', 'italic') : $(element).css('font-style', 'normal');
        $(element).css('font-size', this.size + 'pt');
        this.underLine ? $(element).css('text-decoration', 'underline') : $(element).css('text-decoration', 'none');
    }
}
//----------------------------rColor---------------------------
let rColor = function () {

}
rColor.prototype = {
    colorString: '#000000'
}
//--------------------------
let rAlign = function () {
    
}
rAlign.prototype = {
    /// <field name="horizontalAlign" type="horizontalAlignKind"></field>
    horizontalAlign: null,
    /// <field name="verticalAlign" type="verticalAlignKind"></field>
    verticalAlign: null,

    getAlign: function (element, controlType) {
        ///   <param name="element" type="Element">المان ورودی</param>
        ///   <param name="controlType" type="controlKind">نوع کنترل</param>
        ///   <returns type="rAlign" />
        let align = new rAlign();
        switch (controlType) {
            case report.controlKind.pictureBox:
                let array = $(element).css('background-position').split(' ');
                switch (array[0]) {
                    case '0%': align.horizontalAlign = horizontalAlignKind.left; break;
                    case '50%': align.horizontalAlign = horizontalAlignKind.center; break;
                    case '100%': align.horizontalAlign = horizontalAlignKind.right; break;
                }
                switch (array[1]) {
                    case '0%': align.verticalAlign = verticalAlignKind.top; break;
                    case '50%': align.verticalAlign = verticalAlignKind.middel; break;
                    case '100%': align.verticalAlign = verticalAlignKind.bottom; break;
                }
                break;
            case report.controlKind.textBox:
            case report.controlKind.table:
                let str = $(element).css('text-align');
                if (!str)
                    str = 'right';
                switch (str) {
                    case 'justify': align.horizontalAlign = horizontalAlignKind.justify; break;
                    case 'right': align.horizontalAlign = horizontalAlignKind.right; break;
                    case 'center': align.horizontalAlign = horizontalAlignKind.center; break;
                    case 'left': align.horizontalAlign = horizontalAlignKind.left; break;
                }
                str = $(element).css('vertical-align');
                if (!str)
                    str = 'middel';
                switch (str) {
                    case 'bottom': align.verticalAlign = verticalAlignKind.bottom; break;
                    case 'middle': align.verticalAlign = verticalAlignKind.middel; break;
                    case 'top': align.verticalAlign = verticalAlignKind.top; break;
                }
                break;
        }
        return align;
    },
    initElement: function (element, controlType) {
        ///   <param name="element" type="Element">المان ورودی</param>
        ///   <param name="controlType" type="controlKind">نوع کنترل</param>
        
        switch (controlType) {
            case report.controlKind.textBox:
            case report.controlKind.table:
                switch (this.horizontalAlign) {
                    case horizontalAlignKind.justify: $(element).css('text-align', 'justify'); break;
                    case horizontalAlignKind.right: $(element).css('text-align', 'right'); break;
                    case horizontalAlignKind.center: $(element).css('text-align', 'center'); break;
                    case horizontalAlignKind.left: $(element).css('text-align', 'left'); break;
                }
                switch (this.verticalAlign) {
                    case verticalAlignKind.top: $(element).css('vertical-align', 'top'); break;
                    case verticalAlignKind.middel: $(element).css('vertical-align', 'middle'); break;
                    case verticalAlignKind.bottom: $(element).css('vertical-align', 'bottom'); break;
                }
                break;
            case report.controlKind.pictureBox:
                let str = ""
                switch (this.horizontalAlign) {
                    case horizontalAlignKind.left: str = '0%'; break;
                    case horizontalAlignKind.center: str = '50%'; break;
                    case horizontalAlignKind.right: str = '100%'; break;
                }
                str += '';
                switch (this.verticalAlign) {
                    case verticalAlignKind.top: str += '0%'; break;
                    case verticalAlignKind.middel: str += '50%'; break;
                    case verticalAlignKind.bottom: str += '100%'; break;
                }
                $(element).css('background-position', str);
                break;
        }
    }
}
horizontalAlignKind = {
    justify: 1,
    right: 2,
    center: 3,
    left: 4
}
verticalAlignKind = {
    bottom: 1,
    middel: 2,
    top: 3
}
cellKind = {
    columnHeader:1,
    rowHeader: 2,
    normal: 3
}

$.myExtend = function (target, other) {
    if (target == null || target == undefined)
        target = new Object();
    for (let key in other) {
        if (other[key] != null && (typeof other[key]) == 'object') {
            let temp = new Object();
            $.myExtend(temp, other[key]);
            target[key] = temp;
        }
        else
            if ((typeof other[key]) != 'function') {
                target[key] = other[key];
            }
    }
}
