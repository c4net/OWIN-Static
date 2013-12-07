﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Ormikon.Owin.Static.Extensions;
using Ormikon.Owin.Static.Wrappers;

namespace Ormikon.Owin.Static
{
    internal abstract class StaticMiddlewareBase : OwinMiddleware
    {
        private readonly bool cached;
        private readonly ObjectCache cache;
        private readonly DateTimeOffset expires;
        private readonly int maxAge;

        protected StaticMiddlewareBase(Func<IDictionary<string, object>, Task> next)
            : this(next, false, null, DateTimeOffset.MinValue, 0)
        {
        }

        protected StaticMiddlewareBase(Func<IDictionary<string, object>, Task> next, bool cached)
            : this(next, cached, null, DateTimeOffset.MinValue, 0)
        {
        }

        protected StaticMiddlewareBase(Func<IDictionary<string, object>, Task> next, bool cached, ObjectCache cache)
            : this(next, cached, cache, DateTimeOffset.MinValue, 0)
        {
        }

        protected StaticMiddlewareBase(Func<IDictionary<string, object>, Task> next, bool cached, ObjectCache cache, DateTimeOffset expires, int maxAge)
            : base(next)
        {
            this.cached = cached;
            this.cache = cache;
            this.expires = expires;
            this.maxAge = maxAge;
        }

        protected override Task Invoke(IOwinContext context)
        {
            return IsMethodAllowed(context.Request.Method)
                    ? ProcessStaticIfFound(context) ?? Next(context)
                    : Next(context);
        }

        #region abstract methods

        protected abstract StaticResponse GetResponse(Location location);

        #endregion

        #region private methods

        private static Task SendStreamAsync(Stream from, Stream to)
        {
            return from.CopyToAsync(to)
                .ContinueWith(
                task =>
                {
                    from.Close();
                    task.Wait();
                });
        }

        private DateTimeOffset GetCacheOffset()
        {
            if (maxAge > 0)
                return DateTimeOffset.Now.AddSeconds(maxAge);
            if (expires != DateTimeOffset.MinValue && expires > DateTimeOffset.Now)
                return expires;
            return DateTimeOffset.MaxValue;// never expires
        }

        private CachedResponse CacheGet(Location location)
        {
            var c = cache ?? StaticSettings.DefaultCache;
            return c.Get(location.FullPath) as CachedResponse;
        }

        private void CacheSet(string path, CachedResponse data)
        {
            var c = cache ?? StaticSettings.DefaultCache;
            c.Set(path, data, GetCacheOffset());
        }

        private static void SetResponseHeaders(IResponseHeaders responseHeaders, IOwinResponse response)
        {
            response.StatusCode = responseHeaders.StatusCode;
            responseHeaders.Headers.CopyTo(response.Headers);
        }

        private static Task ProcessResponseStream(IResponseHeaders responseHeaders, Stream stream, IOwinContext ctx)
        {
            SetResponseHeaders(responseHeaders, ctx.Response);
            if (IsBodyRequested(ctx.Request.Method))
                return SendStreamAsync(stream, ctx.Response.Body);
            stream.Close();
            return Task.FromResult<object>(null);
        }

        private static Task SendResponse(CachedResponse cachedResponse, IOwinContext ctx)
        {
            return ProcessResponseStream(cachedResponse, cachedResponse.CreateBodyStream(), ctx);
        }

        private static Task SendResponse(StaticResponse staticResponse, IOwinContext ctx)
        {
            return ProcessResponseStream(staticResponse, staticResponse.Body, ctx);
        }

        private Task CacheResponseAndSend(StaticResponse staticResponse, IOwinContext ctx)
        {
            if (cached)
            {
                return CachedResponse.CreateAsync(staticResponse)
                        .ContinueWith(
                            task =>
                            {
                                task.Wait();
                                CacheSet(ctx.Request.Location.FullPath, task.Result);
                                SendResponse(task.Result, ctx).Wait();
                            });
            }
            return SendResponse(staticResponse, ctx);
        }

        private Task ProcessStaticIfFound(IOwinContext ctx)
        {
            var location = ctx.Request.Location;
            if (cached)
            {
                var response = CacheGet(location);
                if (response != null)
                    return SendResponse(response, ctx);
            }

            var staticResponse = GetResponse(location);
            return staticResponse == null ? null : CacheResponseAndSend(staticResponse, ctx);
        }

        private static bool IsMethodAllowed(string method)
        {
            return string.Compare(method, Constants.Http.Methods.Get, StringComparison.OrdinalIgnoreCase) == 0
                   || string.Compare(method, Constants.Http.Methods.Head, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private static bool IsBodyRequested(string method)
        {
            return string.Compare(method, Constants.Http.Methods.Get, StringComparison.OrdinalIgnoreCase) == 0;
        }

        #endregion
    }
}
