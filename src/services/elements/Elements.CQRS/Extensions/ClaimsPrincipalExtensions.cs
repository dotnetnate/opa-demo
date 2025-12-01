using NOCO.Elements.CQRS;
using NOCO.Threvw.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace System.Security.Claims {
    public static class ClaimsPrincipalExtensions {
        public static string? GetClaimValue(this ClaimsIdentity identity, string claimType) {
            return identity?.FindFirst(claimType)?.Value;
        }

        public static IdentityReference ToIdentityReference(this ClaimsIdentity identity) {

            try {

                var identifier = identity.GetClaimValue(WellKnownClaims.Identifier);
                var issuer = identity.GetClaimValue(WellKnownClaims.Authority);
                var displayName = identity.FindDisplayName();

                if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(issuer)) {
                    throw new InvalidOperationException("Unable to convert ClaimsIdentity to IdentityReference due to missing or empty iss, subj claims");
                }

                return new IdentityReference { Authority = issuer, Identifier = identifier, DisplayName = displayName ?? string.Empty };
            }
            catch {
                throw new InvalidOperationException("Unable to convert ClaimsIdentity to IdentityReference due to missing or empty iss, subj claims");
            }
        }

        private static string? FindDisplayName(this ClaimsIdentity identity) {

            var claimValue = identity.GetClaimValue(WellKnownClaims.DisplayName);

            if( claimValue == null) {
                claimValue = identity.GetClaimValue(WellKnownClaims.Name);
            } 

            if (claimValue == null) {
                claimValue = identity.GetClaimValue(WellKnownClaims.PreferredUserName);
            }

            return claimValue;
        }

        private static class WellKnownClaims {
            public static string Authority => "iss";
            public static string Identifier => "subj";
            public static string Name => "name";
            public static string PreferredUserName => "preferred_username";
            public static string DisplayName => "display_name";
        }
    }
}
