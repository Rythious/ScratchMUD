using ScratchMUD.Server.Exceptions;
using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;

namespace ScratchMUD.Server.Infrastructure
{
    public class EditingState
    {
        private Dictionary<string, EditType> playersCurrentlyEditing { get; } = new Dictionary<string, EditType>();

        // While I don't actually have players, the SignalR connection id is used as the key.
        internal virtual bool IsPlayerCurrentlyEditing(string signalRConnectionId, out EditType? editType)
        {
            editType = null;

            if (playersCurrentlyEditing.ContainsKey(signalRConnectionId))
            {
                editType = playersCurrentlyEditing[signalRConnectionId];

                return true;
            }

            return false;
        }

        internal virtual void AddPlayerEditor(string signalRConnectionId, EditType editType)
        {
            if (playersCurrentlyEditing.ContainsKey(signalRConnectionId))
            {
                throw new PlayerAlreadyEditingException(signalRConnectionId, playersCurrentlyEditing[signalRConnectionId]);
            }

            playersCurrentlyEditing.Add(signalRConnectionId, editType);
        }

        internal virtual void RemovePlayerEditor(string signalRConnectionId)
        {
            if (playersCurrentlyEditing.ContainsKey(signalRConnectionId))
            {
                playersCurrentlyEditing.Remove(signalRConnectionId);
            }
        }
    }
}