(function ($) {
    let $t = $.caspian;
   
    $t.textbox = function (element, options) {
        $(element).mouseenter(function () {
            $(this).parent().addClass('t-state-hover');
        });
        $(element).mouseleave(function () {
            $(this).parent().removeClass('t-state-hover');
        });
        if (this.maskedText)
            $(element).mask(this.maskedText);
        this.id = $(element).attr('id');
        $(element).unbind("keypress.input");
        $(element).bind("keypress.input", e => {
            let isValid = false, total = this.total, digits = this.digits, selection = $(element).getSelection();
            let code = e.keyCode;
            let value = $(element).val();
            if (code == 46 && digits) {
                let remain = value.length - selection.end;
                if (remain <= digits && value.indexOf('.') == -1)
                    isValid = true;
            }
            if (code >= 48 && code <= 57 || code == 13 || code == 45 && selection.start == 0 && value.substr(selection.end).indexOf('-') == -1)
                isValid = true;
            var pointIndex = value.indexOf('.'); 
            if (pointIndex >= 0 && selection.start == selection.end && selection.end > pointIndex && value.split('.')[1].length == digits)
                isValid = false;
            if (selection.start == 0 && selection.end == 0 && value.length > 0 && value[0] == '-' && code >= 48 && code <= 57)
                isValid = false;
            let len = value.replace('-', '').replace('.', '').length;
            if (len == total && selection.start == selection.end && code != 45 && code != 46) 
                isValid = false;
            if (!isValid && this.type != 'string')
                e.preventDefault();
        });
        $(element).blur(function (e) {
            $.caspian.hideErrorMessage($(element).closest('.t-widget')[0]);
        });
        $(element).focus(function () {
            setTimeout(() => {
                $(element).select();
            }, 100);
            $.caspian.showErrorMessage($(element).closest('.t-widget')[0]);
        });
    }

    $t.textbox.prototype = {
        updateState: function (options) {
            $.extend(this, options);
        }
    }
    $.fn.tTextBox = function (options) {
        return this.each(function () {
            let $element = $(this);
            if (!$element.data('tTextBox')) 
                $element.data('tTextBox', new $t.textbox(this));
        });
    };
})(jQuery);