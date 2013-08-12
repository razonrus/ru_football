using System.Web.Mvc;
using System.Web.Routing;
using Domain;
using MvcExtensions;

namespace ru_football
{
    public class RegisterRoutes : RegisterRoutesBase
    {
        public RegisterRoutes(RouteCollection routes)
            : base(routes)
        {
        }

        protected override void Register()
        {
            Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            Routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            Routes.MapRoute("controller", "{controller}", new { action = "Index" });
            Routes.MapRoute("controller/action", "{controller}/{action}", new { controller = "Home", action = "Index" });
        }
    }
}