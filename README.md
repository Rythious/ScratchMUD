# ScratchMUD
Creating a MUD from scratch sounds like a fun challenge.  An extensible MUD framework with a web client seems like something that no one needs and is a good testing ground for all the technologies I'd like to mess around with, like SignalR, Azure DevOps, and Razor Pages.

## Goals
This project will generate a generic MUD server that allows a user to create a new MUD through configuration.  They will be able to pick which systems are included their world such as hunger, random attacks, out of character chat, magic, etc.  In-game editors will be provided to allow players in a world to create new items, npcs, and areas.

Along side this generic MUD server will be a generic responsive web based MUD client.  The client will provide credential management, player connections, and UI.  Like the configurable nature of the server, a player who has created a world will be able to configure the styling of the client and preset hotkey buttons.  Individual players, regardless of world creation will be able to customize their hotkey buttons to whatever string commands they'd like and they'll be stored per world.

Finally, this project will provide a default implementation of all these features in a system-heavy world that is editable by all players.
