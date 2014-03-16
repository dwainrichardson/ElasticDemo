using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using ElasticDemo.Models;

namespace ElasticDemo.Models
{
    public class CMSDocumentValidator : AbstractValidator<CMSDocument>
    {
        public CMSDocumentValidator()
        {
            RuleFor(r => r.AppInstID).NotNull();
            RuleFor(r => r.AssignDate).GreaterThan(DateTime.Now.AddYears(-20));
            RuleFor(r => r.OrderDate).GreaterThan(DateTime.Now.AddYears(-20));
            RuleFor(r => r.DocId).NotEmpty();
            RuleFor(r => r.FolderId).NotEmpty();
            RuleFor(r => r.PortID).NotEmpty();
            RuleFor(r => r.ServiceId).NotEmpty();
        }

    }


    public class BatchUploadValidator : AbstractValidator<UploadedFile>
    {
        public BatchUploadValidator()
        {
            RuleFor(b => b.FirstName).NotEmpty();
            RuleFor(b => b.License).NotEmpty();
            RuleFor(b => b.State).SetValidator(new StateMustExist());
            RuleFor(b => b.ListType).SetValidator(new ListTypeMustExist());
            RuleFor(b => b.LastName).NotEmpty();
            RuleFor(b => b.City).NotEmpty();
            RuleFor(b => b.Street).NotEmpty();
            RuleFor(b => b.Zip).NotEmpty();
        }
    }


    public class StateMustExist : PropertyValidator
    {
        public StateMustExist()
            : base("State {PropertyName} does not exist")
        {

        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            DictionaryStates ds = new DictionaryStates();
            var value= context.PropertyValue as string;
            if (string.Compare(ds.returnState(value), "No Match Found", true) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

    }

    public class ListTypeMustExist : PropertyValidator
    {
        public ListTypeMustExist()
            : base("List Type {ListType} does not exist")
        {

        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            List<string> validTypes = new List<string>();
            validTypes.Add("PRE");
            validTypes.Add("POST");
            validTypes.Add("WATCH");

            var value = context.PropertyValue as string;
            if(!string.IsNullOrEmpty(value)){
            if (validTypes.Contains(value.ToUpper()))
                return true;
            else return false;
            }else
        	{
                return false;
	        }
        }

    }
}
