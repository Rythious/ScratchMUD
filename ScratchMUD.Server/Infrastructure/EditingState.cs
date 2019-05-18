using ScratchMUD.Server.Exceptions;
using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;

namespace ScratchMUD.Server.Infrastructure
{
    public class EditingState
    {
        private Dictionary<string, EditType> PlayersCurrentlyEditing { get; } = new Dictionary<string, EditType>();

        // While I don't actually have players, the SignalR connection id is used as the key.
        internal virtual bool IsPlayerCurrentlyEditing(string signalRConnectionId, out EditType? editType)
        {
            editType = null;

            if (PlayersCurrentlyEditing.ContainsKey(signalRConnectionId))
            {
                editType = PlayersCurrentlyEditing[signalRConnectionId];

                return true;
            }

            return false;
        }

        internal virtual void AddPlayerEditor(string signalRConnectionId, EditType editType)
        {
            if (PlayersCurrentlyEditing.ContainsKey(signalRConnectionId))
            {
                throw new PlayerAlreadyEditingException(signalRConnectionId, PlayersCurrentlyEditing[signalRConnectionId]);
            }

            PlayersCurrentlyEditing.Add(signalRConnectionId, editType);
        }

        internal virtual void RemovePlayerEditor(string signalRConnectionId)
        {
            if (PlayersCurrentlyEditing.ContainsKey(signalRConnectionId))
            {
                PlayersCurrentlyEditing.Remove(signalRConnectionId);
            }
        }
    }
}