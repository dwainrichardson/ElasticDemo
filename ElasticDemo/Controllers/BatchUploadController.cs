//using Aspose.Cells;
using ElasticDemo.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ServiceStack.Redis;
using ServiceStack;
using Newtonsoft.Json;
using LinqToExcel;
using LinqToExcel.Query;
using Excel;
using Excel.Core;

namespace ElasticDemo.Controllers
{
    public class BatchUploadController : ApiController
    {
        public BatchUploadInformation Get()
        {
            DwainEntities cte = new DwainEntities();

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
                 ListTypes = ListTypes.ToList(),
                 Customers = customers.ToList(),
                  CurrentLists  = lists.ToList(),
                Rows = new List<UploadedFile>(),
                InValidRows = new List<UploadedFile>()
            };

            return bui;
        }

        public BatchUploadInformation Post(BatchUploadInformation current)
        {

         //   BatchUploadInformation bui = new BatchUploadInformation();
            ConcurrentBag<UploadedFile> uploadResults = new ConcurrentBag<UploadedFile>();
            var validator = new BatchUploadValidator();
            object source;
            string value = string.Empty;
            States dictStates = new States();
            var client = new RedisClient("localhost");
            var redisUsers = client.As<List<UploadedFile>>();

            
            var redisDT = client.As<List<UploadedFile>>();
          

            var hash = redisUsers.GetHash<string>("RicoHash");
            
          var allitems = hash.GetAll();


            var dtHash = redisDT.GetHash<string>("RicoHashDT");
            var LTEHash = redisDT.GetHash<string>("RICOLTEHASH");
            //var allDtHashed = dtHash.GetAll();

            //var elem = allDtHashed.First().Value.FromJson<string>();
            //var deserializedobject = JsonConvert.DeserializeObject<UploadedFile>(allDtHashed.First().Value);
            // redisUsers.SetEntryInHash<Users>(hash, ulist, ulist);
      //      

            if (current.Rows.Count() == 0 || current.Action == "Page")
            {
              //  Workbook wb = new Workbook();
                string fileName = Path.GetFileName(current.filePath);
                string directory = @"C:\Users\RICO\Documents";
                //wb.Open(Path.Combine(directory, fileName));

                //int rows = wb.Worksheets[0].Cells.MaxDataRow + 1;
                //int cols = wb.Worksheets[0].Cells.MaxDataColumn + 1;

                int startRow = (current.PageNumber - 1) * current.PageSize;
                var LTEItems = LTEHash.GetAll();
                
                if (LTEItems.Count() > 0)
                {
             
                    
                     string myKey = "LTEHash" + (current.PageNumber - 1).ToString();
                     var myList = LTEItems.FirstOrDefault().Value.ToList();// LTEItems.Where(t => t.Key == myKey).FirstOrDefault().Value;
                     ValidateRowErrors(myList);
                     CheckErrors(myList);

                     
                   // ValidateRowErrors(LTEItems.FirstOrDefault().Value.Skip(startRow).Take(current.PageSize).ToList());
                   
                    //CheckErrors(LTEItems.FirstOrDefault().Value.Skip(startRow).Take(current.PageSize).ToList());
      
                     current.Rows = myList; // LTEItems.FirstOrDefault().Value.Skip(startRow).Take(current.PageSize).ToList();
                }
                else
                {
                //    FileStream stream = File.Open(Path.Combine(directory, fileName),FileMode.Open,FileAccess.Read);

                //    var reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                //    reader.IsFirstRowAsColumnNames = true;
                //    var workSheet = reader.AsDataSet().Tables["Sheet1"];
                //    var rows = from DataRow a in workSheet.Rows select a;
                //    //List<UploadedFile> uploadedFiles = new List<UploadedFile>();
                //    ConcurrentBag<UploadedFile> uploadedFiles = new ConcurrentBag<UploadedFile>();
                //    Parallel.ForEach(rows, new ParallelOptions { MaxDegreeOfParallelism = 4 }, r => {
                //        lock (r)
                //        {
                //            var file = new UploadedFile()
                //            {
                //                License = r["License"].ToString(),
                //                Street = r["Street"].ToString(),
                //                Zip = r["Zip"].ToString(),
                //                FirstName = r["FirstName"].ToString(),
                //                LastName = r["LastName"].ToString(),
                //                State = r["State"].ToString(),
                //                City = r["City"].ToString(),
                //                StreetNo = r["StreetNo"].ToString()


                //            };
                //            uploadedFiles.Add(file);
                //        }
                //    });

                    var excelFile = new ExcelQueryFactory(Path.Combine(directory, fileName));
                    var uploadedFiles = from a in excelFile.Worksheet<UploadedFile>("Sheet1") select a;

                    DwainEntities dwe = new DwainEntities();

                    AppinstList ail = new AppinstList();
                    ail.FilePath = Path.Combine(directory, fileName);
                    ail.AppInstId = current.SelectedInstance;
                    ail.ListName = current.ListName;
                    ail.CreatedDt = DateTime.Now;
                    ail.IsDefault = true;
                    dwe.AppinstLists.Add(ail);
                    dwe.SaveChanges();


                    ConcurrentBag<UploadedFile> concurrent = new ConcurrentBag<UploadedFile>();

                    foreach (var src in uploadedFiles)
                    {
                        AppInstListDetail aid = new AppInstListDetail();
                        aid.FirstName = src.FirstName;
                        aid.LastName = src.LastName;
                        aid.LicenseNo = src.License;
                        aid.State = src.State;
                        aid.AppInstListTypeID = DetermineListType(src.ListType);
                        aid.AppInstListID = ail.AppinstListID;
                        aid.CreatedDt = DateTime.Now;
                        dwe.AppInstListDetails.Add(aid);

                    }


                    dwe.SaveChanges();

                    //redisDT.SetEntryInHash<string>(LTEHash, "LTEHash", uploadedFiles.ToList());
                    //LTEItems = LTEHash.GetAll();
                    //int count = uploadedFiles.ToList().Count();
                    //int section = count / current.PageSize;
                    //Parallel.For(0, section, i =>
                    //{

                    //    int startIndex = i * current.PageSize;
                    //    string key = "LTEHash" + i.ToString();
                    //    redisDT.SetEntryInHash<string>(LTEHash, key, uploadedFiles.ToList().Skip(startIndex).Take(current.PageSize).ToList());// LTEItems.FirstOrDefault().Value.Skip(startIndex).Take(current.PageSize).ToList());

                    //});
                    //ValidateRowErrors(LTEItems.FirstOrDefault().Value);
                    //CheckErrors(LTEItems.FirstOrDefault().Value);

                    ////LTEItems.FirstOrDefault().Value.Where(a => a.Errors.Count() > 0).Each(i =>
                    ////{
                    ////    current.InValidRows.Add(i);
                    ////});

                    ////LTEItems.FirstOrDefault().Value.Where(a => a.Errors.Count() == 0).Each(i => {
                    ////    current.Rows.Add(i);
                    ////});
                    //current.Rows = LTEItems.FirstOrDefault().Value.Skip(startRow).Take(current.PageSize).ToList();

                    //current.Rows = uploadedFiles.ToList().Skip(startRow).Take(current.PageSize).ToList();

                }
               

               //var results = wb.Worksheets[0].Cells.ExportTypeArray.ExportArray(startRow,0,rows,cols);
               //JsonSerializerSettings settings = new JsonSerializerSettings();
              
               
               // redisDT.SetEntryInHash<string>(dtHash, "dtdwain", results.ToJson());
                // DataTable dt = wb.Worksheets[0].Cells.ExportDataTableAsString(startRow, 0, current.PageSize == 0 ? rows : current.PageSize, cols);
             //  redisDT.SetEntryInHash<string>(dtHash, "Dtdwain", JsonConvert.SerializeObject(results, typeof(UploadedFile), Formatting.Indented, settings));
               // source = dt.AsEnumerable();

               // List<UploadedFile> uploadResults = new List<UploadedFile>();


                //Parallel.ForEach(dt.AsEnumerable(), new ParallelOptions() { MaxDegreeOfParallelism = 2 }, entry =>
                //{
                //    UploadedFile uf = new UploadedFile();
                //    for (int i = 0; i < cols; i++)
                //    {
                //        value = entry[i].ToString();
                //        switch (i)
                //        {

                //            case 0:
                //                uf.LicenseNo = value;
                //                uf.LicenseIsValid = true;
                //                break;
                //            case 1:
                //                uf.State = value;
                //                uf.StateIsValid = true;
                //                break;
                //            case 2:
                //                uf.FirstName = value;
                //                uf.FirstNameValid = true;
                //                break;
                //            case 3:
                //                uf.LastName = value;
                //                uf.LastNameValid = true;

                //                break;
                //        }


                //    }
                //    if (uploadResults.Where(u => u.LicenseNo == uf.LicenseNo && u.State == uf.State).Any())
                //        uf.duplicateRow = true;

                //    uploadResults.Add(uf);


                //    var results = validator.Validate(uf);


                //    uf.Errors = results.Errors as List<FluentValidation.Results.ValidationFailure>;



                //});
                //ValidateRows(uploadResults);

                //current.Rows = uploadResults.ToList();
            }
            else
            {
                var LTEItems = LTEHash.GetAll(); 
                
            
                string myKey = "LTEHash" + (current.PageNumber - 1).ToString();
               // var myList = LTEItems.Where(t => t.Key == myKey).FirstOrDefault().Value;
               

                ValidateRowErrors(current.Rows);
                CheckErrors(current.Rows);
                redisDT.SetEntryInHash<string>(LTEHash, myKey, current.Rows);
                current.Rows = current.Rows;
            }

          //  redisUsers.SetEntryInHash<string>(hash, "dwain",uploadResults.ToList());

            return current;
        }

