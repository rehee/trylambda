using Spxus.Umbraco.Extend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace System
{
    public static partial class E
    {
        public static Func<int, IEnumerable<int>> GetBreadCrumbById { get; set; }
        public static Func<int, bool?, IContent> GetIContentById { get; set; }

        //常用的获取property


        public static GetContentValue<string> ContentValueString;
        public static GetContentValue<int> ContentValueInt;
        public static GetContentValue<decimal> ContentValueDecimal;
        public static GetContentValue<bool> ContentValueBool;
        public static GetContentValue<DateTime?> ContentValueDate;

        public static T ContentValue<T>(int contentId, string propertyName = "title", bool? keepAlive = null, Dictionary<Type, dynamic> contentValueFuncMap = null)
        {
            if (contentValueFuncMap == null)
            {
                contentValueFuncMap = ContentValueFuncMap;
            }
            if (contentValueFuncMap.ContainsKey(typeof(T)))
            {
                var value = contentValueFuncMap[typeof(T)](contentId, propertyName, keepAlive);
                if (typeof(T) == typeof(IContent))
                    return value;
                return (T)Convert.ChangeType(value, typeof(T));
            }
            else
            {
                return default(T);
            }
        }

        public static Dictionary<Type, dynamic> ContentValueFuncMap { get; set; }

        public static Func<IContent> GetHomePage { get; set; }
        public static Func<IEnumerable<int>, bool, IEnumerable<IContent>> GetIContentByIds { get; set; }
        public static Func<int, Func<IContent, bool>, bool, IEnumerable<IContent>> GetChildContentById { get; set; }
        public static GetAllContentByType AllContentByType;
        //umbraco helper
        public static UmbracoHelper Helper
        {
            get
            {
                return EnvironmentKey.UmbracoHelper.GetConfigByKey<UmbracoHelper>(GetHelper);
            }
        }
        public static UmbracoHelper GetHelper()
        {
            return new UmbracoHelper(UmbracoContext.Current);
        }

        public static void InitContentGet(IContentService service, IContentTypeService typeService)
        {
            AllContentByType = UmbracoContentExtend.GetAllContentTypeFunction(typeService, service);
            ContentValueString = UmbracoContentExtend.GetContentValue<string>(service);
            ContentValueInt = UmbracoContentExtend.GetContentValue<int>(service);
            ContentValueDecimal = UmbracoContentExtend.GetContentValue<decimal>(service);
            ContentValueBool = UmbracoContentExtend.GetContentValue<bool>(service);
            ContentValueDate = UmbracoContentExtend.GetContentValue<DateTime?>(service);
            GetIContentById = (int id, bool? alive) => UmbracoContentExtend.GetContent(service)(id, alive);
            Func<int, string, bool?, IContent> GetIContentFunc = (id, key, alive) => GetIContentById(id, alive);
            ContentValueFuncMap = new Dictionary<Type, dynamic>()
            {
                [typeof(string)] = ContentValueString,
                [typeof(int)] = ContentValueInt,
                [typeof(decimal)] = ContentValueDecimal,
                [typeof(string)] = ContentValueString,
                [typeof(bool)] = ContentValueBool,
                [typeof(DateTime?)] = ContentValueDate,
                [typeof(IContent)] = GetIContentFunc
            };

            GetHomePage = () => UmbracoContentExtend.GetContent(service)(E.Config[EnvironmentKey.HomeId], true);
            GetBreadCrumbById = id => UmbracoContentExtend.GetBreadCrumbFunction(id, b => GetIContentById(b, null));
            GetIContentByIds = (ids, alive) => UmbracoContentExtend.GetContentsByIds(service, ids, alive: alive);
            GetChildContentById = (rootId, where, alive) => UmbracoContentExtend.GetContentsByIds(service, rootId: rootId, alive: alive, where: where);
            ViewTempAndContent.Helper = E.GetHelper;
        }
    }
}