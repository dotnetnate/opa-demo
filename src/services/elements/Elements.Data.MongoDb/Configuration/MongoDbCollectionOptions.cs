using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Elements.Data.MongoDb.Configuration {
    /// <summary>
    /// Represents the options for configuring a MongoDB collection.
    /// </summary>
    public class MongoDbCollectionOptions {
        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        public required string Database { get; set; }

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        public required string Collection { get; set; }

    }
}
