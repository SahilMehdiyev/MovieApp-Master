using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Web.Validators
{
    public class BirthDateAttribute: ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime datetime = Convert.ToDateTime(value);
            return datetime <= DateTime.Now;
        }
    }
}
