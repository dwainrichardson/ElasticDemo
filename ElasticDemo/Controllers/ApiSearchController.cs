using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Nest;
using ElasticDemo.Models;
using Newtonsoft.Json;

namespace ElasticDemo.Controllers
{
    public class ApiSearchController : ApiController
    {

        private static ElasticClient ElasticClient
        {
            get
            {
                var setting = new ConnectionSettings(new Uri("http://dev-elasticsearch:9200/"));
                setting.SetDefaultIndex("fnc");
                return new ElasticClient(setting);
            }
            //
            // GET: /Store/
        }


        public PropertySearchInformation Get()
        {
            PropertySearchInformation nestSearch = new PropertySearchInformation
            {
                SearchText = "CHQ1",
                pageSize = 10,
                BedFacet = new List<StringFacetSearchItem> 
				{
					new StringFacetSearchItem{Checked = false, Key="1"},
					new StringFacetSearchItem{Checked = false, Key="2"},
					new StringFacetSearchItem{Checked = false, Key="3"},
					new StringFacetSearchItem{Checked = false, Key="4"},
					new StringFacetSearchItem{Checked = false, Key="5"},
					new StringFacetSearchItem{Checked = false, Key="6"}
				},
                BathFacet = new List<StringFacetSearchItem>
				{
					new StringFacetSearchItem{Checked = false, Key="1"},
					new StringFacetSearchItem{Checked = false, Key="2"},
					new StringFacetSearchItem{Checked = false, Key="3"},
					new StringFacetSearchItem{Checked = false, Key="4"}
				},
             CustomerFacet = new List<StringFacetSearchItem>(),
              ServiceProviderFacet = new List<StringFacetSearchItem>(),
                NewMsgFacet = new List<StringFacetSearchItem>(),
                StateFacet = new List<StringFacetSearchItem>(),
                CityFacet = new List<StringFacetSearchItem>(),
                Properties = new List<CMSDocument>(),
                SearchTimeInSeconds = 0,
                TotalProperties = 0,
                AddressOnlySearch = true
            };
            return nestSearch;
        }
        // POST api/<controller>
        public PropertySearchInformation Post(PropertySearchInformation facetSearch)
        {
            
           // PropertySearchInformation searchResult = new PropertySearchInformation();

           // if (string.IsNullOrEmpty(searchResult.SearchText))
             //   searchResult.SearchText = param;
            string param = "";

            List<string> cityFilter = facetSearch.CityFacet.Where(item => item.Checked == true).Select(item => item.Key).ToList();
            List<string> stateFilter = facetSearch.StateFacet.Where(item => item.Checked == true).Select(item => item.Key).ToList();
            List<string> newMsgsFilter = facetSearch.NewMsgFacet.Where(item => item.Checked == true).Select(item => item.Key).ToList();
            List<string> customerFilter = facetSearch.CustomerFacet.Where(item => item.Checked == true).Select(item => item.Key).ToList();
            List<string> serviceProviderFilter = facetSearch.ServiceProviderFacet.Where(item => item.Checked == true).Select(item => item.Key).ToList();

            var res = ElasticClient.Search<CMSDocument>(s => s
               .From(0)
                .Size(facetSearch.pageSize)
                .Query(q => q.Bool(qb => qb.Must(qm => qm.QueryString(qs => qs.Query(facetSearch.SearchText)
                    .OnFields(new List<string>() { "addressNA", "address", "citynNA", "city", "state","customer.name", "serviceProviderNA", "docID", "portID" })),
                     qm => qm.Terms("state",stateFilter.ToArray()), 
                     qm => qm.Terms("cityNA",cityFilter.ToArray()),
                     qm => qm.Terms("serviceProviderNA",serviceProviderFilter.ToArray()),
                     qm => qm.Terms("customer",customerFilter.ToArray()),
                     qm => qm.Terms("hasNewMsg", newMsgsFilter.ToArray()))))
                //m => m.Terms("bed","3"))))
                //.Query(q => q.Bool(qb => qb.Must(qm => qm.Term("portID", param.ToLower()))))
                .FacetTerm(t => t.OnField(d => d.IsFHA))
                .FacetTerm(t => t.OnField(d => d.VaFHAType))
                .FacetTerm(t => t.OnField("cityNA"))
                .FacetTerm(t => t.OnField("state"))
                .FacetTerm(t => t.OnField(d => d.HasNewMsg))
                 .FacetTerm(t => t.OnField(d => d.ServiceProviderNA))
                .FacetTerm(t => t.OnField(d => d.Customer))
                .Highlight(h => h.OnFields(f => f.OnField("city"))
                          .PreTags("<span style='background-color:yellow'>")
                                              .PostTags("</span>"))
                );
            var faceTerm = res.Facet<TermFacet>(t => t.IsFHA);

            var facetIt = res.FacetItems<FacetItem>(p => p.IsFHA);

            List<StringFacetSearchItem> stateFacetResults = res.FacetItems<TermItem>("state")
                    .Select(item => new StringFacetSearchItem { Key = item.Term, Count = item.Count, Checked = false }).ToList();

            List<StringFacetSearchItem> cityFacetResults = res.FacetItems<TermItem>("cityNA")
                   .Select(item => new StringFacetSearchItem { Key = item.Term, Count = item.Count, Checked = false }).ToList();

            List<StringFacetSearchItem> newMsgResults = res.FacetItems<TermItem>("hasNewMsg")
                .Select(item => new StringFacetSearchItem { Key = item.Term, Count = item.Count, Checked = false }).ToList();

            List<StringFacetSearchItem> newServiceProviderResults = res.FacetItems<TermItem>(d => d.ServiceProviderNA)
                .Select(item => new StringFacetSearchItem { Key = item.Term, Count = item.Count, Checked = false }).ToList();

            List<StringFacetSearchItem> newCustomerResults = res.FacetItems<TermItem>(d => d.Customer)
                .Select(item => new StringFacetSearchItem { Key = item.Term, Count = item.Count, Checked = false }).ToList();


            List<CMSDocument> resultProperties = res.DocumentsWithMetaData
                .Select<dynamic, CMSDocument>(meta =>
                {
                    CMSDocument result = JsonConvert.DeserializeObject<CMSDocument>(JsonConvert.SerializeObject(meta.Source));
                    return result;
                }).Skip(0).Take(facetSearch.pageSize).ToList();

            //.Skip(0).Take(facetSearch.pageSize).
          //  searchResult.CityFacet = cityFacetResults;
            //searchResult.Properties = resultProperties;
            facetSearch.ServiceProviderFacet = UpdateFacetResultsNewOnly(facetSearch.ServiceProviderFacet, newServiceProviderResults);
            facetSearch.CustomerFacet = UpdateFacetResultsNewOnly(facetSearch.CustomerFacet, newCustomerResults);
            facetSearch.CityFacet = UpdateFacetResultsNewOnly(facetSearch.CityFacet, cityFacetResults);
            facetSearch.StateFacet = UpdateFacetResultsNewOnly(facetSearch.StateFacet, stateFacetResults);
            facetSearch.NewMsgFacet = newMsgResults;
            facetSearch.Properties = resultProperties;
          //  List<PropertySearchInformation> results = new List<PropertySearchInformation>();
            //results.Add(searchResult);

            return facetSearch;
        }




        private static List<StringFacetSearchItem> UpdateFacetResultsNewOnly(List<StringFacetSearchItem> existingList, List<StringFacetSearchItem> newList)
        {
            //Update new to be checked if it was
            newList.ForEach(item =>
            {
                        var existingItem = existingList.FirstOrDefault(existingItemInExisting => existingItemInExisting.Key == item.Key);
                var existingCheckedItem = existingList.FirstOrDefault(existingItemInExisting => existingItemInExisting.Key == item.Key && existingItemInExisting.Checked);
                if (existingCheckedItem != null)
                {
                    item.Checked = true;
                }


                
              //      newList.Add(existingList.FirstOrDefault());
                
                 
              
            });

            //Sort
            return newList.OrderBy(item => item.Key).ToList();
        }
    }
}