(function ($) {
    var $t = $.telerik, _isOpend;
    $t.helpWindow = function (element, options) {
        _isOpend = false;
        this.element = element;
        $.extend(this, options);
        $t.bind(this, {
            send: this.onSend,
            recive: this.onRecive,
            complete: this.onComplete
        });
    }

    $t.helpWindow.prototype = {
        open: function () {
            _isOpend = true;
            $(this.element).css('display', 'block');
            if (this.textBox) {
                let offset = $(this.textBox.element).offset();
                let left = offset.left - ($(this.element).outerWidth() - $(this.textBox.element).parent().outerWidth()) / 2;
                $(this.element).css('left', left);
                $(this.element).css('display', 'block');
            }
        },

        controlInit: function () {
            var grid = $(this.element).find('.t-widget.t-grid').data('tGrid');
           
            if (grid) {
                var obj = grid.getSelectedObject();
                if (this.foreignKeySelectorId) {
                    var item = $('#' + this.foreignKeySelectorId).data('tReportControl');
                    item.addItem(obj)
                }
                else {
                    var item = $t.getItem(this.otherKeyName);
                    if (item.displayFields) {
                        item.keyValue = obj[item.otherKeyName];
                        item.type = "string";
                        item.value(obj[item.displayFields]);
                        item.UpdateContent();
                    } else {
                        item.value(obj[item.otherKeyName]);
                        item.focus();
                        $t.trigger(item.element, 'changeValue', { dataItem: obj[item.otherKeyName] });
                        item.updateDBLable();
                    }
                    if (this.onRowSelected)
                        this.onRowSelected(obj);
                    this.close();
                }
            }
        },

        isOpened: function(){
            return _isOpend;
        },

        close: function () {
            _isOpend = false;
            $(this.element).css('display', 'none');
            if (this.onClose)
                this.onClose();
        }
    }

    $.fn.tHelpWindow = function (options) {
        var q = $t.create(this, {
            name: 'tHelpWindow',
            init: function (element, options) {
                return new $t.helpWindow(element, options);
            },
            options: options        
        });
        
        return q;
    };

    $.fn.tHelpWindow.defaults = {
        submitToUrl:null
    }
})(jQuery);