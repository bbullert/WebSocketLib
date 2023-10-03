const connection = new WebSocketHubConnection("wss://localhost:5001/ws/chat");

connection.connect("user#1");

connection.connected((event) => {
	console.log("connected", event);
	connection.invoke("JoinGroup", "group#1");
});

connection.disconnected((event) => {
	console.log("disconnected", event);
});

connection.error((event) => {
	console.log("error", event);
});

connection.invoke("SendMessageToGroup", "hello world", "group#1");

connection.on("ReceiveMessageToGroup", (message) => {
	console.log(message);
});
