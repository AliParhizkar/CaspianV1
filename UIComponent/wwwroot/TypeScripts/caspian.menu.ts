namespace caspian {
    export class Accordion {
        el: HTMLElement;
        multiple: boolean;
        constructor(el: HTMLElement, multiple: boolean) {
            this.el = el;
            this.multiple = multiple || false;
            el.querySelectorAll('.submenu li').forEach(li => {
                (li as HTMLElement).onclick = e => {
                    el.querySelector('.submenu .selected').classList.remove('selected');
                    (e.target as HTMLElement).closest('li').classList.add('selected');
                }
            });
            el.querySelectorAll('.default .link').forEach(elem => {
                (elem as HTMLElement).onclick = e => {
                    this.setOpenMenusHeight();
                    let target = (e.target as HTMLElement).closest('.default') as HTMLElement;
                    let height = null;
                    if (!target.classList.contains('open')) {
                        let submenu = target.querySelector('.submenu') as HTMLElement;
                        submenu.style.height = 'auto';
                        height = submenu.getBoundingClientRect().height;
                        submenu.style.height = '0';
                    }

                    setTimeout(() => {
                        this.toggleSubmen(target, height);
                    }, 1)
                }
            })
        }

        setOpenMenusHeight() {
            this.el.querySelectorAll('.default.open').forEach(elem => {
                let open = elem.querySelector('.submenu') as HTMLElement;
                let height = open.getBoundingClientRect().height;
                open.style.height = `${height}px`;
            });
        }

        toggleSubmen(menu: HTMLElement, height: number) {
            if (this.multiple) {

            }
            else {
                let opendMenu = this.el.querySelector('.default.open') as HTMLElement;
                if (opendMenu != menu)
                    this.openSubmenu(menu, height);
                if (opendMenu != null)
                    this.closeSubmenu(opendMenu);
            }
        }

        closeSubmenu(category: HTMLElement) {
            category.classList.remove('open');
            (category.querySelector('.submenu') as HTMLElement).style.height = '0';
        }
        openSubmenu(category: HTMLElement, height: number) {
            category.classList.add('open');
            let submenu = (category.querySelector('.submenu') as HTMLElement);
            submenu.style.height = `${height}px`;
        }
    }
}