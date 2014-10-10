using System;
using System.Web.Mvc;
using MongoDB.Component;

namespace MongoDB.Persist.Web.Atrribute
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CacheValidAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (MongoCache.CacheValid())
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectResult("/Home/CacheExpire/");
            }
        }
    }
}
