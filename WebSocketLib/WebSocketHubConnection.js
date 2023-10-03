export class WebSocketHubConnection {
	url = "";
	socket = null;
	callbacks = [];

	constructor(url) {
		this.url = url;
	}

	connect(userId) {
		const params = { userId: userId };
		const url = this.url + "?" + new URLSearchParams(params).toString();
		const ref = this;

		this.socket = new WebSocket(url);
		this.socket.onopen = (event) => {
			const callbacks = ref.#getCallbacks("connected");
			for (const callback of callbacks) {
				callback(event);
			}
		};
		this.socket.onclose = (event) => {
			const callbacks = ref.#getCallbacks("disconnected");
			for (const callback of callbacks) {
				callback(event);
			}
		};
		this.socket.onerror = (event) => {
			const callbacks = ref.#getCallbacks("error");
			for (const callback of callbacks) {
				callback(event);
			}
		};
		this.socket.onmessage = (event) => {
			const payload = JSON.parse(event.data);
			const callbacks = ref.#getCallbacks("on" + payload.actionName);
			for (const callback of callbacks) {
				callback(...payload.data);
			}
		};
	}

	connected(fn) {
		this.#registerCallback("connected", fn);
	}

	disconnected(fn) {
		this.#registerCallback("disconnected", fn);
	}

	error(fn) {
		this.#registerCallback("error", fn);
	}

	on(actionName, fn) {
		this.#registerCallback("on" + actionName, fn);
	}

	invoke(...args) {
		if (!args || args.length == 0) return;
		if (!this.socket || this.socket.readyState != WebSocket.OPEN) return;

		const actionName = args[0];
		const data = args.length > 1 ? args.slice(1, args.length) : [];
		const payload = JSON.stringify({ actionName: actionName, data: data });
		this.socket.send(payload);
	}

	#registerCallback(name, fn) {
		if (!this.callbacks[name]) this.callbacks[name] = [];
		this.callbacks[name].push(fn);
	}

	#getCallbacks(name) {
		return this.callbacks[name] ?? [];
	}
}
