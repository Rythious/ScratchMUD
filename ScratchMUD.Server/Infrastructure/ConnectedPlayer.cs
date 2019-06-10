﻿using ScratchMUD.Server.EntityFramework;
using System.Collections.Generic;

namespace ScratchMUD.Server.Infrastructure
{
    public class ConnectedPlayer
    {
        internal PlayerCharacter PlayerCharacter { get; set; }
        private readonly Queue<string> commandQueue = new Queue<string>();
        private readonly Queue<string> messageQueue = new Queue<string>();

        public ConnectedPlayer(PlayerCharacter playerCharacter)
        {
            PlayerCharacter = playerCharacter;
        }

        internal string Name => PlayerCharacter.Name;
        internal int RoomId => PlayerCharacter.RoomId;
        internal int CommandQueueCount => commandQueue.Count;
        internal int MessageQueueCount => messageQueue.Count;
        internal int PlayerCharacterId => PlayerCharacter.PlayerCharacterId;
        internal int Health { get; private set; } = 50;

        internal void QueueCommand(string commandName)
        {
            commandQueue.Enqueue(commandName);
        }

        internal string DequeueCommand()
        {
            return commandQueue.Dequeue();
        }

        internal void QueueMessage(string message)
        {
            messageQueue.Enqueue(message);
        }

        internal string DequeueMessage()
        {
            return messageQueue.Dequeue();
        }
    }
}