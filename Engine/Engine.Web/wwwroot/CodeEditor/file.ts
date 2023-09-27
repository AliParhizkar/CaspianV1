//export class AutoStepTokenProvider implements languages.TokensProvider {
//    private callback: IBlazorInteropObject;

//    constructor(blazorCallback: IBlazorInteropObject) {
//        this.callback = blazorCallback;
//    }

//    getInitialState(): languages.IState {
//        return new AutoStepTokenState(this.callback.invokeMethod<number>("GetInitialState"));
//    }

//    tokenize(line: string, state: languages.IState): languages.ILineTokens {

//        if (state instanceof AutoStepTokenState) {
//            var result: any = this.callback.invokeMethod("Tokenize", line, state.tokenState);

//            return { tokens: result.tokens, endState: new AutoStepTokenState(result.endState) };
//        }

//        throw "Invalid start state";
//    }
//}