
namespace NOCO.Elements.CQRS.Commands {
    public interface ICommandResultHandlerService<TResult> {
        TResult? HandleCommandResult(CommandResult result);        
    }
}
