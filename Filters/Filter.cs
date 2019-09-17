using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

namespace DynamicFilters.Filters 
{
    public class Filter
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public object Value { get; set; }
        public string Logic { get; set; }
        public IEnumerable<Filter> Filters { get; set; }
    }
}