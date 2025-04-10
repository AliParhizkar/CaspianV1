export function afterWebStarted(blazor) {
    blazor.registerCustomEventType('Ekeydown', {
        browserEventName: 'keydown',
        createEventArgs: e => createData(e)
    });
    blazor.registerCustomEventType('Ekeypress', {
        browserEventName: 'keypress',
        createEventArgs: e => createData(e)
    });
    blazor.registerCustomEventType('Ekeyup', {
        browserEventName: 'keyup',
        createEventArgs: e => createData(e)
    });
    blazor.registerCustomEventType('Eclick', {
        browserEventName: 'click',
        createEventArgs: e => createMouseData(e)
    });
    blazor.registerCustomEventType('Edblclick', {
        browserEventName: 'dblclick',
        createEventArgs: e => createMouseData(e)
    });
    blazor.registerCustomEventType('Emousedown', {
        browserEventName: 'mousedown',
        createEventArgs: e => createMouseData(e)
    });
}
function createMouseData(e) {
    return {
        altKey: e.altKey,
        button: e.button,
        buttons: e.buttons,
        clientX: e.clientX,
        clientY: e.clientY,
        ctrlKey: e.ctrlKey,
        detail: e.detail,
        metaKey: e.metaKey,
        movementX: e.movementX,
        movementY: e.movementY,
        offsetX: e.offsetX,
        offsetY: e.offsetY,
        pageX: e.pageX,
        pageY: e.pageY,
        screenX: e.screenX,
        screenY: e.screenY,
        shiftKey: e.shiftKey,
        type: e.type
    }
}

function createData(e) {
    return {
        code: e.code,
        key: e.key,
        repeat: e.repeat,
        altKey: e.altKey,
        ctrlKey: e.ctrlKey,
        shiftKey: e.shiftKey,
        metaKey: e.metaKey,
        location: e.location,
        type: e.type,
        selectionStart: e.target.selectionStart,
        selectionEnd: e.target.selectionEnd
    }
}