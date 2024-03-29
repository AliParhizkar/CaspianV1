﻿(function ($) {
    var $C = $.caspian;
    $C.textArea = function (element, options) {
        this.element = element;
        $.extend(this, options);
    }

    $C.textArea.prototype = {
        updateState: function (data) {
            if (data.focused)
                this.focus();
        },
        value: function (val) {
            if (arguments.length === 0)
                return $(this.element).val();
            $(this.element).val(val);
            this.showRequired();
            this.hideRequired();
        },
        focus: function () {
            $(this.element).focus();
        },
        enable: function (flag) {
            if (arguments.length == 0)
                return $(this.element).attr('disabled') != "disabled";
            $(this.element).parent().removeClass('t-state-disabled');
            if (flag)
                $(this.element).attr('disabled', null);
            else
                $(this.element).attr('disabled', "disabled");
        },
        disable: function (flag) {
            if (arguments.length == 0)
                return $(this.element).attr('disabled') === "disabled";
            $(this.element).parent().addClass('t-state-disabled');
            if (flag)
                $(this.element).attr('disabled', true);
            else
                $(this.element).attr('disabled', null);
                
        }
    }

    $.fn.tTextArea = function (options) {
        if (!$(this).data('tTextArea'))
            $(this).data('tTextArea', new $C.textArea(this, options));
        return $(this).data('tTextArea');
    };
})(jQuery);