using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Twaddle.Web.Mvc.Extensions.Attributes
{
    /// <summary>
    ///     The HTTP Strict Transport Security filter attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HttpStrictTransportSecurityFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="HttpStrictTransportSecurityFilterAttribute" /> class.
        /// </summary>
        public HttpStrictTransportSecurityFilterAttribute()
        {
            MaxAge = TimeSpan.FromDays(365);
            IncludeSubDomains = false;
        }

        /// <summary>
        ///     The maximum age for the STS setting to be valid for.
        /// </summary>
        public TimeSpan MaxAge { get; set; }

        /// <summary>
        ///     Whether subdomains should also go over SSL.
        /// </summary>
        public bool IncludeSubDomains { get; set; }

        /// <summary>
        ///     Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;
            HttpResponseBase response = filterContext.HttpContext.Response;

            if (request.IsSecureConnection ||
                StringComparer.InvariantCultureIgnoreCase.Equals(request.Url.Scheme, "https"))
            {
                var seconds = (long) MaxAge.TotalSeconds;
                if (seconds > 0)
                {
                    var value = new StringBuilder();
                    value.AppendFormat("max-age={0}", seconds);

                    if (IncludeSubDomains)
                    {
                        value.Append("; includeSubDomains");
                    }

                    response.AddHeader("Strict-Transport-Security", value.ToString());
                }
            }


            base.OnActionExecuting(filterContext);
        }
    }
}