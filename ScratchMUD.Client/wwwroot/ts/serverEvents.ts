import * as signalR from "@aspnet/signalr";

const sendTestMessagesButton: HTMLButtonElement = document.querySelector("#sendTestMessages");
const sendUserCommandButton: HTMLButtonElement = document.querySelector("#sendUserCommand");
const userCommandTextField: HTMLInputElement = document.querySelector("#userCommand");
const connectToSignalRButton: HTMLButtonElement = document.querySelector("#connectEventHub");
const mudOutputWindow: HTMLMainElement = document.querySelector("#mudOutputWindow");
const mudOutputList: HTMLUListElement = document.querySelector("#mudOutputList");
const userCommandForm: HTMLFormElement = document.querySelector("#userCommandForm");
const connection = new signalR.HubConnectionBuilder().withUrl("https://creatorsarelegionserver.azurewebsites.net/EventHub", 0).build();
//const connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:5021/EventHub", 0).build();


// While not connected to SignalR, disable the UI that relies upon it.
sendTestMessagesButton.disabled = true;
sendUserCommandButton.disabled = true;
userCommandTextField.disabled = true;

// Wire up the button to connect to SignalR.
document.getElementById("connectEventHub").addEventListener("click", function (event) {
    connection.on("ReceiveClientCreatedMessage", function (message: string) {
        var newItem = buildMudOutputItemWithMessage(message);
        newItem.classList.add("client-message");
        addMudOutputItemToWindowAndPreserveScrolling(newItem);
    });

    connection.on("ReceiveServerCreatedMessage", function (message: string) {
        var newItem = buildMudOutputItemWithMessage(message);
        newItem.classList.add("server-message");
        addMudOutputItemToWindowAndPreserveScrolling(newItem);
    });

    connection.on("ReceiveRoomMessage", function (message: string) {
        var newItem = buildMudOutputItemWithMessage(message);
        newItem.classList.add("room-message");
        addMudOutputItemToWindowAndPreserveScrolling(newItem);
    });

    connection.start().catch(err => console.error(err.toString()));

    // Enable the UI that relies on a connected SignalR hub.  These items should really be conditional based on a success though...
    connectToSignalRButton.disabled = true;
    userCommandTextField.disabled = false;

    enableTestMessagesButton();
    enableSendUserCommandButton();
});

function buildMudOutputItemWithMessage(message: string) {
    var encodedMessage = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var outputItem = document.createElement("li");
    var newTextNode = document.createTextNode(encodedMessage);
    outputItem.appendChild(newTextNode);
    outputItem.classList.add("mud-output-item");
    return outputItem;
}

function addMudOutputItemToWindowAndPreserveScrolling(item: HTMLLIElement) {
    var isUserScrolledToBottom = mudOutputWindow.scrollHeight - mudOutputWindow.clientHeight <= mudOutputWindow.scrollTop + 1;

    mudOutputList.appendChild(item);

    if (isUserScrolledToBottom) {
        mudOutputWindow.scrollTop = mudOutputWindow.scrollHeight - mudOutputWindow.clientHeight
    }
}

function enableTestMessagesButton() {
    sendTestMessagesButton.addEventListener("click", function (event) {
        connection.invoke("StartTestMessages", 10).catch(function (err: Error) {
            return console.error(err.toString());
        });

        event.preventDefault();
    });

    sendTestMessagesButton.disabled = false;
}

function enableSendUserCommandButton() {
    sendUserCommandButton.addEventListener("click", function (event) {
        const commandString = userCommandTextField.value;

        sendClientMessage(commandString);
        userCommandForm.reset();
        event.preventDefault();
    });

    sendUserCommandButton.disabled = false;
}

function sendClientMessage(message: string) {
    connection.invoke("RelayClientMessage", message).catch(function (err: Error) {
        return console.error(err.toString());
    });
}