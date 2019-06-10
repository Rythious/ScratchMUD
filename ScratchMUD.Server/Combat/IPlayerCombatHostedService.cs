using ScratchMUD.Server.Infrastructure;
using System.Threading.Tasks;

namespace ScratchMUD.Server.Combat
{
    public interface IPlayerCombatHostedService
    {
        Task StartTrackingAltercation(Altercation altercation);
    }
}