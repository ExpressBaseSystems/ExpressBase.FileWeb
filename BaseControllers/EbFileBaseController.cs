using ExpressBase.Common;
using ExpressBase.Common.ServiceClients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ExpressBase.Web.BaseControllers
{
    public class EbFileBaseController : Controller
    {
        protected EbStaticFileClient FileClient { get; set; }

        public EbFileBaseController(IEbStaticFileClient _sfc)
        {
            this.FileClient = _sfc as EbStaticFileClient;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            string sBToken = context.HttpContext.Request.Cookies[RoutingConstants.BEARER_TOKEN];
            string sRToken = context.HttpContext.Request.Cookies[RoutingConstants.REFRESH_TOKEN];

            this.FileClient.BearerToken = sBToken;
            this.FileClient.RefreshToken = sRToken;

        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("On ActionExecutedContext");

            if (!string.IsNullOrEmpty(this.FileClient.BearerToken))
            {
                Console.WriteLine("Bear Token Refreshed");
                Response.Cookies.Append(RoutingConstants.BEARER_TOKEN, this.FileClient.BearerToken, new CookieOptions());
            }

            base.OnActionExecuted(context);
        }
    }
}