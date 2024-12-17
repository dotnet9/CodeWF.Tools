using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CodeWF.Tools.Extensions;

public static class ValidationExtension
{
    public static bool TryValidateObject<T>(this T beValidInstance, out List<ValidationResult>? results) where T : class
    {
        var context = new ValidationContext(beValidInstance);
        var validResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(beValidInstance, context, validResults, true);
        if (isValid)
        {
            validResults = null;
            results = null;
            return true;
        }

        results = validResults;
        return false;
    }
}