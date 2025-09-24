using ExpressBase.Common;
using ExpressBase.Common.Constants;
using ExpressBase.Common.LocationNSolution;
using ExpressBase.Common.ServiceClients;
using ExpressBase.Objects.ServiceStack_Artifacts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using ServiceStack.Redis;
using System;

namespace ExpressBase.Web.BaseControllers
{
    public class EbFileBaseController : Controller
    {
        protected EbStaticFileClient FileClient { get; set; }

        protected EbStaticFileClient2 FileClient2 { get; set; }

        protected RedisClient Redis { get; set; }

        protected PooledRedisClientManager PooledRedisManager { get; set; }

        public string Host { get; set; }

        public string ExtSolutionId { get; set; }

        public string IntSolutionId { get; set; }

        public EbFileBaseController(IEbStaticFileClient _sfc, EbStaticFileClient2 _sfc2, IRedisClient _redis, PooledRedisClientManager pooledRedisManager)
        {
            this.FileClient = _sfc as EbStaticFileClient;
            this.FileClient2 = _sfc2;
            this.Redis = _redis as RedisClient;
            this.PooledRedisManager = pooledRedisManager;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            Host = context.HttpContext.Request.Host.Host.Replace(RoutingConstants.WWWDOT, string.Empty).Replace(RoutingConstants.LIVEHOSTADDRESS, string.Empty).Replace(RoutingConstants.STAGEHOSTADDRESS, string.Empty).Replace(RoutingConstants.LOCALHOSTADDRESS, string.Empty);

            ExtSolutionId = Host.Replace(RoutingConstants.DASHDEV, string.Empty);
            IntSolutionId = this.GetIsolutionId(ExtSolutionId);

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

        public string GetIsolutionId(string esid)
        {
            string solnId = string.Empty;

            if (esid == CoreConstants.MYACCOUNT)
                solnId = CoreConstants.EXPRESSBASE;
            else if (esid == CoreConstants.ADMIN)
                solnId = CoreConstants.ADMIN;
            else if (this.Redis != null)
            {
                if (this.PooledRedisManager != null)
                {
                    using (var redis = this.PooledRedisManager.GetReadOnlyClient())
                    {
                        solnId = redis.Get<string>(string.Format(CoreConstants.SOLUTION_ID_MAP, esid));
                    }
                }
                else
                    solnId = this.Redis.Get<string>(string.Format(CoreConstants.SOLUTION_ID_MAP, esid));
            }
            return solnId;
        }

        public Eb_Solution GetSolutionObject(string cid)
        {
            Eb_Solution s_obj = null;
            try
            {
                if (this.PooledRedisManager != null)
                {
                    using (var redis = this.PooledRedisManager.GetReadOnlyClient())
                        s_obj = redis.Get<Eb_Solution>(string.Format("solution_{0}", cid));
                }
                else
                    s_obj = this.Redis.Get<Eb_Solution>(String.Format("solution_{0}", cid));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
            }
            return s_obj;
        }
    }
}