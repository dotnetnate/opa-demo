namespace NOCO.Threvw.Policy.Service.Http.Features.Shared.Models {


    public class InternalValidationResult {
        public bool IsValid() => Errors.Count == 0; // Check if validation is successful
        public ICollection<InternalValidationError> Errors { get; set; } = new List<InternalValidationError>();
    }

    public class InternalValidationError {
        public string? PropertyName { get; set; } // Name of the property with the error
        public required string ErrorMessage { get; set; } // Error message

        public string? ErrorCode { get; set; }
    }
}
