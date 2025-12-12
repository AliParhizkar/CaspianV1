namespace caspian {
    export class DataGrid {
        grid: HTMLElement;
        content: HTMLElement;
        resize: boolean;
        curent: HTMLElement;
        curentWidth: number;
        gridStatus: number;
        other: HTMLElement;sss
        otherWidth: number;
        xStart: number;
        header: HTMLElement;
        headerColumns: HTMLElement[]; 

        constructor(grv: HTMLElement) {
            if (grv == null)
                return;
            this.grid = grv;
            this.content = (grv.getElementsByClassName('t-grid-content')[0] as HTMLDivElement);
            if (this.content.classList.contains('t-inline-content'))
                this.bindObserverForInsertTable();
            this.bindResizeForHeight();
            this.header = this.grid.getElementsByClassName('t-grid-header-wrap')[0] as HTMLElement;
            this.headerColumns = [];
            this.header.querySelectorAll('table thead tr th').forEach(t => this.headerColumns.push(t as HTMLElement));
            this.headerColumns.forEach(t => {
                t.attributes['default-size'] = t.style.width;
            });
            this.content.onscroll = e => {
                let target = e.target as HTMLElement;
                target.closest('.t-grid').getElementsByClassName('t-grid-header-wrap')[0].scrollLeft = target.scrollLeft;
            }
            this.columnResize();
            this.bindObserverSizeForWidth();
        }

        bindObserverSizeForWidth() {
            const resizeObserver = new ResizeObserver(entries => {
                
                this.headerColumns.forEach(t => {
                    t.style.width = t.attributes['default-size'];
                });
                let insert = this.grid.querySelector('.c-grid-insert');
                if (insert != null) {
                    insert.querySelector('tbody tr').querySelectorAll('td').forEach((t, index) => {
                        if (index < this.headerColumns.length)
                            (t as HTMLElement).style.width = this.headerColumns[index].attributes['default-size'];
                    });
                }
                let tr = this.content.querySelector('.c-grid-items tbody tr');
                if (tr != null) {
                    tr.querySelectorAll('td').forEach((t, index) => {
                        (t as HTMLElement).style.width = this.headerColumns[index].attributes['default-size'];
                    });
                }
            });
            resizeObserver.observe(this.grid);
        }

        bindObserverForContentTable(element: HTMLElement) {
            const mutationObserver = new MutationObserver(list => {
                let table = ((list[0].target as HTMLElement).closest('table') as HTMLTableElement);
                if (table.rows.length == 1) {
                    for (let index = 0; index < this.headerColumns.length; index++)
                        table.rows[0].cells[index].style.width = `${this.headerColumns[index].getBoundingClientRect().width}px`    
                }
            });
            mutationObserver.observe(element.getElementsByTagName('tbody')[0], {
                attributes: false,
                childList: true,
                subtree: false,
            });
        }

        bindResizeForHeight() {
            const resizeObserver = new ResizeObserver(entries => {
                let grv = this.grid;
                for (let entry of entries) {
                    if (entry.contentBoxSize && entry.contentBoxSize[0]) {
                        let container = grv.getElementsByClassName('t-grid-content')[0] as HTMLElement;
                        let header = grv.getElementsByClassName('t-grid-header')[0] as HTMLElement;

                        if (container.scrollHeight > container.clientHeight)
                            header.style.overflowY = 'scroll';
                        else
                            header.style.overflowY = '';
                    }
                }
            });
            resizeObserver.observe(this.grid.getElementsByClassName('t-grid-content')[0]);
        }

        bindObserverForInsertTable() {
            const mutationObserver = new MutationObserver(list => {
                list.every(t => {
                    let container = (t.target as HTMLElement);
                    let header = container.closest('.t-widget').getElementsByClassName('t-grid-header')[0] as HTMLElement;
                    if (container.scrollHeight > container.clientHeight)
                        header.style.overflowY = "scroll";
                    else 
                        header.style.overflowY = '';
                    let insertTable = container.getElementsByClassName('c-grid-insert')[0];
                    let contentTable = container.getElementsByClassName('c-grid-items')[0];
                    if (contentTable && !contentTable.attributes['isbinded']) {
                        contentTable.attributes['isbinded'] = true;
                        this.bindObserverForContentTable(contentTable as HTMLElement);
                    }
                    if (insertTable != null) {
                        let insertColumns = insertTable.querySelectorAll('tbody tr td');
                        for (let index = 0; index < this.headerColumns.length; index++) 
                            (insertColumns[index] as HTMLElement).style.width = `${this.headerColumns[index].getBoundingClientRect().width}px`
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
            if (caspian.common.RightToLeft())
                dif = -dif;
            let curentWidth = this.curentWidth;
            let otherWidth = this.otherWidth - 1;
            let curentResult = curentWidth - dif, otherResult = otherWidth + dif;
            if (curentResult < 30 || otherResult < 30)
                return;
            if (common.convertToInt(this.curent.style.minWidth) > curentResult || common.convertToInt(this.other.style.minWidth) > otherResult)
                return;
            this.curent.style.width = `${curentResult}px`;
            this.other.style.width = `${otherResult }px`;
            let headerColumns = this.grid.getElementsByClassName('t-grid-header-wrap')[0].getElementsByTagName("tr")[0].children;
            let curentIndex = headerColumns.indexOf(this.curent)
            let otherIndex = headerColumns.indexOf(this.other);
            //change width of content
            let contentTable = this.content.getElementsByClassName('c-grid-items')[0] as HTMLTableElement;
            if (contentTable != null && contentTable.rows.length > 0) {
                let contentColumns = contentTable.getElementsByTagName('tr')[0].children;
                (contentColumns.item(curentIndex) as HTMLElement).style.width = `${curentResult}px`;
                (contentColumns.item(otherIndex) as HTMLElement).style.width = `${otherResult}px`;
            }
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

            }
            else {
            }
        }

        public drop() {
            window.onmousemove = null;
            window.onclick = null;
        }
    }
}