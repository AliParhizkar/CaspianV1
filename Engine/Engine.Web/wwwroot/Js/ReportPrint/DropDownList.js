(function ($) {
    var rDropDownList = function (element) {
        this.element = element;

        $element = $(element);
        var obj = this;
        this.selectedValue = 0;
        $(element).find('span').click(function () {
            obj.hide();
            obj.selectedValue = $(element).find('span').index($(this));
            if (obj.change)
                obj.change();
        });
    };

    rDropDownList.prototype = {
        show: function (left, top) {
            var className, length = $(this.element).find('span').length;
            if ($(this.element).hasClass('borderwidthdiv'))
                className = 'borderwidth';
            else
                className = 'borderstyle';
            for (var i = 1; i <= length; i++) {
                $(this.element).find('span').eq(i - 1).removeClass(className + 'selected' + i);
                $(this.element).find('span').eq(i - 1).addClass(className + i);
            }
            $(this.element).find('span').eq(this.selectedValue).removeClass(className + (this.selectedValue + 1));
            $(this.element).find('span').eq(this.selectedValue).addClass(className + 'selected' + (this.selectedValue + 1));
            $(this.element).css('left', left);
            $(this.element).css('top', top);
            $(this.element).css('display', '');
        },
        hide: function () {
            $(this.element).fadeOut(300)
        },
        selecte: function (index) {

        },
        getSelectedIndex: function () {
            return this.selectedValue + 1;
        }
    };
    $.fn.rDropDownList = function () {
        var item = new rDropDownList(this);
        $(this).data('rDropDownList', item);
        return item;
    }
})(jQuery);
