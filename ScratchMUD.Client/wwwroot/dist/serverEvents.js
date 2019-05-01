"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var signalR = require("@aspnet/signalr");
var sendTestMessagesButton = document.querySelector("#sendTestMessages");
var sendUserCommandButton = document.querySelector("#sendUserCommand");
var userCommandTextField = document.querySelector("#userCommand");
var connectToSignalRButton = document.querySelector("#connectEventHub");
var mudOutputWindow = document.querySelector("#mudOutputWindow");
var mudOutputList = document.querySelector("#mudOutputList");
var userCommandForm = document.querySelector("#userCommandForm");
var connection = new signalR.HubConnectionBuilder().withUrl("https://creatorsarelegionserver.azurewebsites.net/EventHub", 0).build();
//const connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:5021/EventHub", 0).build();
// While not connected to SignalR, disable the UI that relies upon it.
sendTestMessagesButton.disabled = true;
sendUserCommandButton.disabled = true;
userCommandTextField.disabled = true;
// Wire up the button to connect to SignalR.
document.getElementById("connectEventHub").addEventListener("click", function (event) {
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
    connection.start().catch(function (err) { return console.error(err.toString()); });
    // Enable the UI that relies on a connected SignalR hub.  These items should really be conditional based on a success though...
    connectToSignalRButton.disabled = true;
    userCommandTextField.disabled = false;
    enableTestMessagesButton();
    enableSendUserCommandButton();
});
function buildMudOutputItemWithMessage(message) {
    var encodedMessage = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var outputItem = document.createElement("li");
    var newTextNode = document.createTextNode(encodedMessage);
    outputItem.appendChild(newTextNode);
    outputItem.classList.add("mud-output-item");
    return outputItem;
}
function addMudOutputItemToWindowAndPreserveScrolling(item) {
    var isUserScrolledToBottom = mudOutputWindow.scrollHeight - mudOutputWindow.clientHeight <= mudOutputWindow.scrollTop + 1;
    mudOutputList.appendChild(item);
    if (isUserScrolledToBottom) {
        mudOutputWindow.scrollTop = mudOutputWindow.scrollHeight - mudOutputWindow.clientHeight;
    }
}
function enableTestMessagesButton() {
    sendTestMessagesButton.addEventListener("click", function (event) {
        connection.invoke("StartTestMessages", 10).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });
    sendTestMessagesButton.disabled = false;
}
function enableSendUserCommandButton() {
    sendUserCommandButton.addEventListener("click", function (event) {
        var commandString = userCommandTextField.value;
        sendClientMessage(commandString);
        userCommandForm.reset();
        event.preventDefault();
    });
    sendUserCommandButton.disabled = false;
}
function sendClientMessage(message) {
    connection.invoke("RelayClientMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
}
