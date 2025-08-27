using System.Linq.Expressions;
using System.Text;

namespace Shared.Queries;

internal class JobFilterVisitor : ExpressionVisitor
{
    private readonly StringBuilder sb = new();

    internal string Apply(LambdaExpression expression)
    {
        Visit(expression);
        return sb.ToString();
    }
        
    protected override Expression VisitBinary(BinaryExpression node)
    {
        sb.Append("(");
        Visit(node.Left);

        switch (node.NodeType)
        {
            case ExpressionType.And:
            case ExpressionType.AndAlso:
                sb.Append(" and ");
                break;
            case ExpressionType.Or:
            case ExpressionType.OrElse:
                sb.Append(" or ");
                break;
            case ExpressionType.Equal:
                sb.Append(" eq ");
                break;
            case ExpressionType.NotEqual:
                sb.Append(" ne ");
                break;
            case ExpressionType.GreaterThan:
                sb.Append(" gt ");
                break;
            case ExpressionType.GreaterThanOrEqual:
                sb.Append(" ge ");
                break;
            case ExpressionType.LessThan:
                sb.Append(" lt ");
                break;
            case ExpressionType.LessThanOrEqual:
                sb.Append(" le ");
                break;
            default: throw new ArgumentOutOfRangeException($"NodeType: {node.NodeType} is not supported.");
        }

        Visit(node.Right);
        sb.Append(")");

        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is ParameterExpression)
        {
            sb.Append(node.Member.Name);
        }
        else
        {
            var compiled = Expression.Lambda(node).Compile();
            var value = compiled.DynamicInvoke(null);
            AppendValue(node.Type, value);
        }

        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        AppendValue(node.Type, node.Value);

        return node;
    }


    private void AppendValue(Type type, object value)
    {
        if (type == typeof(string))
        {
            sb.Append($"'{value}'");
        }
        else if (type == typeof(DateTime))
        {
            // To round-trip/ISO 8601
            var dt = (DateTime)value;
            var isoDate = dt.Kind == DateTimeKind.Utc ? dt.ToString("o") : dt.ToUniversalTime().ToString("o");
            sb.Append(isoDate);
        }
        else if (type == typeof(DateTimeOffset))
        {
            // To round-trip/ISO 8601
            sb.Append(((DateTimeOffset)value).ToString("o"));
        }
        else
        {
            sb.Append(value);
        }
    }
}