using Spxus.Umbraco.Extend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace System
{
    public enum PageKey
    {
        HomePage,
        PageRoot,
    }

    public static partial class E
    {
        public static Dictionary<PageKey, ContentTypeKey> DefailtPageContentTypeMap { get; set; } = new Dictionary<PageKey, ContentTypeKey>()
        {
            [PageKey.HomePage] = ContentTypeKey.Home,
            [PageKey.PageRoot] = ContentTypeKey.Page,
        };
        public static Dictionary<PageKey, int> DefaltPageIdMap { get; set; } = new Dictionary<PageKey, int>()
        {
            [PageKey.PageRoot] = 1065,
        };
        public static void InitContentPage(int homeId)
        {
            DefaltPageIdMap.Add(PageKey.HomePage, homeId);
        }
        public static ViewTempAndContent GetViewTempAndContent(this PageKey input)
        {
            try
            {
                return new ViewTempAndContent(DefailtPageContentTypeMap[input].ContentTypeViewPath(), DefaltPageIdMap[input]);
            }
            catch
            {
                return null;
            }

        }
    }
}