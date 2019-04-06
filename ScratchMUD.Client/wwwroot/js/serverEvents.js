"use strict";

document.getElementById("sendRandomMessages").disabled = true;

document.getElementById("connectEventHub").addEventListener("click", function (event) {
    var connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:5021/EventHub").build();

    connection.on("ReceiveMessage", function (message) {
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;") + "\n";
        var textArea = document.getElementById("messagesTextArea");
        textArea.value += msg;
    });

    connection.start().catch(err => console.error(err.toString()));

    document.getElementById("connectEventHub").disabled = true;

    var sendMessagesButton = document.getElementById("sendRandomMessages");

    sendMessagesButton.addEventListener("click", function (event) {
        connection.invoke("StartRandomMessages", 10).catch(function (err) {
            return console.error(err.toString());
        });

        event.preventDefault();
    });

    sendMessagesButton.disabled = false;
});