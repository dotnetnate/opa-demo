
using NOCO.Elements.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NOCO.Elements.ApplicationModel.CQRS.Commands {
    /// <summary>
    /// The result of executing a command.
    /// </summary>
    public class CommandResult {        
        /// <summary>
        /// The status of the command execution, i.e. success or failure.
        /// </summary>
        public CommandStatus                Status { get; set; }
        /// <summary>
        /// Provides basic failure category information in the event of a failure.
        /// </summary>
        public CommandFailureCategory       FailureCategory { get; set; }
        /// <summary>
        /// Provides the list of validation errors that occurred during execution. These can be
        /// either data or process validation.
        /// </summary>
        public InternalValidationResult?    ValidationResult { get; set; }
        /// <summary>
        /// Gets the collection of any exceptions that occurred during processing.
        /// </summary>
        public Exception?                   ExecutionException { get; set; }
        /// <summary>
        /// Gets a set a list of error messages that occurred during processing.
        /// </summary>
        public IEnumerable<string>?         ErrorMessages { get; set; }

        public bool IsSuccess() => Status == CommandStatus.Succeeded || Status == CommandStatus.Received;

        #region Static Creation Helpers

        /// <summary>
        /// Returns a default result indicating success..
        /// </summary>
        /// <returns>Returns an instance of <see cref="CommandResult"/> corresponding to a successful completion of a command.</returns>
        public static CommandResult SuccessResult() {
            return CreateResult(CommandStatus.Succeeded);            
        }
        /// <summary>
        /// Returns a default result indicating that the command was received successfully.
        /// </summary>
        /// <returns>Returns an instance of <see cref="CommandResult"/> corresponding to a successful receipt of a command.</returns>
        public static CommandResult ReceivedResult() {
            return CreateResult(CommandStatus.Received);
        }
        /// <summary>
        /// Returns a default result indicating that the result is unknown.
        /// </summary>
        /// <returns>Returns an instance of <see cref="CommandResult"/> indicating that the status of the command is unknown.</returns>
        public static CommandResult UnknownResult() {
            return CreateResult(CommandStatus.Unknown);
        }
        /// <summary>
        /// Returns a default result indicating that there is a resource conflict.
        /// </summary>
        /// <param name="validationResult">The result of the command validation.</param>
        /// <param name="errorMessages">(Optional) The error messages that occurred during processing.)</param>
        /// <param name="exception">(Optional) The exception that occurred during processing.</param>
        /// <returns>Returns an instance of <see cref="CommandResult"/> corresponding to a conflict when trying to creat or update a resource.</returns>
        public static CommandResult ResourceConflictResult(InternalValidationResult? validationResult = null, IEnumerable<string>? errorMessages = null, Exception? exception = null) {
            return CreateResult(CommandStatus.Failed, CommandFailureCategory.ResourceConflict, validationResult, errorMessages, exception);             
        }
        /// <summary>
        /// Returns a default result indicating that there is a version conflict.
        /// </summary>
        /// <param name="validationResult">The result of the command validation.</param>
        /// <param name="errorMessages">(Optional) The error messages that occurred during processing.)</param>
        /// <param name="exception">(Optional) The exception that occurred during processing.</param>
        /// <returns>Returns an instance of <see cref="CommandResult"/> corresponding to an error when trying to update a resource with stale information.</returns>
        public static CommandResult VersionConflictResult(InternalValidationResult? validationResult = null, IEnumerable<string>? errorMessages = null, Exception? exception = null) {
            return CreateResult(CommandStatus.Failed, CommandFailureCategory.VersionConlfict, validationResult, errorMessages, exception);            
        }
        /// <summary>
        /// Returns a default result indicating that there is a parameter validation failure.
        /// </summary>
        /// <param name="validationResult">The result of the command validation.</param>
        /// <returns>Returns an instance of <see cref="CommandResult"/> corresponding to a command resulting in validation failure of the command.</returns>
        public static CommandResult ValidationFailureResult(InternalValidationResult validationResult) {
            return CreateResult(CommandStatus.Failed, CommandFailureCategory.ParameterValidation, validationResult);            
        }
        /// <summary>
        /// Returns a default result indicating that there is a general runtime error.
        /// </summary>
        /// <param name="errorMessages">(Optional) The error messages that occurred during processing.)</param>
        /// <param name="executionException">(Optional) The exception that occurred during processing.</param>
        /// <param name="failureCategory">The category to use when creating the result, if it's a failure..</param>
        /// <returns>Returns an instance of <see cref="CommandResult"/> corresponding to a general failure.</returns>
        public static CommandResult FailureResult(InternalValidationResult? validationResult = null, IEnumerable<string>? errorMessages = null, Exception? executionException = null, CommandFailureCategory failureCategory = CommandFailureCategory.GeneralRuntimeError) {
            return CreateResult(CommandStatus.Failed, failureCategory, validationResult, errorMessages, executionException);
        }
        /// <summary>
        /// Returns a default result indicating that there is a general runtime error.
        /// </summary>
        /// <param name="errorMessages">(Optional) The error messages that occurred during processing.)</param>
        /// <param name="executionException">(Optional) The exception that occurred during processing.</param>
        /// <param name="failureCategory">The category to use when creating the result, if it's a failure..</param>
        /// <returns>Returns an instance of <see cref="CommandResult"/> corresponding to a general failure.</returns>       
        public static CommandResult<TResult> FailureResult<TResult>(InternalValidationResult? validationResult = null, IEnumerable<string>? errorMessages = null, Exception? executionException = null, CommandFailureCategory failureCategory = CommandFailureCategory.GeneralRuntimeError) {
            return CreateResult<TResult>(CommandStatus.Failed, default(TResult), failureCategory, validationResult, errorMessages, executionException);
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
        private static CommandResult CreateResult(CommandStatus status, CommandFailureCategory failureCategory = CommandFailureCategory.None, InternalValidationResult? validationResult = null, IEnumerable<string>? errorMessages = null, Exception? executionException = null) {
            return new CommandResult { Status = status, ValidationResult = validationResult, ErrorMessages = errorMessages, ExecutionException = executionException, FailureCategory = failureCategory };
        }
        /// <summary>
        /// Returns a default result indicating that the result is unknown.
        /// </summary>
        /// <returns>Returns an instance of <see cref="CommandResult"/> indicating that the status of the command is unknown.</returns>
        public static CommandResult<TResult> UnknownResult<TResult>(TResult? result = default) {
            return CreateResult(CommandStatus.Unknown, result);
        }
        /// <summary>
        /// Returns a default result indicating success..
        /// </summary>
        /// <returns>Returns an instance of <see cref="CommandResult"/> corresponding to a successful completion of a command.</returns>
        public static CommandResult<TResult> SuccessResult<TResult>(TResult? result = default) {
            return CreateResult(CommandStatus.Succeeded, result);
        }
        /// <summary>
        /// Returns a default result indicating that the command was received successfully.
        /// </summary>
        /// <returns>Returns an instance of <see cref="CommandResult"/> corresponding to a successful receipt of a command.</returns>
        public static CommandResult<TResult> ReceivedResult<TResult>(TResult? result = default) {
            return CreateResult(CommandStatus.Received, result);
        }

        /// <summary>
        /// Returns a default result indicating that there is a resource conflict.
        /// </summary>
        /// <param name="validationResult">The result of the command validation.</param>
        /// <param name="errorMessages">(Optional) The error messages that occurred during processing.)</param>
        /// <param name="exception">(Optional) The exception that occurred during processing.</param>
        /// <returns>Returns an instance of <see cref="CommandResult"/> corresponding to a conflict when trying to creat or update a resource.</returns>
        public static CommandResult<TResult> ResourceConflictResult<TResult>(TResult? result = default, InternalValidationResult? validationResult = null, IEnumerable<string>? errorMessages = null, Exception? exception = null) {
            return CreateResult(CommandStatus.Failed, result, CommandFailureCategory.ResourceConflict, validationResult, errorMessages, exception);
        }
        /// <summary>
        /// Returns a default result indicating that there is a version conflict.
        /// </summary>
        /// <param name="validationResult">The result of the command validation.</param>
        /// <param name="errorMessages">(Optional) The error messages that occurred during processing.)</param>
        /// <param name="exception">(Optional) The exception that occurred during processing.</param>
        /// <returns>Returns an instance of <see cref="CommandResult"/> corresponding to an error when trying to update a resource with stale information.</returns>
        public static CommandResult<TResult> VersionConflictResult<TResult>(TResult? result = default, InternalValidationResult? validationResult = null, IEnumerable<string>? errorMessages = null, Exception? exception = null) {
            return CreateResult(CommandStatus.Failed, result, CommandFailureCategory.VersionConlfict, validationResult, errorMessages, exception);
        }
        /// <summary>
        /// Returns a default result indicating that there is a parameter validation failure.
        /// </summary>
        /// <param name="validationResult">The result of the command validation.</param>
        /// <returns>Returns an instance of <see cref="CommandResult"/> corresponding to a command resulting in validation failure of the command.</returns>
        public static CommandResult<TResult> ValidationFailureResult<TResult>(InternalValidationResult validationResult, TResult? result = default) {
            return CreateResult(CommandStatus.Failed, result, CommandFailureCategory.ParameterValidation, validationResult);
        }
        /// <summary>
        /// Returns a default result indicating that there is a general runtime error.
        /// </summary>
        /// <param name="errorMessages">(Optional) The error messages that occurred during processing.)</param>
        /// <param name="executionException">(Optional) The exception that occurred during processing.</param>
        /// <param name="failureCategory">The category to use when creating the result, if it's a failure..</param>
        /// <returns>Returns an instance of <see cref="CommandResult"/> corresponding to a general failure.</returns>
        public static CommandResult<TResult> FailureResult<TResult>(TResult? result = default, InternalValidationResult? validationResult = null, IEnumerable<string>? errorMessages = null, Exception? executionException = null, CommandFailureCategory failureCategory = CommandFailureCategory.GeneralRuntimeError) {
            return CreateResult(CommandStatus.Failed, result, failureCategory, validationResult, errorMessages, executionException);
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
        private static CommandResult<TResult> CreateResult<TResult>(CommandStatus status, TResult? result, CommandFailureCategory failureCategory = CommandFailureCategory.None, InternalValidationResult? validationResult = null, IEnumerable<string>? errorMessages = null, Exception? executionException = null) {
            return new CommandResult<TResult> { Status = status, ValidationResult = validationResult, ErrorMessages = errorMessages, ExecutionException = executionException, FailureCategory = failureCategory, Result = result };
        }
        #endregion
    }
}
