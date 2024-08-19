// ReSharper disable UnusedMember.Global

[module: System.Runtime.CompilerServices.SkipLocalsInit]

#if !NET8_0_OR_GREATER

// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    /// <summary></summary>
    /// <param name="builderType">The type of the builder to use to construct the collection.</param>
    /// <param name="methodName">The name of the method on the builder to use to construct the collection.</param>
    [AttributeUsage(
        AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Interface,
        Inherited = false)]
    internal sealed class CollectionBuilderAttribute(Type builderType, string methodName) : Attribute
    {
        /// <summary>
        /// Gets the type of the builder to use to construct the collection.
        /// </summary>
        public Type BuilderType => builderType;

        /// <summary>
        /// Gets the name of the method on the builder to use to construct the collection.
        /// </summary>
        public string MethodName => methodName;
    }
}

#endif
