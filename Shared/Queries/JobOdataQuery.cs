using System.Linq.Expressions;

namespace Shared.Queries
{
    internal class JobOdataQuery
    {
        internal static string Where(LambdaExpression expression)
        {
            var filter = new JobFilterVisitor().Apply(expression);
            return filter;
        }
    }
}
