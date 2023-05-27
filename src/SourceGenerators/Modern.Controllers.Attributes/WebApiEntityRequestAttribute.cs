using System;

namespace Modern.Controllers.SourceGenerators
{
    /// <summary>
    /// An attribute that specifies that CreateRequest and UpdateRequest classes should be generated
    /// for the given Class or Record entity using the source generator
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class WebApiEntityRequestAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether to generate the CreateEntityRequest class. Default is true.
        /// </summary>
        public bool GenerateCreateRequest { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to generate the UpdateEntityRequest class. Default is true.
        /// </summary>
        public bool GenerateUpdateRequest { get; set; } = true;

        /// <summary>
        /// Gets or sets a custom name for a create request class
        /// </summary>
        public string? CreateRequestName { get; set; }

        /// <summary>
        /// Gets or sets a custom name for an update request class
        /// </summary>
        public string? UpdateRequestName { get; set; }
    }   
}