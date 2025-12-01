using System;
using System.Collections.Generic;
using System.Text;

namespace NOCO.Elements.ApplicationModel.Commands{
    /// <summary>
    /// Represents the categories of command failures that can occur in the system.
    /// </summary>
    public enum CommandFailureCategory {
        /// <summary>
        /// No failure.
        /// </summary>
        None = 0,

        /// <summary>
        /// The requested resource was not found.
        /// </summary>
        ResourceNotFound = 1,

        /// <summary>
        /// The user does not have sufficient permissions.
        /// </summary>
        InsufficientPermissions = 2,

        /// <summary>
        /// There was a validation error with the provided parameters.
        /// </summary>
        ParameterValidation = 4,

        /// <summary>
        /// The operation is invalid.
        /// </summary>
        InvalidOperation = 8,

        /// <summary>
        /// There was a version conflict.
        /// </summary>
        VersionConlfict = 16,

        /// <summary>
        /// The content was rejected.
        /// </summary>
        RejectedContent = 32,

        /// <summary>
        /// A general runtime error occurred.
        /// </summary>
        GeneralRuntimeError = 64,

        /// <summary>
        /// There was a conflict with an existing resource.
        /// </summary>
        ResourceConflict = 128
    }
}
