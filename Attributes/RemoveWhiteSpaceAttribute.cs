// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoveWhiteSpaceAttribute.cs" company="Twaddle Software">
//   Copyright (c) Twaddle Software
// </copyright>
// <summary>
//   The remove whitespaces attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Using Directives

using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using JetBrains.Annotations;

#endregion

namespace Twaddle.Web.Mvc.Extensions.Attributes
{
    /// <summary>
    ///     The remove whitespaces attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class RemoveWhiteSpaceAttribute : ActionFilterAttribute
    {
        #region Public Methods

        /// <summary>
        ///     Called by the MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting([NotNull] ActionExecutingContext filterContext)
        {
            Contract.Requires(filterContext != null);

            var response = filterContext.HttpContext.Response;

            if (response.ContentType == "text/html" && response.Filter != null)
            {
                response.Filter = new WhitespaceRemovalStream(response.Filter);
            }
        }

        #endregion

        /// <summary>
        ///     The helper class.
        /// </summary>
        private sealed class WhitespaceRemovalStream : Stream
        {
            #region Constants and Fields

            /// <summary>
            ///     The base stream.
            /// </summary>
            [NotNull]
            private readonly Stream _baseStream;

            [NotNull]
            private readonly StringBuilder _buffer = new StringBuilder();

            /// <summary>
            ///     Whitespace removal regular expresion.
            /// </summary>
            /// <remarks>
            ///     See: http://stackoverflow.com/questions/8762993/remove-white-space-from-entire-html-but-inside-pre-with-regular-expressions for more details.
            /// </remarks>
            [NotNull]
            private readonly Regex _whitespaceRemoval =
                new Regex(
                    @"(?<=[^])\t{2,}|(?<=[>])\s{2,}(?=[<])|(?<=[>])\s{2,11}(?=[<])|(?=[\n])\s{2,}",
                    RegexOptions.Compiled);

            #endregion

            #region Constructors and Destructors

            /// <summary>
            ///     Initializes a new instance of the <see cref="WhitespaceRemovalStream" /> class.
            /// </summary>
            /// <param name="responseStream">
            ///     The response stream.
            /// </param>
            public WhitespaceRemovalStream([NotNull] Stream responseStream)
            {
                Contract.Requires(responseStream != null);

                _baseStream = responseStream;
            }

            #endregion

            #region Properties

            /// <summary>
            ///     When overridden in a derived class, gets a value indicating whether the current stream supports reading.
            /// </summary>
            /// <returns>
            ///     True if the stream supports reading; otherwise, false.
            /// </returns>
            /// <value>
            ///     The can read.
            /// </value>
            public override bool CanRead
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///     When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
            /// </summary>
            /// <returns>
            ///     True if the stream supports seeking; otherwise, false.
            /// </returns>
            /// <value>
            ///     The can seek.
            /// </value>
            public override bool CanSeek
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///     When overridden in a derived class, gets a value indicating whether the current stream supports writing.
            /// </summary>
            /// <returns>
            ///     True if the stream supports writing; otherwise, false.
            /// </returns>
            /// <value>
            ///     The can write.
            /// </value>
            public override bool CanWrite
            {
                get
                {
                    return true;
                }
            }

            /// <summary>
            ///     When overridden in a derived class, gets the length in bytes of the stream.
            /// </summary>
            /// <returns>
            ///     A long value representing the length of the stream in bytes.
            /// </returns>
            /// <exception cref="System.NotSupportedException">
            ///     A class derived from Stream does not support seeking.
            /// </exception>
            /// <exception cref="System.ObjectDisposedException">
            ///     Methods were called after the stream was closed.
            /// </exception>
            /// <value>
            ///     The length.
            /// </value>
            public override long Length
            {
                get
                {
                    throw new NotSupportedException();
                }
            }

            /// <summary>
            ///     When overridden in a derived class, gets or sets the position within the current stream.
            /// </summary>
            /// <returns>
            ///     The current position within the stream.
            /// </returns>
            /// <exception cref="System.IO.IOException">
            ///     An I/O error occurs.
            /// </exception>
            /// <exception cref="System.NotSupportedException">
            ///     The stream does not support seeking.
            /// </exception>
            /// <exception cref="System.ObjectDisposedException">
            ///     Methods were called after the stream was closed.
            /// </exception>
            /// <value>
            ///     The position.
            /// </value>
            public override long Position
            {
                get
                {
                    throw new NotSupportedException();
                }

                set
                {
                    throw new NotSupportedException();
                }
            }

            #endregion

            #region Public Methods

            /// <summary>
            ///     When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
            /// </summary>
            /// <exception cref="System.IO.IOException">
            ///     An I/O error occurs.
            /// </exception>
            public override void Flush()
            {
                OutputBuffer();
                _baseStream.Flush();
            }

            /// <summary>
            ///     When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
            /// </summary>
            /// <param name="buffer">
            ///     An array of bytes. When this method returns, the buffer contains the specified byte array with the values between
            ///     <paramref
            ///         name="offset" />
            ///     and
            ///     (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.
            /// </param>
            /// <param name="offset">
            ///     The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.
            /// </param>
            /// <param name="count">
            ///     The maximum number of bytes to be read from the current stream.
            /// </param>
            /// <returns>
            ///     The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available,
            ///     or zero (0) if the end of the stream has been reached.
            /// </returns>
            /// <exception cref="System.ArgumentException">
            ///     The sum of <paramref name="offset" /> and <paramref name="count" /> is larger than the buffer length.
            /// </exception>
            /// <exception cref="System.ArgumentNullException">
            ///     <paramref name="buffer" /> is null.
            /// </exception>
            /// <exception cref="System.ArgumentOutOfRangeException">
            ///     <paramref name="offset" /> or <paramref name="count" /> is negative.
            /// </exception>
            /// <exception cref="System.IO.IOException">
            ///     An I/O error occurs.
            /// </exception>
            /// <exception cref="System.NotSupportedException">
            ///     The stream does not support reading.
            /// </exception>
            /// <exception cref="System.ObjectDisposedException">
            ///     Methods were called after the stream was closed.
            /// </exception>
            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///     When overridden in a derived class, sets the position within the current stream.
            /// </summary>
            /// <param name="offset">
            ///     A byte offset relative to the <paramref name="origin" /> parameter.
            /// </param>
            /// <param name="origin">
            ///     A value of type <see cref="System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.
            /// </param>
            /// <returns>
            ///     The new position within the current stream.
            /// </returns>
            /// <exception cref="System.IO.IOException">
            ///     An I/O error occurs.
            /// </exception>
            /// <exception cref="System.NotSupportedException">
            ///     The stream does not support seeking, such as if the stream is constructed from a pipe or console output.
            /// </exception>
            /// <exception cref="System.ObjectDisposedException">
            ///     Methods were called after the stream was closed.
            /// </exception>
            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///     When overridden in a derived class, sets the length of the current stream.
            /// </summary>
            /// <param name="value">
            ///     The desired length of the current stream in bytes.
            /// </param>
            /// <exception cref="System.IO.IOException">
            ///     An I/O error occurs.
            /// </exception>
            /// <exception cref="System.NotSupportedException">
            ///     The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output.
            /// </exception>
            /// <exception cref="System.ObjectDisposedException">
            ///     Methods were called after the stream was closed.
            /// </exception>
            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            ///     When overridden in a derived class, writes a sequence of bytes to the current stream and advances the current
            ///     position within this stream by the number of bytes written.
            /// </summary>
            /// <param name="buffer">
            ///     An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream.
            /// </param>
            /// <param name="offset">
            ///     The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.
            /// </param>
            /// <param name="count">
            ///     The number of bytes to be written to the current stream.
            /// </param>
            /// <exception cref="System.ArgumentException">
            ///     The sum of <paramref name="offset" /> and <paramref name="count" /> is greater than the buffer length.
            /// </exception>
            /// <exception cref="System.ArgumentNullException">
            ///     <paramref name="buffer" /> is null.
            /// </exception>
            /// <exception cref="System.ArgumentOutOfRangeException">
            ///     <paramref name="offset" /> or <paramref name="count" /> is negative.
            /// </exception>
            /// <exception cref="System.IO.IOException">
            ///     An I/O error occurs.
            /// </exception>
            /// <exception cref="System.NotSupportedException">
            ///     The stream does not support writing.
            /// </exception>
            /// <exception cref="System.ObjectDisposedException">
            ///     Methods were called after the stream was closed.
            /// </exception>
            public override void Write(byte[] buffer, int offset, int count)
            {
                var rawHtml = Encoding.UTF8.GetString(buffer, offset, count);
                _buffer.Append(rawHtml);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    OutputBuffer();
                }

                base.Dispose(disposing);
            }

            #endregion

            #region Methods

            private void OutputBuffer()
            {
                RemoveWhiteSpace(_buffer.ToString());
                _buffer.Clear();
            }

            /// <summary>
            ///     Removes the white space.
            /// </summary>
            /// <param name="rawHtml">The raw HTML.</param>
            private void RemoveWhiteSpace([CanBeNull] string rawHtml)
            {
                if (string.IsNullOrWhiteSpace(rawHtml))
                {
                    return;
                }

                const string html5DocumentType = "<!DOCTYPE html>";

                var cleaned = _whitespaceRemoval.Replace(rawHtml, string.Empty).Trim();
                if (cleaned.StartsWith(html5DocumentType, StringComparison.Ordinal))
                {
                    cleaned = cleaned.Insert(html5DocumentType.Length, Environment.NewLine);
                }

                cleaned += Environment.NewLine;

                var encoded = Encoding.UTF8.GetBytes(cleaned);
                _baseStream.Write(encoded, 0, encoded.Length);
            }

            /// <summary>
            ///     The object invariant.
            /// </summary>
            [ContractInvariantMethod]
            [UsedImplicitly]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
                Justification = "Invoked by Code Contracts")]
            private new void ObjectInvariant()
            {
                Contract.Invariant(_baseStream != null);
                Contract.Invariant(_whitespaceRemoval != null);
                Contract.Invariant(_buffer != null);
            }

            #endregion
        }
    }
}