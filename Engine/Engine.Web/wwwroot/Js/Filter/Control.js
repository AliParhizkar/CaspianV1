(function ($) {
    var xStart, yStart, status, leftStart, topStart, widthStart, heightStart
    var fControl = function (element, data) {
        this.element = element;
    }

    fControl.prototype = {
        focus: function(){
            $(this.element).css('border-style', 'dashed')
        },
        blur: function(){
            $(this.element).css('border-style', 'solid')
        },
        dragStart: function (x, y) {
            var $element = $(this.element);
            xStart = x;
            yStart = y;
            leftStart = $element.offset().left;
            topStart = $element.offset().top;
            widthStart = $element.width();
            heightStart = $element.height();
        },
        draging: function (x, y) {
            
            var $element = $(this.element), difX = x - xStart, difY = y - yStart;
            switch (status) {
                case statusKind.move:
                    $element.css('left', leftStart + difX);
                    $element.css('top', topStart + difY);
                    break;
                case statusKind.changeWidthFromRight:
                    $element.width(widthStart + difX);
                    break;
                case statusKind.changeWidthFromLeft:
                    $element.width(widthStart - difX);
                    $element.css('left', leftStart + widthStart - $element.width());
                    break;
                case statusKind.changeHeightFromBottom:
                    $element.height(heightStart + difY);
                    break;
            }
        },
        drop: function (x, y) {

        },
        getData: function () {
            var $element = $(this.element), offsetBase = $element.closest('.PanelContent').offset();
            var obj = new Object(), offset = $element.offset();
            var value = eval('(' + $element.find('input').val() + ')');
            obj.enTitle = value.enTitle;
            obj.ruleId = value.ruleId;
            obj.faTitle = $element.find('legend').text();
            obj.left = parseInt(offset.left) - parseInt(offsetBase.left) + 1;
            obj.top = parseInt(offset.top - offsetBase.top) + 3;
            obj.width = $element.width() + 2;
            obj.height = $element.height() + 2;
            obj.controlType = this.controlType;
            obj.fromValue = this.fromValue;
            obj.enumFields = this.enumFields;
            obj.toValue = this.toValue;
            var id = $element.attr('id');
            obj.id = parseInt(id.substring(2, id.length));
            obj.title = $element.find('legend').text();
            return obj;
        },
        setData: function(data){
            
        },
        move: function (hChange, vChange) {
            var $element = $(this.element), left = $element.offset().left, top = $element.offset().top;
            $element.css('left', left + hChange);
            $element.css('top', top + vChange);
        },
        change: function(hChange, vChange){
            var $element = $(this.element), left = $element.offset().left, width = $element.width(),
                height = $element.height();
            $element.width(width - hChange);
            $element.css('left', left + hChange);
            $element.height(height + vChange);
        },
        updateCursor: function (x, y) {
            var $element = $(this.element);
            var left = $element.offset().left, right = left + $element.outerWidth(), top = $element.offset().top - 3,
                bottom = top + $element.outerHeight();
            status = statusKind.none;
            if (y >= top && y <= bottom) {
                if (Math.abs(x - left) <= 6)
                    status = statusKind.changeWidthFromLeft;
                else
                    if (Math.abs(x - right) <= 6)
                        status = statusKind.changeWidthFromRight;
                    else
                        if (x >= left && x <= right)
                            status = statusKind.move;

            }
            else
                if (x >= left && x <= right)
                {
                    if (Math.abs(y - top) <= 6)
                        status = statusKind.changeHeightFromTop;
                    else
                        if (Math.abs(y - bottom) <= 6)
                            status = statusKind.changeHeightFromBottom;
                        else
                            if (y >= top && y <= bottom)
                                status = statusKind.move;
                }
            switch (status) {
                case statusKind.changeWidthFromLeft:
                case statusKind.changeWidthFromRight:
                    $('body').css('cursor', 'e-resize');
                    break;
                case statusKind.changeHeightFromBottom:
                case statusKind.changeHeightFromTop:
                    $('body').css('cursor', 's-resize');
                    break;
                case statusKind.move:
                    $('body').css('cursor', 'move');
                    break;
                case statusKind.none:
                    $('body').css('cursor', 'default');
                    break;
            }
        }
    }

    $.fn.fControl = function (data) {
        var item = new fControl(this, data);
        $(this).data('fControl', item);
        return item;
    }
}    
)(jQuery);
statusKind = {
    none:1,
    move:2,
    changeWidthFromLeft:3,
    changeWidthFromRight:4,
    changeHeightFromTop:5,
    changeHeightFromBottom:6
}
filterControlKind = {
    enums: 1,
    foreignKey: 2,
    fromTo: 3,
    boolean: 4
}
