#region
using Spxus.Calture.Service;
using Spxus.Core.Calture;
using Spxus.Core.Email;
using Spxus.Email;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using System.Linq;
using System.Web;
using Spxus.Umbraco.Extend.Models;
using Spxus.Core.SPXMember;
using Spxus.Umbraco.Member;
#endregion

namespace System
{
    public static partial class E
    {
        public static ServiceContext Services { get; set; }
        public static ServiceContext GetService()
        {
            return E.Services;
        }

        //将主页导出为JSON示例
        public static string HomePageString
        {
            get
            {
                return GetHomePage().ConvertToContentTypeView(false, false, false, E.CurrentUmbracoAccess).ConvertToJson();
            }
        }
        public static Func<int, string> PageString
        {
            get
            {
                return (id) =>
                {
                    return GetService().ContentService.GetById(id).ConvertToContentTypeView(false, true).ConvertToJson();
                };
            }
        }


        //当前用户是是否有权限
        public static Func<IEnumerable<string>, AllowUserAccess> CreatePublicAccessCheck
        {
            get
            {
                return UmbracoContentExtend.CheckPublicAccessFunction(E.Services.PublicAccessService, E.Services.ContentService);
            }
        }
        public static IEnumerable<string> CurrentUserUmbracoRoles
        {
            get
            {
                var umbracoRoles = E.Services.MemberGroupService.GetAll().Select(b => b.Name);
                var currentUser = HttpContext.Current.User;
                if (currentUser == null || HttpContext.Current.User.Identity.IsAuthenticated == false)
                {
                    return new List<string>();
                }
                return umbracoRoles.Where(b => currentUser.IsInRole(b));
            }
        }
        public static AllowUserAccess NoGroupUserAccess
        {
            get
            {
                return E.CreatePublicAccessCheck(new List<string>());
            }
        }
        public static AllowUserAccess CurrentUmbracoAccess
        {
            get
            {
                return E.CreatePublicAccessCheck(E.CurrentUserUmbracoRoles);
            }
        }

        //初始化
        public static void Init(int homeID, SpxCaltureType calture)
        {
            Services = ApplicationContext.Current.Services;
            UmbracoContentExtend.ThisDataTypeService = Services.DataTypeService;

            //默认的作为导航显示和显示子node的值 需要在content中加入 对应的boolproperty
            UmbracoContentExtend.IContentShowNav = "showNav";
            UmbracoContentExtend.IContentShowChildren = "showChildren";


            Config.Add(EnvironmentKey.HomeId, homeID);
            InitCacheSetting();
            InitContentGet(E.Services.ContentService, E.Services.ContentTypeService);
            InitContentCreate(E.Services.ContentService);
            InitContentPage(homeID);
            InitEmail(E.GetHomePage);
            InitMember();
            InitCalture((int)calture);
            InitMyContentGet(E.Services.ContentService,E.Services.ContentTypeService);
        }


        
    }


    public enum ContentPages
    {
        HomePage = 1,
    }
}
