using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Elements.Validation {


    public class InternalValidationError {
        public string? PropertyName { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorCode { get; set; }
    }

    public class InternalValidationResult {
        public ICollection<InternalValidationError> Errors { get; set; } = new List<InternalValidationError>();
        public bool IsValid() => !Errors.Any();
    }


    public interface IValidationService {
        Task<InternalValidationResult> Validate<T>(T objectToValidate);
    }

    public abstract class ValidationServiceBase : IValidationService {
        public virtual async Task<InternalValidationResult> Validate<T>(T objectToValidate) {
            return await ValidateImpl(objectToValidate);
        }

        protected abstract Task<InternalValidationResult> ValidateImpl<T>(T objectToValidate);
    }

    
}
