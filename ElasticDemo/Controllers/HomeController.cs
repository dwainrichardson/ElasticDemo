using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nest;
using Newtonsoft.Json;
using ElasticDemo.Models;
using Aspose;
using Aspose.Cells;
using System.IO;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Data.SqlClient;
using FluentValidation;
using FluentValidation.Results;


namespace ElasticDemo.Controllers
{
    public class HomeController : Controller
    {
        private static ElasticClient ElasticClient
        {
            get
            {
                //var setting = new ConnectionSettings(new Uri("http://localhost:9200/"));
                var setting = new ConnectionSettings(new Uri("http://dev-elasticsearch:9200/"));
                setting.SetDefaultIndex("fnc");
                return new ElasticClient(setting);
            }
            //
            // GET: /Store/
        }

        public ActionResult Index()
        {
            CMSDocument myDoc = new CMSDocument()
            {
                 AssignDate = DateTime.Now.AddYears(-21),
                  OrderDate = DateTime.Now.AddYears(-19)

            };

            var validator = new CMSDocumentValidator();
            var results =  validator.Validate(myDoc);

            return View();
        }

        public ActionResult Map()
        {


            var resonse = ElasticClient.MapFromAttributes<CMSDocument>();
            return RedirectToAction("Index");
        }

        public ActionResult ReIndex()
        {
            var setting = new ConnectionSettings(new Uri("http://localhost:9200"));
            var client = new ElasticClient(setting);
            // Document myDoc = new Document();
            //   client.Index(myDoc, "fnc", "documents");


            string docid = "20140125-0000-";
            string folderid = "20140125-0000";
            for (int i = 1; i < 10; i++)
            {
                for (int j = 1; j < 101; j++)
                {

                    if (j % 2 == 0)
                    {
                        Document myDoc = new Document()
                        {
                            Customer = new Customer()
                            {
                                CustomerID = i * j,
                                Name = "Customer" + i.ToString() + j.ToString()
                            },
                            DocID = docid + i.ToString() + j.ToString(),
                            FolderID = folderid,
                            OrderDate = DateTime.Now.AddDays(i),
                            PortID = "CHQ" + i.ToString(),
                            IsFHA = i == j,
                            HasNewCustMsg = i == j,
                            HasNewMsg = i == j,
                            VaFHAType = i == j ? "203" : "VA",
                            ServiceProvider = i == j ? "Dwain Richardson" : "Diana Richardson",
                            ServiceProviderNA = i == j ? "Dwain Richardson" : "Diana Richardson",
                            Address = i == j ? "200 Spring lake Cove" : "1030 SW 100th Terrace",
                            AddressNA = i == j ? "200 Spring lake Cove" : "1030 SW 100th Terrace",
                            State = i == j ? "MS" : "FL",
                            StateNA = i == j ? "MS" : "FL",
                            City = i == j ? "Oxford" : "Pembroke Pines",
                            CityNA = i == j ? "Oxford" : "Pembroke Pines",
                            AssignDate = DateTime.Now.AddDays(-j)


                        };
                        client.Index(myDoc, "fnc", "documents", myDoc.DocID + myDoc.PortID);
                    }
                    else
                    {

                        Document myDoc = new Document()
                        {
                            Customer = new Customer()
                            {
                                CustomerID = i * j,
                                Name = i == j ? "Bank Of America" + i.ToString() + j.ToString() : "Chase QC" + i.ToString() + j.ToString()
                            },
                            DocID = docid + i.ToString() + j.ToString(),
                            FolderID = folderid,
                            OrderDate = DateTime.Now.AddDays(i),
                            PortID = "CHQ" + i.ToString(),
                            IsFHA = i == j,
                            HasNewCustMsg = i == j,
                            HasNewMsg = i == j,
                            VaFHAType = i == j ? "203" : "VA",
                            ServiceProvider = i == j ? "Mervin Scot" : "James Gumbs",
                            ServiceProviderNA = i == j ? "Mervin Scot" : "James Gumbs",
                            Address = i == j ? "1214 Office Park Drive" : "380 AGATE Drive",
                            AddressNA = i == j ? "1214 Office Park Drive" : "380 AGATE Drive",
                            State = i == j ? "MS" : "CA",
                            StateNA = i == j ? "MS" : "CA",
                            City = i == j ? "Oxford" : "Laguna Beach",
                            CityNA = i == j ? "Oxford" : "Laguna Beach",

                            AssignDate = DateTime.Now.AddDays(-j)


                        };
                        client.Index(myDoc, "fnc", "documents", myDoc.DocID + myDoc.PortID);
                    }

                }
            }


            return RedirectToAction("Index");
        }


