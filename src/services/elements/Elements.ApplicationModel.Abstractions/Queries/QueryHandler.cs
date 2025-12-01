using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Elements.ApplicationModel.Queries {
    public abstract class QueryHandler<TQuery, TResult>
        where TQuery : QueryBase {


        private readonly string _sourceName;

        public QueryHandler() {
            _sourceName = $"[QueryHandler]-{GetType().Name}";
        }

        public virtual async Task<QueryResult<TResult>> GetResult(TQuery query) {
            var activitySource = new ActivitySource(_sourceName);

            using (var activity = activitySource.StartActivity("GetResult", ActivityKind.Internal)) {
                try {
                    var queryResults = await ExecuteQuery(query);

                    if (queryResults != null) {
                        return QueryResult<TResult>.SuccessResult(queryResults);
                    }
                    else {
                        switch (NullResultAdvice) {
                            case NullResultAdvices.TreatAsNotFound:
                                return QueryResult<TResult>.NotFoundResult();
                            default:
                                return QueryResult<TResult>.SuccessResult(queryResults);
                        }
                    }
                }
                catch (Exception e) {
                    activity?.AddException(e);
                    return QueryResult<TResult>.FailureResult(executionException: e);
                }
            }
        }

        public abstract Task<TResult?> ExecuteQuery(TQuery query);
        protected virtual NullResultAdvices NullResultAdvice { get; set; } = NullResultAdvices.TreatAsSuccess;

    }



}
