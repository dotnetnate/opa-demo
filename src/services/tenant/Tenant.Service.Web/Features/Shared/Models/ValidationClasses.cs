namespace NOCO.Threvw.Tenants.Service.Http.Features.Shared.Models {
    public class InternalValidationError {
        public string? PropertyName { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorCode { get; set; }
    }

    public class InternalValidationResult {
        public ICollection<InternalValidationError> Errors { get; set; } = new List<InternalValidationError>();        
    }
}
