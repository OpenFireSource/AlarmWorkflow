
namespace AlarmWorkflow.Shared.ObjectExpressions
{
    /// <summary>
    /// Represents a method that gets called by <see cref="ObjectExpressionFormatter{TInput}"/> if it couldn't resolve a specific path.
    /// See documentation for further information.
    /// </summary>
    /// <remarks>The responsible method should </remarks>
    /// <typeparam name="TInput">The type of the object that is in focus.</typeparam>
    /// <param name="graph">The object that is in focus.</param>
    /// <param name="expression">The expression that needs to be resolved.</param>
    /// <returns>An instance of <see cref="ResolveExpressionResult"/> which represents the result from this operation.</returns>
    public delegate ResolveExpressionResult ExpressionResolver<TInput>(TInput graph, string expression);
}
