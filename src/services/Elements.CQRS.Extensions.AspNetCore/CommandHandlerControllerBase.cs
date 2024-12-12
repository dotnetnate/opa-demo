using CitizensFinancialGroup.Elements.CQRS.Commands;
using CitizensFinancialGroup.Elements.CQRS.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace CitizensFinancialGroup.Elements.CQRS.Extensions.AspNetCore {
    public class CommandHandlerControllerBase<TIdentity> : ControllerBase {

        private readonly string _sourceName;

        public CommandHandlerControllerBase(ILogger logger, ICommandResultHandlerService<IActionResult> commandResultHandlerService, IIdentityService<TIdentity> identityService) {
            Logger = logger;
            CommandResultHandlerService = commandResultHandlerService;
            IdentityService = identityService;
            _sourceName = $"[CONTROLLER]-{GetType().Name}";
        }

        protected ILogger Logger { get; private set; }
        protected ICommandResultHandlerService<IActionResult> CommandResultHandlerService { get; private set; }
        protected IIdentityService<TIdentity> IdentityService { get; private set; }


        protected async Task<IActionResult> ExecuteQuery<TQuery, TResult>(TQuery query, QueryHandler<TQuery, TResult> queryHandler, bool treatNullResultAsNotFound = false, Func<TResult, object> resultSelector = null)
            where TQuery : QueryBase, new() {

            //ClaimsIdentity identity;
            /*
            try {
                identity = await IdentityService.CreateIdentityFromRequest(request);
            }
            catch (SecurityTokenExpiredException) {
                return new UnauthorizedObjectResult(new { code = "TOKEN_EXPIRED", message = "Security token has expired." });
            } */


            // see what this was for 
            /*
            if (queryMapper != null) {
                query = queryMapper(request, query);
            }
            */

            query.CorrelationId = Guid.NewGuid();
            //query.OriginatingUser = identity;



            string queryName = query.GetType().Name;

            Stopwatch queryTimer = new Stopwatch();
            queryTimer.Start();

            try {
                object? queryResult = await queryHandler.GetResult(query);

                ICollection? z = queryResult as ICollection;

                if ((queryResult == null || (z != null && z.Count == 0)) && treatNullResultAsNotFound) {
                    return new NotFoundResult();
                }

                if (resultSelector != null && queryResult != null) {
                    queryResult = resultSelector((TResult)queryResult);
                }

                return new JsonResult(queryResult);

            }
            catch (Exception e) {
                Logger.LogError(e, e.Message);
                return new StatusCodeResult(500);
            }
            finally {
                queryTimer.Stop();
            }
        }

        protected async Task<IActionResult> ExecuteCommand<TCommand, TResult>(TCommand command, CommandHandler<TCommand, TResult> commandHandler)
            where TCommand : CommandBase, new()
            where TResult : CommandResult {

            await IdentityService.AcquireIdentitiesFromRequest(Request, out TIdentity originatingIdentity, out TIdentity impersonatedIdentity);

            var activitySource = new ActivitySource(_sourceName);

            using (var activity = activitySource.StartActivity("ExecuteCommand")) {

                try {

                    command.SourceTraceId = Activity.Current?.Id;

                    var commandResult = await commandHandler.Process(command);

                    Logger.LogDebug("Command result: {CommandResult}", commandResult);


                    Logger.LogDebug("Processing command result.");

                    var retVal = CommandResultHandlerService.HandleCommandResult(commandResult);

                    if (retVal != null) {
                        Logger.LogDebug("Returning result of type {ResultType}", retVal.GetType().Name);
                        return retVal;
                    }
                    else {
                        Logger.LogCritical("No command result handler was able to handle the command result. This is a critical system misconfiguration.");
                        return new StatusCodeResult(500);
                    }

                }
                catch (Exception e) {
                    Logger.LogError(e, e.Message);
                    activity?.AddException(e, timestamp: DateTimeOffset.UtcNow);
                    return new StatusCodeResult(500);
                }

            }
        }
    }
}
