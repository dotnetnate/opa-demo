using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Common {

    public enum OperationOutcomes {
        Success = 0,
        Received = 100, 
        Error = 200,
        Conflict = 210,
        Unknown = 300
    }

    public interface IOperationResult {
        public OperationOutcomes Outcome { get; set; }
        public IEnumerable<ValidationResult> ValidationErrors { get; set; }
        public IEnumerable<string> Errors { get; set; } 
    }


    public class Result : IOperationResult {
        public OperationOutcomes Outcome { get; set; } = OperationOutcomes.Unknown;
        public IEnumerable<ValidationResult> ValidationErrors { get; set; } = new List<ValidationResult>();
        public IEnumerable<string> Errors { get; set; } = new List<string>();

        public static Result SuccessResult() {
            return new Result { Outcome = OperationOutcomes.Success };
        }

        public static Result FailureResult(IEnumerable<ValidationResult> errors) {
            return new Result { Outcome = OperationOutcomes.Error, ValidationErrors = errors };
        }
        public static Result FailureResult(IEnumerable<string> errors) {
            return new Result { Outcome = OperationOutcomes.Error, Errors = errors };
        }
        public static Result FailureResult(IEnumerable<ValidationResult> validationErrors, IEnumerable<string> errors) {
            return new Result { Outcome = OperationOutcomes.Error, ValidationErrors = validationErrors, Errors = errors };
        }

        public bool IsSuccess() {
            return Outcome == OperationOutcomes.Success || Outcome == OperationOutcomes.Received;
        }
    }

    public class Result<T> : IOperationResult {
        public T? Data { get; set; } = default;
        public OperationOutcomes Outcome { get; set; } = OperationOutcomes.Unknown;
        public IEnumerable<ValidationResult> ValidationErrors { get; set; } = new List<ValidationResult>();
        public IEnumerable<string> Errors { get; set; } = new List<string>();

        public static Result<T> SuccessResult(T? data) {
            return new Result<T> { Outcome = OperationOutcomes.Success, Data = data };
        }

        public static Result<T> FailureResult(IEnumerable<ValidationResult> errors) {
            return new Result<T> { Outcome = OperationOutcomes.Error, ValidationErrors = errors };
        }
        public static Result<T> FailureResult(IEnumerable<string> errors) {
            return new Result<T> { Outcome = OperationOutcomes.Error, Errors = errors };
        }
        public static Result<T> FailureResult(IEnumerable<ValidationResult> validationErrors, IEnumerable<string> errors) {
            return new Result<T> { Outcome = OperationOutcomes.Error, ValidationErrors = validationErrors, Errors = errors };
        }

        public bool IsSuccess() {
            return Outcome == OperationOutcomes.Success || Outcome == OperationOutcomes.Received;
        }
    }
}
