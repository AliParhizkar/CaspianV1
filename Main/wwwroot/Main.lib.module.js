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