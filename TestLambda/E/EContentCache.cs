using Spxus.Umbraco.Extend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Services;

namespace System
{
    public static partial class E
    {
        public static Dictionary<int, ContentCacheAndTime> PageCache
        {
            get
            {
                if (UmbracoContentExtend.PageCache == null)
                    UmbracoContentExtend.PageCache = new Dictionary<int, ContentCacheAndTime>();
                return UmbracoContentExtend.PageCache;
            }
        }
        public static Dictionary<int, ContentCacheAndTime> CopyDictionary(this Dictionary<int, ContentCacheAndTime> input)
        {
            return new Dictionary<int, ContentCacheAndTime>(input);
        }


        
        public static void InitCacheSetting()
        {
            //默认的傻大黑粗的缓存时间与间隔(分钟)
            //ContentCacheAndTime.CacheTimeMinute
            //ContentCacheAndTime.CacheCheckMaxTime
        }

    }
}