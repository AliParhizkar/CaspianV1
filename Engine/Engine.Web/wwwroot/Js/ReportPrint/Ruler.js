/// <reference path="Common.js" />
(function ($) {
    var $r = $.report, $, canvas;
    function createRuler () {
        canvas = document.getElementById('cnvRuler').getContext('2d');
        const width = document.getElementById('ruler').offsetWidth - 12;
        canvas.canvas.width = width;
        canvas.canvas.height = 24;
        canvas.fillStyle = "#999";
        canvas.shadowColor = "#f00"
        canvas.lineWidth = 100;
        const space = 3.77;
        for (var counter = 1; counter < (width / space); counter++) {
            let height = 8;
            if (counter % 10 == 0) {
                height = 18;
                canvas.fillText(counter / 10, counter * space + 1, 20);
            }
            else if (counter % 5 == 0)
                height = 12;
            canvas.fillRect(counter * space, 0, 1, height);
        }
    };
    var rRuler = function (element) {
        createRuler();
    };
    rRuler.prototype = {
        showStatus: function (start, end) {
            canvas.fillStyle = "blue";
            canvas.fillRect(start - 182, 0, 1, 20);
            if (arguments.length == 2)
                canvas.fillRect(end - 182, 0, 1, 20);
        },
        hideStatus() {
            createRuler();
        }

    };
    $.fn.rRuler = function () {
        var item = new rRuler(this);
        $(this).data('rRuler', item);
        return item;
    }
}
)(jQuery);
