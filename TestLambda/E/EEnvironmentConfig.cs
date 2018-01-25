using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    public static partial class E
    {
        public static Dictionary<EnvironmentKey, dynamic> Config { get; set; } = new Dictionary<EnvironmentKey, dynamic>();


        public static T GetConfigByKey<T>(this EnvironmentKey key, Func<T> getT, Dictionary<EnvironmentKey, dynamic> config = null)
        {
            if (config == null)
                config = Config;
            if (!config.ContainsKey(key))
                config.Add(key, getT());
            if (config[key] == null)
                config[key] = getT();
            try
            {
                return config[key];
            }
            catch { }
            return default(T);
        }

        public static string GetAbsUrl(Uri uri)
        {
            try
            {
                return $"{uri.Scheme}://{uri.Authority}/";
            }
            catch
            {
                return uri.AbsoluteUri;
            }

        }
        public static string DateTimeToString(this DateTime? time, string empty = "", string format = "yyyy-MM-dd")
        {
            if (time == null)
                return empty;
            return ((DateTime)time).ToString(format);
        }

        public static string NumberToString(this decimal input, string empty = "面议")
        {
            if (input <= 0)
                return empty;
            return $"¥ {input}";
        }
        public static string NumberToString(this int input, string empty = "面议")
        {
            return NumberToString((decimal)input, empty);
        }
    }

    public enum EnvironmentKey
    {
        HomeId = 1,
        UmbracoHelper = 2,
        Email = 3,
    }
}