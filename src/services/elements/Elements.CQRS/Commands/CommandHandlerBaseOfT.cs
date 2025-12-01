using System.Diagnostics;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Elements.CQRS.Commands {
    public abstract class CommandHandler<TCommand, TResult>
        where TResult : CommandResult
        where TCommand : CommandBase {

        private readonly string _sourceName;

        public CommandHandler() {
            _sourceName = $"[CommandHandler]-{GetType().Name}";
        }

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


        protected abstract Task<TResult> ProcessImpl(TCommand command);

    }

    public class FunctionCommandHandler<TCommand, TResult> : CommandHandler<TCommand, TResult> 
        where TResult : CommandResult
        where TCommand : CommandBase {

        private readonly Func<TCommand, Task<TResult>> _handler;

        public FunctionCommandHandler(Func<TCommand, Task<TResult>> handler) {
            _handler = handler;
        }

        protected override async Task<TResult> ProcessImpl(TCommand command) {
            return await _handler(command);
        }
    }
}
