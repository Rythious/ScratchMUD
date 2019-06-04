using ScratchMUD.Server.Infrastructure;
using ScratchMUD.Server.Models.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Repositories
{
    public interface ICommandRepository
    {
        Task<IEnumerable<(CommunicationChannel, string)>> ExecuteCommandAsync(ConnectedPlayer connectedPlayer, List<ConnectedPlayer> playersInTheRoom, string command, params string[] parameters);
    }
}