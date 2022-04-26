(function ($) {
    var $r = $.report, guid, hasFocuse;
    var rSubReport = function (element) {
        this.element = element;
        rControl.prototype.constructor.call(this);
        $(element).find('td').first().append('<span class="subreporticon"></span>');
        $(element).css('background-color', 'wheat');
        var obj = this;
        $(element).dblclick(function () {
            if (!hasFocuse)
                return;
            var width = $(obj.element).width();
            var data = $(this).data('pageData');
            if (!data)
                data = new Object();
            data.reportId = $('#ReportId').val();
            data.guid = guid;
            //data.width = $r.getWidth(width);

            //width += 350;
            //var url = '/learning/ReportEngin/ReportPrint/SubReportWindow';
            //var win = $.telerik.getWindow();
            //win.formContentUrl = null;
            //win.content('<iframe height="620" id="iframe__1" frameborder="0" style="margin:0px 0px 0px 0px" width="' + (width + 200)+ '" ></iframe>');
            //win.size(width + 210, 570);
            //win.title('زیرگزارش');
            //$.telerik.post(url, data, function (result) {
            //    win.center();
            //    win.open();
            //    document.getElementById('iframe__1').contentWindow.document.write(result);
            //    document.getElementById('iframe__1').contentWindow.document.close();
            //});
        });
    };
    rSubReport.prototype = Object.create(rControl.prototype);
    rSubReport.prototype.getElement = function () {
            
    }
    rSubReport.prototype.drop = function (x, y) {
        rControl.prototype.drop.call(this, x, y);
        var obj = this;
        //if (!guid)
        //    $.telerik.post('/Learning/ReportEngin/ReportPrint/GetGuid', null, function (result) {
        //        guid = result
        //    });
    }
    rSubReport.prototype.guid = function (item) {
        if (arguments.length == 0)
            return guid;
        guid = item;
    }
    rSubReport.prototype.border = function () {
        return null;
    }
    rSubReport.prototype.getData = function () {
        var data = rControl.prototype.getData.call(this);
        data.type = 5;
        data.subReportPage = $(this.element).data('pageData');
        data.guid = guid;
        return data;
    }
    rSubReport.prototype.setData = function (data) {
        $(this.element).data('pageData', data);
    }
    rSubReport.prototype.init = function (data) {
        rControl.prototype.init.call(this, data);
        this.setData(data.subReportPage);
    }
    rSubReport.prototype.focus = function () {
        rControl.prototype.focus.call(this);
        hasFocuse = true;
    }
    rSubReport.prototype.blur = function () {
        rControl.prototype.blur.call(this);
        hasFocuse = false;
    }

    $.fn.rSubReport = function () {
        var item = new rSubReport(this);
        item.controlType = report.controlKind.subReport;
        $(this).data('rSubReport', item);
        return item;
    }
})(jQuery);
