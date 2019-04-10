"use strict";

// While not connected to SignalR, disable the UI that relies upon it.
document.getElementById("sendTestMessages").disabled = true;
document.getElementById("sendUserCommand").disabled = true;
document.getElementById("userCommand").disabled = true;

// Wire up the button to connect to SignalR.
document.getElementById("connectEventHub").addEventListener("click", function (event) {
    var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:5021/EventHub").build();

    connection.on("ReceiveClientCreatedMessage", function (message) {
        var newItem = buildMudOutputItemWithMessage(message);
        newItem.classList.add("client-message");
        addMudOutputItemToWindowAndPreserveScrolling(newItem);
    });

    connection.on("ReceiveServerCreatedMessage", function (message) {
        var newItem = buildMudOutputItemWithMessage(message);
        newItem.classList.add("server-message");
        addMudOutputItemToWindowAndPreserveScrolling(newItem);
    });

    connection.on("ReceiveRoomMessage", function (message) {
        var newItem = buildMudOutputItemWithMessage(message);
        newItem.classList.add("room-message");
        addMudOutputItemToWindowAndPreserveScrolling(newItem);
    });

    connection.start().catch(err => console.error(err.toString()));

    // Enable the UI that relies on a connected SignalR hub.  These items should really be conditional based on a success though...
    document.getElementById("connectEventHub").disabled = true;
    document.getElementById("userCommand").disabled = false;

    enableTestMessagesButton(connection);
    enableSendUserCommandButton(connection);
});

this.buildMudOutputItemWithMessage = function (message) {
    var encodedMessage = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var outputItem = document.createElement("li");
    var newTextNode = document.createTextNode(encodedMessage);
    outputItem.appendChild(newTextNode);
    outputItem.classList.add("mud-output-item");
    return outputItem;
}

this.addMudOutputItemToWindowAndPreserveScrolling = function (item) {
    var outputWindow = document.getElementById("mudOutputWindow");
    var isUserScrolledToBottom = outputWindow.scrollHeight - outputWindow.clientHeight <= outputWindow.scrollTop + 1;

    var mudOutputList = document.getElementById("mudOutputList");
    mudOutputList.appendChild(item);

    if (isUserScrolledToBottom) {
        outputWindow.scrollTop = outputWindow.scrollHeight - outputWindow.clientHeight
    }
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