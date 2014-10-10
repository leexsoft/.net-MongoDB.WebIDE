using System;
using System.Web.Mvc;

namespace MongoDB.Persist.Web.Atrribute
{
    /// <summary>
    /// 异常JSON拦截器
    /// 因为异常拦截器为从里到外的执行顺序
    /// 需要和LogExceptionAttribute配合来记录异常日志
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class JsonExceptionAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {                                
                //返回异常JSON
                filterContext.Result = new JsonResult
                {
                    Data = new { Success = false, Message = filterContext.Exception.Message }
                };
            }
        }
    }
}
