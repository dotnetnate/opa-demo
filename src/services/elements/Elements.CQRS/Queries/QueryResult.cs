using CitizensFinancialGroup.Elements.CQRS.Commands;
using CitizensFinancialGroup.Threvw.Common.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Elements.CQRS.Queries {
    
    public enum NullResultAdvices {
        TreatAsSuccess = 0,
        TreatAsNotFound = 10,
        Undefined = 20
    }
    
    
    public class QueryResult<TResult> {

        /// <summary>
        /// Gets or sets how null results should be treated.
        /// </summary>
        public NullResultAdvices NullResultAdvice { get; set; } = NullResultAdvices.TreatAsSuccess;

        /// <summary>
        /// The status of the command execution, i.e. success or failure.
        /// </summary>
        public QueryStatus Status { get; set; }
        /// <summary>
        /// Provides basic failure category information in the event of a failure.
        /// </summary>
        public QueryFailureCategory FailureCategory { get; set; }
        /// <summary>
        /// Provides the list of validation errors that occurred during execution. These can be
        /// either data or process validation.
        /// </summary>
        public InternalValidationResult? ValidationResult { get; set; }
        /// <summary>
        /// Gets the collection of any exceptions that occurred during processing.
        /// </summary>
        public Exception? ExecutionException { get; set; }
        /// <summary>
        /// Gets a set a list of error messages that occurred during processing.
        /// </summary>
        public IEnumerable<string>? ErrorMessages { get; set; }
        /// <summary>
        /// Contains the results of the query, if successful.
        /// </summary>
        public TResult?     Result { get; set; }

        public bool IsSuccess() => Status == QueryStatus.Succeeded;

        #region Static Creation Helpers      
        /// <summary>
        /// Returns a default result indicating that the result is unknown.
        /// </summary>
        /// <returns>Returns an instance of <see cref="QueryResult"/> indicating that the status of the command is unknown.</returns>
        public static QueryResult<TResult> UnknownResult(TResult? result = default) {
            return CreateResult(QueryStatus.Unknown, result);
        }
        /// <summary>
        /// Returns a default result indicating success..
        /// </summary>
        /// <returns>Returns an instance of <see cref="QueryResult"/> corresponding to a successful completion of a command.</returns>
        public static QueryResult<TResult> SuccessResult(TResult? result = default) {
            return CreateResult(QueryStatus.Succeeded, result);
        }

        /// <summary>
        /// Returns a default result indicating success..
        /// </summary>
        /// <returns>Returns an instance of <see cref="QueryResult"/> corresponding to a successful completion of a command.</returns>
        public static QueryResult<TResult> NotFoundResult(TResult? result = default) {
            return CreateResult(QueryStatus.Failed, result, failureCategory: QueryFailureCategory.ResourceNotFound);
        }

        /// <summary>
        /// Returns a default result indicating that there is a parameter validation failure.
        /// </summary>
        /// <param name="validationResult">The result of the command validation.</param>
        /// <returns>Returns an instance of <see cref="QueryResult"/> corresponding to a command resulting in validation failure of the command.</returns>
        public static QueryResult<TResult> ValidationFailureResult(InternalValidationResult validationResult, TResult? result = default) {
            return CreateResult(QueryStatus.Failed, result, QueryFailureCategory.ParameterValidation, validationResult);
        }
        /// <summary>
        /// Returns a default result indicating that there is a general runtime error.
        /// </summary>
        /// <param name="errorMessages">(Optional) The error messages that occurred during processing.)</param>
        /// <param name="executionException">(Optional) The exception that occurred during processing.</param>
        /// <param name="failureCategory">The category to use when creating the result, if it's a failure..</param>
        /// <returns>Returns an instance of <see cref="QueryResult"/> corresponding to a general failure.</returns>
        public static QueryResult<TResult> FailureResult(TResult? result = default, InternalValidationResult? validationResult = null, IEnumerable<string>? errorMessages = null, Exception? executionException = null, QueryFailureCategory failureCategory = QueryFailureCategory.GeneralRuntimeError) {
            return CreateResult(QueryStatus.Failed, result, failureCategory, validationResult, errorMessages, executionException);
        }
        /// <summary>
        /// Returns a default result indicating that there is a general runtime error.
        /// </summary>
        /// <param name="status">The status of the operation.</param>
        /// <param name="failureCategory">The category to use when creating the result, if it's a failure..</param>
        /// <param name="validationResult">The result of the command validation.</param>
        /// <param name="errorMessages">(Optional) The error messages that occurred during processing.)</param>
        /// <param name="executionException">(Optional) The exception that occurred during processing.</param>
        /// <returns></returns>
        private static QueryResult<TResult> CreateResult(QueryStatus status, TResult? result, QueryFailureCategory failureCategory = QueryFailureCategory.None, InternalValidationResult? validationResult = null, IEnumerable<string>? errorMessages = null, Exception? executionException = null) {
            return new QueryResult<TResult> { Status = status, ValidationResult = validationResult, ErrorMessages = errorMessages, ExecutionException = executionException, FailureCategory = failureCategory, Result = result };
        }
        #endregion

    }
}
