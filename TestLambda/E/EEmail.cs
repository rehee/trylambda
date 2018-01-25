using Spxus.Core.Email;
using Spxus.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace System
{
    public static partial class E
    {
        //email默认实现
        public static Func<IEmailServer> GetEmailService { get; set; }
        public static IEmailServer EmailService
        {
            get
            {
                return GetEmailService();
            }
        }

        public static void InitEmail(Func<IContent> getHome)
        {
            GetEmailService = () =>
            {
                IContent home = getHome();
                var email = home.IContentTo<SpxusUmbracoEmailKey>();
                IEmailServer es = new SPXEmailServer(email.EmailHost, email.EmailPort, email.EmailUser, email.EmailPassword, email.EmailSsl);
                return es;
            };
        }

    }

    public class SpxusUmbracoEmailKey : IIdentifyId
    {
        public int Id { get; set; }
        public string EmailHost { get; set; }
        public int EmailPort { get; set; }
        public string EmailUser { get; set; }
        public string EmailPassword { get; set; }
        public bool EmailSsl { get; set; }
    }
}