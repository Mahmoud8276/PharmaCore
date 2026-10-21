using System;
using System.ComponentModel.DataAnnotations;

namespace Pharmacy.System.Core.Attributes
{
    public class NotInFutureAttribute : ValidationAttribute
    {
        public NotInFutureAttribute(): base() {}

        public override bool IsValid(object? value)
        {
            if (value is null)
                return true; // Use [Required] if null isn't allowed.

            if (value is DateTime dateTime)
                return dateTime <= DateTime.Now;

            return false;
        }
    }
}
