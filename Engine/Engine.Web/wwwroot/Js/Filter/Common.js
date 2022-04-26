(function ($) {
    var $f = $.filtering = {
        getPage: function () {
            return $('#TabPanels').data('fPage');
        },
        mouseDown: false
    }
})(jQuery);
$.extend($.telerik.tabstrip.prototype,
    {
        bindTo: function (data) {
            data = eval('(' + data + ')');
            var $element = $(this.element);
            $element.find('ul .t-item').remove();
            $element.find('.t-content').remove();
            var obj = this;
            data.forEach(function (value) {
                $(obj.element).children().eq(0).find('.item-add').before('<li class="t-item t-state-default"><a class="t-link" href="#' + obj.element.id + '-' + value.id + '">' + value.title + '</a><span class="t-close"></span></li> ');
                $(obj.element).append('<div style="height:300px;" id="' + obj.element.id + '-' + value.id + '" class="t-content"><div class="PanelContent"></div></div>');
                obj.$contentElements = $(obj.element).find('> .t-content');
            });
            $(obj.element).find('.t-close').click(function () {
                var id = $(this).parent().find('a').attr('href');
                if ($(obj.element).find(id).find('.filterControl').length > 0) {
                    alert('پنل دارای کنترل می باشد و امکان حذف آن وجود ندارد.');
                }
                else {
                    var panels = $.filtering.getPage().getAllData();
                    if (panels.length > 1) {
                        if (confirm('آیا با حذف موافقید؟')) {
                            var tempData = [];
                            id = parseInt(id.split('-')[1]);
                            panels.forEach(function (item) {
                                if (item.id != id)
                                    tempData.push(item);
                            });
                            $('#TabPanels').data('tTabStrip').bindTo($.telerik.toJson(tempData));
                        }
                    } else
                        alert('گزارش باید حداقل دارای یک پنل باشد.');
                }
            });
            this.activateTab($element.find('ul li:not(.item-add)').last());
            var page = $('#TabPanels').fPage();
            data.forEach(function (value) {
                var $page = $(obj.element).find('#' + obj.element.id + '-' + value.id).find('.PanelContent');
                value.controls.forEach(function (value1) {
                    page.addControl(value1, $page);
                });
            });
        },
        getId: function () {
            var maxId = 0;
            $(this.element).find('ul li .t-link').each(function () {
                var id = parseInt($(this).attr('href').split('-')[1]);
                if (id > maxId)
                    maxId = id;
            });
            return maxId + 1;
        }
    });