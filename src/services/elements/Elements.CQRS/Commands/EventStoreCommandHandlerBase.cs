//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace NOCO.Elements.CQRS.Commands {
//    public abstract class EventStoreCommandHandler<TCommand, TResult, TEvent> : CommandHandler<TCommand, TResult>
//        where TCommand : CommandBase
//        where TResult : CommandResult, new() {

//        private IEventRepository m_eventRepository;

//        public EventStoreCommandHandler(IEventRepository eventRepository) {
//            m_eventRepository = eventRepository;
//        }

//        public override async Task<TResult> Process(TCommand command) {

//            TResult retVal = new TResult {                
//                Status = CommandStatus.Succeeded
//            };

//            if (command != null) {

//                retVal.CorrelationId = command.CorrelationId;

//                var (succeeded, errors) = command.Validate();

//                if (succeeded) {
//                    var eventToPublish = CreateEventFromCommand(command);

//                    bool isEventGeneric = IsSubclassOfGenericType(eventToPublish.GetType(), typeof(EventBase<>));
//                    bool isResultGeneric = IsSubclassOfGenericType(retVal.GetType(), typeof(CommandResult<>));

//                    if( isEventGeneric && isResultGeneric) { 
//                        ((dynamic)retVal).Result = ((dynamic)eventToPublish).Model;
//                    }

//                    await m_eventRepository.Publish(eventToPublish);
     
//                }
//                else {
//                    retVal.Status = CommandStatus.Failed;
//                    retVal.FailureCategory = CommandFailureCategory.ParameterValidation;
//                    retVal.ValidationErrors = errors;
//                }
//            }
//            else {
//                retVal.Status = CommandStatus.Failed;
//                retVal.FailureCategory = CommandFailureCategory.ParameterValidation;
//            }

//            return retVal;

//        }

//        protected abstract EventBase CreateEventFromCommand(TCommand command);

//        private bool IsSubclassOfGenericType(Type typeToCheck, Type generic) {
//            while (typeToCheck != null && typeToCheck != typeof(object)) {
//                var cur = typeToCheck.IsGenericType ? typeToCheck.GetGenericTypeDefinition() : typeToCheck;
//                if (generic == cur) {
//                    return true;
//                }
//                typeToCheck = typeToCheck.BaseType;
//            }
//            return false;
//        }
//    }
//}
