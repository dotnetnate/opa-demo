using System.Diagnostics;
using System.Threading.Tasks;

namespace NOCO.Elements.ApplicationModel.Commands {
    /// <summary>
    /// Abstract base class for handling commands.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public abstract class CommandHandler<TCommand, TResult>
        where TResult : CommandResult
        where TCommand : CommandBase {

        private readonly string _sourceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandler{TCommand, TResult}"/> class.
        /// </summary>
        public CommandHandler() {
            _sourceName = $"[CommandHandler]-{GetType().Name}";
        }

        /// <summary>
        /// Processes the specified command.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the command.</returns>
        public virtual async Task<TResult> Process(TCommand command) {

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
        /// When overridden in a derived class, processes the command.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the command.</returns>
        protected abstract Task<TResult> ProcessImpl(TCommand command);

    }

    /// <summary>
    /// A command handler that uses a function to process commands.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class FunctionCommandHandler<TCommand, TResult> : CommandHandler<TCommand, TResult>
        where TResult : CommandResult
        where TCommand : CommandBase {

        private readonly Func<TCommand, Task<TResult>> _handler;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionCommandHandler{TCommand, TResult}"/> class.
        /// </summary>
        /// <param name="handler">The function to handle the command.</param>
        public FunctionCommandHandler(Func<TCommand, Task<TResult>> handler) {
            _handler = handler;
        }

        /// <summary>
        /// Processes the command using the provided function.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the result of the command.</returns>
        protected override async Task<TResult> ProcessImpl(TCommand command) {
            return await _handler(command);
        }
    }
}
