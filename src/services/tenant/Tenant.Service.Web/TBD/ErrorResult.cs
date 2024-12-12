using Microsoft.AspNetCore.StaticFiles.Infrastructure;

namespace CitizensFinancialGroup.Threvw.Tenants.Service.Http.TBD {
    public class ErrorResult {
        public Features.Shared.Models.InternalValidationResult? ValidationResult { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
