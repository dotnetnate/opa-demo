
namespace NOCO.Elements.ApplicationModel.CQRS.Commands {
    public interface ICommandResultHandlerService<TResult> {
        TResult? HandleCommandResult(CommandResult result);        
    }
}
