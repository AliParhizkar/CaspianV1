//interface Array<T extends number> {
//    sum(): number;
//}
// Implement the Extension
HTMLCollection.prototype.indexOf = function (element) {
    let items = this;
    for (let index = 0; index < items.length; index++)
        if (items[index] == element)
            return index;
    return -1;
};
HTMLElement.prototype.getPosition = function () {
    let parent = this, left = 0, top = 0;
    let rect = parent.getBoundingClientRect();
    while (parent != document.body && parent != null) {
        left += parent.offsetLeft;
        top += parent.offsetTop;
        parent = parent.offsetParent;
    }
    return new DOMRect(left, top, rect.width, rect.height);
};
//# sourceMappingURL=caspian.base.js.map