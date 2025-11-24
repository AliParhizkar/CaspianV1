var caspian;
(function (caspian) {
    class Accordion {
        constructor(el, multiple) {
            this.el = el;
            this.multiple = multiple || false;
            el.querySelectorAll('.submenu li').forEach(li => {
                li.onclick = e => {
                    let selected = el.querySelector('.submenu .selected');
                    if (selected != null)
                        selected.classList.remove('selected');
                    e.target.classList.add('selected');
                };
            });
            el.querySelectorAll('.default .link').forEach(elem => {
                elem.onclick = e => {
                    this.setOpenMenusHeight();
                    let target = e.target.closest('.default');
                    let height = null;
                    if (!target.classList.contains('open')) {
                        let submenu = target.querySelector('.submenu');
                        submenu.style.height = 'auto';
                        height = submenu.getBoundingClientRect().height;
                        submenu.style.height = '0';
                    }
                    setTimeout(() => {
                        this.toggleSubmen(target, height);
                    }, 1);
                };
            });
        }
        setOpenMenusHeight() {
            this.el.querySelectorAll('.default.open').forEach(elem => {
                let open = elem.querySelector('.submenu');
                let height = open.getBoundingClientRect().height;
                open.style.height = `${height}px`;
            });
        }
        toggleSubmen(menu, height) {
            if (this.multiple) {
            }
            else {
                let opendMenu = this.el.querySelector('.default.open');
                if (opendMenu != menu)
                    this.openSubmenu(menu, height);
                if (opendMenu != null)
                    this.closeSubmenu(opendMenu);
            }
        }
        closeSubmenu(category) {
            category.classList.remove('open');
            category.querySelector('.submenu').style.height = '0';
        }
        openSubmenu(category, height) {
            category.classList.add('open');
            let submenu = category.querySelector('.submenu');
            submenu.style.height = `${height}px`;
        }
    }
    caspian.Accordion = Accordion;
})(caspian || (caspian = {}));
//# sourceMappingURL=caspian.menu.js.map