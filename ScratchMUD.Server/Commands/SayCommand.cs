using System.Threading.Tasks;

namespace ScratchMUD.Server.Commands
{
    internal class SayCommand : ICommand
    {
        internal const string NAME = "say";

        private readonly PlayerContext playerContext;

        public SayCommand(PlayerContext playerContext)
        {
            this.playerContext = playerContext;
        }

        public Task<string> ExecuteAsync(params string[] parameters)
        {
            string output = string.Empty;

            if (parameters.Length > 0)
            {
                output = $"{playerContext.Name} says \"{parameters[0]}\"";
            }

            return Task.Run(() => output);
        }
    }
}