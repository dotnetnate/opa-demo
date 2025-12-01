using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Threvw.Identity {
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
        public required string Authority {  get; set; }

        public override bool Equals(object? obj) {

            if (obj is not IdentityReference otherUser) {
                return false;
            }
            return otherUser.Authority == this.Authority && otherUser.Identifier == this.Identifier;
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public override string ToString() {
            return $"[{DisplayName}] - {Identifier}@{Authority}";
        }
    }
}
