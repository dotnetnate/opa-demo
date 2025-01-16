using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Elements.Security.Identity {
    /// <summary>
    /// Interface for identity services that handle acquiring identities from a given context.
    /// </summary>
    /// <typeparam name="TIdentity">The type representing the identity.</typeparam>
    /// <typeparam name="TContext">The type representing the context from which identities are acquired.</typeparam>
    public interface IIdentityService<TIdentity, TContext> {
        /// <summary>
        /// Acquires the originating and impersonated identities from the provided context.
        /// </summary>
        /// <param name="context">The context from which to acquire identities.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with the originating and impersonated identities.</returns>
        Task<(TIdentity? originatingIdentity, TIdentity? impersonatedIdentity)> AcquireIdentitiesFromRequest(TContext context);
    }
}
