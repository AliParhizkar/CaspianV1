(function ($) {
    let $r = $.report, hasFocuse;
    let rChart = function (element) {
        this.element = element;
        rControl.prototype.constructor.call(this);
        $(element).find('td').first().append('<span class="charticon"></span>');
        $(element).css('background-color', 'wheat');
        let obj = this;
        $(element).dblclick(function () {
            if (!hasFocuse)
                return;
            let url = '/ReportEngin/ReportPrint/ChartWindow';

        });
    };
    rChart.prototype = Object.create(rControl.prototype);
    rChart.prototype.getElement = function () {
            
    }
    rChart.prototype.drop = function (x, y) {
        rControl.prototype.drop.call(this, x, y);
        ///باز کردن فرم برای انتخاب چارت
    }
    rChart.prototype.border = function () {

        ///باز کردن فرم برای انتخاب چارت
    }
    rChart.prototype.getData = function () {
        let data = rControl.prototype.getData.call(this);
        data.type = 5;
        data.subReportPage = $(this.element).data('pageData');
        data.guid = guid;
        return data;
    }
    rChart.prototype.setData = function (data) {
        $(this.element).data('pageData', data);
    }
    rChart.prototype.init = function (data) {
        rControl.prototype.init.call(this, data);
        this.setData(data.chart);
    }
    rChart.prototype.focus = function () {
        rControl.prototype.focus.call(this);
        hasFocuse = true;
    }
    rChart.prototype.blur = function () {
        rControl.prototype.blur.call(this);
        hasFocuse = false;
    }

    $.fn.rChart = function () {
        let item = new rChart(this);
        item.controlType = report.controlKind.chart;
        $(this).data('rChart', item);
        return item;
    }
})(jQuery);
