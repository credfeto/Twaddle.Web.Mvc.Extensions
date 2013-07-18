// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheFilterAttribute.cs" company="Twaddle Software">
//   Copyright (c) Twaddle Software
// </copyright>
// <summary>
//   The cache filter attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Web;
using System.Web.Mvc;
using JetBrains.Annotations;

#endregion

namespace Twaddle.Web.Mvc.Extensions.Attributes
{
    /// <summary>
    /// The cache filter attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class CacheFilterAttribute : ActionFilterAttribute
    {
        #region Constants and Fields

        /// <summary>
        /// The duration in seconds.
        /// </summary>
        private int _duration;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheFilterAttribute"/> class.
        /// </summary>
        public CacheFilterAttribute()
        {
            _duration = 10;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the cache duration in seconds. The default is 10 seconds.
        /// </summary>
        /// <value>
        /// The cache duration in seconds.
        /// </value>
        public int Duration
        {
            get
            {
                Contract.Ensures(Contract.Result<int>() > 0);
                return _duration;
            }

            set
            {
                Contract.Requires(value > 0);
                _duration = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Called by the MVC framework after the action method executes.
        /// </summary>
        /// <param name="filterContext">
        /// The filter context.
        /// </param>
        public override void OnActionExecuted([NotNull] ActionExecutedContext filterContext)
        {
            Contract.Requires(filterContext != null);

            if (Duration <= 0)
            {
                return;
            }

            var cache = filterContext.HttpContext.Response.Cache;
            var cacheDuration = TimeSpan.FromSeconds(Duration);

            cache.SetCacheability(HttpCacheability.Public);
            cache.SetExpires(DateTime.Now.Add(cacheDuration));
            cache.SetMaxAge(cacheDuration);
            cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
        }

        #endregion

        #region Methods

        /// <summary>
        /// The object invariant.
        /// </summary>
        [ContractInvariantMethod]
        [UsedImplicitly]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Invoked by Code Contracts")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_duration > 0);
        }

        #endregion
    }
}