using System.Linq.Expressions;

namespace Modern.Repositories.MongoDB.Specifications.Base;

/// <summary>
/// The specification expression visitor that replaces parameter expression
/// </summary>
public class SpecificationExpressionVisitor : ExpressionVisitor
{
    private readonly ParameterExpression _parameter;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="parameter">Parameter expression</param>
    internal SpecificationExpressionVisitor(ParameterExpression parameter)
    {
        _parameter = parameter;
    }

    /// <summary>
    /// <inheritdoc cref="ExpressionVisitor.VisitParameter"/>
    /// </summary>
    protected override Expression VisitParameter(ParameterExpression node)
        => base.VisitParameter(_parameter);
}