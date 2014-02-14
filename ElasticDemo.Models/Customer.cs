using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
namespace ElasticDemo.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }

        [ElasticProperty(Index=FieldIndexOption.not_analyzed)]
        public string Name { get; set; }
    }
}
