// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplicitlyNotCachedFilterAttribute.cs" company="Twaddle Software">
//   Copyright (c) Twaddle Software
// </copyright>
// <summary>
//   Marks a response as explicitly not being cached.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives

using System;
using System.Diagnostics.Contracts;
using System.Web.Mvc;
using JetBrains.Annotations;

#endregion

namespace Twaddle.Web.Mvc.Extensions.Attributes
{
    /// <summary>
    ///     Marks a response as explicitly not being cached.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ExplicitlyNotCachedFilterAttribute : ActionFilterAttribute
    {
        #region Public Methods

        /// <summary>
        ///     Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting([NotNull] ActionExecutingContext filterContext)
        {
            Contract.Requires(filterContext != null);

            var response = filterContext.HttpContext.Response;
            response.Clear();
            response.Expires = -1;
            response.AddHeader("Cache-Control", "no-cache, no-store, max-age=0, must-revalidate");
            response.AddHeader("Pragma", "no-cache");

            base.OnActionExecuting(filterContext);
        }

        #endregion
    }
}