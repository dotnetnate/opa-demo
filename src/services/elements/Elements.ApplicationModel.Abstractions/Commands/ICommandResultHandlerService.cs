
namespace CitizensFinancialGroup.Elements.ApplicationModel.Commands {
    public interface ICommandResultHandlerService<TResult> {
        TResult? HandleCommandResult(CommandResult result);        
    }
}
