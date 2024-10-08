﻿(function ($) {
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
            if (window.chrome.webview)
                window.chrome.webview.postMessage(data);
            else
                caspian.common.showMessage('Using WPF for coding')
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
        sendSaveRequest: async function () {
            if (window.chrome.webview) {
                let data = {
                    action: 'sendSourceCode',
                    content: ''
                };
                window.chrome.webview.postMessage(data);
            }
            else
                await $w.saveCodeFile(null);
        },
        loadForm: function (formId) {
            var data = {
                action: 'sendSourceCode',
                content: formId
            };
            window.chrome.webview.postMessage(data);;
        },
        saveCodeFile: async function (code) {
            await $.workflowForm.dotnet.invokeMethodAsync('SaveCodeAndView', code);
        },
    }
})(jQuery);
