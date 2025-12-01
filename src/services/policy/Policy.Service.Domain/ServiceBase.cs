using NOCO.Elements.ApplicationModel.Commands;
using NOCO.Elements.ApplicationModel.Queries;

using NOCO.Elements;
using NOCO.Elements.Security.Identity;
using NOCO.Elements.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Policies.Domain {
    /// <summary>
    /// Base class for services that provides common functionality for executing commands and queries.
    /// </summary>
    /// <typeparam name="TIdentity">The type of the identity.</typeparam>
    /// <typeparam name="TIdentityContext">The type of the identity context.</typeparam>
    public abstract class ServiceBase<TIdentity, TIdentityContext> {

        private readonly string _sourceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TIdentity, TIdentityContext}"/> class.
        /// </summary>
        /// <param name="validationService">The validation service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="identityService">The identity service.</param>
        public ServiceBase(IValidationService validationService, ILogger<ServiceBase<TIdentity, TIdentityContext>> logger, IIdentityService<TIdentity, TIdentityContext> identityService) {
            _sourceName = $"[Service]-{GetType().Name}";
            ValidationService = validationService;
            Logger = logger;
            IdentityService = identityService;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger<ServiceBase<TIdentity, TIdentityContext>> Logger { get; private set; }

        /// <summary>
        /// Gets the identity service.
        /// </summary>
        protected IIdentityService<TIdentity, TIdentityContext> IdentityService { get; private set; }

        /// <summary>
        /// Gets the validation service.
        /// </summary>
        protected IValidationService ValidationService { get; private set; }

        /// <summary>
        /// Executes the specified command using the provided command handler.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="commandHandler">The command handler function.</param>
        /// <returns>The result of the command execution.</returns>
        protected async Task<TResult> ExecuteCommand<TCommand, TResult>(TCommand command, Func<TCommand, Task<TResult>> commandHandler)
                   where TCommand : CommandBase
                    where TResult : CommandResult, new() {

            TResult retVal;

            var activitySource = new ActivitySource(_sourceName);

            using (var activity = activitySource.StartActivity("ExecuteCommand", ActivityKind.Internal)) {

                Logger.LogDebug("Starting validation on {Command}", command);

                var validationResult = await ValidationService.Validate(command);

                Logger.LogDebug("Validation completed on {Command}", command);

                if (!validationResult.IsValid()) {

                    retVal = new TResult {
                        ValidationResult = validationResult,
                        FailureCategory = CommandFailureCategory.ParameterValidation,
                        Status = CommandStatus.Failed
                    };

                    Logger.LogInformation("Validation failed on {Command}.", command);

                    return retVal;
                }
                try {

                    retVal = await commandHandler(command);

                    Logger.LogDebug("Command result: {CommandResult}", retVal);

                    Logger.LogDebug("Processing command result.");

                    if (retVal != null) {
                        Logger.LogDebug("Returning result of type {ResultType}", retVal.GetType().Name);
                        return retVal;
                    }
                    else {
                        retVal = new TResult {
                            FailureCategory = CommandFailureCategory.GeneralRuntimeError,
                            Status = CommandStatus.Failed,
                            ExecutionException = new Exception("Command handler did not return a result.")
                        };
                    }

                }
                catch (Exception e) {
                    Logger.LogError(e, e.Message);
                    activity?.AddException(e, timestamp: DateTimeOffset.UtcNow);

                    retVal = new TResult {
                        FailureCategory = CommandFailureCategory.GeneralRuntimeError,
                        Status = CommandStatus.Failed,
                        ExecutionException = e
                    };
                }


                Logger.LogDebug("Returning {Result}.", retVal);

                return retVal;
            }
        }

        /// <summary>
        /// Executes the specified command using the provided command handler.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="command">The command to execute.</param>
        /// <param name="commandHandler">The command handler.</param>
        /// <returns>The result of the command execution.</returns>
        protected async Task<TResult> ExecuteCommand<TCommand, TResult>(TCommand command, CommandHandler<TCommand, TResult> commandHandler)
            where TCommand : CommandBase
            where TResult : CommandResult, new() {
            return await ExecuteCommand(command, commandHandler.Process);
        }

        /// <summary>
        /// Executes the specified query using the provided query handler.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query to execute.</param>
        /// <param name="queryHandler">The query handler.</param>
        /// <param name="resultFilter">The result filter function.</param>
        /// <returns>The result of the query execution.</returns>
        protected async Task<QueryResult<TResult>> ExecuteQuery<TQuery, TResult>(TQuery query, QueryHandler<TQuery, TResult> queryHandler, Func<TResult, TResult>? resultFilter = default)
            where TQuery : QueryBase {
            return await ExecuteQuery(query, queryHandler.GetResult);
        }

        /// <summary>
        /// Executes the specified query using the provided query handler function.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query to execute.</param>
        /// <param name="queryHandler">The query handler function.</param>
        /// <param name="resultFilter">The result filter function.</param>
        /// <returns>The result of the query execution.</returns>
        protected async Task<QueryResult<TResult>> ExecuteQuery<TQuery, TResult>(TQuery query, Func<TQuery, Task<QueryResult<TResult>>> queryHandler)
            where TQuery : QueryBase {

            var activitySource = new ActivitySource(_sourceName);

            using (var activity = activitySource.StartActivity("ExecuteQuery", ActivityKind.Internal)) {

                Logger.LogDebug("Starting validation on {Query}", query);

                var validationResult = await ValidationService.Validate(query);

                Logger.LogDebug("Validation completed on {Query}", query);

                if (!validationResult.IsValid()) {

                    Logger.LogInformation("Validation failed on {Query}.", query);

                    return QueryResult<TResult>.ValidationFailureResult(validationResult);
                }
                try {

                    QueryResult<TResult> queryResult = await queryHandler(query);

                    if (queryResult.IsSuccess()) {

                        Logger.LogDebug("Returning from query.");

                        return queryResult;

                    }
                    else {
                        return queryResult;
                    }
                }
                catch (Exception e) {
                    Logger.LogError(e, e.Message);
                    activity?.AddException(e, timestamp: DateTimeOffset.UtcNow);
                    return QueryResult<TResult>.FailureResult(executionException: e);
                }

            }
        }
    }
}
