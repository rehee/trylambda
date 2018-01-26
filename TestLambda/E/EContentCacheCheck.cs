using Services.Models.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestLambda.Models;


namespace System
{
    public static partial class E
    {
        public static void CheckMyCache<T,K>(Dictionary<T,K> cache, int expireMins) where K: CacheBase
        {
            var keys = cache.Keys.ToList();
            keys.ForEach(
                b =>
                {
                    if (cache[b].KeepAlive != true && (DateTime.Now - cache[b].Time).TotalMinutes > expireMins)
                        cache.Remove(b);
                });
        }

        public static Action RefreshAction(dynamic cache, int expireMins)
        {
            return () => CheckMyCache(cache, expireMins);
        }

        public static List<Action> InitCheckFuncList(List<dynamic> input, int expireMins)
        {
            return input.Select(b=> (Action)RefreshAction(b, expireMins)).ToList();
        }
    }
}