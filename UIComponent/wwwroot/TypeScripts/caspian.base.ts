interface HTMLCollection {
    indexOf(element: HTMLElement): number;
}

interface HTMLElement {
    getPosition(): DOMRect;
}

//interface Array<T extends number> {
//    sum(): number;
//}


// Implement the Extension
HTMLCollection.prototype.indexOf = function (element: HTMLElement): number {
    let items = this as HTMLCollection;
    for (let index = 0; index < items.length; index++)
        if (items[index] == element)
            return index;
    return -1;
}

HTMLElement.prototype.getPosition = function () {
    let parent: HTMLElement = this, left = 0, top = 0;
    let rect = parent.getBoundingClientRect();
    while (parent != document.body) {
        left += parent.offsetLeft;
        top += parent.offsetTop;
        parent = parent.offsetParent as HTMLElement;
    }
    return new DOMRect(left, top, rect.width, rect.height);
}

//Array.prototype.sum = function (): number {
//    let sumArray = 0;
//    for (var index = 0; index < (this as Array<number>).length; index++)
//        sumArray += this[index];
//    return sumArray;
//}

interface dotnetInvoker {
    invokeMethodAsync(str: string): Promise<void>;
}