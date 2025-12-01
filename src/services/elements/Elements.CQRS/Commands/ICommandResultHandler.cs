
namespace CitizensFinancialGroup.Elements.CQRS.Commands {
    public interface ICommandResultHandler<TResult> {
        TResult? HandleCommandResult(CommandResult result);
    }
}
