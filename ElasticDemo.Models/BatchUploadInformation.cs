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
        public string Action { get; set; }
        public List<UploadedFile> Rows { get; set; }
        public List<UploadedFile> InValidRows { get; set; }
    }
}
