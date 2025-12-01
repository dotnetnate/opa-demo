using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NOCO.Elements.Data.MongoDb.Configuration {

    /// <summary>
    /// Represents the options required to establish a connection to a MongoDB database.
    /// </summary>
    public class MongoDbConnectionOptions {
        /// <summary>
        /// Gets or sets the connection string used to connect to the MongoDB database.
        /// </summary>
        public required string ConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the server address of the MongoDB database.
        /// </summary>
        public required string Server { get; set; }

        /// <summary>
        /// Gets or sets the username used to authenticate with the MongoDB database.
        /// </summary>
        public required string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password used to authenticate with the MongoDB database.
        /// </summary>
        public required string Password { get; set; }
    }
}
