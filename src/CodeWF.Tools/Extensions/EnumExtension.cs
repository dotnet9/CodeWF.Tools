using CodeWF.Tools.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CodeWF.Tools.Extensions;

public static class EnumExtension
{
    private static readonly ConcurrentDictionary<Type, Dictionary<int, string>> EnumNameValueDict = new();

    /// <summary>
    /// 获取枚举对象Key与显示名称的字典
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    public static Dictionary<int, string> GetDictionary(this Type enumType)
    {
        if (!enumType.IsEnum)
        {
            throw new Exception("给定的类型不是枚举类型");
        }

        return EnumNameValueDict.GetOrAdd(enumType, _ => GetDictionaryItems(enumType));
    }

    private static Dictionary<int, string> GetDictionaryItems(Type enumType)
    {
        var enumItems = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        var names = new Dictionary<int, string>(enumItems.Length);
        foreach (var enumItem in enumItems)
        {
            names[(int)enumItem.GetValue(enumType)] = enumItem.Name;
        }

        return names;
    }

    /// <summary>
    /// 根据枚举成员获取Display的属性Name
    /// </summary>
    /// <returns></returns>
    public static string GetDisplay(this Enum en)
    {
        var type = en.GetType(); //获取类型
        var memberInfos = type.GetMember(en.ToString()); //获取成员
        if (memberInfos.Any() && memberInfos[0].GetCustomAttributes(typeof(DisplayAttribute), false) is DisplayAttribute
                []
                {
                    Length: > 0
                } attrs)
        {
            return attrs[0].Name; //返回当前描述
        }

        return en.ToString();
    }

    /// <summary>
    /// 获取枚举值的Description信息，多位域会排除0值
    /// </summary>
    /// <param name ="value">枚举值</param>
    /// <param name ="args">要格式化的对象</param>
    /// <returns>如果未找到DescriptionAttribute则返回null或返回类型描述</returns>
    public static string GetDescription(this Enum value, params object[] args)
    {
        var enumType = value.GetType();

        // 1、检查是否是位域枚举的组合值  
        var isFlagsEnum = enumType.GetCustomAttribute<FlagsAttribute>() != null;

        // 2、非位域枚举直接返回描述
        if (!isFlagsEnum) return GetDescriptionPrivate(value, args);

        // 3、位域枚举获取每个标志的描述并用逗号分隔  
        var descriptions = new List<string>();
        foreach (Enum enumValue in Enum.GetValues(enumType))
        {
            // 跳过值为0的枚举成员，因为任何数与0进行“或”运算都不会改变该数的值
            if (Convert.ToInt64(enumValue) == 0) continue;

            if (value.HasFlag(enumValue)) descriptions.Add(GetDescriptionPrivate(enumValue));
        }

        return descriptions.Count <= 0 ? GetDescriptionPrivate(value) : string.Join(",", descriptions);
    }

    /// <summary>
    /// 获取枚举值的Description信息
    /// </summary>
    /// <param name ="value">枚举值</param>
    /// <param name ="args">要格式化的对象</param>
    /// <returns>如果未找到DescriptionAttribute则返回null或返回类型描述</returns>
    public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Enum value) where TAttribute : Attribute
    {
        var type = value.GetType();
        if (!Enum.IsDefined(type, value))
        {
            return Enum.GetValues(type).OfType<Enum>().Where(value.HasFlag).SelectMany(e =>
                type.GetField(e.ToString()).GetCustomAttributes<TAttribute>(false));
        }

        return type.GetField(value.ToString()).GetCustomAttributes<TAttribute>(false);
    }

    /// <summary>
    /// 拆分枚举值
    /// </summary>
    /// <param name ="value">枚举值</param>
    public static IEnumerable<TEnum> Split<TEnum>(this TEnum value) where TEnum : Enum
    {
        var type = typeof(TEnum);
        return Enum.IsDefined(type, value)
            ? new[]
            {
                value
            }
            : Enum.GetValues(type).Cast<TEnum>().Where(e => value.HasFlag(e));
    }

    /// <summary>
    /// 获取枚举值的Description信息
    /// </summary>
    /// <param name ="value">枚举值</param>
    /// <returns>如果未找到DescriptionAttribute则返回null或返回类型描述</returns>
    public static EnumDescriptionAttribute GetEnumDescription(this Enum value)
    {
        return GetEnumDescriptions(value).FirstOrDefault();
    }

    /// <summary>
    /// 获取枚举值的Description信息
    /// </summary>
    /// <param name ="value">枚举值</param>
    /// <returns>如果未找到DescriptionAttribute则返回null或返回类型描述</returns>
    public static IEnumerable<EnumDescriptionAttribute> GetEnumDescriptions(this Enum value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var type = value.GetType();
        if (!Enum.IsDefined(type, value))
        {
            return Enum.GetValues(type).OfType<Enum>().Where(value.HasFlag).SelectMany(e =>
                type.GetField(e.ToString()).GetCustomAttributes(typeof(EnumDescriptionAttribute), false)
                    .OfType<EnumDescriptionAttribute>());
        }

        return type.GetField(value.ToString()).GetCustomAttributes(typeof(EnumDescriptionAttribute), false)
            .OfType<EnumDescriptionAttribute>();
    }

    /// <summary>
    /// 扩展方法：根据枚举值得到相应的枚举定义字符串
    /// </summary>
    /// <param name="value"></param>
    /// <param name="enumType"></param>
    /// <returns></returns>
    public static string ToEnumString(this int value, Type enumType)
    {
        return GetEnumStringFromEnumValue(enumType)[value.ToString()];
    }

    /// <summary>
    /// 根据枚举类型得到其所有的 值 与 枚举定义字符串 的集合
    /// </summary>
    /// <param name="enumType"></param>
    /// <returns></returns>
    public static NameValueCollection GetEnumStringFromEnumValue(Type enumType)
    {
        var nvc = new NameValueCollection();
        var fields = enumType.GetFields();
        foreach (var field in fields)
        {
            if (!field.FieldType.IsEnum) continue;

            var strValue = ((int)enumType.InvokeMember(field.Name, BindingFlags.GetField, null, null, null)).ToString();
            nvc.Add(strValue, field.Name);
        }

        return nvc;
    }

    /// <summary>
    /// 根据枚举成员获取自定义属性EnumDisplayNameAttribute的属性DisplayName
    /// </summary>
    public static Dictionary<string, int> GetDescriptionAndValue(this Type enumType) => Enum.GetValues(enumType)
        .Cast<object>().ToDictionary(e => (e as Enum).GetDescription(), e => (int)e);

    private static string GetDescriptionPrivate(Enum value, params object[] args)
    {
        var fieldInfo = value.GetType().GetField(value.ToString());
        var attribute =
            Attribute.GetCustomAttribute(fieldInfo!, typeof(DescriptionAttribute)) as DescriptionAttribute;
        var description = attribute.Description;
        return args.Length > 0 ? string.Format(description, args) : description;
    }
}