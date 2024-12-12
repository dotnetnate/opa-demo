using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Common.Validation {
    public class FluentValidationValidationService : ValidationServiceBase {

        private readonly Dictionary<Type, object> _validators = new Dictionary<Type, object>();

        public FluentValidationValidationService() {
            LoadValidators();
        }

        protected override async Task<InternalValidationResult> ValidateImpl<T>(T objectToValidate) {


            var validationResult = new InternalValidationResult();

            if (_validators.TryGetValue(typeof(T), out var validator)) {
                var result = await ((IValidator<T>)validator).ValidateAsync(objectToValidate);

                if (!result.IsValid) {
                    foreach (var error in result.Errors) {
                        validationResult.Errors.Add(new InternalValidationError {
                            PropertyName = error.PropertyName,
                            ErrorMessage = error.ErrorMessage,
                            ErrorCode = error.ErrorCode
                        });
                    }
                }
            }

            return validationResult;
        }

        private void LoadValidators() {
            var validatorType = typeof(IValidator<>);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies) {
                var validatorTypes = assembly.GetTypes()
                    .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorType && !i.IsAbstract))
                    .ToList();

                foreach (var type in validatorTypes) {
                    var interfaceType = type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == validatorType);
                    var modelType = interfaceType.GetGenericArguments()[0];
                    var validatorInstance = Activator.CreateInstance(type);
                    if (validatorInstance != null) {
                        _validators[modelType] = validatorInstance;
                    }
                }
            }
        }
    }
}
