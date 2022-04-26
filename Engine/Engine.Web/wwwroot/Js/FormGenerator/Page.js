(function ($) {
    var $f = $.form, xStart, yStart, curentControl, mousePosition, ctrForCopy, pageHeight;
    function copyCtr() {
        ctrForCopy = curentControl;
    }
    function getControlForFocuse(x, y) {
        var controls = [];
        $('#page').find('.reportcontrol').each(function () {
            var left = $(this).offset().left, top = $(this).offset().top, width = $(this).width(),
                height = $(this).height();
            var bottom = top + height, right = left + width;
            if (x > left && x < right && y > top && y < bottom)
                controls.push({width: width, height:height, id: $(this).attr('id')});
        });
        var $bond = $('#bond');
        
        if (controls.length == 0) {
            var left = $bond.offset().left, top = $bond.offset().top;
            if (x > left && x < left + $bond.width() && y > top && y < top + $bond.height())
                return $('#bond').data('fBond');
            return null;
        }
        
        var ctr = controls[0];
        for (var i = 1; i < controls.length; i++) {
            var flag = $('#' + controls[0].id).data('fPanel') != null;
            if (flag || controls[i].width <= ctr.width && controls[i].height <= ctr.height)
                ctr = controls[i];
        }
        
        var $temp = $('#' + ctr.id);
        var controlTypes = ['fCheckBox', 'fTextBox', 'fCheckListBox', 'fDropdownList', 'fLabel', 'fPanel'];
        var temp = null;
        for (var i = 0; i < controlTypes.length; i++) {
            temp = $temp.data(controlTypes[i]);
            if (temp) 
                break;
        }
        if (temp == null)
            alert('نیاز به تغییرات دارد');
        return temp;
    }
    function isReportContrl(ctr) {
        return $(ctr).closest('.reportcontrol').hasClass('reportcontrol') || $(ctr).closest('.bond').hasClass('bond') ||
            $(ctr).closest('.spliter').hasClass('spliter') || $(ctr).closest('.dataBond').hasClass('dataBond');
    }
    function initDataBind(self, data) {
        self.bond = $('#bond').fBond(data);
        self.toolsBox = $('#toolsBox').fToolsBox();
        curentControl = self.bond;
    }

    var fPage = function (element, data) {
        this.element = element;
        mousePosition = new Object();
        var thisObj = this;
        $(element).bind('keydown', function (e) { thisObj.keyDown(e); });
        $(element).bind('mousedown', function (e) { thisObj.mouseDown(e); });
        $(element).bind('mouseup', function (e) { thisObj.mouseUp(e); });
        $(element).bind('mousemove', function (e) { thisObj.mouseMove(e); });
        $(element).bind('click', function (e) { thisObj.mouseClick(e); });
    };
    fPage.prototype = {
        size: function(width, height){
            if (arguments.length > 0) {
                $('#bond').data('fBond').width(width);
                pageHeight = height;
            } else {
                var size = new Object();
                size.width = $('#bond').data('fBond').width();
                size.height = pageHeight;
                return size;
            }
        },
        mouseDown: function (e) {
            if (e.button == 2)
                return;
            $f.mouseDown = true;
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
                        $.form.print(tempStatus);
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
                    if (/*!$.telerik.getWindow().isOpened && */key == 46) {
                        if (confirm("آیا با حذف موافقید؟")) {
                            var ctrType = curentControl.controlType;
                            if ([1, 2, 3, 4, 5, 6].indexOf(ctrType) >= 0)
                                $(curentControl.element).remove();
                            this.resetCurentControl();
                        }
                    }
                    else
                        if (curentControl.move) {
                            curentControl.move(hChange, vChange);
                            $('#bond').data('fBond').updateBondHeight(curentControl.element);
                        }
                } else {
                    if (curentControl.change) {
                        curentControl.change(hChange, vChange)
                        $('#bond').data('fBond').updateBondHeight(curentControl.element);
                    }
                }
            }
        },
        mouseMove: function (e) {
            var x = e.clientX, y = e.clientY;
            mousePosition.x = x;
            mousePosition.y = y;
            if (this.toolsBox.controlType != controlKind.none) {
                xStart = 0;
                yStart = 0;
                var temp = this.toolsBox.createAndDrag(x, y);
                if (temp != null) 
                    curentControl = temp;
            }
            if (curentControl/* && !$.telerik.getWindow().isOpened*/) {
                if ($f.mouseDown == true) {
                    
                    if (curentControl.getControlType && curentControl.getControlType() == controlKind.bond || isReportContrl(e.target) || !curentControl.controlType)
                        curentControl.drag(x - xStart, y - yStart);
                }
                else
                    curentControl.updateCursor(x, y);
            }
        },
        mouseUp: function (e) {
            if (curentControl) {
                curentControl.drop(e);
                curentControl.focus();
                $(document).data('fToolsBar').update();
            }
            $f.mouseDown = false;
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
            $(document).data('fToolsBar').update();
        },
        disable: function () {
         
        },
        getPageProperty: function(){
            var obj = new Object();
            var size = this.size();
            obj.PageWidth = $.form.getWidth(size.width);
            obj.PrintOn = this.bond.printOn;
            obj.PageHeight = size.height;
            $.extend(obj, $('#bond').data('fBond').getBondProperty());
            return obj;
        },
        getPageData: function () {
            var obj = new Object();
            obj.id = $('#Id').val();
            obj.bond = $('#bond').data('fBond').getData();
            return obj;
        }
    };
    $.fn.fPage = function (data) {
        data = eval('(' + data + ')');
        var item = new fPage(this, data);
        $(this).data('fPage', item);
        
        initDataBind(item, data);
        item.bond.createBond(data.bond);
        
        return item;
    }
})(jQuery);