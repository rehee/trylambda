using Services;
using Services.Models.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestLambda.Models;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace System
{
    public static partial class E
    {
        public static Dictionary<int, CacheIContent> MyCache { get; set; } = new Dictionary<int, CacheIContent>();
        public static Dictionary<string, CacheQuery> MyQuerys { get; set; } = new Dictionary<string, CacheQuery>();
        public static Func<string,bool> MyQueryIsNew =
        query =>
        {
            if (MyQuerys.ContainsKey(query))
            {
                var isexpire = ((CacheBase)MyQuerys[query]).IsExpire(30);
                if (isexpire)
                    MyQuerys[query].Time = DateTime.Now;
                return isexpire;
            }
            else
            {
                MyQuerys.Add(query, new CacheQuery());
                return true;
            }
        };

        public static Func<int, IEnumerable<IContent>> GetChildContentsByRootIdFromService(Func<int, IEnumerable<IContent>> serviceFunction)
        {
            return rootId => serviceFunction(rootId);
        }
        public static Func<int, IDictionary<int, CacheIContent>, IEnumerable<CacheIContent>> GetChildContentsByRootIdFromCache()
        {
            return (id, cache) =>
            {
                var copy = new Dictionary<int, CacheIContent>(cache);
                return copy.Where(b => b.Value.Content.ParentId == id).Select(b => b.Value);
            };
        }

        public static Func<IEnumerable<IContent>> MyGetIContentsByTypeFromService(ContentTypeKey key, IContentService service, IContentTypeService typeService)
        {
            var type = typeService.GetContentType(key.ToString());
            return () =>
            {
                return service.GetContentOfContentType(type.Id);
            };
        }
        public static Func<IDictionary<int, CacheIContent>, IEnumerable<CacheIContent>> MyGetContentByTypeFromCache(ContentTypeKey key)
        {
            return (cacheInput) =>
            {
                var cache = new Dictionary<int, CacheIContent>(cacheInput);
                return cache
                    .Where(b => b.Value.Content.ContentType.Alias.Equals(key.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    .Select(b => b.Value);
            };
        }
        
        public static Func<IEnumerable<int>, IEnumerable<IContent>> GetContentsByIdsFromService(IContentService service)
        {
            return ids =>
            {
                return service.GetByIds(ids);
            };
        }
        public static Func<IEnumerable<int>, IDictionary<int, CacheIContent>, IEnumerable<CacheIContent>> GetContentsByIdsFromCache()
        {
            return (ids, cacheInput) =>
            {
                var cache = new Dictionary<int, CacheIContent>(cacheInput);
                return cache.Where(b => ids.Contains(b.Key)).Select(b => b.Value);
            };
        }

        public static Func<int, IContent> MyGetContentByid { get; set; }
        public static Func<IEnumerable<int>,bool?,string, IEnumerable<IContent>> MyGetContentsByIds { get; set; }
        public static Func<ContentTypeKey, Func<IContent, bool>,bool?,string, IEnumerable<IContent>> MyGetIContentsByType { get; set; }
        public static Func<int, Func<IContent, bool>, bool?, string, IEnumerable<IContent>> MyGetChildByRootId { get; set; }


        public static void InitMyContentGet(IContentService service, IContentTypeService typeService)
        {
            MyGetContentByid = id => {
                return GetFunc.GetContentByKey<int, IContent, CacheIContent>(id, true, service.GetById, MyCache);
            };
            MyGetContentsByIds = (ids, alive, queryName) =>
            {
                return GetFunc.GetContentByKeys<int, IContent, CacheIContent>(ids, alive, 
                    GetContentsByIdsFromService(service), 
                    GetContentsByIdsFromCache(), MyCache, b => b.Id, 30, MyQueryIsNew(queryName));
            };
            MyGetIContentsByType = (type, where,alive,queryName) =>
            {
                return GetFunc.GetChildContentsByQuery<int, IContent, CacheIContent>(
                    MyGetIContentsByTypeFromService(type, service, typeService),
                    MyGetContentByTypeFromCache(type), b => b.Id, MyCache, where, alive, 30, MyQueryIsNew(queryName));
            };
            MyGetChildByRootId = (rootId, where, alive,queryName) =>
            {
                return GetFunc.GetChildContentsByKey<int, IContent, CacheIContent>(
                    GetChildContentsByRootIdFromService(service.GetChildren),
                    GetChildContentsByRootIdFromCache(), b => b.Id, MyCache, rootId, alive, 30,where, MyQueryIsNew(queryName));
            };
        }
    }
}
