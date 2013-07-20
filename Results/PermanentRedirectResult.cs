// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PermanentRedirectResult.cs" company="Twaddle Software">
//   Copyright (c) Twaddle Software
// </copyright>
// <summary>
//   The permanent redirect result.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives

using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Web.Mvc;
using JetBrains.Annotations;

#endregion

namespace Twaddle.Web.Mvc.Extensions.Results
{
    /// <summary>
    ///     The permanent redirect result.
    /// </summary>
    public sealed class PermanentRedirectResult : ActionResult
    {
        #region Constants and Fields

        /// <summary>
        ///     The URL to redirect to.
        /// </summary>
        [NotNull]
        private readonly string _url;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PermanentRedirectResult" /> class.
        /// </summary>
        /// <param name="url">
        ///     The URL to redirect to.
        /// </param>
        public PermanentRedirectResult([NotNull] string url)
        {
            Contract.Requires(!string.IsNullOrEmpty(url));

            _url = url;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the Url to redirect to.
        /// </summary>
        /// <value>
        ///     The URL to redirect to.
        /// </value>
        [NotNull]
        public string Url
        {
            get
            {
                Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));

                return _url;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     Enables processing of the result of an action method by a custom type that inherits from the
        ///     <see
        ///         cref="T:System.Web.Mvc.ActionResult" />
        ///     class.
        /// </summary>
        /// <param name="context">
        ///     The context in which the result is executed. The context information includes the controller, HTTP content, request context, and route data.
        /// </param>
        public override void ExecuteResult([NotNull] ControllerContext context)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = 301;
            response.RedirectLocation = Url;
            response.End();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The object invariant.
        /// </summary>
        [ContractInvariantMethod]
        [UsedImplicitly]
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Invoked by Code Contracts")]
        private void ObjectInvariant()
        {
            Contract.Invariant(!string.IsNullOrEmpty(_url));
        }

        #endregion
    }
}