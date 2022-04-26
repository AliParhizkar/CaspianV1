$(function() {
    var Accordion = function (el, multiple) {
        this.el = el || {};
        this.multiple = multiple || false;
        el.find('.submenu li').click(function () {
            el.find('.submenu li').removeClass('selected');
            $(this).addClass('selected');
        });
        var links = this.el.find('.link');
        links.on('click', { el: this.el, multiple: this.multiple }, this.dropdown);
    };
    Accordion.prototype.dropdown = function (e) {
        var $el = e.data.el, $this = $(this), $next = $this.next();
        $next.slideToggle();
        $this.parent().toggleClass('open');
        if (!e.data.multiple) 
            $el.find('.submenu').not($next).slideUp().parent().removeClass('open');
    };	
	new Accordion($('#accordion'), false);
});