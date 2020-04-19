using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace prj3beer.Models
{
    class ValidationHelper
    {
        public static IList<ValidationResult> Validate(object model)
        {
            var results = new List<ValidationResult>();

            var vc = new ValidationContext(model, null, null);

            Validator.TryValidateObject(model, vc, results, true);

            if (model is IValidatableObject) (model as IValidatableObject).Validate(vc);

            return results;
        }
    }
}