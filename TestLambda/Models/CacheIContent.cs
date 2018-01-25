using Services.Models.Caches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;

namespace TestLambda.Models
{
    
    public class CacheIContent: CacheBase
    {
        public new IContent Content { get; set; }
        public override T GetContent<T>()
        {
            return (T)Content;
        }
        public override void Set(dynamic content, bool? keepAlive = null, DateTime? time = null)
        {
            SetAliveAndTime(keepAlive, time);
            this.Content = content;
        }
    }
}