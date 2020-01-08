using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.ServiceClients;
using ExpressBase.Common.ServiceStack.Auth;
using ExpressBase.Objects.ServiceStack_Artifacts;
using ExpressBase.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceStack;
using ServiceStack.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

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
    }
}