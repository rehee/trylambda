using Services;
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

        public static Func<int, IContent> MyGetContentByid { get; set; }
        public static Func<ContentTypeKey, IEnumerable<IContent>> MyGetContentByType { get; set; }

        public static void InitMyContentGet(IContentService service, IContentTypeService typeService)
        {
            MyGetContentByid = id => GetFunc.GetContentByKey<int, IContent, CacheIContent>(id, true, service.GetById, MyCache);
            MyGetContentByType = type =>
            {
                Func<IContent, bool> where = b => b!=null && b.ContentType.Alias.Equals(type.ToString(), StringComparison.InvariantCultureIgnoreCase);
                Func<Func<IContent, bool>, IEnumerable<IContent>> getContentFromService = whereFunc =>
                 {
                     var typeId = typeService.GetContentType(type.ToString()).Id;
                     return service.GetContentOfContentType(typeId);
                 };
                Func<Func<IContent, bool>, IDictionary<int, CacheIContent>, IEnumerable<CacheIContent>> getFromCache = (whereF,cache) =>
                {
                    var c = new Dictionary<int, CacheIContent>(cache);
                    var list =  c.Select(b=>b.Value).ToList();
                    return list.Where(b => whereF(((CacheIContent)b).Content)).ToList();
                };
                return GetFunc.GetChildContentsByQuery<int,IContent,CacheIContent>(getContentFromService, getFromCache,b=>b.Id,where,true,60,MyCache);
            };
        }
    }
}
