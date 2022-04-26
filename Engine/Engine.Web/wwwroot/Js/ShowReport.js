$(document).ready(function () {
    function getReportValue() {
        var obj = new Object();
        var array = [];
        $('body').find('fieldset').each(function () {
            var item = $(this).data('tReportControl');
            if (item) {
                var value = item.value();
                if (value)
                    array.push(value);
            }
        });
        obj.Values = array;
        obj.ReportId = $('#ReportId').val();
        obj.OrderBy1 = $('#OrderBy1').getComboBox().value();
        obj.OrderBy2 = $('#OrderBy2').getComboBox().value();
        obj.DescOrderBy1 = $('#DescOrderBy1').getCheckBox().value();
        obj.DescOrderBy2 = $('#DescOrderBy2').getCheckBox().value();
        return obj;
    }

    $('#showReportButton .dropdown').mouseenter(function () {
        $(this).addClass('hover');
    });
    $('#showReportButton .dropdown').mouseout(function () {
        $(this).removeClass('hover');
    });

    $('#showReportButton .dropdown').mousedown(function () {
        $(this).addClass('click');
        $('#reportContextMenu').css('left', $(this).offset().left - 75).css('top', $(this).offset().top - 247);
        $('#reportContextMenu').fadeIn(500);
    });
    $('#showReportButton .dropdown').mouseup(function () {
        $(this).removeClass('click');
    });
    $('#showReportButton .middel,#showReportButton .search').mouseenter(function () {
        $('#showReportButton .search').addClass('hover');
        $('#showReportButton .middel').addClass('hover');
        $('#showReportButton .dropdown').addClass('hover');
    });
    $('#showReportButton .middel,#showReportButton .search').mouseleave(function () {
        $('#showReportButton .search').removeClass('hover');
        $('#showReportButton .middel').removeClass('hover');
        $('#showReportButton .dropdown').removeClass('hover');
    });
    $('#showReportButton .middel,#showReportButton .search').mousedown(function () {
        $('#showReportButton .search').addClass('click');
        $('#showReportButton .middel').addClass('click');
        $('#showReportButton .dropdown').addClass('click');
    });
    $('#showReportButton .middel,#showReportButton .search').click(function () {
        var obj = getReportValue();
        $('body').block({ message: '<h>لطفا منتظر بمانید..</h>' });
        $.telerik.post(searchUrl, obj, function (result) {
            var iframe = document.getElementById('reportPrint');
            document.getElementById('reportPrint').contentWindow.document.write(result);
            var body = $('#reportPrint').contents().find('body');
            var width = body.find('table').first().width();
            body.css('margin', '10 auto');
            body.css('border', '2px solid black');
            body.width(width);
            iframe.contentWindow.document.close();
        });
    });
    $('#showReportButton .middel,#showReportButton .search').mouseup(function () {
        $('#showReportButton .search').removeClass('click');
        $('#showReportButton .middel').removeClass('click');
        $('#showReportButton .dropdown').removeClass('click');
    });
    $('body').unbind('click.showReportButtonHidden');
    $('body').bind('click.showReportButtonHidden', function (e) {
        if (!$(e.target).closest('#reportContextMenu').is('#reportContextMenu') && !$(e.target).hasClass('dropdown'))
            $('#reportContextMenu').fadeOut(500);
    });
    $('#reportContextMenu div').hover(function () {
        $('#reportContextMenu div').css('background-color', '');
        $(this).css('background-color', 'blue')
    });
    $('#reportContextMenu div').click(function () {
        $('#reportContextMenu').fadeOut(500);
        var index = $('#reportContextMenu div').index($(this));
        switch (index) {
            case 0:
                let obj = getReportValue();
                var url = pdfUrl + '?param=' + encodeURI(JSON.stringify(obj));
                $(this).find('a').attr('href', url);
                break;
            case 1:
                var tabStrip = top.$('#a111').data('tTabStrip');
                let obj = getReportValue();
                tabStrip.addOrOpen(pdfUrl, obj, 'چاپ گزارش');
                break;
            case 2:
                var obj = getReportValue();
                $('body').block({ message: '<h>لطفا منتظر بمانید..</h>' });
                $.telerik.post(searchUrl, obj, function (result) {
                    var iframe = document.getElementById('reportPrint');
                    document.getElementById('reportPrint').contentWindow.document.write(result);
                    var body = $('#reportPrint').contents().find('body');
                    var width = body.find('table').first().width();
                    body.css('margin', '10 auto');
                    body.css('border', '2px solid black');
                    body.width(width);
                    iframe.contentWindow.document.close();
                    window.frames["reportPrint"].focus();
                    window.frames["reportPrint"].print();
                });
                break;
            case 3:
                downLoadReport(excelUrl);
                break;
            case 4:
                downLoadReport(wordUrl);
                break;
            case 5:
                downLoadReport(textUrl);
                break;
            case 6:
                var tabStrip = top.$('#a111').data('tTabStrip');
                var obj = getReportValue();
                tabStrip.addOrOpen(pngUrl, obj, 'چاپ گزارش');
                break;
            case 7:
                var tabStrip = top.$('#a111').data('tTabStrip');
                var obj = getReportValue();
                tabStrip.addOrOpen(jpegUrl, obj, 'چاپ گزارش');
                break;
        }
    });
    function downLoadReport(url) {
        var data = $.telerik.toJson(getReportValue());
        $('#formForDownload').find('input').val(data);
        $('#formForDownload').attr('action', url);
        $('#formForDownload').submit();
    }
});