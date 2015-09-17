using System;
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
            var result = defaultValue;
            try
            {
                if (value == null)
                    return result;
                //if (value == DBNull.Value)
                //    return result;
                if (typeof (T) == value.GetType())
                {
                    result = (T) value;
                }
                else if (typeof(T).GetTypeInfo().IsEnum)
                {
                    var str = value.SafeConvert<string>();
                    var obj = Enum.Parse(typeof(T), str);
                    result = (T)obj;
                }
                //else if (typeof (IConvertible).IsAssignableFrom(typeof (T)))
                else if (TypeWrapper.IsAssignableFrom(typeof (IConvertible), typeof (T)))
                {
                    var obj = Convert.ChangeType(value, typeof (T));
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
                return result;
            }
        }

        public static object SafeConvert(this object value, TypeWrapper targetType, object defaultValue)
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
                else if (targetType.Type.IsAssignableFrom(type))
                {
                    result = value;
                }
                else if (targetType.Type.GetTypeInfo().IsEnum)
                {
                    var str = value.SafeConvert<string>();
                    result = Enum.Parse(targetType, str);
                }
                //else if (typeof (IConvertible).IsAssignableFrom(targetType))
                else if (TypeWrapper.IsAssignableFrom(typeof (IConvertible), targetType))
                {
                    result = Convert.ChangeType(value, targetType);
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
