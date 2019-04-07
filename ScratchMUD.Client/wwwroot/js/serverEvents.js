"use strict";

document.getElementById("sendTestMessages").disabled = true;
document.getElementById("sendUserCommand").disabled = true;

document.getElementById("connectEventHub").addEventListener("click", function (event) {
    var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:5021/EventHub").build();

    connection.on("ReceiveMessage", function (message) {
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;") + "\n";
        var textArea = document.getElementById("messagesTextArea");
        textArea.value += msg;
    });

    connection.start().catch(err => console.error(err.toString()));

    document.getElementById("connectEventHub").disabled = true;

    enableTestMessagesButton(connection);
    enableSendUserCommandButton(connection);
});

this.enableTestMessagesButton = function (connection) {
    var sendMessagesButton = document.getElementById("sendTestMessages");

    sendMessagesButton.addEventListener("click", function (event) {
        connection.invoke("StartTestMessages", 10).catch(function (err) {
            return console.error(err.toString());
        });

        event.preventDefault();
    });

    sendMessagesButton.disabled = false;
}

this.enableSendUserCommandButton = function (connection) {
    var sendUserCommandButton = document.getElementById("sendUserCommand");

    sendUserCommandButton.addEventListener("click", function (event) {
        var userCommand = document.getElementById("userCommand").value;

        connection.invoke("RelayClientMessage", userCommand).catch(function (err) {
            return console.error(err.toString());
        });

        event.preventDefault();
    });

    sendUserCommandButton.disabled = false;
}