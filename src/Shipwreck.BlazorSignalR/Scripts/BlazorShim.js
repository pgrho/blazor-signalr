var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/signalr/signalr.d.ts" />
/// <reference path="typings/Blazor.d.ts" />
/// <reference path="typings/es6-promise/es6-promise.d.ts" />
(function () {
    var Instance = /** @class */ (function () {
        function Instance(hashCode, args) {
            var _this = this;
            this.hashCode = hashCode;
            var c = this._connection = $.hubConnection(args.Url, {
                qs: args.QueryString,
                logging: args.Logging,
                useDefaultPath: args.UseDefaultPath
            });
            c.connectionSlow(function () { return DotNet.invokeMethodAsync('Shipwreck.BlazorSignalR', 'OnSignalRConnectionSlow', _this.hashCode); });
            c.disconnected(function () { return DotNet.invokeMethodAsync('Shipwreck.BlazorSignalR', 'OnSignalRDisconnected', _this.hashCode); });
            c.error(function (e) { return DotNet.invokeMethodAsync('Shipwreck.BlazorSignalR', 'OnSignalRError', _this.hashCode, JSON.stringify({
                readyState: e.context ? e.context.readyState : null,
                responseText: e.context ? e.context.responseText : null,
                status: e.context ? e.context.status : null,
                statusText: e.context ? e.context.statusText : null,
                message: e.message,
                name: e.name,
                source: e.source,
                stack: e.stack,
                transport: e.transport,
            })); });
            c.reconnected(function () { return DotNet.invokeMethodAsync('Shipwreck.BlazorSignalR', 'OnSignalRReconnected', _this.hashCode); });
            c.reconnecting(function () { return DotNet.invokeMethodAsync('Shipwreck.BlazorSignalR', 'OnSignalRReconnecting', _this.hashCode); });
            c.stateChanged(function (e) { return DotNet.invokeMethodAsync('Shipwreck.BlazorSignalR', 'OnSignalRStateChanged', _this.hashCode, e.oldState, e.newState); });
            this._proxies = {};
            var _self = this;
            for (var _i = 0, _a = args.Hubs; _i < _a.length; _i++) {
                var h = _a[_i];
                var p = this._proxies[h.Name] = c.createHubProxy(h.Name);
                if (h.Callbacks) {
                    for (var _b = 0, _c = h.Callbacks; _b < _c.length; _b++) {
                        var c_1 = _c[_b];
                        p.on(c_1.Name, (function (h, c) {
                            return function () {
                                var args = [];
                                for (var i = 0; i < c.Length; i++) {
                                    args[i] = arguments[i];
                                }
                                _self._on(h.Name, c.Name, args);
                            };
                        })(h, c_1));
                    }
                }
            }
        }
        Instance.prototype.start = function () {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, this._connection.start()];
                        case 1:
                            _a.sent();
                            return [2 /*return*/];
                    }
                });
            });
        };
        Instance.prototype.invoke = function (hub, method, args) {
            return __awaiter(this, void 0, void 0, function () {
                var p, r;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            p = this._proxies[hub];
                            if (!p) return [3 /*break*/, 2];
                            return [4 /*yield*/, p.invoke.apply(p, __spreadArrays([method], args))];
                        case 1:
                            r = _a.sent();
                            return [2 /*return*/, r === undefined ? null : JSON.stringify(r)];
                        case 2: return [2 /*return*/];
                    }
                });
            });
        };
        Instance.prototype.stop = function () {
            return __awaiter(this, void 0, void 0, function () {
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0: return [4 /*yield*/, this._connection.stop(true, true)];
                        case 1:
                            _a.sent();
                            return [2 /*return*/];
                    }
                });
            });
        };
        Instance.prototype._on = function (hubName, eventName, args) {
            return __awaiter(this, void 0, void 0, function () {
                var ex_1, msg;
                return __generator(this, function (_a) {
                    switch (_a.label) {
                        case 0:
                            _a.trys.push([0, 2, , 3]);
                            return [4 /*yield*/, DotNet.invokeMethodAsync('Shipwreck.BlazorSignalR', 'OnSignalREvent', this.hashCode, hubName, eventName, JSON.stringify(args))];
                        case 1:
                            _a.sent();
                            return [3 /*break*/, 3];
                        case 2:
                            ex_1 = _a.sent();
                            msg = "An error caught while receiving SignalR event " + hubName + "#" + eventName;
                            console.error(msg, args, ex_1);
                            throw msg;
                        case 3: return [2 /*return*/];
                    }
                });
            });
        };
        return Instance;
    }());
    var instances = {};
    var __start = function (hashCode, argsJson) {
        var i = instances[hashCode] = new Instance(hashCode, JSON.parse(argsJson));
        return i.start();
    };
    var __invoke = function (hashCode, hub, method, argsJson) {
        var i = instances[hashCode];
        if (i) {
            return i.invoke(hub, method, argsJson ? JSON.parse(argsJson) : []);
        }
    };
    var __stop = function (hashCode) {
        var i = instances[hashCode];
        if (i) {
            delete instances[hashCode];
            return i.stop();
        }
    };
    window.shipwreckBlazorSignalR = {
        start: __start,
        invoke: __invoke,
        stop: __stop,
    };
})();
//# sourceMappingURL=BlazorShim.js.map