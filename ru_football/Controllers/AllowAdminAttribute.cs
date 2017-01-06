using System;
using System.Configuration;
using System.Web.Mvc;

namespace ru_football.Controllers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AllowAdminAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (ConfigurationManager.AppSettings["AllowAdmin"] == "true")
                base.OnActionExecuting(filterContext);
            else
                filterContext.Result = new HttpStatusCodeResult(404);
        }
    }
}