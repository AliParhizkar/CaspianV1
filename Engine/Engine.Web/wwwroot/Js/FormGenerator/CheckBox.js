(function ($) {
    var $f = $.form;
    var fCheckBox = function (element) {
        this.element = element;
        fControl.prototype.constructor.call(this);
    };
    fCheckBox.prototype = {

    };
    fCheckBox.prototype.setDefaultName = function (name) {

    };
    fCheckBox.prototype.drag = function (x, y) {
        fControl.prototype.drag.call(this, x, y);
    };
    $.fn.fCheckBox = function () {
        var item = new fCheckBox(this);
        $(this).data('fCheckBox', item);
        return item;
    };
})(jQuery);
