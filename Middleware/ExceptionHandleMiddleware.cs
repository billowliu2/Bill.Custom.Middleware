using Bill.Custom.Middleware.Result;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Bill.Custom.Middleware.Middleware
{
    public  class ExceptionHandleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;


        public  ExceptionHandleMiddleware( RequestDelegate next,  ILogger<ExceptionHandleMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var error =  exception.ToString();

            _logger.LogError(error);

            return context.Response.WriteAsync(JsonConvert.SerializeObject(ResultModel.Failed(error), new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
        }

    }
}
