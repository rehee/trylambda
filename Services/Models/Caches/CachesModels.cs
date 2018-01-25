using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.Caches
{
    public abstract class CacheBase
    {
        public virtual dynamic Content { get; set; }
        public DateTime Time { get; set; } = DateTime.Now;
        public bool KeepAlive { get; set; } = false;
        public virtual T GetContent<T>()
        {
            return Content;
        }
        public void SetAliveAndTime(bool? keepAlive = null, DateTime? time = null)
        {
            if (keepAlive != null)
                KeepAlive = (bool)KeepAlive;
            if (time != null)
                Time = (DateTime)time;
        }
        public virtual void Set(dynamic content, bool? keepAlive = null, DateTime? time = null)
        {
            SetAliveAndTime(keepAlive, time);
            this.Content = content;
            
        }
        public virtual T GetContentT<T>()
        {
            return Content;
        }
    }

    public static class Extends
    {
        public static bool IsExpire(this CacheBase model, int expireMin, DateTime? compire = null)
        {
            if (compire == null)
                compire = DateTime.Now;
            var result = ((DateTime)compire - model.Time).TotalMinutes > expireMin;
            return result;
        }

        public static bool IsExpire<T>(this IEnumerable<T> model, int expireMin, DateTime? compire = null) where T : CacheBase, new()
        {
            if (model == null || model.Count() <= 0)
                return true;
            return model.Where(b => b.IsExpire(expireMin, compire)).Count() > 0;
        }
        
    }
}
