export function AbortError(errorMessage: any): any;
export namespace AbortError {
    function captureStackTrace(p0: any, p1: any): any;
    const stackTraceLimit: number;
}
export class DefaultHttpClient {
    constructor(logger: any);
    get(url: any, options: any): any;
    getCookieString(url: any): any;
    post(url: any, options: any): any;
    send(request: any): any;
}
export class HttpClient {
    get(url: any, options: any): any;
    getCookieString(url: any): any;
    post(url: any, options: any): any;
}
export function HttpError(errorMessage: any, statusCode: any): any;
export namespace HttpError {
    function captureStackTrace(p0: any, p1: any): any;
    const stackTraceLimit: number;
}
export class HttpResponse {
    constructor(statusCode: any, statusText: any, content: any);
    statusCode: any;
    statusText: any;
    content: any;
}
export const HttpTransportType: {
    "0": string;
    "1": string;
    "2": string;
    "4": string;
    LongPolling: number;
    None: number;
    ServerSentEvents: number;
    WebSockets: number;
};
export class HubConnection {
    static create(connection: any, logger: any, protocol: any): any;
    constructor(connection: any, logger: any, protocol: any);
    serverTimeoutInMilliseconds: any;
    keepAliveIntervalInMilliseconds: any;
    logger: any;
    protocol: any;
    connection: any;
    handshakeProtocol: any;
    callbacks: any;
    methods: any;
    closedCallbacks: any;
    id: any;
    receivedHandshakeResponse: any;
    connectionState: any;
    cachedPingMessage: any;
    cleanupPingTimer(): void;
    cleanupTimeout(): void;
    connectionClosed(error: any): void;
    createCancelInvocation(id: any): any;
    createInvocation(methodName: any, args: any, nonblocking: any, ...args: any[]): any;
    createStreamInvocation(methodName: any, args: any, ...args: any[]): any;
    invoke(methodName: any, ...args: any[]): any;
    invokeClientMethod(invocationMessage: any): void;
    off(methodName: any, method: any): void;
    on(methodName: any, newMethod: any): void;
    onclose(callback: any): void;
    processHandshakeResponse(data: any): any;
    processIncomingData(data: any): void;
    resetKeepAliveInterval(): void;
    resetTimeoutPeriod(): void;
    send(methodName: any, ...args: any[]): any;
    sendMessage(message: any): any;
    serverTimeout(): void;
    start(): any;
    stop(): any;
    stream(methodName: any, ...args: any[]): any;
}
export class HubConnectionBuilder {
    build(): any;
    configureLogging(logging: any): any;
    withHubProtocol(protocol: any): any;
    withUrl(url: any, transportTypeOrOptions: any): any;
}
export const HubConnectionState: {
    "0": string;
    "1": string;
    Connected: number;
    Disconnected: number;
};
export class JsonHubProtocol {
    name: any;
    version: any;
    transferFormat: any;
    assertNotEmptyString(value: any, errorMessage: any): void;
    isCompletionMessage(message: any): void;
    isInvocationMessage(message: any): void;
    isStreamItemMessage(message: any): void;
    parseMessages(input: any, logger: any): any;
    writeMessage(message: any): any;
}
export const LogLevel: {
    "0": string;
    "1": string;
    "2": string;
    "3": string;
    "4": string;
    "5": string;
    "6": string;
    Critical: number;
    Debug: number;
    Error: number;
    Information: number;
    None: number;
    Trace: number;
    Warning: number;
};
export const MessageType: {
    "1": string;
    "2": string;
    "3": string;
    "4": string;
    "5": string;
    "6": string;
    "7": string;
    CancelInvocation: number;
    Close: number;
    Completion: number;
    Invocation: number;
    Ping: number;
    StreamInvocation: number;
    StreamItem: number;
};
export class NullLogger {
    log(_logLevel: any, _message: any): void;
}
export namespace NullLogger {
    const instance: {
        log: Function;
    };
}
export function TimeoutError(errorMessage: any): any;
export namespace TimeoutError {
    function captureStackTrace(p0: any, p1: any): any;
    const stackTraceLimit: number;
}
export const TransferFormat: {
    "1": string;
    "2": string;
    Binary: number;
    Text: number;
};
export const VERSION: string;
