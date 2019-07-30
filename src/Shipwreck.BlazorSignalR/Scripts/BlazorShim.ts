﻿/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/signalr/signalr.d.ts" />
/// <reference path="typings/Blazor.d.ts" />

(function () {
    interface StartArgs {
        Url: string;
        QueryString: string;
        Logging: boolean;
        UseDefaultPath: boolean;

        Hubs: HubArgs[];
    }
    interface HubArgs {
        Name: string;
        Callbacks: CallbackArgs[];
    }
    interface CallbackArgs {
        Name: string;
        Length: number;
    }
    class Instance {
        readonly hashCode: number;
        readonly _connection: SignalR.Hub.Connection;
        readonly _proxies: { [hubName: string]: SignalR.Hub.Proxy };

        constructor(hashCode: number, args: StartArgs) {
            this.hashCode = hashCode;

            const c = this._connection = $.hubConnection(args.Url, {
                qs: args.QueryString,
                logging: args.Logging,
                useDefaultPath: args.UseDefaultPath
            });

            c.connectionSlow(() => DotNet.invokeMethodAsync('Shipwreck.BlazorSignalR', 'OnSignalRConnectionSlow', this.hashCode));
            c.disconnected(() => DotNet.invokeMethodAsync('Shipwreck.BlazorSignalR', 'OnSignalRDisconnected', this.hashCode));
            c.error(e => DotNet.invokeMethodAsync('Shipwreck.BlazorSignalR', 'OnSignalRError', this.hashCode, JSON.stringify({
                readyState: e.context ? e.context.readyState : null,
                responseText: e.context ? e.context.responseText : null,
                status: e.context ? e.context.status : null,
                statusText: e.context ? e.context.statusText : null,
                message: e.message,
                name: e.name,
                source: e.source,
                stack: e.stack,
                transport: e.transport,
            })));
            c.reconnected(() => DotNet.invokeMethodAsync('Shipwreck.BlazorSignalR', 'OnSignalRReconnected', this.hashCode));
            c.reconnecting(() => DotNet.invokeMethodAsync('Shipwreck.BlazorSignalR', 'OnSignalRReconnecting', this.hashCode));
            c.stateChanged(e => DotNet.invokeMethodAsync('Shipwreck.BlazorSignalR', 'OnSignalRStateChanged', this.hashCode, e.oldState, e.newState))

            this._proxies = {};
            const _self = this;
            for (const h of args.Hubs) {
                const p = this._proxies[h.Name] = c.createHubProxy(h.Name);
                if (h.Callbacks) {
                    for (const c of h.Callbacks) {
                        p.on(c.Name, (function (h, c) {
                            return function () {
                                const args = [];
                                for (let i = 0; i < c.Length; i++) {
                                    args[i] = arguments[i];
                                }
                                _self._on(h.Name, c.Name, args);
                            };
                        })(h, c));
                    }
                }
            }
        }

        async start() {
            await this._connection.start();
        }

        async invoke(hub: string, method: string, args: any[]) {
            const p = this._proxies[hub];
            if (p) {
                const r = await p.invoke(method, ...args);
                return r === undefined ? null : JSON.stringify(r);
            }
        }

        async stop() {
            await this._connection.stop(true, true);
        }

        private async _on(hubName: string, eventName: string, args: any[]) {
            try {
                await DotNet.invokeMethodAsync('Shipwreck.BlazorSignalR', 'OnSignalREvent', this.hashCode, hubName, eventName, JSON.stringify(args));
            } catch (ex) {
                const msg = `An error caught while receiving SignalR event ${hubName}#${eventName}`;
                console.error(msg, args, ex);
                throw msg;
            }
        }
    }

    const instances: { [hashCode: number]: Instance } = {};

    const __start = function (hashCode: number, argsJson: string) {
        const i = instances[hashCode] = new Instance(hashCode, JSON.parse(argsJson));
        return i.start();
    };
    const __invoke = function (hashCode: number, hub: string, method: string, argsJson: string) {
        const i = instances[hashCode];
        if (i) {
            return i.invoke(hub, method, argsJson ? JSON.parse(argsJson) : []);
        }
    };
    const __stop = function (hashCode: number) {
        const i = instances[hashCode];
        if (i) {
            delete instances[hashCode];
            return i.stop();
        }
    };

    (<any>window).shipwreckBlazorSignalR = {
        start: __start,
        invoke: __invoke,
        stop: __stop,
    };
})();