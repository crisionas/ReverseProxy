using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ReverseProxy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace ReverseProxy.Middlewares
{
    public class ProxyMiddleware
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly RequestDelegate _next;
        private readonly IServerStorage _storage;

        public ProxyMiddleware(RequestDelegate next,IServiceProvider service)
        {
            _storage = service.GetRequiredService<IServerStorage>();
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var targetUri = BuildTargetUri(context.Request);

            if(targetUri!=null)
            {
                var targetRequestMessage = CreateTargetMessage(context, targetUri);
                try
                {


                    using (var responseMessage = await _httpClient.SendAsync(targetRequestMessage, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted))
                    {
                        context.Response.StatusCode = (int)responseMessage.StatusCode;
                        CopyFromTargetResponse(context, responseMessage);
                        await responseMessage.Content.CopyToAsync(context.Response.Body);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            await _next(context);
        }

        private void CopyFromTargetResponse(HttpContext context, HttpResponseMessage responseMessage)
        {
            foreach(var header in responseMessage.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }

            foreach(var header in responseMessage.Content.Headers)
            {
                context.Response.Headers[header.Key] = header.Value.ToArray();
            }
            context.Response.Headers.Remove("transfer-encoding");
        }

        private HttpRequestMessage CreateTargetMessage(HttpContext context, Uri targetUri)
        {
            var message = new HttpRequestMessage()
            {
                Method = GetMethod(context.Request),
                RequestUri = targetUri,
                Content = new StreamContent(context.Request.Body)
            };
            
            foreach(var header in context.Request.Headers)
            {
                message.Content.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }

            return message;
        }

        private HttpMethod GetMethod(HttpRequest request)
        {
            var method = request.Method;
            return new HttpMethod(method);
        }

        private Uri BuildTargetUri(HttpRequest request)
        {
            var server = _storage.GetServer(request.Path);
            var uri = new Uri(server.Location + request.Path);
            return uri;
        }
    }
}
