
using CitizensFinancialGroup.Threvw.Common.Validation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CitizensFinancialGroup.Elements.ApplicationModel.CQRS.Commands {

    public abstract class CommandResultHandlerBase<T,TResult> : ICommandResultHandler<TResult>{
        public TResult HandleCommandResult(CommandResult result) {
            if (!(result is CommandResult<T> castedResult)) {
                throw new ArgumentException("Must be of required type.", "result");
            }
            return HandleCommandResult(castedResult);
        }
        protected abstract TResult HandleCommandResult(CommandResult<T> result);

        private bool IsSubclassOfGenericType(Type typeToCheck, Type generic) {
            while (typeToCheck != null && typeToCheck != typeof(object)) {
                var cur = typeToCheck.IsGenericType ? typeToCheck.GetGenericTypeDefinition() : typeToCheck;
                if (generic == cur) {
                    return true;
                }
                typeToCheck = typeToCheck.BaseType;
            }
            return false;
        }
    }

    public abstract class CommandResultHandlerBase<TResult> : ICommandResultHandler<TResult> {
        public virtual TResult? HandleCommandResult(CommandResult result) {

            if (ShouldHandleResult(result)) {
                if (IsSubclassOfGenericType(result.GetType(), typeof(CommandResult<>))) {
                    if (result.ValidationResult != null &&  result.ValidationResult.IsValid()) {
                        return CreateResult(((dynamic)result).Result);
                    }
                    else {
                        return CreateResult(((dynamic)result).Result, result.ValidationResult);
                    }
                }
                else {

                    if (result.ValidationResult != null &&  !result.ValidationResult.IsValid()) {
                        return CreateResult(result.ValidationResult);
                    }
                    else {
                        return CreateResult();
                    }                    
                }
            }
            return default;
        }
        protected abstract bool ShouldHandleResult(CommandResult result);

        protected virtual TResult? CreateResult(object model) {
            return default;
        }

        protected virtual TResult? CreateResult(object model, InternalValidationResult validationResult) {
            return default;
        }
        protected virtual TResult? CreateResult(InternalValidationResult validationResult) {
            return default;
        }
        protected virtual TResult? CreateResult() {
            return default;
        }

        private bool IsSubclassOfGenericType(Type typeToCheck, Type generic) {
            while (typeToCheck != null && typeToCheck != typeof(object)) {
                var cur = typeToCheck.IsGenericType ? typeToCheck.GetGenericTypeDefinition() : typeToCheck;
                if (generic == cur) {
                    return true;
                }
                typeToCheck = typeToCheck.BaseType;
            }
            return false;
        }
    }
}
