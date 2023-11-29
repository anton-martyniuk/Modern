using System.Linq.Expressions;

namespace Modern.Repositories.LiteDB.Specifications.Base;

/// <summary>
/// The specification expression visitor that replaces parameter expression
/// </summary>
internal class ReplaceExpressionVisitor : ExpressionVisitor
{
    private readonly Expression _oldValue;
    private readonly Expression _newValue;

    /// <summary>
    /// Initializes a new instance of the class
    /// </summary>
    /// <param name="oldValue">Old expression to be replaced</param>
    /// <param name="newValue">A new expression that replaces the old one</param>
    public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
    {
        _oldValue = oldValue;
        _newValue = newValue;
    }

    public override Expression Visit(Expression? node) => (node == _oldValue ? _newValue : base.Visit(node))!;
}