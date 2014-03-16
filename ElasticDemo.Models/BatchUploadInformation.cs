using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticDemo.Models
{
    public class BatchUploadInformation
    {
        public string filePath { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int SelectedInstance { get; set; }
        public int SelectedList { get; set; }
        public string Action { get; set; }
        public string ListName { get; set; }

        public List<Picker> Instances { get; set; }
        public List<Picker> ListTypes { get; set; }
        public List<Picker> CurrentLists { get; set; }
        public List<Picker> Customers { get; set; }
        public List<UploadedFile> Rows { get; set; }
        public List<UploadedFile> InValidRows { get; set; }
    }

    public class Picker
    {
        public string Description { get; set; }
        public int Id { get; set; }
    }
}
