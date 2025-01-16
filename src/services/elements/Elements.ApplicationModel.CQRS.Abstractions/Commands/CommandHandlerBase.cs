using System.Diagnostics;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Elements.ApplicationModel.CQRS.Commands {
    /// <summary>
    /// Base class for handling commands.
    /// </summary>
    public abstract class CommandHandlerBase {

        private readonly string _sourceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerBase"/> class.
        /// </summary>
        public CommandHandlerBase() {
            _sourceName = $"[CommandHandler]-{GetType().Name}";
        }

        /// <summary>
        /// Processes the specified command.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the command result.</returns>
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

        /// <summary>
        /// Processes the command implementation.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the command result.</returns>
        protected abstract Task<CommandResult> ProcessImpl(CommandBase command);
    }

    /// <summary>
    /// A command handler that uses a function to process commands.
    /// </summary>
    public class FunctionCommandHandler : CommandHandlerBase {

        private readonly Func<CommandBase, Task<CommandResult>> _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionCommandHandler"/> class.
        /// </summary>
        /// <param name="handler">The function to handle the command.</param>
        public FunctionCommandHandler(Func<CommandBase, Task<CommandResult>> handler) {
            _handler = handler;
        }

        /// <summary>
        /// Processes the command implementation using the provided function.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the command result.</returns>
        protected override async Task<CommandResult> ProcessImpl(CommandBase command) {
            return await _handler(command);
        }
    }
}
