using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace System
{
    public static partial class E
    {
        //内容添加 父node为首页,名字随机.无内容
        public static CreateContent CreateNodeRandonName { get; set; }
        //内容添加 选择父nodeId,名字随机.无内容
        public static CreateContentForRoot CreateNodeIdRandonName { get; set; }
        //内容添加 父node为首页,名字指定.无内容
        public static CreateContentWithName CreateNodeWithName { get; set; }
        //内容添加 选择父nodeId,名字指定.无内容
        public static CreateContentForRootWithName CreateNodeIdWithName { get; set; }


        public static void InitContentCreate(IContentService service)
        {
            //"page" 为contenttype的alise Name. 这些方法需要提前设置.
            CreateNodeRandonName = UmbracoContentExtend.CreateContentFunction(
                    service, E.Config[EnvironmentKey.HomeId], "page", EmptyNodeCreate);
            CreateNodeIdRandonName = UmbracoContentExtend.CreateContentForRootFunction(
                    service, "page", EmptyNodeCreate);
            CreateNodeWithName = UmbracoContentExtend.CreateContentWithNameFunction(
                service, E.Config[EnvironmentKey.HomeId], "page", EmptyNodeCreate);
            CreateNodeIdWithName = UmbracoContentExtend.CreateContentForRootWithNameFunction(
                    service, "page", EmptyNodeCreate);
        }


        //填充空的icontent
        public static SetModelInIContent EmptyNodeCreate =
        (dynamic inputC, IContent content) =>
        {
            return content;
        };
    }
}