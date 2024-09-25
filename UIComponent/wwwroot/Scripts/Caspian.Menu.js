var caspian;
(function (caspian) {
    class Accordion {
        constructor(el, multiple) {
            this.el = el;
            this.multiple = multiple || false;
            el.querySelector('.submenu .selected').onclick = e => {
                e.target.classList.remove('selected');
                //$(this).addClass('selected');
            };
        }
    }
    caspian.Accordion = Accordion;
})(caspian || (caspian = {}));
//# sourceMappingURL=Caspian.Menu.js.map