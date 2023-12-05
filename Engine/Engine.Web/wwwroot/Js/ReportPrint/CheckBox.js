(function ($) {
    let $r = $.report;
    let rCheckBox = function (element) {
        this.element = element;
    };
    rCheckBox.prototype = {

    };
    $.fn.rCheckBox = function () {
        let item = new rCheckBox(this);
        $(this).data('rCheckBox', item);
        return item;
    }
})(jQuery);
