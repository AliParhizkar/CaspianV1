var caspian;
(function (caspian) {
    class DataGrid {
        constructor(grv) {
            if (grv == null)
                return;
            this.grid = grv;
            this.content = grv.getElementsByClassName('t-grid-content')[0];
            if (this.content.classList.contains('t-inline-content'))
                this.bindObserverForInsertTable();
            this.bindResizeForHeight();
            this.header = this.grid.getElementsByClassName('t-grid-header-wrap')[0];
            this.headerColumns = [];
            this.header.querySelectorAll('table thead tr th').forEach(t => this.headerColumns.push(t));
            this.headerColumns.forEach(t => {
                t.attributes['default-size'] = t.style.width;
            });
            this.content.onscroll = e => {
                let target = e.target;
                target.closest('.t-grid').getElementsByClassName('t-grid-header-wrap')[0].scrollLeft = target.scrollLeft;
            };
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
                        t.style.width = this.headerColumns[index].attributes['default-size'];
                    });
                }
                let tr = this.content.querySelector('.c-grid-items tbody tr');
                if (tr != null) {
                    tr.querySelectorAll('td').forEach((t, index) => {
                        t.style.width = this.headerColumns[index].attributes['default-size'];
                    });
                }
            });
            resizeObserver.observe(this.grid);
        }
        bindObserverForContentTable(element) {
            const mutationObserver = new MutationObserver(list => {
                let table = list[0].target.closest('table');
                if (table.rows.length == 1) {
                    for (let index = 0; index < this.headerColumns.length; index++)
                        table.rows[0].cells[index].style.width = `${this.headerColumns[index].getBoundingClientRect().width}px`;
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
                        let container = grv.getElementsByClassName('t-grid-content')[0];
                        let header = grv.getElementsByClassName('t-grid-header')[0];
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
                    let container = t.target;
                    let header = container.closest('.t-widget').getElementsByClassName('t-grid-header')[0];
                    if (container.scrollHeight > container.clientHeight)
                        header.style.overflowY = "scroll";
                    else
                        header.style.overflowY = '';
                    let insertTable = container.getElementsByClassName('c-grid-insert')[0];
                    let contentTable = container.getElementsByClassName('c-grid-items')[0];
                    if (contentTable && !contentTable.attributes['isbinded']) {
                        contentTable.attributes['isbinded'] = true;
                        this.bindObserverForContentTable(contentTable);
                    }
                    if (insertTable != null) {
                        let insertColumns = insertTable.querySelectorAll('tbody tr td');
                        for (let index = 0; index < this.headerColumns.length; index++)
                            insertColumns[index].style.width = `${this.headerColumns[index].getBoundingClientRect().width}px`;
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
            let head = this.grid.getElementsByClassName('t-grid-header-wrap')[0];
            head.onmousemove = e => {
                let element = e.target;
                if (element.tagName != 'th')
                    element = element.closest('th');
                if (element) {
                    let loc = element.getBoundingClientRect(), x = e.clientX;
                    if ((x - loc.left) < 5 || (loc.right - x) < 5) {
                        e.target.style.cursor = 'col-resize';
                        this.resize = true;
                    }
                    else {
                        e.target.style.cursor = '';
                        this.resize = false;
                    }
                }
            };
            head.onmousedown = e => {
                if (this.resize) {
                    let rtl = caspian.common.RightToLeft();
                    let element = e.target;
                    if (element.tagName != 'th')
                        element = element.closest('th');
                    let loc = element.getBoundingClientRect(), x = e.clientX;
                    this.curent = element;
                    this.curentWidth = loc.width;
                    let other = null;
                    if (x - loc.left < 5 && rtl || loc.right - x < 5 && !rtl) {
                        other = element.nextSibling;
                        this.gridStatus = 1;
                    }
                    if (loc.right - x < 5 && rtl || x - loc.left < 5 && !rtl) {
                        other = element.previousSibling;
                        this.gridStatus = 2;
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
            };
        }
        dragging(e) {
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
            if (caspian.common.convertToInt(this.curent.style.minWidth) > curentResult || caspian.common.convertToInt(this.other.style.minWidth) > otherResult)
                return;
            this.curent.style.width = `${curentResult}px`;
            this.other.style.width = `${otherResult}px`;
            let headerColumns = this.grid.getElementsByClassName('t-grid-header-wrap')[0].getElementsByTagName("tr")[0].children;
            let curentIndex = headerColumns.indexOf(this.curent);
            let otherIndex = headerColumns.indexOf(this.other);
            //change width of content
            let contentTable = this.content.getElementsByClassName('c-grid-items')[0];
            if (contentTable != null && contentTable.rows.length > 0) {
                let contentColumns = contentTable.getElementsByTagName('tr')[0].children;
                contentColumns.item(curentIndex).style.width = `${curentResult}px`;
                contentColumns.item(otherIndex).style.width = `${otherResult}px`;
            }
            let contentHeight = this.content.getBoundingClientRect().height;
            let tableHeight = this.grid.getElementsByClassName('c-grid-items')[0].getBoundingClientRect().height;
            let header = this.grid.getElementsByClassName('t-grid-header')[0];
            let insertTable = this.grid.getElementsByClassName('c-grid-insert')[0];
            if (insertTable != null) {
                let insertColumns = insertTable.getElementsByTagName('tr')[0].children;
                insertColumns.item(curentIndex).style.width = `${curentResult}px`;
                insertColumns.item(otherIndex).style.width = `${otherResult}px`;
            }
            if (contentHeight < tableHeight) {
            }
            else {
            }
        }
        drop() {
            window.onmousemove = null;
            window.onclick = null;
        }
    }
    caspian.DataGrid = DataGrid;
})(caspian || (caspian = {}));
//# sourceMappingURL=caspian.datagrid.js.map