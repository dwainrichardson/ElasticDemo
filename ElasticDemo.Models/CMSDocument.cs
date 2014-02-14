using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticDemo.Models
{
    [ElasticType(SearchAnalyzer = "standard")]
    public class CMSDocument
    {
        public string DocId { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string DocIdNA { get; set; }
        public string FolderId { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string FolderIdNA { get; set; }
        public string PortID { get; set; }
        public long AppInstID { get; set; }
        public string LoanNo { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string LoanNoNA { get; set; }
        public string AdditionalLoanNo { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string AdditionalLoanNoNA { get; set; }
        public long ServiceProviderID { get; set; }
        public string ServiceProvider { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string ServiceProviderNA { get; set; }
        public Customer CustomerEntity { get; set; }
        public string GovCaseNo { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string GovCaseNoNA { get; set; }
        public string Customer { get; set; }
        public string CustomerNA { get; set; }
        public string ParentCustomer { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string ParentCustomerNA { get; set; }
        public string LoanOfficer { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string LoanOfficerNA { get; set; }
        public string ParentLoanOfficer { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string ParentLoanOfficerNA { get; set; }
        public string Address { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string AddressNA { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string StreetNo { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string Street { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string Suffix { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string UnitNo { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string CityNA { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string City { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string State { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string StateNA { get; set; }
        public string Zip { get; set; }
        public string County { get; set; }
        public string Borrower { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string BorrowerNA { get; set; }
        public string Processor { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string ProcessorNA { get; set; }
        public string Reviewer { get; set; }
        [ElasticProperty(IncludeInAll = false, Index = FieldIndexOption.not_analyzed)]
        public string ReviewerNA { get; set; }
        public Nullable<DateTime> OrderDate { get; set; }
        public Nullable<DateTime> DueFromProviderDt { get; set; }
        public Nullable<DateTime> DraftReceivedDt { get; set; }
        [ElasticProperty(Index = FieldIndexOption.not_analyzed, IncludeInAll = false)]
        public string DraftReceivedDtYYYYMM { get; set; }
        public Nullable<DateTime> FirstDraftReceivedDt { get; set; }
        [ElasticProperty(Index = FieldIndexOption.not_analyzed, IncludeInAll = false)]
        public string FirstDraftReceivedDtYYYYMM { get; set; }
        public Nullable<DateTime> DueToCustomerDt { get; set; }
        [ElasticProperty(Index = FieldIndexOption.not_analyzed, IncludeInAll = false)]
        public string DueToCustomerDtYYYYMM { get; set; }
        public Nullable<DateTime> CompletionDt { get; set; }
        [ElasticProperty(Index = FieldIndexOption.not_analyzed, IncludeInAll = false)]
        public string CompletionDtYYYYMM { get; set; }
        public Nullable<DateTime> InspectionDt { get; set; }
        [ElasticProperty(Index = FieldIndexOption.not_analyzed, IncludeInAll = false)]
        public string InspectionDtYYYYMM { get; set; }
        public bool IsFHA { get; set; }
        public string VaFHAType { get; set; }
        public bool HasNewCustMsg { get; set; }
        public bool HasNewMsg { get; set; }
        public string docStatusId { get; set; }
        [ElasticProperty(Index = FieldIndexOption.not_analyzed, IncludeInAll = false)]
        public string docStatusIdNA { get; set; }
        public Nullable<double> Longitude { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> LoanAmt { get; set; }
        public Nullable<double> ApprValue { get; set; }
        public Nullable<double> SalesAmt { get; set; }
        [ElasticProperty(Index = FieldIndexOption.not_analyzed, IncludeInAll = false)]
        public Nullable<double> vendorFee { get; set; }
        [ElasticProperty(Index = FieldIndexOption.not_analyzed, IncludeInAll = false)]
        public Nullable<double> custumerFee { get; set; }
        public Nullable<DateTime> AssignDate { get; set; }
        public string LoanPurposeId { get; set; }
        [ElasticProperty(Index = FieldIndexOption.not_analyzed, IncludeInAll = false)]
        public string LoanPurposeIdNA { get; set; }
        public string Channel { get; set; }
        [ElasticProperty(Index = FieldIndexOption.not_analyzed, IncludeInAll = false)]
        public string ChannelNA { get; set; }
        public long ServiceId { get; set; }


    }
}
