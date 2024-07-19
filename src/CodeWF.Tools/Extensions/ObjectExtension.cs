using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace CodeWF.Tools.Extensions
{
    /// <summary>
    /// 对象扩展
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// 是否是基本数据类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(string))
            {
                return true;
            }

            return type.IsValueType && type.IsPrimitive;
        }

        /// <summary>
        /// 判断是否为null，null或0长度都返回true
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this T? value)
            where T : class
        {
            switch (value)
            {
                case null:
                    return true;
                case string s:
                    return string.IsNullOrWhiteSpace(s);
                case IEnumerable list:
                    return !list.GetEnumerator().MoveNext();
                default:
                    return false;
            }
        }

        /// <summary>
        /// 链式操作
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="source"></param>
        /// <param name="action"></param>
        public static T2 Next<T1, T2>(this T1 source, Func<T1, T2> action)
        {
            return action(source);
        }

        public static string GetDescription<T>()
        {
            var type = typeof(T);
            var descAttr = type.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return descAttr.Length <= 0 ? nameof(T) : ((DescriptionAttribute)descAttr.First()).Description;
        }
    }
}