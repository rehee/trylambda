using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Net.Http;
using Newtonsoft.Json;
using Spxus.Authorize;
using System.Web.Security;

namespace System
{
    public class SXPApiAuth : AuthorizationFilterAttribute
    {
        public string Roles { get; set; }
        public int PageId { get; set; } = 0;
        public int ExpireMins { get; set; } = 60 * 24 * 31;
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var c = actionContext.Request;
            var str = c.Content.ReadAsStringAsync().Result;
            IEnumerable<string> authorize;
            actionContext.Request.Headers.TryGetValues("authorize", out authorize);
            if (authorize == null)
                goto Error;
            var token = authorize.FirstOrDefault().DecryptToken();
            if (token == null || token.UserKey == null || token.TimeStamp == null)
                goto Error;
            if ((DateTime.UtcNow - (DateTime)token.TimeStamp).TotalMinutes > ExpireMins)
                goto Error;
            var user = E.Services.MemberService.GetByKey((Guid)token.UserKey);
            if (user == null)
                goto Error;
            if (Roles == null || Roles == "")
                return;
            var roles = Roles.Split(',').Select(b => b.Text().ToLower()).ToList();
            var userRoles = E.Services.MemberService.GetAllRoles(user.Id);
            bool userHasRole = false;
            foreach (var r in userRoles)
            {
                if (roles.IndexOf(r.Text().ToLower()) > 0)
                {
                    userHasRole = true;
                    break;
                }
            }
            bool userHasAccess = false;
            if (PageId <= 0)
            {
                userHasAccess = true;
            }
            else
            {
                userHasAccess = E.CreatePublicAccessCheck(userRoles)(PageId);
            }
            if(userHasRole&& userHasAccess)
            {
                return;
            }
            Error:
            HandleUnauthorized(actionContext);
        }

        private void HandleUnauthorized(HttpActionContext actionContext)
        {
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", "");
        }
    }
}