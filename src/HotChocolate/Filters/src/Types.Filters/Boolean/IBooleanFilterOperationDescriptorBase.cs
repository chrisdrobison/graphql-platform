using System;
using HotChocolate.Language;

namespace HotChocolate.Types.Filters;

[Obsolete("Use HotChocolate.Data.")]
public interface IBooleanFilterOperationDescriptorBase
    : IDescriptor<FilterOperationDefinition>
        , IFluent
{
    /// <summary>
    /// Specify the name of the filter operation.
    /// </summary>
    /// <param name="value">
    ///  The operation name.
    /// </param>
    IBooleanFilterOperationDescriptorBase Name(string value);

    /// <summary>
    /// Specify the description of the filter operation.
    /// </summary>
    /// <param name="value">
    ///  The operation description.
    /// </param>
    IBooleanFilterOperationDescriptorBase Description(string value);

    /// <summary>
    /// Annotate the operation filter field with a directive.
    /// </summary>
    /// <param name="directiveInstance">
    /// The directive with which the field shall be annotated.
    /// </param>
    /// <typeparam name="T">
    /// The directive type.
    /// </typeparam>
    IBooleanFilterOperationDescriptorBase Directive<T>(T directiveInstance)
        where T : class;

    /// <summary>
    /// Annotate the operation filter field with a directive.
    /// </summary>
    /// <typeparam name="T">
    /// The directive type.
    /// </typeparam>
    IBooleanFilterOperationDescriptorBase Directive<T>()
        where T : class, new();

    /// <summary>
    /// Annotate the operation filter field with a directive.
    /// </summary>
    /// <param name="name">
    /// The name of the directive.
    /// </param>
    /// <param name="arguments">
    /// The argument values of the directive.
    /// </param>
    IBooleanFilterOperationDescriptorBase Directive(string name, params ArgumentNode[] arguments);
}
