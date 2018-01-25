using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Security;

namespace Spxus.Authorize
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AuthController : ApiController
    {
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        public HttpResponseMessage Post([FromBody]SpxusAuthorizeLogin input)
        {
            var isValuedUser = Membership.ValidateUser(G.Text(input.Name), G.Text(input.Password));
            if(!isValuedUser)
                return Request.CreateResponse(HttpStatusCode.NotAcceptable);
            var user = E.Services.MemberService.GetByUsername(input.Name);
            var token = user.Key.GetEncryptToken();
            var use = token.DecryptToken();
            return Request.CreateResponse(HttpStatusCode.OK, new { authorize = token });
        }
    }
}
