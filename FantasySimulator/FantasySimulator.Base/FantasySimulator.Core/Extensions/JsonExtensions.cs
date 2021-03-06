﻿using Newtonsoft.Json.Linq;

namespace FantasySimulator.Core
{
    public static class JsonExtensions
    {
        public static TObj ToObjectOrDefault<TObj>(this JToken obj)
        {
            var res = obj.ToObjectOrDefault<TObj>(default(TObj));
            return res;
        }

        public static TObj ToObjectOrDefault<TObj>(this JToken obj, TObj defaultValue)
        {
            if (obj == null)
                return defaultValue;
            if (obj.Type == JTokenType.Null)
                return defaultValue;
            var res = obj.ToObject<TObj>();
            return res;
        }


        public static TValue GetPropertyValue<TValue>(this JObject obj, string propertyName)
        {
            var result = GetPropertyValue<TValue>(obj, propertyName, default(TValue));
            return result;
        }

        public static TValue GetPropertyValue<TValue>(this JObject obj, string propertyName, TValue defaultValue)
        {
            if (obj == null)
                return defaultValue;
            var prop = obj.Property(propertyName);
            if (prop == null)
                return defaultValue;
            var res = prop.Value.ToObjectOrDefault<TValue>(defaultValue);
            return res;
        }

    }
}
