using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CodeWF.Tools.Extensions;

public static class IConvertibleExtensions
{
    public static bool IsNumeric(this Type type)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// 类型直转
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T ConvertTo<T>(this IConvertible value) where T : IConvertible
    {
        if (value != null)
        {
            var type = typeof(T);
            if (value.GetType() == type)
            {
                return (T)value;
            }

            if (type.IsEnum)
            {
                return (T)Enum.Parse(type, value.ToString(CultureInfo.InvariantCulture));
            }

            if (value == DBNull.Value)
            {
                return default;
            }

            if (type.IsNumeric())
            {
                return (T)value.ToType(type, new NumberFormatInfo());
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                return (T)(underlyingType!.IsEnum
                    ? Enum.Parse(underlyingType, value.ToString(CultureInfo.CurrentCulture))
                    : Convert.ChangeType(value, underlyingType));
            }

            var converter = TypeDescriptor.GetConverter(value);
            if (converter.CanConvertTo(type))
            {
                return (T)converter.ConvertTo(value, type);
            }

            converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(value.GetType()))
            {
                return (T)converter.ConvertFrom(value);
            }

            return (T)Convert.ChangeType(value, type);
        }

        return (T)value;
    }
}