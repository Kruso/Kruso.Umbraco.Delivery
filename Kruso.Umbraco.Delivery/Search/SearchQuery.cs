using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Extensions;

namespace Kruso.Umbraco.Delivery.Search
{
    public class SearchQuery
    {
        private readonly string[] Operators = new[] { "AND", "OR", "NOT" };
        public string DefaultOperator { get; set; } = "AND";

        Stack<string> InternalQuery { get; set; } = new Stack<string>();
        public string Query 
        { 
            get
            {
                StringBuilder res = new StringBuilder();

                foreach (var item in InternalQuery.Reverse())
                {
                    if (item == ")" && res.Length > 0)
                        res.Remove(res.Length - 1, 1);

                    res.Append(item);

                    if (item != "(")
                        res.Append(" ");
                }

                return res.ToString();
            }
        }

        public SearchQuery DocumentType(params string[] documentTypes) => And("__NodeTypeAlias", documentTypes);
        public SearchQuery IsPublished(string? culture = null)
        {
            return string.IsNullOrEmpty(culture)
                ? And($"__Published", "y")
                : And($"__Published_{culture}", "y");
        }

        public SearchQuery And<T>(string field, params T[] vals)
        {
            vals = Clean(vals);
            if (vals.Any())
                And().Matches(field, vals);

            return this;
        }

        public SearchQuery Or<T>(string field, params T[] vals)
        {
            vals = Clean(vals);
            if (vals.Any())
                Or().Matches(field, vals);
            
            return this;
        }

        public SearchQuery Not<T>(string field, params T[] vals)
        {
            vals = Clean(vals);
            if (vals.Any())
                Not().Matches(field, vals);

            return this;
        }

        public SearchQuery Between(string field, DateTime from, DateTime to)
            => Between(field, long.Parse(from.ToString("yyyyMMddHHmmssfff")), long.Parse(to.ToString("yyyyMMddHHmmssfff")));

        public SearchQuery Between(string field, long from, long to)
        {
            if (!string.IsNullOrEmpty(field))
                InternalQuery.Push($"{field}:[{from} TO {to}]");

            return this;
        }

        public SearchQuery Matches<T>(string field, params T[] vals)
        {
            if (string.IsNullOrEmpty(field))
                return this;

            var subClauses = Clean(vals)
                .Select(x => $"{field}:{x}");

            return Group(subClauses);
        }

        public SearchQuery Group(string subClause)
        {
            if (!string.IsNullOrEmpty(subClause))
            {
                StartGroup();
                InternalQuery.Push(subClause);
                EndGroup();
            }
                
            return this;
        }

        public SearchQuery Group(IEnumerable<string> subClauses)
        {
            if (subClauses.Count() > 1) StartGroup();
            InternalQuery.Push(string.Join(" OR ", subClauses));
            if (subClauses.Count() > 1) EndGroup();

            return this;
        }

        public SearchQuery Group(Action<SearchQuery> action)
        {
            var deliQuery = new SearchQuery();
            if (action != null)
            {
                action(deliQuery);
                var subClause = deliQuery.Query;
                return Group(subClause);
            }

            return deliQuery;
        }

        public SearchQuery And()
        {
            AddOperator("AND");
            return this;
        }

        public SearchQuery And(Action<SearchQuery> action)
        {
            var subClause = Group(action).Query.Trim();

            return !string.IsNullOrEmpty(subClause)
                ? And().Group(subClause)
                : this;
        }

        public SearchQuery Or()
        {
            AddOperator("OR");
            return this;
        }

        public SearchQuery Or(Action<SearchQuery> action)
        {
            var subClause = Group(action).Query.Trim();

            return !string.IsNullOrEmpty(subClause)
                ? Or().Group(subClause) 
                : this;
        }

        public SearchQuery Not()
        {
            AddOperator("NOT");
            return this;
        }

        private T[] Clean<T>(IEnumerable<T> vals)
        {
            return typeof(T) == typeof(string)
                ? vals.Where(x => !string.IsNullOrEmpty(x?.ToString())).ToArray()
                : vals.ToArray();
        }

        private void AddOperator(string op)
        {
            if (HasOperator())
                InternalQuery.Pop();

            if (InternalQuery.Count() > 0 && Operators.Contains(op))
                InternalQuery.Push(op);
        }

        private bool HasOperator()
        {
            if (!InternalQuery.Any())
                return false;

            var op = InternalQuery.Peek();
            return !string.IsNullOrEmpty(op) && Operators.Contains(op);
        }

        private void StartGroup()
        {
            InternalQuery.Push("(");
        }

        private void EndGroup()
        {
            InternalQuery.Push(")");
        }
    }
}
