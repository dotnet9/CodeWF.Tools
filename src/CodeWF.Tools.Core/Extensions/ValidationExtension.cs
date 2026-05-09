using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace CodeWF.Tools.Extensions;

public static class ValidationExtension
{
    [RequiresUnreferencedCode("DataAnnotations 验证会通过反射读取模型和特性信息，Native AOT/裁剪发布请优先使用源生成或手写验证。")]
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
