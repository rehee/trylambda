using Newtonsoft.Json.Serialization;
using Spxus.Authorize;
using Spxus.Core.Calture;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Models;

namespace Umbraco.Web
{
    public class CustomUmbracoApplication : UmbracoApplication
    {
        protected override IBootManager GetBootManager()
        {
            return new CustomWebBootManager(this);
        }

        public class CustomWebBootManager : WebBootManager
        {
            public CustomWebBootManager(UmbracoApplicationBase umbracoApplication) : base(umbracoApplication)
            {
            }

            public override IBootManager Complete(Action<ApplicationContext> afterComplete)
            {
                var rootPath = HostingEnvironment.ApplicationPhysicalPath;
                var config = System.IO.File.OpenRead($@"{rootPath}\Web.config");
                G.SetSettingStream(config);
                config.Close();
                var rootid = G.AppSettings("homeID");
                E.Init(
                    rootid.Int32(),
                    SpxCaltureType.Chinese
                );
                GlobalConfiguration.Configure(con =>
                {
                    con.EnableCors();
                    var jsonFormat = con.Formatters.OfType<JsonMediaTypeFormatter>().FirstOrDefault();
                    jsonFormat.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
                AreaRegistration.RegisterAllAreas();
                GlobalConfiguration.Configure(ApiAuth.Register);
                Umbraco.Core.Services.ContentService.Saved += ContentService_Saved;
                Umbraco.Core.Services.ContentService.Deleted += ContentService_Deleted;
                return base.Complete(afterComplete);
            }

            private void ContentService_Deleted(IContentService sender, Core.Events.DeleteEventArgs<IContent> e)
            {
                e.DeletedEntities.ToList().ForEach(b => E.PageCache.Remove(b.Id));
            }

            private void ContentService_Saved(Core.Services.IContentService sender, Core.Events.SaveEventArgs<Core.Models.IContent> e)
            {
                var user = HttpContext.Current.User;
                e.SavedEntities.ToList().ForEach(b => UmbracoContentExtend.UpdateICache(b, null, E.PageCache));
                return;
            }
        }
    }
}
