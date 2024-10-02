namespace caspian {
    export class DataGrid {
        grid: HTMLElement;
        resize: boolean;
        curent: HTMLElement;
        curentWidth: number;
        gridStatus: number;
        other: HTMLElement;
        otherWidth: number;
        xStart: number;
        
        constructor(grv: HTMLElement) {
            this.grid = grv;
            this.bindObserver();
            (grv.getElementsByClassName('t-grid-content')[0] as HTMLDivElement).onscroll = e => {
                let target = e.target as HTMLElement;
                target.closest('.t-grid').getElementsByClassName('t-grid-header-wrap')[0].scrollLeft = target.scrollLeft;
            }
        }

        bindObserver() {
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
                                    header.style.paddingLeft = 11 + 'px';
                                else
                                    header.style.paddingRight = 11 + 'px';
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
                window.onclick = this.drop;
                window.onmousemove = this.dragging;
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
            this.curent.style.width = curentResult + 'px';
            this.other.style.width = otherResult + 'px';
            let columns = this.grid.getElementsByClassName('t-grid-header-wrap')[0].getElementsByTagName("tr")[0].children;
            let curentIndex = columns.indexOf(this.curent)
            let otherIndex = columns.indexOf(this.other);
            columns = this.grid.getElementsByClassName('t-grid-content')[0].getElementsByTagName('tr')[0].children;
            (columns.item(curentIndex) as HTMLElement).style.width = curentResult + 'px';
            (columns.item(otherIndex) as HTMLElement).style.width = otherResult + 'px';
            let contentHeight = this.grid.getElementsByClassName('t-grid-content')[0].getBoundingClientRect().height;
            let tableHeight = this.grid.getElementsByClassName('c-grid-items')[0].getBoundingClientRect().height;
            let header = this.grid.getElementsByClassName('t-grid-header')[0] as HTMLElement;
            if (contentHeight < tableHeight) {
                if (caspian.common.RightToLeft())
                    header.style.paddingLeft = 11 + 'px';
                else
                    header.style.paddingRight = 11 + 'px';
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