using Contracts;
using System.Linq.Expressions;

namespace Shared.Queries
{
    public class JobOdataQueryBuilder
    {
        private string filter = string.Empty;
        private int? top;
        private bool isInlineCount;
        private int? skip;
        private readonly List<string> orderByClauses = new();

        internal IDictionary<string, string> ToRequestParams()
        {
            var dict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(filter))
            {
                dict.Add("$filter", this.filter);
            }

            if (top.HasValue)
            {
                dict.Add("$top", this.top.Value.ToString());
            }

            if (skip.HasValue)
            {
                dict.Add("$skip", this.skip.ToString());
            }

            if (isInlineCount)
            {
                dict.Add("$count", "true");
            }

            if (orderByClauses.Any())
            {
                dict["$orderby"] = string.Join(",", orderByClauses);
            }

            return dict;
        }

        private JobOdataQueryBuilder()
        {
        }

        public static JobOdataQueryBuilder Create() => new JobOdataQueryBuilder();

        public JobOdataQueryBuilder Where(Expression<Func<Job, bool>> expression)
        {
            this.filter = new JobFilterVisitor().Apply(expression);
            return this;
        }

        public JobOdataQueryBuilder OrderBy<TKey>(Expression<Func<Job, TKey>> keySelector)
        {
            var field = new JobFilterVisitor().Apply(keySelector);
            this.orderByClauses.Add(field);
            return this;
        }

        public JobOdataQueryBuilder OrderByDescending<TKey>(Expression<Func<Job, TKey>> keySelector)
        {
            var field = new JobFilterVisitor().Apply(keySelector);
            this.orderByClauses.Add(field + " desc");
            return this;
        }

        public JobOdataQueryBuilder Skip(int skip)
        {
            AssertNonNullPositive(nameof(skip), skip);

            this.skip = skip;
            return this;
        }

        public JobOdataQueryBuilder Top(int top)
        {
            AssertNonNullPositive(nameof(top), top);

            this.top = top;
            return this;
        }

        /// <summary>
        /// Request for additional count json field in the output.
        /// Not the same as /$count
        /// </summary>
        /// <returns></returns>
        public JobOdataQueryBuilder InlineCount()
        {
            this.isInlineCount = true;
            return this;
        }

        private void AssertNonNullPositive(string fieldName, int? value)
        {
            if (!value.HasValue)
            {
                throw new ArgumentNullException(fieldName);
            }

            if (value.Value <= 0)
            {
                throw new ArgumentOutOfRangeException($"Field {fieldName} must be a positive integer.");
            }
        }
    }
}
