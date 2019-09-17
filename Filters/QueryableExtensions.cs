using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Collections.Generic;
using System;

namespace DynamicFilters.Filters
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ToFilterView<T>(
            this IQueryable<T> query, FilterDTO filter)
        {
            // filter
            query = Filter(query, filter.Filter);
            //sort
            if (filter.Sort != null) {
                query = Sort(query, filter.Sort);
                // EF does not apply skip and take without order
                query = Limit(query, filter.Limit, filter.Offset);
            }
            // return the final query
            return query;
        }

        private static IQueryable<T> Filter<T>(
            IQueryable<T> queryable, Filter filter)
        {
            if ((filter != null) && (filter.Logic != null))
            {
                var filters = GetAllFilters(filter);
                var values = filters.Select(f => f.Value).ToArray();
                var where = Transform(filter, filters);
                queryable = queryable.Where(where, values);
            }
            return queryable;
        }

        private static IQueryable<T> Sort<T>(
            IQueryable<T> queryable, IEnumerable<Sort> sort)
        {
            if (sort != null && sort.Any())
            {
                var ordering = string.Join(",", sort.Select(s => $"{s.Field} {s.Dir}"));
                return queryable.OrderBy(ordering);
            }
            return queryable;
        }

        private static IQueryable<T> Limit<T>(IQueryable<T> queryable, int limit, int offset)
        {
            return queryable.Skip(offset).Take(limit); 
        }

        private static readonly IDictionary<string, string> 
        Operators = new Dictionary<string, string>
        {
            {"eq", "="},
            {"neq", "!="},
            {"lt", "<"},
            {"lte", "<="},
            {"gt", ">"},
            {"gte", ">="},
            {"startswith", "StartsWith"},
            {"endswith", "EndsWith"},
            {"contains", "Contains"},
            {"doesnotcontain", "Contains"},
        };

        public static IList<Filter> GetAllFilters(Filter filter)
        {
            var filters = new List<Filter>();
            GetFilters(filter, filters);
            return filters;
        }

        private static void GetFilters(Filter filter, IList<Filter> filters)
        {
            if (filter.Filters != null && filter.Filters.Any())
            {
                foreach (var item in filter.Filters)
                {
                    GetFilters(item, filters);
                }
            }
            else
            {
                filters.Add(filter);
            }
        }

        public static string Transform(Filter filter, IList<Filter> filters)
        {
            if (filter.Filters != null && filter.Filters.Any())
            {
                return "(" + String.Join(" " + filter.Logic + " ", 
                    filter.Filters.Select(f => Transform(f, filters)).ToArray()) + ")";
            }
            int index = filters.IndexOf(filter);
            var comparison = Operators[filter.Operator];
            if (filter.Operator == "doesnotcontain")
            {
                return String.Format("({0} != null && !{0}.ToString().{1}(@{2}))",
                    filter.Field, comparison, index);
            }
            if (comparison == "StartsWith" || 
                comparison == "EndsWith" || 
                comparison == "Contains")
            {
                return String.Format("({0} != null && {0}.ToString().{1}(@{2}))",
                filter.Field, comparison, index);
            }
            return String.Format("{0} {1} @{2}", filter.Field, comparison, index);
        }
    }
}