        public ActionResult ReIndexCmsData()
        {
            GetCMSData();
            return RedirectToAction("Index");
        }
        public void GetCMSData()
        {
            SqlCommand command = new SqlCommand();
            string sqlQuery = @"
	 Select  
	  Ai.SubPortID, Ai.FullPortId,
	  Case isnull(dstat.MesgNewCnt,0) when 0 then 'False' else 'True' END as [Has New Message],
	  CASE dstat.HasFollowUp when 1 THEN 'True' Else 'False' END as [Has Follow Up], 
	  	  isnull( dstat.IsRushOrder,0) as  [Is Rush Order],
	 --isnull(FoldInd.RushOrder,0) as [Is Rush Order],
	
	 case isnull(FoldInd.IsBorrowerCC,0) when 1 then 'Yes' else 'No' end IsBorrowerCC,
	 case isnull(d.isFha, 0) when 1 then 'yes' else 'no' end as IsFHA,
	  case isnull(L.IsHighValue, 0) when 1 then 'yes' else 'no' end as IsHighValue,
	  d.LoanNo as [LoanNo],
	  d.AppInstId,
	  isnull(d.ApprValue,0) as ApprValue,
      ds.Description as Status,
	  sa.ServiceAlias as [Service Name],
      isnull(fm.ApplicantFName+' '+fm.ApplicantLName,'') as Borrower,
	  isnull(dbo.ufgetPrsnName(d.ProcessorPrsn),'') as Processor,
	  isnull(dbo.ufgetPrsnName(d.PreparerPrsn),'') as [Service Provider],
	  isnull(dbo.ufgetPrsnName(d.ReviewerPrsn),'') as Reviewer,
	  d.CreatedDt as [Order Date],
	 
	  isnull(convert(varchar(20),dd.DueFromVendorDt,111),'') as [Due from Service Provider],
	  isnull(convert(varchar(20),dd.DraftDt,111),'') as [Date Draft Received],
      isnull(dd.FirstDraftDt,'') as [First Draft Date],
	  isnull(convert(varchar(20),dd.FirstDraftDt,111),'') as [First Draft Received],
	  isnull(convert(varchar(20),dd.DueToCustDt,111),'') as [Due to Customer], 
	  isnull(convert(varchar,dd.CompletionDt,111),'') as [Date Completed],
	  isnull(dd.CompletionDt,'') as CompletionDt,
	  isnull(dd.DueToCustDt,'') as DueToCustDt,
	 	isnull(dd.DraftDt,'') as DraftDt,
		isnull(dd.DueFromVendorDt,'') as DueFromVendorDt,

	  isnull(Org.OrgName,'') as Customer, 
	  Org.ExtOrgID as [Branch #],
	  isnull(P.Longitude,'') as Longitude,
	  isnull(P.Latitude,'') as Latitude,
	  isnull(pa.SalesPrice,0) as SalesPrice,
	  isnull(DC.CustomerBillFee,0) as CustomerBillFee
      ,isnull(DC.CustomerAdjFee,0)
      ,isnull(DC.VendorBillFee,0) as VendorBillFee
      ,isnull(DC.VendorAdjFee,0),

	  isnull(P.StreetNo,'')+' '+isnull(P.Prefix,'')+' '+isnull(P.Street,' ')+' '+isnull(P.Suffix,'') +' '+isnull(P.UnitNo,'') as Address,
				isnull(P.StreetNo,'') as StreetNo,
				isnull(P.Street,' ') as Street,
				isnull(P.Prefix,'') as Prefix,
				isnull(P.Suffix,'') as Suffix,
				isnull(P.UnitNo,'') as UnitNo,
			   P.City as City,
			   P.State as State, 
			   P.Zip as Zip,
			   isnull(P.County,'') as County,  
			   d.DocId as [Doc ID], 
			  ds.DocStatusID as [Status ID],  
			  dd.DueFromVendorDt as [Row Color Date],
			  isnull(dd.AssignedDt,'') as AssignedDt,
			  isnull(dd.InspectionDt,'') as InspectionDt,
			 isnull(fm.Reference1,'') as [Additional Loan number] ,
			 org.ExtOrgPk as [Customer ID],
			 dbo.ufgetPrsnName(f.originationPrsn) as [Loan Officer Broker], 
			 convert(varchar(20),dd.InspectionDt,111) as [Inspection Date],
			 f.originationPrsn, f.OriginationOrg , f.OwnerOrg, f.ParentOriginationOrg, f.ParentOriginationPrsn,f.FolderID,f.recedatetime,
			 l.LoanPurposeID, l.VaFhaTypeID,l.GovCaseNo,  LP.Description as [Loan Purpose], isnull(dstat.MesgNewCnt,'') as MesgNewCnt, dstat.HasFollowUp,dd.DueToCustDt,dd.DueFromVendorDt,dd.InspectionDt, dd.CompletionDt, s.ServiceTypeID,
			 fm.ApplicantFName, fm.ApplicantLName, d.processorprsn, d.ReviewerPrsn, d.preparerprsn, d.processingorg,d.SupervisorPrsn,d.EscalatedPrsn, isnull(Org.OrgName,'') as [Group], c.parentCustOrgPk, dp.IsMainprop, s.ServiceID, fm.Reference1,
			-- dc.docchargestatusid, dc.InvoiceDate, dc.vendoradjfee, dc.customeradjfee, dc.CustDocChargeStatusID,
			  isnull(vft.description,'') as [SubLoanType], isnull(l.LoanAmt,0) as LoanAmt
	
	  from Folder f with (nolock)
	  inner join FolderMisc fm (nolock) ON  (f.folderid = fm.folderid)
      inner join Doc d with (nolock) ON (f.folderid = d.folderid) 
	 
      inner join DocStatus ds with (nolock) ON ( d.DocStatusID = ds.DocStatusID)
	  inner join AppInst Ai with (nolock) on d.AppinstiD = Ai.AppinstID
      left join APSLProcess APSL with (nolock) ON d.APSLProcessID = APSL.APSLProcessID
      inner join DocDates dd with (nolock) ON d.DocID=dd.DocID
      inner join DocProp dp with (nolock) ON d.DocID=dp.DocID
      inner  join DocStatistics dstat with (nolock) ON d.DocID = dstat.DocID
      inner join Service s with (nolock) ON (d.ServiceID = s.ServiceID)
      inner join ServiceAttr sa with (nolock) ON (d.ServiceID = sa.ServiceID and d.AppInstId = sa.AppInstId)
	  left join Organization Org ON f.OriginationOrg = Org.OrgPk
      left  join [Property] p with (nolock) ON dp.PropID = p.PropID
	  left join PropertyAttr pa with (nolock) on p.PropID = pa.propID
      left join FolderIndicators FoldInd with (nolock) on D.FolderID = FoldInd.FolderID
	  left join loan L with(nolock) on d.folderId = L.folderId
      left join LoanPurpose LP with (nolock) on (L.LoanPurposeID = LP.LoanPurposeID)
	   left join DocCharge dc with (nolock) on (d.DocID = dc.DocId)
	   left join Customer C with (nolock) on (f.OriginationOrg = c.OrgPk)
	    left join VaFhaType vft with (nolock) on  l.VaFhaTypeID = vft.VaFHATypeID";
            string connectionString = @"SERVER=collateralhq.dev.db.fncinc.com,14360; Database=collateralhq; Integrated Security=SSPI;";
            using (var conn = new SqlConnection(connectionString))
            {

                command.Connection = conn;
                command.CommandText = sqlQuery;
                conn.Open();
                SqlDataReader dr = command.ExecuteReader();

                while (dr.Read())
                {
                    CMSDocument myDoc = new CMSDocument()
                    {
                        PortID = dr["FullPortID"].ToString(),
                        AdditionalLoanNo = dr["Additional Loan Number"].ToString(),
                        AdditionalLoanNoNA = dr["Additional Loan Number"].ToString(),
                        Address = dr["Address"].ToString(),
                        AddressNA = dr["Address"].ToString(),
                        AppInstID = long.Parse(dr["AppInstID"].ToString()),
                        AssignDate = DateTime.Parse(dr["AssignedDt"].ToString()),
                        Borrower = dr["Borrower"].ToString(),
                        BorrowerNA = dr["Borrower"].ToString(),
                        City = dr["City"].ToString(),
                        CityNA = dr["City"].ToString(),
                        CompletionDt = DateTime.Parse(dr["CompletionDt"].ToString()),
                        CompletionDtYYYYMM = dr["Date Completed"].ToString(),
                        County = dr["County"].ToString(),
                        DocId = dr["Doc ID"].ToString(),
                        DocIdNA = dr["Doc ID"].ToString(),
                        Customer = dr["Group"].ToString(),
                        CustomerNA = dr["Group"].ToString(),
                        ServiceProvider = dr["Service Provider"].ToString(),
                        ServiceProviderNA = dr["Service Provider"].ToString(),
                        custumerFee = double.Parse(dr["CustomerBillFee"].ToString()),
                        vendorFee = double.Parse(dr["VendorBillFee"].ToString()),
                        docStatusId = dr["Status"].ToString(),
                        docStatusIdNA = dr["Status"].ToString(),
                        FolderId = dr["FolderID"].ToString(),
                        FolderIdNA = dr["FolderID"].ToString(),
                        Street = dr["Street"].ToString(),
                        StreetNo = dr["StreetNo"].ToString(),
                        Suffix = dr["Suffix"].ToString(),
                        UnitNo = dr["UnitNo"].ToString(),
                        GovCaseNo = dr["GovCaseNo"].ToString(),
                        GovCaseNoNA = dr["GovCaseNo"].ToString(),
                        Zip = dr["Zip"].ToString(),
                        State = dr["State"].ToString(),
                        StateNA = dr["State"].ToString(),
                        Latitude = double.Parse(dr["Latitude"].ToString()),
                        Longitude = double.Parse(dr["Longitude"].ToString()),
                        LoanAmt = double.Parse(dr["LoanAmt"].ToString()),
                        LoanNo = dr["LoanNo"].ToString(),
                        LoanNoNA = dr["LoanNo"].ToString(),
                        LoanOfficer = dr["Loan Officer Broker"].ToString(),
                        LoanOfficerNA = dr["Loan Officer Broker"].ToString(),
                        VaFHAType = dr["SubLoanType"].ToString(),
                        Processor = dr["Processor"].ToString(),
                        ProcessorNA = dr["Processor"].ToString(),
                        Reviewer = dr["Reviewer"].ToString(),
                        ReviewerNA = dr["Reviewer"].ToString(),
                        OrderDate = DateTime.Parse(dr["Order Date"].ToString()),
                        DraftReceivedDt = DateTime.Parse(dr["DraftDt"].ToString()),
                        DueToCustomerDt = DateTime.Parse(dr["DueToCustDt"].ToString()),
                        DueFromProviderDt = DateTime.Parse(dr["DueFromVendorDt"].ToString()),
                        FirstDraftReceivedDt = DateTime.Parse(dr["First Draft Date"].ToString()),
                        InspectionDt = DateTime.Parse(dr["InspectionDt"].ToString()),
                        HasNewMsg = dr["Has New Message"].ToString() == "True" ? true : false,
                        IsFHA = dr["IsFHA"].ToString() == "True" ? true : false,
                        ApprValue = double.Parse(dr["ApprValue"].ToString()),
                        InspectionDtYYYYMM = dr["Inspection Date"].ToString(),
                        DraftReceivedDtYYYYMM = dr["Date Draft Received"].ToString(),
                        FirstDraftReceivedDtYYYYMM = dr["First Draft Received"].ToString(),
                        DueToCustomerDtYYYYMM = dr["Due to Customer"].ToString(),
                        CustomerEntity = new Customer()
                        {
                            Name = dr["Group"].ToString()
                        }

                    };

                    ElasticClient.Index(myDoc, "fnc", "cmsdocuments", myDoc.PortID + myDoc.DocId);

                }

                dr.Close();
            }

   
        }

        public ActionResult IndexCMSData()
        {
            CMSDocument myCms = new CMSDocument();
            ElasticClient.Index(myCms, "fnc", "cmsdocuments");
            return RedirectToAction("Index");
        }


        public JsonResult getDocuments(string param, PropertySearchInformation PropertyModel)
        {


            PropertySearchInformation searchResult = new PropertySearchInformation();

            if (string.IsNullOrEmpty(searchResult.SearchText))
                searchResult.SearchText = param;


            var res = ElasticClient.Search<CMSDocument>(s => s
                .From(0)
                .Size(20)
                .Query(q => q.Bool(qb => qb.Must(qm => qm.QueryString(qs => qs.Query(param).OnFields(new List<string>() { "addressna", "address", "customer.name", "cityna", "city", "state", "serviceProviderNA", "docID", "portID" })))))
                //m => m.Terms("bed","3"))))
                //.Query(q => q.Bool(qb => qb.Must(qm => qm.Term("portID", param.ToLower()))))
                .FacetTerm(t => t.OnField(d => d.IsFHA).Size(20))
                .FacetTerm(t => t.OnField(d => d.VaFHAType).Size(20))
                .FacetTerm(t => t.OnField("cityNA"))
                .FacetTerm(t => t.OnField("state"))
                .FacetTerm(t => t.OnField(d => d.ServiceProviderNA))
                .FacetTerm(t => t.OnField(d => d.CustomerEntity.Name))
                .Highlight(h => h.OnFields(f => f.OnField("city"))
                          .PreTags("<span style='background-color:yellow'>")
                                              .PostTags("</span>"))
                );
            var faceTerm = res.Facet<TermFacet>(t => t.IsFHA);

            var facetIt = res.FacetItems<FacetItem>(p => p.IsFHA);

            List<StringFacetSearchItem> cityFacetResults = res.FacetItems<TermItem>("state")
                    .Select(item => new StringFacetSearchItem { Key = item.Term, Count = item.Count, Checked = false }).ToList();


            List<CMSDocument> resultProperties = res.DocumentsWithMetaData
                .Select<dynamic, CMSDocument>(meta =>
                {
                    CMSDocument result = JsonConvert.DeserializeObject<CMSDocument>(JsonConvert.SerializeObject(meta.Source));
                    return result;
                }).ToList();
            searchResult.CityFacet = cityFacetResults;
            searchResult.Properties = resultProperties;

            List<PropertySearchInformation> results = new List<PropertySearchInformation>();
            results.Add(searchResult);
            return Json(searchResult, JsonRequestBehavior.AllowGet);
            //Document doc = new Document();
            //doc.cityFacet = cityFacetResults;
            //if (res.Documents.Any())
            //{
            //    return Json(res.Documents, JsonRequestBehavior.AllowGet);
            //}
            //else
            //    return Json(doc, JsonRequestBehavior.AllowGet);
        }

        public JsonResult uploadSpreadSheet(string filePath)
        {

            Workbook wb = new Workbook();
            string fileName = Path.GetFileName(filePath);
            string directory = @"C:\Users\drichardson\Downloads";
            wb.Open(Path.Combine(directory, fileName));

            int rows = wb.Worksheets[0].Cells.MaxDataRow + 1;
            int cols = wb.Worksheets[0].Cells.MaxDataColumn + 1;
            DataTable dt = wb.Worksheets[0].Cells.ExportDataTableAsString(0, 0, rows, cols);

            //List<UploadedFile> uploadResults = new List<UploadedFile>();
            ConcurrentBag<UploadedFile> uploadResults = new ConcurrentBag<UploadedFile>();
            string value = string.Empty;
            States dictStates = new States();
            Parallel.ForEach(dt.AsEnumerable(), new ParallelOptions() { MaxDegreeOfParallelism = 2 }, entry =>
            {
                UploadedFile uf = new UploadedFile();
                for (int i = 0; i < cols; i++)
                {
                    value = entry[i].ToString();
                    switch (i)
                    {

                        case 0:
                            uf.License = value;
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                uf.LicenseIsValid = true;
                            }

                            break;
                        case 1:

                            uf.State = value;
                            if (string.Compare(dictStates.returnState(value), "No Match Found") != 0)
                            {

                                uf.StateIsValid = true;
                            }

                            break;
                        case 2:
                            if (!string.IsNullOrEmpty(value))
                            {
                                uf.FirstName = value;
                                uf.FirstNameValid = true;
                            }
                            break;
                        case 3:
                            if (!string.IsNullOrEmpty(value))
                            {
                                uf.LastName = value;
                                uf.LastNameValid = true;
                            }

                            break;
                    }


                }
                if (uploadResults.Where(u => u.License == uf.License && u.State == uf.State).Any())
                    uf.duplicateRow = true;

                uploadResults.Add(uf);

            });



            UploadedFile df = new UploadedFile();
            df.LicenseIsValid = false;
            uploadResults.Add(df);

            return Json(uploadResults, JsonRequestBehavior.AllowGet);
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}