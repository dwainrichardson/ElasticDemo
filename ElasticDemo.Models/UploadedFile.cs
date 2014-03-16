using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
namespace ElasticDemo.Models
{
    public class UploadedFile
    {
        public string filePath { get; set; }
        public string State { get; set; }
        public bool StateIsValid { get; set; }
        public string FirstName { get; set; }
        public bool FirstNameValid { get; set; }
        public string LastName { get; set; }
        public bool LastNameValid { get; set; }
        public string License { get; set; }
        public bool LicenseIsValid { get; set; }
        public string Street { get; set; }
        public bool StreetIsValid { get; set; }
        public string Zip { get; set; }
        public bool ZipIsValid { get; set; }
        public string StreetNo { get; set; }
        public string City { get; set; }
        public bool CityIsValid { get; set; }
        public string ListType { get; set; }
        public bool ListTypeIsValid { get; set; }

        public bool duplicateRow { get; set; }
        public List<FluentValidation.Results.ValidationFailure> Errors { get; set; }
    }
}
