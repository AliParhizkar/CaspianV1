(function ($) {
    var $r = $.report;
    var rCheckBox = function (element) {
        this.element = element;
    };
    rCheckBox.prototype = {

    };
    $.fn.rCheckBox = function () {
        var item = new rCheckBox(this);
        $(this).data('rCheckBox', item);
        return item;
    }
})(jQuery);
