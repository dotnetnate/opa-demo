using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Threvw.Policies.Domain {

    /// <summary>
    ///  This represents an identity that is guaranteed to be unique within a scope, but not globally.
    /// </summary>
    /// <remarks>
    /// While UUIDs are often a preferable solution to guarantee uniqueness, <see cref="ScopedIdentity"/> allows for
    /// non-guaranteed values such as usernames, email addresses, etc. that may have different meanings in different contexts.
    /// </remarks>
    public class ScopedIdentity {
        /// <summary>
        /// The unique identifier for the identity.
        /// </summary>
        public required string Identifier { get; set; }
        /// <summary>
        /// The owning scope of the identity or the authority under which the identity is unique.
        /// </summary>
        public required string Authority { get; set; }

        public override bool Equals(object? obj) {
            return obj is ScopedIdentity identity &&
                   Identifier == identity.Identifier &&
                   Authority == identity.Authority;
        }

        public override string ToString() {
            return $"{Identifier}@{Authority}";
        }

        public override int GetHashCode() {
            return HashCode.Combine(Identifier, Authority);
        }

        public static bool operator ==(ScopedIdentity left, ScopedIdentity right) {
            return left.Equals(right);
        }
        public static bool operator !=(ScopedIdentity left, ScopedIdentity right) {
            return !left.Equals(right);
        }
    }
}
