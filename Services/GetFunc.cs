using Services.Models.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class GetFunc
    {
        public static K GetContentByKey<T, K, C>(
            T key, bool? alive,
            Func<T, K> service,
            IDictionary<T, C> cache,
            Func<T, K, bool?, IDictionary<T, C>, K> updateCacheFunction = null
            ) where C : CacheBase, new()
        {
            if (updateCacheFunction == null)
                updateCacheFunction = DefaultUpdateCache;
            K content;
            if (cache.ContainsKey(key))
            {
                content = (K)cache[key].Content;
            }
            else
            {
                content = service(key);
            }
            return updateCacheFunction(key, content, alive, cache);
        }

        public static IEnumerable<K> GetChildContentsByKey<T, K, C>(
            Func<T, IEnumerable<K>> service,
            Func<T, IDictionary<T, C>, IEnumerable<C>> getFromCache,
            Func<K, T> getKeys,
            IDictionary<T, C> cacheInput,
            T key,
            bool? alive,
            int expireMin,
            Func<K, bool> where = null,
            Func<T, K, bool?, IDictionary<T, C>, K> updateCacheFunction = null
            ) where C : CacheBase, new()
        {
            IEnumerable<K> result;
            if (updateCacheFunction == null)
                updateCacheFunction = DefaultUpdateCache;
            IDictionary<T, C> cache = new Dictionary<T, C>(cacheInput);
            var resultIncache = getFromCache(key, cache);
            if (where != null)
                resultIncache.Where(b => where((K)b.Content));
            if (resultIncache.IsExpire<C>(expireMin))
                goto GetFromService;

            result = resultIncache.Select(b => (K)b.Content);

            goto UpdateCache;

            GetFromService:
            result = service(key);
            if (where != null)
                result = result.Where(where);

            UpdateCache:
            foreach (var r in result)
            {
                updateCacheFunction(getKeys(r), r, alive, cacheInput);
            }
            return result;
        }
        public static IEnumerable<K> GetChildContentsByQuery<T, K, C>(
            Func<Func<K, bool>, IEnumerable<K>> service,
            Func<Func<K, bool>, IDictionary<T, C>, IEnumerable<C>> getFromCache,
            Func<K, T> getKeys,
            Func<K, bool> where,
            bool? alive,
            int expireMin,
            IDictionary<T, C> cacheInput,
            Func<T, K, bool?, IDictionary<T, C>, K> updateCacheFunction = null
            ) where C : CacheBase, new()
        {
            if (updateCacheFunction == null)
                updateCacheFunction = DefaultUpdateCache;
            var cache = new Dictionary<T, C>(cacheInput);
            IEnumerable<K> result;
            var resultFromCache = getFromCache(where, cache);
            if (resultFromCache.IsExpire<C>(expireMin))
                goto GetFromServer;

            result = resultFromCache.Select(b => b.GetContent<K>());
            goto UpdateCache;

            GetFromServer:
            result = service(where);

            UpdateCache:
            foreach (var item in result)
            {
                updateCacheFunction(getKeys(item), item, alive, cacheInput);
            }
            return result;
        }
        public static K DefaultUpdateCache<T, K, C>(T key, K value, bool? alive, IDictionary<T, C> cache)
            where C : CacheBase, new()
        {
            if (value == null)
                goto RemoveCache;
            if (cache.ContainsKey(key))
            {
                cache[key].Time = DateTime.Now;
                cache[key].Content = value;
                if (alive != null)
                    cache[key].KeepAlive = (bool)alive;
            }
            else
            {
                var v = new C();
                v.Set(value, alive);
                cache.Add(key, v);
            }
            return value;

            RemoveCache:
            if (cache.ContainsKey(key))
            {
                cache.Remove(key);
            }
            return value;
        }


    }
}
