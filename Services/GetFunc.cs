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
                content = cache[key].GetContent<K>();
            else
                content = service(key);
            return updateCacheFunction(key, content, alive, cache);
        }

        public static IEnumerable<K> GetContentByKeys<T, K, C>(
            IEnumerable<T> keys, bool? alive, Func<IEnumerable<T>, IEnumerable<K>> serviceFunction, Func<IEnumerable<T>, IDictionary<T, C>, IEnumerable<C>> cacheFunction,
            IDictionary<T, C> cacheInput, Func<K, T> getKey, int expireMin = 30, bool isNewQuery = false, Func<T, K, bool?, IDictionary<T, C>, K> updateCacheFunction = null
            ) where C : CacheBase, new()
        {
            if (updateCacheFunction == null)
                updateCacheFunction = DefaultUpdateCache;
            IEnumerable<K> content;
            if (isNewQuery)
                goto NewQuery;
            var cache = new Dictionary<T, C>(cacheInput);
            var cacheResult = cacheFunction(keys, cache);
            if (cacheResult.IsExpire(expireMin))
                goto NewQuery;
            content = cacheResult.Select(b => b.GetContent<K>());
            goto UpdateCache;
            NewQuery:
            content = serviceFunction(keys);
            UpdateCache:
            var result = content.ToList();
            result.ForEach(r => updateCacheFunction(getKey(r), r, alive, cacheInput));
            return result;
        }

        public static IEnumerable<K> GetChildContentsByKey<T, K, C>(
            Func<T, IEnumerable<K>> service,
            Func<T, IDictionary<T, C>, IEnumerable<C>> getFromCache,
            Func<K, T> getKeys,
            IDictionary<T, C> cacheInput,
            T key,
            bool? alive,
            int expireMin,
            Func<K, bool> where = null, bool newQuery = false,
            Func<T, K, bool?, IDictionary<T, C>, K> updateCacheFunction = null
            ) where C : CacheBase, new()
        {
            IEnumerable<K> result;
            if (updateCacheFunction == null)
                updateCacheFunction = DefaultUpdateCache;
            if (newQuery)
                goto GetFromService;
            IDictionary<T, C> cache = new Dictionary<T, C>(cacheInput);
            var resultIncache = getFromCache(key, cache);
            if (where != null)
                resultIncache.Where(b => where(b.GetContent<K>()));
            if (resultIncache.IsExpire(expireMin))
                goto GetFromService;

            result = resultIncache.Select(b => b.GetContent<K>());

            goto UpdateCache;

            GetFromService:
            result = service(key);
            if (where != null)
                result = result.Where(where);

            UpdateCache:
            result.ToList().ForEach(r => updateCacheFunction(getKeys(r), r, alive, cacheInput));
            return result;
        }
        public static IEnumerable<K> GetChildContentsByQuery<T, K, C>(
            Func<IEnumerable<K>> getFromService,
            Func<IDictionary<T, C>, IEnumerable<C>> getFromCache,
            Func<K, T> getKeys,
            IDictionary<T, C> cacheInput,
            Func<K, bool> where = null,
            bool? alive = null,
            int expireMin = 30, bool newQuery = false,
            Func<T, K, bool?, IDictionary<T, C>, K> updateCacheFunction = null
            ) where C : CacheBase, new()
        {
            if (updateCacheFunction == null)
                updateCacheFunction = DefaultUpdateCache;
            var cache = new Dictionary<T, C>(cacheInput);
            IEnumerable<K> result;
            if (newQuery)
                goto GetFromServer;
            var resultFromCache = getFromCache(cache);
            if (where != null)
                resultFromCache = resultFromCache.Where(b => where(b.GetContent<K>()));
            if (resultFromCache.IsExpire(expireMin))
                goto GetFromServer;

            result = resultFromCache.Select(b => b.GetContent<K>());
            goto UpdateCache;

            GetFromServer:
            result = getFromService();
            if (where != null)
                result = result.Where(where);
            UpdateCache:

            result.ToList().ForEach(r => updateCacheFunction(getKeys(r), r, alive, cacheInput));

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
