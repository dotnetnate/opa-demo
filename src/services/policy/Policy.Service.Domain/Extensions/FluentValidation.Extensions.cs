using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentValidation {
    public static class Extensions {
        public static List<ValidationResult> ToDataAnnotationsResult(this FluentValidation.Results.ValidationResult fluentValidationResult) {
            var results = new List<ValidationResult>();

            foreach (var error in fluentValidationResult.Errors) {
                var memberNames = new[] { error.PropertyName };
                var validationResult = new ValidationResult(error.ErrorMessage, memberNames);
                results.Add(validationResult);
            }

            return results;
        }
    }
}
