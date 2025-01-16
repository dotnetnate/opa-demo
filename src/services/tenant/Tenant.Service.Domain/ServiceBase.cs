using CitizensFinancialGroup.Elements.ApplicationModel.Commands;
using CitizensFinancialGroup.Elements.ApplicationModel.Queries;

using CitizensFinancialGroup.Elements;
using CitizensFinancialGroup.Elements.Security.Identity;
using CitizensFinancialGroup.Elements.Validation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Tenants.Domain {
    public abstract class ServiceBase<TIdentity, TIdentityContext> {

        private readonly string _sourceName;

        public ServiceBase(IValidationService validationService, ILogger<ServiceBase<TIdentity, TIdentityContext>> logger, IIdentityService<TIdentity, TIdentityContext> identityService) {
            _sourceName = $"[Service]-{GetType().Name}";
            ValidationService = validationService;
            Logger = logger;
            IdentityService = identityService;
        }

        protected ILogger<ServiceBase<TIdentity, TIdentityContext>> Logger { get; private set; }
        protected IIdentityService<TIdentity, TIdentityContext> IdentityService { get; private set; }

        protected IValidationService ValidationService { get; private set; }

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

                    var result = new TResult {
                        ValidationResult = validationResult,
                        FailureCategory = CommandFailureCategory.ParameterValidation,
                        Status = CommandStatus.Failed
                    };

                    Logger.LogInformation("Validation failed on {Command}.", command);
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

        protected async Task<TResult> ExecuteCommand<TCommand, TResult>(TCommand command, CommandHandler<TCommand, TResult> commandHandler)
            where TCommand : CommandBase
            where TResult : CommandResult, new() {


            TResult retVal;

            var activitySource = new ActivitySource(_sourceName);

            using (var activity = activitySource.StartActivity("ExecuteCommand", ActivityKind.Internal)) {

                Logger.LogDebug("Starting validation on {Command}", command);

                var validationResult = await ValidationService.Validate(command);

                Logger.LogDebug("Validation completed on {Command}", command);

                if (!validationResult.IsValid()) {

                    var result = new TResult {
                        ValidationResult = validationResult,
                        FailureCategory = CommandFailureCategory.ParameterValidation,
                        Status = CommandStatus.Failed
                    };

                    Logger.LogInformation("Validation failed on {Command}.", command);
                }
                try {

                    command.SourceTraceId = Activity.Current?.Id;

                    retVal = await commandHandler.Process(command);

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



        protected async Task<QueryResult<TResult>> ExecuteQuery<TQuery, TResult>(TQuery query, QueryHandler<TQuery, TResult> queryHandler, Func<TResult, TResult>? resultFilter = default)
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

                    QueryResult<TResult> queryResult = await queryHandler.GetResult(query);

                    if (queryResult.IsSuccess()) {

                        if (resultFilter != null && queryResult.Result != null) {

                            Logger.LogDebug("Result filter is enabled for this query. Executing filter.");

                            queryResult.Result = resultFilter(queryResult.Result);
                        }

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
                    return QueryResult<TResult>.FailureResult();
                }

            }
        }


        protected async Task<QueryResult<TResult>> ExecuteQuery<TQuery, TResult>(TQuery query, Func<TQuery, Task<QueryResult<TResult>>> queryHandler, Func<TResult, TResult>? resultFilter = default)
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

                        if (resultFilter != null && queryResult.Result != null) {

                            Logger.LogDebug("Result filter is enabled for this query. Executing filter.");

                            queryResult.Result = resultFilter(queryResult.Result);
                        }

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
                    return QueryResult<TResult>.FailureResult();
                }

            }
        }
    }
}
