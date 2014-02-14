using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace ElasticDemo.Models
{

    [ElasticType(SearchAnalyzer="standard")]
    public class Document
    {
        
        public string DocID { get; set; }
        public string FolderID { get; set; }
        public string PortID { get; set; }
        public DateTime OrderDate { get; set; }

        public long ServiceProviderID { get; set; }

        
        public string ServiceProvider { get; set; }

        [ElasticProperty(IncludeInAll=false, Index=FieldIndexOption.not_analyzed)]
        public string ServiceProviderNA { get; set; }
        
        public Customer Customer { get; set; }
        
        
        public string Address { get; set; }

        [ElasticProperty(IncludeInAll=false, Index=FieldIndexOption.not_analyzed)]
        public string AddressNA { get; set; }

        public string StreetNo { get; set; }

        [ElasticProperty(IncludeInAll=false, Index=FieldIndexOption.not_analyzed)]
        public string CityNA { get; set; }



        [ElasticProperty(IncludeInAll=false, Index=FieldIndexOption.not_analyzed)]
        public string City { get; set; }
        
        [ElasticProperty(IncludeInAll=false, Index=FieldIndexOption.not_analyzed)]
        public string State { get; set; }

        [ElasticProperty(IncludeInAll=false, Index=FieldIndexOption.not_analyzed)]
        public string StateNA { get; set; }

 

        public bool IsFHA { get; set; }
        
        public string VaFHAType { get; set; }
        
        public bool HasNewCustMsg { get; set; }
        
        public bool HasNewMsg { get; set; }
        
        public DateTime AssignDate { get; set; }

        public List<StringFacetSearchItem> cityFacet { get; set; }





    }
}
