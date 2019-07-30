/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="typings/signalr/signalr.d.ts" />
/// <reference path="typings/es6-promise/es6-promise.d.ts" /> 
const b1 = $('#connect');
const b2 = $('#disconnect');
const b3 = $('#ping');
const t = $('#text');

let connection: SignalR.Hub.Connection;
let proxy: SignalR.Hub.Proxy;

function __append(l: string) {
    t.val(l + "\n" + t.val());
}

function __toggleState() {
    b1.prop('disabled', !!connection);
    b2.prop('disabled', !connection);
    b3.prop('disabled', !connection);
}

b1.click(async function () {
    try {
        connection = $.hubConnection();
        proxy = connection.createHubProxy('pingpong');
        proxy.on('Connected', (c) => __append(`${c} Connected`));
        proxy.on('Pong', (c, t) => __append(`Pong from [${c}]@[${t}]`));
        proxy.on('Disconnected', (args) => __append(`${args.ConnectionId} Disconnected(${args.StopCalled})@${args.Timestamp}`));
        __append('Starting..');
        await connection.start();
        __append('Started..');

    } catch (ex) {
        connection = null;
        proxy = null;
        __append('Exception caught: ' + ex);
    }

    __toggleState();
});

b2.click(function () {
    if (connection) {
        try {
            __append('Stopping..');
            connection.stop(true, true);
            connection = null;
            proxy = null;
        } catch (ex) {
            connection = null;
            proxy = null;
            __append('Exception caught: ' + ex);
        }
    }
    __toggleState();
});

b3.click(async function () {
    if (proxy) {
        try {
            __append('Pinging..');
            const r = await proxy.invoke('Ping', $('#client').val(), $('#count').val());
            __append('Ping completed:' + r);
        } catch (ex) {
            __append('Exception caught: ' + ex);
        }
    }
});

__toggleState();
