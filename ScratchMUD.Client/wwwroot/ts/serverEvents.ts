import * as signalR from "@aspnet/signalr";

const sendUserCommandButton: HTMLButtonElement = document.querySelector("#sendUserCommand");
const userCommandTextField: HTMLInputElement = document.querySelector("#userCommand");
const mudOutputWindow: HTMLMainElement = document.querySelector("#mudOutputWindow");
const mudOutputList: HTMLUListElement = document.querySelector("#mudOutputList");
const userCommandForm: HTMLFormElement = document.querySelector("#userCommandForm");
const connection = new signalR.HubConnectionBuilder().withUrl("https://creatorsarelegionserver.azurewebsites.net/EventHub", 0).build();
//const connection = new signalR.HubConnectionBuilder().withUrl("https://localhost:5021/EventHub", 0).build();

// While not connected to SignalR, disable the UI that relies upon it.
sendUserCommandButton.disabled = true;
userCommandTextField.disabled = true;

// Connect to SignalR.
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
userCommandTextField.disabled = false;

enableSendUserCommandButton();

function buildMudOutputItemWithMessage(message: string) {
    var outputItem = document.createElement("li");
    var newTextNode = document.createTextNode(message);
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