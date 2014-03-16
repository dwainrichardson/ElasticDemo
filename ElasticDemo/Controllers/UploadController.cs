using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElasticDemo.Models;
namespace ElasticDemo.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult GetInformation()
        {
            DwainEntities cte = new DwainEntities();
            var instances = from a in cte.AppInsts
                            select new Picker { Description = a.AppInstName, Id = a.AppInstId };

            var ListTypes = from c in cte.ListTypes
                            select new Picker { Description = c.Enum, Id = c.ListTypeID };

            var customers = from c in cte.Customers
                            select new Picker { Description = c.CustomerName, Id= c.CustomerId};

            var lists = from l in cte.AppinstLists
                        select new Picker { Description = l.ListName, Id = l.AppinstListID };

            BatchUploadInformation bui = new BatchUploadInformation()
            {
                PageNumber = 1,
                 PageSize = 100,
                filePath = "",
                 ListName = "",
                  Instances = instances.ToList(),
                 ListTypes = ListTypes.ToList(),
                 Customers = customers.ToList(),
                  CurrentLists  = new List<Picker>(), //lists.ToList(),
                Rows = new List<UploadedFile>(),
                InValidRows = new List<UploadedFile>()
            };
            
            return Json(bui, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RetrieveInformation(BatchUploadInformation current)
        {
            DwainEntities cte = new DwainEntities();
            var myBui = from b in cte.AppinstLists
                        where b.AppInstId == current.SelectedInstance
                        select b;

            var instances = from a in cte.AppInsts
                            select new Picker { Description = a.AppInstName, Id = a.AppInstId };

            var ListTypes = from c in cte.ListTypes
                            select new Picker { Description = c.Enum, Id = c.ListTypeID };

            var customers = from c in cte.Customers
                            select new Picker { Description = c.CustomerName, Id = c.CustomerId };

            var lists = from l in myBui
                        select new Picker { Description = l.ListName, Id = l.AppinstListID };

            BatchUploadInformation bui = new BatchUploadInformation()
            {
                PageNumber = 1,
                PageSize = 100,
                filePath = "",
                ListName = "",
                Instances = instances.ToList(),
                ListTypes = ListTypes.ToList(),
                Customers = customers.ToList(),
                CurrentLists = lists.ToList(),
                Rows = new List<UploadedFile>(),
                InValidRows = new List<UploadedFile>()
            };

            return Json(bui, JsonRequestBehavior.AllowGet);
          
        }


        public JsonResult RetrieveRows(BatchUploadInformation current)
        {
            DwainEntities cte = new DwainEntities();
            //current.SelectedList;

            var myBui = from b in cte.AppInstListDetails
                        where b.AppInstListID == current.SelectedList
                        select b;


            var instances = from a in cte.AppInsts
                            select new Picker { Description = a.AppInstName, Id = a.AppInstId };

            var ListTypes = from c in cte.ListTypes
                            select new Picker { Description = c.Enum, Id = c.ListTypeID };

            var customers = from c in cte.Customers
                            select new Picker { Description = c.CustomerName, Id = c.CustomerId };


            List<UploadedFile> fileslist = new List<UploadedFile>();
            foreach (var item in myBui)
            {
                UploadedFile uf = new UploadedFile()
                {
                    State = item.State,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    License = item.LicenseNo,
                    ListType = DetermineListType(item.AppInstListTypeID.Value)

                };
                fileslist.Add(uf);
            }
            

            BatchUploadInformation bui = new BatchUploadInformation()
            {
                PageNumber = 1,
                PageSize = 100,
                filePath = "",
                ListName = "",
                SelectedList = current.SelectedList,
                SelectedInstance = current.SelectedInstance,
                Instances = instances.ToList(),
                ListTypes = ListTypes.ToList(),
                Customers = customers.ToList(),
                CurrentLists = new List<Picker>(),
                Rows = fileslist,
                InValidRows = new List<UploadedFile>()
            };

            return Json(bui, JsonRequestBehavior.AllowGet);

        }



        private string DetermineListType(int p)
        {
            string val = "";
            switch (p)
            {
                case 1:
                    val = "Pre";
                    break;
                case 2:
                    val = "Post";
                    break;
                case 3:
                    val = "Watch";
                    break;
                default:
                    val = "";
                    break;

            }
            return val;
        }
	}

  
}