(function ($) {
    var $t = $.telerik;
    $t.checkBox = function (element, options) {
        this.element = element;
        $.extend(this, options);
        $(element).bind({
            click: $.proxy(this.click, this),
            change: $.proxy(this.change, this)
        });
        $t.bind(this, {
            change: this.onChange
        });
    }

    $.fn.tCheckBox = function (options) {
        var q = $t.create(this, {
            name: 'tCheckBox',
            init: function (element, options) {
                return new $t.checkBox(element, options);
            },
            options: options
        });

        return q;
    };

    $t.checkBox.prototype = {
        change: function(e){

        },
        focus: function(){
            $(this.element).focus();
        },
        enable: function () {
            $(this.element).attr('disabled', null);
        },
        disable: function(){
            $(this.element).attr('disabled', "disabled");
        },
        value: function (value) {
            if (arguments.length == 0)
                return $(this.element).prop('checked');
            $(this.element).prop('checked', value);
        },
        updateState: function () {
            
        }
    }
})(jQuery);