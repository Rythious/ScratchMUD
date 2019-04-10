"use strict";

// While not connected to SignalR, disable the UI that relies upon it.
document.getElementById("sendTestMessages").disabled = true;
document.getElementById("sendUserCommand").disabled = true;
document.getElementById("userCommand").disabled = true;

// Wire up the button to connect to SignalR.
document.getElementById("connectEventHub").addEventListener("click", function (event) {
    var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:5021/EventHub").build();

    connection.on("ReceiveMessage", function (message) {
        var newItem = buildMudOutputItemWithMessage(message);

        var outputWindow = document.getElementById("mudOutputWindow");
        var isUserScrolledToBottom = outputWindow.scrollHeight - outputWindow.clientHeight <= outputWindow.scrollTop + 1;

        var mudOutputList = document.getElementById("mudOutputList");
        mudOutputList.appendChild(newItem);

        if (isUserScrolledToBottom) {
            outputWindow.scrollTop = outputWindow.scrollHeight - outputWindow.clientHeight
        }
    });

    connection.start().catch(err => console.error(err.toString()));

    // Enable the UI that relies on a connected SignalR hub.  These items should really be conditional based on a success though...
    document.getElementById("connectEventHub").disabled = true;
    document.getElementById("userCommand").disabled = false;

    enableTestMessagesButton(connection);
    enableSendUserCommandButton(connection);
});

this.buildMudOutputItemWithMessage = function (message) {
    var encodedMessage = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;") + "\n";
    var outputItem = document.createElement("li");
    var newTextNode = document.createTextNode(encodedMessage);
    outputItem.appendChild(newTextNode);
    outputItem.classList.add("mudOutputItem");
    return outputItem;
}

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