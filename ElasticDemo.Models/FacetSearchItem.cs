using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticDemo.Models
{
    public abstract class BaseFacetSearchItem
    {
        public bool Checked { get; set; }
        public int Count { get; set; }
    }

    public class StringFacetSearchItem : BaseFacetSearchItem
    {
        public string Key { get; set; }
    }

    public class DoubleFacetSearchItem : BaseFacetSearchItem
    {
        public double Key { get; set; }
    }

    public class DoubleRangeFacetSearchItem : DoubleFacetSearchItem
    {
        public double Range { get; set; }
    }
}
