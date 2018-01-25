using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace System
{
    public enum ContentTypeKey
    {
        Home,
        Page,
    }

    public static partial class E
    {
        public static bool ContentIsSameType(this IContent content, ContentTypeKey key)
        {
            if (content == null)
                return false;
            return content.ContentType.Alias.Equals(key.ToString(), StringComparison.InvariantCultureIgnoreCase);
        }
        public static bool ContentIsSameType(this IContent content, List<ContentTypeKey> keys)
        {
            return keys.Where(b => ContentIsSameType(content, b) == true).Count() > 0;
        }
        public static IContent GetIContentWithType(this ContentTypeKey key, int id)
        {
            var page = E.GetIContentById(id,false);
            if (page == null || !page.ContentIsSameType(key))
                return null;
            return page;
        }
        public static string ContentTypeViewPath(this ContentTypeKey type)
        {
            return $"~/Views/{type.ToString()}.cshtml";
        }
    }
}