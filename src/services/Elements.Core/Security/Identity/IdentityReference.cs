using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizensFinancialGroup.Elements.Security.Identity {
    /// <summary>
    /// Represents a reference to an identity with an identifier, display name, and authority.
    /// </summary>
    public class IdentityReference {
        /// <summary>
        /// Gets or sets the id of the user.
        /// </summary>
        public required string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the display name of the user.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or set the authority of the identity.
        /// </summary>
        public required string Authority { get; set; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object? obj) {
            if (obj is not IdentityReference otherUser) {
                return false;
            }
            return otherUser.Authority == this.Authority && otherUser.Identifier == this.Identifier;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            return $"[{DisplayName}] - {Identifier}@{Authority}";
        }
    }
}
