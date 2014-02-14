using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElasticDemo.Models;

namespace ElasticDemo.Models
{
	public class PropertySearchInformation
	{
		public string SearchText { get; set; }
		public double SearchTimeInSeconds { get; set; }
		public int TotalProperties { get; set; }
        public int pageSize { get; set; }
        public string Action { get; set; }

		public bool AddressOnlySearch { get; set; }
		public List<CMSDocument> Properties { get; set; }
        public List<StringFacetSearchItem> CustomerFacet { get; set; }
        public List<StringFacetSearchItem> ServiceProviderFacet { get; set; }
		public List<StringFacetSearchItem> BedFacet { get; set; }
		public List<StringFacetSearchItem> BathFacet { get; set; }
		public List<StringFacetSearchItem> CityFacet { get; set; }
        public List<StringFacetSearchItem> NewMsgFacet { get; set; }
        public List<StringFacetSearchItem> StateFacet { get; set; }
    }

}
