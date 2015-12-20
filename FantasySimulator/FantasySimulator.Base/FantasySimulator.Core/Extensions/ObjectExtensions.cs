using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FantasySimulator.Interfaces;

namespace FantasySimulator.Core
{
    public static class ObjectExtensions
    {
        public static T SafeConvert<T>(this object value)
        {
            var result = SafeConvert<T>(value, default(T));
            return result;
        }


        public static T SafeConvert<T>(this object value, T defaultValue)
        {
            bool success;
            var result = SafeConvert<T>(value, defaultValue, out success);
            return result;
        }

        public static T SafeConvert<T>(this object value, T defaultValue, out bool success)
        {
            success = true;
            var result = defaultValue;
            try
            {
                if (value == null)
                    return result;
                //if (value == DBNull.Value)
                //    return result;
                var type = value.GetType();
                if (typeof (T) == type)
                {
                    result = (T) value;
                }
                else if (typeof(T).GetTypeInfo().IsEnum)
                {
                    var str = value.SafeConvert<string>();
                    if (type == typeof (string))
                    {
                        var values = Enum.GetValues(typeof(T)).Cast<T>().Select(x => x.ToString()).ToList();
                        var index = values.FindIndex(x => string.Equals(x, str, StringComparison.OrdinalIgnoreCase));
                        if (index >= 0)
                            str = values.ElementAt(index);
                    }
                    var obj = Enum.Parse(typeof(T), str);
                    result = (T)obj;
                }
                //else if (typeof (IConvertible).IsAssignableFrom(typeof (T)))
                else if (TypeWrapper.IsAssignableFrom(typeof (IConvertible), typeof (T)))
                {
                    if (type == typeof (string))
                    {
                        var str = value.SafeConvert<string>() ?? "";
                        var isNumeric = str.Length > 0 && str.All(c => char.IsDigit(c) || char.IsPunctuation(c) || c == ',' || c == '+');
                        if (isNumeric)
                            value = str.Replace(',', '.');
                    }
                    var obj = Convert.ChangeType(value, typeof (T), CultureInfo.InvariantCulture);
                    result = (T) obj;
                }
                else
                {
                    result = (T) value;
                }
                return result;
            }
            catch (Exception ex)
            {
                success = false;
                return result;
            }
        }


        public static object SafeConvertDynamic(this object value, TypeWrapper targetType)
        {
            object result = null;
            try
            {
                var defaultValue = targetType.Type.IsInterface
                    ? null
                    : Activator.CreateInstance(targetType.Type);
                result = SafeConvertDynamic(value, targetType.Type, defaultValue);
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

        public static object SafeConvertDynamic(this object value, TypeWrapper targetType, object defaultValue)
        {
            var result = defaultValue;
            try
            {
                if (value == null)
                    return result;
                //if (value == DBNull.Value)
                //    return result;
                var type = value.GetType();
                if (targetType.Type == type)
                {
                    result = value;
                }
                else if (TypeWrapper.IsAssignableFrom(targetType.Type, type))
                {
                    result = value;
                }
                else if (targetType.Type.GetTypeInfo().IsEnum)
                {
                    var str = value.SafeConvert<string>();
                    if (type == typeof (string))
                    {
                        var values = Enum.GetValues(targetType.Type).Cast<Enum>().Select(x => x.ToString()).ToList();
                        var index = values.FindIndex(x => string.Equals(x, str, StringComparison.OrdinalIgnoreCase));
                        if (index >= 0)
                            str = values.ElementAt(index);
                    }
                    result = Enum.Parse(targetType.Type, str);
                }
                //else if (typeof (IConvertible).IsAssignableFrom(targetType))
                else if (TypeWrapper.IsAssignableFrom(typeof (IConvertible), targetType.Type))
                {
                    if (type == typeof (string))
                    {
                        var str = value.SafeConvert<string>() ?? "";
                        var isNumeric = str.Length > 0 && str.All(c => char.IsDigit(c) || char.IsPunctuation(c) || c == ',' || c == '+');
                        if (isNumeric)
                            value = str.Replace(',', '.');
                    }
                    result = Convert.ChangeType(value, targetType.Type);
                }
                else
                {
                    //result = (T) value;       // todo?
                    throw new NotImplementedException();
                }
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
        }

    }
}
