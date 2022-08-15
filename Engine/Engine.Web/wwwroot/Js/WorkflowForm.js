(function ($) {
    var $w = $.workflowForm = {
        init: function (dotNetObjectReference) {
            this.dotnet = dotNetObjectReference;
        },

        findCode: function (formName, ctrName, propertyName, code) {
            var data = {
                action: 'findCode',
                content: JSON.stringify({
                    Id: ctrName,
                    Property: propertyName,
                    Code: code,
                    ClassName: formName
                })
            };
            window.chrome.webview.postMessage(data);
        },

        findEventHandler: function (formName, ctrName, eventName) {
            var data = {
                action: 'findEventHandler',
                content: JSON.stringify({
                    Id: ctrName,
                    EventHandler: eventName,
                    ClassName: formName
                })
            };
            window.chrome.webview.postMessage(data);
        },

        getSourceCodeString() {
            $.workflowForm.dotnet.invokeMethodAsync("GetSourceCodeString").then(t => {
                var data = {
                    action: 'setSourceCode',
                    content: t
                };
                window.chrome.webview.postMessage(data);
            });
        },

        getCodebehindString() {
            $.workflowForm.dotnet.invokeMethodAsync("GetCodebehindString").then(t => {
                var data = {
                    action: 'setCodebehind',
                    content: t
                };
                window.chrome.webview.postMessage(data);
            });
        },
        sendSaveRequest: function () {
            var data = {
                action: 'sendSourceCode',
                content: ''
            };
            window.chrome.webview.postMessage(data);;
        },
        loadForm: function (formId) {
            var data = {
                action: 'sendSourceCode',
                content: formId
            };
            window.chrome.webview.postMessage(data);;
        },
        saveCodeFile: async function (code) {
            await $.workflowForm.dotnet.invokeMethodAsync('SaveFile', code);
        },
    }
})(jQuery);
function save() {
}
