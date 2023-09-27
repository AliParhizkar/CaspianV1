class AutoStepTokenState {
    state = 0;

    constructor(state) {
        this.state = state
    }

    clone() {
        return new AutoStepTokenState(this.state + 1);
    }
    equals() {
        return true;
    }
}

function registerCsharpProvider() {
    
    monaco.languages.registerCompletionItemProvider('csharp', {
        triggerCharacters: [".", " "],
        provideCompletionItems: async (model, position) => {
            let data = await $.caspian.dotnet.invokeMethodAsync("Complete", model.getValue(), model.getOffsetAt(position));
            let suggestions = [];
            for (let elem of data) {
                suggestions.push({
                    label: {
                        label: elem.suggestion,
                        description: elem.description
                    },
                    kind: elem.completionItemKind,
                    insertText: elem.suggestion
                });
            }

            return { suggestions: suggestions };
        }
    });

    //monaco.languages.registerSignatureHelpProvider('csharp', {
    //    signatureHelpTriggerCharacters: ["("],
    //    signatureHelpRetriggerCharacters: [","],

    //    provideSignatureHelp: async (model, position, token, context) => {

    //        let request = {
    //            Code: model.getValue(),
    //            Position: model.getOffsetAt(position),
    //            Assemblies: assemblies
    //        }

    //        let resultQ = await sendRequest("signature", request);
    //        if (!resultQ.data) return;

    //        let signatures = [];
    //        for (let signature of resultQ.data.Signatures) {
    //            let params = [];
    //            for (let param of signature.Parameters) {
    //                params.push({
    //                    label: param.Label,
    //                    documentation: param.Documentation ?? ""
    //                });
    //            }

    //            signatures.push({
    //                label: signature.Label,
    //                documentation: signature.Documentation ?? "",
    //                parameters: params,
    //            });
    //        }

    //        let signatureHelp = {};
    //        signatureHelp.signatures = signatures;
    //        signatureHelp.activeParameter = resultQ.data.ActiveParameter;
    //        signatureHelp.activeSignature = resultQ.data.ActiveSignature;

    //        return {
    //            value: signatureHelp,
    //            dispose: () => { }
    //        };
    //    }
    //});

    monaco.editor.defineTheme('caspianTheme', {
        base: 'vs',
        inherit: true,
        rules: [
            { token: 'method', foreground: '875940', fontStyle:'bold' },
            { token: 'identifire', foreground: '30278F' },
            { token: 'comment-gray', foreground: '939393' },
            { token: 'comment-green', foreground: '198754' },
            { token: 'comment-blue', foreground: '4743ac' }
        ],
        colors: {}
    });
    monaco.editor.setTheme('caspianTheme')
    monaco.languages.setTokensProvider("csharp", {
        getInitialState() {
            return new AutoStepTokenState(0);
        },

        tokenize(line, state) {
            let array = $.caspian.tokensData.filter(t => t.line == state.state - 1);
            return {
                tokens: array.map(t => {
                    let scopes = ''
                    switch (t.kind) {
                        case 1:
                            scopes = 'keyword';
                            break;
                        case 2:
                            scopes = 'type';
                            break;
                        case 3:
                            scopes = 'method';
                            break;
                        case 4:
                            scopes = 'identifire';
                            break;
                        case 5:
                            scopes = 'comment-gray';
                            break;
                        case 6:
                            scopes = 'comment-blue';
                            break;
                        case 7:
                            scopes = 'comment-green';
                            break;
                    }
                    return {
                        startIndex: t.startIndex,
                        scopes: scopes
                    };
                }),
                endState: new AutoStepTokenState(30)
            }
        }
    });


    

    //monaco.languages.registerHoverProvider('csharp', {
    //    provideHover: async function (model, position) {
    //        let resultQ = await $.caspian.dotnet.invokeMethodAsync("Hover", model.getValue(), model.getOffsetAt(position));
    //        if (resultQ.data) {
    //            posStart = model.getPositionAt(resultQ.data.OffsetFrom);
    //            posEnd = model.getPositionAt(resultQ.data.OffsetTo);

    //            return {
    //                range: new monaco.Range(posStart.lineNumber, posStart.column, posEnd.lineNumber, posEnd.column),
    //                contents: [
    //                    { value: resultQ.data.Information }
    //                ]
    //            };
    //        }

    //        return null;
    //    }
    //});

    monaco.editor.onDidCreateModel(function (model) {
        async function validate() {
            let data = await $.caspian.dotnet.invokeMethodAsync("CodeCheck", model.getValue());
            let markers = [];

            for (let elem of data) {
                posStart = model.getPositionAt(elem.offsetFrom);
                posEnd = model.getPositionAt(elem.offsetTo);
                markers.push({
                    severity: elem.severity,
                    startLineNumber: posStart.lineNumber,
                    startColumn: posStart.column,
                    endLineNumber: posEnd.lineNumber,
                    endColumn: posEnd.column,
                    message: elem.message,
                    code: elem.id
                });
            }
            monaco.editor.setModelMarkers(model, 'csharp', markers);
        }
        var handle = null;
        model.onDidChangeContent(async () => {
               
            monaco.editor.setModelMarkers(model, 'csharp', []);
            debugger;
            $.caspian.tokensData = await $.caspian.dotnet.invokeMethodAsync("GetTokensAsync", model.getValue());
            clearTimeout(handle);
            handle = setTimeout(async () => {
                
                //var q = monaco.editor.getModels()[0]._resetTokenizationState;
                //debugger
                validate();
            }, 500);
        });
        validate();
    });

    /*monaco.languages.registerInlayHintsProvider('csharp', {
        displayName: 'test',
        provideInlayHints(model, range, token) {
            return {
                hints: [
                    {
                        label: "Test",
                        tooltip: "Tooltip",
                        position: { lineNumber: 3, column: 2},
                        kind: 2
                    }
                ],
                dispose: () => { }
            };
        }

    });*/

    /*monaco.languages.registerCodeActionProvider("csharp", {
        provideCodeActions: async (model, range, context, token) => {
            const actions = context.markers.map(error => {
                console.log(context, error);
                return {
                    title: `Example quick fix`,
                    diagnostics: [error],
                    kind: "quickfix",
                    edit: {
                        edits: [
                            {
                                resource: model.uri,
                                edits: [
                                    {
                                        range: error,
                                        text: "This text replaces the text with the error"
                                    }
                                ]
                            }
                        ]
                    },
                    isPreferred: true
                };
            });
            return {
                actions: actions,
                dispose: () => { }
            }
        }
    });*/

}