using System.Diagnostics;
using System.Threading.Tasks;

namespace NOCO.Elements.CQRS.Commands {
    public abstract class CommandHandlerBase {

        private readonly string _sourceName;

        public CommandHandlerBase() {
            _sourceName = $"[CommandHandler]-{GetType().Name}";
        }

        public virtual async Task<CommandResult> Process(CommandBase command) {

            var activitySource = new ActivitySource(_sourceName);

            using (var activity = activitySource.StartActivity("Process", ActivityKind.Internal)) {
                try {
                    return await ProcessImpl(command);
                }
                catch (Exception e) { 
                    activity?.AddException(e);
                    throw;
                }
            }

        }

        protected abstract Task<CommandResult> ProcessImpl(CommandBase command);
    }

    public class FunctionCommandHandler : CommandHandlerBase {

        private readonly Func<CommandBase, Task<CommandResult>> _handler;

        public FunctionCommandHandler(Func<CommandBase, Task<CommandResult>> handler) {
            _handler = handler;
        }

        protected override async Task<CommandResult> ProcessImpl(CommandBase command) {
            return await _handler(command);
        }
    }
}