        private int DetermineListType(string p)
        {
            int val = 1;
            switch (p)
            {
                case "Pre":
                    val= 1;
                    break;
                case "Post":
                   val =  2;
                    break;
                case "Watch":
                    val = 3;
                    break;
                default:
                    val = 0;
                    break;

            }
            return val;
        }

        private static void ValidateRows(ConcurrentBag<UploadedFile> uploadResults)
        {
            Parallel.ForEach(uploadResults.ToList(), res =>
            {
                lock (res)
                {
                    Parallel.ForEach(res.Errors, err =>
                    {
                        lock (err)
                        {
                            switch (err.PropertyName)
                            {

                                case "LastName":
                                    res.LastNameValid = false;
                                    break;
                                case "FirstName":
                                    res.FirstNameValid = false;
                                    break;
                                case "LicenseNo":
                                    res.LicenseIsValid = false;
                                    break;
                                case "State":
                                    res.StateIsValid = false;
                                    break;
                                case "Zip":
                                    res.ZipIsValid = false;
                                    break;
                                case "Street":
                                    res.StreetIsValid = false;
                                    break;
                                case "ListType":
                                    res.ListTypeIsValid = false;
                                    break;
                            }
                        }
                    });

                }

            });
        }

        private static void ValidateRowErrors(List<UploadedFile> uploadResults)
        {

            var validator = new BatchUploadValidator();
            Parallel.ForEach(uploadResults.ToList(), res =>
            {
                lock (res)
                {
          
                   var results = validator.Validate(res);
                    res.Errors = results.Errors as List<FluentValidation.Results.ValidationFailure>;
        

                }

            });


        }

         private static void CheckErrors(List<UploadedFile> rows)
         {
             Parallel.ForEach(rows, row =>
             {

                 lock (row)
                 {
                     row.FirstNameValid = true;
                     row.LastNameValid = true;
                     row.LicenseIsValid = true;
                     row.StateIsValid = true;
                     row.StreetIsValid = true;
                     row.ZipIsValid = true;
                     row.ListTypeIsValid = true;
                     Parallel.ForEach(row.Errors, err =>
                     {
                         lock (err)
                         {
                             switch (err.PropertyName)
                             {

                                 case "LastName":
                                     row.LastNameValid = false;
                                     break;
                                 case "FirstName":
                                     row.FirstNameValid = false;
                                     break;
                                 case "LicenseNo":
                                     row.LicenseIsValid = false;
                                     break;
                                 case "State":
                                     row.StateIsValid = false;
                                     break;
                                 case "ListType":
                                     row.ListTypeIsValid = false;
                                     break;
                             }
                         }
                     });

                 }
             });

         }
    }


}