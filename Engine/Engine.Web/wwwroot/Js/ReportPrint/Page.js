(function ($) {
    var $r = $.report, xStart, yStart, curentControl, mousePosition, ctrForCopy, pageHeight;
    function copyCtr() {
        ctrForCopy = curentControl;
    }
    function pasteCtr() {
        if (!ctrForCopy) {
            alert('لطفا ابتدا یک کنترل را کپی کنید');
            return
        }
        if (ctrForCopy.controlType == report.controlKind.textBox) {
            var max = 0;
            $('#page .reportcontrol').each(function () {
                var id = $(this).attr('id');
                id = parseInt(id.substring(2, id.length));
                if (id > max)
                    max = id;
            });
            var $element = $(ctrForCopy.element), $bond = $element.closest('.bond'), width = $element.width(),
                height = $element.height(), id = 'id' + (max + 1), str;
            var bondOffset = $bond.offset();
            if (mousePosition.x <= bondOffset.left || mousePosition.y <= bondOffset.top || mousePosition.x >= bondOffset.left + $bond.width() ||
                mousePosition.y >= bondOffset.top + $bond.height()) {
                alert('لطفا موس را در موقعیت مناسب قرار دهید تا کنترل در آن موقعیت کپی شود')
            } else {
                var x = mousePosition.x - $bond.offset().left - width, y = mousePosition.y - $bond.offset().top;
                if (x < 1)
                    x = 1;
                $bond.append($r.getTextBox(id, width, height));
                ///////////////////////
                var ctr = $('#' + id).rTextBox();
                var data = ctrForCopy.getData();
                if (data.text)
                    data.text = decodeURIComponent(data.text);
                data.position.left = $r.getWidth(x);
                data.position.top = $r.getHeight(y);
                ctr.init(data);
                if (curentControl)
                    curentControl.blur();
                ctr.focus();
                curentControl = ctr;
                $('#bond').data('rBond').updateBondHeight(ctr.element);
            }
        }
    }
    function getControlForFocuse(x, y) {
        var controls = [];
        $('#page').find('.reportcontrol').each(function () {
            var left = $(this).offset().left, top = $(this).offset().top, width = $(this).width(),
                height = $(this).height();
            var bottom = top + height, right = left + width;
            var ctr = $(this).data('rTable'), margin = 5;
            if (ctr)
                margin = 0;
            if (x > left - margin && x < right + margin && y > top - margin && y < bottom + margin)
                controls.push({width: width, height:height, id: $(this).attr('id')});
        });
        var $bond = $('#bond');
        
        if (controls.length == 0) {
            var left = $bond.offset().left, top = $bond.offset().top;
            if (x > left && x < left + $bond.width() && y > top && y < top + $bond.height())
                return $('#bond').data('rBond');
            return null;
        }
        
        var ctr = controls[0];
        for (var i = 1; i < controls.length; i++)
            if (controls[i].width <= ctr.width && controls[i].height <= ctr.height)
                ctr = controls[i];
        var $temp = $('#' + ctr.id);
        var controlTypes = ['rCheckBox', 'rPictureBox', 'rSubReport', 'rTable', 'rTextBox', 'rChart'];
        var temp = null;
        for (var i = 0; i < controlTypes.length; i++) {
            temp = $temp.data(controlTypes[i]);
            if (temp)
                break;
        }
        var table = $('#' + ctr.id).data('rTable');
        if (table)
            return table;
        if (temp == null)
            alert('نیاز به تغییرات دارد');
        return temp;
    }
    function isReportContrl(ctr) {
        return $(ctr).closest('.reportcontrol').hasClass('reportcontrol') || $(ctr).closest('.bond').hasClass('bond') ||
            $(ctr).closest('.spliter').hasClass('spliter') || $(ctr).closest('.dataBond').hasClass('dataBond');
    }
    function initDataBind(self, data) {
        self.bond = $('#bond').rBond(data);
        self.toolsBox = $('#toolsBox').rToolsBox();
        $('#bond').data('rBond').width($r.getPixelWidth(data.width));
        if (data.isSubReport) {
            self.isSubReport = true;
            $('#bond').parent().height(290);
        }
        else
            $('#bond').parent().height(500);
        curentControl = self.bond;
    }

    var rPage = function (element, data) {
        this.element = element;
        mousePosition = new Object();
        var thisObj = this;
        $(element).bind('keydown', function (e) { thisObj.keyDown(e); });
        $(element).bind('mousedown', function (e) { thisObj.mouseDown(e); });
        $(element).bind('mouseup', function (e) { thisObj.mouseUp(e); });
        $(element).bind('mousemove', function (e) { thisObj.mouseMove(e); });
        $(element).bind('click', function (e) { thisObj.mouseClick(e); });
        pageHeight = data.height;
        this.LeftMargin = 0;
        this.report = data.report;
        this.RightMargin = 0;
        this.TopMargin = 0;
        this.isSubReport = data.IsSubReport;
        this.BottmMargin = 0;
        this.BackGroundColor = new Object();
        this.BackGroundColor.ColorString = 'transparent';
        this.Color = new Object();
        this.Color.ColorString = "#000000";
    };
    rPage.prototype = {
        size: function(width, height){
            if (arguments.length > 0) {
                $('#bond').data('rBond').width(width);
                pageHeight = height;
            } else {
                var size = new Object();
                size.width = $('#bond').data('rBond').width();
                size.height = pageHeight;
                return size;
            }
        },
        mouseDown: function (e) {
            if (e.button == 2)
                return;
            $r.mouseDown = true;
            xStart = e.clientX;
            yStart = e.clientY;
            if (isReportContrl(e.target)) {
                
                var newControl = getControlForFocuse(e.clientX, e.clientY);
                var flag = false;
                
                if (curentControl) {
                    if (curentControl == newControl)
                        curentControl.dragStart(xStart, yStart);
                    else {
                        flag = true, tempStatus = curentControl.getStatus();
                        curentControl.blur();
                        if (!tempStatus && newControl) {
                            newControl.focus();
                            newControl.dragStart(xStart, yStart);
                            curentControl = newControl;
                        }
                        else
                            curentControl.dragStart(xStart, yStart);
                    }
                }
                else {
                    newControl.focus();
                    curentControl = newControl;
                }
                if (flag && this.chageControl)
                    this.chageControl();
            }
        },
        keyDown: function (e) {
            var key = e.keyCode | e.which, type = 1, hChange, vChange;
            if (e.ctrlKey) 
                type = 2;
            switch (key) {
                case 37: hChange = -1; break;
                case 38: vChange = -1; break;
                case 39: hChange = 1; break;
                case 40: vChange = 1; break;
            }
            if (curentControl) {
                if (type == 1) {
                    if (key == 46){
                        if (confirm("آیا با حذف موافقید؟")) {
                            var ctrType = curentControl.controlType;
                            if (ctrType == 1 || ctrType == 2 || ctrType == 3) {
                                $(curentControl.element).remove();
                            } else {
                                var tbl = $(curentControl.element).data('rTable');
                                if (tbl)
                                    $(curentControl.element).remove();
                            }
                            this.resetCurentControl();
                        }
                    }
                    else
                        if (curentControl.move) {
                            curentControl.move(hChange, vChange);
                            $('#bond').data('rBond').updateBondHeight(curentControl.element);
                        }
                } else {
                    switch (key) {
                        case 67:
                            copyCtr();
                            break;
                        case 86:
                            pasteCtr();
                            break;
                    }
                    if (curentControl.change) {
                        curentControl.change(hChange, vChange)
                        $('#bond').data('rBond').updateBondHeight(curentControl.element);
                    }
                }
            }
        },
        mouseMove: function (e) {
            var x = e.clientX, y = e.clientY;
            mousePosition.x = x;
            mousePosition.y = y;
            if (this.toolsBox.controlType != report.controlKind.none) {
                xStart = 0;
                yStart = 0;
                var temp = this.toolsBox.createAndDrag(x, y);
                if (temp != null) 
                    curentControl = temp;
            }
            if (curentControl /*&& !$.telerik.getWindow().isOpened*/) {
                if ($r.mouseDown == true) {
                    
                    if (curentControl.getControlType && curentControl.getControlType() == report.controlKind.bond || isReportContrl(e.target) || !curentControl.controlType)
                        curentControl.drag(x - xStart, y - yStart);
                }
                else
                    curentControl.updateCursor(x, y);
            }
        },
        mouseUp: function (e) {
            if (curentControl) {
                curentControl.drop(e);
                let tollsBar = $(document).data('rToolsBar');
                if (toolbar)
                    tollsBar.update();
            }
            $r.mouseDown = false;
            this.bond.destroyAfterResize();
        },
        mouseClick: function (e) {
            //if (curentControl)
            //    curentControl.focus();
        },
        getCurentControl: function () {
            return curentControl;
        },
        resetCurentControl: function () {
            if (curentControl)
                curentControl.blur();
            curentControl = null;
            $(document).data('rToolsBar').update();
        },
        disable: function () {
         
        },
        getPageProperty: function(){
            var obj = new Object();
            var size = this.size();
            obj.PageWidth = $.report.getWidth(size.width);
            obj.PageHeight = size.height;
            obj.PrintOn = this.printOn;
            obj.PageType = this.pageType || 3;
            $.extend(obj, $('#bond').data('rBond').getBondProperty());
            return obj;
        },
        getPageData: function () {
            var obj = new Object();
            obj.width = $r.getWidth($('.bond').first().width());
            obj.height = pageHeight;
            obj.leftMargin = $r.getWidth(this.LeftMargin);
            obj.rightMargin = $r.getWidth(this.RightMargin);
            obj.topMargin = $r.getHeight(this.TopMargin);
            obj.bottmMargin = $r.getHeight(this.BottmMargin);
            obj.backGroundColor = this.BackGroundColor;
            obj.color = this.Color;
            obj.border = new Object();
            obj.border.borderKind = 0;
            obj.border.style = 1;
            obj.reportId = $('#ReportId').val();
            obj.border.color = new Object();
            obj.border.color.ColorString = 'rgb(0, 0, 0)';
            obj.bonds = $('#bond').data('rBond').getData();
            return obj;
        }
    };
    $.fn.rPage = function (data) {
        data = eval('(' + data + ')');
        var item = new rPage(this, data);
        item.isSubReport = data.isSubReport;
        $(this).data('rPage', item);
        initDataBind(item, data);
        item.bond.createBond(data.bonds);
        return item;
    }
})(jQuery);