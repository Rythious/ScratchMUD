# ScratchMUD
Creating a MUD from scratch sounds like a fun challenge.  An extensible MUD framework with a web client seems like something that no one needs and is a good testing ground for all the technologies I'd like to mess around with, like SignalR, Azure DevOps, and Razor Pages.

## Goals
This project will generate a MUD server that will allow world editing in-game.  World editing in this case is defined as being able to create areas, rooms, non player characters (NPCs), and items all while maintaining a playable game.  Players will also have the ability to create their own world, bringing with them any creations they'd like.  Though the default implementation of the world will have every system that was created in this project, a newly created world will be able to pick which systems are included their world such as hunger, random attacks, out of character chat, magic, etc.  In-game editors will be provided to allow players in a world to create new items, NPCs, and areas.

Along side this MUD server will be a generic responsive web based MUD client.  The client will provide credential management, player connections, and UI.  Like the configurable nature of a player created world, a player who has created a world will be able to configure the styling of the client and preset hotkey buttons.  Individual players, regardless of world creation will be able to customize their hotkey buttons to whatever string commands they'd like and they'll be stored per world.
