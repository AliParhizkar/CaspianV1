namespace caspian {
    export class DataGrid {
        grid: HTMLElement;
        content: HTMLElement;
        resize: boolean;
        curent: HTMLElement;
        curentWidth: number;
        gridStatus: number;
        other: HTMLElement;
        otherWidth: number;
        xStart: number;

        constructor(grv: HTMLElement) {
            this.grid = grv;
            this.content = (grv.getElementsByClassName('t-grid-content')[0] as HTMLDivElement);
            if (this.content.classList.contains('t-inline-content'))
                this.bindObserver();
            this.bindResizeObserver();
            this.content.onscroll = e => {
                let target = e.target as HTMLElement;
                target.closest('.t-grid').getElementsByClassName('t-grid-header-wrap')[0].scrollLeft = target.scrollLeft;
            }
            this.columnResize();
        }

        bindResizeObserver() {
            const resizeObserver = new ResizeObserver(entries => {
                let grv = this.grid;
                for (let entry of entries) {
                    if (entry.contentBoxSize && entry.contentBoxSize[0]) {
                        let contentHeight = grv.getElementsByClassName('t-grid-content')[0].getBoundingClientRect().height;
                        let table = grv.getElementsByClassName('t-grid-content')[0].getElementsByTagName('table')[0];
                        if (table) {
                            let tableHeight = table.getBoundingClientRect().height;
                            let header = grv.getElementsByClassName('t-grid-header')[0] as HTMLElement;
                            if (contentHeight < tableHeight) {
                                if (caspian.common.RightToLeft())
                                    header.style.paddingLeft = '11px';
                                else
                                    header.style.paddingRight = '11px';
                            }
                            else {
                                if (caspian.common.RightToLeft())
                                    header.style.paddingLeft = '0';
                                else
                                    header.style.paddingRight = '0';
                            }
                        }
                    }
                }
            });
            resizeObserver.observe(this.grid.getElementsByClassName('t-grid-content')[0]);

        }

        bindObserver() {
            const mutationObserver = new MutationObserver(list => {
                list.every(t => {
                    let insertTable = (t.target as HTMLElement).getElementsByClassName('c-grid-insert')[0];
                    if (insertTable != null) {
                        let columns = this.grid.getElementsByClassName('t-grid-header-wrap')[0].getElementsByTagName("tr")[0].children;
                        let insertColumns = insertTable.getElementsByTagName('tr')[0].children;
                        for (let index = 0; index < columns.length; index++) 
                            (insertColumns[index] as HTMLElement).style.width = `${columns[index].getBoundingClientRect().width}px`
                    }
                });
            });
            mutationObserver.observe(this.content, {
                attributes: false,
                childList: true,
                subtree: false,
            });
        }

        columnResize() {
            let head = this.grid.getElementsByClassName('t-grid-header-wrap')[0] as HTMLElement;

            head.onmousemove = e => {
                let element = (e.target as HTMLElement);
                if (element.tagName != 'th')
                    element = element.closest('th');
                if (element) {
                    let loc = element.getBoundingClientRect(), x = e.clientX;
                    if ((x - loc.left) < 5 || (loc.right - x) < 5) {
                        (e.target as HTMLElement).style.cursor = 'col-resize';
                        this.resize = true;
                    }
                    else {
                        (e.target as HTMLElement).style.cursor = '';
                        this.resize = false;
                    }
                }
            }
            head.onmousedown = e => {
                if (this.resize) {
                    let rtl = caspian.common.RightToLeft();
                    let element = (e.target as HTMLElement);
                    if (element.tagName != 'th')
                        element = element.closest('th');
                    let loc = element.getBoundingClientRect(), x = e.clientX;
                    this.curent = element;
                    this.curentWidth = loc.width;
                    let other = null;
                    if (x - loc.left < 5 && rtl || loc.right - x < 5 && !rtl) {
                        other = element.nextSibling as HTMLElement;
                        this.gridStatus = 1;
                    }
                    if (loc.right - x < 5 && rtl || x - loc.left < 5 && !rtl) {
                        other = element.previousSibling as HTMLElement;
                        this.gridStatus = 2
                    }
                    this.other = other;
                    if (other == null)
                        this.gridStatus = 3;
                    else
                        this.otherWidth = other.getBoundingClientRect().width;
                    this.xStart = e.clientX;
                }
                window.onclick = () => this.drop();
                window.onmousemove = e => this.dragging(e);
            }
        }
        public dragging(e: MouseEvent) {
            
            if (this.other == null)
                return;
            let dif = this.xStart - e.clientX;
            if (this.gridStatus == 2)
                dif = -dif;
            let curentWidth = this.curentWidth;
            let otherWidth = this.otherWidth - 1;
            let curentResult = curentWidth - dif, otherResult = otherWidth + dif;
            if (curentResult < 30 || otherResult < 30)
                return;
            this.curent.style.width = `${curentResult}px`;
            this.other.style.width = `${otherResult }px`;
            let columns = this.grid.getElementsByClassName('t-grid-header-wrap')[0].getElementsByTagName("tr")[0].children;
            let curentIndex = columns.indexOf(this.curent)
            let otherIndex = columns.indexOf(this.other);
            columns = this.content.getElementsByTagName('tr')[0].children;
            (columns.item(curentIndex) as HTMLElement).style.width = `${curentResult}px`;
            (columns.item(otherIndex) as HTMLElement).style.width = `${otherResult}px`;
            let contentHeight = this.content.getBoundingClientRect().height;
            let tableHeight = this.grid.getElementsByClassName('c-grid-items')[0].getBoundingClientRect().height;
            let header = this.grid.getElementsByClassName('t-grid-header')[0] as HTMLElement;
            let insertTable = this.grid.getElementsByClassName('c-grid-insert')[0];
            if (insertTable != null) {
                let insertColumns = insertTable.getElementsByTagName('tr')[0].children;
                (insertColumns.item(curentIndex) as HTMLElement).style.width = `${curentResult}px`;
                (insertColumns.item(otherIndex) as HTMLElement).style.width = `${otherResult}px`;
            }
            if (contentHeight < tableHeight) {
                if (caspian.common.RightToLeft())
                    header.style.paddingLeft = '11px';
                else
                    header.style.paddingRight = '11px';
            }
            else {
                header.style.paddingLeft = '0';
                header.style.paddingRight = '0';
            }
        }

        public drop() {
            window.onmousemove = null;
            window.onclick = null;
        }
    }
}